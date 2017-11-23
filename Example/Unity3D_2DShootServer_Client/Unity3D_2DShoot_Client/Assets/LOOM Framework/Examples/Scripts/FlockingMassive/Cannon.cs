using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cannon : MonoBehaviour 
{
    public Transform CannonballToInstantiate;

    public float LeftBorder;
    public float BottomBorder;
    public float TopBorder;
    public float RightBorder = 350;

    public float CannonballSpeed = 80;

    private List<Transform> cannonBalls = new List<Transform>();
    private Camera cam;
    private float lastShot = 0f;
    private HUDmanager hud;

    private void Start()
    {
        cam = this.gameObject.GetComponentInChildren<Camera>();
        hud = (HUDmanager)GameObject.FindObjectOfType(typeof(HUDmanager));
    }
	
	
	// Update is called once per frame
	void Update () 
    {
        if (hud.panelIdx == 0 && Input.GetMouseButton(0))
        {
            if (Time.realtimeSinceStartup - lastShot > 0.35f)
            {
                Vector2 invMousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
                if (invMousePos.y > 50)
                {

                    //New Cananball
                    Transform cannonball = (Transform)Instantiate(CannonballToInstantiate);
                    Vector3 newForward = cam.ScreenPointToRay(Input.mousePosition).direction;
                    cannonball.position = cam.transform.position + (newForward * 15f);
                    cannonball.forward = newForward;
                    cannonBalls.Add(cannonball);

                    lastShot = Time.realtimeSinceStartup;
                }
            }
        }


        List<Transform> destroyAndRemove = new List<Transform>();
        foreach (Transform cannonball in cannonBalls)
        {
            cannonball.position += cannonball.forward * (Time.deltaTime * CannonballSpeed);

            if (cannonball.position.y < -20f)
                destroyAndRemove.Add(cannonball);
        }

        if (destroyAndRemove.Count > 0)
        {
            foreach (Transform cannonball in destroyAndRemove)
            {
                Destroy(cannonball.gameObject);
                cannonBalls.Remove(cannonball);
            }
        }
	}
}
