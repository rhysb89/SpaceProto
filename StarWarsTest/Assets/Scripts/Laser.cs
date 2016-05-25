using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Laser : MonoBehaviour {

	public GameObject laser;
	public Transform firePos;

	public float fireSpeed;
	public bool canFire;
	public Movement moveScript;

	public Transform fireBox;
	GameObject target;

	public Transform leadAim;

	public float overHeat;
	public bool overHeated;
	public Slider laserSlider;
	public bool firing;

	// Use this for initialization
	void Start () {
		canFire = true;
		overHeat = 0;
		firing = false;

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		ReticleBehavior reticleBehaviour;
		reticleBehaviour = Camera.main.GetComponent<ReticleBehavior> ();

		target = reticleBehaviour.targetObject;

		GameObject laserBeam;
	

		if (overHeat >= 100) {
			overHeat = 100;
			overHeated = true;
			Invoke ("OverHeatedGuns", 5f);
		}
		if (overHeated) {

			overHeat -= 20 * Time.deltaTime;
		}
		if (overHeat == 0) {
			overHeated = false;
		}
		if (overHeat > 0 && Input.GetAxis("Fire1") < 0.2f && !firing) {
			if (!firing) {
				Invoke ("CoolDown", 2f);
			}
		}
		if (overHeat < 0) {
			overHeat = 0;
		}
		laserSlider.value = overHeat;

		if (reticleBehaviour.lockedOn) {

			//AI sphere;
			//sphere = target.GetComponent<AI> ();

		

			float time;
			float distance;

			//float distanceOfLead;
			Vector3 distanceBetween;

			distanceBetween = target.transform.position - transform.position;
			distance = distanceBetween.magnitude;

			Vector3 targetVel = target.GetComponent<Rigidbody> ().velocity;

			time = fireSpeed / distance;
		

			leadAim.position = (targetVel.normalized * time) + target.transform.position;
	

			firePos.LookAt (leadAim);
		


			if (Input.GetAxis ("Fire1") > 0.8f && canFire && !overHeated) {
				//fireSpeed += moveScript.speed/10;
				for (int i = 0; i < 1; i++) {

					firing = true;
					laserBeam = Instantiate (laser, transform.position, firePos.rotation) as GameObject;
					laserBeam.GetComponent<Rigidbody> ().velocity = transform.forward * fireSpeed;
					overHeat = overHeat + 20;
					canFire = false;
					Invoke ("FireDelay", 0.5f);
				}

			}
			}else if (!reticleBehaviour.lockedOn) {

			if (Input.GetAxis ("Fire1") > 0.8f && canFire && !overHeated) {
				//fireSpeed += moveScript.speed/10;
				for (int i = 0; i < 1; i++) {
					firing = true;
					firePos.LookAt (fireBox);
					laserBeam = Instantiate (laser, transform.position, firePos.rotation) as GameObject;
					laserBeam.GetComponent<Rigidbody> ().velocity = (fireBox.position - transform.position).normalized * fireSpeed;
					overHeat = overHeat + 20;
					canFire = false;
					Invoke ("FireDelay", 0.5f);
				}
			
			}
		}
		if (Input.GetAxis ("Fire1") < 0.5f) {
			firing = false;
		}
	}

	void FireDelay(){
		canFire = true;
	}
	void OverHeatedGuns(){
		overHeated = false;
	}
	void CoolDown (){
		if (!firing && overHeat > 0) {

			overHeat -= 10 * Time.deltaTime;
		}
		else {
//			overHeat = overHeat;
			Debug.Log ("Started Firing again");
			return;
		}
	}
}
