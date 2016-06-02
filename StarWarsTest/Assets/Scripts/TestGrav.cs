using UnityEngine;
using System.Collections;

public class TestGrav : MonoBehaviour {


	public Transform planet;
	public bool AlignToPlanet;
	public float gravityConstant = 20f;

	//public Transform spareXWing;
	//public Vector3 col;
	//public Vector3 colX;

	void Start () {
		AlignToPlanet = true;
	}

	void FixedUpdate () {
		Rigidbody rb = GetComponent<Rigidbody> ();
//		CharacterController cont = GetComponent <CharacterController> ();
		Vector3 toCenter = planet.position - transform.position;
		toCenter.Normalize();

		rb.AddForce(toCenter * gravityConstant, ForceMode.Acceleration);
	
		if (AlignToPlanet)
		{
			Quaternion q = Quaternion.FromToRotation(transform.up, -toCenter);
			q = q * transform.rotation;
			transform.rotation = Quaternion.Lerp(transform.rotation, q, Time.deltaTime);

		
		}
	}
}
