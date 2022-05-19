using UnityEngine;

public class BallisticPhaseController : MonoBehaviour {
	public RocketBehaviour rocketBehaviour;
	public GameObject FirstStage;

	public GameObject RocketShape;
	public GameObject FirstStageShape;

	public void Update() {
		if (rocketBehaviour != null) {
			if (rocketBehaviour.IsBallistic()) {
				if (!this.PayloadDelivered()) {
					if (!RocketShape.activeInHierarchy) {
						RocketShape.SetActive(true);
					}
				} else {
					if (!FirstStageShape.activeInHierarchy) {
						RocketShape.SetActive(false);
						FirstStageShape.SetActive(true);
					}
				}

				RocketShape.transform.position = rocketBehaviour.transform.position;
				FirstStageShape.transform.position = FirstStage.transform.position;
			} else {
				if (FirstStageShape.activeInHierarchy) {
					FirstStageShape.SetActive(false);
				}
			}
		}
	}

	private bool PayloadDelivered() {
		return rocketBehaviour.Payload.transform.parent == null;
	}
}