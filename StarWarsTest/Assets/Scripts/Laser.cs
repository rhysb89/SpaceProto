using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

	public GameObject laser;
	public Transform firePos;

	public float fireSpeed;
	public bool canFire;
	public Movement moveScript;

	public Transform fireBox;
	GameObject target;

	public Transform leadAim;

	// Use this for initialization
	void Start () {
		canFire = true;

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		ReticleBehavior reticleBehaviour;
		reticleBehaviour = Camera.main.GetComponent<ReticleBehavior> ();

		target = reticleBehaviour.targetObject;

		GameObject laserBeam;
	

		if (reticleBehaviour.lockedOn) {

			AI sphere;
			sphere = target.GetComponent<AI> ();

		

			float time;
			float distance;

			float distanceOfLead;
			Vector3 distanceBetween;

			distanceBetween = target.transform.position - transform.position;
			distance = distanceBetween.magnitude;

			Vector3 targetVel = target.GetComponent<Rigidbody> ().velocity;

			time = fireSpeed / distance;
			//Debug.Log ("Estimated laser travel time to target: " + time);

			//futureEnemyPosition = (enemyVelocity.normalized * time) + enemy.transform.position;
			//leadAim.position = futureEnemyPosition;

			//distanceOfLead = sphere.AISpeed * time;
			//Debug.Log("Target velocity is:" + targetVel);

			leadAim.position = (targetVel.normalized * time * 2) + target.transform.position;
			//Debug.Log ("Enemy position is: " + target.transform.position + " so we are aiming at pos: " + ((targetVel.normalized * time) + target.transform.position)); 
			//Debug.Log ("Move");

			firePos.LookAt (leadAim);

			if (Input.GetAxis ("Fire1") > 0.8f && canFire) {
				//fireSpeed += moveScript.speed/10;
				for (int i = 0; i < 1; i++) {


					laserBeam = Instantiate (laser, transform.position, firePos.rotation) as GameObject;
					laserBeam.GetComponent<Rigidbody> ().velocity = transform.forward * fireSpeed;

					canFire = false;
					Invoke ("FireDelay", 0.5f);
				}

			}
			}else if (!reticleBehaviour.lockedOn) {

			if (Input.GetAxis ("Fire1") > 0.8f && canFire) {
				//fireSpeed += moveScript.speed/10;
				for (int i = 0; i < 1; i++) {

					firePos.LookAt (fireBox);
					laserBeam = Instantiate (laser, transform.position, firePos.rotation) as GameObject;
					laserBeam.GetComponent<Rigidbody> ().velocity = (fireBox.position - transform.position).normalized * fireSpeed;
					canFire = false;
					Invoke ("FireDelay", 0.5f);
				}
			
			}


				//laserBeam.GetComponent<Rigidbody> ().velocity = new Vector3 (0, 0, fireSpeed);

				//fireSpeed = 10;
			}
	}

	void FireDelay(){
		canFire = true;
	}
}
