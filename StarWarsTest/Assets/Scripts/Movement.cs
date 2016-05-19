using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

	public float startSpeed = 100f;
	public float maxSpeed = 400f;

	public float acceleration = 1f;
	public float speed = 10f;

	public float pitchSpeed;
	public float rollSpeed;
	public Vector3 currentAngle;

	public float AmbientSpeed = 100.0f;

	public float RotationSpeed = 200.0f;

	public Transform reticle;
	public Vector3 reticleCentre;

	// Use this for initialization
	void Start () {
	
		reticleCentre = reticle.localPosition;
		Rigidbody rb = GetComponent <Rigidbody> ();

		rb.AddForce (new Vector3 (0, 0, startSpeed));

	}
	
	// Update is called once per frame
	void FixedUpdate () {

		ReticleBehavior ReticleScript = Camera.main.GetComponent<ReticleBehavior> ();
		//reticle.localPosition = reticleCentre;

		currentAngle = transform.eulerAngles;
		Quaternion AddRot = Quaternion.identity;
		Rigidbody rigidbody = GetComponent <Rigidbody> ();


		float roll = 0;
		float pitch = 0;
		float yaw = 0;
		roll = Input.GetAxis("Horizontal") * (Time.deltaTime * rollSpeed);
		pitch = Input.GetAxis("Vertical") * (Time.deltaTime * pitchSpeed);
		yaw = Input.GetAxis("Yaw") * (Time.deltaTime * RotationSpeed);
		AddRot.eulerAngles = new Vector3(pitch, yaw, -roll);
		rigidbody.rotation *= AddRot;
		Vector3 AddPos = Vector3.forward;
		AddPos = rigidbody.rotation * AddPos;


		if (ReticleScript.lockedOn == false) {

			if (Input.GetAxis ("Vertical") > 0.8f && reticle.localPosition.y > -50) {
				reticle.localPosition = new Vector3 (reticleCentre.x, (reticle.localPosition.y - pitch), reticleCentre.z);
			}
			if (Input.GetAxis ("Vertical") < -0.8f && reticle.localPosition.y < 50) {
				reticle.localPosition = new Vector3 (reticleCentre.x, (reticle.localPosition.y - pitch), reticleCentre.z);
			} else if (Input.GetAxis ("Vertical") == 0) {

				reticle.localPosition = Vector3.Lerp (reticle.localPosition, reticleCentre, Time.deltaTime * 5);
			}
		}

		CheckInputs ();

		//Rigidbody rb = GetComponent <Rigidbody> ();


		if (speed > maxSpeed) {
			speed = maxSpeed;
		}
		if (speed < 1) {
			speed = 1;
		}
		rigidbody.velocity = AddPos * (Time.deltaTime * speed);
		//rb.velocity = (transform.forward * speed * Time.deltaTime);

	}
	void CheckInputs(){
		

		//Debug.Log (Input.GetAxis ("Speed"));

		if (Input.GetAxis("Speed") > 0 && speed < maxSpeed){

			acceleration = Input.GetAxis("Speed") * 20;
			//Debug.Log (speed);
			speed += acceleration;
			//rb.angularVelocity = (new Vector3 (0, 0, speed) *Time.deltaTime);
		}
		if (Input.GetAxis("Speed") < 0 && speed > 1){

			acceleration = Input.GetAxis("Speed") * 40;

			speed += acceleration;
			//rb.angularVelocity = (new Vector3 (0, 0, -speed) * Time.deltaTime);
		}

		//if (Input.GetAxis ("Yaw") > 0) {
		

	//	}


	
	}

}
