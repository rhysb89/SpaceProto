using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
	
public class FX_3DRadar_RID : MonoBehaviour {
	
	FX_3DRadar_Mgr FX3DRM; // Local Cache To The FX_3DRadar_Mgr Script
	
	public enum iffStatus{Neutral, Friendly, Hostile, Unknown, Abandoned, Owned, Objective, NAV}
	public iffStatus IFFStatus;
	
	public enum ridType {RadarID, NAVPoint}
	public ridType RIDType;
	
	public enum navArrive {DoNothing, Disable, Destroy }
	public navArrive NAVArrive;
	
	public FX_Mission.objectiveType ObjectiveType;
	
	public int[] MainClass = new int[2]; //0 = Current Main Class, 1 = Check Last Main Class
	public int[] SubClass = new int[2]; //0 = Current Sub Class, 1 = Check Last Sub Class
	public int[] ThisFaction = new int[2]; //0 = Current Faction, 1 = Checl Last Faction,
	public int ThisFactionID;
	
	Transform ThisTransform; // Local Cache To This Transform
	public List<Renderer> ThisRenderer = new List<Renderer>(1);
	public Rigidbody ThisRigidbody;
	Vector3 ThisPos; // This Transforms Position
	Vector3 RelPos; // This Transforms Relative Position To The Player
	
	public RectTransform ThisButton;
	public RectTransform ThisHID;
	public RectTransform RIDI; // Radar ID Icon
	RectTransform RIDV; // Radar ID Vertical Distance Indicator
	RectTransform RIDB; // Radar ID Base Icon
	RectTransform BoundCorner; // Bounding Corner Indicator
	
	public Image Image_RIDI; // Radar ID Icon
	Image Image_RIDV; // Radar ID Vertical Distance Indicator
	Image Image_RIDB; // Radar ID Base Icon
	Image Image_BC;
	Image Image_HID;
	
	private Color32 ThisColor;
	
	//Current State
	bool Enabled = false;
	public bool IsPOI;
	public bool IsNAV;
	public bool IsActiveNAV;
	public bool IsPlayer;
	public bool IsPlayerTarget;
	public bool[] IsPlayerOwned = new bool[2];
	public bool[] IsAbandoned = new bool[2];
	public bool[] IsObjective = new bool[2];
	public bool BlindRadarOverride;
	bool VDIDown;
	
	//Detection & Targeting States
	public bool IsTargetable = true; // Determines if this object can be targeted while displayed on the players radar.
	public bool IsDetectable = true; // Determines if this object can be detected.
	public bool IsDiscovered; // Stores if this object has been detected by the players radar at any point.
	public bool DetectionReset; // Reset this objects Is Discovered if it enters an undetectable state. 
	
	//HUD Display Settings
	bool InRadarRange;
	bool HUDIDEnabled; //The Current State Of The HUD ID
	public bool DisplayHUDID = true; //This Local HUD ID Enable - Disable Override
	public bool HUDIDOnScreen;
	public bool OffScreen;
	public bool VDIBaseEnabled;
	
	//Bounds
	Bounds ThisBounds;
	public enum _boundsShow {OnlyInRadarRange, AlwaysAfterContact, Always}
	public _boundsShow BoundsShow;
	public _boundsShow HUDIDShow;
	public bool WasPlayerTarget;
	public bool DisplayBounds = true; //This Local Bounds Enable - Disable Override
	bool BoundsEnabled = true; //The Current State Of The Bounds
	bool LOS;
	
	//Local Radar
	public List<Transform> HostileList = new List<Transform>(0);
	public float UpdateTime = 1;
	
	public float RadarRange = 100;
	public bool EnableLocalRadar;
	
	//NAV
	public float ArrivalDist = 10;
	
	//Timers
	float StatusTimer;
	float RadarUpdateTimer;
	float RadarResetTimer;
	float LocalRadarTimer;
	
	
	void Start () {
		FX3DRM = GameObject.Find("_GameMgr").GetComponent<FX_3DRadar_Mgr>();
		ThisTransform = transform;
	
		if(RIDType == FX_3DRadar_RID.ridType.NAVPoint) {
			IsNAV = true;
			HUDIDOnScreen = true;
			HUDIDShow = _boundsShow.Always;
			BoundsEnabled = false;
		} else {
			IsNAV = false;
			IsActiveNAV = false;
		}
	
		if(!IsNAV) {
			if(FX3DRM.DefaultListButton && !ThisButton) {
				ThisButton = Instantiate(FX3DRM.DefaultListButton, Vector3.zero, Quaternion.identity) as RectTransform;
			}
		
			ThisButton.GetComponent<FX_MouseSelect>().ThisParent = ThisTransform;
			ThisButton.SetParent(FX3DRM._Content);
			ThisButton.eulerAngles = Vector3.zero;
			ThisButton.anchoredPosition = Vector3.zero; 
			ThisButton.localScale = Vector3.one;
		}
	
		MakeRID();
		MakeHUDID();
		MakeBounds();
		UpdateIFFTexture();
		SetIFFColor();
		DisableThis();
		RemoveFromTargetList();
		UpdateTime = 0;
	
		if(IsNAV && !IsActiveNAV) {
			SetNAVInactive();
		}
	}
	
	void LateUpdate() {
	
		//Disable Target Radar Elements If The Players Radar Is Disabled
	
		if (!FX3DRM.RadarEnabled){
			if(Enabled){
				DisableThis();
				DisableBounds();
				DisableHUDID();
			}
			return;
		}
		
		//Update Radar NAV Style
		if(IsNAV){
			if(IsActiveNAV) {
				UpdateRadar();
			}
			return;
		}
	
		//Check For Status Changes
		StatusCheck();
		
		if(!IsPlayer){
			//Update Main Radar ID Loop
			UpdateRadar();
	
			//Build The Hostile List
			if(EnableLocalRadar && Time.time >= LocalRadarTimer) {
				BuildHostileList();
			}
		}else if(Enabled){
			DisableThis();
			DisableHUDID();
			DisableBounds();
		}
	}
	
	//*************************************************************************************************************************************
	//															Main Loop
	//*************************************************************************************************************************************
	void UpdateRadar(){
		//Store this transform's current position
		ThisPos = ThisTransform.position;
		RelPos = FX3DRM.Player.InverseTransformPoint(ThisPos) * FX3DRM.RadarLocalScale;
	
		//Determine if this object is in the players radar range
		float CurDist = Vector3.SqrMagnitude(ThisPos - FX3DRM.PlayerPos);
		InRadarRange = (CurDist < FX3DRM.RadarRangeSQR);

        if(IsPlayerTarget) {
            if(FX3DRM.TextTargetDistance || FX3DRM.NameAndDistance) {
                string thisDist = "";
                float DisplayDistance = Mathf.Sqrt(CurDist);

			    if(DisplayDistance < 1000){
				    thisDist = FX3DRM.TargetDistancePrefix + DisplayDistance.ToString("0." + FX3DRM.SuffixMeters);
			    }else{
				    thisDist = FX3DRM.TargetDistancePrefix + (DisplayDistance *.001).ToString("#.0" + FX3DRM.SuffixKilometers);
			    }

                if(FX3DRM.NameAndDistance) {
                    FX3DRM.TextTargetName.text = FX3DRM.TargetNamePrefix + ThisTransform.name.ToString() + "  " + thisDist;
                } else {
                    FX3DRM.TextTargetDistance.text = thisDist;
                }
            }
        }
	
		//Render this object to the radar as a NAV or POI
		if(IsNAV && FX3DRM.RenderNAVRadar|| IsPOI){
			if(IsNAV){
				ThisHID.anchoredPosition =  GetScreenPos(FX3DRM.HUDIDPadding) + FX3DRM.NAVHUDOffset;
	
				if(IsActiveNAV && CurDist < (ArrivalDist * ArrivalDist)) {
					if(NAVArrive == navArrive.Disable) {
						SetNAVInactive();
					}
					if(NAVArrive == navArrive.Destroy) {
						DestroyThis();
					}
				}
			}
	
			if(InRadarRange){
				RenderToRadar(false);
			}else{
				if(FX3DRM.NAVPointer && FX3DRM.PointerTarget == ThisTransform) {
					if(!FX3DRM.NAVPointer) {
						RenderToRadar(true);
					} else if(Enabled){
						DisableThis();
					}
				} else {
					RenderToRadar(true);
				}
			}
	
			SetColorAlphaDistance();
	
			RenderHUD();
			return;
		}
	
		if(FX3DRM.FilterHostile && IFFStatus != iffStatus.Hostile){
			if(Enabled){
				DisableThis();
				DisableHUDID();
				DisableBounds();
			}
			return;
		}
	
			SetColorAlphaDistance();
	
		//Render this object to the radar normal
		if(InRadarRange && IsDetectable || FX3DRM.FilterHostile && IFFStatus != iffStatus.Hostile){
			if(FX3DRM.EnableBlindRadar){
				UpdateBlindRadar();
			}else{
				RenderToRadar(false);
			}
		}else if(Enabled){
			DisableThis();
		}
	
	
		//Check if this objects Is Discovered will be reset
		if(DetectionReset && IsDiscovered && !IsDetectable){
			IsDiscovered = false;
		}
	
		//Render the HUDID and Bounding Indicators to the screen
		RenderHUD();
	}
	
	void UpdateBlindRadar(){
		RaycastHit hit;
		bool isThis = false;
	
		if(FX3DRM.CurTime > RadarUpdateTimer){
			RadarUpdateTimer = FX3DRM.CurTime + FX3DRM.RadarUpdateTime;
			
			if(Physics.Linecast(FX3DRM.PlayerPos,ThisPos, out hit, FX3DRM.BRL)){
	
				//Check if the hit object is ThisTransform or a child of ThisTransform
				Transform t = hit.transform;
				if(t == ThisTransform) {
					isThis = true;
				} else {
					Transform[] child = ThisTransform.GetComponentsInChildren<Transform>();
	
					foreach (Transform c in child) {
						if(c == t) {
							isThis = true;
						}
					}
				}
			}
	
			if(!isThis) {
				if(LOS){
				DisableRID();
				DisableHUDID();
				DisableBounds();
				RemoveFromTargetList();
				LOS = false;
	
				if(IsPlayerTarget){
					RadarResetTimer = FX3DRM.CurTime + FX3DRM.RadarResetTime;
					FX3DRM.ClearTarget();
					WasPlayerTarget = true; 
				}
			}
	
			}else	if(!LOS && IsTargetable){
				if(WasPlayerTarget){
					WasPlayerTarget = false;
					IsPlayerTarget = true;
					FX3DRM.SetTarget(ThisTransform);
				}
	
				if(FX3DRM.EnablePulseRadar) {
					if(FX3DRM.RadarPulse) {
						EnableThis();
						EnableHUDID();
						EnableBounds();
					}
				}else{
					EnableThis();
					EnableHUDID();
					EnableBounds();
				} 
	
				LOS = true;
			} 
		}
	
		if(!LOS && WasPlayerTarget && FX3DRM.CurTime > RadarResetTimer){
			SetInactiveTarget();
			FX3DRM.ClearTarget();
		} 
	
		if(LOS){
			RenderToRadar(false);
		}
	}
		
	//*************************************************************************************************************************************
	//														Render RID To Players Radar
	//*************************************************************************************************************************************
	
	public void RenderToRadar(bool RenderOnEdge){
	
		if(FX3DRM.EnablePulseRadar && !FX3DRM.RadarPulse) {
			return;
		}
	
		if(!Enabled){
			EnableThis();
			IsDiscovered = true;
			if(ObjectiveType == FX_Mission.objectiveType.Destroy){
				UpdateMissionObjective();
			}
		}
	
		bool RTT = FX3DRM.RadarPos == FX_3DRadar_Mgr._radarPos.RenderToTexture;
	
		//Convert Radar Contact From World Space To Local Radar Space / Scale
		Vector3 newPosA = new Vector3(0,0,0);
		Vector3 newPosB = new Vector3(0,0,0);
	
		if(RenderOnEdge){
			newPosA = FX3DRM.RadarCamera.WorldToScreenPoint(RelPos.normalized * .5f);
		}else{
			newPosA = FX3DRM.RadarCamera.WorldToScreenPoint(RelPos);
			newPosB = FX3DRM.RadarCamera.WorldToScreenPoint(new Vector3(RelPos.x, 0 ,RelPos.z));
		}
	
		newPosA.x = Mathf.Round(newPosA.x); 
		newPosA.y = Mathf.Round(newPosA.y);
		newPosB.x = Mathf.Round(newPosB.x); 
		newPosB.y = Mathf.Round(newPosB.y);
	
		//Hide The VDI & Base Icon If RID Is Too Close
		bool HideVDI = (Mathf.Abs(newPosB.y - newPosA.y) <= 3);
	
		if(FX3DRM.Radar2D || HideVDI || RenderOnEdge){
			if(VDIBaseEnabled){
				DisableRIDVDIBase();
			}
		}else if(!VDIBaseEnabled){
			EnableRIDVDIBase();
		}
	
		Vector3 o = Vector3.zero;
	
		if(!IsNAV) {
			o = FX3DRM.FXCM.ObjectClassList[MainClass[0]].RIDOffset[SubClass[0]];
		} else {
			o = FX3DRM.NAVRIDOffset;
		}
	
		//Place This RID Icon On The Canvas
		if (newPosA.y < newPosB.y) {
			RIDI.anchoredPosition = newPosA + new Vector3(o.x, o.y, 0);
		}else{
			RIDI.anchoredPosition = newPosA + new Vector3(o.x, o.y, 0);
		}
		//Place This RID Base Icon On The Canvas
		if(!HideVDI || !RenderOnEdge){
			Vector3 NegOffset = new Vector3(0,0,0);
	
			if (newPosA.y < newPosB.y) {
				NegOffset.x = 1;
	
				if (RTT && !VDIDown) {
					Vector3 tempEuler = RIDV.eulerAngles;
					tempEuler.z -= 180;
					RIDV.eulerAngles = tempEuler;
					VDIDown = true;
				}
			} else {
				if (RTT && VDIDown) {
					Vector3 tempEuler = RIDV.eulerAngles;
					tempEuler.z = 0;
					RIDV.eulerAngles = tempEuler;
					VDIDown = false;
				}
			}
	
			if(FX3DRM.RenderVDI){
				RIDV.anchoredPosition = newPosB + NegOffset;
				float NewHeight = Mathf.Abs (newPosB.y - newPosA.y);
	
				if (newPosA.y < newPosB.y) {
					NewHeight -= o.z;
				}
	
				RIDV.sizeDelta = new Vector2(1,NewHeight);
	
				if (!RTT) {
					Vector3 dir = newPosA - newPosB;
					float angle = (Mathf.Atan2 (dir.y, dir.x)) * Mathf.Rad2Deg;
	
					RIDV.rotation = Quaternion.AngleAxis ((angle - 90), Vector3.forward);
				}
			}
		}
	}
	
	//*************************************************************************************************************************************
	//														Status & Relations Check
	//*************************************************************************************************************************************
	//Check For IFF & Class Status Changes Once A Second
	void StatusCheck(){
		if(FX3DRM.CurTime >= StatusTimer){
	
			if(IsPlayer) {
				ThisFaction[0] = FX3DRM.FXFM.PlayerFaction;
			}
	
			//Check For Main Class & Sub Class Changes
			if(MainClass[0] != MainClass[1] || SubClass[0] != SubClass[1]){
				UpdateIFFTexture();
			}
	
			//Check For Faction Change
			if(!FX3DRM.FXFM.UsePlayerRelations) {
				if (ThisFaction[1] != ThisFaction[0] || IsAbandoned[0] != IsAbandoned[1] || IsPlayerOwned[0] != IsPlayerOwned[1] || IsObjective[0] != IsObjective[1]){
					SetIFFColor();
					StatusTimer = FX3DRM.CurTime + 1;
				}
			} else {
				SetIFFColor();
				StatusTimer = FX3DRM.CurTime + FX3DRM.FXFM.UpdatePlayerRelation;
			}
		}
	}
	
	//Check This Objects Relationship With The Player
	void CheckPlayerRelation(){
		ThisFaction[1] = ThisFaction[0];
		IsObjective[1] = IsObjective[0];
	
		if(IsNAV){
			IFFStatus = iffStatus.NAV;
			return;
		}
	
		if(IsAbandoned[0]){
			IFFStatus = iffStatus.Abandoned;
			IsAbandoned[1] = true;
			RemoveFromTargetList();
			AddToTargetList();
			return;
		}else{
			IsAbandoned[1] = false;
		}
	
		if(IsPlayerOwned[0]){
			IFFStatus = iffStatus.Owned;
			ThisFactionID = FX3DRM.FXFM.PlayerFactionID;
			ThisFaction[0] = FX3DRM.FXFM.PlayerFaction;
			ThisFaction[1] = ThisFaction[0];
			IsPlayerOwned[1] = true;
			RemoveFromTargetList();
			AddToTargetList();
			return;
		}else{
			IsPlayerOwned[1] = false;
		}
	
		ThisFactionID = FX3DRM.FXFM.FactionID[ThisFaction[0]];
	
		float ThisRelation = 0f;
	
		if(FX3DRM.FXFM.UsePlayerRelations) {
			ThisRelation = FX3DRM.FXFM.PlayerRelations[ThisFaction[0]];
	
			if(ThisRelation < FX3DRM.FXFM.HFS[0]){ // Player Is Hostile
				IFFStatus = iffStatus.Hostile;
			}else if(ThisRelation > FX3DRM.FXFM.HFS[1]){ // Player Is Friendly
				IFFStatus = iffStatus.Friendly;
			}else{ // Player Is Netural
				IFFStatus = iffStatus.Neutral;
			}
		}else
	
		if(ThisFactionID != FX3DRM.FXFM.PlayerFactionID){
			ThisRelation = FX3DRM.FXFM.FactionRelations[(FX3DRM.FXFM.PlayerFactionID + ThisFactionID)];
	
			if(ThisRelation < FX3DRM.FXFM.HFS[0]){ // Player Is Hostile
				IFFStatus = iffStatus.Hostile;
			}else if(ThisRelation > FX3DRM.FXFM.HFS[1]){ // Player Is Friendly
				IFFStatus = iffStatus.Friendly;
			}else{ // Player Is Netural
				IFFStatus = iffStatus.Neutral;
			}
		}else{// Player Is Same Faction
			IFFStatus = iffStatus.Friendly;
		}
	
		RemoveFromTargetList();
		AddToTargetList();
	}
	
	//*************************************************************************************************************************************
	//														Render HUD Elements To The Screen
	//*************************************************************************************************************************************
	void RenderHUD(){
		if(IsPlayerTarget){

			if(BoundsEnabled){
				DisableBounds();
			}
	
			//Check Which Method To Call For HUD ID As Player Target
			if(FX3DRM.RenderHUDID && FX3DRM.G_DisplayHUDID && FX3DRM.DisableTSIOS && FX3DRM.TargetOffScreen){
				DrawHUDIDMethod();
			}else if(HUDIDEnabled){
				DisableHUDID();
			}
		}else{
			//Check Which Method To Call For Bounds
			if(!IsNAV && FX3DRM.RenderBounds){
				DrawBoundsMethod();
				if(BoundsEnabled){
					DrawBounds();
				}
			}
			//Check Which Method To Call For HUD ID
			if(FX3DRM.RenderHUDID && FX3DRM.G_DisplayHUDID && DisplayHUDID){
				DrawHUDIDMethod();
			}else if(HUDIDEnabled){
				DisableHUDID();
			}
		}
		if(!IsNAV && FX3DRM.RenderHUDID && FX3DRM.G_DisplayHUDID && DisplayHUDID){
			Vector2 o = FX3DRM.FXCM.ObjectClassList[MainClass[0]].HUDOffset[SubClass[0]];
			ThisHID.anchoredPosition = GetScreenPos(FX3DRM.HUDIDPadding) + new Vector3(o.x, o.y, 0);
		}
	}
	
	//*************************************************************************************************************************************
	//														HUD ID Functions
	//*************************************************************************************************************************************
	void DrawHUDIDMethod(){
	
		switch((int)HUDIDShow){
		case 0:
			DrawHUDIDInRadar();
			break;
		case 1:
			DrawHUDIDAfterContact();
			break;
		case 2:
			DrawHUDIDAlways();
			break;
		}
	}
	
	void DrawHUDIDInRadar(){
		if(!FX3DRM.G_DisplayHUDID && HUDIDEnabled || !DisplayHUDID && HUDIDEnabled || !HUDIDOnScreen && !OffScreen && HUDIDEnabled ||!Enabled && HUDIDEnabled || FX3DRM.EnableBlindRadar && !LOS || !InRadarRange){
			DisableHUDID();
		}else if(InRadarRange && FX3DRM.G_DisplayHUDID && !HUDIDEnabled && DisplayHUDID && OffScreen && Enabled || FX3DRM.G_DisplayHUDID && !HUDIDEnabled && DisplayHUDID && HUDIDOnScreen && Enabled || !HUDIDEnabled && IsPlayerTarget && FX3DRM.DisableTSIOS && FX3DRM.TargetOffScreen){
			EnableHUDID();
		}
	}
	
	void DrawHUDIDAfterContact(){
		if(!FX3DRM.G_DisplayHUDID && HUDIDEnabled || !DisplayHUDID && HUDIDEnabled || !IsDiscovered && HUDIDEnabled || !IsDetectable && HUDIDEnabled || !HUDIDOnScreen && !OffScreen && HUDIDEnabled || FX3DRM.EnableBlindRadar && !LOS){
			DisableHUDID();
		}else if(FX3DRM.G_DisplayHUDID && !HUDIDEnabled && DisplayHUDID && OffScreen && IsDiscovered && IsDetectable || FX3DRM.G_DisplayHUDID && !HUDIDEnabled && DisplayHUDID && HUDIDOnScreen && IsDiscovered && IsDetectable || !HUDIDEnabled && IsPlayerTarget && FX3DRM.DisableTSIOS && FX3DRM.TargetOffScreen){
			EnableHUDID();
		}
	}
	
	void DrawHUDIDAlways(){
		if(!FX3DRM.G_DisplayHUDID && HUDIDEnabled || !DisplayHUDID && HUDIDEnabled || !IsDetectable && HUDIDEnabled || !HUDIDOnScreen && !OffScreen && HUDIDEnabled || FX3DRM.EnableBlindRadar && !LOS){
			DisableHUDID();
		}else if(FX3DRM.G_DisplayHUDID && !HUDIDEnabled && DisplayHUDID && OffScreen && IsDetectable || FX3DRM.G_DisplayHUDID && !HUDIDEnabled && DisplayHUDID && IsDetectable && HUDIDOnScreen || !HUDIDEnabled && IsPlayerTarget && FX3DRM.DisableTSIOS && FX3DRM.TargetOffScreen){
			EnableHUDID();
		}
	}
	
	//Utility : Gets the screen position for a UI element
	Vector3 GetScreenPos(int padding){
	
		Vector3 ScreenPos = FX3DRM.PlayerCameraC.WorldToScreenPoint(ThisPos);
		OffScreen = (ScreenPos.x > FX3DRM.ScreenWidth || ScreenPos.x < 0 || ScreenPos.y > FX3DRM.ScreenHeight || ScreenPos.y < 0 || ScreenPos.z <= 0.01);
	
		if(OffScreen){
	
			if(IsPlayerTarget && FX3DRM.DisableTSIOS){
				FX3DRM.EnableTSI2();
			} 
			
			RaycastHit Hit;
			Vector3 TRelPos;
			
			TRelPos = FX3DRM.PlayerCameraT.InverseTransformPoint(ThisPos);
	
			if(ScreenPos.x ==0.5f && ScreenPos.y ==0.5f){
				TRelPos.y = 1;
			}
			
			Physics.Raycast (Vector3.zero, new Vector3(TRelPos.x, TRelPos.y, 0),out Hit, 2, FX3DRM.RadarMask);
			ScreenPos = new Vector3((((FX3DRM.VFBounds * 0.5f) + Hit.point.x) / FX3DRM.VFBounds) * FX3DRM.ScreenWidth, (0.5f + Hit.point.y) * FX3DRM.ScreenHeight, 0 );
	
		}else if(IsPlayerTarget && FX3DRM.TSIEnabled){
			FX3DRM.DisableTSI2();
		}
	
		// Screen Offset Normal
	
		float swp = FX3DRM.ScreenWidth - padding;
		float shp = FX3DRM.ScreenHeight - padding;
	
		if(ScreenPos.x < padding){
			ScreenPos.x = padding;
		}
		
		if(ScreenPos.x > swp){
			ScreenPos.x = swp;
		}
				
		if(ScreenPos.y < padding){
			ScreenPos.y = padding;
		}
		
		if(ScreenPos.y > shp){
			ScreenPos.y = shp;
		}
	
		ScreenPos.x = Mathf.Round(ScreenPos.x);
		ScreenPos.y = Mathf.Round(ScreenPos.y);
	
		return ScreenPos;
	}
	
	//*************************************************************************************************************************************
	//														Bounds Functions
	//*************************************************************************************************************************************
	void DrawBoundsMethod(){
		bool InFront = (Vector3.Dot(FX3DRM.PlayerCameraT.forward, ThisPos - FX3DRM.PlayerCamPos) > 0);
	
		switch((int)BoundsShow){
		case 0:
			DrawBoundsInRadar(InFront);
			break;
		case 1:
			DrawBoundsAfterContact(InFront);
			break;
		case 2:
			DrawBoundsAlways(InFront);
			break;
		}
	}
	
	void DrawBoundsInRadar(bool InFront){
		if(!FX3DRM.G_DisplayBounds && BoundsEnabled ||!InFront && BoundsEnabled || !DisplayBounds && BoundsEnabled || !Enabled && BoundsEnabled || OffScreen && BoundsEnabled || FX3DRM.EnableBlindRadar && !LOS || !InRadarRange && BoundsEnabled){
			DisableBounds();
		}else if(InRadarRange && FX3DRM.G_DisplayBounds && !BoundsEnabled && DisplayBounds && !OffScreen && InFront && Enabled){
			EnableBounds();
		}
	}
	
	void DrawBoundsAfterContact(bool InFront){
		if(!FX3DRM.G_DisplayBounds && BoundsEnabled || !InFront && BoundsEnabled || !DisplayBounds && BoundsEnabled || !IsDiscovered && BoundsEnabled || !IsDetectable && BoundsEnabled || OffScreen && BoundsEnabled || FX3DRM.EnableBlindRadar && !LOS){
			DisableBounds();
		}else if(FX3DRM.G_DisplayBounds && !BoundsEnabled && DisplayBounds && IsDiscovered && IsDetectable && !OffScreen && InFront){
			EnableBounds();
		}
	}
	
	void DrawBoundsAlways(bool InFront){
		if(!FX3DRM.G_DisplayBounds && BoundsEnabled || !InFront && BoundsEnabled || !DisplayBounds && BoundsEnabled || !IsDetectable && BoundsEnabled || OffScreen && BoundsEnabled || FX3DRM.EnableBlindRadar && !LOS){
			DisableBounds();
		}else if(FX3DRM.G_DisplayBounds && !BoundsEnabled && DisplayBounds && IsDetectable && !OffScreen && InFront){
			EnableBounds();
		}
	}
	
	// Draws the target bounding square to the screen
	void DrawBounds(){
		Bounds ThisBounds;
	
		if(ThisRenderer.Count == 1) {
			ThisBounds = ThisRenderer[0].bounds;
		}else{
			ThisBounds = GetBoundingArea();
		}
	
		Vector3 Center = ThisBounds.center;
	
		if(FX3DRM.AdvancedBounds){
			Vector3 Ext = ThisBounds.extents;
	
			Vector2[] ExtPoints = new Vector2[8]{
				FX3DRM.PlayerCameraC.WorldToScreenPoint(new Vector3(-Ext.x, Ext.y, Ext.z) + Center),
				FX3DRM.PlayerCameraC.WorldToScreenPoint(new Vector3(Ext.x, Ext.y, Ext.z) + Center),
				FX3DRM.PlayerCameraC.WorldToScreenPoint(new Vector3(-Ext.x, -Ext.y, Ext.z) + Center),
				FX3DRM.PlayerCameraC.WorldToScreenPoint(new Vector3(Ext.x, -Ext.y, Ext.z) + Center),
				
				FX3DRM.PlayerCameraC.WorldToScreenPoint(new Vector3(-Ext.x, Ext.y, -Ext.z) + Center),
				FX3DRM.PlayerCameraC.WorldToScreenPoint(new Vector3(Ext.x, Ext.y, -Ext.z) + Center),
				FX3DRM.PlayerCameraC.WorldToScreenPoint(new Vector3(-Ext.x, -Ext.y, -Ext.z) + Center),
				FX3DRM.PlayerCameraC.WorldToScreenPoint(new Vector3(Ext.x, -Ext.y, -Ext.z) + Center)
			};
			
			Vector2 min = ExtPoints[0];
			Vector2 max = ExtPoints[0];
	
			foreach(Vector2 v2 in ExtPoints){
				min = Vector2.Min(min, v2);
				max = Vector2.Max(max, v2);
			}
	
			Vector2 newCenter = new Vector2(min.x + ((max.x - min.x) * 0.5f), min.y + ((max.y - min.y) * 0.5f));
			
			if(FX3DRM.LBS){
				if(max.x - min.x > FX3DRM.MBS){
					min.x = newCenter.x - (FX3DRM.MBS * 0.5f);
					max.x = newCenter.x + (FX3DRM.MBS * 0.5f);
				}
				
				if(max.y - min.y > FX3DRM.MBS){
					min.y = newCenter.y - (FX3DRM.MBS * 0.5f);
					max.y = newCenter.y + (FX3DRM.MBS * 0.5f);
				}
			}
			
			min.x = Mathf.Round(min.x) - FX3DRM.BPadding;
			min.y = Mathf.Round(min.y) - FX3DRM.BPadding;
			
			max.x = Mathf.Round(max.x) + FX3DRM.BPadding;
			max.y = Mathf.Round(max.y) + FX3DRM.BPadding;
	
			Vector2 newSize = new Vector2((max.x - min.x), (max.y - min.y));
	
			newCenter.x = Mathf.Round(newCenter.x - (newSize.x * 0.5f));
			newCenter.y = Mathf.Round(newCenter.y - (newSize.y * 0.5f));
	
			BoundCorner.anchoredPosition = new Vector3(newCenter.x, newCenter.y, 0);
			BoundCorner.sizeDelta = newSize;
		}else{
			Vector3 minX = FX3DRM.PlayerCameraC.WorldToScreenPoint(Center + new Vector3 (ThisBounds.min.x,0,0));
			Vector3 maxX = FX3DRM.PlayerCameraC.WorldToScreenPoint(Center + new Vector3 (ThisBounds.max.x,0,0));
			Vector3 minY = FX3DRM.PlayerCameraC.WorldToScreenPoint(Center + new Vector3 (0,ThisBounds.min.y,0));
			Vector3 maxY = FX3DRM.PlayerCameraC.WorldToScreenPoint(Center + new Vector3 (0,ThisBounds.max.y,0));
			Center = FX3DRM.PlayerCameraC.WorldToScreenPoint(Center);
	
			if(FX3DRM.LBS){
				if(maxX.x - minX.x > FX3DRM.MBS){
					minX.x = Center.x - (FX3DRM.MBS * 0.5f);
					maxX.x = Center.x + (FX3DRM.MBS * 0.5f);
				}
				
				if(maxY.y - minY.y > FX3DRM.MBS){
					minY.y = Center.y - (FX3DRM.MBS * 0.5f);
					maxY.y = Center.y + (FX3DRM.MBS * 0.5f);
				}
			}
			
			minX.x = Mathf.Round(minX.x) - FX3DRM.BPadding;
			minY.y = Mathf.Round(minY.y) - FX3DRM.BPadding;
			
			maxX.x = Mathf.Round(maxX.x) + FX3DRM.BPadding;
			maxY.y = Mathf.Round(maxY.y) + FX3DRM.BPadding;
			
			Vector2 newSize = new Vector2((maxX.x - minX.x), (maxY.y - minY.y));
	
			Center.x = Mathf.Round(Center.x - (newSize.x * 0.5f));
			Center.y = Mathf.Round(Center.y - (newSize.y * 0.5f));
	
			BoundCorner.anchoredPosition = new Vector3(Center.x, Center.y, 0);
			BoundCorner.sizeDelta = newSize;
		}
	}
	
	// Utility : Gets the combined bounding size of multiple renderers
	Bounds GetBoundingArea () { 
		Vector3 Center = Vector3.zero;
	
		for(int i = 0; i < ThisRenderer.Count; i++) {
			Center += ThisRenderer[i].bounds.center;
		}
	
		Center = Center / ThisRenderer.Count;
		Bounds ThisBounds = new Bounds(Center, Vector3.zero);
		
		foreach(Renderer r in ThisRenderer){
			ThisBounds.Encapsulate(r.bounds);
		}
		return ThisBounds;
	}
	
	//*************************************************************************************************************************************
	//														Create This Objects RID & HUD Elements
	//*************************************************************************************************************************************
	
	void MakeRID(){
		//Make This Objects Radar ID
		RIDI = FX3DRM.MakeImage("RID_Icons", 0.0f, GameObject.Find("RadarTargets").transform);
		RIDI.pivot = Vector2.zero;
		Image_RIDI = RIDI.GetComponent<Image>();
	
		//Make This Objects VDI
		if(FX3DRM.RenderVDI){
			RIDV = FX3DRM.MakeImage("RID_VDI",0.0f, GameObject.Find("RadarTargets").transform);
			RIDV.pivot = Vector2.zero;
			
			Image_RIDV = RIDV.GetComponent<Image>();
			Image_RIDV.raycastTarget = false;
			RIDV.sizeDelta = Vector3.zero;
		}
	
		//Make This Objects VDI Base
		if(FX3DRM.RenderRIDB){
			RIDB = FX3DRM.MakeImage("RID_Base",0.0f, RIDV.transform);
			RIDB.pivot = Vector2.zero;
			RIDB.anchoredPosition = FX3DRM.RIDBOffset;
			Image_RIDB = FX3DRM.SetImage(RIDB.GetComponent<Image>(), FX3DRM.Sprite_RIDBase, false);
		}
	
		if(!IsNAV && FX3DRM.SelectableRID){
			RIDI.gameObject.AddComponent<Button>();
			RIDI.gameObject.GetComponent<Button>().onClick.AddListener(() => { ThisHID.GetComponent<FX_MouseSelect>().SetTarget();});
			RIDI.gameObject.AddComponent<FX_MouseSelect>().ThisParent = ThisTransform;
		}
	
		DisableThis();
	}
	
	//Create This Objects HUD Target Indicator
	void MakeHUDID() {
		
		if(FX3DRM.RenderHUDID){
			ThisHID = FX3DRM.MakeImage("HUD_ID",0.0f, GameObject.Find("HUDIDs").transform);
			ThisHID.pivot = Vector2.zero;
			Image_HID = ThisHID.GetComponent<Image>();
			
			if(IsNAV){
				Image_HID.sprite = FX3DRM.Sprite_NAV;
				ThisHID.pivot = new Vector2(0.5f,0.5f);
			}else if(FX3DRM.HUDIAsIcon){
				Image_HID.sprite = FX3DRM.FXCM.ObjectClassList[MainClass[0]].ClassSprite[SubClass[0]];
			}else{
				Image_HID.sprite = FX3DRM.FXCM.ObjectClassList[MainClass[0]].ClassSprite[SubClass[0]];
			}
			
			ThisHID.GetComponent<Image>().SetNativeSize();
			
			if(!IsNAV && FX3DRM.SelectableHUDID){
				ThisHID.gameObject.AddComponent<Button>();
				ThisHID.gameObject.GetComponent<Button>().onClick.AddListener(() => { ThisHID.GetComponent<FX_MouseSelect>().SetTarget();});
				ThisHID.gameObject.AddComponent<FX_MouseSelect>().ThisParent = ThisTransform;
			}
			DisableHUDID();
		}
	}
	
	//Create This Objects Bounding Indicators
	void MakeBounds() {
		
		if(!IsNAV){
			if(FX3DRM.RenderBounds){
				BoundCorner = FX3DRM.MakeImage(transform.name.ToString() + " Bounds",0.0f, GameObject.Find("BoundCorners").transform);
				BoundCorner.pivot = Vector2.zero;
				Image_BC = FX3DRM.SetImage(BoundCorner.GetComponent<Image>(), FX3DRM.Sprite_BoundSquare, false);
				Image_BC.type = Image.Type.Sliced;
			}
		}
	}
	
	//Change This Objects Displayed Radar ID Icon
	public void UpdateIFFTexture(){
		if(IsNAV){
			Image_RIDI.sprite = FX3DRM.Sprite_RadarNAV;
			Image_RIDI.SetNativeSize();
		}else{
			Image_RIDI.sprite = FX3DRM.FXCM.ObjectClassList[MainClass[0]].ClassSprite[SubClass[0]];
			Image_RIDI.SetNativeSize();
	
			if(IsPlayerTarget && FX3DRM.RenderTSIID){
				FX3DRM.Image_TSIID.sprite = FX3DRM.FXCM.ObjectClassList[MainClass[0]].ClassSprite[SubClass[0]];
				FX3DRM.Image_TSIID.SetNativeSize();
			}
	
			if(FX3DRM.RenderHUDID){
				if(FX3DRM.HUDIAsIcon){
					Image_HID.sprite = FX3DRM.FXCM.ObjectClassList[MainClass[0]].ClassSprite[SubClass[0]];
				}else{
					Image_HID.sprite = FX3DRM.Sprite_HUDID;
				}
				Image_HID.SetNativeSize();
			}
		}
	
		if(ThisButton){
			SetButtonSprite();
		}
	
		MainClass[1] = MainClass[0];
		SubClass[1] = SubClass[0];
	}
	
	void SetButtonSprite(){
		Image ButtonImage = ThisButton.FindChild("Image").GetComponent<Image>();
		ButtonImage.sprite = FX3DRM.FXCM.ObjectClassList[MainClass[0]].ClassSprite[SubClass[0]];
		ButtonImage.SetNativeSize();
		
		RectTransform ImageRect = ButtonImage.GetComponent<RectTransform>();
		float ImageHeight = ImageRect.sizeDelta.y;
		Vector3 ImagePos = ImageRect.anchoredPosition;
		ImagePos.y = Mathf.Round(-ImageHeight * 0.5f);
		ImageRect.anchoredPosition = ImagePos;
		ThisButton.FindChild("Text").GetComponent<Text>().text = this.name.ToString ();
	}
	
	//Change This Objects IFF Color
	void SetIFFColor(){
		CheckPlayerRelation();
	
		if(IsObjective[0] && FX3DRM.UseObjectiveColor){
			ThisColor = FX3DRM.IFFColor[(int)iffStatus.Objective];
		}else if(IsNAV){
			ThisColor = FX3DRM.IFFColor[(int)iffStatus.NAV];
		}else{
			ThisColor = FX3DRM.IFFColor[(int)IFFStatus];
		}
	
		SetButtonColor();
	
		ThisColor.a = FX3DRM.RadarRIDAlpha;
		Image_RIDI.color = ThisColor;
	
		if(FX3DRM.RenderRIDB){
			Image_RIDB.color = ThisColor;
		}
		if(FX3DRM.RenderVDI){
			ThisColor.a = FX3DRM.RadarVDIAlpha;
			Image_RIDV.color = ThisColor;
		}
		
		if(FX3DRM.RenderHUDID){
			ThisColor.a = FX3DRM.HUDAlpha;
			Image_HID.color = ThisColor;
		}
		
		if(FX3DRM.RenderBounds && !IsNAV){
			ThisColor.a = FX3DRM.HUDAlpha;
			Image_BC.color = ThisColor;
		}
	
		if(IsPlayerTarget){
			SetHUDColor(ThisColor);
			SetAsActiveTarget();
		}
		SetColorAlphaDistance();
	}
	
	void SetHUDColor(Color32 ThisColor){
		if(FX3DRM.RenderTLI){
			FX3DRM.Image_TLI.color = ThisColor;
		}
		FX3DRM.Image_TSI.color = ThisColor;
		FX3DRM.Image_TSISC.color = ThisColor;
		FX3DRM.Image_RTSI.color = ThisColor;
	
		if(FX3DRM.RenderHUDID){
			FX3DRM.Image_TSI2.color = ThisColor;
		}
		if(FX3DRM.RenderHUDDIA){
			FX3DRM.Image_DIA.color = ThisColor;
		}
		if(FX3DRM.RenderTSIID){
			FX3DRM.Image_TSIID.color = ThisColor;
		}
	}
	
	void SetColorAlphaDistance(){
		if(Enabled && IsDetectable){
			if(IsPOI && FX3DRM.POINoFade || IsNAV && FX3DRM.POINoFade) {
				return;
			}
			if(FX3DRM.FadeBounds && BoundsEnabled || FX3DRM.FadeHUDID && HUDIDEnabled || FX3DRM.FadeTSI){
	
				float Distance = RelPos.sqrMagnitude * 2;
				byte Alpha;
				
				if(Distance > 0.5f){
					Alpha = FX3DRM.MinFadeAmount;
				}else{
					Alpha = (byte)(-((Distance * 2)) * 255  % 256);
				}
	
				if(FX3DRM.FadeInvert){
					Alpha = (byte)(255 - Alpha);
				}
	
				if(Alpha < FX3DRM.MinFadeAmount){
					Alpha = FX3DRM.MinFadeAmount;
				}
	
				Color32 newColor = ThisColor;
				newColor.a = Alpha;
	
				if(!IsNAV && FX3DRM.RenderBounds && FX3DRM.FadeBounds){
					Image_BC.color = newColor;
				}
				
				if(FX3DRM.RenderHUDID && FX3DRM.FadeHUDID){
					Image_HID.color = newColor;
				}
				
				if(IsPlayerTarget){
					if(FX3DRM.FadeTSI){
						FX3DRM.Image_TSI.color = newColor;
						FX3DRM.Image_TSISC.color = newColor;
	
						if(FX3DRM.RenderTSIID){
							FX3DRM.Image_TSIID.color = newColor;
						}
						if(FX3DRM.DisableTSIOS && FX3DRM.RenderHUDID && FX3DRM.FadeHUDID){
							FX3DRM.Image_TSI2.color = newColor;
						}
					}
	
					if(FX3DRM.RenderTLI && FX3DRM.FadeTLI){
						FX3DRM.Image_TLI.color = newColor;
					}
				}
	
				if(FX3DRM.FadeRID){
					Image_RIDI.color = newColor;
					Image_RIDB.color = newColor;
					Image_RIDV.color = newColor;
	
				}
			}
		}
	}
	
	void SetButtonColor(){
		Color32 ColorNormal = ThisColor;
		ColorNormal.a = (byte)50;
		Color32 ColorHover = ThisColor;
		ColorHover.a = (byte)200;
		Color32 ColorActive = ThisColor;
		ColorActive.a = (byte)255;
	
		if(ThisButton){
			ColorBlock ButtonColor = ThisButton.GetComponent<Button>().colors;
			ButtonColor.normalColor = ColorNormal;
			ButtonColor.highlightedColor = ColorHover;
			ButtonColor.pressedColor = ColorActive;
			ThisButton.GetComponent<Button>().colors = ButtonColor;
		}
	}
	
	//*************************************************************************************************************************************
	//														Called Functions
	//*************************************************************************************************************************************
	
	//Enable All Radara Elements For This Object
	void EnableThis(){
		EnableRID();
		Enabled = true;
		RemoveFromTargetList();
		AddToTargetList();
	
		if(IFFStatus == iffStatus.Hostile) {
			FX3DRM.PlayHostileSound();
		}else if(!IsNAV){
				FX3DRM.PlayNewContactSound();
		}
	}
	
	//Disable All Radar Elements For This Object
	void DisableThis(){
		ResetTSIElements();
	
		if(IsPlayerTarget){
			FX3DRM.ClearTarget();
			FX3DRM.PlayClearSound();
		}
	
		
		DisableRID();
		RemoveFromTargetList();
	
		IsPlayerTarget = false;
		Enabled = false;
	}
	
	void ResetTSIElements(){
		if(IsPlayerTarget){
			FX3DRM.RTSI.SetParent(RIDI.parent);
	
			if(FX3DRM.RenderHUDID) {
				FX3DRM.TSI2.SetParent(ThisHID.parent);
			}
		}
	}
	
	//Destroy This Object And All Radar Elements
	public void DestroyThis(){
		if(ObjectiveType == FX_Mission.objectiveType.Destroy){
			UpdateMissionObjective();
		}
	
		if(IsPlayerTarget && FX3DRM.AutoTargetDestroy) {
			FX3DRM.ClosestHostile();
		}
	
		ResetTSIElements();
		DestroyRID();
		DestroyHUDID();
		DestroyBounds();
		RemoveFromTargetList();
	
		if(ThisButton){
			Destroy (ThisButton.gameObject);
		}
	
		Destroy(ThisTransform.gameObject);
	}
	
	// Destroy The Radar ID,VDI, VDI Base
	void DestroyRID(){
		Destroy(RIDI.gameObject);
		if(FX3DRM.RenderVDI){
			Destroy(RIDV.gameObject);
		}
	}
	
	// Enable / Disable RID
	void EnableRID(){
		Image_RIDI.enabled = true;
		EnableRIDVDIBase();
	}
	
	
	void DisableRID(){
		ResetTSIElements();
		Image_RIDI.enabled = false;
		DisableRIDVDIBase();
	}
	
	
	// Enable / Disable VDI & VDI Base Icon
	void EnableRIDVDIBase(){
		if(FX3DRM.RenderRIDB){
			Image_RIDB.enabled = true;
		}
		if(FX3DRM.RenderVDI){
			Image_RIDV.enabled = true;
		}
		VDIBaseEnabled = true;
	}
	
	void DisableRIDVDIBase(){
		if(FX3DRM.RenderRIDB){
			Image_RIDB.enabled = false;
		}
		if(FX3DRM.RenderVDI){
			Image_RIDV.enabled = false;
		}
		VDIBaseEnabled = false;
	}
	
	
	// Enable / Disable Bounding Indicator
	void EnableBounds(){
		SetColorAlphaDistance();
	
		if(FX3DRM.RenderBounds && !IsNAV){
			Image_BC.enabled = true;
			BoundsEnabled = true;
		}
	}
	
	void DisableBounds(){
		if(FX3DRM.RenderBounds && !IsNAV){
			Image_BC.enabled = false;
			BoundsEnabled = false;
		}
	}
	
	void DestroyBounds(){
		if(FX3DRM.RenderBounds && !IsNAV){
			Destroy(BoundCorner.gameObject);
		}
	}
	
	
	//Enable / Disable HUD ID
	void EnableHUDID(){
		if(FX3DRM.RenderHUDID){
			Image_HID.enabled = true;
			HUDIDEnabled = true;
		}
	}
	
	void DisableHUDID(){
		if(FX3DRM.RenderHUDID){
			Image_HID.enabled = false;
			HUDIDEnabled = false;
		}
	
		if(IsPlayerTarget) {
			FX3DRM.DisableTSI2();
		}
	}
	
	void DestroyHUDID(){
		if(FX3DRM.RenderHUDID){
			Destroy(ThisHID.gameObject);
		}
	}
	
	// Set NAV Active / Inactive
	
	public void SetNAVActive(){
		IsActiveNAV = true;
		if(ThisHID) {
			EnableHUDID();
			EnableRID();
			HUDIDOnScreen = true;
		}
	}
	
	public void SetNAVInactive(){
		IsActiveNAV = false;
			if(ThisHID) {
			DisableHUDID();
			DisableRID();
			HUDIDOnScreen = false;
		}
	}
	
	void UpdateMissionObjective(){
		if(IsObjective[0]){
			for(int i = 0; i < FX3DRM.FXMM.MissionList.Count; i++){
				FX3DRM.FXMM.MissionList[i].Mission.ObjectiveDestroyed(ThisTransform);
			}
		}
	}
	
	public void SetAsActiveTarget(){
		IsPlayerTarget = true;
	
		Color32 ColorNormal = ThisColor;
		ColorNormal.a = (byte)255;
	
		if (ThisButton) {
			Button b = ThisButton.GetComponent<Button> ();
			ColorBlock ButtonColor = b.colors;
			ButtonColor.normalColor = ColorNormal;
			b.colors = ButtonColor;
			SetHUDColor (ThisColor);
		}
	}
	
	public void SetInactiveTarget(){
		ResetTSIElements();
		IsPlayerTarget = false;
		WasPlayerTarget = false;
		Color32 ColorNormal = ThisColor;
		ColorNormal.a = (byte)100;
	
		if(ThisButton){
			Button b = ThisButton.GetComponent<Button>();
			ColorBlock ButtonColor = b.colors;
			ButtonColor.normalColor = ColorNormal;
			b.colors = ButtonColor;
		}
	}
	
	public void ResetObjectiveStatus(){
		IsObjective[0] = false;
		ObjectiveType = FX_Mission.objectiveType.None;
	}
	
	
	
	//Add This Object To The Players Target List
	void AddToTargetList(){
		if(!IsNAV){
			if(IsTargetable){
				FX3DRM.TargetListAll.Add(ThisTransform);
			}
			if(IFFStatus == iffStatus.Neutral){
				FX3DRM.TargetListNeutral.Add(ThisTransform);
			}
			if(IFFStatus == iffStatus.Friendly){
				FX3DRM.TargetListFriendly.Add(ThisTransform);
			}
			if(IFFStatus == iffStatus.Hostile){
				FX3DRM.TargetListHostile.Add(ThisTransform);
			}
			if(IFFStatus == iffStatus.Unknown){
				FX3DRM.TargetListAband.Add(ThisTransform);
			}
			if(IFFStatus == iffStatus.Owned){
				FX3DRM.TargetListOwned.Add(ThisTransform);
			}
			if(IFFStatus == iffStatus.Objective){
				FX3DRM.TargetListObj.Add(ThisTransform);
			}
		}
		FX3DRM.UpdateDisplayedList();
	}
	
	//Remove This Object From The Players Target List
	void RemoveFromTargetList(){
		if(!IsNAV){
			for(int a = 0; a < FX3DRM.TargetListAll.Count; a++){
				if(ThisTransform == FX3DRM.TargetListAll[a]){
					FX3DRM.TargetListAll.RemoveAt(a);
				}
			}
	
			for(int n = 0; n < FX3DRM.TargetListNeutral.Count; n++){
				if(ThisTransform == FX3DRM.TargetListNeutral[n]){
					FX3DRM.TargetListNeutral.RemoveAt(n);
				}
			}
	
			for(int f = 0; f < FX3DRM.TargetListFriendly.Count; f++){
				if(ThisTransform == FX3DRM.TargetListFriendly[f]){
					FX3DRM.TargetListFriendly.RemoveAt(f);
				}
			}
	
			for(int h = 0; h < FX3DRM.TargetListHostile.Count; h++){
				if(ThisTransform == FX3DRM.TargetListHostile[h]){
					FX3DRM.TargetListHostile.RemoveAt(h);
				}
			}
	
			for(int ab = 0; ab < FX3DRM.TargetListAband.Count; ab++){
				if(ThisTransform == FX3DRM.TargetListAband[ab]){
					FX3DRM.TargetListAband.RemoveAt(ab);
				}
			}
	
			for(int o = 0; o < FX3DRM.TargetListOwned.Count; o++){
				if(ThisTransform == FX3DRM.TargetListOwned[o]){
					FX3DRM.TargetListOwned.RemoveAt(o);
				}
			}
	
		}
		FX3DRM.UpdateDisplayedList();
	}
	
	
	void BuildHostileList() {
		
		HostileList.Clear();
	
		//Find all objects in range of the radar
		Collider[] TempTargetList = Physics.OverlapSphere(ThisPos, RadarRange / FX3DRM.WorldScale);
	
			for(int i = 0; i < TempTargetList.Length; i++){
				if(TempTargetList[i].GetComponent<FX_3DRadar_RID>()) {
	
				//Check if the object has an FX_3DRadar_RID Script
				FX_3DRadar_RID FXRID = TempTargetList[i].GetComponent<FX_3DRadar_RID>();
	
				if(FXRID) {
					//Get the FactionID of the current object
					int GetFactionID = FXRID.ThisFactionID;
	
					//Check if the object is the Player
					if(FXRID.IsPlayer && FX3DRM.FXFM.UsePlayerRelations) {
						//Check if the players unique relations is Hostile. If so then add the player to the Hostile List
						if(FX3DRM.FXFM.PlayerRelations[ThisFaction[0]] <= FX3DRM.FXFM.HFS[0]) {
							HostileList.Add(TempTargetList[i].transform);
						}
					}else
					//Check if the FactionID is different from ThisFactionID
					if(GetFactionID > 0 && GetFactionID != ThisFactionID){
						//Get the relation value
						float ThisRelation = FX3DRM.FXFM.FactionRelations[(GetFactionID + ThisFactionID)];
						
						//Check if the object is Hostile. If so then add it to the Hostile List
						if(ThisRelation <= FX3DRM.FXFM.HFS[0]){
							HostileList.Add(TempTargetList[i].transform);
						}
					}
				}
			}
		}
	
	LocalRadarTimer += Time.time + UpdateTime;
	}
}
