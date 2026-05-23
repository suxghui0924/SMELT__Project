using System;
using UnityEngine;

/// <summary>
/// 손님 1명의 의뢰(주문) 데이터.
/// Leedoyun_SellManager가 생성하고 관리함.
/// 담당자: 이도윤
/// </summary>
[Serializable]
public class Leedoyun_CustomerOrder
{
    // ─────────────────────────────────────────
    // 주문 식별
    // ─────────────────────────────────────────
    /// <summary>주문 고유 ID (GUID 기반)</summary>
    public string orderId;

    // ─────────────────────────────────────────
    // 요청 내용 (인벤토리 ID로 관리)
    // ─────────────────────────────────────────
    /// <summary>
    /// 요청 무기 인벤토리 ID.
    /// 형식: "weapon_{type}_{mainOre}"
    /// ex) "weapon_sword_apple", "weapon_gauntlet_grape"
    /// </summary>
    public string requestedWeaponId;

    /// <summary>요청 무기 타입 (표시용)</summary>
    public WeaponType weaponType;

    /// <summary>요청 메인 광석 ID (표시용)</summary>
    public string mainOreId;

    // ─────────────────────────────────────────
    // 보상 / 시간
    // ─────────────────────────────────────────
    /// <summary>납품 성공 시 지급 골드</summary>
    public ulong rewardGold;

    /// <summary>주문 제한 시간 (초)</summary>
    public float timeLimit;

    /// <summary>현재까지 경과 시간 (초)</summary>
    public float elapsedTime;

    // ─────────────────────────────────────────
    // 상태
    // ─────────────────────────────────────────
    /// <summary>납품 완료 여부</summary>
    public bool isFulfilled;

    /// <summary>시간 초과 여부</summary>
    public bool isExpired;

    /// <summary>남은 시간 (초). 0 이하면 만료.</summary>
    public float RemainingTime => Mathf.Max(0f, timeLimit - elapsedTime);

    /// <summary>남은 시간 비율 (0~1). UI 타이머 바에 사용.</summary>
    public float RemainingRatio => timeLimit > 0f ? RemainingTime / timeLimit : 0f;

    /// <summary>주문이 아직 활성 상태인지 (납품도 만료도 안 된 상태)</summary>
    public bool IsActive => !isFulfilled && !isExpired;

    // ─────────────────────────────────────────
    // 생성자
    // ─────────────────────────────────────────
    public Leedoyun_CustomerOrder(WeaponType weaponType, string mainOreId,
        int rewardGold, float timeLimit)
    {
        this.orderId     = Guid.NewGuid().ToString("N").Substring(0, 8); // 짧은 ID
        this.weaponType  = weaponType;
        this.mainOreId   = mainOreId;
        this.rewardGold  = (ulong)rewardGold;
        this.timeLimit   = timeLimit;
        this.elapsedTime = 0f;
        this.isFulfilled = false;
        this.isExpired   = false;

        // 인벤토리 조회용 복합 ID 조합
        string oreShortName      = mainOreId.Replace("fruitstone_", ""); // "apple"
        string weaponTypeId      = GetWeaponTypeId(weaponType);           // "sword"
        this.requestedWeaponId   = $"weapon_{weaponTypeId}_{oreShortName}"; // "weapon_sword_apple"
    }

    // ─────────────────────────────────────────
    // 내부 유틸
    // ─────────────────────────────────────────
    private static string GetWeaponTypeId(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.Sword:    return "sword";
            case WeaponType.Axe:      return "axe";
            case WeaponType.Spear:    return "spear";
            case WeaponType.Hammer:   return "hammer";
            case WeaponType.Gauntlet: return "gauntlet";
            default:                  return type.ToString().ToLower();
        }
    }
}
