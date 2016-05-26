/* This script attempts to gracefully handle collisions between the camera and its target. It does this by casting several rays 
and adjusting the camera's behavior when those arrays hit an object.

Click and hold the left mouse button to move around the target object. Use the mousescroll wheel to zoom in and out.
*/

// The target we are following
var target : Transform;
// The distance to the target
var distance : float = 8;
//The desired distance to the target
var wantedDistance : float;
// the initial height we want the camera to be above the target
var height = 2.0;
// How much we want to dampen the height and rotation
var heightDamping = 2.0;
var rotationDamping = 3.0;

//Scroll speed and distance limits to set up distance control
var scrollSpeed = 3;
var scrollMinLimit = 1;
var scrollMaxLimit = 10;

//Mouse speed controls to affect rotation around the character
var xSpeed : float = 2;
var ySpeed : float = 2;

//Set boolean to track whether there is anything behind the camera. If true, the camera will try to move backwards toward its wantedDistance.
private var clearBehind : boolean = true;
//Value that determines how much space should be left behind the camera.
var rearClearance : float = 3;
//Set boolean to track whether anything is below the camera. 
private var clearDown : boolean = true;
//Value representing desired clearance from the ground.
var downClearance : float = 0.3; 
//Variable to tell whether the desired range around the camera is clear
private var allClear : boolean = true; 
//Variable to store the desired clear distance
var clearRange : float = 0.4;

//These variables handle rotation around the target.
private var x = 0.0;
private var y = 0.0;

function Start() {
	wantedDistance = distance;
}

function LateUpdate () {
	// Early out if we don't have a target
	if (!target)
		return;

	//------
	//Begin collision and distance control

	//Set the desired distance (determined by the scroll wheel). This will be used to Lerp to the correct distance when possible.
	wantedDistance -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
	wantedDistance = Mathf.Clamp(wantedDistance, scrollMinLimit, scrollMaxLimit);
	
	//Set variables to store raycast info for the rays looking back from the target, at the ground, and behind the camera
	var hit : RaycastHit;
	var groundHit : RaycastHit;
	var leftHit : RaycastHit;
	var rightHit : RaycastHit;

	//First check below the camera. If it is within downClearance from the ground, adjust its distance to be proportional to the distance off the ground.
	if (Physics.Raycast(transform.position, -Vector3.up, groundHit, downClearance + 0.3)) {
		if (groundHit.distance < downClearance) {
			distance = Mathf.Lerp(distance, distance * (groundHit.distance / downClearance), Time.deltaTime * scrollSpeed);
		}
		clearDown = false;
	} else {
		clearDown = true;
	}
	
	//Now do the same for left and right of the camera, adjusting toward the target.
	if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.right), leftHit, clearRange)) {
		if (leftHit.distance < 0.2) {
			distance = Mathf.Lerp(distance, scrollMinLimit, Time.deltaTime * scrollSpeed);
		}
		allClear = false;
	} else {
		allClear = true;
	}
	
	if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), rightHit, clearRange)) {
		if (rightHit.distance < 0.2) {
			distance = Mathf.Lerp(distance, scrollMinLimit, Time.deltaTime * scrollSpeed);
		}
		allClear = false;
	} else {
		allClear = true;
	}
	
	//Now cast four "helper" rays from the camera to track whether it is clear of surrouding objects.
	var diagonalRight = Physics.Raycast(transform.position, transform.TransformDirection(Vector3(1, 0, -1)), clearRange);
	var diagonalLeft = Physics.Raycast(transform.position, transform.TransformDirection(Vector3(-1, 0, -1)), clearRange);
	var downRight = Physics.Raycast(transform.position, transform.TransformDirection(Vector3(1, -1, -1)), clearRange);
	var downLeft = Physics.Raycast(transform.position, transform.TransformDirection(Vector3(-1, -1, -1)), clearRange);
	var down = Physics.Raycast(transform.position, transform.TransformDirection(Vector3(0, -1, 0)), clearRange);
	
	//If any of these rays hit something, set allClear to false
	if (diagonalRight || diagonalLeft || downRight || downLeft || down) {
		allClear = false;
	} else {
		allClear = true;
	}
	
	//Check between the player and the camera, and a specified distance behind the camera. If something is in front of the camera, jump to where the player is visible. If something is behind the camera, get as close to it as possible.
	if (Physics.Raycast(target.position, transform.TransformDirection(-Vector3.forward), hit, distance + rearClearance) && allClear) {
		Debug.DrawRay(target.position, transform.TransformDirection(-Vector3.forward) * (distance + rearClearance), Color.yellow);
		if (hit.distance < distance) {
			distance = hit.distance - 0.1;
		} else if (distance < wantedDistance && clearDown) {
			distance = Mathf.Lerp(distance, hit.distance - 0.1, Time.deltaTime * scrollSpeed);
		}
		clearBehind = false;
	} else {
		clearBehind = true;
	}
	
	//If clearBehind and allClear are true or the current distance is less than the wanted distance, lerp the distance toward wantedDistance.
	if ((clearBehind && allClear) || wantedDistance < distance) {
		distance = Mathf.Lerp(distance, wantedDistance, Time.deltaTime * scrollSpeed);
	}
	
	//End collision and distance control
	//------
	
	//------
	//Begin rotation control
	
	//Set x and y variables to be equal to the current rotation of the camera
	x = transform.eulerAngles.y;
	y = transform.eulerAngles.x;
	
		
	if (Input.GetButton("Fire1") /*|| Input.GetButton("Fire2")*/) {
 		x += Input.GetAxis("Mouse X") * xSpeed;
		y -= Input.GetAxis("Mouse Y") * ySpeed;
 	}
		
	var rotation = Quaternion.EulerAngles(y * Mathf.Deg2Rad, x * Mathf.Deg2Rad, 0);
	transform.position = rotation * Vector3(0.0, 0.0, -distance) + target.position;
	
	//If something is below the camera, clamp its distance to that value. This prevents the camera from passing through the ground.
	if (!clearDown)
		transform.position.y = Mathf.Clamp(transform.position.y, groundHit.point.y + 0.1, Mathf.Infinity);
	
	// Always look at the target
	transform.LookAt (target);
}