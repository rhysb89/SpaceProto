using UnityEngine;
using System.Collections;

public class CameraScipt : MonoBehaviour {
	public Transform player;

	public float smooth = 2f;
	public float tiltAngle = 10f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		float tiltAroundZ = Input.GetAxis ("Horizontal") * tiltAngle;
		float tiltAroundX = Input.GetAxis ("Vertical") * tiltAngle;
		var target = Quaternion.Euler (tiltAroundX, 0, tiltAroundZ);

		transform.localRotation = Quaternion.Slerp (transform.localRotation, target, Time.deltaTime * smooth);
	
	}
}
