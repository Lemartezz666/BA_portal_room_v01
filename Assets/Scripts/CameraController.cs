using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public float velocity = 20; // km/h

	public float acceleration = 2f; // km/h²

	public float rotationSpeed = -10f;

	public float jumpForce = 500f;

	public bool fixedHeight = false;

	private Vector3 lastMouse;
	private bool mouseActive = false;

	private Transform rotationTransform;

	private Rigidbody physicsBody;

	private bool jump = false;

	private Vector3 rotation;

    void Start() {
		rotationTransform = GetComponentInChildren<Camera>().transform;
		physicsBody = GetComponent<Rigidbody>();

		rotation = rotationTransform.localEulerAngles;
    }

	void Update() {
		if (Input.GetKeyUp(KeyCode.Space)) {
			jump = true;
		}
	}

    void FixedUpdate() {
		// rotation
		if(Input.GetMouseButton(0)) {
			if(mouseActive) {
				var mouseMove = lastMouse - Input.mousePosition;
				rotation += new Vector3(-mouseMove.y, mouseMove.x) * rotationSpeed * Time.fixedDeltaTime;
				rotation.x = Mathf.Clamp(rotation.x, -80, 80);
				rotationTransform.localEulerAngles = rotation;
			}

			mouseActive = true;
			lastMouse = Input.mousePosition;

		} else {
			mouseActive = false;
		}

		// get local movement
		var localDir = new Vector3(0,0,0);
        if (Input.GetKey (KeyCode.W))
            localDir += new Vector3(0, 0 , 1);
        if (Input.GetKey (KeyCode.S))
            localDir += new Vector3(0, 0, -1);
        if (Input.GetKey (KeyCode.A))
            localDir += new Vector3(-1, 0, 0);
        if (Input.GetKey (KeyCode.D))
            localDir += new Vector3(1, 0, 0);

		if(localDir.magnitude>1f)
			localDir.Normalize();

		// calculate global movement
		var dir = rotationTransform.rotation * localDir;

		if(physicsBody) { // first-person controller
			var targetVelocity = dir * (velocity/3.6f);
			var deltaVelocity = targetVelocity - physicsBody.velocity;
			// clamp delta to acceleration
			var deltaVelocityLen = deltaVelocity.magnitude;
			if(deltaVelocityLen > acceleration) {
				deltaVelocity = deltaVelocity/deltaVelocityLen * acceleration;
			}

			deltaVelocity.y = 0; // let gravity act normally
			physicsBody.AddForce(deltaVelocity * physicsBody.mass, ForceMode.Impulse);

			// jump
			if (jump && IsGrounded()) {
				physicsBody.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
			}
			jump = false;

		} else { // freefly camera
			if(fixedHeight)
				dir.y = 0;
			
			transform.position += (dir * (velocity/3.6f) * Time.fixedDeltaTime);
		}
    }

	private bool IsGrounded() {
		return Physics.Raycast(transform.position + new Vector3(0, 0.2f, 0), Vector3.down, 0.5f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
	}
}

