using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StationTrig : MonoBehaviour {

	bool inTrig;
	bool firstVisit;
	bool missionComplete;
	public GameObject missionText;
	public GameObject missionText2;
	public static bool missionStarted;
	public static int killCount;

	public GameObject point1;

	public Text killCountText;
	public Text killCountTextEnd;
	public GameObject count;


	// Use this for initialization
	void Start () {
		inTrig = false;
		killCount = 0;
		firstVisit = true;
		missionText.SetActive (false);
		missionText2.SetActive (false);
		missionStarted = false;
		missionComplete = false;
		count.SetActive (false);


	}

	// Update is called once per frame
	void Update () {
		if (inTrig && firstVisit) {
			missionStarted = true;
			missionText.SetActive (true);
			count.SetActive (true);
		
			Invoke ("Tut", 5f);
		}
		if (killCount >= 8 && !missionComplete) {
			missionStarted = false;
			missionText2.SetActive (true);
			killCountTextEnd.color = Color.green;
			killCountText.color = Color.green;
			Invoke ("End", 5f);
		}
		killCountText.text = killCount.ToString();

	}
	void OnTriggerEnter (Collider col){
		if (col.tag == "Player") {
			inTrig = true;

		}
	}
	void OnTriggerExit(Collider col){
		if (col.tag == "Player") {
			inTrig = false;

		}
	}
	void Tut(){
		point1.SetActive (false);
		missionText.SetActive (false);
		firstVisit = false;
	}
	void End(){
		missionText2.SetActive (false);
		count.SetActive (false);
	
		missionComplete = true;
	}
}
