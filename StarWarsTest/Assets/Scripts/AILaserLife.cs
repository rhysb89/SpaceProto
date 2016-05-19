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
	void OnTriggerEnter(Collider col){


		if (col.tag != "EnemyLaser") {
			//if (col.tag != "Player") {
				//			Debug.Log (col.tag);
				Destroy (gameObject);
			//}

		}
	}
}
