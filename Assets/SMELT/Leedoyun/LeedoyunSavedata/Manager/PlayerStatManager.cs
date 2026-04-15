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
        data.getFriuts = UPGetFruits;
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
            case "AllUp1":
                UPGetFruits += 0.2f;
                break;
            case "AllUp2":
                UPGetFruits += 0.2f;
                break;
            case "AllUp3":
                UPGetFruits += 0.2f;
                break;
            case "AllUp4":
                UPGetFruits += 0.2f;
                break;
            case "AllUp5":
                UPGetFruits += 0.2f;
                break;
            case "AppleUp1":
                UPGetApple += 0.2f;
                break;
            case "AppleUp2":
                UPGetApple += 0.2f;
                break;
            case "AppleUp3":
                UPGetApple += 0.2f;
                break;
            case "AppleUp4":
                UPGetApple += 0.2f;
                break; ;
            case "LemonUp1":
                UPGetLemon += 0.2f;
                break;
            case "LemonUp2":
                UPGetLemon += 0.2f;
                break;
            case "LemonUp3":
                UPGetLemon += 0.2f;
                break;
            case "LemonUp4":
                UPGetLemon += 0.2f;
                break; ;
            case "MelonUp1":
                UPGetMelon += 0.2f;
                break;
            case "MelonUp2":
                UPGetMelon += 0.2f;
                break;
            case "MelonUp3":
                UPGetMelon += 0.2f;
                break;
            case "MelonUp4":
                UPGetMelon += 0.2f;
                break; ;
            case "GrapeUp1":
                UPGetGrape += 0.2f;
                break;
            case "GrapeUp2":
                UPGetGrape += 0.2f;
                break;
            case "GrapeUp3":
                UPGetGrape += 0.2f;
                break;
            case "GrapeUp4":
                UPGetGrape += 0.2f;
                break; ;
            case "OrangeUp1":
                UPGetOrange += 0.2f;
                break;
            case "OrangeUp2":
                UPGetOrange += 0.2f;
                break;
            case "OrangeUp3":
                UPGetOrange += 0.2f;
                break;
            case "OrangeUp4":
                UPGetOrange += 0.2f;
                break; ;
            case "WeaponUp1":
                UPMakeSpeedWeapon += 0.2f;
                break;
            case "WeaponUp2":
                UPMakeSpeedWeapon += 0.2f;
                break;
            case "WeaponUp3":
                UPMakeSpeedWeapon += 0.2f;
                break;
            case "WeaponUp4":
                UPMakeSpeedWeapon += 0.2f;
                break;
            case "WeaponUp5":
                UPMakeSpeedWeapon += 0.2f;
                break;
            case "WeaponUp6":
                UPMakeSpeedWeapon += 0.2f;
                break;
            case "WeaponUp7":
                UPMakeSpeedWeapon += 0.2f;
                break;
            case "WeaponUp8":
                UPMakeSpeedWeapon += 0.2f;
                break;
            case "AttackSpeedUp1":
                UPAttackSpeed += 0.2f;
                break;
            case "AttackSpeedUp2":
                UPAttackSpeed += 0.2f;
                break;
            case "AttackSpeedUp3":
                UPAttackSpeed += 0.2f;
                break;
            case "AttackSpeedUp4":
                UPAttackSpeed += 0.2f;
                break;
            case "CoinUp1":
                UPMoreSell += 0.2f;
                break;
            case "CoinUp2":
                UPMoreSell += 0.2f;
                break;
            case "CoinUp3":
                UPMoreSell += 0.2f;
                break;
            case "CoinUp4":
                UPMoreSell += 0.2f;
                break;
            case "CoinUp5":
                UPMoreSell += 0.2f;
                break;
            case "CoinUp6":
                UPMoreSell += 0.2f;
                break;
            case "CoinUp7":
                UPMoreSell += 0.2f;
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