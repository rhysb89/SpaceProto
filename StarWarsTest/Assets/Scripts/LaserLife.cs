using UnityEngine;
using System.Collections;

public class LaserLife : MonoBehaviour {
	public float lifetime;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Destroy (gameObject, lifetime);

	}
	void OnTriggerEnter(Collider col){


		if (col.tag != "Laser") {
			if (col.tag != "Enemy") {
//			Debug.Log (col.tag);
				Destroy (gameObject);
			}

		}
	}
}
