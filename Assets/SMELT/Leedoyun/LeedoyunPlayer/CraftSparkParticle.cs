using UnityEngine;

/// <summary>
/// 무기 제작 전용 스파크 파티클.
/// CraftAnimationController에서 Play()를 호출하면 _interval 간격으로 3번 터짐.
///
/// 담당자: 이도윤
/// </summary>
public class CraftSparkParticle : MonoBehaviour
{
    public static CraftSparkParticle Instance { get; private set; }

    [SerializeField] private ParticleSystem _particle;
    [Tooltip("파티클 재생 간격 (초). 줄일수록 빠르게 연속 재생.")]
    [SerializeField] private float _interval = 0.4f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (_particle == null)
            _particle = GetComponent<ParticleSystem>();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Play()
    {
        if (_particle == null) return;
        _particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _particle.Play();
    }
}
