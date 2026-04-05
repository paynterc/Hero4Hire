using UnityEngine;

public class SlashFX : MonoBehaviour
{
    public float lifetime = 0.25f;
    public float growSpeed = 80f;

    private float timer;
    
    private Material mat;

	void Start()
	{
		mat = GetComponent<Renderer>().material;
	}

    void Update()
	{
		timer += Time.deltaTime;

		transform.localScale += new Vector3(growSpeed * Time.deltaTime, 0, 0);

		// Fade out
		Color c = mat.color;
		c.a = Mathf.Lerp(1f, 0f, timer / lifetime);
		mat.color = c;

		if (timer >= lifetime)
		{
			Destroy(gameObject);
		}
	}
}
