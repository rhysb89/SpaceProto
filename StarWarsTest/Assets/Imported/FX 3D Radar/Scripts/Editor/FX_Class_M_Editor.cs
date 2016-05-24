using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FX_Class_Mgr))]
public class FX_Class_M_Editor : Editor {
	FX_Class_Mgr t;
    FX_3DRadar_Mgr FX3DRM;
	SerializedObject GetTarget;
	SerializedProperty ThisObjectClassList;
    bool UpdateAll;
	void OnEnable(){
		t = (FX_Class_Mgr)target;
		GetTarget = new SerializedObject(t);
		ThisObjectClassList = GetTarget.FindProperty("ObjectClassList");

        if(GameObject.Find("_GameMgr")) {
            FX3DRM = GameObject.Find("_GameMgr").GetComponent<FX_3DRadar_Mgr>();
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

	public override void OnInspectorGUI(){
		CheckMinSize();
        IndicatorSize();
		CustomClassEditor();
	}

	void CheckMinSize(){
		GetTarget.Update();
		if(ThisObjectClassList.arraySize < 1){
			t.ObjectClassList.Add(new FX_Class_Mgr.objectClassList());
		}
		GetTarget.ApplyModifiedProperties();
	}

    void IndicatorSize() {
        EditorGUILayout.Space ();
		EditorGUILayout.Space ();
        EditorGUILayout.LabelField ("Selection Indicator");
        EditorGUILayout.BeginVertical("HelpBox",GUILayout.MaxWidth(245));
        if(FX3DRM) {
            if(FX3DRM.Sprite_RHTSI) {
                EditorGUILayout.Space ();
                GoodStatus(" Sprite Detected Using Automatic Size");

                t.IndicatorSize = FX3DRM.Sprite_RHTSI.rect.size;
                EditorGUILayout.Space ();
            } else {
                GoodStatus("         FX_3DRadar_Mgr Detected ");
                Warning("        No Sprite Assigned For RHTSI ");
                Warning("               Using Manual Entry ");
                 t.IndicatorSize = EditorGUILayout.Vector2Field("Size (Pixels)", t.IndicatorSize);
            }
        } else {
            Warning("      No FX_3DRadar_Mgr Detected ");
            Warning("               Using Manual Entry ");
            t.IndicatorSize = EditorGUILayout.Vector2Field("Size (Pixels)", t.IndicatorSize);
        }
        EditorGUILayout.EndVertical();
    }

	void CustomClassEditor(){
		GetTarget.Update();
		
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

        EditorGUILayout.BeginHorizontal();
        GUI.color = Color.green;
		if(GUILayout.Button("Add New Class",GUILayout.MaxWidth(100),GUILayout.MaxHeight(20))){
			t.ObjectClassList.Add(new FX_Class_Mgr.objectClassList());
		}
        GUI.color = Color.cyan;
        if (GUILayout.Button ("Calculate All Offsets", GUILayout.MaxWidth (140), GUILayout.MaxHeight (20))) {
            UpdateAll = true;
        }
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		for(int i = 0; i < ThisObjectClassList.arraySize; i++){
			SerializedProperty MyListRef = ThisObjectClassList.GetArrayElementAtIndex(i);
			SerializedProperty ClassName = MyListRef.FindPropertyRelative("ClassName");
			SerializedProperty SubClassName = MyListRef.FindPropertyRelative("SubClassName");
			SerializedProperty ClassSprite = MyListRef.FindPropertyRelative("ClassSprite");
			SerializedProperty RIDOffset = MyListRef.FindPropertyRelative("RIDOffset");
            SerializedProperty TSIOffset = MyListRef.FindPropertyRelative("TSIOffset");
            SerializedProperty HUDOffset = MyListRef.FindPropertyRelative("HUDOffset");
            SerializedProperty Toggle = MyListRef.FindPropertyRelative("Toggle");

			GUI.color = GetColor(i);

			EditorGUILayout.BeginVertical("HelpBox", GUILayout.MaxWidth(250));
			GUI.color = Color.white;
			EditorGUILayout.BeginHorizontal();
			if(i > 0 && GUILayout.Button("-",GUILayout.MaxWidth(15),GUILayout.MaxHeight(15))){
				t.ObjectClassList.RemoveAt(i);
                break;
			}
			EditorGUILayout.LabelField ("Class Name : ",GUILayout.MaxWidth(85));
			ClassName.stringValue = EditorGUILayout.TextField("",ClassName.stringValue,GUILayout.MaxWidth(130));

			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();

			GUI.color = GetColor(i);

			EditorGUILayout.BeginVertical("HelpBox", GUILayout.MaxWidth(250));
			GUI.color = Color.white;
			EditorGUILayout.Space ();
			
			if(SubClassName.arraySize < 1){
				SubClassName.InsertArrayElementAtIndex(SubClassName.arraySize);
				ClassSprite.InsertArrayElementAtIndex(ClassSprite.arraySize);
				RIDOffset.InsertArrayElementAtIndex(RIDOffset.arraySize);
                TSIOffset.InsertArrayElementAtIndex(TSIOffset.arraySize);
                HUDOffset.InsertArrayElementAtIndex(HUDOffset.arraySize);
                Toggle.InsertArrayElementAtIndex(Toggle.arraySize);
				SubClassName.GetArrayElementAtIndex(SubClassName.arraySize - 1).stringValue = "Sub Class " + (SubClassName.arraySize - 1).ToString ();
				ClassSprite.GetArrayElementAtIndex(ClassSprite.arraySize - 1).objectReferenceValue = null;
                GetTarget.ApplyModifiedProperties();
			}

			if(GUILayout.Button("Add New Sub Class",GUILayout.MaxWidth(130),GUILayout.MaxHeight(20))){
				SubClassName.InsertArrayElementAtIndex(SubClassName.arraySize);
				ClassSprite.InsertArrayElementAtIndex(ClassSprite.arraySize);
				RIDOffset.InsertArrayElementAtIndex(RIDOffset.arraySize);
                TSIOffset.InsertArrayElementAtIndex(TSIOffset.arraySize);
                HUDOffset.InsertArrayElementAtIndex(HUDOffset.arraySize);
                Toggle.InsertArrayElementAtIndex(Toggle.arraySize);
				SubClassName.GetArrayElementAtIndex(SubClassName.arraySize - 1).stringValue = "Sub Class " + (SubClassName.arraySize - 1).ToString ();
				ClassSprite.GetArrayElementAtIndex(ClassSprite.arraySize - 1).objectReferenceValue = null;
                GetTarget.ApplyModifiedProperties();
			}

			EditorGUILayout.Space ();
			for(int a = 0; a < SubClassName.arraySize; a++){
				EditorGUILayout.BeginVertical("HelpBox",GUILayout.MaxWidth(250));
				EditorGUILayout.Space ();
				EditorGUILayout.BeginHorizontal();
				if(a > 0 && GUILayout.Button("-",GUILayout.MaxWidth(15),GUILayout.MaxHeight(15))){
					t.ObjectClassList[i].SubClassName.RemoveAt(a);
					t.ObjectClassList[i].ClassSprite.RemoveAt(a);
					t.ObjectClassList[i].RIDOffset.RemoveAt(a);
                    t.ObjectClassList[i].TSIOffset.RemoveAt(a);
                    t.ObjectClassList[i].HUDOffset.RemoveAt(a);
                    t.ObjectClassList[i].Toggle.RemoveAt(a);
                    break;
				}
				SubClassName.GetArrayElementAtIndex(a).stringValue = EditorGUILayout.TextField("",SubClassName.GetArrayElementAtIndex(a).stringValue,GUILayout.MaxWidth(85));
				ClassSprite.GetArrayElementAtIndex(a).objectReferenceValue = EditorGUILayout.ObjectField("", ClassSprite.GetArrayElementAtIndex(a).objectReferenceValue, typeof(Sprite), true,GUILayout.MaxWidth(120),GUILayout.MaxHeight(50));


				EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel = 1;
                Toggle.GetArrayElementAtIndex(a).boolValue = EditorGUILayout.Foldout(Toggle.GetArrayElementAtIndex(a).boolValue, "Display Offset Values");
                EditorGUI.indentLevel = 0;

                EditorGUILayout.Space ();

                if (UpdateAll || GUILayout.Button ("Calculate Offset", GUILayout.MaxWidth (130), GUILayout.MaxHeight (20))) {
                    Vector2 ThisSize = t.ObjectClassList[i].ClassSprite[a].rect.size;
                    Vector2 IDOffset = FindCenter(ThisSize);
                    Vector2 TOffset = FindCenter(t.IndicatorSize);
                    HUDOffset.GetArrayElementAtIndex(a).vector2Value = IDOffset;
                    RIDOffset.GetArrayElementAtIndex(a).vector3Value = new Vector3(IDOffset.x + 1, 0, ThisSize.y);

                    IDOffset *= 1;
                    TOffset *=1;
                    
                    TSIOffset.GetArrayElementAtIndex(a).vector2Value = new Vector2(TOffset.x  - IDOffset.x,TOffset.y - IDOffset.y);
                }

                if(t.ObjectClassList[i].Toggle[a]) {
                EditorGUILayout.Space();
				EditorGUILayout.Space ();
                EditorGUILayout.LabelField("HUD ID Center Offset",EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical("HelpBox");
                EditorGUILayout.Space ();

                HUDOffset.GetArrayElementAtIndex(a).vector2Value = EditorGUILayout.Vector2Field(" ", HUDOffset.GetArrayElementAtIndex(a).vector2Value);
                EditorGUILayout.Space ();
                EditorGUILayout.EndVertical();

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Radar ID Center Offset",EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical("HelpBox");
                EditorGUILayout.Space ();

                EditorGUILayout.LabelField("X = Horizontal");
                EditorGUILayout.LabelField("Y = +Vertical");
                EditorGUILayout.LabelField("Z = -Vertical");
                RIDOffset.GetArrayElementAtIndex(a).vector3Value = EditorGUILayout.Vector3Field(" ", RIDOffset.GetArrayElementAtIndex(a).vector3Value);

                EditorGUILayout.Space ();
                EditorGUILayout.EndVertical();

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("TSI Center Offset",EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical("HelpBox");
                
                TSIOffset.GetArrayElementAtIndex(a).vector2Value = EditorGUILayout.Vector2Field(" ", TSIOffset.GetArrayElementAtIndex(a).vector2Value);
                EditorGUILayout.Space ();
                EditorGUILayout.EndVertical();
               }
                EditorGUILayout.Space ();
                EditorGUILayout.EndVertical();

            }
			EditorGUILayout.Space ();
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
		}
		UpdateAll = false;
		GetTarget.ApplyModifiedProperties();
	}

    Vector2 FindCenter(Vector2 size) {

        if(size.x % 2 == 1) {
            size.x = (size.x * 0.5f) + 0.5f;
        } else {
            size.x = size.x * 0.5f;
        }
        if(size.y % 2 == 1) {
            size.y = (size.y * 0.5f) + 0.5f;
        } else {
            size.y = size.y * 0.5f;
        }
        return -size;
    }

	Color GetColor(int i){
		Color NewColor = Color.white;

		if(i%2==0){
			NewColor = new Color(0,.5f,1,1); //Blue
			//GUI.color = new Color(1,.2f,.2f,1); //Red
			//GUI.color = new Color(.5f,1,0,1); //Green
		}else{
			//GUI.color = new Color(0,.5f,1,1); //Blue
			NewColor = new Color(1,.2f,.2f,1); //Red
			//GUI.color = new Color(.5f,1,0,1); //Green
		}
		return NewColor;
	}

    void GoodStatus(string s) {
        GUI.color = Color.green;
        EditorGUILayout.BeginVertical("HelpBox");
        EditorGUILayout.LabelField(s);
        EditorGUILayout.EndVertical();
        GUI.color = Color.white;
    }

    void Warning(string s) {
        GUI.color = Color.red;
        EditorGUILayout.BeginVertical("HelpBox");
        EditorGUILayout.LabelField(s);
        EditorGUILayout.EndVertical();
        GUI.color = Color.white;
    }
}
