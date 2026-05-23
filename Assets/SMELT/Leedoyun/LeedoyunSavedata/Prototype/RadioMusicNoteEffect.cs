using System.Collections;
using UnityEngine;

/// <summary>
/// 라디오 오브젝트 위에 음표가 떠오르는 이펙트.
/// 라디오 GameObject에 붙이고 noteSprites에 음표 이미지 2개 연결.
/// </summary>
public class RadioMusicNoteEffect : MonoBehaviour
{
    [Header("음표 스프라이트 (2개 연결)")]
    [SerializeField] private Sprite[] noteSprites;

    [Header("이펙트 설정")]
    [SerializeField] private float spawnInterval = 0.65f;   // 음표 생성 간격 (초)
    [SerializeField] private float floatHeight   = 1.0f;    // 위로 올라가는 거리
    [SerializeField] private float duration      = 1.4f;    // 음표 하나의 수명
    [SerializeField] private float noteScale     = 0.25f;   // 음표 크기
    [SerializeField] private float spawnOffsetY  = 0.3f;    // 라디오 상단 오프셋
    [SerializeField] private float spreadX       = 0.15f;   // 좌우 랜덤 범위

    [Header("렌더링")]
    [SerializeField] private string sortingLayerName = "Default";
    [SerializeField] private int    sortingOrder     = 10;
    [SerializeField] private Color  noteColor        = Color.white;

    private int _noteIndex;

    private void OnEnable()
    {
        StartCoroutine(SpawnLoop());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (noteSprites == null || noteSprites.Length == 0) continue;

            SpawnNote();
        }
    }

    private void SpawnNote()
    {
        var go = new GameObject("MusicNote");
        go.transform.position   = transform.position + new Vector3(
            Random.Range(-spreadX, spreadX), spawnOffsetY, 0f);
        go.transform.localScale = Vector3.one * noteScale;

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite           = noteSprites[_noteIndex % noteSprites.Length];
        sr.color            = noteColor;
        sr.sortingLayerName = sortingLayerName;
        sr.sortingOrder     = sortingOrder;

        _noteIndex++;

        StartCoroutine(AnimateNote(go.transform, sr));
    }

    private IEnumerator AnimateNote(Transform noteTr, SpriteRenderer sr)
    {
        float elapsed  = 0f;
        Vector3 origin = noteTr.position;

        // 음표마다 살짝 다른 좌우 흔들림
        float wobbleSpeed = Random.Range(3.5f, 5.5f);
        float wobbleAmp   = Random.Range(0.04f, 0.08f);
        float xDrift      = Random.Range(-0.08f, 0.08f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // 위로 ease-out 이동
            float y = Mathf.Lerp(0f, floatHeight, 1f - Mathf.Pow(1f - t, 2.2f));
            // 좌우 부드러운 흔들림
            float x = Mathf.Sin(elapsed * wobbleSpeed) * wobbleAmp + xDrift * t;

            noteTr.position = origin + new Vector3(x, y, 0f);

            // 전반 유지, 후반 페이드 아웃
            float alpha = t < 0.45f ? 1f : Mathf.Lerp(1f, 0f, (t - 0.45f) / 0.55f);
            sr.color = new Color(noteColor.r, noteColor.g, noteColor.b, alpha);

            yield return null;
        }

        Destroy(noteTr.gameObject);
    }
}
