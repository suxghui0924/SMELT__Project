using UnityEngine;

public class TextUP : MonoBehaviour
{
    private float speed = 200f;
    private float destroyY = 500f;

    private RectTransform rect;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        rect.anchoredPosition += Vector2.up * speed * Time.deltaTime;

        if (rect.anchoredPosition.y > destroyY)
        {
            Destroy(gameObject);
        }
    }
}
