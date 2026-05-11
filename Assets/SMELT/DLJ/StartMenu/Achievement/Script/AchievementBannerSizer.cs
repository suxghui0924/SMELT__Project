using System.Collections;
using UnityEngine;

public class AchievementBannerSizer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Coroutine bannerSizeCoroutine;
    private Coroutine insideSizeCoroutine;

    public Transform Inside;


    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ChangeSize(float startWidth, float targetWidth, float height, float duration)
    {
        gameObject.SetActive(true);
        spriteRenderer.color = Color.white;
        if (bannerSizeCoroutine != null)
            StopCoroutine(bannerSizeCoroutine);
            StartCoroutine(DisableBanner());

        bannerSizeCoroutine = StartCoroutine(SizeRoutine(startWidth, targetWidth, height, duration));
    }
    public void ChangeInsideSize(float startSize, float targetSize, float duration)
    {
        if (insideSizeCoroutine != null)
            StopCoroutine(insideSizeCoroutine);

        bannerSizeCoroutine = StartCoroutine(InsideSizer(startSize, targetSize, duration));
    }

    private IEnumerator SizeRoutine(float startWidth, float targetWidth, float height, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            float width = Mathf.Lerp(startWidth, targetWidth, t);

            spriteRenderer.size = new Vector2(width, height);

            yield return null;
        }

        spriteRenderer.size = new Vector2(targetWidth, height);
    }
    private IEnumerator InsideSizer(float startSize, float targetSize, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            float size = Mathf.Lerp(startSize, targetSize, t);

            Inside.localScale = new Vector2(size, 1);

            yield return null;
        }

        Inside.localScale = new Vector2(targetSize, 1);
    }
    private IEnumerator DisableBanner()
    {
        yield return new WaitForSeconds(2);
        StartCoroutine(SizeRoutine(11.3f, 0, 1.37f, 0.5f));
        StartCoroutine(InsideSizer(1, 0, 0.45f));

        float duration = 0.2f;
        float timer = 0f;
        Color startColor = spriteRenderer.color;

        yield return new WaitForSeconds(0.2f);
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;


            float alpha = Mathf.Lerp(1, 0, t);

            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, 0f);

        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}
