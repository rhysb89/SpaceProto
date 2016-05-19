using UnityEngine;
using System.Collections;

public class LockOnRocket : MonoBehaviour {
	public GameObject player;
	public GameObject rocket;
	public Transform rocketPos;

	public float speed;
	public float origSpeed;

	public bool canFire;
	public Transform aimBox;
	public float reloadTime;

	void Start(){
	
		canFire = true;
		origSpeed = speed;
	}

	void Update () {

		Rigidbody playerRB = player.GetComponent<Rigidbody> ();


		if (Input.GetButtonDown ("Rocket") && canFire) {
		
		

			for (int i = 0; i < 1; i++) {

				
					GameObject rocketSpawn;
					rocketSpawn = Instantiate (rocket, rocketPos.position, rocketPos.rotation) as GameObject;
		

				speed += playerRB.velocity.magnitude;
				//Debug.Log (speed);

				rocketSpawn.GetComponent<Rigidbody>().velocity = (aimBox.position - transform.position).normalized * speed;
					



				canFire = false;
				Invoke ("Reload", reloadTime);
			
			}
			
			}
		speed = origSpeed;
		}
	void Reload(){
		canFire = true;
	
	}

}

