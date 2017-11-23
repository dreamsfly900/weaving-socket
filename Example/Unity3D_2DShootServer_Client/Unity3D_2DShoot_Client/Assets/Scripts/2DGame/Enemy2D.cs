using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2D : MonoBehaviour {

    #region Fields
    public float minSpeed;
    public float maxSpeed;

    private float minRotateSpeed = 60f;
    private float maxRotateSpeed = 120f;
    private float minScale = .8f;
    private float maxScale = 2f;

    private float currentRotationSpeed;
    private float currentScaleX;
    private float currentScaleY;
    private float currentScaleZ;

    private float currentSpeed;
    private float x, y, z;
    #endregion

    #region Functions
    void Start()
    {
        SetPositionAndSpeed();
    }

    void Update()
    {
        float rotationSpeed = currentRotationSpeed * Time.deltaTime;
        transform.Rotate(new Vector3(-1, 0, 0) * rotationSpeed);

        //move on by time not cpu power
        float amtToMove = currentSpeed * Time.deltaTime;
        transform.Translate(Vector3.down * amtToMove, Space.World);

        if (transform.position.y <= -5.7  && Player2D.Instance.Lives !=0 ) //enemy down over screen
        {
            SetPositionAndSpeed();
            Player2D.Instance.Missed++;
            Debug.Log("Miss enemy...");

            GameScoreHandler.updateMissedEvent.Invoke(Player2D.Instance.Missed);
        }
    }

    public void SetPositionAndSpeed()
    {
        currentRotationSpeed = Random.Range(minRotateSpeed, maxRotateSpeed);

        currentScaleX = Random.Range(minScale, maxScale);
        currentScaleY = Random.Range(minScale, maxScale);
        currentScaleZ = Random.Range(minScale, maxScale);

        currentSpeed = Random.Range(minSpeed, maxSpeed);
        x = Random.Range(-6f, 6f);
        y = 7.0f;
        z = 0.0f;

        transform.position = new Vector3(x, y, z);

        transform.localScale = new Vector3(currentScaleX, currentScaleY, currentScaleZ);
    }
    #endregion
}
