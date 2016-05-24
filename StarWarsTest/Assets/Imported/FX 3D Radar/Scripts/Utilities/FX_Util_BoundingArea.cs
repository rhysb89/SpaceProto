using UnityEngine;
using System.Collections;

public class FX_Util_BoundingArea : MonoBehaviour {
	static public Bounds GetBoundingArea (Transform o) {
		Vector3 Center = Vector3.zero;

		foreach(Transform tr in o.GetComponentsInChildren<Transform>()){
			if(tr != o){
				Center += tr.position;
			}
		}
		
		Center = Center / o.childCount;
		Bounds ThisBounds = new Bounds(Center, Vector3.zero);
		
		foreach(Renderer r in o.GetComponentsInChildren<Renderer>()){
			if(r != o.GetComponent<Renderer>()){
				ThisBounds.Encapsulate(r.bounds);
			}
		}
		return ThisBounds;
	}


}
