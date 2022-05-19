using UnityEngine;

public class Gain {
	private int id;
	private int hue;
	private int value;
	private Vector2 range;

	public Gain(int id, int hue, int value, Vector2 range) {
		this.id = id;
		this.hue = hue;
		this.value = value;
		this.range = range;
	}

	public int Id {
		get {
			return id;
		}

		set {
			id = value;
		}
	}

	public int Hue {
		get {
			return hue;
		}

		set {
			hue = value;
		}
	}

	public int Value {
		get {
			return value;
		}

		set {
			this.value = value;
		}
	}

	public Vector2 Range {
		get {
			return range;
		}

		set {
			range = value;
		}
	}
}