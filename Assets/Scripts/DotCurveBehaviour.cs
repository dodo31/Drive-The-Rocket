using UnityEngine;
using System.Collections;

public class DotCurveBehaviour : MonoBehaviour {
	public float MaxDistance;
	public float MaxHeight;

	public IEnumerator GenerateThrustingCurveSin(Vector3 startPosition) {
		enabled = false;

		GameObject previousDot = null;
		for (float i = 0.1f * Mathf.Deg2Rad; i <= 60 * Mathf.Deg2Rad; i += 0.002f) {
			GameObject dot = Instantiate(Resources.Load<GameObject>("Dot"));
			dot.name = "Dot" + (transform.childCount + 1);

			dot.transform.position = new Vector3(startPosition.x + Mathf.Cos(i + Mathf.PI) * (MaxDistance / 2f) + (MaxDistance / 2f), startPosition.y + Mathf.Sin(i) * MaxHeight, 0);
			if (previousDot != null) {
				Vector3 deltaFromPrevious = dot.transform.position - previousDot.transform.position;
				float dotOrientation = Mathf.Atan2(deltaFromPrevious.y, deltaFromPrevious.x) * Mathf.Rad2Deg - 90;
				dot.transform.rotation = Quaternion.Euler(0, 0, dotOrientation);
			}

			dot.transform.localScale = Vector3.one * 0.2f;

			dot.transform.SetParent(transform, false);

			previousDot = dot;

			yield return new WaitForSeconds(0.001f);
		}
	}

	public void GenerateBallisticCurve(Vector3 startPosition, float horizontalSpeed) {
		GameObject previousDot = transform.GetChild(transform.childCount - 1).gameObject;
		int stepCount = 0;
		while (previousDot.transform.position.y > 0) {
			GameObject dot = Instantiate(Resources.Load<GameObject>("Dot"));
			dot.name = "Dot" + (transform.childCount + 1);

			dot.transform.position = new Vector3(startPosition.x + stepCount * horizontalSpeed, startPosition.y - (0.0025f * (stepCount * stepCount)), 0);

			if (previousDot != null) {
				Vector3 deltaFromPrevious = dot.transform.position - previousDot.transform.position;
				float dotOrientation = Mathf.Atan2(deltaFromPrevious.y, deltaFromPrevious.x) * Mathf.Rad2Deg - 90;
				dot.transform.rotation = Quaternion.Euler(0, 0, dotOrientation);
			}

			dot.transform.SetParent(transform, false);

			previousDot = dot;
			stepCount++;
		}
	}

	public IEnumerator FadeInDots(int animatedDots) {
		const int FRAME_COUNT = 15;
		for (int i = 0; i < FRAME_COUNT; i++) {
			for (int j = 0; j < Mathf.Min(animatedDots, transform.childCount); j++) {
				GameObject dot = transform.GetChild(j).gameObject;
				SpriteRenderer dotSprite = dot.GetComponent<SpriteRenderer>();
				Color spriteColor = dotSprite.color;
				dotSprite.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, i / (FRAME_COUNT * 1f));
			}

			yield return new WaitForSeconds(0.02f);
		}
	}

	public IEnumerator FadeOutDots(int animatedDots) {
		const int FRAME_COUNT = 15;
		for (int i = 0; i < FRAME_COUNT; i++) {
			for (int j = 0; j < Mathf.Min(animatedDots, transform.childCount); j++) {
				GameObject dot = transform.GetChild(j).gameObject;
				SpriteRenderer dotSprite = dot.GetComponent<SpriteRenderer>();
				Color spriteColor = dotSprite.color;
				dotSprite.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, 1 - i / (FRAME_COUNT * 1f));
			}

			yield return new WaitForSeconds(0.02f);
		}
	}
}