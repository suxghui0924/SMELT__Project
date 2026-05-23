using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 스킬 트리 오브젝트 비주얼 컨트롤러 (회전 애니메이션 + 발광)
/// Animator bool 파라미터 "IsInteracting":
///   false → 회전(Spin) 상태, true → 정지 + 발광 상태
/// 담당자: 이도윤
/// </summary>
public class SkillTreeZoneVisual : MonoBehaviour
{
    [Header("발광")]
    [SerializeField] private Light2D _glowLight;
    [SerializeField] private float   _lightMinIntensity = 0.8f;
    [SerializeField] private float   _lightMaxIntensity = 2.0f;
    [SerializeField] private float   _pulseSpeed        = 1.5f;

    private Coroutine _pulseCoroutine;

    private void OnEnable()  => SkillTreeController.OnStateChanged += SetInteracting;
    private void OnDisable() => SkillTreeController.OnStateChanged -= SetInteracting;

    private void Awake()
    {
        if (_glowLight != null)
        {
            _glowLight.color   = new Color(1f, 0.85f, 0.2f); // 노란빛
            _glowLight.enabled = false;
        }
    }

    public void SetInteracting(bool interacting)
    {

        if (_pulseCoroutine != null) StopCoroutine(_pulseCoroutine);

        if (_glowLight != null)
        {
            _glowLight.enabled = interacting;
            if (interacting)
                _pulseCoroutine = StartCoroutine(PulseLight());
        }
    }

    private IEnumerator PulseLight()
    {
        while (true)
        {
            float t = (Mathf.Sin(Time.time * _pulseSpeed) + 1f) / 2f;
            _glowLight.intensity = Mathf.Lerp(_lightMinIntensity, _lightMaxIntensity, t);
            yield return null;
        }
    }
}
