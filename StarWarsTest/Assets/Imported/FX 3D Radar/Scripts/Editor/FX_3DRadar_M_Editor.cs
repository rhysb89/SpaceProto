using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[CustomEditor(typeof(FX_3DRadar_Mgr))]

public class FX_3DRadar_M_Editor : Editor {
	FX_3DRadar_Mgr t;

    Color MyBlue = new Color(0f, 0.75f, 1f, 1f);
    bool[] srh = new bool[20];

	void OnEnable(){
		t = (FX_3DRadar_Mgr)target;
        if(PrefabUtility.GetPrefabType(t) == PrefabType.PrefabInstance){
            PrefabUtility.DisconnectPrefabInstance(t.gameObject);
        }
        t.transform.position = Vector3.zero;
        t.transform.eulerAngles = Vector3.zero;
        t.transform.localScale = Vector3.one;
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

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
        EditorGUILayout.LabelField("ForceX 3D Radar v. Beta 2c", EditorStyles.boldLabel);
        EditorGUILayout.Space ();
        EditorGUILayout.Space ();
        EditorGUILayout.LabelField("* = Set at Start() or Awake() only", EditorStyles.boldLabel);
        EditorGUILayout.Space ();
        EditorGUILayout.Space ();

        Header("Menu Selection");

		t.RSetup = (FX_3DRadar_Mgr._rSetup)EditorGUILayout.EnumPopup("", t.RSetup, GUILayout.MaxWidth (220));
		EditorGUILayout.Space ();
		EditorGUILayout.EndVertical();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();


		switch((int)t.RSetup){

			case 0:
				RadarConfig();
                EditorGUILayout.Space ();
				Components();
                EditorGUILayout.Space ();
                Audio();
                EditorGUILayout.Space ();
                Pointer();
                EditorGUILayout.Space ();
                PulseRadar();
                EditorGUILayout.Space ();
                TargetSettings();
                EditorGUILayout.Space ();
				BlindRadar();
                EditorGUILayout.Space ();
				RenderOptions();
			break;

			case 1:
			IFFColors();
			EditorGUILayout.Space ();
			HUDDisplaySettings();
			EditorGUILayout.Space ();
			Padding();
			EditorGUILayout.Space ();
			MouseSelection();
            EditorGUILayout.Space ();
            UITextInfo();
			EditorGUILayout.Space ();
            TargetSelectionIndicator();
            EditorGUILayout.Space ();
			TLISettings();
            EditorGUILayout.Space ();
            DIASettings();
			EditorGUILayout.Space ();
			BoundIndicators();
			EditorGUILayout.Space ();
            HUDIDSettings();
            EditorGUILayout.Space ();
            RadarHUDTSI();
            EditorGUILayout.Space ();
			RIDSettings();
            EditorGUILayout.Space ();
            NAVSettings();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			break;

			case 2:
				
			break;

			case 3:

			break;
		}
	}

    void Pointer() {
        Header("Radar Pointer");
        t.PointerStyle = (FX_3DRadar_Mgr.pointerStyle)EditorGUILayout.EnumPopup("", t.PointerStyle, GUILayout.MaxWidth (220));
        EditorGUILayout.Space ();

        if(t.PointerStyle != FX_3DRadar_Mgr.pointerStyle.Disabled) {
            GUI.color = MyBlue;
		    EditorGUILayout.LabelField("Radar Pointer", EditorStyles.boldLabel);
            GUI.color = Color.white;
            EditorGUILayout.Space ();

            t.Pointer = (Transform)Object("* Pointer", t.Pointer, typeof(Transform), true);

            EditorGUILayout.Space ();
            GUI.color = MyBlue;
		    EditorGUILayout.LabelField("Pointer Target", EditorStyles.boldLabel);
            GUI.color = Color.white;
            EditorGUILayout.Space ();

            t.PointerTarget = (Transform)Object("Target", t.PointerTarget, typeof(Transform), true);

            EditorGUILayout.Space ();
            GUI.color = MyBlue;
		    EditorGUILayout.LabelField("Pointer Options", EditorStyles.boldLabel);
            GUI.color = Color.white;

            EditorGUILayout.Space ();

            t.DisableInRange = Toggle("Disable When In Range", t.DisableInRange);
            t.NAVPointer = Toggle("Override NAV", t.NAVPointer);
            EditorGUILayout.Space ();
        }

        
        EditorGUI.indentLevel = 1;
        GUI.color = Color.green;
        srh[0] = EditorGUILayout.Foldout(srh[0], "Scripting Reference");
            if(srh[0]) {

            GUI.color = MyBlue;
            EditorGUILayout.LabelField("Pointer Rotation Style (enum)");
            GUI.color = Color.white;
            EditorGUILayout.LabelField("FX_3DRadar_Mgr.PointerStyle"); 

            GUI.color = MyBlue;
            EditorGUILayout.LabelField("Pointer (Transform)");
            GUI.color = Color.white;
            EditorGUILayout.LabelField("FX_3DRadar_Mgr.Pointer");  
                             
            GUI.color = MyBlue;
            EditorGUILayout.LabelField("Pointer Target (Transform)");
            GUI.color = Color.white;
            EditorGUILayout.LabelField("FX_3DRadar_Mgr.PointerTarget");
        }

        Footer();
    }

    void RadarConfig(){
        Header("Radar Config");
        GUI.color = MyBlue;
		EditorGUILayout.LabelField("World Scale : Default 1 unit = 1m", EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.Space ();

		t.WorldScale = EditorGUILayout.FloatField("Scale", t.WorldScale,GUILayout.MaxWidth(212));

        EditorGUILayout.Space ();
        GUI.color = MyBlue;
		EditorGUILayout.LabelField("Radar Detection Range", EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.Space ();

		t.RadarRange = EditorGUILayout.FloatField("Radar Range :", t.RadarRange,GUILayout.MaxWidth(220));

		EditorGUILayout.Space ();
		t.RadarZoom = (FX_3DRadar_Mgr._radarZoom)EditorGUILayout.EnumPopup("Radar Zoom Level :", t.RadarZoom ,GUILayout.MaxWidth(220));

        if(t.RTSI) {
            EditorGUILayout.Space ();
            EditorGUILayout.LabelField("Detection Range : " + (Mathf.Sqrt(t.RadarRangeSQR) * t.WorldScale).ToString(), GUILayout.MaxWidth(160));
        }

        EditorGUILayout.Space ();
		if (t.RadarEnabled) {
			if (GUILayout.Button ("Disable Radar", GUILayout.MaxWidth (110), GUILayout.MaxHeight (20))) {
				t.DisableRadar ();
			}
		} else {
			if (GUILayout.Button ("Enable Radar", GUILayout.MaxWidth (110), GUILayout.MaxHeight (20))) {
				t.EnableRadar();
			}
		}

        if(t.RTSI) {
            EditorGUILayout.Space ();
            if(GUILayout.Button("Update Radar Range",GUILayout.MaxWidth(130),GUILayout.MaxHeight(20))){
                t.SetRadarRange();
            }
             EditorGUILayout.Space ();
        }

        EditorGUILayout.Space ();
        EditorGUI.indentLevel = 1;
        GUI.color = Color.green;
        srh[3] = EditorGUILayout.Foldout(srh[3], "Scripting Reference");
            if(srh[3]) {
            GUI.color = MyBlue;
            EditorGUILayout.LabelField("World Scale (Float)");
            GUI.color = Color.white;
            EditorGUILayout.LabelField("FX_3DRadar_Mgr.WorldScale");

            GUI.color = MyBlue;
            EditorGUILayout.LabelField("Radar Range (Float)");
            GUI.color = Color.white;
            EditorGUILayout.LabelField("FX_3DRadar_Mgr.RadarRange");

            GUI.color = MyBlue;
            EditorGUILayout.LabelField("Enable Radar (void)");
            GUI.color = Color.white;
            EditorGUILayout.LabelField("FX_3DRadar_Mgr.EnableRadar()");

            GUI.color = MyBlue;
            EditorGUILayout.LabelField("Disable Radar (void)");
            GUI.color = Color.white;
            EditorGUILayout.LabelField("FX_3DRadar_Mgr.DisableRadar()");

            GUI.color = MyBlue;
            EditorGUILayout.LabelField("Update Radar Range (void)");
            GUI.color = Color.white;
            EditorGUILayout.LabelField("FX_3DRadar_Mgr.SetRadarRange()");
        }
        Footer();
	}

	void Components(){
        Header("* Core Components");

        GUI.color = MyBlue;
        EditorGUILayout.LabelField("Player Components", EditorStyles.boldLabel);

        EditorGUILayout.Space ();
        t.Player = (Transform)Object("Player", t.Player, typeof(Transform), true);
        t.PlayerCameraC = (Camera)Object("Player Camera", t.PlayerCameraC, typeof(Camera), true);
        EditorGUILayout.Space ();

        GUI.color = MyBlue;
        EditorGUILayout.LabelField("Radar Components", EditorStyles.boldLabel);
        GUI.color = Color.white;

        EditorGUILayout.Space ();
        t.RadarG = (GameObject)Object("Radar", t.RadarG, typeof(GameObject), true);
        t.RadarCamera = (Camera)Object("Radar Camera", t.RadarCamera, typeof(Camera), true);


		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Radar Layer", GUILayout.MaxWidth(100));
        if(1<<t.RadarLayer == 1<<0) {
            GUI.color = Color.red;
        }
        t.RadarLayer = EditorGUILayout.LayerField("", t.RadarLayer,GUILayout.MaxWidth (105));
        GUI.color = Color.white;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space ();

		GUI.color = MyBlue;
		EditorGUILayout.LabelField("UI Target List Components", EditorStyles.boldLabel);
		GUI.color = Color.white;
		EditorGUILayout.Space ();

        t._TargetList = (RectTransform)Object("List Window", t._TargetList, typeof(RectTransform), true);
        t._Content = (RectTransform)Object("List Content", t._Content, typeof(RectTransform), true);

        EditorGUILayout.Space ();
        GUI.color = MyBlue;
        EditorGUILayout.LabelField("Default Target Button (Optional)", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("  -------  Assign Prefab  -------", EditorStyles.boldLabel);
        GUI.color = Color.white;

        EditorGUILayout.Space ();
   		EditorGUILayout.BeginHorizontal();
        if(t.DefaultListButton) {
            GUI.color = Color.green;
        }
        EditorGUILayout.LabelField("Button", GUILayout.MaxWidth(100));
        GUI.color = Color.white;
		t.DefaultListButton = (Transform)EditorGUILayout.ObjectField("",t.DefaultListButton, typeof(Transform),true,GUILayout.MaxWidth(100));
           
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space ();
        EditorGUILayout.Space ();
        GUI.color = MyBlue;
        EditorGUILayout.LabelField("Target List Window Actions", EditorStyles.boldLabel);
        GUI.color = Color.white;

		EditorGUILayout.Space ();
        
		EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(250));
		
		if(!t.EnableTargetList){
			if(GUILayout.Button("Open Target List",GUILayout.MaxWidth(110),GUILayout.MaxHeight(20))){
				t.EnableTargetList = true;
				t._TargetList.gameObject.SetActive(true);
				t.DisplayTargetListAll();
			}
		}else if(GUILayout.Button("Close Target List",GUILayout.MaxWidth(110),GUILayout.MaxHeight(20))){
			t.EnableTargetList = false;
			t._TargetList.gameObject.SetActive(false);
			t.ListTargets = FX_3DRadar_Mgr._listTargets.DisableList;
			t.ClearList();
		}
		EditorGUILayout.LabelField("    " + t.ListTargets.ToString (), GUILayout.MaxWidth(120));
		
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		t.FilterHostile = Toggle("Filter Hostile Only", t.FilterHostile);

        EditorGUILayout.Space ();
        EditorGUI.indentLevel = 1;
        GUI.color = Color.green;
        srh[1] = EditorGUILayout.Foldout(srh[1], "Scripting Reference");
            if(srh[1]) {
                GUI.color = MyBlue;
            EditorGUILayout.LabelField("Player (Transform)");
            GUI.color = Color.white;
            EditorGUILayout.LabelField("FX_3DRadar_Mgr.Player");               
            GUI.color = MyBlue;
            EditorGUILayout.LabelField("Player Camera (Camera)");
            GUI.color = Color.white;
            EditorGUILayout.LabelField("FX_3DRadar_Mgr.PlayerCameraC");
            GUI.color = MyBlue;
            EditorGUILayout.LabelField("Player Camera (Transform)");
            GUI.color = Color.white;
            EditorGUILayout.LabelField("FX_3DRadar_Mgr.PlayerCameraT");
        }

        Footer();
	}

    void Audio() {
        Header("Radar Sound Effects");
            
        if(t.ACCycleTarget || t.ACClearTarget || t.ACNewContact || t.ACWarning || t.ACPulse) {
        EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Audio Source", GUILayout.MaxWidth(100));
        if(!t.AS) {
            GUI.color = Color.red;
        }
		t.AS = (AudioSource)EditorGUILayout.ObjectField("",t.AS, typeof(AudioSource),true,GUILayout.MaxWidth(100));
		EditorGUILayout.EndHorizontal();

        GUI.color = Color.white;
        }

        EditorGUILayout.Space ();
		EditorGUILayout.BeginHorizontal();
        if(t.ACCycleTarget) {
            GUI.color = Color.green;
        }
		EditorGUILayout.LabelField("Select Target", GUILayout.MaxWidth(100));
        GUI.color = Color.white;
		t.ACCycleTarget = (AudioClip)EditorGUILayout.ObjectField("",t.ACCycleTarget, typeof(AudioClip),true,GUILayout.MaxWidth(100));
        EditorGUILayout.EndHorizontal();
   
   		EditorGUILayout.BeginHorizontal();
        if(t.ACClearTarget) {
            GUI.color = Color.green;
        }
        EditorGUILayout.LabelField("Clear Target", GUILayout.MaxWidth(100));
        GUI.color = Color.white;
		t.ACClearTarget = (AudioClip)EditorGUILayout.ObjectField("",t.ACClearTarget, typeof(AudioClip),true,GUILayout.MaxWidth(100));
           
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if(t.ACNewContact) {
            GUI.color = Color.green;
        }
		EditorGUILayout.LabelField("New Contact", GUILayout.MaxWidth(100));
        GUI.color = Color.white;
		t.ACNewContact = (AudioClip)EditorGUILayout.ObjectField("",t.ACNewContact, typeof(AudioClip),true,GUILayout.MaxWidth(100));
           
        EditorGUILayout.EndHorizontal();

   		EditorGUILayout.BeginHorizontal();
        if(t.ACWarning) {
            GUI.color = Color.green;
        }
		EditorGUILayout.LabelField("Warning", GUILayout.MaxWidth(100));
        GUI.color = Color.white;
		t.ACWarning = (AudioClip)EditorGUILayout.ObjectField("",t.ACWarning, typeof(AudioClip),true,GUILayout.MaxWidth(100));
        EditorGUILayout.EndHorizontal();

   		EditorGUILayout.BeginHorizontal();
        if(t.ACPulse) {
            GUI.color = Color.green;
        }
		EditorGUILayout.LabelField("Radar Pulse", GUILayout.MaxWidth(100));
        GUI.color = Color.white;
		t.ACPulse = (AudioClip)EditorGUILayout.ObjectField("",t.ACPulse, typeof(AudioClip),true,GUILayout.MaxWidth(100));
            
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space ();
		t.PlaySoundsStart = Toggle("* Play Sounds At Start", t.PlaySoundsStart);


        Footer();
    }

    void PulseRadar() {
        Header("* Pulse Radar Settings");

        t.PulseSettings = (FX_3DRadar_Mgr._pulseSettings)EditorGUILayout.EnumPopup("", t.PulseSettings, GUILayout.MaxWidth (220));
        EditorGUILayout.Space ();

        if (t.PulseSettings == FX_3DRadar_Mgr._pulseSettings.PulseAndAnimationUnlinked) {
            EditorGUILayout.BeginHorizontal();
		    EditorGUILayout.LabelField("Sync Sound To Animation", GUILayout.MaxWidth(160));
		    t.PulseSoundSyncAnim = EditorGUILayout.Toggle("", t.PulseSoundSyncAnim, GUILayout.MaxWidth(20));
		    EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space ();
        }

        if (t.PulseSettings == FX_3DRadar_Mgr._pulseSettings.PulseOnly || t.PulseSettings == FX_3DRadar_Mgr._pulseSettings.PulseAndAnimationUnlinked) {
            EditorGUILayout.BeginHorizontal();
		    EditorGUILayout.LabelField("Radar Pulse Time (sec)",GUILayout.MaxWidth(150));
            t.RadarPulseTime = EditorGUILayout.FloatField("", t.RadarPulseTime,GUILayout.MaxWidth(100));
		    EditorGUILayout.EndHorizontal();
        }


        if(t.PulseSettings == FX_3DRadar_Mgr._pulseSettings.PulseAndAnimationLinked || t.PulseSettings == FX_3DRadar_Mgr._pulseSettings.PulseAndAnimationUnlinked || t.PulseSettings == FX_3DRadar_Mgr._pulseSettings.AnimationOnly) {
            EditorGUILayout.Space ();
            EditorGUILayout.Space ();
            RadarAnimation();
        }

        Footer();
        
        if(t.PulseSettings == FX_3DRadar_Mgr._pulseSettings.PulseOnly || t.PulseSettings == FX_3DRadar_Mgr._pulseSettings.PulseAndAnimationLinked || t.PulseSettings == FX_3DRadar_Mgr._pulseSettings.PulseAndAnimationUnlinked) {
            t.EnablePulseRadar = true;
        }

        if(t.PulseSettings == FX_3DRadar_Mgr._pulseSettings.Disabled || t.PulseSettings == FX_3DRadar_Mgr._pulseSettings.AnimationOnly) {
            t.EnablePulseRadar = false;
        }

        if(t.PulseSettings == FX_3DRadar_Mgr._pulseSettings.PulseAndAnimationLinked || t.PulseSettings == FX_3DRadar_Mgr._pulseSettings.AnimationOnly) {
            t.PulseSoundSyncAnim = true;
        }
         if(t.PulseSettings == FX_3DRadar_Mgr._pulseSettings.PulseOnly) {
            t.PulseSoundSyncAnim = false;
        }
    }

    void RadarAnimation() {
        GUI.color = MyBlue;
        EditorGUILayout.LabelField("Radar Animatioin Settings", EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.Space ();

        t.PulseMat = (Material)Object("Pulse Material", t.PulseMat,typeof(Material),true);

        t.PulseDuration = Float("Pulse Duration", t.PulseDuration);
    }

    void TargetSettings() {
        Header("Targeting Options");

		t.AutoTargetDestroy = Toggle("Auto Target On Destroy", t.AutoTargetDestroy);

        Footer();
    }

	void BlindRadar(){
        Header("Blind Radar Settings");

		t.EnableBlindRadar = Toggle("Enable Blind Radar", t.EnableBlindRadar);


			if(t.EnableBlindRadar){
				EditorGUILayout.Space ();
                GUI.color = MyBlue;
                EditorGUILayout.LabelField("Radar Refresh Rate", EditorStyles.boldLabel);
                GUI.color = Color.white;
				t.RadarUpdateTime = Float("Time (sec)", t.RadarUpdateTime);
                EditorGUILayout.Space ();
                GUI.color = MyBlue;
                EditorGUILayout.LabelField("Auto Reacquire Target Duration", EditorStyles.boldLabel);
                GUI.color = Color.white;
				t.RadarResetTime = Float("Time (sec)", t.RadarResetTime);
				
			int o = t.BlindRadarLayers.Length;

			if(o < 1){
				System.Array.Resize(ref t.BlindRadarLayers, 1);
			}
				
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
            GUI.color = MyBlue;
			EditorGUILayout.LabelField("Occlusion Layers", EditorStyles.boldLabel);
            GUI.color = Color.white;

			if(GUILayout.Button("Add Occlusion Layer",GUILayout.MaxWidth(130),GUILayout.MaxHeight(20))){
				System.Array.Resize(ref t.BlindRadarLayers, o + 1);
			}
			EditorGUILayout.Space ();
				for(int i = 0; i < o; i++){
					EditorGUILayout.BeginHorizontal();
					t.BlindRadarLayers[i] = EditorGUILayout.LayerField("Occlusion Layer (" + i.ToString() + ")", t.BlindRadarLayers[i], GUILayout.MaxWidth(220));
					if(o > 1 && GUILayout.Button("-",GUILayout.MaxWidth(15),GUILayout.MaxHeight(15))){
						ResizeArray(i);
						break;
					}
					EditorGUILayout.EndHorizontal();
				}
			}
		EditorGUILayout.Space ();
		EditorGUILayout.EndVertical ();
	}

	void RenderOptions(){
        Header("Radar Display Setup");
        GUI.color = MyBlue;
		EditorGUILayout.LabelField("Radar Materials",EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.Space ();

        t.RadarSphere = (Material)Object("Radar Sphere", t.RadarSphere, typeof(Material), false);
        t.RadarPlane = (Material)Object("Radar Plane", t.RadarPlane, typeof(Material), false);

        EditorGUILayout.Space ();
        GUI.color = MyBlue;
		EditorGUILayout.LabelField("Radar Sphere Line Width",EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.Space ();

        t.LineWidthO = Slider("Orthographic", t.LineWidthO,1);
        t.LineWidthP = Slider("Perspective", t.LineWidthP,1);

        EditorGUILayout.Space ();

        GUI.color = MyBlue;
		EditorGUILayout.LabelField("Radar Zoom (Texture Override)",EditorStyles.boldLabel);
        GUI.color = new Color(2, 0.75f, 1f, 1f);
        EditorGUILayout.Space ();
        EditorGUI.indentLevel = 1;

        string zoom = "Expand";

        if (srh[5]) {
            zoom = "Collapse";
        }

        srh[5] = EditorGUILayout.Foldout(srh[5], zoom);
            if(srh[5]) {
                EditorGUILayout.Space ();
                t.RadarZoom4 = (Texture2D)Object("Zoom In 4x", t.RadarZoom4, typeof(Texture2D), true);
                t.RadarZoom2 = (Texture2D)Object("Zoom In 2x", t.RadarZoom2, typeof(Texture2D), true);
                t.RadarNormal = (Texture2D)Object("Zoom Normal", t.RadarNormal, typeof(Texture2D), true);
                t.RadarBoost15 = (Texture2D)Object("Boost 1.5x", t.RadarBoost15, typeof(Texture2D), true);
                t.RadarBoost2 = (Texture2D)Object("Boost 2x", t.RadarBoost2, typeof(Texture2D), true);
        }

        EditorGUI.indentLevel = 0;
        EditorGUILayout.Space ();
        GUI.color = MyBlue;
		EditorGUILayout.LabelField("Radar Display Mode",EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.Space ();

		if(t.Radar2D){
			if(GUILayout.Button("Switch To 3D",GUILayout.MaxWidth(150),GUILayout.MaxHeight(20))){
				t.Radar3D_2D();
			}
		}else{
			if(GUILayout.Button("Switch To 2D",GUILayout.MaxWidth(150),GUILayout.MaxHeight(20))){
				t.Radar3D_2D();
			}
		}

		if(!t.Radar2D){
			if(t.PerspectiveRadar){
				if(GUILayout.Button("Switch To Orthographic",GUILayout.MaxWidth(150),GUILayout.MaxHeight(20))){
					t.SelectRadarStyle();
				}
			}else{
				if(GUILayout.Button("Switch To Perspective",GUILayout.MaxWidth(150),GUILayout.MaxHeight(20))){
					t.SelectRadarStyle();
				}
			}
		}

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		t.RadarPos = (FX_3DRadar_Mgr._radarPos)EditorGUILayout.EnumPopup("* Radar Position", t.RadarPos ,GUILayout.MaxWidth(220));

        if(t.RadarPos == FX_3DRadar_Mgr._radarPos.RenderToTexture) {
            EditorGUILayout.Space ();
            EditorGUILayout.Space ();
            RTTSettings ();
        }

        EditorGUILayout.Space ();
        EditorGUI.indentLevel = 1;
        GUI.color = Color.green;
        srh[4] = EditorGUILayout.Foldout(srh[4], "Scripting Reference");
            if(srh[4]) {
            GUI.color = MyBlue;
            EditorGUILayout.LabelField("Radar Materials (Material)");
            GUI.color = Color.white;
            EditorGUILayout.LabelField("FX_3DRadar_Mgr.RadarSphere");
            EditorGUILayout.LabelField("FX_3DRadar_Mgr.RadarPlane");
            GUI.color = MyBlue;
            EditorGUILayout.LabelField("Material Properties");
            GUI.color = Color.white;
            EditorGUILayout.LabelField("Texture :             _MainText");
            EditorGUILayout.LabelField("Texture Color :     _TintColor");
            EditorGUILayout.LabelField("LineWidth :          _LineWidth");
            EditorGUILayout.LabelField("LineColor :           _LineColor");
        }

		Footer();
	}

	void IFFColors(){
        Header("IFF Color Assignments");

		t.IFFColor[0] = _Color("Color Neutral",t.IFFColor[0]);
		t.IFFColor[1] = _Color("Color Friendly", t.IFFColor[1]);
		t.IFFColor[2] = _Color("Color Hostile", t.IFFColor[2]);
		t.IFFColor[3] = _Color("Color Unknown", t.IFFColor[3]);
		t.IFFColor[4] = _Color("Color Abandoned", t.IFFColor[4]);
		t.IFFColor[5] = _Color("Color Player Owned", t.IFFColor[5]);
		t.IFFColor[6] = _Color("Color Objective", t.IFFColor[6]);
		t.IFFColor[7] = _Color("Color NAV", t.IFFColor[7]);

		t.UseObjectiveColor = EditorGUILayout.Toggle("Objective Coolor", t.UseObjectiveColor);

		Footer();
	}

    void HUDDisplaySettings(){
        Header("HUD Opacity");

        t.HUDAlpha = (byte)IntSlider("HUD Opacity", t.HUDAlpha, 255);
        t.RadarRIDAlpha = (byte)IntSlider("RID Opacity", t.RadarRIDAlpha, 255);
        t.RadarVDIAlpha = (byte)IntSlider("VDI Opacity", t.RadarVDIAlpha, 255);
        EditorGUILayout.Space ();

        t.MinFadeAmount = (byte)IntSlider("Min Fade Amount", t.MinFadeAmount, 255);

		EditorGUILayout.Space ();

		t.FadeInvert = Toggle("Invert Fade", t.FadeInvert);
		t.FadeTSI = Toggle("Fade TSI", t.FadeTSI);
    	t.FadeTLI = Toggle("Fade TLI", t.FadeTLI);
		t.FadeHUDID = Toggle("Fade HUDID", t.FadeHUDID);
		t.FadeBounds = Toggle("Fade Bounds", t.FadeBounds);
		t.FadeRID = Toggle("Fade RID's", t.FadeRID);
        t.POINoFade = Toggle("Do Not Fade NAV / POI", t.POINoFade);

		Footer();
	}

	void Padding(){
        Header("Screen Edge Padding (Pixels)");

		t.HUDMainOffset = Int("Target Selection : ",t.HUDMainOffset);
		t.HUDIDPadding = Int("HUD ID",t.HUDIDPadding);

		Footer();
	}

	void MouseSelection(){
        Header("Mouse Selection Options");


		t.SelectableHUDID = Toggle("* Target HUD ID", t.SelectableHUDID);
		t.SelectableRID = Toggle("* Target RID", t.SelectableRID);
		t.SelectableObj = Toggle("Target Object", t.SelectableObj);

		if(t.SelectableObj){
			t.SelectableSub = Toggle("Target Sub Component", t.SelectableSub);
		}else{
			t.SelectableSub = false;
		}

		Footer();
	}

	void NAVSettings(){
        Header("* NAV Indicator");

        GUI.color = MyBlue;
		EditorGUILayout.LabelField("HUD Dispaly Settings", EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.Space ();

		t.Sprite_NAV = (Sprite)Object("* Sprite", t.Sprite_NAV, typeof(Sprite), true);
        t.NAVHUDOffset = EditorGUILayout.Vector2Field("HUD Offset", t.NAVHUDOffset);

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		GUI.color = MyBlue;
		EditorGUILayout.LabelField("Radar Dispaly Settings", EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.Space ();

		t.RenderNAVRadar = Toggle("Render NAV In Radar", t.RenderNAVRadar);
        EditorGUILayout.Space ();

		if(t.RenderNAVRadar){
            t.Sprite_RadarNAV = (Sprite)Object("* Sprite", t.Sprite_RadarNAV, typeof(Sprite), true);
            t.NAVRIDOffset = EditorGUILayout.Vector3Field("Radar Offset", t.NAVRIDOffset);

                if (GUILayout.Button ("Calculate Offset", GUILayout.MaxWidth (150), GUILayout.MaxHeight (20))) {
                    Vector2 ThisSize = t.Sprite_RadarNAV.rect.size;
                    Vector2 IDOffset = FindCenter(ThisSize);

                    t.NAVRIDOffset = new Vector3(IDOffset.x + 1, 0, ThisSize.y);
                }
		}

		Footer();
	}

	void DIASettings(){
        Header("* Directional Indicator Arrow");

        GUI.color = MyBlue;
		EditorGUILayout.LabelField("HUD Dispaly Settings", EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.Space ();

		t.RenderHUDDIA = Toggle("* Render DIA", t.RenderHUDDIA);

		if(t.RenderHUDDIA){
            EditorGUILayout.Space ();
            t.HUDDIARad = Int("Radius : ",t.HUDDIARad);
			EditorGUILayout.Space ();

            t.Sprite_DIA = (Sprite)Object("* Sprite", t.Sprite_DIA, typeof(Sprite), true);
		}

		Footer();
	}

    void UITextInfo() {
        Header("HUD Info Display (UI Text)");
        GUI.color = MyBlue;
		EditorGUILayout.LabelField("Radar Range Text", EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.Space ();

        t.TextRadarRange = (Text)Object("Radar Range",t.TextRadarRange, typeof(Text),false);
        if(t.TextRadarRange) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Prefix", GUILayout.MaxWidth(100));
            t.RadarRangePrefix = EditorGUILayout.TextField("",t.RadarRangePrefix, GUILayout.MaxWidth(100));
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space ();
        GUI.color = MyBlue;
		EditorGUILayout.LabelField("Radar Zoom Text", EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.Space ();

        t.TextRadarZoom = (Text)Object("Radar Zoom",t.TextRadarZoom, typeof(Text),false);
        if(t.TextRadarZoom) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Prefix", GUILayout.MaxWidth(100));
            t.RadarZoomPrefix = EditorGUILayout.TextField("",t.RadarZoomPrefix, GUILayout.MaxWidth(100));
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space ();
        GUI.color = MyBlue;
		EditorGUILayout.LabelField("Target Name Text", EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.Space ();

        t.TextTargetName = (Text)Object("Target Name",t.TextTargetName, typeof(Text),false);
        if(t.TextTargetName) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Prefix", GUILayout.MaxWidth(100));
            t.TargetNamePrefix = EditorGUILayout.TextField("",t.TargetNamePrefix, GUILayout.MaxWidth(100));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space ();
            t.NameAndDistance = Toggle("Append Distance To Name", t.NameAndDistance);

        } else {
            t.NameAndDistance = false;
        }

        EditorGUILayout.Space ();
        GUI.color = MyBlue;
		EditorGUILayout.LabelField("Target Sub Component Text", EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.Space ();

        t.TextSubComp = (Text)Object("Sub Component",t.TextSubComp, typeof(Text),false);
        if(t.TextSubComp) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Prefix", GUILayout.MaxWidth(100));
            t.TargetSubPrefix = EditorGUILayout.TextField("",t.TargetSubPrefix, GUILayout.MaxWidth(100));
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space ();

        EditorGUILayout.Space ();
        GUI.color = MyBlue;
		EditorGUILayout.LabelField("Target Distance Text", EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.Space ();

        if(!t.NameAndDistance) {
            t.TextTargetDistance = (Text)Object("Target Distance",t.TextTargetDistance, typeof(Text),false);
        }
            if(t.TextTargetDistance || t.NameAndDistance) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Prefix", GUILayout.MaxWidth(100));
                t.TargetDistancePrefix = EditorGUILayout.TextField("",t.TargetDistancePrefix, GUILayout.MaxWidth(100));
                EditorGUILayout.EndHorizontal();
            }

        EditorGUILayout.Space ();
        GUI.color = MyBlue;
		EditorGUILayout.LabelField("Target Class Text", EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.Space ();

        t.TextTargetClass = (Text)Object("Target Class",t.TextTargetClass, typeof(Text),false);
        if(t.TextTargetClass) {
            EditorGUILayout.Space ();
            t.AsSubClass = Toggle("As Sub Class", t.AsSubClass);
            EditorGUILayout.Space ();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Prefix : ", GUILayout.MaxWidth(100));
            t.TargetClassPrefix = EditorGUILayout.TextField("",t.TargetClassPrefix, GUILayout.MaxWidth(100));
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space ();
        GUI.color = MyBlue;
		EditorGUILayout.LabelField("Global Distance Suffix", EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.Space ();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Suffix Meter", GUILayout.MaxWidth(100));
        t.SuffixMeters = EditorGUILayout.TextField("",t.SuffixMeters, GUILayout.MaxWidth(100));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Suffix Kilometer", GUILayout.MaxWidth(100));
        t.SuffixKilometers = EditorGUILayout.TextField("",t.SuffixKilometers, GUILayout.MaxWidth(100));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space ();
        Footer();
    }

	void TargetSelectionIndicator(){
        Header("* Target Selection Indicator");

        GUI.color = MyBlue;
		EditorGUILayout.LabelField("HUD Dispaly Settings", EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.Space ();

		t.DisableTSIOS = Toggle("Disable Off Screen", t.DisableTSIOS);
		EditorGUILayout.Space ();

		t.TSI = (RectTransform)Object("Target Indicator",t.TSI, typeof(RectTransform),true);
		EditorGUILayout.Space ();
        EditorGUILayout.Space ();

		t.RenderTSIID = Toggle("Render Target ID", t.RenderTSIID);
		
		if(t.RenderTSIID){
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Target ID", GUILayout.MaxWidth(100));
			if(!t.Image_TSIID){
				GUI.color = Color.red;
			}
			t.Image_TSIID = (Image)EditorGUILayout.ObjectField("",t.Image_TSIID, typeof(Image),true,GUILayout.MaxWidth(125));
			GUI.color = Color.white;
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.Space ();

        GUI.color = MyBlue;
		EditorGUILayout.LabelField("SubComponent Selection Indicator", EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.Space ();

		t.Sprite_SCTSI = (Sprite)Object("* Sprite", t.Sprite_SCTSI, typeof(Sprite), true);


		EditorGUILayout.Space ();
		EditorGUILayout.EndVertical();
	}

	void TLISettings(){
        Header("* Target Lead Indicator");

        GUI.color = MyBlue;
		EditorGUILayout.LabelField("HUD Display Settings", EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.Space ();

		t.RenderTLI = Toggle("* Render TLI", t.RenderTLI);


		if(t.RenderTLI){

			EditorGUILayout.Space ();
			t.Sprite_TLI = (Sprite)Object("* Sprite", t.Sprite_TLI, typeof(Sprite), true);
			EditorGUILayout.Space ();

            GUI.color = MyBlue;
		    EditorGUILayout.LabelField("Velocity Calculations", EditorStyles.boldLabel);
            GUI.color = Color.white;
            EditorGUILayout.Space ();

			t.UsePlayerPhysics = Toggle("Use Player Physics", t.UsePlayerPhysics);
			t.UseTargetPhysics = Toggle("Use Target Physics", t.UseTargetPhysics);

            EditorGUILayout.Space ();
            EditorGUILayout.Space ();
            GUI.color = MyBlue;
		    EditorGUILayout.LabelField("Projectile Velocity", EditorStyles.boldLabel);
            GUI.color = Color.white;
            EditorGUILayout.Space ();
			t.ProjectileVelocity = Int("Velocity",t.ProjectileVelocity);
		}
            EditorGUILayout.Space ();
            EditorGUI.indentLevel = 1;
            GUI.color = Color.green;
            srh[2] = EditorGUILayout.Foldout(srh[2], "Scripting Reference");
             if(srh[2]) {
                
                GUI.color = MyBlue;
                EditorGUILayout.LabelField("Projectile Velocity (Int)");
                GUI.color = Color.white;
                EditorGUILayout.LabelField("FX_3DRadar_Mgr.ProjectileVelocity");
            }

		Footer();
	}

    void HUDIDSettings(){
        Header("Global HUD Identifier Settings");

        GUI.color = MyBlue;
		EditorGUILayout.LabelField("HUD Display Settings", EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.Space ();

		t.RenderHUDID = Toggle("* Render HUD ID's", t.RenderHUDID);


		if(t.RenderHUDID){

			t.HUDIAsIcon = Toggle("* Display As ID", t.HUDIAsIcon);

			if(!t.HUDIAsIcon){
				EditorGUILayout.Space ();
				t.Sprite_HUDID = (Sprite)Object("* Sprite", t.Sprite_HUDID, typeof(Sprite), true);
			}

			EditorGUILayout.Space ();

			t.G_DisplayHUDID =Toggle("Display HUD ID", t.G_DisplayHUDID);
		}

			Footer();
	}

	void BoundIndicators(){
        Header("Global Bounds Settings");

        GUI.color = MyBlue;
		EditorGUILayout.LabelField("HUD Display Settings", EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.Space ();

		t.RenderBounds = Toggle("* Render Bounds", t.RenderBounds);

		EditorGUILayout.Space ();

		if(t.RenderBounds){

			t.AdvancedBounds = Toggle("Advanced Bounds", t.AdvancedBounds);

			EditorGUILayout.Space ();
			t.Sprite_BoundSquare = (Sprite)Object("", t.Sprite_BoundSquare, typeof(Sprite), true);

            EditorGUILayout.Space ();
			t.G_DisplayBounds = Toggle("Display Bounds", t.G_DisplayBounds);
			t.LBS = Toggle("Limit Screen Size", t.LBS);

            if(t.LBS) {
			    t.MBS = Int("Max Size (Pixels)", t.MBS);
            }
			t.BPadding = Int("Padding", t.BPadding);

		}

		Footer();
	}

    void RadarHUDTSI() {
        Header("Radar & HUD Shared Settings");
        GUI.color = MyBlue;
		EditorGUILayout.LabelField("Radar & HUD Selection Indicator", EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.Space ();

		t.Sprite_RHTSI = (Sprite)Object("* Sprite", t.Sprite_RHTSI, typeof(Sprite), true);

        Footer();
    }

	void RIDSettings(){
        Header("Global RID Settings");

		EditorGUILayout.Space ();
        GUI.color = MyBlue;
		EditorGUILayout.LabelField("VDI Settings", EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.Space ();
		t.RenderVDI = Toggle("* Render VDI", t.RenderVDI);

		
		if(t.RenderVDI){
            EditorGUILayout.Space ();
            GUI.color = MyBlue;
		    EditorGUILayout.LabelField("VDI Base Settings", EditorStyles.boldLabel);
            GUI.color = Color.white;
            EditorGUILayout.Space ();

			t.RenderRIDB =Toggle("* Render VDI Base", t.RenderRIDB);


			if(t.RenderRIDB){
				
				EditorGUILayout.Space ();
				EditorGUILayout.LabelField("VDI Base Spite", GUILayout.MaxWidth(210));

				t.Sprite_RIDBase = (Sprite)Object("* Sprite", t.Sprite_RIDBase, typeof(Sprite), true);
                t.RIDBOffset = EditorGUILayout.Vector2Field("VDI Base Offset", t.RIDBOffset,GUILayout.MaxWidth(220));

			}

		}else{
			t.RenderRIDB = false;
		}

		Footer();
	}

	void RTTSettings(){

        EditorGUILayout.Space ();
        GUI.color = MyBlue;
		EditorGUILayout.LabelField("Render To Texture Settings", EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.Space ();

		t.RenderMaterial = (Material)Object("Render Material",t.RenderMaterial, typeof(Material),true);

		t.RenderTarget = (GameObject)Object("Render Target",t.RenderTarget, typeof(GameObject),true);


        EditorGUILayout.Space ();
		t.BGColor = _Color ("Background Color",t.BGColor);

        EditorGUILayout.Space ();
		t.RT = (RenderTexture)Object("Render Texture",t.RT, typeof(RenderTexture),true);

        EditorGUILayout.Space ();
	}

	void ResizeArray(int remove){
		int[] tempArray = new int[t.BlindRadarLayers.Length];
		int cnt = 0;
		for(int i = 0; i < tempArray.Length; i++){
			if(i != remove){
				tempArray[cnt] = t.BlindRadarLayers[i];
				cnt++;
			}
		}
		System.Array.Resize(ref tempArray, tempArray.Length - 1);
		t.BlindRadarLayers = tempArray;
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
        o = EditorGUILayout.ObjectField("",o, type,true,GUILayout.MaxWidth(100),GUILayout.MinWidth(100), GUILayout.MaxHeight(50));
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

    int IntSlider(string s, int o, int range) {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(s,GUILayout.MaxWidth(110) );
        o = EditorGUILayout.IntSlider("", o, 0, range,GUILayout.MaxWidth(115) );
        EditorGUILayout.EndHorizontal();
        return o;
    }

    float Slider (string s, float o, float range) {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(s,GUILayout.MaxWidth(110) );
        o = EditorGUILayout.Slider("", o, 0, range,GUILayout.MaxWidth(115) );
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

    bool HelpWindow(bool b, string s) {
        EditorGUILayout.Space ();
        EditorGUI.indentLevel = 1;
        b = EditorGUILayout.Foldout(b,"?");
        EditorGUI.indentLevel = 0;

        if(b) {
           	EditorGUI.indentLevel = 0;
            GUI.color = Color.white;
		    EditorGUILayout.BeginVertical ("HelpBox", GUILayout.MaxWidth(280),GUILayout.MinWidth(280));
		    EditorGUILayout.Space ();
            EditorGUILayout.LabelField(s);

            Footer();
        }
        return b;
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
}