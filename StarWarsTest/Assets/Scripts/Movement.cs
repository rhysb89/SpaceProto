using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class Movement : MonoBehaviour {

	public float startSpeed = 100f;
	public float maxSpeed = 400f;
	public float lightSpeed;
	public bool travellingLight;

	public float acceleration = 1f;
	public float speed = 10f;

	public float pitchSpeed;
	public float rollSpeed;
	public Vector3 currentAngle;

	public float AmbientSpeed = 100.0f;

	public float RotationSpeed = 200.0f;

	public Transform reticle;
	public Vector3 reticleCentre;

	public float health;
	public float shield;
	public bool hit;
	public bool shot;
	public float damageByLaser;
	public float damageByHit;

	bool shieldDepleted = false;
	bool shieldRegenAllowed = true;

	public float rechargeTime;
	public float maxShield;
	public Slider shieldSlider;
	public Slider healthSlider;

	public bool inPlanet;
	public bool landing = false;
	public float landingSpeed;

	public static bool onLand;
	public Transform gravityBox;
	public GameObject landPlayer;
	public Transform playerExit;
	public Transform spareX;

	public GameObject landingGear;
	public Fisheye fish;
	public GameObject particles;


	// Use this for initialization
	void Start () {
		landPlayer.SetActive (false);
		spareX.gameObject.SetActive (false);
		onLand = false;
		inPlanet = false;
		//landing = false;
		travellingLight = false;
		shieldDepleted = false;
		hit = false;
		shot = false;
		shieldRegenAllowed = true;
		reticleCentre = reticle.localPosition;
		Rigidbody rb = GetComponent <Rigidbody> ();

		rb.AddForce (new Vector3 (0, 0, startSpeed));
		InvokeRepeating ("Allowed", 1f, 1f);
		landingGear.SetActive (false);

	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (onLand) {
			speed = LandMovement.takeOffSpeed;
		}

		if (!onLand) {
			ReticleBehavior ReticleScript = Camera.main.GetComponent<ReticleBehavior> ();
			//reticle.localPosition = reticleCentre;

			currentAngle = transform.eulerAngles;
			Quaternion AddRot = Quaternion.identity;
			Rigidbody rigidbody = GetComponent <Rigidbody> ();


			float roll = 0;
			float pitch = 0;
			float yaw = 0;
			roll = Input.GetAxis ("Horizontal") * (Time.deltaTime * rollSpeed);
			pitch = Input.GetAxis ("Vertical") * (Time.deltaTime * pitchSpeed);
			yaw = Input.GetAxis ("Yaw") * (Time.deltaTime * RotationSpeed);
			AddRot.eulerAngles = new Vector3 (pitch, yaw, -roll);
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


			if (speed > maxSpeed && !travellingLight) {
				speed = maxSpeed;
			}
			if (speed < 1 && !landing) {
				speed = 1;
			}

			//rb.velocity = (transform.forward * speed * Time.deltaTime);

			if (shieldRegenAllowed && shield < maxShield) {

				if (shield > maxShield) {
					shield = maxShield;
				}
		

			}
			shieldSlider.value = shield;
			healthSlider.value = health;
			if (travellingLight) {
		
				speed = lightSpeed;
			}
			if (Input.GetButtonDown ("LightSpeed")) {

				travellingLight = !travellingLight;

				if (!travellingLight) {
					speed = maxSpeed;

				}
			}
			if (travellingLight) {
				if (fish.strengthY < 0.8f) {
					fish.strengthY += 0.8f * Time.deltaTime;
					Camera.main.fieldOfView += 40 * Time.deltaTime;
				}
				if (fish.strengthY > 0.8f) {
					fish.strengthY = 0.8f;
					Camera.main.fieldOfView = 100;
				
				}
				particles.SetActive (true);


			} else {

				if (fish.strengthY > 0) {
					fish.strengthY -= 1.6f * Time.deltaTime;
					Camera.main.fieldOfView -= 80 * Time.deltaTime;
				}
				if (fish.strengthY < 0){
					fish.strengthY = 0;
					Camera.main.fieldOfView = 60;

				}
				particles.SetActive (false);

			
			}
			if (Input.GetButtonDown ("Land") && inPlanet) {

				landing = !landing;

			}

			if (landing) {

				landingGear.SetActive (true);
				//speed = 10;
				//transform.position = Vector3.MoveTowards (transform.position, gravityBox.position, landingSpeed * Time.deltaTime);// * (Time.deltaTime * landingSpeed);
			}
			//}
				if (!landing) {
					landingGear.SetActive (false);
				}
					
				rigidbody.velocity = AddPos * (Time.deltaTime * speed);
			//}
		}
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

	
	
	}
	void OnCollisionEnter(Collision col){
	
		if (col.transform.tag == "EnemyLaser") {
		
			Shot();
		}
		if (col.transform.tag == "Enemy") {

			Rigidbody enemyHit = col.gameObject.GetComponent <Rigidbody> ();
			Vector3 enemyVel = enemyHit.velocity;


			Rigidbody playerRB = gameObject.GetComponent<Rigidbody> ();
			Vector3 playerVel = playerRB.velocity;

			Vector3 normal = col.contacts [0].normal;
			Debug.Log ("Player angle" + Vector3.Angle(normal,playerVel));

			damageByHit = (enemyVel.magnitude + playerVel.magnitude) - (Vector3.Angle(normal,playerVel))/2;
			damageByHit = -damageByHit;
			Hit ();
		}
		if (col.transform.tag == "Asteroid" && !landing) {

			Rigidbody enemyHit = col.gameObject.GetComponent <Rigidbody> ();
			Vector3 enemyVel = enemyHit.velocity;


			Rigidbody playerRB = gameObject.GetComponent<Rigidbody> ();
			Vector3 playerVel = playerRB.velocity;

			Vector3 normal = col.contacts [0].normal;
			Debug.Log ("Player angle" + Vector3.Angle(normal,playerVel));

			damageByHit = ((enemyVel.magnitude + playerVel.magnitude) - (Vector3.Angle(normal, playerVel)))/2;
			damageByHit = -damageByHit/2;
			Hit ();
		
		}
		if (col.transform.tag == "Asteroid" && landing) {
		
			onLand = true;
			gravityBox.position = col.transform.position;
			spareX.position = transform.position;
			//spareX.rotation = transform.rotation;
			spareX.gameObject.SetActive (true);
			gameObject.SetActive (false);

			landPlayer.transform.position = playerExit.position;
			landPlayer.SetActive (true);

		}
	}
	public void Shot(){

		StartCoroutine (Disallow (3f));
		if (shield > 0) {
			shield -= damageByLaser;
			if (damageByLaser > shield) {
				health += shield; 
				shield = 0;
				shieldDepleted = true;
				//Invoke ("RechargeShield",3f);
			}
				
		}
		if (shield == 0 && shieldDepleted) {

			health -= damageByLaser;

		}
	
	

	}
	public void Hit(){



		StartCoroutine (Disallow (3f));
		if (shield > 0) {
			shield -= damageByHit;
			if (damageByHit > shield) {
				//health += shield; 
				shield = 0;
				shieldDepleted = true;
				//Invoke ("RechargeShield",3f);
			}

		}
		if (shield == 0) {

			health -= damageByHit;
		

		}

	}

	IEnumerator Disallow (float delay){
	
		shieldRegenAllowed = false;
		yield return new WaitForSeconds (delay);
		shieldRegenAllowed = true;

	}
	void Allowed(){
		if (shieldRegenAllowed && (shield < maxShield)) {
			
			shield += rechargeTime;

		}
	}
	void OnTriggerEnter (Collider col){

		if (col.tag == "Planet") {

			inPlanet = true;
			travellingLight = false;

			if (travellingLight) {
				speed = maxSpeed;
			}
		
		}
	}
	void OnTriggerExit (Collider col){
		if (col.tag == "Planet") {

			inPlanet = false;

		}

	}
}
