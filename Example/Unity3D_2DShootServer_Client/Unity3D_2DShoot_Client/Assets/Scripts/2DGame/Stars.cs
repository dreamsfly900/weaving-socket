using UnityEngine;
using System.Collections;


public class Stars : MonoBehaviour {

	public float speed;
	
	void Update() {
		float amtToMove = speed * Time.deltaTime;
		transform.Translate(Vector3.down * amtToMove, Space.World);
		
		if(transform.position.y < -10.75) {
			transform.position = new Vector3(transform.position.x, 14f, transform.position.z);
		}
	}
}
