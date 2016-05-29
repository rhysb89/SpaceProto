using UnityEngine;
using System.Collections;

public class AIGround : MonoBehaviour {
	public float health;

	public float wanderRadius;
	public float wanderTimer;

	private Transform target;
	private NavMeshAgent agent;
	private float timer;
	public FX_3DRadar_RID marker;

	private bool canFire;
	private bool hasTarget = false;
	public Transform gun;
	public float fireSpeed;

	public float sightRange = 150.0F;

	public GameObject bullet;

	public Transform startOrigin;
	public Transform player;

	void OnEnable(){
		canFire = true;
		agent = GetComponent<NavMeshAgent> ();
		timer = wanderTimer;

	}
	void Update () {
		timer += Time.deltaTime;

		if (timer >= wanderTimer && !hasTarget) {
			Vector3 newPos = RandomNavSphere(startOrigin.position, wanderRadius, -1);
			agent.SetDestination(newPos);
			timer = 0;
		}
		AITrig trig = GetComponentInParent<AITrig> ();


		if (trig.inTrig) {
			hasTarget = true;
		} else {
			hasTarget = false;
		}
		if (health <= 0) {

			marker.DestroyThis ();
		
			gameObject.SetActive (false);

		}
		if (hasTarget) {
			transform.LookAt (player);
			agent.SetDestination(player.position);
			Shoot ();
		
		}

	}
	public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask) {
		Vector3 randDirection = Random.insideUnitSphere * dist;

		randDirection += origin;

		NavMeshHit navHit;

		NavMesh.SamplePosition (randDirection, out navHit, dist, layermask);

		return navHit.position;
	}


	public void Shoot () {

		RaycastHit hit;

		if (Physics.SphereCast(gun.position, 10f, transform.forward, out hit, sightRange)){// Vector3.forward, out hit, sightRange)){

			if (hit.transform.tag == "Player" && canFire) {
				for (int i = 0; i < 1; i++) {
					

					gun.LookAt (GameObject.FindGameObjectWithTag("Player").transform);
				
					GameObject laserBeam = Instantiate (bullet, gun.position, gun.rotation) as GameObject;

					laserBeam.GetComponent<Rigidbody> ().velocity = transform.forward * fireSpeed;
				


					canFire = false;
					i = 0;
			
					Invoke ("FireDelay", 0.5f);
				}
			}
		}
	}

	void FireDelay(){
		canFire = true;

	}
	void OnCollisionEnter (Collision col){

		if (col.transform.tag == "Laser") {

			Destroy (col.gameObject);
		
			health -= 5;
			Invoke ("Explodes", 0.5f);

		}
		if (col.transform.tag == "Rocket") {

			Destroy (col.gameObject);

			health -= health;
			Invoke ("Explodes", 0.5f);
		}


	}
	void Explodes(){

	}
}
