using UnityEngine;
using System.Collections;

public class VaporatorDestroy : MonoBehaviour {
	public float health = 50;

	public GameObject explodeDebris;
	public GameObject hitExplosion;
	public float explodePower;
	public float explodeRadius;
	public Transform explodePoint;
	public GameObject bigExplode;
	public GameObject Parent;
	// Use this for initialization
	void Start () {
		bigExplode.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (health <= 0) {
			Mothership.vapCount += 1;
			bigExplode.SetActive (true);
			Parent.gameObject.SetActive (false);
			Debug.Log (Mothership.vapCount);
			Invoke ("Explodes", 0.5f);

		}
	}
	void OnCollisionEnter (Collision col){

		if (col.transform.tag == "Laser") {
			//Debug.Log ("Hit");
			//		Vector3 impactPoint = col.transform.position;
			Destroy (col.gameObject);
			//			GameObject explosion = Instantiate (hitExplosion, impactPoint, transform.rotation) as GameObject;
			health -= 5;
			//Invoke ("Explodes", 0.5f);

		}
		if (col.transform.tag == "Rocket") {
			//Debug.Log ("Hit");
			Vector3 impactPoint = col.transform.position;
			Destroy (col.gameObject);
			GameObject explosion = Instantiate (hitExplosion, impactPoint, transform.rotation) as GameObject;
			health -= 20;
			//
		}
	}
	void Explodes(){
		
	}
}