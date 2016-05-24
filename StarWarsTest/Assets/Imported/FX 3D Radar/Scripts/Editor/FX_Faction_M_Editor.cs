using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(FX_Faction_Mgr))]
public class FX_Faction_M_Editor : Editor {

	FX_Faction_Mgr t;
	int FactionCNT;
	int NumberOfFactions;
	int UniqueRelations;

     Color MyBlue = new Color(0f, 0.75f, 1f, 1f);

	public override void OnInspectorGUI(){
		Checker();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		MenuSelection();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		CheckFactionSize();

		switch((int)t.FMSetup){
		case 0:
			FactionRelationValues();
			EditorGUILayout.Space ();
			FactionSetup ();
			break;

		case 1:
			SetPlayerFaction();
			EditorGUILayout.Space ();
			SetPlayerRelations();
			break;

		case 2:
			SetFactionRelations();
			break;
		}
	}

	void OnEnable(){
		t = (FX_Faction_Mgr)target;
		FactionCNT = t.Factions.Length;
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

	void Checker(){
		if(t.PlayerFaction > t.Factions.Length){
			t.PlayerFaction = 0;
		}
	}

	void MenuSelection(){
        Header("Menu Selection");

		t.FMSetup = (FX_Faction_Mgr._fmSetup)EditorGUILayout.EnumPopup("", t.FMSetup);

		Footer();
	}

	void CheckFactionSize(){
		if(FactionCNT != t.Factions.Length){
			System.Array.Resize(ref t.Factions, FactionCNT);
			System.Array.Resize(ref t.FactionID, FactionCNT);
			for(int n = 0; n < t.Factions.Length; n++){
				if(t.Factions[n] == null){
					t.Factions[n] = "";
				}
			}
		}

		if (t.PlayerRelations.Length != t.Factions.Length){//resize the array
			System.Array.Resize(ref t.PlayerRelations, t.Factions.Length);
		}
		
		NumberOfFactions = t.Factions.Length;
		UniqueRelations = (int)((NumberOfFactions - 1)  * (NumberOfFactions * 0.5f));

		if(UniqueRelations != t.StartRelations.Length){
			System.Array.Resize(ref t.StartRelations, UniqueRelations);
		}
	}

	void FactionSetup(){
        Header("Number Of Factions (2-32)");

		FactionCNT = IntSlider( "Factions: ", FactionCNT, 2, 32);

		Footer();

        EditorGUILayout.Space ();

        Header("Faction Names");

		for (int i = 0; i < t.Factions.Length; i++){//display  all the elements of the array
			EditorGUILayout.Space ();
			t.Factions[i] = EditorGUILayout.TextField ("Faction (" + (i).ToString() + ")", t.Factions[i]);
		}

		Footer();
	}

	void FactionRelationValues(){
        Header("Relation Ranges");

		EditorGUILayout.LabelField ("Ranges -1000 --> 1000");
		EditorGUILayout.Space ();

		t.HFS[0] = IntSlider("Hostile Value < ", t.HFS[0], -1000, 0);
		t.HFS[1] = IntSlider("Friendly Value > ", t.HFS[1],0, 1000);
		
		float h = t.HFS[0];
		float f = t.HFS[1];
		
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		
        EditorGUILayout.BeginHorizontal();
        GUI.color = new Color(1.5f,0,0,1);
        GUILayout.Label("<-- Hostile");
        GUI.color = Color.white;
        GUILayout.Label("<-- Neutral -->");
        GUI.color = new Color(0,1.5f,0,1);
        GUILayout.Label("Friendly -->");
        EditorGUILayout.EndHorizontal();
        GUI.color = Color.white;
		EditorGUILayout.MinMaxSlider(ref h,ref f, -1000.0f, 1000.0f);
		
		t.HFS[0] = Mathf.RoundToInt(h);
		t.HFS[1] = Mathf.RoundToInt(f);
		
		Footer();
	}

	void SetPlayerFaction(){
        Header("Player Faction");
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Faction", GUILayout.MaxWidth(100));
		t.PlayerFaction = EditorGUILayout.Popup(t.PlayerFaction, t.Factions, GUILayout.MaxWidth(120));
		EditorGUILayout.EndHorizontal();	
		Footer();
	}

	void SetPlayerRelations(){
        Header("Player Relations Matrix");
		
         t.UsePlayerRelations = Toggle("Use Player Relatioins", t.UsePlayerRelations);

            if(t.UsePlayerRelations) {

            EditorGUILayout.Space ();
            t.UpdatePlayerRelation = Int("Refresh (sec)", t.UpdatePlayerRelation);

		    for (int i = 0; i < t.PlayerRelations.Length; i++){//display  all the elements of the array
			    EditorGUILayout.Space ();
                    //EditorGUILayout.BeginHorizontal();
                    GUI.color = Color.green;
                    EditorGUILayout.LabelField("Player  <-->  " + t.Factions[i].ToString(),GUILayout.MaxWidth(150));
                    GUI.color = Color.white;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Faction Relation",GUILayout.MaxWidth(110));
				    t.PlayerRelations[i] = EditorGUILayout.Slider( "",t.PlayerRelations[i], -1000, 1000, GUILayout.MaxWidth(115));
                    EditorGUILayout.EndHorizontal();
			    //t.PlayerRelations[i] = EditorGUILayout.Slider("Player  <-->  " + t.Factions[i].ToString(),t.PlayerRelations[i], -1000, 1000,GUILayout.MaxWidth (240));
		    }
        } else {
            EditorGUILayout.BeginVertical("HelpBox");
            EditorGUILayout.LabelField("     Player Will Use Global Relations");
            EditorGUILayout.EndVertical();
        }
		Footer();
	}

	void SetFactionRelations(){
        Header("Faction Relations Matrix");

		int[] cnt = new int[3];

		for(int n = 0; n < UniqueRelations; n++){
			if(n < NumberOfFactions - 1){

                GUI.color = MyBlue;
                
				EditorGUILayout.LabelField("Unique Relations (" + t.Factions[n].ToString() + ")", EditorStyles.boldLabel);
                GUI.color = Color.white;
				EditorGUILayout.BeginVertical ("HelpBox");
			}

			for(int x = 0; x < ((NumberOfFactions - 1) - cnt[2]); x++){
				EditorGUILayout.Space ();
                GUI.color = Color.green;
				EditorGUILayout.LabelField(t.Factions[cnt[2]].ToString() + "  <---->  " + t.Factions[((cnt[2] + cnt[1]) + 1)]);
                GUI.color = Color.white;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Faction Relation",GUILayout.MaxWidth(100));
				t.StartRelations[cnt[0]] = EditorGUILayout.Slider( "",t.StartRelations[cnt[0]], -1000, 1000, GUILayout.MaxWidth(120));
                 EditorGUILayout.EndHorizontal();
				if(cnt[0] < UniqueRelations){
					cnt[0]++;
				}

				cnt[1]++;
			}

			if(n < NumberOfFactions - 1){
				EditorGUILayout.Space ();
				EditorGUILayout.EndVertical ();
			}

			if(cnt[1] > 0){
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
			}

			cnt[1] = 0;
			cnt[2]++;
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

    Color _Color(string s, Color o) {
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(s, GUILayout.MaxWidth(167), GUILayout.MinWidth(167));
		o = EditorGUILayout.ColorField("", o, GUILayout.MaxWidth(50));
		EditorGUILayout.EndHorizontal();
        return o;
    }

    bool Toggle(string s, bool o) {
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(s, GUILayout.MaxWidth(167), GUILayout.MinWidth(167));
		o = EditorGUILayout.Toggle("", o, GUILayout.MaxWidth(20));
		EditorGUILayout.EndHorizontal();
        return o;
    }

    int IntSlider(string s, int o, int min, int max) {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(s,GUILayout.MaxWidth(100) );
        o = EditorGUILayout.IntSlider("", o, min, max,GUILayout.MaxWidth(130) );
        EditorGUILayout.EndHorizontal();
        return o;
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
