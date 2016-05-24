using UnityEngine;
using System.Collections;

public class LandMovement : MonoBehaviour {
	public Transform gravityBox;
	public float gravity;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		Rigidbody rb = GetComponent<Rigidbody> ();

		rb.AddForce((gravityBox.position - transform.position).normalized * gravity);

	}
}
