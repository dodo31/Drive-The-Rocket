using UnityEngine;

public class BallController : MonoBehaviour {
	private Vector3 previousPosition;

	public void Start() {

	}

	public void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			ConstantForce constantForce = this.GetComponent<ConstantForce>();
			constantForce.force = new Vector3(-9999995, 0, 0);
		}

		if (Input.GetKeyUp(KeyCode.Space)) {
			ConstantForce constantForce = this.GetComponent<ConstantForce>();
			constantForce.force = Vector3.zero;
		}

		if (Input.GetKeyUp(KeyCode.Keypad0)) {
			transform.position = new Vector3(1.93f, -4.71f, -1.9f);
		}

		float speed = Vector3.Distance(previousPosition, transform.position);
		float maxDistance = speed + transform.localScale.x / 2f;
		Vector3 direction = (transform.position - previousPosition).normalized;
		RaycastHit hitInfo = new RaycastHit();
		bool willCollide = Physics.Raycast(transform.position, direction, out hitInfo, maxDistance);

		if (willCollide) {
			if (hitInfo.collider.name == "LeftBumperRacket") {
			}
		}

		previousPosition = transform.position;
	}
}
