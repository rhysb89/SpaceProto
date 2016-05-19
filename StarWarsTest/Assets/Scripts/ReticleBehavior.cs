using UnityEngine;
using System.Collections;

public class ReticleBehavior : MonoBehaviour {
	public Transform shootPoint;
	public float radius;
	Camera camera;

	public Transform target;
	public Transform reticle;

	public GameObject lockOnReticle;
	public GameObject notLockOn;
	public GameObject canLockOnReticle;

	public Transform fireFrom;
	//public GameObject marker;

	public bool canLockOn;
	public bool lockedOn;

	public float rayLength;

	public Transform rocketLock;

	public GameObject targetObject;

	//public Vector3 reticleCentre;

	// Use this for initialization
	void Start () {
		Screen.lockCursor = true;
		camera = GetComponent<Camera> ();
		canLockOn = false;
		lockOnReticle.SetActive (false);
		notLockOn.SetActive (true);
		canLockOnReticle.SetActive (false);

	//	reticleCentre = target.localPosition;
	}
	
	// Update is called once per frame
	void FixedUpdate () {


		Vector3 screenPos = camera.WorldToScreenPoint (target.position);
		reticle.position = screenPos;


		Vector3 fwd = fireFrom.TransformDirection (Vector3.forward);
		RaycastHit hitInfo;
		//Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		int layerMask = 1 << 8;
		//layerMask = ~layerMask;
		//canLockOn = false;

		if (Physics.SphereCast (fireFrom.position, radius, fwd, out hitInfo, rayLength, layerMask)) {
		
			targetObject = hitInfo.transform.gameObject;
//			marker.transform.position = hitInfo.transform.position;

				canLockOn = true;
				//Debug.Log ("CanLock");
			canLockOnReticle.SetActive (true);
			notLockOn.SetActive (false);

		} else {
			lockedOn = false;
			canLockOn = false;
			canLockOnReticle.SetActive (false);
		
		}


		if (!lockedOn) {
			lockOnReticle.SetActive (false);
			notLockOn.SetActive (true);
	//		target.localPosition = reticleCentre;
		}
		if (canLockOn && Input.GetAxis ("LockOn") > 0.8f ) {
			//Debug.Log ("LockedOn");
			lockedOn = true;
			rocketLock = hitInfo.transform;

		}
		if (Input.GetAxis ("LockOn") < 0.8f) {
		
			//Debug.Log ("Not LockedOn");
			//canLockOn = false;
			lockedOn = false;
			lockOnReticle.SetActive (false);
			notLockOn.SetActive (true);
			rocketLock = null;
		}
		if (!canLockOn) {
			rocketLock = null;

		}
	
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Screen.lockCursor = false;
		}
		if (lockedOn) {
			target.position = hitInfo.transform.position;
			targetObject = hitInfo.transform.gameObject;
			lockOnReticle.SetActive (true);
			notLockOn.SetActive (false);
			canLockOnReticle.SetActive (false);
		
		}

	}
}
