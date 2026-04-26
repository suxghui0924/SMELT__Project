using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 무기 제작 관리 싱글톤.
/// 광석(재료) 소모 → 무기 인벤토리 추가.
/// 무기는 인벤토리에서 관리되므로 ISaveable 불필요.
///
/// [인벤토리 무기 ID 형식]
///   "weapon_{weaponType}_{mainOreName}"
///   ex) 사과석으로 만든 검 → "weapon_sword_apple"
///       멜론석으로 만든 도끼 → "weapon_axe_melon"
///
/// 담당자: 이도윤
/// </summary>
public class WeaponCraftManager : MonoBehaviour
{
    public static WeaponCraftManager Instance { get; private set; }

    // ─────────────────────────────────────────
    // 무기 레시피 (static: ShopManager 가격 계산에서도 참조)
    // ─────────────────────────────────────────

    /// <summary>무기 1종의 레시피 정보</summary>
    public class WeaponRecipe
    {
        public int basePrice;   // 무기 기본금 (가격 계산 기준)
        public int mainCount;   // 메인 광석 소모 개수
        public int subCount;    // 서브 광석 소모 개수

        public WeaponRecipe(int basePrice, int mainCount, int subCount)
        {
            this.basePrice = basePrice;
            this.mainCount = mainCount;
            this.subCount  = subCount;
        }
    }

    /// <summary>
    /// 무기별 레시피 테이블.
    /// (외부 접근 가능 - ShopManager 가격 계산용)
    /// </summary>
    public static readonly Dictionary<WeaponType, WeaponRecipe> Recipes =
        new Dictionary<WeaponType, WeaponRecipe>
        {
            { WeaponType.Sword,    new WeaponRecipe(basePrice: 500,  mainCount: 1, subCount: 1) }, // 검:    메인×1 + 서브×1
            { WeaponType.Axe,      new WeaponRecipe(basePrice: 800,  mainCount: 1, subCount: 2) }, // 도끼:  메인×1 + 서브×2
            { WeaponType.Spear,    new WeaponRecipe(basePrice: 1000, mainCount: 2, subCount: 2) }, // 창:    메인×2 + 서브×2
            { WeaponType.Hammer,   new WeaponRecipe(basePrice: 1200, mainCount: 2, subCount: 3) }, // 망치: 메인×2 + 서브×3
            { WeaponType.Gauntlet, new WeaponRecipe(basePrice: 1500, mainCount: 3, subCount: 2) }, // 건틀릿: 메인×3 + 서브×2
        };

    /// <summary>
    /// 광석별 가치 테이블.
    /// (외부 접근 가능 - ShopManager 가격 계산용)
    /// </summary>
    public static readonly Dictionary<string, int> OreValues =
        new Dictionary<string, int>
        {
            { "fruitstone_apple",  1000 }, // 사과석
            { "fruitstone_melon",  2000 }, // 멜론석
            { "fruitstone_orange", 3000 }, // 귤석
            { "fruitstone_lemon",  4000 }, // 레몬석
            { "fruitstone_grape",  5000 }, // 포도석
        };

    // ─────────────────────────────────────────
    // 초기화
    // ─────────────────────────────────────────
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ─────────────────────────────────────────
    // 무기 제작
    // ─────────────────────────────────────────

    /// <summary>
    /// 무기 제작 처리.
    /// 인벤토리에서 광석을 소모하고 제작된 무기를 추가.
    ///
    /// ex) CraftWeapon("fruitstone_apple", "fruitstone_melon", WeaponType.Sword)
    ///     → fruitstone_apple ×1, fruitstone_melon ×1 소모
    ///     → "weapon_sword_apple" ×1 추가
    /// </summary>
    /// <param name="mainOreId">메인 광석 ID (가격 계산 기준)</param>
    /// <param name="subOreId">서브 광석 ID (소모만, 가격 무관)</param>
    /// <param name="weaponType">제작할 무기 종류</param>
    /// <returns>제작 성공 여부 (재료 부족 시 false)</returns>
    public bool CraftWeapon(string mainOreId, string subOreId, WeaponType weaponType)
    {
        // 레시피 유효성 검사
        if (!Recipes.TryGetValue(weaponType, out WeaponRecipe recipe))
        {
            Debug.LogWarning($"[WeaponCraft] 알 수 없는 무기 타입: {weaponType}");
            return false;
        }

        // 광석 ID 유효성 검사
        if (!OreValues.ContainsKey(mainOreId))
        {
            Debug.LogWarning($"[WeaponCraft] 알 수 없는 메인 광석: {mainOreId}");
            return false;
        }
        if (!OreValues.ContainsKey(subOreId))
        {
            Debug.LogWarning($"[WeaponCraft] 알 수 없는 서브 광석: {subOreId}");
            return false;
        }

        // 보유 수량 확인 (같은 광석을 메인+서브로 쓸 경우 합산 체크)
        int mainNeed = recipe.mainCount;
        int subNeed  = recipe.subCount;

        if (mainOreId == subOreId)
        {
            // 같은 광석이면 합산 소모량으로 한 번에 확인
            int totalNeed = mainNeed + subNeed;
            if (!InventoryManager.Instance.HasItem(mainOreId, totalNeed))
            {
                Debug.LogWarning($"[WeaponCraft] 광석 부족 (같은 종류): {mainOreId} " +
                                 $"(필요: {totalNeed}, 보유: {InventoryManager.Instance.GetQuantity(mainOreId)})");
                return false;
            }
        }
        else
        {
            // 메인/서브 각각 확인
            if (!InventoryManager.Instance.HasItem(mainOreId, mainNeed))
            {
                Debug.LogWarning($"[WeaponCraft] 메인 광석 부족: {mainOreId} " +
                                 $"(필요: {mainNeed}, 보유: {InventoryManager.Instance.GetQuantity(mainOreId)})");
                return false;
            }
            if (!InventoryManager.Instance.HasItem(subOreId, subNeed))
            {
                Debug.LogWarning($"[WeaponCraft] 서브 광석 부족: {subOreId} " +
                                 $"(필요: {subNeed}, 보유: {InventoryManager.Instance.GetQuantity(subOreId)})");
                return false;
            }
        }

        // 재료 소모
        InventoryManager.Instance.RemoveItem(mainOreId, mainNeed);
        InventoryManager.Instance.RemoveItem(subOreId,  subNeed);

        // 무기를 인벤토리에 추가
        // ID 형식: "weapon_{type}_{mainOreName}"
        string oreName     = mainOreId.Replace("fruitstone_", ""); // ex) "apple"
        string weaponItemId = $"weapon_{GetWeaponTypeId(weaponType)}_{oreName}"; // ex) "weapon_sword_apple"
        InventoryManager.Instance.AddItem(weaponItemId, 1);

        Debug.Log($"[WeaponCraft] 제작 완료: {weaponItemId} " +
                  $"(재료: {mainOreId}×{mainNeed} + {subOreId}×{subNeed})");
        return true;
    }

    /// <summary>
    /// 제작 가능 여부만 확인 (UI 버튼 활성화 등, 실제 소모 없음).
    /// </summary>
    public bool CanCraft(string mainOreId, string subOreId, WeaponType weaponType)
    {
        if (!Recipes.TryGetValue(weaponType, out WeaponRecipe recipe)) return false;
        if (!OreValues.ContainsKey(mainOreId) || !OreValues.ContainsKey(subOreId)) return false;

        if (mainOreId == subOreId)
            return InventoryManager.Instance.HasItem(mainOreId, recipe.mainCount + recipe.subCount);

        return InventoryManager.Instance.HasItem(mainOreId, recipe.mainCount) &&
               InventoryManager.Instance.HasItem(subOreId,  recipe.subCount);
    }

    // ─────────────────────────────────────────
    // 유틸리티 (ShopManager 파싱에서도 사용)
    // ─────────────────────────────────────────

    /// <summary>WeaponType → 인벤토리 ID용 문자열 변환.</summary>
    public static string GetWeaponTypeId(WeaponType type)
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

    /// <summary>
    /// 인벤토리 무기 ID → WeaponType + mainOreId 역파싱.
    /// ex) "weapon_sword_apple" → WeaponType.Sword, "fruitstone_apple"
    /// </summary>
    public static bool TryParseWeaponItemId(string weaponItemId,
        out WeaponType weaponType, out string mainOreId)
    {
        weaponType = WeaponType.Sword;
        mainOreId  = string.Empty;

        // 형식: "weapon_{type}_{mainOreName}" → Split 결과 최소 3개
        string[] parts = weaponItemId.Split('_');
        if (parts.Length < 3 || parts[0] != "weapon") return false;

        // 무기 타입 파싱
        switch (parts[1])
        {
            case "sword":    weaponType = WeaponType.Sword;    break;
            case "axe":      weaponType = WeaponType.Axe;      break;
            case "spear":    weaponType = WeaponType.Spear;    break;
            case "hammer":   weaponType = WeaponType.Hammer;   break;
            case "gauntlet": weaponType = WeaponType.Gauntlet; break;
            default: return false;
        }

        // 메인 광석 ID 복원
        mainOreId = "fruitstone_" + parts[2]; // ex) "fruitstone_apple"
        return OreValues.ContainsKey(mainOreId);
    }
}

// ─────────────────────────────────────────
// 무기 종류 열거형 (프로젝트 공용)
// ─────────────────────────────────────────
/// <summary>제작 가능한 무기 종류</summary>
public enum WeaponType
{
    Sword,    // 검    (기본금 500)
    Axe,      // 도끼  (기본금 800)
    Spear,    // 창    (기본금 1000)
    Hammer,   // 망치   (기본금 1200)
    Gauntlet  // 건틀릿 (기본금 1500)
}
