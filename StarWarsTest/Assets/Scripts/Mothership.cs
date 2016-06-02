using UnityEngine;
using System.Collections;

public class Mothership : MonoBehaviour {
	public static int vapCount;
	public GameObject cannons;


	public bool hasTarget = false;
	// Use this for initialization
	void Start () {
		vapCount = 0;
		cannons.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (vapCount >= 3) {
		
		}
	}
	void OnTriggerEnter (Collider col){
	
		if (col.tag == "Player") {
			hasTarget = true;
			cannons.SetActive (true);
		}
	}
	void OnTriggerExit (Collider col){

		if (col.tag == "Player") {
			hasTarget = false;
			cannons.SetActive (false);
		}
	}

}

