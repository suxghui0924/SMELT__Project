using UnityEngine;

/// <summary>
/// 자동 광석 채집 — 플레이어가 어느 구역에 있든 일정 시간마다 랜덤 과일석 1개를 자동 수집.
///
/// [Inspector 설정]
///   gatherInterval : 채집 주기 (초, 기본 2.5초)
///
/// 담당자: 이도윤
/// </summary>
public class AutoGatherManager : MonoBehaviour
{
    public static AutoGatherManager Instance { get; private set; }

    [Header("자동 채집 설정")]
    [Tooltip("광석 자동 채집 주기 (초)")]
    [SerializeField] private float gatherInterval = 2.5f;

    private static readonly string[] ORE_IDS =
    {
        "fruitstone_apple",
        "fruitstone_melon",
        "fruitstone_orange",
        "fruitstone_lemon",
        "fruitstone_grape",
    };

    private float _timer;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < gatherInterval) return;
        _timer = 0f;

        string id = ORE_IDS[Random.Range(0, ORE_IDS.Length)];
        InventoryManager.Instance?.AddItem(id, 1);
        PrototypeHUD.Instance?.OnOreGathered(id);
    }
}
