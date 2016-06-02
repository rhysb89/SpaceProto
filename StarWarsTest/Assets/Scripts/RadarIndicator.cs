using UnityEngine;
using System.Collections;

public class RadarIndicator : MonoBehaviour {
	public GameObject landPlayer;
	public GameObject airPlayer;

	public GameObject missionMarker1;
	public GameObject missionMarker2;


	public bool doIt;
	public bool canDoIt;
	// Use this for initialization
	void Start () {
		landPlayer.SetActive (false);
		doIt = false;
		canDoIt = true;
	}
	
	// Update is called once per frame
	void Update () {

	
		if (doIt && canDoIt) {
			GameObject marker1 = Instantiate (missionMarker1, missionMarker1.transform.position, missionMarker1.transform.rotation) as GameObject;
			//GameObject marker2 = Instantiate (missionMarker2, missionMarker2.transform.position, missionMarker2.transform.rotation) as GameObject;
			canDoIt = false;
			doIt = false;
		}


		if (Movement.onLand) {
			landPlayer.SetActive (true);
			airPlayer.SetActive (false);
			doIt = true;


		
		} else if (!Movement.onLand) {

			landPlayer.SetActive (false);
			airPlayer.SetActive (true);


		}
	}
}
