using UnityEngine;
using System.Collections;

public class WaterTower : MonoBehaviour {
	bool inTrig;
	public GameObject point2;
	public GameObject Water;

	public static bool hasRefilled;
	// Use this for initialization
	void Start () {
		inTrig = false;
		Water.SetActive (false);
		hasRefilled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (inTrig && Input.GetButtonDown ("BoardShip")) {

			point2.GetComponent<FX_3DRadar_RID> ().DestroyThis ();
			Water.SetActive (true);
			hasRefilled = true;
			gameObject.SetActive (false);
		}
	}
	void OnTriggerEnter(Collider col){

		if (col.tag == "Player") {

			inTrig = true;
		}

	}
	void OnTriggerExit(Collider col){

		if (col.tag == "Player") {

			inTrig = false;
		}

	}
}
