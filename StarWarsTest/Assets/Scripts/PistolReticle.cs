using UnityEngine;
using System.Collections;

public class PistolReticle : MonoBehaviour {

	public Camera camera;
	public Transform target;
	public Transform reticle;

	public float yMin;
	public float yMax;



	// Use this for initialization
	void Start () {
		//camera = GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 screenPos = camera.WorldToScreenPoint (target.position);
		reticle.position = screenPos;


		float rotSpeed = 3.0f;
		//float xRot = rotSpeed * Input.GetAxis("Vertical");
		float yRot = rotSpeed * Input.GetAxis("Vertical");
		yRot = Mathf.Clamp(yRot,yMin,yMax);
		//transform.rotation = Quaternion.identity;
		//transform.localEulerAngles = new Vector3 ( Mathf.Clamp (transform.rotation.y, cMin, cMax), 0,0);

		transform.Rotate  (yRot, 0, 0.0f);
	}
}