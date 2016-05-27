using UnityEngine;
using System.Collections;

public class PistolReticle : MonoBehaviour {

	public Camera camera;
	public Transform target;
	public Transform reticle;
	public Vector3 reticleCentre;
	public float lookSpeed;


	public float yMin;
	public float yMax;



	// Use this for initialization
	void Start () {
		//camera = GetComponent<Camera> ();
		reticleCentre = target.localPosition;
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 screenPos = camera.WorldToScreenPoint (target.position);
		reticle.position = screenPos;
		//target.localPosition = screenPos;

		/*
		float rotYSpeed = 0f;
		float rotXSpeed = 0f;
		//float xRot = rotSpeed * Input.GetAxis("Vertical");
		rotYSpeed = Input.GetAxis("Vertical")* (Time.deltaTime);
		rotXSpeed = Input.GetAxis("Horizontal")* (Time.deltaTime);
	

		if (Input.GetAxis ("Vertical") > 0.1f && target.localPosition.y > -10 ) {

			//target.localRotation = Quaternion.Euler ((target.localRotation.x + rotYSpeed * lookSpeed), target.localRotation.y , target.localRotation.z);
			target.localPosition = new Vector3 (target.localPosition.x, (target.localPosition.y - rotYSpeed * lookSpeed), reticleCentre.z);
		}
		if (Input.GetAxis ("Vertical") < -0.1f && target.localPosition.y < 10 ) {

			//target.localRotation = Quaternion.Euler ((target.localRotation.x + rotYSpeed * lookSpeed), target.localRotation.y , target.localRotation.z);
			target.localPosition = new Vector3 (target.localPosition.x, (target.localPosition.y - rotYSpeed * lookSpeed), reticleCentre.z);
		}
		if (Input.GetAxis ("Horizontal") > 0.1f && target.localPosition.x < 1.5f ) {
			target.localPosition = new Vector3 ((target.localPosition.x + rotXSpeed * lookSpeed), target.localPosition.y, reticleCentre.z);
		}
		if (Input.GetAxis ("Horizontal") < -0.1f && target.localPosition.x > -1.5f ) {
			target.localPosition = new Vector3 ((target.localPosition.x + rotXSpeed * lookSpeed), target.localPosition.y, reticleCentre.z);
		} 
	*/
		//yRot = Mathf.Clamp(yRot,yMin,yMax);
		//transform.rotation = Quaternion.identity;
		//transform.localEulerAngles = new Vector3 ( Mathf.Clamp (transform.rotation.y, cMin, cMax), 0,0);

		//target.position = new Vector3 (yRot, 0, 0.0f);
	}
}