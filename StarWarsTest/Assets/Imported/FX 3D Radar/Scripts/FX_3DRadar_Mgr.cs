	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections.Generic;
	
	public class FX_3DRadar_Mgr : MonoBehaviour {
	#region Inspector Current Window
	public enum _rSetup {RadarSettings, HUDDisplaySettings}
	public _rSetup RSetup;
	#endregion
	
	int ButtonPadding = 2;

	#region Layers
	public int[] BlindRadarLayers = new int[1];
	public LayerMask BRL; // Blind Radar Layer Mask
	public int RadarLayer;
	public LayerMask RadarMask;
	#endregion
	
	#region External Scripts
	FX_Input_Mgr FXIM;
	FX_3DRadar_RID TargetRID;
	public FX_Class_Mgr FXCM;
	public FX_Mission_Mgr FXMM;
	public FX_Faction_Mgr FXFM;
	#endregion
	
	#region Player Cache
	//Player
	public Transform Player;
	public Vector3 PlayerPos;
	public AudioSource AS;
	
	//Player Camera
	public Camera PlayerCameraC;
	public Transform PlayerCameraT;
	public Vector3 PlayerCamPos;
	
	//Radar Camera
	public GameObject RadarG;
	public Camera RadarCamera;
	#endregion
	
	#region Radar Settings & Globals
	public bool RadarEnabled = true;
	public float WorldScale = 1.0f;  //World Scale is used to define the size of a world unit in meters. The default 1.0f = 1x1x1 meters. Example a value of 10 will make a world unit equal 10x10x10 meters.
	public bool PerspectiveRadar; //If the radar will be Perspective or Orthographic
	public bool Radar2D; //If the radar will be 3D or top down 2D
	
	public enum _radarZoom {Zoom_In_X4 = 0, Zoom_In_2X = 1, Normal = 2, Boost_1_5 = 3, Boost_2 = 4}
	public _radarZoom RadarZoom = _radarZoom.Normal;
	
	public float RadarRange = 100.0f; //The distance in meters the radar can detect an active target from the player.
	public float RadarRangeSQR; //PreCompute Radar Range with scaling. This is used by all potental targets for distance calculations.
	public float RadarLocalScale; //Scales the Targets distance in the Radar view.
	
	public enum _radarPos {CustomPosition, TopLeft, TopRight, BottomLeft, BottomRight, RenderToTexture}
	public _radarPos RadarPos;
    #endregion

    #region UI Text Information
    public Text TextRadarRange;
    public string RadarRangePrefix  = "Range : ";

    public Text TextRadarZoom;
    public string RadarZoomPrefix = "x ";

    public bool NameAndDistance;
    public bool AsSubComp;
    public Text TextTargetName;
    public Text TextTargetDistance;
    public string TargetNamePrefix = "Target : ";
    public string TargetDistancePrefix = "Distance : ";
    public string SuffixMeters = " m";
    public string SuffixKilometers = " klm";

    public Text TextTargetClass;
    public string TargetClassPrefix = "ID : ";
    public bool AsSubClass;

    public Text TextSubComp;
    public string TargetSubPrefix = "Sub Comp : ";
    #endregion

    #region Blind Radar Settings
    public bool EnableBlindRadar = false;
	public float RadarUpdateTime = 0.5f;
	public float RadarResetTime = 5.0f;
	#endregion
	
	#region Pulse Radar Settings
	public enum _pulseSettings {Disabled, PulseOnly, PulseAndAnimationLinked, PulseAndAnimationUnlinked, AnimationOnly}
	public _pulseSettings PulseSettings;
	public bool EnablePulseRadar;
	public bool RadarPulse;
	public float RadarPulseTime = 0.5f;
	public float PulseDuration = 2.0f;
	float RadarPulseTimer;
	float AnimPulseTimer = 1.5f;
	public bool PulseSoundSyncAnim;
	public Material PulseMat;
	#endregion
	
	#region Target/NAV List & Current Target/NAV Info
	public bool EnableTargetList;
	
	//Target List
	public RectTransform _TargetList;
	public RectTransform _Content;
	
	public enum _listTargets {DisableList, ListNAV, ListAllTargets, ListNeutralTargets, ListFriendlyTargets, ListHostileTargets, ListOwnedTargets, ListAbandonedTargets, ListObjectiveTargets}
	public _listTargets ListTargets;
	
	public List<Transform> TargetListAll = new List<Transform>(); //A list contaning all avaliable targets that are in the Players radar range.
	public List<Transform> TargetListNeutral = new List<Transform>(); //A list contaning all Neutral targets that are in the Players radar range.
	public List<Transform> TargetListFriendly = new List<Transform>(); //A list contaning all Friendly targets that are in the Players radar range.
	public List<Transform> TargetListHostile = new List<Transform>(); //A list contaning all Hostile targets that are in the Players radar range.
	public List<Transform> TargetListAband = new List<Transform>(); //A list contaning all Abandoned targets that are in the Players radar range.
	public List<Transform> TargetListOwned = new List<Transform>(); //A list contaning all Owned targets that are in the Players radar range.
	public List<Transform> TargetListObj = new List<Transform>(); //A list contaning all Owned targets that are in the Players radar range.
	public List<Transform> TargetListSubC = new List<Transform>(); //A list contaning the Sub Compoents of a selected target.
	public Transform[] TargetStored = new Transform[4]; // A list contaning the players quick selection stored targets.
	
	public Transform CurTarget; //The currently selected target.
	Transform CurTargetSub; //The selected Sub Component of the Current Target.
	
	int CurTargetCnt;
	int CurTargetSubCnt;
	#endregion
	
	#region MISC Globals
	public int ProjectileVelocity = 100;
	public float VFBounds;
	public float CurTime;
	public int ScreenHeight;
	public int ScreenWidth;
	#endregion
	
	#region Global Bools
	
	public bool SelectableHUDID = true; // Target mouse selectable VIA HUD ID icon 
	public bool SelectableRID = true; // Target mouse selectable VIA radar RID icon
	public bool SelectableObj = true; // Target mouse selectable VIA target Object
	public bool SelectableSub = true; // Target Sub Component mouse selectabe VIA sub component Object
	
	public bool RenderNAVRadar; // Display the NAV in the radar
	public bool DisableTSIOS = true;  // Disable main target selection indicator when target is off screen
	
	public bool RenderHUDID;
	public bool RenderHUDDIA;
	public bool RenderBounds;
	public bool AdvancedBounds = true;
	public bool RenderTLI = true;
	public bool RenderTSIID = true; // Display the Target Selection Indicator target ID icon 
	public bool RenderVDI = true; // Display the radars Vertical Directional Indicator line. Will also disable the RID Base icon
	public bool RenderRIDB = true; // Display the radars RID Base icon.
	
	bool HUDEnabled = true;
	public bool TSIEnabled = true;
	bool TSI2Enabled = true;
	bool DIAEnabled = true;
	bool RTSIEnabled = true;
	bool TLIEnabled = true;
	bool TSISCEnabled = true;
	
	public bool G_DisplayBounds = true;
	public bool G_DisplayHUDID = true;
	
	public bool HUDIAsIcon;
	public bool FadeInvert;
	public bool FadeBounds;
	public bool FadeHUDID;
	public bool FadeTSI;
	public bool FadeRID;
	public bool FadeTLI;
	public bool POINoFade = true;
	public bool UseObjectiveColor;
	
	public bool FilterHostile;
	public bool TargetOffScreen;
	
	public bool AutoTargetDestroy;
	#endregion
	
	#region HUD Elements
	public RectTransform TSIID; //The Target Selection Indicator Target ID Icon.
	public RectTransform TSI; //The Target Selection Indicator.
	public RectTransform TSISC; //The Sub Component Target Selection Indicator
	public RectTransform TLI; //The Target Lead Indicator.
	public RectTransform TSI2; //The HUD ID Target Selectioni Indicator
	public RectTransform RTSI; //The Radar Target Selection Box.
	public RectTransform RadarNAV; //The Radar NAV Indicator
	public Transform DefaultListButton; //The default button for the Target List Window.
	RectTransform DIA;
	
	public Image Image_TSI;
	public Image Image_TSIID;
	public Image Image_TSISC;
	public Image Image_TLI;
	public Image Image_TSI2;
	public Image Image_RTSI;
	public Image Image_DIA;
	
	public Sprite Sprite_RHTSI; // Radar & HUD ID Target Selection Indicator
	public Sprite Sprite_TLI;
	public Sprite Sprite_SCTSI;
	public Sprite Sprite_HUDID;
	public Sprite Sprite_RIDBase;
	public Sprite Sprite_RadarNAV;
	public Sprite Sprite_DIA;
	public Sprite Sprite_NAV;
	public Sprite Sprite_BoundSquare;
	public Sprite Sprite_SolidColor;
	#endregion
	
	#region HUD Screen Padding
	public int HUDMainOffset = 32;
	public int HUDIDPadding = 16;
	public Vector3 NAVHUDOffset;
	public Vector3 NAVRIDOffset;
	#endregion
	
	#region IFF Colors
	//Color Key : 0 = Color Neutral, 1 = Color Friendly, 2 = Color Hostile, 3 = Color Unknown, 4 = Color Abandoned, 5 = Color Player Owned, 6 = Color Objective 7 = Color NAV
	public Color32[] IFFColor = new Color32[8]{
		new Color32(255,255,255,255), 
		new Color(0,255,0,255),
		new Color32(255,0,0,255), 
		new Color32(128,128,128,255), 
		new Color32(255,0,255,255),
		new Color32(0,255,255,255), 
		new Color32(255, 255, 26, 255),
		new Color32(255, 128, 0, 255)};
	
	public byte HUDAlpha = 255;
	public byte RadarRIDAlpha = 255;
	public byte RadarVDIAlpha = 50;
	public byte MinFadeAmount = 25;
	#endregion
	
	#region Bounding Indicator Settings
	public bool LBS;
	public int MBS = 64;
	public int BPadding = 15;
	#endregion
	
	#region Directional Indicator Arrow Settings
	public bool RenderDIA; // Directional Indicator Arrow
	public int HUDDIARad = 120; // The radius of the Directional Indicator Arrow
	#endregion
	
	#region Target Lead Settings
	//Player Information
	public bool UsePlayerPhysics;
	Rigidbody PlayerRigidbody;
	Vector3 PlayerPreviousPos;
	
	//Target Information
	public bool UseTargetPhysics;
	Rigidbody TargetRigidbody;
	Vector3 TargetPreviousPos;
	#endregion
	
	public Vector2 RIDBOffset = new Vector2(-1,-1);
	
	//Render Radar To Texture
	public Color32 BGColor = new Color32(0,0,0,0);
	public RenderTexture RT;
	public Material RenderMaterial;
	public GameObject RenderTarget;
	
	//Audio
	public bool PlaySoundsStart;
	public AudioClip ACCycleTarget;
	public AudioClip ACClearTarget;
	public AudioClip ACWarning;
	public AudioClip ACNewContact;
	public AudioClip ACPulse;
	
	//Radar Pointer
	public Transform Pointer;
	public enum pointerStyle {Disabled, RotationPlanar, Rotation3D }
	public pointerStyle PointerStyle;
	public Transform PointerTarget;
	public bool PointerEnabled = true;
	public bool DisableInRange = true;
	public bool NAVPointer = true;
	
	//Radar Material
    public Texture2D RadarNormal, RadarZoom2, RadarZoom4, RadarBoost15, RadarBoost2;
	public Material RadarSphere;
	public Material RadarPlane;
	public float LineWidthO = 0.48f;
	public float LineWidthP = 0.15f;
	
	void Start(){

        transform.position = Vector3.zero;
        transform.eulerAngles = Vector3.zero;
        transform.localScale = Vector3.one;

		//Cache Globals
		FXIM = GetComponent<FX_Input_Mgr>();
		FXFM = GetComponent<FX_Faction_Mgr>();
		FXCM = GetComponent<FX_Class_Mgr>();
		FXMM = GetComponent<FX_Mission_Mgr>();
	
		MakeContainer("HUDIDs");
		MakeContainer("BoundCorners");
		MakeContainer("RadarTargets");
		
		//Disable Global SubComponent targeting based on FX_Input_Mgr SubComponent settings
		if(!FXIM.EnableSubComponents) {
			SelectableSub = false;
		}
	
		if(UsePlayerPhysics){
			PlayerRigidbody = Player.GetComponent<Rigidbody>();
		}
	
		PlayerCameraT = PlayerCameraC.transform;
		ScreenHeight = (int)Screen.height;
		ScreenWidth = (int)Screen.width;
	
		TSI.sizeDelta = TSI.FindChild("TSI").GetComponent<RectTransform>().sizeDelta;
		Image_TSI = TSI.FindChild("TSI").GetComponent<Image>();
		TSIID = Image_TSIID.GetComponent<RectTransform>();
	
		MakeVFB("VFB", transform);
	
		if(EnableBlindRadar) {
			foreach(int i in BlindRadarLayers){
				BRL |= (1 << i);
			}
		}
	
		RadarMask = (1 << RadarLayer);
		RadarCamera.cullingMask = RadarMask;
	
		if(RadarG) {
			Transform[] child = RadarG.GetComponentsInChildren<Transform>();
	
			foreach (Transform c in child) {
				c.gameObject.layer = RadarLayer;
			}
		}
	
		if(Pointer) {
			Transform[] child = Pointer.GetComponentsInChildren<Transform>();
	
			foreach (Transform c in child) {
				c.gameObject.layer = RadarLayer;
			}
		}
	
		//Initalize The Radar
		SetRadarRange();
		RadarCameraSetup();
	
		//Create The HUD Elements
	
		CreateHUD ();
	
		if(RadarPos == _radarPos.RenderToTexture){
			Canvas C = new GameObject ("RadarRTTC").AddComponent<Canvas>();
			C.renderMode = RenderMode.ScreenSpaceCamera;
			C.worldCamera = RadarCamera;
			C.gameObject.layer = RadarLayer;
			C.planeDistance = 0.5f;
			C.pixelPerfect = true;
			C.GetComponent<RectTransform> ().SetParent (GameObject.Find("_GameMgr").GetComponent<Transform>());
	
	
			RectTransform RTC = GameObject.Find ("RadarTargets").GetComponent<RectTransform> ();
			RTC.SetParent(C.GetComponent<RectTransform>());
			RTC.anchorMax = Vector2.zero;
			RTC.anchorMin = Vector2.zero;
			RTC.anchoredPosition = Vector3.zero;
	
			RadarCamera.targetTexture = RT;
			RadarCamera.clearFlags = CameraClearFlags.SolidColor;
			RadarCamera.backgroundColor = BGColor;
			RadarCamera.rect = new Rect (0,0,1,1);
			RenderTarget.GetComponent<Renderer> ().material = RenderMaterial;
		}
	
		if (!EnableTargetList) {
			_TargetList.gameObject.SetActive (false);
		} else {
			UpdateDisplayedList ();
		}
	}
	
	/*************************************************************************************************************************************/
	//	--------------------------------------------------------------Late Update------------------------------------------------------------------------------------------------
	/*************************************************************************************************************************************/
	void LateUpdate(){
	
		CurTime = Time.time;
		PlayerPos = Player.position;
		PlayerCamPos = PlayerCameraT.position;
	
		if(RadarEnabled){
			if(ScreenHeight != Screen.height){
				ResetScale();
			}
	
			InputMonitor();
			MouseSelectObject();
	
			if(CurTarget){
				Radar();
			}else{
				DisableTargetIndicators();
			}
		}else{
			DisableTargetIndicators();
			return;
		}
	
		switch(PulseSettings) {
			case _pulseSettings.PulseOnly:
				PulseRadar();
			break;
	
			case _pulseSettings.PulseAndAnimationLinked:
				AnimPulseRadar();
			break;
	
			case _pulseSettings.PulseAndAnimationUnlinked:
				PulseRadar();
				AnimPulseRadar();
			break;
	
			case _pulseSettings.AnimationOnly:
				AnimateRadar();
			break;
		}
		DrawPointer();
	}
	
	void PulseRadar() {
		if(CurTime > RadarPulseTimer) {
			if(!RadarPulse) {
				RadarPulse = true;
	
				if (!PulseSoundSyncAnim) {
					PlayPulseSound();
				}
	
				return;
			}
			RadarPulseTimer = CurTime + RadarPulseTime;
			RadarPulse = false;
		}
	}
	
	void AnimPulseRadar() {
		if (AnimPulseTimer > 1.0f) {
			AnimPulseTimer = 0.0f;
	
			if(PulseSettings == _pulseSettings.PulseAndAnimationLinked) {
				RadarPulse = true;
			}
	
			if (PulseSoundSyncAnim) {
				PlayPulseSound();
			}
	
		}else{
			AnimPulseTimer += Time.deltaTime / PulseDuration;
			PulseMat.mainTextureOffset = new Vector2(AnimPulseTimer, 0);
				if(PulseSettings == _pulseSettings.PulseAndAnimationLinked) {
				RadarPulse = false;
				}
		}
	}
	
	void AnimateRadar() {
		if (AnimPulseTimer >1.0f) {
			AnimPulseTimer = 0.0f;
			PlayPulseSound();
		}else{
			AnimPulseTimer += Time.deltaTime / PulseDuration;
			PulseMat.mainTextureOffset = new Vector2(AnimPulseTimer, 0);
		}
	}
	
	public void SelectRadarStyle(){// Switch between Perspective & Orthographic Camera Views
		PerspectiveRadar = !PerspectiveRadar;
	
		if(PerspectiveRadar){
			RadarCamera.orthographic = false;
			if(RadarSphere) {
				RadarSphere.SetFloat("_LineWidth", LineWidthP);
			}
		}else{
			RadarCamera.orthographic = true;
			if(RadarSphere) {
				RadarSphere.SetFloat("_LineWidth", LineWidthO);
			}
		}
	}
	
	public void Radar3D_2D(){// Switch between 2D / 3D Camera views
		Radar2D = !Radar2D;
		if(Radar2D){
			RadarCamera.orthographic = true;
			RadarCamera.transform.eulerAngles = new Vector3(90,0,0);
			RadarCamera.transform.position = new Vector3(0,0.85f, 0);
		}else{
			RadarCamera.transform.eulerAngles = new Vector3(42,0,0);
			RadarCamera.transform.position = new Vector3(0,0.85f, -0.93f);
			if(PerspectiveRadar){
				RadarCamera.orthographic = false;
			}else{
				RadarCamera.orthographic = true;
			}
		}
	}
	
	/*************************************************************************************************************************************/
	//																						Radar Main Loop
	/*************************************************************************************************************************************/
	public void Radar(){
		Vector3 TargetPos = CurTarget.position;
		Vector3 TargetSubPos = TargetPos;
	
		if(CurTargetSub){
			TargetSubPos = CurTargetSub.position;
		}
	
		TSI.anchoredPosition = GetScreenPos(TargetPos, true, HUDMainOffset);
	
		if (CurTargetSub) {
			TSISC.anchoredPosition = GetScreenPos (TargetSubPos, false, 0);
			FindTargetLead (TargetSubPos);
		} else {
			FindTargetLead (TargetPos);
		}
	
		if(RenderHUDDIA){
			DrawDIA(CurTarget, DIA);
		}
	}
	
	void FindTargetLead(Vector3 TargetPos){
		float SmoothDT = Time.smoothDeltaTime;
		
		//Get Player Velocity
		Vector3 PlayerVel = Vector3.zero;
		Vector3 TargetVel = Vector3.zero;
		
		if (UsePlayerPhysics) {
			PlayerVel = PlayerRigidbody.velocity;
		} else {
			PlayerVel = (PlayerPos - PlayerPreviousPos) / SmoothDT;
			PlayerPreviousPos = PlayerPos;
		}
		
		//Get Target Velocity
		if(UseTargetPhysics){
			TargetVel = TargetRigidbody.velocity;
		}else{
			TargetVel = (TargetPos - TargetPreviousPos) / SmoothDT;
			TargetPreviousPos = TargetPos;
		}
	
		if(RenderTLI){
			TLI.anchoredPosition = TLIScreenPos(TargetLead.FirstOrderIntercept(PlayerPos, PlayerVel, TargetPos, TargetVel, ProjectileVelocity));
		}
	}
	
	Vector3 TLIScreenPos(Vector3 Target){
		Vector3 ScreenPos = PlayerCameraC.WorldToScreenPoint(Target);
		bool IsFront = Vector3.Dot (PlayerCameraT.TransformDirection(Vector3.forward), Target - PlayerCameraT.position) > 0;
		bool OffScreen = (ScreenPos.x > ScreenWidth || ScreenPos.x < 0 || ScreenPos.y > ScreenHeight || ScreenPos.y < 0);
	
		if(IsFront && !OffScreen){
			EnableTLI();
		}else if(!IsFront || OffScreen){
			DisableTLI();
		}
		return ScreenPos;
	}
	
	Vector3 GetScreenPos(Vector3 Target, bool IsTSI, int Offset){
		Vector3 ScreenPos = PlayerCameraC.WorldToScreenPoint(Target);
	
		TargetOffScreen = (ScreenPos.x > ScreenWidth || ScreenPos.x < 0 || ScreenPos.y > ScreenHeight || ScreenPos.y < 0 || ScreenPos.z <= 0.01);
	
		if (TargetOffScreen){
			if(IsTSI && DisableTSIOS){
				DisableTSI();
				DisableTSISC();
			}else{
				RaycastHit Hit;
				Vector3 TRelPos;
				TRelPos = PlayerCameraT.InverseTransformPoint(Target);
	
				if(IsTSI){
					EnableTSI();
					DisableTSISC();
					//Future Change TSI Texture
				}
				
				if(ScreenPos.x ==0.5f && ScreenPos.y ==0.5f){
					TRelPos.y = 1;
				}
	
				Physics.Raycast (Vector3.zero, new Vector3(TRelPos.x, TRelPos.y, 0),out Hit, 2, RadarMask);
				ScreenPos = new Vector3((((VFBounds * 0.5f) + Hit.point.x) / VFBounds) * ScreenWidth, (0.5f + Hit.point.y) * ScreenHeight, 0 );
			}
		}else{
			if(IsTSI){
				EnableTSI();
				//Future Change TSI Texture
			}else if(CurTargetSub) {
				//Enable The Sub Component Target Selection Box If Active Sub Component Is Selected
				EnableTSISC();
			}
		}
	
		// Screen Edge Offset
		if(ScreenPos.x < Offset){
			ScreenPos.x = Offset;
		}
		
		if(ScreenPos.x > ScreenWidth - Offset){
			ScreenPos.x = ScreenWidth - Offset;
		}
	
		if(ScreenPos.y < Offset){
			ScreenPos.y = Offset;
		}
		
		if(ScreenPos.y > ScreenHeight - Offset){
			ScreenPos.y = ScreenHeight - Offset;
		}
		
		ScreenPos.x = Mathf.Round(ScreenPos.x);
		ScreenPos.y = Mathf.Round(ScreenPos.y);
	
		return ScreenPos;
	}
	
	void DrawDIA(Transform ThisObject, Transform ThisIndicator){
		Vector3 ThisScreenPos = GetScreenPos(ThisObject.position, false, HUDMainOffset);
		Vector3 DIAPos = DIA.position;
		ThisScreenPos.z = DIAPos.z;
		Vector3 dir = ThisScreenPos - DIAPos;
	
		float angle = (Mathf.Atan2(dir.y, dir.x)) * Mathf.Rad2Deg;
		DIA.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
	
		if(dir.sqrMagnitude < (HUDDIARad * HUDDIARad)){
			DisableDIA();
		}else{
			EnableDIA();
		}
	}
	
	void DrawPointer() {
	
		if(PointerStyle != pointerStyle.Disabled && PointerTarget) {
	
			if(DisableInRange){
				bool InRange = Vector3.SqrMagnitude(PointerTarget.position - PlayerPos) < RadarRangeSQR;
	
				if(InRange && PointerEnabled) {
					DisablePointer();
				}
	
				if (!InRange && !PointerEnabled) {
					EnablePointer();
				}
			}else if(!PointerEnabled) {
				EnablePointer();
			}
	
			if(PointerEnabled) {
				Vector2 AngleUDLR = new Vector4(180,180); //Rotation Limits
				
				if(PointerStyle == pointerStyle.RotationPlanar) {
					AngleUDLR = new Vector4(0,180);
				}
	
				Vector3 relPos1 = Player.InverseTransformPoint(PointerTarget.position); //Target position relative to the Player based around world 0,0,0
				Vector3 newPos1 = new Vector3(relPos1.x, 0, relPos1.z);
				Vector3 newPos2 = new Vector3(0, relPos1.y, newPos1.magnitude);
	
				float angle1 = Vector3.Angle(newPos1, new Vector3(0,0,1)); //Angle for Y rotation
				float angle2 = Vector3.Angle(newPos2, new Vector3(0,0,1)); //Angle for X rotation
				float absAngle1 = Mathf.Abs(angle1);
	
				if(Vector3.Dot(new Vector3(1,0,0), newPos1) < 0){ //LR angle adjust
					angle1 = -angle1;
				}
	
				if(Vector3.Dot(new Vector3(0,1,0), newPos2) > 0){ //UD angle adjust
					angle2 = -angle2;
				}
	
				float x = ClampAngle(angle2, -AngleUDLR.x, AngleUDLR.x); //Up & Down Limits
				float y = ClampAngle(angle1, -AngleUDLR.y, AngleUDLR.y); //Left & Right Limits
	
				if((AngleUDLR.y + AngleUDLR.y) != 360){
	
					float leaY = Pointer.localEulerAngles.y;
	
					if(angle1 < 0 && absAngle1 > AngleUDLR.y || angle1 > 0 && absAngle1 > AngleUDLR.y){ // Target is behind
						y = 0;
						x = 0;
				
					}else if(angle1 < 0 && leaY > 10 && leaY < 180){ // Target is left, Pointer is facing right
						y = 0;
					}else if(angle1 > 0 && leaY < 350 && leaY > 180){// Target is right, Pointer is facing left
						y = 0;
					}
				}
	
				if(angle1 <= 0){
					angle1 += 360;
				}
		
				if(angle2 <= 0 ){
					angle2 += 360;
				}
	
				//Rotate the pointer
				Pointer.localEulerAngles = new Vector3(x,y,0);
			}
		}else if(PointerEnabled) {
			DisablePointer();
		}
	}
	
	float ClampAngle (float x , float min, float max) {
		if (x >= 360) {
			x -=360;
		}
		return Mathf.Clamp (x, min, max);
	}
	
	public void SetRadarRange(){
        string zoom = "";
		float RadarZoomAmount = 0.0f;

		switch ((int)RadarZoom){
		case 0:
			    RadarZoomAmount = 0.5f;
                RadarPlane.SetTexture("_MainTex", RadarZoom4);
                zoom = "0.5";
			break;
			
		case 1:
			RadarZoomAmount = 0.75f;
            RadarPlane.SetTexture("_MainTex", RadarZoom2);
            zoom = "0.75";
			break;
			
		case 2:
			RadarZoomAmount = 1;
            RadarPlane.SetTexture("_MainTex", RadarNormal);
            zoom = "1";
			break;
			
		case 3:
            RadarZoomAmount =1.5f;
            RadarPlane.SetTexture("_MainTex", RadarBoost15);
            zoom = "1.5";
			break;
			
		case 4:
            RadarZoomAmount =2f;
            RadarPlane.SetTexture("_MainTex", RadarBoost2);
            zoom = "2";
			break;
		}
	
		RadarRangeSQR = (RadarRange * RadarZoomAmount) / WorldScale;
		RadarLocalScale = (1 / (RadarRangeSQR * 2));
		RadarRangeSQR *= RadarRangeSQR;

        if(TextRadarRange) {
            float range = (Mathf.Sqrt(RadarRangeSQR) * WorldScale);
            string thisRange = "";

			if(range < 1000){
				thisRange = range.ToString("0." + SuffixMeters);
			}else{
				thisRange = (range *.001).ToString("#.0" + SuffixKilometers);
			}

            TextRadarRange.text = RadarRangePrefix + thisRange;
        }
        if(TextRadarZoom) {
            TextRadarZoom.text = RadarZoomPrefix + zoom;
        }
	}
	
	/*************************************************************************************************************************************/
	//																						Monitor User Input
	/*************************************************************************************************************************************/
	void InputMonitor(){
		// Find the closest target to the player
		if((int)FXIM.TargetClosestKM == 0 && Input.GetKeyDown(FXIM.TargetClosest)){
			if(CurTarget){
				TargetRID.SetInactiveTarget();
			}
			ClosestTarget();
		}else if((int)FXIM.TargetClosestKM == 1 && Input.GetKeyDown(FXIM.TargetClosest) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
			if(CurTarget){
				TargetRID.SetInactiveTarget();
			}
			ClosestTarget();	
		}else if((int)FXIM.TargetClosestKM == 2 && Input.GetKeyDown(FXIM.TargetClosest) && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
			if(CurTarget){
				TargetRID.SetInactiveTarget();
			}
			ClosestTarget();	
		}else if((int)FXIM.TargetClosestKM == 3 && Input.GetKeyDown(FXIM.TargetClosest) && Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)){
			if(CurTarget){
				TargetRID.SetInactiveTarget();
			}
			ClosestTarget();
		}
		
		// Find the next target in the array
		if((int)FXIM.TargetNextKM == 0 && Input.GetKeyDown(FXIM.TargetNext)){
			if(CurTarget){
				TargetRID.SetInactiveTarget();
			}
			NextTarget();
		}else if((int)FXIM.TargetNextKM == 1 && Input.GetKeyDown(FXIM.TargetNext) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
			if(CurTarget){
				TargetRID.SetInactiveTarget();
			}
			NextTarget();
		}else if((int)FXIM.TargetNextKM == 2 && Input.GetKeyDown(FXIM.TargetNext) && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
			if(CurTarget){
				TargetRID.SetInactiveTarget();
			}
			NextTarget();
		}else if((int)FXIM.TargetNextKM == 3 && Input.GetKeyDown(FXIM.TargetNext) && Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)){
			if(CurTarget){
				TargetRID.SetInactiveTarget();
			}
			NextTarget();
		}
		
		// Find the previous target in the array
		if((int)FXIM.TargetPrevKM == 0 && Input.GetKeyDown(FXIM.TargetPrev)){
			if(CurTarget){
				TargetRID.SetInactiveTarget();
			}
			PreviousTarget();
		}else if((int)FXIM.TargetPrevKM == 1 && Input.GetKeyDown(FXIM.TargetPrev) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
			if(CurTarget){
				TargetRID.SetInactiveTarget();
			}
			PreviousTarget();
		}else if((int)FXIM.TargetPrevKM == 2 && Input.GetKeyDown(FXIM.TargetPrev) && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
			if(CurTarget){
				TargetRID.SetInactiveTarget();
			}
			PreviousTarget();
		}else if((int)FXIM.TargetPrevKM == 3 && Input.GetKeyDown(FXIM.TargetPrev) && Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)){
			if(CurTarget){
				TargetRID.SetInactiveTarget();
			}
			PreviousTarget();
		}
		
		if(FXIM.EnableSubComponents) {
			// Find the next Sub-component on the selected target	
			if((int)FXIM.TargetNextSKM == 0 && Input.GetKeyDown(FXIM.TargetNextS)){
				NextSubComp();
				PlaySelectSound();
			}else if((int)FXIM.TargetNextSKM == 1 && Input.GetKeyDown(FXIM.TargetNextS) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
				NextSubComp();
				PlaySelectSound();
			}else if((int)FXIM.TargetNextSKM == 2 && Input.GetKeyDown(FXIM.TargetNextS) && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
				NextSubComp();
				PlaySelectSound();
			}else if((int)FXIM.TargetNextSKM == 3 && Input.GetKeyDown(FXIM.TargetNextS) && Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)){
				NextSubComp();
				PlaySelectSound();
			}
		
			// Find the previous Sub-component on the selected target
			if((int)FXIM.TargetPrevSKM == 0 && Input.GetKeyDown(FXIM.TargetPrevS)){
				PreviousSubComp();
				PlaySelectSound();
			}else if((int)FXIM.TargetPrevSKM == 1 && Input.GetKeyDown(FXIM.TargetPrevS) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
				PreviousSubComp();
				PlaySelectSound();
			}else if((int)FXIM.TargetPrevSKM == 2 && Input.GetKeyDown(FXIM.TargetPrevS) && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
				PreviousSubComp();
				PlaySelectSound();
			}else if((int)FXIM.TargetPrevSKM == 3 && Input.GetKeyDown(FXIM.TargetPrevS) && Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)){
				PreviousSubComp();
				PlaySelectSound();
			}
		
			// Clear selected Sub-component
			if((int)FXIM.ClearSubCKM == 0 && Input.GetKeyDown(FXIM.ClearSubC)){
				PlayClearSound();
				ClearSubC();
			}else if((int)FXIM.ClearSubCKM == 1 && Input.GetKeyDown(FXIM.ClearSubC) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
				PlayClearSound();
				ClearSubC();
			}else if((int)FXIM.ClearSubCKM == 2 && Input.GetKeyDown(FXIM.ClearSubC) && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
				PlayClearSound();
				ClearSubC();
			}else if((int)FXIM.ClearSubCKM == 3 && Input.GetKeyDown(FXIM.ClearSubC) && Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)){
				PlayClearSound();
				ClearSubC();
			}
		}
		// Clear selected Target
		if((int)FXIM.ClearTargetKM == 0 && Input.GetKeyDown(FXIM.ClearTarget)){
			if(CurTarget){
				PlayClearSound();
				TargetRID.SetInactiveTarget();
			}	
			ClearTarget();
		}else if((int)FXIM.ClearTargetKM == 1 && Input.GetKeyDown(FXIM.ClearTarget) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
			if(CurTarget){
				PlayClearSound();
				TargetRID.SetInactiveTarget();
			}	
			ClearTarget();
		}else if((int)FXIM.ClearTargetKM == 2 && Input.GetKeyDown(FXIM.ClearTarget) && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
			if(CurTarget){
				PlayClearSound();
				TargetRID.SetInactiveTarget();
			}	
			ClearTarget();
		}else if((int)FXIM.ClearTargetKM == 3 && Input.GetKeyDown(FXIM.ClearTarget) && Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)){
			if(CurTarget){
				PlayClearSound();
				TargetRID.SetInactiveTarget();
			}	
			ClearTarget();
		}
		
		// Find the closest Hostile to the player
		if((int)FXIM.TargetClosestHKM == 0 && Input.GetKeyDown(FXIM.TargetClosestH)){
			if(CurTarget){
				TargetRID.SetInactiveTarget();
			}	
			ClosestHostile();
		}else if((int)FXIM.TargetClosestHKM == 1 && Input.GetKeyDown(FXIM.TargetClosestH) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
			if(CurTarget){
				TargetRID.SetInactiveTarget();
			}	
			ClosestHostile();
		}else if((int)FXIM.TargetClosestHKM == 2 && Input.GetKeyDown(FXIM.TargetClosestH) && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
			if(CurTarget){
				TargetRID.SetInactiveTarget();
			}	
			ClosestHostile();
		}else if((int)FXIM.TargetClosestHKM == 3 && Input.GetKeyDown(FXIM.TargetClosestH) && Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)){
			if(CurTarget){
				TargetRID.SetInactiveTarget();
			}	
			ClosestHostile();
		}
		
		// Find the next Hostile in the array
		if((int)FXIM.TargetNextHKM == 0 && Input.GetKeyDown(FXIM.TargetNextH)){
			if(CurTarget){
				TargetRID.SetInactiveTarget();
			}	
			NextHostile();
		}else if((int)FXIM.TargetNextHKM == 1 && Input.GetKeyDown(FXIM.TargetNextH) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
			if(CurTarget){
				TargetRID.SetInactiveTarget();
			}	
			NextHostile();
		}else if((int)FXIM.TargetNextHKM == 2 && Input.GetKeyDown(FXIM.TargetNextH) && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
			if(CurTarget){
				TargetRID.SetInactiveTarget();
			}	
			NextHostile();
		}else if((int)FXIM.TargetNextHKM == 3 && Input.GetKeyDown(FXIM.TargetNextH) && Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)){
			if(CurTarget){
				TargetRID.SetInactiveTarget();
			}	
			NextHostile();
		}
		
		// Find the previous Hostile in the array
		if((int)FXIM.TargetPrevHKM == 0 && Input.GetKeyDown(FXIM.TargetPrevH)){
			if(CurTarget){
				TargetRID.SetInactiveTarget();
			}	
			PreviousHostile();
		}else if((int)FXIM.TargetPrevHKM == 1 && Input.GetKeyDown(FXIM.TargetPrevH) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
			if(CurTarget){
				TargetRID.SetInactiveTarget();
			}	
			PreviousHostile();
		}else if((int)FXIM.TargetPrevHKM == 2 && Input.GetKeyDown(FXIM.TargetPrevH) && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
			if(CurTarget){
				TargetRID.SetInactiveTarget();
			}	
			PreviousHostile();
		}else if((int)FXIM.TargetPrevHKM == 3 && Input.GetKeyDown(FXIM.TargetPrevH) && Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)){
			if(CurTarget){
				TargetRID.SetInactiveTarget();
			}	
			PreviousHostile();
		}
	
		// Display / Hide Target List
		if(FXIM.TargetListKM == 0 && Input.GetKeyDown(FXIM.TargetList)){
			EnableTargetList = !EnableTargetList;
	
			if(EnableTargetList){
				OpenTargetList();
			}else{
				CloseTargetList();
			}
	
		}else if((int)FXIM.TargetListKM == 1 && Input.GetKeyDown(FXIM.TargetList) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
			EnableTargetList = !EnableTargetList;
		}else if((int)FXIM.TargetListKM == 2 && Input.GetKeyDown(FXIM.TargetList) && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
			EnableTargetList = !EnableTargetList;
		}else if((int)FXIM.TargetListKM == 3 && Input.GetKeyDown(FXIM.TargetList) && Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)){
			EnableTargetList = !EnableTargetList;
		}
	
	
		// Filter Hostile
		if(FXIM.FilterHostileKM == 0 && Input.GetKeyDown(FXIM.FilterHostile)){
			FilterHostile = !FilterHostile;
		}else if((int)FXIM.FilterHostileKM == 1 && Input.GetKeyDown(FXIM.FilterHostile) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
			FilterHostile = !FilterHostile;
		}else if((int)FXIM.FilterHostileKM == 2 && Input.GetKeyDown(FXIM.FilterHostile) && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
			FilterHostile = !FilterHostile;
		}else if((int)FXIM.FilterHostileKM == 3 && Input.GetKeyDown(FXIM.FilterHostile) && Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)){
			FilterHostile = !FilterHostile;
		}
	
		// Switch 3D 2D Radar
		if(FXIM.Switch3D2DKM == 0 && Input.GetKeyDown(FXIM.Switch3D2D)){
			Radar3D_2D();
		}else if((int)FXIM.Switch3D2DKM == 1 && Input.GetKeyDown(FXIM.Switch3D2D) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
			Radar3D_2D();
		}else if((int)FXIM.Switch3D2DKM == 2 && Input.GetKeyDown(FXIM.Switch3D2D) && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
			Radar3D_2D();
		}else if((int)FXIM.Switch3D2DKM == 3 && Input.GetKeyDown(FXIM.Switch3D2D) && Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)){
			Radar3D_2D();
		}
		
		//Store Targets 1
		if(FXIM.StoreTarget1KM == 0 && Input.GetKeyDown(FXIM.StoreTarget1)){
			if(CurTarget) {
				TargetStored[0] = CurTarget;
			}
		}else if((int)FXIM.StoreTarget1KM == 1 && Input.GetKeyDown(FXIM.StoreTarget1) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
			if(CurTarget) {
				TargetStored[0] = CurTarget;
			}
		}else if((int)FXIM.StoreTarget1KM == 2 && Input.GetKeyDown(FXIM.StoreTarget1) && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
			if(CurTarget) {
				TargetStored[0] = CurTarget;
			}
		}else if((int)FXIM.StoreTarget1KM == 3 && Input.GetKeyDown(FXIM.StoreTarget1) && Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)){
			if(CurTarget) {
				TargetStored[0] = CurTarget;
			}
		}
	
		//Store Targets 2
		if(FXIM.StoreTarget2KM == 0 && Input.GetKeyDown(FXIM.StoreTarget2)){
			if(CurTarget) {
				TargetStored[1] = CurTarget;
			}
		}else if((int)FXIM.StoreTarget2KM == 1 && Input.GetKeyDown(FXIM.StoreTarget2) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
			if(CurTarget) {
				TargetStored[1] = CurTarget;
			}
		}else if((int)FXIM.StoreTarget2KM == 2 && Input.GetKeyDown(FXIM.StoreTarget2) && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
			if(CurTarget) {
				TargetStored[1] = CurTarget;
			}
		}else if((int)FXIM.StoreTarget2KM == 3 && Input.GetKeyDown(FXIM.StoreTarget2) && Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)){
			if(CurTarget) {
				TargetStored[1] = CurTarget;
			}
		}
	
		//Store Targets 3
		if(FXIM.StoreTarget3KM == 0 && Input.GetKeyDown(FXIM.StoreTarget3)){
			if(CurTarget) {
				TargetStored[2] = CurTarget;
			}
		}else if((int)FXIM.StoreTarget3KM == 1 && Input.GetKeyDown(FXIM.StoreTarget3) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
			if(CurTarget) {
				TargetStored[2] = CurTarget;
			}
		}else if((int)FXIM.StoreTarget3KM == 2 && Input.GetKeyDown(FXIM.StoreTarget3) && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
			if(CurTarget) {
				TargetStored[2] = CurTarget;
			}
		}else if((int)FXIM.StoreTarget3KM == 3 && Input.GetKeyDown(FXIM.StoreTarget3) && Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)){
			if(CurTarget) {
				TargetStored[2] = CurTarget;
			}
		}
	
		//Store Targets 4
		if(FXIM.StoreTarget4KM == 0 && Input.GetKeyDown(FXIM.StoreTarget4)){
			if(CurTarget) {
				TargetStored[3] = CurTarget;
			}
		}else if((int)FXIM.StoreTarget4KM == 1 && Input.GetKeyDown(FXIM.StoreTarget4) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
			if(CurTarget) {
				TargetStored[3] = CurTarget;
			}
		}else if((int)FXIM.StoreTarget4KM == 2 && Input.GetKeyDown(FXIM.StoreTarget4) && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
			if(CurTarget) {
				TargetStored[3] = CurTarget;
			}
		}else if((int)FXIM.StoreTarget4KM == 3 && Input.GetKeyDown(FXIM.StoreTarget4) && Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)){
			if(CurTarget) {
				TargetStored[3] = CurTarget;
			}
		}
	
		//Recall Stored Target 1
		if(FXIM.SelectStoredTarget1KM == 0 && Input.GetKeyDown(FXIM.SelectStoredTarget1)){
			if(TargetStored[0] != null) {
				SetTarget(TargetStored[0]);
			}
		}else if((int)FXIM.SelectStoredTarget1KM == 1 && Input.GetKeyDown(FXIM.SelectStoredTarget1) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
			if(TargetStored[0]) {
					SetTarget(TargetStored[0]);
			}
		}else if((int)FXIM.SelectStoredTarget1KM == 2 && Input.GetKeyDown(FXIM.SelectStoredTarget1) && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
			if(TargetStored[0]) {
					SetTarget(TargetStored[0]);
			}
		}else if((int)FXIM.SelectStoredTarget1KM == 3 && Input.GetKeyDown(FXIM.SelectStoredTarget1) && Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)){
			if(TargetStored[0]) {
					SetTarget(TargetStored[0]);
			}
		}
	
		//Recall Stored Target 2
		if(FXIM.SelectStoredTarget2KM == 0 && Input.GetKeyDown(FXIM.SelectStoredTarget2)){
			if(TargetStored[1]) {
				SetTarget(TargetStored[1]);
			}
		}else if((int)FXIM.SelectStoredTarget2KM == 1 && Input.GetKeyDown(FXIM.SelectStoredTarget2) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
			if(TargetStored[1]) {
					SetTarget(TargetStored[1]);
			}
		}else if((int)FXIM.SelectStoredTarget2KM == 2 && Input.GetKeyDown(FXIM.SelectStoredTarget2) && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
			if(TargetStored[1]) {
					SetTarget(TargetStored[1]);
			}
		}else if((int)FXIM.SelectStoredTarget2KM == 3 && Input.GetKeyDown(FXIM.SelectStoredTarget2) && Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)){
			if(TargetStored[1]) {
					SetTarget(TargetStored[1]);
			}
		}
	
		//Recall Stored Target 3
		if(FXIM.SelectStoredTarget2KM == 0 && Input.GetKeyDown(FXIM.SelectStoredTarget3)){
			if(TargetStored[2]) {
				SetTarget(TargetStored[2]);
			}
		}else if((int)FXIM.SelectStoredTarget3KM == 1 && Input.GetKeyDown(FXIM.SelectStoredTarget3) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
			if(TargetStored[2]) {
					SetTarget(TargetStored[2]);
			}
		}else if((int)FXIM.SelectStoredTarget3KM == 2 && Input.GetKeyDown(FXIM.SelectStoredTarget3) && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
			if(TargetStored[2]) {
					SetTarget(TargetStored[2]);
			}
		}else if((int)FXIM.SelectStoredTarget3KM == 3 && Input.GetKeyDown(FXIM.SelectStoredTarget3) && Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)){
			if(TargetStored[2]) {
					SetTarget(TargetStored[2]);
			}
		}
	
		//Recall Stored Target 4
		if(FXIM.SelectStoredTarget4KM == 0 && Input.GetKeyDown(FXIM.SelectStoredTarget4)){
			if(TargetStored[3] != null) {
				SetTarget(TargetStored[3]);
			}
		}else if((int)FXIM.SelectStoredTarget4KM == 1 && Input.GetKeyDown(FXIM.SelectStoredTarget4) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
			if(TargetStored[3]) {
				SetTarget(TargetStored[3]);
			}
		}else if((int)FXIM.SelectStoredTarget4KM == 2 && Input.GetKeyDown(FXIM.SelectStoredTarget4) && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
			if(TargetStored[3]) {
				SetTarget(TargetStored[3]);
			}
		}else if((int)FXIM.SelectStoredTarget4KM == 3 && Input.GetKeyDown(FXIM.SelectStoredTarget4) && Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)){
			if(TargetStored[3]) {
				SetTarget(TargetStored[3]);
			}
		}

		if(FXIM.ZoomNormalKM == 0 && Input.GetKeyDown(FXIM.ZoomNormal)){
            RadarZoom = _radarZoom.Normal;
            SetRadarRange();
		}else if((int)FXIM.ZoomNormalKM == 1 && Input.GetKeyDown(FXIM.ZoomNormal) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
            RadarZoom = _radarZoom.Normal;
            SetRadarRange();
		}else if((int)FXIM.ZoomNormalKM == 2 && Input.GetKeyDown(FXIM.ZoomNormal) && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
            RadarZoom = _radarZoom.Normal;
            SetRadarRange();
		}else if((int)FXIM.ZoomNormalKM == 3 && Input.GetKeyDown(FXIM.ZoomNormal) && Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)){
            RadarZoom = _radarZoom.Normal;
            SetRadarRange();
		}

		if(FXIM.ZoomInKM == 0 && Input.GetKeyDown(FXIM.ZoomIn)){
            if((int)RadarZoom < 4) {
                RadarZoom = (_radarZoom)(int)RadarZoom + 1;
                SetRadarRange();
            }
		}else if((int)FXIM.ZoomInKM == 1 && Input.GetKeyDown(FXIM.ZoomIn) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
            if((int)RadarZoom < 5) {
                RadarZoom = (_radarZoom)(int)RadarZoom + 1;
                SetRadarRange();
            }
		}else if((int)FXIM.ZoomInKM == 2 && Input.GetKeyDown(FXIM.ZoomIn) && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
            if((int)RadarZoom < 5) {
                RadarZoom = (_radarZoom)(int)RadarZoom + 1;
                SetRadarRange();
            }
		}else if((int)FXIM.ZoomInKM == 3 && Input.GetKeyDown(FXIM.ZoomIn) && Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)){
            if((int)RadarZoom < 5) {
                RadarZoom = (_radarZoom)(int)RadarZoom + 1;
                SetRadarRange();
            }
		}

		if(FXIM.ZoomOutKM == 0 && Input.GetKeyDown(FXIM.ZoomOut)){
            if((int)RadarZoom > 0) {
                RadarZoom = (_radarZoom)(int)RadarZoom - 1;
                SetRadarRange();
            }
		}else if((int)FXIM.ZoomOutKM == 1 && Input.GetKeyDown(FXIM.ZoomOut) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
            if((int)RadarZoom > 0) {
                RadarZoom = (_radarZoom)(int)RadarZoom - 1;
                SetRadarRange();
            }
		}else if((int)FXIM.ZoomOutKM == 2 && Input.GetKeyDown(FXIM.ZoomOut) && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
            if((int)RadarZoom > 0) {
                RadarZoom = (_radarZoom)(int)RadarZoom - 1;
                SetRadarRange();
            }
		}else if((int)FXIM.ZoomOutKM == 3 && Input.GetKeyDown(FXIM.ZoomOut) && Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)){
            if((int)RadarZoom > 0) {
                RadarZoom = (_radarZoom)(int)RadarZoom - 1;
                SetRadarRange();
            }
		}
		/*
		// Toggle Indicators
		if(FXIM.ToggleIndicatorsKM == 0 && Input.GetKeyDown(FXIM.ToggleIndicators)){
			SetStatus[13] = !SetStatus[13];
		}else if(FXIM.ToggleIndicatorsKM == 1 && Input.GetKeyDown(FXIM.ToggleIndicators) && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
			SetStatus[13] = !SetStatus[13];
		}else if(FXIM.ToggleIndicatorsKM == 2 && Input.GetKeyDown(FXIM.ToggleIndicators) && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
			SetStatus[13] = !SetStatus[13];            RTSI.SetParent(TargetRID.RIDI);
		}else if(FXIM.ToggleIndicatorsKM == 3 && Input.GetKeyDown(FXIM.ToggleIndicators) && Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)){
			SetStatus[13] = !SetStatus[13];
		}
		*/
	}
	
	public void ClearTarget(){//Targeting Input Command
		/***********************************************************************************/
		//Clear the current target
		/***********************************************************************************/
		if(CurTarget){
			TargetRID.SetInactiveTarget();
			TargetRigidbody = null;
			CurTarget = null;
		}
            if(TextTargetName) {
	            TextTargetName.text = "";
             }
            if(TextTargetDistance) {
	            TextTargetDistance.text = "";
             }
            if(TextTargetClass) {
                TextTargetClass.text = "";
            }
            if(TextSubComp) {
                TextSubComp.text = "";
            }
			ClearSubC();
	}
	
	public void SetTarget(Transform t){//Targeting Function
		/***********************************************************************************/
		//Gather the current target components & set target state
		/***********************************************************************************/
		ClearTarget();
	
		CurTarget = t;
	
		if(TargetRID){
			TargetRID.SetInactiveTarget();
		}
	
		if(CurTarget){
			PlaySelectSound();
			
			TargetRID = CurTarget.GetComponent<FX_3DRadar_RID>();
	
			Vector2 o = FXCM.ObjectClassList[TargetRID.MainClass[0]].TSIOffset[TargetRID.SubClass[0]];
	
			RTSI.SetParent(TargetRID.RIDI);
			RTSI.anchoredPosition = o;
	
			TSI2.SetParent(TargetRID.ThisHID);
			TSI2.anchoredPosition = o;
	
			TargetRID.SetAsActiveTarget();
			TargetRID.RenderToRadar(false);
			EnableTargetIndicators();
	
			if(UseTargetPhysics){
				TargetRigidbody = TargetRID.ThisRigidbody;
			}
	
			//Set The HUD Target Selection Box Target Icon
			if(RenderTSIID){
				Image_TSIID.sprite = TargetRID.Image_RIDI.sprite;
				Image_TSIID.SetNativeSize();
			}

            ClearSubC();
			FindSubComp();

            if(TextTargetName && !NameAndDistance) {
                TextTargetName.text = TargetNamePrefix + CurTarget.name.ToString();
            }

            if(TextTargetClass) {
                if(AsSubClass) {
                    TextTargetClass.text = TargetClassPrefix + FXCM.ObjectClassList[TargetRID.MainClass[0]].SubClassName[TargetRID.SubClass[0]];
                } else {
                    TextTargetClass.text = TargetClassPrefix + FXCM.ObjectClassList[TargetRID.MainClass[0]].ClassName;
                }
            }
		}
	}
	
    void SetSubComp(Transform t) {
		CurTargetSub = t;

        if(TextSubComp) {
            TextSubComp.text = TargetSubPrefix + t.name;
        }
    }

	void ClosestTarget(){//Targeting Input Command
		float closestDistance = Mathf.Infinity;
		foreach (Transform t in TargetListAll){
			float curDistance = (t.position - PlayerPos).sqrMagnitude;	
			if(curDistance <= closestDistance){
				SetTarget(t);
				closestDistance = curDistance;
			}
		}
	}
	
	void NextTarget(){//Targeting Input Command
		if(TargetListAll.Count > 0){
			CurTargetCnt = (CurTargetCnt + 1) % TargetListAll.Count;
			SetTarget(TargetListAll[CurTargetCnt]);
		}
	}
	
	void PreviousTarget(){//Targeting Input Command
		if(TargetListAll.Count > 0){
			if (CurTargetCnt == 0){
				CurTargetCnt = TargetListAll.Count;
			}
			if(CurTargetCnt > 0){
				CurTargetCnt = CurTargetCnt -1;
			}
			SetTarget(TargetListAll[CurTargetCnt]);
		}
	}
	
	public void ClosestHostile(){//Targeting Input Command
		float closestDistance = Mathf.Infinity;
		foreach (Transform t in TargetListHostile){
			float curDistance = (t.position - PlayerPos).sqrMagnitude;	
			if(curDistance <= closestDistance){
				SetTarget(t);
				closestDistance = curDistance;
			}
		}
	}
	
	void NextHostile(){//Targeting Input Command
		if(TargetListHostile.Count > 0){
			CurTargetCnt = (CurTargetCnt + 1) % TargetListHostile.Count;
			SetTarget(TargetListHostile[CurTargetCnt]);
		}
	}
	
	void PreviousHostile(){//Targeting Input Command
		if(TargetListHostile.Count > 0){
			if (CurTargetCnt == 0){
				CurTargetCnt = TargetListHostile.Count;
			}
			if(CurTargetCnt > 0){
				CurTargetCnt = CurTargetCnt -1;
			}
			SetTarget(TargetListHostile[CurTargetCnt]);
		}
	}
	
	void FindSubComp(){//Targeting Function
		/***********************************************************************************/
		//Create an array of all subcomponents on the selected target
		/***********************************************************************************/
		if(CurTarget){
			TargetListSubC.Clear();
			foreach(Transform s in CurTarget){
				if(s.tag == "Sub_Component"){
					TargetListSubC.Add(s);
				}
			}
		}
	}
	
	void NextSubComp(){//Targeting Input Command
		if(CurTarget && TargetListSubC.Count > 0){
			CurTargetSubCnt = (CurTargetSubCnt + 1) % TargetListSubC.Count;
            SetSubComp(TargetListSubC[CurTargetSubCnt]);
		}
	}
	
	void PreviousSubComp(){//Targeting Input Command
		if(CurTarget && TargetListSubC.Count > 0){
			if (CurTargetSubCnt == 0){
				CurTargetSubCnt = TargetListSubC.Count;
			}
			if(CurTargetSubCnt > 0){
				CurTargetSubCnt = CurTargetSubCnt -1;
			}
			SetSubComp(TargetListSubC[CurTargetSubCnt]);
		}
	}
	
	void ClearSubC(){//Targeting Input Command
		if(FXIM.EnableSubComponents) {
			DisableTSISC();
			CurTargetSub = null;
			CurTargetSubCnt = 0;
		}
        if(TextSubComp) {
            TextSubComp.text = "";
        }
	}
	
	void MouseSelectObject(){
		if (SelectableObj && Input.GetMouseButtonDown(0)){
			LayerMask mask0 = (1 << 0);
			Ray ray0 = PlayerCameraC.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit0;
	
			if (Physics.Raycast(ray0, out hit0, Mathf.Infinity, mask0)){
	
				//Cache the hit information
				Transform t = hit0.transform;
				FX_3DRadar_RID r = t.GetComponent<FX_3DRadar_RID>();
	
				//Check if the selected object is a NAV
				if(t.tag != "Sub_Component" && r && r.IsNAV){
					return;
				}
	
				ClearTarget();
	
				if (!SelectableSub || SelectableSub && t.tag != "Sub_Component"){
					if (r){
						SetTarget(t);
					}
					else if (t.parent && t.parent.GetComponent<FX_3DRadar_RID>()){
						SetTarget(t.parent);
					}
				}else{
					SetTarget(t.parent);
                    SetSubComp(t);
				}
			}
		}
	}
	
	/*************************************************************************************************************************************/
	//																			Initalize The Players HUD Elements
	/*************************************************************************************************************************************/
	
	void RadarCameraSetup(){//Setup Radar Settings & Camera Viewport
		SetRadarRange();
		
		switch((int)RadarPos){
		case 1: // top left
			RadarCamera.targetTexture = null;
			RadarCamera.rect = new Rect( 1 - (RadarCamera.rect.x + RadarCamera.rect.width), 1 - (RadarCamera.rect.y + RadarCamera.rect.height), RadarCamera.rect.width, RadarCamera.rect.height);
			break;
			
		case 2: // top right
			RadarCamera.targetTexture = null;
			RadarCamera.rect = new Rect(RadarCamera.rect.x, 1 - (RadarCamera.rect.y + RadarCamera.rect.height), RadarCamera.rect.width, RadarCamera.rect.height);
			break;
			
		case 3: // bottom left
			RadarCamera.targetTexture = null;
			RadarCamera.rect = new Rect( 1 - (RadarCamera.rect.x +  RadarCamera.rect.width), RadarCamera.rect.y, RadarCamera.rect.width, RadarCamera.rect.height);
			break;
			
		case 4: // bottom right
			RadarCamera.targetTexture = null;
			RadarCamera.rect =new Rect(RadarCamera.rect.x, RadarCamera.rect.y, RadarCamera.rect.width, RadarCamera.rect.height);
			break;
		}
	}
	
	void ResetScale(){
		ScreenHeight = (int)Screen.height;
		ScreenWidth = (int)Screen.width;
		
		//Rebuild The View Fustrum
		Destroy(GameObject.Find("VFB"));
		MakeVFB("VFB", transform);
	}
	
	void CreateHUD(){
	
		Transform radarUI = GameObject.Find("Radar UI").transform;
	
		//Make The HUD Sub Component Target Selection Box
		TSISC = MakeImage("HUD_TSISC", 0.0f, radarUI);
		Image_TSISC = SetImage(TSISC.GetComponent<Image>(), Sprite_SCTSI, false);
	
		if(RenderTLI){
			//Make The HUD Target Lead Indicator
			TLI = MakeImage("HUD_TLI", 0.0f, radarUI);
			Image_TLI = SetImage(TLI.GetComponent<Image>(), Sprite_TLI, false);
		}
	
		//Make The Target Selection Box Screen Edge Indicator
		TSI2 = MakeImage("HUD_TSI2",0.0f, GameObject.Find("HUDIDs").transform);
		TSI2.pivot = Vector2.zero;
		Image_TSI2 = SetImage(TSI2.GetComponent<Image>(), Sprite_RHTSI, false);
	
		//Make The Radar Target Selection Box
		RTSI = MakeImage("Radar_TSI",0.0f, GameObject.Find("RadarTargets").transform);
		RTSI.pivot = Vector2.zero;
		Image_RTSI = SetImage(RTSI.GetComponent<Image>(), Sprite_RHTSI, false);
	
		//Make The Directional Indicator Arrow
		if(RenderHUDDIA){
			DIA = MakeImage("HUD_DIA_Root", 0.5f, radarUI);
			Destroy (DIA.GetComponent<Image>());
			DIA.anchoredPosition = Vector3.zero;
	
			RectTransform newDIA = MakeImage("HUD_DIA_Arrow", 0.5f, DIA.transform);
			Image_DIA = SetImage(newDIA.GetComponent<Image>(), Sprite_DIA, false);
			newDIA.anchoredPosition = new Vector3(0,HUDDIARad,0);
		}
	
		if(!CurTarget){
			DisableTargetIndicators();
		}
	}
	
	public RectTransform MakeImage(string name, float Anchor, Transform parent){
		GameObject newImage = new GameObject(name);
		newImage.layer = 5;
		newImage.AddComponent<Image>();
		newImage.GetComponent<RectTransform>().anchorMin = new Vector2(Anchor,Anchor);
		newImage.GetComponent<RectTransform>().anchorMax = new Vector2(Anchor,Anchor);
		newImage.GetComponent<RectTransform>().SetParent(parent);
		return newImage.GetComponent<RectTransform>();
	}
	
	public Image SetImage(Image i, Sprite s, bool b) {
		i.sprite = s;
		i.raycastTarget = b;
		i.SetNativeSize();
		return i;
	}
	
	void MakeContainer(string s) {
		RectTransform UIElement = new GameObject (s).AddComponent<RectTransform> ();
		UIElement.SetParent(GameObject.Find("Radar UI").transform);
		UIElement.anchorMin = Vector2.zero;
		UIElement.anchorMax = Vector2.one;
		UIElement.pivot = Vector2.zero;
		UIElement.offsetMax = Vector2.zero;
		UIElement.offsetMin = Vector2.zero;
	}
	
	void MakeVFB(string Name, Transform ThisParent){
	
		Transform ThisMesh  = new GameObject(Name).transform;
		Mesh mesh = ThisMesh.gameObject.AddComponent <MeshFilter>().mesh;
		Vector3[] vertices = new Vector3[24];
	
		float scale = (1.0f * ((ScreenWidth  * 1.0f) / ScreenHeight)) * 0.5f;
	
		//Face Down - TOP
		vertices[0] = new Vector3( -scale,0.5f,0.5f); //bottom left
		vertices[1] =  new Vector3( -scale,0.5f, -0.5f); // Top Left
		vertices[2] =  new Vector3(scale,0.5f,0.5f); // bottom right
		vertices[3] =  new Vector3(scale,0.5f, -0.5f); // Top right
		
		//Face Up - Down
		vertices[4] = new Vector3( -scale, -0.5f, -0.5f); //bottom left
		vertices[5] =  new Vector3( -scale, -0.5f,0.5f); // Top Left
		vertices[6] =  new Vector3(scale, -0.5f, -0.5f); // bottom right
		vertices[7] =  new Vector3(scale, -0.5f,0.5f); // Top right
		
		//Face Right - Left
		vertices[8] = new Vector3( -scale, -0.5f,0.5f); //bottom left
		vertices[9] =  new Vector3( -scale, -0.5f, -0.5f); // Top Left
		vertices[10] =  new Vector3(-scale,0.5f,0.5f); // bottom right
		vertices[11] =  new Vector3(-scale,0.5f, -0.5f); // Top right
		
		//Face Left - Right
		vertices[12] = new Vector3(scale, -0.5f, -0.5f); //bottom left
		vertices[13] =  new Vector3(scale, -0.5f,0.5f); // Top Left
		vertices[14] =  new Vector3(scale,0.5f, -0.5f); // bottom right
		vertices[15] =  new Vector3(scale,0.5f,0.5f); // Top right
		
		//Face Back
		vertices[16] = new Vector3( -scale,0.5f, -0.5f); //bottom left
		vertices[17] =  new Vector3( -scale, -0.5f, -0.5f); // Top Left
		vertices[18] =  new Vector3(scale,0.5f, -0.5f); // bottom right
		vertices[19] =  new Vector3(scale, -0.5f, -0.5f); // Top right
		
		//Face Front
		vertices[20] = new Vector3( -scale, -0.5f,0.5f); //bottom left
		vertices[21] =  new Vector3( -scale,0.5f,0.5f); // Top Left
		vertices[22] =  new Vector3(scale, -0.5f,0.5f); // bottom right
		vertices[23] =  new Vector3(scale,0.5f,0.5f); // Top right
		
		Vector2[] uv = new Vector2[16];
		uv[0] =  new Vector2(0, 0);
		uv[1] =  new Vector2(0, 1);
		uv[2] =  new Vector2(1, 0);
		uv[3] =  new Vector2(1, 1);
		
		uv[4] =  new Vector2(0, 0);
		uv[5] =  new Vector2(0, 1);
		uv[6] =  new Vector2(1, 0);
		uv[7] =  new Vector2(1, 1);
		
		uv[8] =  new Vector2(0, 0);
		uv[9] =  new Vector2(0, 1);
		uv[10] =  new Vector2(1, 0);
		uv[11] =  new Vector2(1, 1);
		
		uv[12] =  new Vector2(0, 0);
		uv[13] =  new Vector2(0, 1);
		uv[14] =  new Vector2(1, 0);
		uv[15] =  new Vector2(1, 1);
		
		int[] triangles = new int[24];
		
		//Top
		triangles[0] = 0;
		triangles[1] = 1; 
		triangles[2] = 2;
		triangles[3] = 2; 
		triangles[4] = 1; 
		triangles[5] = 3;
		
		//Bottom
		triangles[6] = 4;
		triangles[7] = 5; 
		triangles[8] = 6;
		triangles[9] = 6; 
		triangles[10] = 5; 
		triangles[11] = 7;
		
		//Left
		triangles[12] = 8;
		triangles[13] = 9; 
		triangles[14] = 10;
		triangles[15] = 10; 
		triangles[16] = 9; 
		triangles[17] = 11;
		
		//Right
		triangles[18] = 12;
		triangles[19] = 13; 
		triangles[20] = 14;
		triangles[21] = 14; 
		triangles[22] = 13; 
		triangles[23] = 15;
		
		mesh.vertices = vertices;
		mesh.triangles = triangles;
	
		ThisMesh.parent = ThisParent;
		ThisMesh.localPosition = Vector3.zero;
	
		ThisMesh.gameObject.AddComponent<MeshCollider>();
		ThisMesh.gameObject.isStatic = true;
		ThisMesh.gameObject.layer = RadarLayer;
	
		VFBounds = scale * 2;
	}
	
	LayerMask LayerNumbersToMask(params int[] layerNumbers){
		LayerMask ret = (LayerMask)0;
		foreach(var layer in layerNumbers){
			ret |= (1 << layer);
		}
		return ret;
	}
	
	/*************************************************************************************************************************************/
	//																			Set HUD Display Enable / Disable
	/*************************************************************************************************************************************/
	public void EnableTargetIndicators(){
		if(!HUDEnabled) {
			EnableTSI();
			EnableRTSI();
			HUDEnabled = true;
		}
	}
	
	//Quick Disable All HUD Elements
	void DisableTargetIndicators(){
		if(HUDEnabled) {
			DisableTSI();
			DisableTSI2();
			DisableTSISC();
			DisableDIA();
			DisableTLI();
			DisableRTSI();
			HUDEnabled = false;
		}
	}
	
	//Main Target Selection Indicator
	void EnableTSI() {
		if(!TSIEnabled) {
			TSI.gameObject.SetActive(true);
			TSIEnabled = true;
		}
	}
	
	void DisableTSI(){
		if(TSIEnabled) {
			TSI.gameObject.SetActive(false);
			TSIEnabled = false;
		}
	}
	
	//Sub Component Target Selection Indicator
	void EnableTSISC() {
		if(!TSISCEnabled) {
			Image_TSISC.enabled = true;
			TSISCEnabled = true;
		}
	}
	
	void DisableTSISC(){
		if(TSISCEnabled) {
			Image_TSISC.enabled = false;
			TSISCEnabled = false;
		}
	}
	
	//HUD ID Target Selection Indicator
	public void EnableTSI2() {
		if(!TSI2Enabled) {
			Image_TSI2.enabled = true;
			TSI2Enabled = true;
		}
	}
	
	public void DisableTSI2(){
		if(TSI2Enabled) {
			Image_TSI2.enabled = false;
			TSI2Enabled = false;
		}
	}
	
	//Target Lead Indicator
	void EnableTLI(){
		if (RenderTLI && !TLIEnabled) {
			Image_TLI.enabled = true;
			TLIEnabled = true;
		}
	}
	
	void DisableTLI(){
		if(RenderTLI && TLIEnabled){
			Image_TLI.enabled = false;
			TLIEnabled = false;
		}
	}
	
	//Directional Indicator
	void EnableDIA(){
		if (RenderHUDDIA && !DIAEnabled) {
			Image_DIA.enabled = true;
			DIAEnabled = true;
		}
	}
	
	void DisableDIA(){
		if(RenderHUDDIA && DIAEnabled){
			Image_DIA.enabled = false;
			DIAEnabled = false;
		}
	}
	
	//Radar Target Selection Indicator
	public void EnableRTSI(){
		if (!RTSIEnabled) {
			Image_RTSI.enabled = true;
			RTSIEnabled = true;
		}
	}
	
	public void DisableRTSI(){
		if(RTSIEnabled){
			Image_RTSI.enabled = false;
			RTSIEnabled = false;
		}
	}
	
	public void EnablePointer() {
		if(Pointer) {
			Pointer.gameObject.SetActive(true);
			PointerEnabled = true;
		}
	}

	public void DisablePointer() {
		if(Pointer) {
			Pointer.gameObject.SetActive(false);
			PointerEnabled = false;
		}
	}
	
	/*************************************************************************************************************************************/
	//																						Sound FX
	/*************************************************************************************************************************************/	
	void PlaySelectSound(){
		if(ACCycleTarget) {
			AS.PlayOneShot(ACCycleTarget);
		}
	}
	
	public void PlayClearSound(){
		if(ACClearTarget) {
			AS.PlayOneShot(ACClearTarget);
		}
	}
	
	public void PlayNewContactSound() {
		if(ACNewContact) {
			if (PlaySoundsStart) {
				AS.PlayOneShot(ACNewContact);
			}else if(Time.time > 1) {
				AS.PlayOneShot(ACNewContact);
			}
		}
	}
	
	public void PlayHostileSound() {
		if(ACWarning) {
			if (PlaySoundsStart) {
				AS.PlayOneShot(ACWarning);
			}else if(Time.time > 1) {
				AS.PlayOneShot(ACWarning);
			}
		}
	}
	
	public void PlayPulseSound() {
		if(ACPulse) {
			AS.PlayOneShot(ACPulse);
		}
	}
	/*************************************************************************************************************************************/
	//																						Display Target List
	/*************************************************************************************************************************************/	
	
	public void DisplayTargetListAll(){
		BuildList(TargetListAll);
		ListTargets = _listTargets.ListAllTargets;
	}
	
	public void DisplayTargetListHostile(){
		BuildList(TargetListHostile);
		ListTargets = _listTargets.ListHostileTargets;
	
	}
	
	public void DisplayTargetListFriendly(){
		BuildList(TargetListFriendly);
		ListTargets = _listTargets.ListFriendlyTargets;
	
	}
	
	public void DisplayTargetListOwned(){
		BuildList(TargetListOwned);
		ListTargets = _listTargets.ListOwnedTargets;
	
	}
	
	public void UpdateDisplayedList(){
		if(EnableTargetList){
			switch((int)ListTargets){
			case 1:
				
				break;
				
			case 2:
				DisplayTargetListAll();
				break;
				
			case 3:
				
				break;
				
			case 4:
				DisplayTargetListFriendly();
				break;
				
			case 5:
				DisplayTargetListHostile();
				break;
				
			case 6:
				DisplayTargetListOwned();
				break;
			}
		}
	}
	
	public void ClearList(){
	foreach(GameObject button in GameObject.FindGameObjectsWithTag("TargetButton")){
			if (button.name == "TargetButton"){
				button.SetActive(false);
			}
		}
	}
	
	void BuildList(List<Transform> ThisList){
		ClearList();
		if(ThisList.Count > 0){
			for(int i = 0; i < ThisList.Count; i++){
				RectTransform button = ThisList[i].GetComponent<FX_3DRadar_RID>().ThisButton;
				button.anchoredPosition = new Vector3(0,(button.sizeDelta.y + ButtonPadding)* -i, 0);
				button.gameObject.SetActive(true);
			}
	
			Vector2 size = _Content.sizeDelta;
			size.y = ThisList.Count * ThisList[0].GetComponent<FX_3DRadar_RID>().ThisButton.sizeDelta.y;
			_Content.sizeDelta = size;
		}
	}
	
	void OpenTargetList() {
		_TargetList.gameObject.SetActive(true);
		DisplayTargetListAll();
	}
	
	void CloseTargetList() {
		_TargetList.gameObject.SetActive(false);
		ListTargets = _listTargets.DisableList;
		ClearList();
	}
	
	public void EnableRadar(){
		RadarCamera.enabled = true;
		RadarEnabled = true;
	}
	
	public void DisableRadar(){
		RadarCamera.enabled = false;
		RadarEnabled = false;
		CloseTargetList();
		EnableTargetList = false;
	}
	
	void OnApplicationQuit() {
		if(PulseMat) {
			PulseMat.mainTextureOffset = Vector2.zero;
		}
	}
	}