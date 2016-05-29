using UnityEngine;
using System.Collections;

public class Gravity : MonoBehaviour {

	public Transform planet;
	public bool AlignToPlanet;
	public float gravityConstant = 9.8f;

	public Transform spareXWing;
	public Vector3 col;
	public Vector3 colX;

	void Start () {
		AlignToPlanet = true;
	}

	void FixedUpdate () {
		Rigidbody rb = GetComponent<Rigidbody> ();
		CharacterController cont = GetComponent <CharacterController> ();
		Vector3 toCenter = planet.position - transform.position;
		toCenter.Normalize();

		//rb.AddForce(toCenter * gravityConstant, ForceMode.Acceleration);
		RaycastHit hit;
		if (Physics.Raycast (transform.position, -transform.up, out hit)) {
			//Debug.Log (hit.transform.tag);
			col = hit.normal;
		}
		RaycastHit hitX;
		if (Physics.Raycast (spareXWing.transform.position, -spareXWing.transform.up, out hitX)) {
			//Debug.Log (hit.transform.tag);
			colX = hitX.normal;
		}

		//Vector3 col = cont.CollisionFlags.Below;
		if (AlignToPlanet)
		{
			Quaternion q = Quaternion.FromToRotation(transform.up, col);
			q = q * transform.rotation;
			transform.rotation = Quaternion.Lerp(transform.rotation, q, Time.deltaTime);

			Quaternion qx = Quaternion.FromToRotation(spareXWing.transform.up, colX);
			qx = qx * spareXWing.transform.rotation;
			spareXWing.transform.rotation = Quaternion.Slerp(spareXWing.transform.rotation, qx, 1);
		}
	}
}
