using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {
	private readonly Gain VERY_LOW_GAIN		= new Gain(0, 0, 50, new Vector2(10, 15));
	private readonly Gain LOW_GAIN			= new Gain(1, 20, 100, new Vector2(6, 10));
	private readonly Gain MEDIUM_GAIN		= new Gain(2, 40, 300, new Vector2(3, 6));
	private readonly Gain HIGH_GAIN			= new Gain(3, 60, 500, new Vector2(1, 3));
	private readonly Gain VERY_HIGH_GAIN	= new Gain(4, 80, 1000, new Vector2(0, 1));

	private readonly Vector2 COMBO_1_RANGE = new Vector2(48, 72);
	private readonly Vector2 COMBO_2_RANGE = new Vector2(28, 48);
	private readonly Vector2 COMBO_3_RANGE = new Vector2(12, 28);
	private readonly Vector2 COMBO_4_RANGE = new Vector2(5, 12);
	private readonly Vector2 COMBO_5_RANGE = new Vector2(0, 5);

	private const int GAUGE_MAX_POS_X = 570;

	private List<Gain> possibleGains;

	public Text ScoreText;
	public GameObject GainGauge;
	public Image GainGaugeSprite;
	public Text ComboText;

	private int scoreValue;
	private int gainValue;
	private int comboValue;

	private float lastDistanceFromDot;

	public void Start () {
		possibleGains = new List<Gain>() {
			VERY_LOW_GAIN,
			LOW_GAIN,
			MEDIUM_GAIN,
			HIGH_GAIN,
			VERY_HIGH_GAIN
		};
	}

	public void Update() {
		this.UpdateGaugePositionAndColor();

		ScoreText.text = "";

		string stringScore = scoreValue.ToString();
		for (int i = stringScore.Length - 1; i >= 0; i--) {
			if (((stringScore.Length - 1) - i) % 3 == 0 && i < stringScore.Length - 1) {
				ScoreText.text = " " + ScoreText.text;
			}
			ScoreText.text = stringScore.Substring(i, 1) + ScoreText.text;
		}
	}

	public void UpdateGaugePositionAndColor() {
		RectTransform gaugeTransform = GainGauge.GetComponent<RectTransform>();
		Vector2 gaugeAnchorPosition = gaugeTransform.anchoredPosition;

		int gainId = this.GetGainId(lastDistanceFromDot);
		Gain currentGain = possibleGains[gainId];

		Vector2 gainRange = currentGain.Range;

		float gainRangeLength = gainRange.y - gainRange.x;

		float gaugeRangeLength = GAUGE_MAX_POS_X / 5f;
		float min = gaugeRangeLength * gainId;
		float max = gaugeRangeLength * (gainId + 1);

		float targetLateralPosition = Mathf.Max(gaugeRangeLength - ((lastDistanceFromDot - gainRange.x) / gainRangeLength * gaugeRangeLength) + min, 0);

		const float GAUGE_SPEED = 5;

		if (gaugeAnchorPosition.x + GAUGE_SPEED < targetLateralPosition) {
			gaugeTransform.anchoredPosition = new Vector2(gaugeAnchorPosition.x += GAUGE_SPEED, gaugeAnchorPosition.y);
		} else if (gaugeAnchorPosition.x - GAUGE_SPEED > targetLateralPosition) {
			gaugeTransform.anchoredPosition = new Vector2(gaugeAnchorPosition.x -= GAUGE_SPEED, gaugeAnchorPosition.y);
		} else {
			gaugeTransform.anchoredPosition = new Vector2(targetLateralPosition, gaugeAnchorPosition.y);
		}

		int gaugeHue = currentGain.Hue;
		GainGaugeSprite.color = Color.HSVToRGB(gaugeHue / 255f, 0.75f, 1);
	}

	public void UpdateGainValue(float distanceFromDot) {
		int gainId = this.GetGainId(distanceFromDot);
		gainValue = possibleGains[gainId].Value;
	}

	public void UpdateComboValue(float angularDistance) {
		int comboId = this.GetComboId(angularDistance);
		comboValue = Mathf.Max(comboId + 1, 1);
		ComboText.text = "x" + comboValue;
	}

	public void UpdateScore() {
		scoreValue += Mathf.FloorToInt(gainValue * comboValue);
	}

	public void IncrementScoreFromDelivering(float angularDistance) {
		int comboId = this.GetComboId(angularDistance);
		scoreValue *= Mathf.Max(comboId + 1, 1);
	}

	public int GetGainId(float distanceFromDot) {
		if (distanceFromDot >= VERY_LOW_GAIN.Range.x && distanceFromDot < VERY_LOW_GAIN.Range.y) {
			return 0;
		} else if (distanceFromDot >= LOW_GAIN.Range.x && distanceFromDot < LOW_GAIN.Range.y) {
			return 1;
		} else if (distanceFromDot >= MEDIUM_GAIN.Range.x && distanceFromDot < MEDIUM_GAIN.Range.y) {
			return 2;
		} else if (distanceFromDot >= HIGH_GAIN.Range.x && distanceFromDot < HIGH_GAIN.Range.y) {
			return 3;
		} else if (distanceFromDot >= VERY_HIGH_GAIN.Range.x && distanceFromDot < VERY_HIGH_GAIN.Range.y) {
			return 4;
		} else {
			return 0;
		}
	}

	public int GetComboId(float angularDistance) {
		if (angularDistance >= COMBO_1_RANGE.x && angularDistance < COMBO_1_RANGE.y) {
			return 0;
		} else if (angularDistance >= COMBO_2_RANGE.x && angularDistance < COMBO_2_RANGE.y) {
			return 1;
		} else if (angularDistance >= COMBO_3_RANGE.x && angularDistance < COMBO_3_RANGE.y) {
			return 2;
		} else if (angularDistance >= COMBO_4_RANGE.x && angularDistance < COMBO_4_RANGE.y) {
			return 3;
		} else if (angularDistance >= COMBO_5_RANGE.x && angularDistance < COMBO_5_RANGE.y) {
			return 4;
		} else {
			return -1;
		}
	}

	public Gain GetGain(int gainId) {
		return possibleGains[gainId];
	}

	public void HideScore() {
		ScoreText.transform.parent.gameObject.SetActive(false);
	}

	public void ShowScore() {
		ScoreText.transform.parent.gameObject.SetActive(true);
	}

	public void HideGain() {
		GainGauge.transform.parent.gameObject.SetActive(false);
	}

	public void ShowGain() {
		GainGauge.transform.parent.gameObject.SetActive(true);
	}

	public void HideCombo() {
		ComboText.gameObject.SetActive(false);
	}

	public void ShowCombo() {
		ComboText.gameObject.SetActive(true);
	}

	public float LastDistanceFromDot {
		get {
			return lastDistanceFromDot;
		}

		set {
			lastDistanceFromDot = value;
		}
	}
}