using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Frankfort.Threading;


public class MassiveFlockingExample : MonoBehaviour 
{
    public bool FlockingDebugMode = false;
    public int TargetFrameRate = 30;
    public float ParticleSize = 3f;
    public float ParticleColorLerpSpeed = 3f;
    public int FlockingSpawnCount = 10000;
    public int FlockingMaxRandomSiblingsTested = 500;

    public float FlockingMaxSpawnRadius = 50f;
    public float FlockingStartVelocityMin = 15f;
    public float FlockingStartVelocityMax = 20f;
    public float FlockingSteeringSpeed = 0.5f;

    public float FlockingSeperationWeight = 1f;
    public float FlockingAlignmentWeight = 1f;
    public float FlockingCohesionWeight = 1f;

    public float FlockingSeperationRadius = 10;
    public float FlockingAlignmentRadius = 30;
    public float FlockingCohesionRadius = 75f;

    public float FlockingDestinationAttractRadius = 0f;
    public float FlockingDestinationReachedRadius = 0f;
    public float FlockingBoundsRadius = 100f;


    public bool MultithreadingEnabled = false;
    public int ThreadingMaxThreads = -1;
    public int ThreadingPackagesPerThread = 4;
    
    private FlockData[] flockers;
    private EnvironmentCollider[] colliders;
    private Vector3[] destinationPoints;

    private ParticleSystem flockParticleEmitter;
    private FlockingDataWorker[] workerObjects;

    public ThreadPoolScheduler myThreadScheduler;

    private float flockingStartTime = 0;
    public float flockingUpdateTime = 0;

    
    void awake()
    {
        Application.targetFrameRate = TargetFrameRate;
    }


	// Use this for initialization
	void Start () 
    {
        flockParticleEmitter = this.gameObject.GetComponentInChildren<ParticleSystem>();


        //--------------- Gather all colliders --------------------
        colliders = GetColliders();
        //--------------- Gather all colliders --------------------


        //--------------- Gather all Destination points --------------------
        destinationPoints = GetDestinationPoints();
        destinationPoints.Shuffle();
        //--------------- Gather all Destination points --------------------


        //--------------- Spawn Flocks --------------------
        flockers = new FlockData[FlockingSpawnCount];
        int i = FlockingSpawnCount;

        Vector3 center = this.transform.position;
        while (--i > -1)
            SpawnRandomFlockSpherical(center, flockers, i);
        //--------------- Spawn Flocks --------------------



        //--------------- For each Flock, spawn a Particle--------------------
        if (flockParticleEmitter != null)
        {
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[FlockingSpawnCount];
            i = FlockingSpawnCount;
            while (--i > -1)
                SpawnNewParticle(particles, i);

            flockParticleEmitter.SetParticles(particles, particles.Length);
            flockParticleEmitter.Play();
        }
        //--------------- For each Flock, spawn a Particle --------------------

        InitThreadPool();
    }


    private void InitThreadPool()
    {
        //--------------- Cap the number of threads --------------------
        int maxThreads = ThreadingMaxThreads;
        if (maxThreads <= 0)
            maxThreads = Mathf.Max(SystemInfo.processorCount - 1, 1);
        //--------------- Cap the number of threads --------------------
	
        //--------------- Spread the Flocks over multiple worker-packages --------------------
        int ThreadingPoolPackages = ThreadingPackagesPerThread * maxThreads;

        int flocksPerBlock = Mathf.CeilToInt((float)FlockingSpawnCount / (float)ThreadingPoolPackages);
        workerObjects = new FlockingDataWorker[ThreadingPoolPackages];

        int i = ThreadingPoolPackages;
        int startIdx = 0;
        while (--i > -1)
        {
            FlockingDataWorker workerBlock = new FlockingDataWorker();
            UpdateWorkerObjectStats(workerBlock);

            workerBlock.startWorkIndex = startIdx;
            workerBlock.endWorkIndex = Mathf.Min(flockers.Length, startIdx + flocksPerBlock);
            workerObjects[i] = workerBlock;
            startIdx += flocksPerBlock;
        }
        //--------------- Spread the Flocks over multiple worker-packages --------------------


        myThreadScheduler = Loom.CreateThreadPoolScheduler();
        myThreadScheduler.ForceToMainThread = !MultithreadingEnabled;
        myThreadScheduler.StartASyncThreads(workerObjects, onThreadWorkComplete, null, maxThreads);
    }

    private void RestartThreadPoolWork()
    {
        //if (tickTock && !myThreadScheduler.isBusy && workerObjects != null)
        if (workerObjects != null)
        {
            //--------------- Update WorkerObjects first--------------------
            colliders = GetColliders();

            int i = workerObjects.Length;
            while (--i > -1)
                UpdateWorkerObjectStats(workerObjects[i]);
            //--------------- Update WorkerObjects first --------------------

            //--------------- Then restart the calculations --------------------
            myThreadScheduler.ForceToMainThread = !MultithreadingEnabled;
            myThreadScheduler.StartASyncThreads(workerObjects, onThreadWorkComplete, null, ThreadingMaxThreads);
            //--------------- Then restart the calculations --------------------	
        }
    }




    private EnvironmentCollider[] GetColliders()
    {
        GameObject[] collGOs = GameObject.FindGameObjectsWithTag("EnvironmentCollider");
        
        EnvironmentCollider[] result = new EnvironmentCollider[collGOs.Length];
        int i = collGOs.Length;
        while(--i > -1)
        {
            MeshFilter filter = collGOs[i].gameObject.GetComponentInChildren<MeshFilter>();
            if(filter != null)
            {
                Vector3 sizeRadius = Vector3.Scale( filter.mesh.bounds.extents, collGOs[i].transform.localScale);
                result[i] = new EnvironmentCollider(collGOs[i].transform.position, Mathf.Max(sizeRadius.x, sizeRadius.y, sizeRadius.z) * 1.2f);
            }
        }
        return result;
    }



    private Vector3[] GetDestinationPoints()
    {
        List<Vector3> result = new List<Vector3>();

        GameObject[] destGOs = GameObject.FindGameObjectsWithTag("ParticleDestination");
        int i = destGOs.Length;
        while (--i > -1)
        {
            Transform goTransform = destGOs[i].transform;
            MeshFilter filter = destGOs[i].gameObject.GetComponentInChildren<MeshFilter>();
            if (filter != null)
            {
                Vector3[] verts = filter.mesh.vertices;
                int j = verts.Length;
                while (--j > -1)
                    verts[j] = goTransform.TransformPoint(verts[j]);

                result.AddRange(verts);
            }
        }

        return result.ToArray();
    }



    private void SpawnRandomFlockSpherical(Vector3 center, FlockData[] storeTo, int index)
    {
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        Vector3 randomOrientation = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        Vector3 pos = center + (randomDirection * Random.Range(0f, FlockingMaxSpawnRadius));

        FlockData flock = new FlockData();
        flock.position = pos;
        flock.currentDirection = randomOrientation;
        flock.targetDirection = randomOrientation;
        flock.targetVelocity = Random.Range(FlockingStartVelocityMin, FlockingStartVelocityMax);

        if (index < destinationPoints.Length)
            flock.destinationPoint = destinationPoints[index];

        storeTo[index] = flock;
    }

    private void SpawnNewParticle(ParticleSystem.Particle[] storeTo, int index)
    {
        ParticleSystem.Particle particle = new ParticleSystem.Particle();
        particle.position = flockers[index].position;
        particle.startLifetime = float.MaxValue;
        particle.remainingLifetime = float.MaxValue;
        particle.size = ParticleSize;
        particle.color = Color.white;
        storeTo[index] = particle;
    }


    private void UpdateWorkerObjectStats(FlockingDataWorker flockingDataWorker)
    {
        flockingDataWorker.universeCenter   = this.transform.position;
        flockingDataWorker.AllFlocks      = flockers;
        flockingDataWorker.AllColliders     = colliders;
        flockingDataWorker.maxRandomSiblingsTested = FlockingMaxRandomSiblingsTested;
        flockingDataWorker.maxBoundsRadius  = FlockingBoundsRadius;
        flockingDataWorker.seperationWeight = FlockingSeperationWeight;
        flockingDataWorker.seperationRadius = FlockingSeperationRadius;
        flockingDataWorker.alignmentWeight = FlockingAlignmentWeight;
        flockingDataWorker.alignmentRadius = FlockingAlignmentRadius;
        flockingDataWorker.cohesionWeight   = FlockingCohesionWeight;
        flockingDataWorker.cohesionRadius   = FlockingCohesionRadius;
        flockingDataWorker.destinationAttractRadius = FlockingDestinationAttractRadius;
        flockingDataWorker.destinationReachedRadius = FlockingDestinationReachedRadius;
    }


    private void onThreadWorkComplete(IThreadWorkerObject[] finishedObjects)
    {
        flockingUpdateTime = Time.realtimeSinceStartup - flockingStartTime;
        flockingStartTime = Time.realtimeSinceStartup;

        //--------------- Resize arrays if needed --------------------
        if (flockers.Length != FlockingSpawnCount)
        {
            int startCount = flockers.Length;
            System.Array.Resize(ref flockers, FlockingSpawnCount);

            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[flockParticleEmitter.particleCount];
            flockParticleEmitter.GetParticles(particles);
            System.Array.Resize(ref particles, FlockingSpawnCount);

            if (startCount < FlockingSpawnCount)
            {
                Vector3 center = this.transform.position;
                for (int i = startCount; i < FlockingSpawnCount; i++)
                {
                    SpawnRandomFlockSpherical(center, flockers, i);
                    SpawnNewParticle(particles, i);
                }
            }
            flockParticleEmitter.SetParticles(particles, particles.Length);

            if (myThreadScheduler != null)
                 Destroy(myThreadScheduler.gameObject);

            //Reinit the threadpool
            InitThreadPool();
        }
        else
        {
            //Restart the threadpool
            RestartThreadPoolWork();
        }
        //--------------- Resize arrays if needed --------------------
		
    }






	// Update is called once per frame
	void Update () 
    {
        //--------------- Get particles to alter --------------------
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[flockParticleEmitter.particleCount];
        flockParticleEmitter.GetParticles(particles);
        //--------------- Get particles to alter --------------------
			


        int i = Mathf.Min(flockers.Length, particles.Length); //Should be the same, but hey...
        while (--i > -1)
        {
            //--------------- Update Flocking positions based on Velocity and targetDirection --------------------
            FlockData flock = flockers[i];
            flock.currentDirection = Vector3.Slerp(flock.currentDirection, flock.targetDirection, Time.deltaTime * FlockingSteeringSpeed);
            flock.position += flock.currentDirection * (Time.deltaTime * flock.currentVelocity);
            flock.currentColor = Color.Lerp(flock.currentColor, flock.targetColor, Time.deltaTime * ParticleColorLerpSpeed);
            //--------------- Update Flocking positions based on Velocity and targetDirection --------------------
		
            //--------------- Update assosiated particle --------------------
            particles[i].position = flock.position;
            particles[i].remainingLifetime = float.MaxValue;
            particles[i].color = flock.currentColor;
            particles[i].size = ParticleSize;
            //--------------- Update assosiated particle --------------------

            if (FlockingDebugMode)
            {
                Debug.DrawRay(flock.position, flock.targetDirection * 2f, Color.white);
                Debug.DrawRay(flock.position, flock.currentDirection * 2f, Color.red);
            }
        }

        flockParticleEmitter.SetParticles(particles, particles.Length);
	}
}
