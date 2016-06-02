using UnityEngine;
using System.Collections;

public class PlanetMission : MonoBehaviour {
	public bool inTrig;
	public bool firstVisit;
	public bool hasStarted;
	public GameObject point1;
	public GameObject point1Clone;
	public GameObject point2;

	public GameObject missionText;
	public GameObject missionText2;

	// Use this for initialization
	void Start () {
		inTrig = false;
		firstVisit = true;
		missionText.SetActive (false);
		missionText2.SetActive (false);
		hasStarted = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (PlanetMissionTrig.inTrig && Input.GetButtonDown ("BoardShip") && firstVisit) {
			hasStarted = true;
			//point1.GetComponent<FX_3DRadar_RID> ().DestroyThis ();

			point2.SetActive (true);
		
			missionText.SetActive (true);



		}
		if (WaterTower.hasRefilled && PlanetMissionTrig.inTrig && Input.GetButtonDown ("BoardShip")) {

			missionText2.SetActive (true);

			Invoke ("WaitSecond", 5f);


		}
		if (hasStarted) {
			Invoke ("Wait", 5f);
		}

	
	}

	public void Wait(){

		Debug.Log ("WaitOne");
		firstVisit = false;
		missionText.SetActive (false);

	}
	public void WaitSecond(){
		missionText2.SetActive (false);
		point1.GetComponent<FX_3DRadar_RID> ().DestroyThis ();
	
		//gameObject.SetActive (false);
	}
}
