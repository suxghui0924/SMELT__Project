using System.Collections.Generic;
using UnityEngine;
using static EricUpgradeStat;

/// <summary>
/// 플레이어 스탯 및 업그레이드 관리.
/// 담당자: 이윤건
/// </summary>
public class PlayerStatManager : MonoBehaviour, ISaveable
{
    public static PlayerStatManager Instance { get; private set; }

    // ─────────────────────────────────────────
    // 런타임 스탯
    // ─────────────────────────────────────────

    public float UPMakeSpeedJuice { get; private set; } = 1.0f;//판매 수익 보너스 (0.05 = 5%)
    public float UPMakeSpeedWeapon { get; private set; } = 1.0f;//판매 수익 보너스 (0.05 = 5%)
    public float UPParryRange { get; private set; } =   1.0f;   // 패링 판정 범위 배율 (0.05 = 5%)
    public float UPMoreSell { get; private set; } =     0.0f;   //판매 수익 보너스 (0.05 = 5%)
    public float UPAttackSpeed { get; private set; } =  0.0f;   //공속 (0.05 = 5%)
    public float UPGetApple { get; private set; } =     0.0f;   //사과 배수 (0.05 = 5%)
    public float UPGetLemon { get; private set; } =     0.0f;   //레몬 배수 (0.05 = 5%)
    public float UPGetMelon { get; private set; } =     0.0f;   //멜론 배수 (0.05 = 5%)
    public float UPGetGrape { get; private set; } =     0.0f;   //포도 배수 (0.05 = 5%)
    public float UPGetOrange { get; private set; } =    0.0f;   //귤 배수 (0.05 = 5%)
    public float UPGetFriuts { get; private set; } =    0.0f;   //전체 과일 배수 (0.05 = 5%)

    private List<string> _purchasedUpgrades = new List<string>();

    // ─────────────────────────────────────────
    // 초기화
    // ─────────────────────────────────────────
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        SaveManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        SaveManager.Instance?.Unregister(this);
    }

    // ─────────────────────────────────────────
    // 세이브 / 로드
    // ─────────────────────────────────────────
    public void OnSave(SaveData data)
    {
        data.makeSpeedJuice = UPMakeSpeedJuice;
        data.makeSpeedWeapon = UPMakeSpeedWeapon;
        data.parryRange = UPParryRange;
        data.moreSell = UPMoreSell;
        data.attackSpeed = UPAttackSpeed;
        data.getApple = UPGetApple;
        data.getLemon = UPGetLemon;
        data.getMelon = UPGetMelon;
        data.getGrape = UPGetGrape;
        data.getOrange = UPGetOrange;
        data.getFriuts = UPGetFriuts;
        data.purchasedUpgrades = _purchasedUpgrades;
    }

    public void OnLoad(SaveData data)
    {
        UPMakeSpeedJuice = data.makeSpeedJuice;
        UPMakeSpeedWeapon = data.makeSpeedWeapon;
        UPParryRange = data.parryRange;
        UPMoreSell = data.moreSell;
        UPAttackSpeed = data.attackSpeed;
        UPGetApple = data.getApple;
        UPGetLemon = data.getLemon;
        UPGetMelon = data.getMelon;
        UPGetGrape = data.getGrape;
        UPGetOrange = data.getOrange;
        UPGetFriuts = data.getFriuts;
        _purchasedUpgrades = data.purchasedUpgrades;
    }

    // ─────────────────────────────────────────
    // 업그레이드 구매
    // ─────────────────────────────────────────

    /// <summary>
    /// 업그레이드 구매.
    /// ex) BuyUpgrade("upgrade_parry_range", 200)
    ///     → 골드 200 차감 후 패링 범위 증가
    /// </summary>
    public bool BuyUpgrade(string upgradeId, int cost)
    {
        // 이미 구매한 업그레이드인지 확인
        if (_purchasedUpgrades.Contains(upgradeId))
        {
            Debug.LogWarning($"[Upgrade] 이미 구매함: {upgradeId}");
            return false;
        }

        // 골드 차감
        /*if (!InventoryManager.Instance.SpendGold(cost))
            return false;*/

        // 업그레이드 적용
        _purchasedUpgrades.Add(upgradeId);
        ApplyUpgrade(upgradeId);
        Debug.Log($"[Upgrade] 구매 완료: {upgradeId}");
        return true;
    }

    private void ApplyUpgrade(string id)
    {
        switch (id)
        {
            // 패링 판정 범위 증가
            case "upgrade_parry_range":
                UPParryRange += 0.2f;
                break;

            // 판매 수익 5% 증가
            case "upgrade_sales_05":
                UPMoreSell += 0.05f;
                break;
/*
            // 가공 속도 향상
            case "upgrade_process_speed":
                UPMakeSpeed += 0.3f;
                break;*/

            default:
                Debug.LogWarning($"[Upgrade] 알 수 없는 업그레이드: {id}");
                break;
        }
    }

    public bool IsUpgradePurchased(string upgradeId)
        => _purchasedUpgrades.Contains(upgradeId);

    // ─────────────────────────────────────────
    // 스탯 적용 헬퍼
    // ─────────────────────────────────────────

    /// <summary>판매 가격에 수익 보너스 적용. ex) 기본가 100 → 105 (5% 보너스 시)</summary>
    public int ApplySalesBonus(int basePrice)
        => Mathf.RoundToInt(basePrice * (1f + UPMoreSell));
}