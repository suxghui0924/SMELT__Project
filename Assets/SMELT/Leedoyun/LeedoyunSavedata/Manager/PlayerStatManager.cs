using System.Collections.Generic;
using UnityEngine;

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

    public float UpMakeSpeedJuice { get; private set; } = 1.0f;//판매 수익 보너스 (0.05 = 5%)
    public float UpMakeSpeedWeapon { get; private set; } = 1.0f;//판매 수익 보너스 (0.05 = 5%)
    public float UpParryRange { get; private set; } =   1.0f;   // 패링 판정 범위 배율 (0.05 = 5%)
    public float UpMoreSell { get; private set; } =     0.0f;   //판매 수익 보너스 (0.05 = 5%)
    public float UpAttackSpeed { get; private set; } =  0.0f;   //공속 (0.05 = 5%)
    public float UpGetApple { get; private set; } =     0.0f;   //사과 배수 (0.05 = 5%)
    public float UpGetLemon { get; private set; } =     0.0f;   //레몬 배수 (0.05 = 5%)
    public float UpGetMelon { get; private set; } =     0.0f;   //멜론 배수 (0.05 = 5%)
    public float UpGetGrape { get; private set; } =     0.0f;   //포도 배수 (0.05 = 5%)
    public float UpGetOrange { get; private set; } =    0.0f;   //귤 배수 (0.05 = 5%)
    public float UpGetFruits { get; private set; } =    0.0f;   //전체 과일 배수 (0.05 = 5%)

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
        data.makeSpeedWeapon = UpMakeSpeedWeapon;
        data.parryRange = UpParryRange;
        data.moreSell = UpMoreSell;
        data.attackSpeed = UpAttackSpeed;
        data.getApple = UpGetApple;
        data.getLemon = UpGetLemon;
        data.getMelon = UpGetMelon;
        data.getGrape = UpGetGrape;
        data.getOrange = UpGetOrange;
        data.getFriuts = UpGetFruits;
        data.purchasedUpgrades = _purchasedUpgrades;
    }

    public void OnLoad(SaveData data)
    {
        UpMakeSpeedWeapon = data.makeSpeedWeapon;
        UpParryRange = data.parryRange;
        UpMoreSell = data.moreSell;
        UpAttackSpeed = data.attackSpeed;
        UpGetApple = data.getApple;
        UpGetLemon = data.getLemon;
        UpGetMelon = data.getMelon;
        UpGetGrape = data.getGrape;
        UpGetOrange = data.getOrange;
        UpGetFruits = data.getFriuts;
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
    public bool BuyUpgrade(string upgradeId, long cost, float times)
    {
        // 이미 구매한 업그레이드인지 확인
        if (_purchasedUpgrades.Contains(upgradeId))
        {
            Debug.LogWarning($"[Upgrade] 이미 구매함: {upgradeId}");
            return false;
        }

        // 골드 차감
        if (!InventoryManager.Instance.SpendGold((int)cost))
            return false;

        // 업그레이드 적용
        _purchasedUpgrades.Add(upgradeId);
        ApplyUpgrade(upgradeId, times);
        Debug.Log($"[Upgrade] 구매 완료: {upgradeId}");
        return true;
    }

    private void ApplyUpgrade(string id, float times)
    {
        switch (id)
        {
            case "AllUp1":
                UpGetFruits += times;
                 break;
            case "AllUp2":
                UpGetFruits += times;
                break;
            case "AllUp3":
                UpGetFruits += times;
                break;
            case "AllUp4":
                UpGetFruits += times;
                break;
            case "AllUp5":
                UpGetFruits += times;
                break;
            case "AppleUp1":
                UpGetApple += times;
                break;
            case "AppleUp2":
                UpGetApple += times;
                break;
            case "AppleUp3":
                UpGetApple += times;
                break;
            case "AppleUp4":
                UpGetApple += times;
                break; ;
            case "LemonUp1":
                UpGetLemon += times;
                break;
            case "LemonUp2":
                UpGetLemon += times;
                break;
            case "LemonUp3":
                UpGetLemon += times;
                break;
            case "LemonUp4":
                UpGetLemon += times;
                break; ;
            case "MelonUp1":
                UpGetMelon += times;
                break;
            case "MelonUp2":
                UpGetMelon += times;
                break;
            case "MelonUp3":
                UpGetMelon += times;
                break;
            case "MelonUp4":
                UpGetMelon += times;
                break; ;
            case "GrapeUp1":
                UpGetGrape += times;
                break;
            case "GrapeUp2":
                UpGetGrape += times;
                break;
            case "GrapeUp3":
                UpGetGrape += times;
                break;
            case "GrapeUp4":
                UpGetGrape += times;
                break; ;
            case "OrangeUp1":
                UpGetOrange += times;
                break;
            case "OrangeUp2":
                UpGetOrange += times;
                break;
            case "OrangeUp3":
                UpGetOrange += times;
                break;
            case "OrangeUp4":
                UpGetOrange += times;
                break; ;
            case "WeaponUp1":
                UpMakeSpeedWeapon += times;
                break;
            case "WeaponUp2":
                UpMakeSpeedWeapon += times;
                break;
            case "WeaponUp3":
                UpMakeSpeedWeapon += times;
                break;
            case "WeaponUp4":
                UpMakeSpeedWeapon += times;
                break;
            case "WeaponUp5":
                UpMakeSpeedWeapon += times;
                break;
            case "WeaponUp6":
                UpMakeSpeedWeapon += times;
                break;
            case "UpParryRange1":
                UpMakeSpeedWeapon += times;
                break;
            case "UpParryRange2":
                UpMakeSpeedWeapon += times;
                break;
            case "AttackSpeedUp1":
                UpAttackSpeed += times;
                break;
            case "AttackSpeedUp2":
                UpAttackSpeed += times;
                break;
            case "AttackSpeedUp3":
                UpAttackSpeed += times;
                break;
            case "AttackSpeedUp4":
                UpAttackSpeed += times;
                break;
            case "CoinUp1":
                UpMoreSell += times;
                break;
            case "CoinUp2":
                UpMoreSell += times;
                break;
            case "CoinUp3":
                UpMoreSell += times;
                break;
            case "CoinUp4":
                UpMoreSell += times;
                break;
            case "CoinUp5":
                UpMoreSell += times;
                break;
            case "CoinUp6":
                UpMoreSell += times;
                break;
            case "CoinUp7":
                UpMoreSell += times;
                break;
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
        => Mathf.RoundToInt(basePrice * (1f + UpMoreSell));
}