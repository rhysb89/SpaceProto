using UnityEngine;
using System.Collections;

public class RocketMovement : MonoBehaviour {
	public float speed;
	Transform target;
	public float damping = 1.0f;


	// Use this for initialization
	void Start () {

		ReticleBehavior reticleBehaviour;
		reticleBehaviour = Camera.main.GetComponent<ReticleBehavior> ();

		target = reticleBehaviour.rocketLock;
	}
	
	// Update is called once per frame
	void Update () {

	
		if (target != null) {

			transform.Translate (Vector3.forward * Time.deltaTime * speed);

			var rotation = Quaternion.LookRotation (target.position - transform.position);
			transform.rotation = Quaternion.Slerp (transform.rotation, rotation, Time.deltaTime * damping);
		} 
	}
}
