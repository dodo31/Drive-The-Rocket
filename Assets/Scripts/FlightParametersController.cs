using UnityEngine;
using UnityEngine.UI;

public class FlightParametersController : MonoBehaviour {
	public float UnitScale;

	public RocketBehaviour RocketBehaviour;

	public Text AltitudeText;
	public Text SpeedText;

	public void Update() {
		float fps = 1.0f / Time.deltaTime;

		float altitude = Mathf.Floor(RocketBehaviour.transform.position.y * UnitScale);
		float speed = Mathf.Floor(RocketBehaviour.Speed() / 3600f * fps * 1000 * UnitScale);

		if (altitude > 1000) {
			AltitudeText.text = Mathf.Floor(altitude / 1000) + " " + (altitude - Mathf.Floor(altitude / 1000) * 1000) + " m";
		} else {
			AltitudeText.text = altitude + " m";
		}

		if (speed > 1000) {
			SpeedText.text = Mathf.Floor(speed / 1000) + " " + (speed - Mathf.Floor(speed / 1000) * 1000) + " km/h";
		} else {
			SpeedText.text = speed + " km/h";
		}
	}

	public void HideAltitudeIndicator() {
		AltitudeText.transform.parent.gameObject.SetActive(false);
	}

	public void ShowAltitudeIndicator() {
		AltitudeText.transform.parent.gameObject.SetActive(true);
	}

	public void HideSpeedIndicator() {
		SpeedText.transform.parent.gameObject.SetActive(false);
	}

	public void ShowSpeedIndicator() {
		SpeedText.transform.parent.gameObject.SetActive(true);
	}
}