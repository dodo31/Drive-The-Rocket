using System;
using UnityEngine;

public class CollisionsController : MonoBehaviour {
	public GameObject LeftRacket;
	public GameObject Ball;
	public GameObject MarkPoint;

	public int forceCount;
	public int forceCountLock;

	public void Start() {
		forceCount = 0;
	}

	public void LateUpdate() {
		RacketController leftRacketController = LeftRacket.GetComponent<RacketController>();
		ConstantForce ballForce = Ball.GetComponent<ConstantForce>();

		if (forceCount <= 0) {
			ballForce.force = new Vector3(0, -30, 0);
		} else {
			forceCount--;
		}

		if (forceCountLock > 0) {
			forceCountLock--;
		} else {
			LeftRacket.GetComponent<BoxCollider>().enabled = true;
		}

		Vector3 ballPositionFromRacket = MarkPoint.transform.worldToLocalMatrix.MultiplyPoint(Ball.transform.position);

		float minAngle = LeftRacket.transform.localRotation.eulerAngles.z;
		float maxAngle = minAngle + leftRacketController.CurrentSpeed;

		float ballAngle = Mathf.Atan2(ballPositionFromRacket.y, ballPositionFromRacket.x) * Mathf.Rad2Deg - 30;
		float ballDistance = Vector2.Distance(new Vector2(ballPositionFromRacket.x, ballPositionFromRacket.y), Vector2.zero);

		//Debug.Log(ballPositionFromRacket + " ==> " + ballAngle);

		if (ballAngle > minAngle && ballAngle < maxAngle && Math.Abs(ballDistance) < leftRacketController.Length() && forceCountLock == 0) {
			float force = Math.Abs(ballDistance) / (leftRacketController.Length() * 1f) * /*leftFacketController.CurrentSpeed*/1;
			float forceAngle = ballAngle;

			float forceX = (float) (force * Math.Cos(forceAngle * Mathf.Deg2Rad)) * 100;
			float forceZ = (float) (force * Math.Sin(forceAngle * Mathf.Deg2Rad)) * 100;

			LeftRacket.GetComponent<BoxCollider>().enabled = false;
			ballForce.force = new Vector3(forceX, -30, forceZ);

			forceCount = 1;
			forceCountLock = 10;
		}
	}
}