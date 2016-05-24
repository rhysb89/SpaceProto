using UnityEngine;
using System.Collections;

public class AITrig : MonoBehaviour {
	public bool inTrig;




	// Use this for initialization
	void Start () {
		inTrig = false;



	}
	
	// Update is called once per frame
	void Update () {

	}
	void OnTriggerEnter (Collider col){
		if (col.tag == "Player") {
			inTrig = true;
		
		}
	}
	void OnTriggerExit(Collider col){
		if (col.tag == "Player") {
			//inTrig = false;

		}
	}
}
