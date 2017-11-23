using System;
using System.Collections.Generic;
using UnityEngine;
using Frankfort.Threading;



public class FlockData
{
    public Vector3 position;
    public Vector3 currentDirection;
    public Vector3 targetDirection;
    public Color targetColor;
    public Color currentColor;
    public Vector3 destinationPoint;
    public float targetVelocity;
    public float currentVelocity;
}


public struct EnvironmentCollider
{
    public Vector3 position;
    public float radius;
    public float radiusSqr;

    public EnvironmentCollider(Vector3 position, float radius)
    {
        this.position = position;
        this.radius = radius;
        this.radiusSqr = radius * radius;
    }
}


public class FlockingDataWorker : IThreadWorkerObject
{

    //--------------- Flock Frame/Cycledata --------------------
    public FlockData[] AllFlocks;
    public EnvironmentCollider[] AllColliders;
    public int startWorkIndex;
    public int endWorkIndex;
    public int maxRandomSiblingsTested;

    public Vector3 universeCenter;
    public float maxBoundsRadius;

    public float seperationWeight;
    public float alignmentWeight;
    public float cohesionWeight;

    private float _seperationRadius;
    private float _seperationRadiusSqr;
    private float _alignmentRadius;
    private float _alignmentRadiusSqr;
    private float _cohesionRadius;
    private float _cohesionRadiusSqr;
    private float _destinationAttractRadius;
    private float _destinationAttractRadiusSqr;
    public float destinationReachedRadius;


    public float seperationRadius
    {
        get { return _seperationRadius; }
        set { _seperationRadius = value; _seperationRadiusSqr = value * value; }
    }
    public float alignmentRadius
    {
        get { return _alignmentRadius; }
        set { _alignmentRadius = value; _alignmentRadiusSqr = value * value; }
    }

    public float cohesionRadius
    {
        get { return _cohesionRadius; }
        set { _cohesionRadius = value; _cohesionRadiusSqr = value * value; }
    }
    public float destinationAttractRadius
    {
        get { return _destinationAttractRadius; }
        set { _destinationAttractRadius = value; _destinationAttractRadiusSqr = value * value; }
    }


    public float seperationRadiusSqr
    {
        get { return _seperationRadiusSqr; }
    }
    public float alignmentRadiusSqr
    {
        get { return _alignmentRadiusSqr; }
    }
    public float cohesionRadiusSqr
    {
        get { return _cohesionRadiusSqr; }
    }
    public float destinationAttractRadiusSqr
    {
        get { return _destinationAttractRadiusSqr; }
    }
    //--------------- Flock Frame/Cycledata --------------------




    //--------------- IThreadWorker Implementation --------------------
    private bool isAborted = false;

    public void AbortThreadedWork()
    {
        isAborted = true;
    }
    //--------------- IThreadWorker Implementation --------------------




    public void ExecuteThreadedWork()
    {
        startWorkIndex = Mathf.Clamp(startWorkIndex, 0, AllFlocks.Length);
        endWorkIndex = Mathf.Clamp(endWorkIndex, 0, AllFlocks.Length);

        //--------------- Creat list with indexes --------------------
        int[] randomIndexes = new int[AllFlocks.Length];
        int i = AllFlocks.Length;
        while (--i > -1)
            randomIndexes[i] = i;

        //--------------- Creat list with indexes --------------------


        for (i = startWorkIndex; i < endWorkIndex && !isAborted; i++)
        {
            //--------------- Shuffle indexes every-something-flocks --------------------
            if ((float)i % (float)maxRandomSiblingsTested == 0f)
                randomIndexes.Shuffle();
            //--------------- Shuffle indexes every-something-flocks --------------------

            FlockData flock = AllFlocks[i];

            if (flock.position.y < 0f) //Through the ground....
            {
                flock.position.y = -flock.position.y;
                flock.targetDirection.y = Mathf.Abs(flock.targetDirection.y);
                flock.currentDirection = flock.targetDirection;
                flock.currentColor = flock.targetColor = new Color(1f, 1f, 0f, 0.65f);
                flock.currentVelocity = flock.targetVelocity;
            }
            else
            {
                Vector3 toCenterVec = universeCenter - flock.position;
                if (toCenterVec.sqrMagnitude > maxBoundsRadius * maxBoundsRadius) //If outside of univere limits
                {
                    flock.targetDirection = toCenterVec.normalized;
                    flock.targetColor = new Color(1f, 0f, 1f, 0.25f);
                    flock.currentVelocity = flock.targetVelocity;
                }
                else
                {
                    //--------------- Check for Collisions --------------------
                    bool isColliding = false;
                    int j = AllColliders.Length;

                    Vector3 collisionAvoidenceDir = Vector3.zero;
                    while (--j > -1 && !isAborted)
                    {
                        EnvironmentCollider coll = AllColliders[j];
                        Vector3 diffVec = coll.position - flock.position;

                        if (diffVec.sqrMagnitude < coll.radiusSqr)
                        {
                            isColliding = true;
                            Vector3 invDiff = -diffVec.normalized;
                            Vector3 reflectionDir = FlockHelper.ReflectVector(invDiff, flock.currentDirection);
                            collisionAvoidenceDir += reflectionDir;
                            flock.position = coll.position + (invDiff * coll.radius);
                        }
                    }
                    //--------------- Check for Collisions --------------------

                    if (isColliding) //If Colliding
                    {
                        flock.targetDirection = flock.currentDirection = collisionAvoidenceDir.normalized;
                        flock.currentColor = flock.targetColor = new Color(1f, 0.1f, 0f, 0.85f);
                        flock.currentVelocity = flock.targetVelocity * 2f;
                    }
                    else
                    {
                        Vector3 diffDestionation = flock.destinationPoint - flock.position;
                        if (diffDestionation.sqrMagnitude < destinationAttractRadiusSqr) //If close to unique target point...
                        {
                            flock.targetDirection = diffDestionation.normalized;
                            float lerpT = Mathf.Clamp01(diffDestionation.magnitude / destinationReachedRadius);
                            flock.currentVelocity = flock.targetVelocity * lerpT;
                            flock.targetColor = Color.Lerp(new Color(0f, 1f, 0f, 1f), new Color(1f, 1f, 1f, 0.15f), lerpT);
                        }
                        else
                        {
                            Vector3 combinedDirection = flock.targetDirection;
                            Vector3 combinedColor = Vector3.zero;

                            j = Mathf.Min(AllFlocks.Length, maxRandomSiblingsTested);
                            while (--j > -1 && !isAborted)
                            {

                                //--------------- Get a random sibling --------------------
                                int randomIndex = randomIndexes[j];
                                if (i == randomIndex)
                                    continue;
                                FlockData sibling = AllFlocks[randomIndex];
                                //--------------- Get a random sibling --------------------


                                Vector3 diffVec = sibling.position - flock.position;
                                float distSqr = diffVec.sqrMagnitude;

                                if (distSqr < seperationRadiusSqr)
                                {
                                    combinedDirection += Vector3.Cross(flock.currentDirection, diffVec.normalized) * seperationWeight;
                                    combinedColor.y += seperationWeight; //y == green;
                                }
                                //else if (distSqr < alignmentRadiusSqr)
                                if (distSqr < alignmentRadiusSqr)
                                {
                                    combinedDirection += sibling.currentDirection * alignmentWeight;
                                    combinedColor.z += alignmentWeight; //z == blue;
                                }
                                //else if (distSqr < cohesionRadiusSqr)
                                if (distSqr < cohesionRadiusSqr)
                                {
                                    combinedDirection += diffVec.normalized * cohesionWeight;
                                    combinedColor += new Vector3(cohesionWeight, cohesionWeight, cohesionWeight); //Add white
                                }
                            }

                            flock.targetDirection = combinedDirection.normalized;
                            flock.currentVelocity = flock.targetVelocity;
                            combinedColor.Normalize();
                            flock.targetColor = new Color(combinedColor.x, combinedColor.y, combinedColor.z, 0.15f);
                        }
                    }
                }
            }
        }
    }

}