using UnityEngine;
using System.Collections;

public class AILaserLife : MonoBehaviour {
	public float lifetime;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		Destroy (gameObject, lifetime);

	}
	void OnCollisionEnter(Collision col){


		if (col.transform.tag != "EnemyLaser") {
			if (col.transform.tag != "Enemy") {
				
				Destroy (gameObject);
			}

		}
	}
}
