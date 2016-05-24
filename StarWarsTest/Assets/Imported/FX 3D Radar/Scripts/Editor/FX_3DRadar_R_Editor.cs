using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(FX_3DRadar_RID))]

public class FX_3DRadar_R_Editor : Editor {

	FX_3DRadar_RID t;
	FX_Class_Mgr FXCM;
    FX_3DRadar_Mgr FX3DRM;
    FX_Faction_Mgr FXFM;
	bool setup;
    bool[] srh = new bool[10];

    Color MyBlue = new Color(0f, 0.75f, 1f, 1f);

	public override void OnInspectorGUI(){

		EditorGUILayout.Space ();
        Components();
        if(t.RIDType == FX_3DRadar_RID.ridType.RadarID) {
            EditorGUILayout.Space ();
		    ClassSelecetion();
		    EditorGUILayout.Space ();
		    FactionSelection();
		    EditorGUILayout.Space ();
		    StateCondition();
            EditorGUILayout.Space ();
            TaregtDetection();
            EditorGUILayout.Space ();
            ThisHUDID();
            EditorGUILayout.Space ();
            ThisBounds();
		    EditorGUILayout.Space ();
            BlindRadar();
            EditorGUILayout.Space ();
            LocalRadar();
            EditorGUILayout.Space ();
        }
	}

	void OnEnable(){
		t = (FX_3DRadar_RID)target;
        if(PrefabUtility.GetPrefabType(t) == PrefabType.PrefabInstance){
            PrefabUtility.DisconnectPrefabInstance(t.gameObject);
        }

		if(!setup){
            if(GameObject.Find("_GameMgr")) {
                FX3DRM = GameObject.Find("_GameMgr").GetComponent<FX_3DRadar_Mgr>();
                FXCM = GameObject.Find("_GameMgr").GetComponent<FX_Class_Mgr>();
			    FXFM = GameObject.Find("_GameMgr").GetComponent<FX_Faction_Mgr>();
            }


			if(FXFM && t.ThisFaction[0] > FXFM.Factions.Length - 1){
				t.ThisFaction[0] = 0;
			}
			setup = true;
		}

        if(t.ThisRenderer.Count == 0) {
            t.ThisRenderer.Add(new Renderer());
        }
        MarkDirty();
	}

    void MarkDirty() {
		 if(GameObject.Find("_GameMgr")) {
            Transform t = GameObject.Find("_GameMgr").transform;
            Undo.ClearUndo(t);
            t.position = Vector3.one;
            Undo.RecordObject (t, "Zero Transform Position");
            t.position = Vector3.zero;
        }
    }

    void Components() {
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
        EditorGUILayout.LabelField("ForceX 3D Radar v. Beta 2c", EditorStyles.boldLabel);
        EditorGUILayout.Space ();
        EditorGUILayout.Space ();
        EditorGUILayout.LabelField("* = Set at Start() or Awake() only", EditorStyles.boldLabel);
        EditorGUILayout.Space ();
        EditorGUILayout.Space ();

        Header("* Core Components");

        GUI.color = MyBlue;
        EditorGUILayout.LabelField("* Radar ID Type", EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.Space ();
        t.RIDType = (FX_3DRadar_RID.ridType)EditorGUILayout.EnumPopup("", t.RIDType);
        EditorGUILayout.Space ();

        if(t.RIDType == FX_3DRadar_RID.ridType.RadarID) {
            GUI.color = MyBlue;
            EditorGUILayout.LabelField("Renderers", EditorStyles.boldLabel);


            for(int i = 0; i < t.ThisRenderer.Count; i++) {
                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(180));

                if (!t.ThisRenderer[i]){
                    GUI.color = Color.red;
                }

		         t.ThisRenderer[i] = (Renderer)Object("Renderer" + " (" + i.ToString() + ")", t.ThisRenderer[i], typeof(Renderer), true);
            
                if(i > 0) {
                    if (GUILayout.Button ("-", GUILayout.MaxWidth (20),GUILayout.MinWidth (20), GUILayout.MaxHeight (20))) {
                        t.ThisRenderer.RemoveAt(i);
                        break;
                    }
                }else{
                    if (GUILayout.Button ("+", GUILayout.MaxWidth (20),GUILayout.MinWidth (20), GUILayout.MaxHeight (20))) {
                        t.ThisRenderer.Add(new Renderer());
                    }
                }
		        EditorGUILayout.EndHorizontal();
            }

           EditorGUILayout.Space();

            GUI.color = MyBlue;
            EditorGUILayout.LabelField("Rigidbody (Optional)", EditorStyles.boldLabel);
  		    EditorGUILayout.BeginHorizontal();
            t.ThisRigidbody = (Rigidbody)Object("Rigidbody", t.ThisRigidbody, typeof(Rigidbody), true);
		    EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUI.color = MyBlue;
            EditorGUILayout.LabelField("Target List Button Override", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("    -----  Assign GameObject  -----", EditorStyles.boldLabel);
            EditorGUILayout.Space ();
            GUI.color = Color.white;
             if(FX3DRM) {
   		    EditorGUILayout.BeginHorizontal();
       
                if(t.ThisButton || FX3DRM.DefaultListButton && !t.ThisButton) {
                    GUI.color = Color.green;
                }
                EditorGUILayout.LabelField("Button", GUILayout.MaxWidth(100));
                GUI.color = Color.white;
                if(!FX3DRM.DefaultListButton && !t.ThisButton) {
                    GUI.color = Color.red;
                }

		        t.ThisButton = (RectTransform)EditorGUILayout.ObjectField("",t.ThisButton, typeof(RectTransform),true,GUILayout.MaxWidth(100));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space ();
       
                if(FX3DRM.DefaultListButton && !t.ThisButton) {
                    EditorGUILayout.BeginVertical("HelpBox");
                    GUI.color = Color.green;
                    EditorGUILayout.LabelField("              Using Default Button");
                    GUI.color = Color.white;
                    EditorGUILayout.EndVertical();
                }
                if(t.ThisButton) {
                    EditorGUILayout.BeginVertical("HelpBox");
                    GUI.color = Color.green;
                    EditorGUILayout.LabelField("              Using Override Button");
                    GUI.color = Color.white;
                    EditorGUILayout.EndVertical();
                }
                if(!FX3DRM.DefaultListButton && !t.ThisButton) {
                    EditorGUILayout.BeginVertical("HelpBox");
                    GUI.color = Color.red;
                    EditorGUILayout.LabelField("              No Button Assigned");
                    GUI.color = Color.white;
                    EditorGUILayout.EndVertical();
                }
            } else {
                Warning("      No FX_3DRadar_Mgr Detected");
            }
        } else {
            NAV();
        }
		Footer();
    }

    void NAV() {
        GUI.color = MyBlue;
        EditorGUILayout.LabelField("NAV Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space ();
        GUI.color = Color.white;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("NAV Arrival",GUILayout.MaxWidth(100));
        t.NAVArrive = (FX_3DRadar_RID.navArrive)EditorGUILayout.EnumPopup("", t.NAVArrive,GUILayout.MaxWidth(90));
        EditorGUILayout.EndHorizontal();
        t.ArrivalDist = Float("Arrival Distance",t.ArrivalDist);

        EditorGUILayout.Space ();
        GUI.color = MyBlue;
        EditorGUILayout.LabelField("NAV Status", EditorStyles.boldLabel);
        EditorGUILayout.Space ();
        GUI.color = Color.white;

        if(!t.IsActiveNAV && GUILayout.Button("Set NAV Active",GUILayout.MaxWidth(110),GUILayout.MaxHeight(20))){
            t.SetNAVActive();
        }
        if(t.IsActiveNAV && GUILayout.Button("Set NAV Inactive",GUILayout.MaxWidth(110),GUILayout.MaxHeight(20))){
            t.SetNAVInactive();
        }
                EditorGUILayout.Space ();
        if(t.IsActiveNAV) {
            EditorGUILayout.BeginVertical("HelpBox");
            GUI.color = Color.green;
            EditorGUILayout.LabelField("                    NAV Is Active");
            GUI.color = Color.white;
            EditorGUILayout.EndVertical();
        } else {
            EditorGUILayout.BeginVertical("HelpBox");
            GUI.color = Color.white;
            EditorGUILayout.LabelField("                  NAV Is Inactive");
            GUI.color = Color.white;
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.Space ();
        EditorGUI.indentLevel = 1;
        GUI.color = Color.green;
        srh[2] = EditorGUILayout.Foldout(srh[2], "Scripting Reference");
            if(srh[2]) {
            GUI.color = MyBlue;
            EditorGUILayout.LabelField("NAV Arrival (enum)");
            GUI.color = Color.white;
            EditorGUILayout.LabelField("FX_3DRadar_RID.NAVArrive");

            GUI.color = MyBlue;
            EditorGUILayout.LabelField("Arrival Distance (float)");
            GUI.color = Color.white;
            EditorGUILayout.LabelField("FX_3DRadar_RID.ArrivalDist");

            GUI.color = MyBlue;
            EditorGUILayout.LabelField("Set NAV Active (void)");
            GUI.color = Color.white;
            EditorGUILayout.LabelField("FX_3DRadar_RID.SetNAVActive()");

            GUI.color = MyBlue;
            EditorGUILayout.LabelField("Set NAV Inactive (void)");
            GUI.color = Color.white;
            EditorGUILayout.LabelField("FX_3DRadar_RID.SetNAVInactive()");

        }
    }

	void FactionSelection(){
        Header("Faction Selection");
		if(FXFM) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Faction", GUILayout.MaxWidth(100));
			t.ThisFaction[0] = EditorGUILayout.Popup(t.ThisFaction[0], FXFM.Factions, GUILayout.MaxWidth(120));
			EditorGUILayout.EndHorizontal();	
		
		    EditorGUILayout.LabelField("Faction ID :	" + t.ThisFactionID.ToString());
        } else {
            Warning("        No FX_Faction_Mgr Detected");
        }
		Footer();
	}

	void ClassSelecetion(){
        Header("Class Selection");
        if(FXCM) {
		    if(t.MainClass[0] > FXCM.ObjectClassList.Count - 1){
			    t.MainClass[0] = 0;
		    }
			string[] mc = new string[FXCM.ObjectClassList.Count];
			for(int a = 0; a < FXCM.ObjectClassList.Count; a++){
				mc[a] = FXCM.ObjectClassList[a].ClassName;
			}
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Main Class", GUILayout.MaxWidth(100));
			t.MainClass[0] = EditorGUILayout.Popup(t.MainClass[0], mc, GUILayout.MaxWidth(120));
			EditorGUILayout.EndHorizontal();



		    if(t.SubClass[0] > FXCM.ObjectClassList[t.MainClass[0]].SubClassName.Count - 1){
			    t.SubClass[0] = 0;
		    }

            if(FXCM.ObjectClassList[t.MainClass[0]].SubClassName.Count > 1) {
				string[] sc = new string[FXCM.ObjectClassList[t.MainClass[0]].SubClassName.Count];
				for(int a = 0; a < FXCM.ObjectClassList[t.MainClass[0]].SubClassName.Count; a++){
					sc[a] = FXCM.ObjectClassList[t.MainClass[0]].SubClassName[a];
				}
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Sub Class", GUILayout.MaxWidth(100));
				t.SubClass[0] = EditorGUILayout.Popup(t.SubClass[0], sc, GUILayout.MaxWidth(120));
				EditorGUILayout.EndHorizontal();
            } else {
                EditorGUILayout.LabelField("No Sub Class");
            }
		    EditorGUILayout.Space ();

		    DrawOnGUISprite (FXCM.ObjectClassList[t.MainClass[0]].ClassSprite[t.SubClass[0]]);
        } else {
            Warning("         No FX_Class_Mgr Detected");
        }
		Footer();
	}

	void DrawOnGUISprite(Sprite aSprite){
		EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(10));
		EditorGUILayout.LabelField("Class ID : ",GUILayout.MaxWidth(100));
		Rect c = aSprite.rect;
		Rect rect = GUILayoutUtility.GetRect(10, 10);
		rect.y += 7;

		if (Event.current.type == EventType.Repaint){
			Texture tex = aSprite.texture;
			c.xMin /= tex.width;
			c.xMax /= tex.width;
			c.yMin /= tex.height;
			c.yMax /= tex.height;

			GUI.DrawTextureWithTexCoords(rect, tex, c);

		}
		EditorGUILayout.EndHorizontal();
	}

	void StateCondition(){
        Header("State");

		EditorGUILayout.LabelField("IFF Status : " + t.IFFStatus.ToString());

		if(t.IsObjective[0]){
			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("R",GUILayout.MaxWidth(18),GUILayout.MaxHeight(18))){
				t.ResetObjectiveStatus();
			}
			EditorGUILayout.LabelField("Is Objective : Yes  (" + t.ObjectiveType.ToString() + ")",GUILayout.MaxWidth(220));
			EditorGUILayout.EndHorizontal();
		}else{
			EditorGUILayout.LabelField("Is Objective : No");
		}


		if(t.IsPlayerTarget){
			EditorGUILayout.LabelField("Player Target : Yes");
		}else{
			EditorGUILayout.LabelField("Player Target : No");
		}
		EditorGUILayout.Space ();

         t.IsPOI = Toggle("* Is POI", t.IsPOI);
		
        t.IsPlayer = Toggle("Is Local Player", t.IsPlayer);
		EditorGUILayout.Space ();

        t.IsPlayerOwned[0] = Toggle("Is Player Owned", t.IsPlayerOwned[0]);
        t.IsAbandoned[0] = Toggle("Is Abandoned", t.IsAbandoned[0]);

		EditorGUILayout.Space ();

		GUI.color = new Color(.75f,0,0,1);
		if(t.RIDI && GUILayout.Button("Destroy This",GUILayout.MaxWidth(100),GUILayout.MaxHeight(18))){
			t.DestroyThis();
		}
		GUI.color = Color.white;

        EditorGUILayout.Space ();
        EditorGUI.indentLevel = 1;
        GUI.color = Color.green;
        srh[1] = EditorGUILayout.Foldout(srh[1], "Scripting Reference");
            if(srh[1]) {
            GUI.color = MyBlue;
            EditorGUILayout.LabelField("Is Local Player (bool)");
            GUI.color = Color.white;
            EditorGUILayout.LabelField("FX_3DRadar_RID.IsPlayer");

            GUI.color = MyBlue;
            EditorGUILayout.LabelField("Is Player Owned (bool[])");
            GUI.color = Color.white;
            EditorGUILayout.LabelField("FX_3DRadar_RID.IsPlayerOwned[0]");

            GUI.color = MyBlue;
            EditorGUILayout.LabelField("Is Abandoned (bool[])");
            GUI.color = Color.white;
            EditorGUILayout.LabelField("FX_3DRadar_RID.IsAbandoned[0]");

            GUI.color = MyBlue;
            EditorGUILayout.LabelField("Destroy This (Void)");
            GUI.color = Color.white;
            EditorGUILayout.LabelField("FX_3DRadar_RID.DestroyThis()");
        }
		Footer();
	}

    void TaregtDetection() {
        Header("Targeting & Detection Settings");

        t.IsTargetable = Toggle("Is Targetable", t.IsTargetable);
        t.IsDetectable = Toggle("Is Detectable", t.IsDetectable);
        t.IsDiscovered = Toggle("Is Discovered", t.IsDiscovered);
        t.DetectionReset = Toggle("Detection Reset", t.DetectionReset);

		Footer();
    }

    void ThisHUDID() {
        Header("This HUD ID Settings");

        t.DisplayHUDID = Toggle("* Display This ID", t.DisplayHUDID);

		if(t.DisplayHUDID){
            t.HUDIDOnScreen = Toggle("Show On Screen", t.HUDIDOnScreen);
			EditorGUILayout.Space ();
            if(t.HUDIDOnScreen) {
			    t.HUDIDShow = (FX_3DRadar_RID._boundsShow)EditorGUILayout.EnumPopup("Display Condition",t.HUDIDShow);
            }

            EditorGUILayout.Space ();
            EditorGUILayout.BeginVertical("HelpBox");
            GUI.color = Color.green;
            if(!t.HUDIDOnScreen) {
                EditorGUILayout.LabelField("   Display Screen Edge (Off Screen)");
            } else {
                EditorGUILayout.LabelField("    Display Full Screen (On Screen)");
                EditorGUILayout.LabelField("   Display Screen Edge (Off Screen)");
            }
            GUI.color = Color.white;
            EditorGUILayout.EndVertical();
		}

		Footer();
    }

    void ThisBounds() {
        Header("This Bounds Settings");

        t.DisplayBounds = Toggle("Display This Bounds", t.DisplayBounds);

		EditorGUILayout.Space ();
		if(t.DisplayBounds){
			t.BoundsShow = (FX_3DRadar_RID._boundsShow)EditorGUILayout.EnumPopup("Display Condition",t.BoundsShow);
		}
        Footer();
    }

    void BlindRadar() {
        Header("Blind Radar Settings");
        if(FX3DRM) {
            if(FX3DRM.EnableBlindRadar) {
                t.BlindRadarOverride = Toggle("Radar Override", t.BlindRadarOverride);
                EditorGUILayout.Space ();
            }

            
            EditorGUILayout.BeginVertical("HelpBox");
            GUI.color = Color.green;
            if(FX3DRM.EnableBlindRadar) {
                if(t.BlindRadarOverride) {
                    GUI.color = new Color(2f,.3f,0f,1f);
                    EditorGUILayout.LabelField("           Will Ignore Blind Radar");
                } else {
                    EditorGUILayout.LabelField("             Will Use Blind Radar");
                }
            } else {
            GUI.color = Color.white;
            EditorGUILayout.LabelField("           Blind Radar Is Disabled");
        }
            GUI.color = Color.white;
            EditorGUILayout.EndVertical();
        } else {
            Warning("      No FX_3DRadar_Mgr Detected");
        }
        Footer();
    }

    void LocalRadar() {
        Header("Local Radar");

        t.EnableLocalRadar = Toggle("Enable Local Radar", t.EnableLocalRadar);
        if(t.EnableLocalRadar) {
            EditorGUILayout.Space ();
            t.RadarRange = Float("Radar Range", t.RadarRange);
            t.UpdateTime = Float("Refresh (sec)", t.UpdateTime);

            EditorGUILayout.Space ();
            EditorGUILayout.LabelField("Target Count :  " + t.HostileList.Count.ToString(), GUILayout.MaxWidth(150));
        }
            EditorGUILayout.Space ();
            EditorGUI.indentLevel = 1;
            GUI.color = Color.green;
            srh[0] = EditorGUILayout.Foldout(srh[0], "Scripting Reference");
             if(srh[0]) {
                GUI.color = MyBlue;
                EditorGUILayout.LabelField("Hostile List (List<Transform>)");
                GUI.color = Color.white;
                EditorGUILayout.LabelField("FX_3DRadar_RID.HostileList");

                GUI.color = MyBlue;
                EditorGUILayout.LabelField("Enable Local Radar (bool)");
                GUI.color = Color.white;
                EditorGUILayout.LabelField("FX_3DRadar_RID.EnableLocalRadar"); 
                         
                GUI.color = MyBlue;
                EditorGUILayout.LabelField("Radar Range (Float)");
                GUI.color = Color.white;
                EditorGUILayout.LabelField("FX_3DRadar_RID.RadarRange");
                           
                GUI.color = MyBlue;
                EditorGUILayout.LabelField("Radar Refresh (Float)");
                GUI.color = Color.white;
                EditorGUILayout.LabelField("FX_3DRadar_RID.UpdateTime");
            }
        Footer();
    }

    int Int(string s, int o){
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(s, GUILayout.MaxWidth(100), GUILayout.MinWidth(100));
		o = EditorGUILayout.IntField("", o, GUILayout.MaxWidth(82));
		EditorGUILayout.EndHorizontal();
        return o;
    }

    float Float(string s, float o){
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(s, GUILayout.MaxWidth(100), GUILayout.MinWidth(100));
		o = EditorGUILayout.FloatField("", o, GUILayout.MaxWidth(82));
		EditorGUILayout.EndHorizontal();
        return o;
    }

    object Object(string s, Object o, System.Type type, bool Error ) {
		EditorGUILayout.BeginHorizontal();
        GUI.color = Color.white;
		EditorGUILayout.LabelField(s, GUILayout.MaxWidth(100), GUILayout.MinWidth(100));
        if(Error && !o){
			GUI.color = Color.red;
		}
        o = EditorGUILayout.ObjectField("",o, type,true,GUILayout.MaxWidth(100),GUILayout.MinWidth(100));
		EditorGUILayout.EndHorizontal();
        GUI.color = Color.white;
        return o;
    }

    bool Toggle(string s, bool o) {
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(s, GUILayout.MaxWidth(167), GUILayout.MinWidth(167));
		o = EditorGUILayout.Toggle("", o, GUILayout.MaxWidth(20));
		EditorGUILayout.EndHorizontal();
        return o;
    }

    int IntSlider(string s, int o, int range) {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(s,GUILayout.MaxWidth(110) );
        o = EditorGUILayout.IntSlider("", o, 0, range,GUILayout.MaxWidth(115) );
        EditorGUILayout.EndHorizontal();
        return o;
    }

    void Warning(string s) {
        GUI.color = Color.red;
        EditorGUILayout.BeginVertical("HelpBox");
        EditorGUILayout.LabelField(s);
        EditorGUILayout.EndVertical();
        GUI.color = Color.white;
    }

    void Header(string s) {
		EditorGUI.indentLevel = 0;
		EditorGUILayout.Space ();
        GUI.color = new Color(1f, 0.75f, 0f, 1f);
		EditorGUILayout.LabelField(s, EditorStyles.boldLabel);
        GUI.color = Color.gray;
		EditorGUILayout.BeginVertical ("HelpBox", GUILayout.MaxWidth(245),GUILayout.MinWidth(245));
		EditorGUILayout.Space ();
        GUI.color = Color.white;
    }

    void Footer() {
		EditorGUILayout.Space ();
		EditorGUILayout.EndVertical();
    }
}
