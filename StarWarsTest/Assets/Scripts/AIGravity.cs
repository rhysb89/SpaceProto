using UnityEngine;
using System.Collections;

public class AIGravity : MonoBehaviour {

	public Transform planet;
	public bool AlignToPlanet;
	public float gravityConstant = 9.8f;


	public Vector3 col;


	void Start () {
		AlignToPlanet = true;
	}

	void FixedUpdate () {

		//NavMeshAgent nav = GetComponent <NavMeshAgent> ();

		Rigidbody rb = GetComponent<Rigidbody> ();
		//CharacterController cont = GetComponent <CharacterController> ();
		Vector3 toCenter = planet.position - transform.position;
		toCenter.Normalize();

		//rb.AddForce(toCenter * gravityConstant, ForceMode.Acceleration);
		RaycastHit hit;
		if (Physics.Raycast (transform.position, -transform.up, out hit)) {
			//Debug.Log (hit.transform.tag);
			col = hit.normal;
		}
	

		//Vector3 col = cont.CollisionFlags.Below;
		if (AlignToPlanet)
		{
			Quaternion q = Quaternion.FromToRotation(transform.up, col);
			q = q * transform.rotation;
			transform.rotation = Quaternion.Lerp(transform.rotation, q, Time.deltaTime);
			//nav.updateRotation = true;
		}
	}
}
