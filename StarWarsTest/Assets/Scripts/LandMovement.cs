using UnityEngine;
using System.Collections;

public class LandMovement : MonoBehaviour {
	//public Transform gravityBox;
	//public float gravityForce;

	public Transform LookTransform;

	public float speed = 6.0f;
	public float maxVelocityChange = 10.0f;
	public float jumpForce = 5.0f;
	public float GroundHeight = 1.1f;
	private float xRotation;
	private float yRotation;

	public GameObject flightCanvas;

	public bool canShip;

	public GameObject airPlayer;
	public GameObject landShip;

	public Movement airMove;
	// Use this for initialization
	void Start () {
		canShip = true;
		//airPlayer.SetActive (false);
		//landShip.SetActive (true);
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		Rigidbody rb = GetComponent<Rigidbody> ();

		RaycastHit groundedHit;
		bool grounded = Physics.Raycast(transform.position, -transform.up, out groundedHit, GroundHeight);

		if (grounded)
		{
			flightCanvas.SetActive (false);
			// Calculate how fast we should be moving
			Vector3 forward = Vector3.Cross(transform.up, -LookTransform.right).normalized;
			Vector3 right = Vector3.Cross(transform.up, LookTransform.forward).normalized;
			Vector3 targetVelocity = (forward * Input.GetAxis("Speed") + right * Input.GetAxis("WalkHorizontal")) * speed;

			Vector3 velocity = transform.InverseTransformDirection(rb.velocity);
			velocity.y = 0;
			velocity = transform.TransformDirection(velocity);
			Vector3 velocityChange = transform.InverseTransformDirection(targetVelocity - velocity);
			velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
			velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
			velocityChange.y = 0;
			velocityChange = transform.TransformDirection(velocityChange);

			rb.AddForce(velocityChange, ForceMode.VelocityChange);

			if (Input.GetButton("Jump"))
			{
				rb.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
			}
		}

		float rotSpeed = 3.0f;
		float xRot = rotSpeed * Input.GetAxis("Vertical");
		float yRot = rotSpeed * Input.GetAxis("Horizontal");

		transform.Rotate(xRot, yRot, 0.0f);

		if (canShip && Input.GetButton ("BoardShip")) {
		
			grounded = false;

			airPlayer.SetActive (true);
			airPlayer.transform.position = new Vector3 (landShip.transform.position.x, landShip.transform.position.y + 10, landShip.transform.position.z);
			airPlayer.transform.rotation = landShip.transform.rotation;
			landShip.SetActive (false);
			Movement.onLand = false;
			airMove.landing = false;
			flightCanvas.SetActive (true);
			this.gameObject.SetActive (false);


		}

	}
	void OnTriggerEnter (Collider col){
	
		if (col.tag == "Player") {
		
			canShip = true;
		}
	}
	void OnTriggerExit(Collider col){

		if (col.tag == "Player") {

			canShip = false;
		}
	}
}
