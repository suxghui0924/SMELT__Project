using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AchievementBannerSizer : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private RectTransform tr;

    private Coroutine bannerSizeCoroutine;
    private Coroutine insideSizeCoroutine;

    public Transform Inside;


    private void Awake()
    {
        if (image == null)
            image = GetComponent<Image>();

        // 도전과제 배너가 항상 모든 UI 위에 표시되도록 루트 Canvas의 sortingOrder를 최상단으로 설정
        Canvas rootCanvas = GetComponentInParent<Canvas>();
        if (rootCanvas != null)
            rootCanvas.rootCanvas.sortingOrder = 1000;
    }

    public void ChangeSize(float startWidth, float targetWidth, float height, float duration)
    {
        gameObject.SetActive(true);
        image.color = Color.white;
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
            timer += Time.unscaledDeltaTime;

            float t = timer / duration;

            float width = Mathf.Lerp(startWidth, targetWidth, t);

            tr.sizeDelta = new Vector2(width, height);

            yield return null;
        }

        tr.sizeDelta = new Vector2(targetWidth, height);
    }
    private IEnumerator InsideSizer(float startSize, float targetSize, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

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
        StartCoroutine(SizeRoutine(1248.5f, 0, 191f, 0.5f));
        StartCoroutine(InsideSizer(1, 0, 0.45f));

        float duration = 0.2f;
        float timer = 0f;
        Color startColor = image.color;

        yield return new WaitForSeconds(0.2f);
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / duration;


            float alpha = Mathf.Lerp(1, 0, t);

            image.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        image.color = new Color(startColor.r, startColor.g, startColor.b, 0f);

        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}
