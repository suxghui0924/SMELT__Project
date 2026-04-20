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
    public float UPGetFruits { get; private set; } =    0.0f;   //전체 과일 배수 (0.05 = 5%)

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
        data.makeSpeedWeapon = UPMakeSpeedWeapon;
        data.parryRange = UPParryRange;
        data.moreSell = UPMoreSell;
        data.attackSpeed = UPAttackSpeed;
        data.getApple = UPGetApple;
        data.getLemon = UPGetLemon;
        data.getMelon = UPGetMelon;
        data.getGrape = UPGetGrape;
        data.getOrange = UPGetOrange;
        data.getFriuts = UPGetFruits;
        data.purchasedUpgrades = _purchasedUpgrades;
    }

    public void OnLoad(SaveData data)
    {
        UPMakeSpeedWeapon = data.makeSpeedWeapon;
        UPParryRange = data.parryRange;
        UPMoreSell = data.moreSell;
        UPAttackSpeed = data.attackSpeed;
        UPGetApple = data.getApple;
        UPGetLemon = data.getLemon;
        UPGetMelon = data.getMelon;
        UPGetGrape = data.getGrape;
        UPGetOrange = data.getOrange;
        UPGetFruits = data.getFriuts;
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
    public bool BuyUpgrade(string upgradeId, int cost, float times)
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
        ApplyUpgrade(upgradeId, times);
        Debug.Log($"[Upgrade] 구매 완료: {upgradeId}");
        return true;
    }

    private void ApplyUpgrade(string id, float times)
    {
        switch (id)
        {
            case "AllUp1":
                UPGetFruits += times;
                break;
            case "AllUp2":
                UPGetFruits += times;
                break;
            case "AllUp3":
                UPGetFruits += times;
                break;
            case "AllUp4":
                UPGetFruits += times;
                break;
            case "AllUp5":
                UPGetFruits += times;
                break;
            case "AppleUp1":
                UPGetApple += times;
                break;
            case "AppleUp2":
                UPGetApple += times;
                break;
            case "AppleUp3":
                UPGetApple += times;
                break;
            case "AppleUp4":
                UPGetApple += times;
                break; ;
            case "LemonUp1":
                UPGetLemon += times;
                break;
            case "LemonUp2":
                UPGetLemon += times;
                break;
            case "LemonUp3":
                UPGetLemon += times;
                break;
            case "LemonUp4":
                UPGetLemon += times;
                break; ;
            case "MelonUp1":
                UPGetMelon += times;
                break;
            case "MelonUp2":
                UPGetMelon += times;
                break;
            case "MelonUp3":
                UPGetMelon += times;
                break;
            case "MelonUp4":
                UPGetMelon += times;
                break; ;
            case "GrapeUp1":
                UPGetGrape += times;
                break;
            case "GrapeUp2":
                UPGetGrape += times;
                break;
            case "GrapeUp3":
                UPGetGrape += times;
                break;
            case "GrapeUp4":
                UPGetGrape += times;
                break; ;
            case "OrangeUp1":
                UPGetOrange += times;
                break;
            case "OrangeUp2":
                UPGetOrange += times;
                break;
            case "OrangeUp3":
                UPGetOrange += times;
                break;
            case "OrangeUp4":
                UPGetOrange += times;
                break; ;
            case "WeaponUp1":
                UPMakeSpeedWeapon += times;
                break;
            case "WeaponUp2":
                UPMakeSpeedWeapon += times;
                break;
            case "WeaponUp3":
                UPMakeSpeedWeapon += times;
                break;
            case "WeaponUp4":
                UPMakeSpeedWeapon += times;
                break;
            case "WeaponUp5":
                UPMakeSpeedWeapon += times;
                break;
            case "WeaponUp6":
                UPMakeSpeedWeapon += times;
                break;
            case "UpParryRange1":
                UPMakeSpeedWeapon += times;
                break;
            case "UpParryRange2":
                UPMakeSpeedWeapon += times;
                break;
            case "AttackSpeedUp1":
                UPAttackSpeed += times;
                break;
            case "AttackSpeedUp2":
                UPAttackSpeed += times;
                break;
            case "AttackSpeedUp3":
                UPAttackSpeed += times;
                break;
            case "AttackSpeedUp4":
                UPAttackSpeed += times;
                break;
            case "CoinUp1":
                UPMoreSell += times;
                break;
            case "CoinUp2":
                UPMoreSell += times;
                break;
            case "CoinUp3":
                UPMoreSell += times;
                break;
            case "CoinUp4":
                UPMoreSell += times;
                break;
            case "CoinUp5":
                UPMoreSell += times;
                break;
            case "CoinUp6":
                UPMoreSell += times;
                break;
            case "CoinUp7":
                UPMoreSell += times;
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
        => Mathf.RoundToInt(basePrice * (1f + UPMoreSell));
}