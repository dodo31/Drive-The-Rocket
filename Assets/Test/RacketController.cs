using System;
using UnityEngine;

public class RacketController : MonoBehaviour {
	private const float ACCELERATION = 6;
	private const float MAX_SPEED = 18;

	private const float MIN_ORIENTATION = 60;
	private const float MAX_ORIENTATION = 125;

	public KeyCode triggerKey;

	private float currentSpeed;

	private float previousOrientation;

	public void Start() {
		currentSpeed = 0;
	}

	public void Update() {
		if (Input.GetKey(triggerKey)) {
			currentSpeed = Mathf.Min(currentSpeed + ACCELERATION, MAX_SPEED);
			float currentOriention = Mathf.Clamp(transform.localRotation.eulerAngles.z + currentSpeed, MIN_ORIENTATION, MAX_ORIENTATION);

			if (currentOriention == MAX_ORIENTATION) {
				currentSpeed = 0;
			}

			Vector3 currentRotation = transform.localRotation.eulerAngles;
			transform.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, currentOriention);
		} else {
			currentSpeed = Mathf.Min(currentSpeed + ACCELERATION / 2f, MAX_SPEED / 2f);
			float currentOriention = Mathf.Clamp(transform.localRotation.eulerAngles.z - currentSpeed, MIN_ORIENTATION, MAX_ORIENTATION);

			if (currentOriention == MIN_ORIENTATION) {
				currentSpeed = 0;
			}

			Vector3 currentRotation = transform.localRotation.eulerAngles;
			transform.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, currentOriention);
		}
	}

	public float Length() {
		BoxCollider collider = this.GetComponent<BoxCollider>();
		return collider.size.y * transform.localScale.y;
	}

	public float CurrentSpeed {
		get {
			return currentSpeed;
		}

		set {
			currentSpeed = value;
		}
	}
}