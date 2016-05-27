using UnityEngine;
using System.Collections;

public class Pistol : MonoBehaviour {

	public GameObject laser;
	public Transform firePos;

	public float fireSpeed;
	public bool canFire;
	public LandMovement moveScript;

	public Transform fireBox;
	//GameObject target;

	//public Transform leadAim;

	public float overHeat;
	public bool overHeated;
	//public Slider laserSlider;
	public bool firing;

	// Use this for initialization
	void Start () {
		canFire = true;
		overHeat = 0;
		firing = false;

	}

	// Update is called once per frame
	void FixedUpdate () {
		//ReticleBehavior reticleBehaviour;
		//reticleBehaviour = Camera.main.GetComponent<ReticleBehavior> ();

		//target = reticleBehaviour.targetObject;

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


			if (Input.GetAxis ("Fire1") > 0.8f && canFire && !overHeated) {
				//fireSpeed += moveScript.speed/10;
				for (int i = 0; i < 1; i++) {
				Debug.Log ("Fired");
					firing = true;
					firePos.LookAt (fireBox);
					laserBeam = Instantiate (laser, transform.position, transform.rotation) as GameObject;
					laserBeam.GetComponent<Rigidbody> ().velocity = new Vector3 (transform.localPosition.x,transform.localPosition.y,transform.localPosition.z + fireSpeed);
			
					overHeat = overHeat + 20;
					canFire = false;
					Invoke ("FireDelay", 0.5f);
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

			//Debug.Log ("Started Firing again");
			return;
		}
	}
}

