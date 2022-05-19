using UnityEngine;

public class RocketBehaviour : MonoBehaviour {
	private enum FlyingStates { MOTIONLESS, THRUST, BALLISTRIC, RETROTHRUST };
	private FlyingStates flyingState;

	private enum DistanceStates { APPROACHING, LEAVING };
	private DistanceStates distanceState;

	public GameObject DotCurve;
	public GameObject Payload;
	public GameObject FirstStageEngines;
	public GameObject SecondStageEngine;
	public ScoreController ScoreController;

	private DotCurveBehaviour dotCurveController;

	private Rigidbody2D rocketBody;
	private ConstantForce2D rocketForce;

	private GameObject currentDot;
	private GameObject nextDot;
	private int currentDotId;

	private Vector3 previousPosition;

	private Vector3 currentDotPreviousRelativePosition;
	private Vector3 currentDotCurrentRelativePosition;

	private float currentDotPreviousDistance;
	private float currentDotCurrentDistance;

	private float nextDotPreviousDistance;
	private float nextDotCurrentDistance;

	public void Start() {
		flyingState = FlyingStates.MOTIONLESS;
		distanceState = DistanceStates.APPROACHING;

		currentDotId = -1;
		currentDotPreviousDistance = float.PositiveInfinity;

		dotCurveController = DotCurve.GetComponent<DotCurveBehaviour>();

		rocketBody = this.GetComponent<Rigidbody2D>();
		rocketForce = this.GetComponent<ConstantForce2D>();

		currentDotPreviousRelativePosition = Vector3.zero;
		currentDotCurrentRelativePosition = Vector3.zero;

		currentDot = null;
		nextDot = null;
	}

	public void Update() {
		switch (flyingState) {
		case FlyingStates.MOTIONLESS:

			break;
		case FlyingStates.THRUST:
			if (this.DotCurveReady() && !this.DotCurveEnded()) {
				rocketForce.force = Vector2.zero;
				this.ApplyThrustingTasks();

				if (this.DotCurveEnded()) {
					this.StopFirstStageEngines();
					flyingState = FlyingStates.BALLISTRIC;
				}
			} else {
				if (DotCurve.transform.childCount >= 2) {
					currentDotId = 0;
					currentDot = DotCurve.transform.GetChild(currentDotId).gameObject;
					nextDot = DotCurve.transform.GetChild(currentDotId + 1).gameObject;

					currentDotCurrentDistance = Vector2.Distance(transform.position, currentDot.transform.position);
					nextDotCurrentDistance = Vector2.Distance(transform.position, nextDot.transform.position);
				}
			}
			break;
		case FlyingStates.BALLISTRIC:
			this.ApplyBallisticTasks();

			if (transform.position.y < previousPosition.y) {
				GameObject firstStageCentralEngine = FirstStageEngines.transform.GetChild(0).gameObject;
				Animator centralEngineAnimator = firstStageCentralEngine.GetComponent<Animator>();
				centralEngineAnimator.SetTrigger("TurnOn");

				float horizontalSpeed = transform.position.x - previousPosition.x;
				dotCurveController.GenerateBallisticCurve(transform.position, horizontalSpeed);
				flyingState = FlyingStates.RETROTHRUST;
			}
			break;
		case FlyingStates.RETROTHRUST:
			if (!this.DotCurveEnded()) {
				this.ApplyRetrothrustTasks();
			}
			break;
		}

		previousPosition = transform.position;
	}

	private void ApplyThrustingTasks() {
		this.ManageKeyboardInputs();
		this.UpdateSituation();
		this.UpdateDotsData();
		this.UpdateCurrentDot();
		this.SetDotsSize();
		this.UpdateDisplayedCombo();
	}

	private void ApplyBallisticTasks() {
		this.ManageKeyboardInputs();
		this.UpdateSituation();
		this.UpdateDisplayedCombo();
	}

	private void ApplyRetrothrustTasks() {
		this.ManageKeyboardInputs();
		this.UpdateSituation();
		this.UpdateDotsData();
		this.UpdateCurrentDot();
		this.SetDotsSize();
		this.UpdateDisplayedCombo();
	}

	private void ManageKeyboardInputs() {
		switch (flyingState) {
		case FlyingStates.THRUST:
			if (Input.GetKey(KeyCode.LeftArrow)) {
				rocketForce.relativeForce = new Vector2(-35 - (90 - this.CurrentSegmentOrientation()) / 90f * 10, rocketForce.relativeForce.y);
			} else if (Input.GetKey(KeyCode.Q)) {
				rocketForce.torque = 35;
			} else if (Input.GetKey(KeyCode.RightArrow)) {
				rocketForce.relativeForce = new Vector2(35 + (90 - this.CurrentSegmentOrientation()) / 90f * 10, rocketForce.relativeForce.y);
			} else if (Input.GetKey(KeyCode.D)) {
				rocketForce.torque = -35;
			} else {
				rocketForce.torque = 0;
				rocketForce.relativeForce = new Vector2(0, rocketForce.relativeForce.y);
			}

			break;
		case FlyingStates.BALLISTRIC:
			if (Input.GetKey(KeyCode.Q)) {
				rocketForce.torque = 35;
			} else if (Input.GetKey(KeyCode.D)) {
				rocketForce.torque = -35;
			} else {
				rocketForce.torque = 0;
				rocketForce.relativeForce = new Vector2(0, rocketForce.relativeForce.y);
			}

			if (Input.GetKeyDown(KeyCode.Space)) {
				this.DeliverPayload();

				float angularDistance = Mathf.Abs(((transform.rotation.eulerAngles.z + 90) % 360) - (90 - 70));

				ScoreController.IncrementScoreFromDelivering(angularDistance);
			}

			break;
		case FlyingStates.RETROTHRUST:
			if (Input.GetKey(KeyCode.LeftArrow)) {
				rocketForce.relativeForce = new Vector2(-35 - (90 - this.CurrentSegmentOrientation()) / 90f * 10, rocketForce.relativeForce.y);
			} else if (Input.GetKey(KeyCode.Q)) {
				rocketForce.torque = 35;
			} else if (Input.GetKey(KeyCode.RightArrow)) {
				rocketForce.relativeForce = new Vector2(35 + (90 - this.CurrentSegmentOrientation()) / 90f * 10, rocketForce.relativeForce.y);
			} else if (Input.GetKey(KeyCode.D)) {
				rocketForce.torque = -35;
			} else {
				rocketForce.torque = 0;
				rocketForce.relativeForce = new Vector2(0, rocketForce.relativeForce.y);
			}

			break;
		}

		//if (Input.GetKeyDown(KeyCode.LeftArrow)) {
		//	RotateEnginesRig
		//} else if (Input.GetKey(KeyCode.Q)) {
		//	rocketForce.torque = 35;
		//} else if (Input.GetKey(KeyCode.RightArrow)) {
		//	rocketForce.relativeForce = new Vector2(35 + (90 - this.CurrentSegmentOrientation()) / 90f * 10, rocketForce.relativeForce.y);
		//} else if (Input.GetKey(KeyCode.D)) {
		//	rocketForce.torque = -35;
		//} else {
		//	rocketForce.torque = 0;
		//	rocketForce.relativeForce = new Vector2(0, rocketForce.relativeForce.y);
		//}
	}

	private void UpdateSituation() {
		switch (flyingState) {
		case FlyingStates.THRUST:
			rocketForce.relativeForce = new Vector2(rocketForce.relativeForce.x, 12 + Mathf.Min((90 - this.CurrentSegmentOrientation()) / 10f, 1) * 0.85f);
			break;
		case FlyingStates.BALLISTRIC:
			rocketForce.relativeForce = new Vector2(rocketForce.relativeForce.x, 0);
			break;
		case FlyingStates.RETROTHRUST:
			rocketForce.force = new Vector2(0, 7.8f);
			rocketForce.relativeForce = new Vector2(rocketForce.relativeForce.x, 3f);
			break;
		}
	}

	private void UpdateDotsData() {
		currentDotPreviousRelativePosition = currentDotCurrentRelativePosition;
		currentDotCurrentRelativePosition = transform.position - currentDot.transform.position;

		currentDotPreviousDistance = currentDotCurrentDistance;
		currentDotCurrentDistance = Vector2.Distance(transform.position, currentDot.transform.position);

		nextDotPreviousDistance = nextDotCurrentDistance;
		nextDotCurrentDistance = Vector2.Distance(transform.position, nextDot.transform.position);
	}

	private void UpdateCurrentDot() {
		if (currentDotPreviousDistance < currentDotCurrentDistance && !this.DotCurveEnded()) {
			float actualDistance = this.FindClosestDistance();
			ScoreController.LastDistanceFromDot = actualDistance;

			int matchingGainId = ScoreController.GetGainId(actualDistance);
			float dotHue = ScoreController.GetGain(matchingGainId).Hue;

			ScoreController.UpdateGainValue(actualDistance);
			ScoreController.UpdateScore();

			SpriteRenderer dotSprite = currentDot.GetComponent<SpriteRenderer>();
			dotSprite.color = Color.HSVToRGB(dotHue / 255f, 0.8f, 1f);

			currentDotId++;
			currentDot = DotCurve.transform.GetChild(currentDotId).gameObject;
			if (currentDotId < DotCurve.transform.childCount - 2) {
				nextDot = DotCurve.transform.GetChild(currentDotId + 1).gameObject;
			}

			currentDotPreviousDistance = nextDotPreviousDistance;
			currentDotCurrentDistance = nextDotCurrentDistance;

			nextDotPreviousDistance = float.PositiveInfinity;
			nextDotCurrentDistance = Vector2.Distance(transform.position, nextDot.transform.position);
		}
	}

	private void UpdateDisplayedCombo() {
		float compatibilityShift = 0;
		float referenceOrientation = 0;

		switch (flyingState) {
		case FlyingStates.THRUST:
			compatibilityShift = 90;
			referenceOrientation = this.CurrentSegmentOrientation();
			break;
		case FlyingStates.BALLISTRIC:
			referenceOrientation = 290;
			break;
		case FlyingStates.RETROTHRUST:
			compatibilityShift = -90;
			referenceOrientation = this.CurrentSegmentOrientation();
			break;
		}


		float rocketOrientation = ((transform.rotation.eulerAngles.z + compatibilityShift) % 360);
		float angularDistance = Mathf.Abs(rocketOrientation - referenceOrientation);
		ScoreController.UpdateComboValue(angularDistance);
	}

	private float FindClosestDistance() {
		Vector3 delta = transform.position - previousPosition;

		float minDistance = float.PositiveInfinity;

		const int CUTS = 1000;
		for (int i = 0; i < CUTS; i++) {
			Vector3 fictivePosition = previousPosition + delta * (i / (CUTS * 1f));
			minDistance = Mathf.Min(Vector3.Distance(fictivePosition, currentDot.transform.position), minDistance);
		}

		return minDistance;
	}

	private float ClosestDotDistance() {
		float minDistance = float.PositiveInfinity;
		GameObject closestDot = null;

		foreach (Transform dotTransform in DotCurve.transform) {
			float distance = Mathf.Abs(Vector3.Distance(dotTransform.position, transform.position));

			if (distance < minDistance) {
				closestDot = dotTransform.gameObject;
				minDistance = distance;
			}
		}

		return minDistance;
	}

	public void LaunchRocket() {
		this.StartCoroutine(dotCurveController.GenerateThrustingCurveSin(transform.position));

		rocketBody.bodyType = RigidbodyType2D.Dynamic;
		rocketForce.force = -Physics2D.gravity;

		flyingState = FlyingStates.THRUST;

		ScoreController.ShowScore();
		ScoreController.ShowGain();
		ScoreController.ShowCombo();
	}

	private void DeliverPayload() {
		Payload.transform.SetParent(null, true);

		Rigidbody2D payloadBody = Payload.GetComponent<Rigidbody2D>();
		ConstantForce2D payloadForce = Payload.GetComponent<ConstantForce2D>();

		payloadBody.bodyType = RigidbodyType2D.Dynamic;
		payloadBody.velocity = rocketBody.velocity;
		payloadForce.relativeForce = new Vector2(rocketForce.relativeForce.x, rocketForce.relativeForce.y + 5);

		Animator secondStageEngineAnimator = SecondStageEngine.GetComponentInChildren<Animator>();
		secondStageEngineAnimator.SetTrigger("TurnOn");
	}

	public void StartFirstStageEngines() {
		Animator[] firstStageEngineAnimators = FirstStageEngines.GetComponentsInChildren<Animator>();
		foreach (Animator firstStageEngineAnimator in firstStageEngineAnimators) {
			firstStageEngineAnimator.SetTrigger("TurnOn");
		}
	}

	public void StopFirstStageEngines() {
		Animator[] firstStageEngineAnimators = FirstStageEngines.GetComponentsInChildren<Animator>();
		foreach (Animator firstStageEngineAnimator in firstStageEngineAnimators) {
			firstStageEngineAnimator.SetTrigger("TurnOff");
		}
	}

	private bool DotCurveReady() {
		return currentDot != null && nextDot != null;
	}

	private bool DotCurveEnded() {
		return currentDotId >= DotCurve.transform.childCount - 1;
	}

	public float Speed() {
		return Mathf.Abs(Vector3.Distance(transform.position, previousPosition));
	}

	public float CurrentSegmentOrientation() {
		Vector3 deltaDots = nextDot.transform.position - currentDot.transform.position;
		return Mathf.Atan2(deltaDots.y, deltaDots.x) * Mathf.Rad2Deg;
	}

	public bool IsBallistic() {
		return flyingState == FlyingStates.BALLISTRIC;
	}

	private void SetDotsSize() {
		foreach (Transform dotTransform in DotCurve.transform) {
			float distanceFromDot = Vector3.Distance(transform.position, dotTransform.position);
			float dotSize = Mathf.Max(8 - distanceFromDot, 2) / 8f;
			dotTransform.localScale = Vector3.one * dotSize;
		}
	}
}