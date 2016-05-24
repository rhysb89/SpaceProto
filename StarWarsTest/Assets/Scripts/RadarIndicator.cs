using UnityEngine;
using System.Collections;

public class RadarIndicator : MonoBehaviour {
	public GameObject landPlayer;
	public GameObject airPlayer;
	// Use this for initialization
	void Start () {
		landPlayer.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Movement.onLand) {
			landPlayer.SetActive (true);
			airPlayer.SetActive (false);
		}
		else if (!Movement.onLand) {

			landPlayer.SetActive (false);
			airPlayer.SetActive (true);
		}
	}
}
