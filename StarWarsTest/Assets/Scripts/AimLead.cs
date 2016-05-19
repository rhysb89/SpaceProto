using UnityEngine;
using System.Collections;

public class AimLead : MonoBehaviour {

	Transform target;
	float speed;

	public Laser laserSpeed;

	// Use this for initialization
	void Start () {
		ReticleBehavior reticleBehaviour;
		reticleBehaviour = Camera.main.GetComponent<ReticleBehavior> ();

		target = reticleBehaviour.rocketLock;

		laserSpeed = GetComponent<Laser> ();

		speed = laserSpeed.fireSpeed;
	}
	
	// Update is called once per frame
	void Update () {


	
	}
}
