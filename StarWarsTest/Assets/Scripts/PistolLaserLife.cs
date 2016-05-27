using UnityEngine;
using System.Collections;

public class PistolLaserLife : MonoBehaviour {
	public float lifetime;
	public GameObject hitExplosion;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		Destroy (gameObject, lifetime);

	}
	void OnCollisionEnter(Collision col){

		ContactPoint point = col.contacts[0];
		Vector3 impactPoint = point.point;

		if (col.transform.tag != "Planet") {
			if (col.transform.tag != "Laser" && col.transform.tag != "Player") {
				GameObject explosion = Instantiate (hitExplosion, impactPoint, transform.rotation) as GameObject;

				if (col.transform.tag != "Enemy") {
					Debug.Log (col.transform.tag);
					Destroy (gameObject);
				}

			}
		}
	}
}
