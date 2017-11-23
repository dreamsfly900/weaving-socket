using UnityEngine;
using System.Collections;

public class MovingBodies : MonoBehaviour 
{
    public Transform rotateAroundTarget;
    public Vector3 rotateAroundAxis = Vector3.up;
    public float rotateAroundSpeed;

    private float angle;

	
	// Update is called once per frame
	void Update () 
    {
        this.transform.RotateAround(rotateAroundTarget.position, rotateAroundAxis, Time.deltaTime * rotateAroundSpeed);
	}
}
