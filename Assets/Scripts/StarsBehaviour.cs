using UnityEngine;

public class StarsBehaviour : MonoBehaviour {
	public int DotsCount;

	public GameObject Rocket;

	public GameObject Sky;
	public GameObject Stars;

	public void Start() {
		for (int i = 0; i < DotsCount; i++) {
			float posXRand = Random.Range(-100, 100);
			float posYRand = Random.Range(-100, 100);
			float posZRand = Random.Range(10, 14);

			GameObject star = Instantiate(Resources.Load<GameObject>("Star"));
			star.name = "Star" + (i + 1);
			star.transform.position = new Vector3(posXRand, posYRand, posZRand);
			star.transform.localScale = Vector3.one * 0.5f;

			float starAlpha = Mathf.Max(posYRand / 100f, 0);

			SpriteRenderer starSprite = star.GetComponent<SpriteRenderer>();
			Color spriteColor = starSprite.color;
			starSprite.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, starAlpha);

			star.transform.SetParent(Stars.transform, false);
		}
	}

	public void Update() {
		Stars.transform.position = new Vector3(Rocket.transform.position.x * 0.995f, Rocket.transform.position.y * 0.995f, 0);
		Sky.transform.position = new Vector3(Rocket.transform.position.x * 0.95f, Rocket.transform.position.y * 0.95f - 40, 0);
	}
}