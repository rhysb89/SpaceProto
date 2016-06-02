using UnityEngine;
using System.Collections;

public class MotherShipLaser : MonoBehaviour {
	public float sightRange = 150.0F;

	public GameObject bullet;

	//public AudioClip shot;
	public float fireSpeed;

	//private float nextFire = 0.0F;
	private bool canFire;

	public Transform gun;
	private RaycastHit hit;
	public Mothership mother;
	GameObject player;

	// Use this for initialization
	void Start () {
		canFire = true;
		player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if(mother.hasTarget) {
			transform.LookAt (player.transform);
			Shoot ();
		}
	}
	void Shoot(){

		//Debug.DrawRay (gun.position, Vector3.forward, Color.red);
		//Debug.Log ("CanShoot");
		if (canFire) {			
			if (Physics.SphereCast (gun.position, 10f, transform.forward, out hit, sightRange)) {// Vector3.forward, out hit, sightRange)){
				//Debug.Log ("HasPlayerInSights");

				if (hit.transform.tag == "Player") {
					for (int i = 0; i < 1; i++) {
				
						GameObject laserBeam = Instantiate (bullet, gun.position, gun.rotation) as GameObject;

						laserBeam.GetComponent<Rigidbody> ().velocity = transform.forward * fireSpeed;
	
						canFire = false;
						Invoke ("FireDelay", 0.5f);
					}
				}
			}
		}
	}
	void FireDelay(){
		canFire = true;

	}
}

