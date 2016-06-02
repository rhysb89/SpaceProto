using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour {
	public float health;

	public float speed;
	public float rotateDamp;
	public float acceleration = 20.0F;
	public float minimumSpeed = 30.0F;
	public float maximumSpeed = 100.0F;
	public float bufferRange = 25.0F;

	private bool slowDown;
	private bool keepSpeed;
	private bool speedUp;

	public string enemy;

	public float sightRange = 150.0F;
	public float attackRange = 75.0F;
	public GameObject bullet;
	public float fireRate = 0.09F;
	public int rounds = 10000;
	//public AudioClip shot;

	//private float nextFire = 0.0F;
	private bool canFire;
	private bool hasTarget = false;
	public Transform gun;
	public Transform gun2;

	private Transform target;
	private RaycastHit hit;
	private float distance;

	//public Transform centerOfMap;

	private Transform plane;
	public float AISpeed;

	public Transform x1;
	public Transform x2;
	public Transform y1;
	public Transform y2;
	public Transform z1;
	public Transform z2;

	public Transform destination;
	public float targetSpeed =1f;
	public Movement playerSpeed;

	public float fireSpeed;

	public GameObject hitExplosion;
	//public GameObject deathExplosion;
	public GameObject explodeDebris;

	public float explodePower;
	public float explodeRadius;
	public Transform explodePoint;
	public FX_3DRadar_RID marker;

	void Awake () {

		plane = transform;
	
	}

	void Start () {
		gameObject.SetActive (true);
		target = GameObject.FindWithTag("Player").transform;

		hasTarget = false;

		float xp = Random.Range (x1.position.x, x2.position.x);
		float yp = Random.Range (y1.position.y, y2.position.y);
		float zp = Random.Range (z1.position.z, z1.position.z);

		destination.position = new Vector3 (xp, yp, zp);
		canFire = true;
	
	}

	void FixedUpdate () {

	


		AISpeed = gameObject.GetComponent<Rigidbody> ().velocity.magnitude;

		AITrig trig = GetComponentInParent<AITrig> ();


		if(!hasTarget){
			FindTarget();
		}
		else if(hasTarget){
			
			targetSpeed = playerSpeed.speed;
			var rotate = Quaternion.LookRotation(target.position - plane.position);
			plane.rotation = Quaternion.Slerp(plane.rotation, rotate, Time.deltaTime * rotateDamp);


			//Physics.Raycast(plane.position, Vector3.forward, out hit, sightRange);
			//Physics.Raycast(plane.position, Vector3.forward, out hit, attackRange);
			plane.Translate(Vector3.forward * speed * Time.deltaTime);
			distance = Vector3.Distance(target.transform.position, plane.position);

			//if (hit.collider.tag == ("Player")) {
				Shoot ();
			//} else {

			//}
			//Shoot();
		}



		if(distance <= sightRange &&  distance > attackRange){
			speed += acceleration;
			speed = Mathf.Clamp(speed, minimumSpeed, maximumSpeed);
		}

		else if(distance <= bufferRange){
			
		

			if(targetSpeed < speed){
				speed -= acceleration;
				speed = Mathf.Clamp(speed, targetSpeed, maximumSpeed);
			}
			else if(targetSpeed > speed){
				speed += acceleration * Time.deltaTime;
				speed = Mathf.Clamp(speed, minimumSpeed, targetSpeed);
			}
		}

		if (transform.position.magnitude < (destination.position.magnitude + 2) && !hasTarget) {

			float xp = Random.Range (x1.position.x, x2.position.x);
			float yp = Random.Range (y1.position.y, y2.position.y);
			float zp = Random.Range (z1.position.z, z1.position.z);

			destination.position = new Vector3 (xp, yp, zp);
			plane.Translate(Vector3.forward * speed * Time.deltaTime);
		
		}
		if (trig.inTrig) {
			hasTarget = true;
		} else {
			hasTarget = false;
		}
		if (health <= 0) {

			if (StationTrig.missionStarted) {
				StationTrig.killCount += 1;
				Debug.Log (StationTrig.killCount);
			}

			marker.DestroyThis ();
			GameObject debris = Instantiate (explodeDebris, this.transform.position, explodePoint.transform.rotation) as GameObject;
			explodeDebris.transform.position = this.transform.position;
			Rigidbody rb = debris.GetComponent<Rigidbody> ();
			if (rb != null) {
				rb.AddExplosionForce (explodePower, explodePoint.transform.position, explodeRadius, 3f);
			}
			gameObject.SetActive (false);
		
		}
	}

	public void Shoot () {
		//Debug.DrawRay (gun2.position, Vector3.forward, Color.red);
		//Debug.Log ("CanShoot");
		if (Physics.SphereCast(gun2.position, 10f, transform.forward, out hit, sightRange)){// Vector3.forward, out hit, sightRange)){
			//Debug.Log ("HasPlayerInSights");


			if (hit.transform.tag == "Player" && canFire) {
			for (int i = 0; i < 1; i++) {
				//nextFire = Time.time + fireRate;

					gun.LookAt (GameObject.FindGameObjectWithTag("Player").transform);
					gun2.LookAt (GameObject.FindGameObjectWithTag ("Player").transform);
				GameObject laserBeam = Instantiate (bullet, gun.position, gun.rotation) as GameObject;
				GameObject laserBeamTwo = Instantiate (bullet, gun2.position, gun2.rotation) as GameObject;
				laserBeam.GetComponent<Rigidbody> ().velocity = transform.forward * fireSpeed;
				laserBeamTwo.GetComponent<Rigidbody> ().velocity = transform.forward * fireSpeed;
				//rounds--;


				canFire = false;
				//	i = 0;
				//audio.PlayOneShot(shot);
				Invoke ("FireDelay", 0.5f);
			}
		}
	}
	}
	void FindTarget () {



			target = GameObject.FindWithTag ("Player").transform;

			speed = minimumSpeed;
			plane.Translate(Vector3.forward * speed * Time.deltaTime);

			var rotate = Quaternion.LookRotation (destination.position - plane.position);
			plane.rotation = Quaternion.Slerp (plane.rotation, rotate, Time.deltaTime * rotateDamp);

		if (Physics.SphereCast (gun2.position, 10f, transform.forward, out hit, sightRange)) {
			if (hit.transform.tag == "Player") {
				hasTarget = true;
				//Debug.Log ("HasTarget");
			}
		}

	}
	void FireDelay(){
		canFire = true;

	}
	void OnCollisionEnter (Collision col){

		if (col.transform.tag == "Laser") {
			//Debug.Log ("Hit");
	//		Vector3 impactPoint = col.transform.position;
			Destroy (col.gameObject);
//			GameObject explosion = Instantiate (hitExplosion, impactPoint, transform.rotation) as GameObject;
			health -= 5;
			Invoke ("Explodes", 0.5f);

		}
		if (col.transform.tag == "Rocket") {
			//Debug.Log ("Hit");
			Vector3 impactPoint = col.transform.position;
			Destroy (col.gameObject);
			GameObject explosion = Instantiate (hitExplosion, impactPoint, transform.rotation) as GameObject;
			health -= health;
			Invoke ("Explodes", 0.5f);
		}
		if (col.transform.tag == "Player") {



			Rigidbody enemyHit = col.gameObject.GetComponent <Rigidbody> ();
			Vector3 enemyVel = enemyHit.velocity;


			Rigidbody playerRB = gameObject.GetComponent<Rigidbody> ();
			Vector3 playerVel = playerRB.velocity;

			Vector3 normal = col.contacts [0].normal;
			Debug.Log (Vector3.Angle(normal,playerVel));

			health +=((enemyVel.magnitude + playerVel.magnitude) - (Vector3.Angle(normal,enemyVel))/2)/5;

			//health -= health;
			Invoke ("Explodes", 0.5f);
		
		}
		if (col.transform.tag == "Asteroid" && hasTarget) {

			Rigidbody enemyHit = col.gameObject.GetComponent <Rigidbody> ();
			Vector3 enemyVel = enemyHit.velocity;


			Rigidbody playerRB = gameObject.GetComponent<Rigidbody> ();
			Vector3 playerVel = playerRB.velocity;

			Vector3 normal = col.contacts [0].normal;
			Debug.Log (Vector3.Angle(normal,playerVel));

			health +=( (enemyVel.magnitude + playerVel.magnitude) - (Vector3.Angle(normal,enemyVel))/2)/5;

		}
	
	}
	void Explodes(){
		
	}
}
