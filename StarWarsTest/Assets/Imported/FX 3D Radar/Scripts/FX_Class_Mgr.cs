using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FX_Class_Mgr : MonoBehaviour {

	[System.Serializable]
	public class objectClassList{
		public string ClassName = "New Class";
		public List<string> SubClassName = new List<string>(1);
		public List<Sprite> ClassSprite = new List<Sprite>(1);
		public List<Vector3> RIDOffset = new List<Vector3>(1);
		public List<Vector2> TSIOffset = new List<Vector2>(1);
		public List<Vector2> HUDOffset = new List<Vector2>(1);
	
		//Editor Show / Hide Offset Values
		public List<bool> Toggle = new List<bool>(1); 
	}
	
	public List<objectClassList> ObjectClassList = new List<objectClassList>(1);
	public Vector2 IndicatorSize; // The manual entry for the size of the Radar / HUD Target Selection indicator.
}