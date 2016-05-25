using UnityEngine;
using System.Collections;

public class Gravity : MonoBehaviour {

	public Transform planet;
	public bool AlignToPlanet;
	public float gravityConstant = 9.8f;

	public Transform spareXWing;

	void Start () {
		AlignToPlanet = true;
	}

	void FixedUpdate () {
		Rigidbody rb = GetComponent<Rigidbody> ();

		Vector3 toCenter = planet.position - transform.position;
		toCenter.Normalize();

		rb.AddForce(toCenter * gravityConstant, ForceMode.Acceleration);

		if (AlignToPlanet)
		{
			Quaternion q = Quaternion.FromToRotation(transform.up, -toCenter);
			q = q * transform.rotation;
			transform.rotation = Quaternion.Slerp(transform.rotation, q, 1);

			Quaternion qx = Quaternion.FromToRotation(spareXWing.transform.up, -toCenter);
			qx = qx * spareXWing.transform.rotation;
			spareXWing.transform.rotation = Quaternion.Slerp(spareXWing.transform.rotation, qx, 1);
		}
	}
}
