using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour {
	private const string START_MENU_STATE = "StartMenu";
	private const string COUNTING_DOWN_STATE = "CountingDown";
	private const string FLYING_STATE = "Flying";
	private const string LANDING_STATE = "Landing";

	private const string CAMERA_TRACKING_STATE = "Falcon9Tracking";

	private const float COUTNDOWN_DURATION = 3;

	public GameObject MainCamera;
	public RocketBehaviour RocketBehaviour;
	public LaunchController LaunchController;
	public FlightParametersController flightParametersController;

	private string currentStateName;
	private string previousStateName;

	private Animator scheduler;
	private Animator cameraAnimator;

	private Vector3 cameraPositionOverride;
	private bool isCameraOnTrack;

	public void Start() {
		scheduler = this.GetComponent<Animator>();
		cameraAnimator = MainCamera.GetComponent<Animator>();

		cameraPositionOverride = Vector3.positiveInfinity;
		isCameraOnTrack = false;
	}

	public void Update() {
		AnimatorStateInfo currentStateInfo = scheduler.GetCurrentAnimatorStateInfo(0);

		this.UpdateStateName();

		if (currentStateName == START_MENU_STATE) {

		} else if (currentStateName == COUNTING_DOWN_STATE) {
			if (previousStateName == START_MENU_STATE) {
				LaunchController.StartLaunchSequence(COUTNDOWN_DURATION);
				cameraAnimator.SetTrigger("LookAtLaunchComplex");
			}
		} else if (currentStateName == FLYING_STATE) {
			if (previousStateName == COUNTING_DOWN_STATE) {
				cameraAnimator.SetTrigger("TrackFalcon9");

				flightParametersController.ShowAltitudeIndicator();
				flightParametersController.ShowSpeedIndicator();
			}
		} else if (currentStateName == LANDING_STATE) {

		}
	}

	public void LateUpdate() {
		if (currentStateName == FLYING_STATE) {
			Vector3 rocketPosition = RocketBehaviour.transform.position;
			if (cameraAnimator.GetCurrentAnimatorStateInfo(0).IsName(CAMERA_TRACKING_STATE)) {
				Vector3 targetCameraPosition = new Vector3(rocketPosition.x, rocketPosition.y + 1, -12);

				if (!isCameraOnTrack) {
					if (float.IsInfinity(cameraPositionOverride.x) || float.IsInfinity(cameraPositionOverride.y) || float.IsInfinity(cameraPositionOverride.z)) {
						cameraPositionOverride = MainCamera.transform.position;
					}

					Vector3 deltaFromCamera = cameraPositionOverride - targetCameraPosition;
					float distanceFromCamera = Mathf.Abs(Vector3.Distance(cameraPositionOverride, targetCameraPosition));

					if (distanceFromCamera >= 0.2f) {
						MainCamera.transform.position = cameraPositionOverride - deltaFromCamera.normalized * (distanceFromCamera / 2f);
						cameraPositionOverride = MainCamera.transform.position;
					} else {
						isCameraOnTrack = true;
					}
				}

				if (isCameraOnTrack) {
					MainCamera.transform.position = targetCameraPosition;
				}
			}
		}
	}

	private void UpdateStateName() {
		previousStateName = currentStateName;

		AnimatorStateInfo currentStateInfo = scheduler.GetCurrentAnimatorStateInfo(0);
		if (currentStateInfo.IsName(START_MENU_STATE)) {
			currentStateName = START_MENU_STATE;
		} else if (currentStateInfo.IsName(COUNTING_DOWN_STATE)) {
			currentStateName = COUNTING_DOWN_STATE;
		} else if (currentStateInfo.IsName(FLYING_STATE)) {
			currentStateName = FLYING_STATE;
		} else if (currentStateInfo.IsName(LANDING_STATE)) {
			currentStateName = LANDING_STATE;
		}
	}
}
