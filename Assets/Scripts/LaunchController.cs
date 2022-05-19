using UnityEngine;

public class LaunchController : MonoBehaviour {
	private const float STARTING_ENGINES_TIME = 1;
	private const float LAUNCH_ROCKET_TIME = 0;

	private const float SMOKE_DURATION = 5;

	private enum LaunchStates { FILLED, ENGINES_FIRE, LIFTOFF }
	private LaunchStates launchState;

	public RocketBehaviour rocketBehaviour;
	public GameObject Smoke;

	private float launchTimeTarget;

	private float smokeTimeTarget;

	public void Start () {
		launchState = LaunchStates.FILLED;

		launchTimeTarget = float.PositiveInfinity;
	}

	public void Update () {
		switch (launchState) {
		case LaunchStates.FILLED:
			if (launchTimeTarget - Time.time < STARTING_ENGINES_TIME) {
				rocketBehaviour.StartFirstStageEngines();
				this.StartSmoke(SMOKE_DURATION);
				launchState = LaunchStates.ENGINES_FIRE;
			}
			break;
		case LaunchStates.ENGINES_FIRE:
			if (Time.time >= smokeTimeTarget) {
				this.StopSmoke();
			}

			if (launchTimeTarget - Time.time < LAUNCH_ROCKET_TIME) {
				rocketBehaviour.LaunchRocket();

				Animator launchPadCLosingAnimator = this.GetComponent<Animator>();
				launchPadCLosingAnimator.SetTrigger("CloseFootBridge");

				launchState = LaunchStates.LIFTOFF;
			}
			break;
		}
	}

	public void StartLaunchSequence(float countDownDuration) {
		launchTimeTarget = Time.time + countDownDuration;
	}

	private void StartSmoke(float duration) {
		ParticleSystem[] smokeEmitters = Smoke.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem smokeEmitter in smokeEmitters) {
			smokeEmitter.Play();
			smokeTimeTarget = Time.time + duration;
		}
	}

	private void StopSmoke() {
		ParticleSystem[] smokeEmitters = Smoke.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem smokeEmitter in smokeEmitters) {
			if (smokeEmitter.isPlaying) {
				smokeEmitter.Stop();
				smokeTimeTarget = float.PositiveInfinity;
			}
		}
	}
}
