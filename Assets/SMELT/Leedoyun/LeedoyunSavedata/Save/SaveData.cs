using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스멜트(SMELT) 게임의 모든 세이브 데이터를 담는 컨테이너.
/// 팀원이 새 데이터를 추가할 때는 해당 Region에만 필드를 추가하세요.
/// SaveManager.cs는 건드리지 마세요.
/// </summary>
[Serializable]
public class SaveData
{
    // ─────────────────────────────────────────
    // 메타 정보 (건드리지 마세요 - SaveManager가 자동 기록)
    // ─────────────────────────────────────────
    public string version = "1.0.0"; // 버전 불일치 감지용
    public string saveTime = ""; // 저장 시각 (자동 기록)
    public float  playTime = 0f; // 총 플레이 시간(초)

    // ─────────────────────────────────────────
    // 경제 시스템 & 인벤토리 & 테크트리 담당자: 주다림
    // ─────────────────────────────────────────
    [Header("Economy")]
    public int currentDay = 1; // 현재 날짜
    public ulong gold = 0; // 보유 골드
    public ulong maintenanceCost = 6000; // 오늘의 유지비 (일차별 고정 테이블)

    [Header("FruitStones")]  // 추가
    public int fruitStoneApple  = 0; // 사과 과일석 개수  // 추가
    public int fruitStoneMelon  = 0; // 멜론 과일석 개수  // 추가
    public int fruitStoneOrange = 0; // 오렌지 과일석 개수 // 추가
    public int fruitStoneLemon  = 0; // 레몬 과일석 개수  // 추가
    public int fruitStoneGrape  = 0; // 포도 과일석 개수  // 추가

    [Header("Inventory")]
    public List<ItemSaveData> inventory = new List<ItemSaveData>();
    // 저장 예시:
    // { itemId: "fruitstone_strawberry", quantity: 5 }  → 딸기 과일석 5개
    // { itemId: "weapon_sword", quantity: 2 }  → 검 2개
    // { itemId: "juice_strawberry", quantity: 3 }  → 딸기주스 3개

    [Header("Tech Tree")]
    public int currentTechLevel = 1; // 1=무기, 2=주스, 3=+α
    public List<string> unlockedTechs   = new List<string>();
    // 저장 예시: ["tech_weapon", "tech_juice"]

    [Header("Achievement")]
    public List<int> clearedAchievements = new List<int>();

    // ─────────────────────────────────────────
    // 플레이어 스탯 & 업그레이드 담당자: 이윤건
    // ─────────────────────────────────────────
    [Header("Player Stats & Upgrades")]
    public float makeSpeedJuice = 1.0f; // 가공 속도 배율 (0.05 = 5%)
    public float makeSpeedWeapon = 1.0f; // 가공 속도 배율 (0.05 = 5%)
    public float parryRange = 1.0f; // 패링 판정 범위 배율 (0.05 = 5%)
    public float moreSell = 0.0f; // 판매 수익 보너스 (0.05 = 5%)
    public float attackSpeed = 0.0f; //공속 (0.05 = 5%)
    public float getApple = 0.0f; //사과 배수 (0.05 = 5%)
    public float getLemon = 0.0f; //레몬 배수 (0.05 = 5%)
    public float getMelon = 0.0f; //멜론 배수 (0.05 = 5%)
    public float getGrape = 0.0f; //포도 배수 (0.05 = 5%)
    public float getOrange = 0.0f; //귤 배수 (0.05 = 5%)
    public float getFriuts = 0.0f; //전체 과일 배수 (0.05 = 5%)
    public List<string> purchasedUpgrades  = new List<string>();
    // 저장 예시: ["upgrade_parry_range", "upgrade_sales_05"]

    // ─────────────────────────────────────────
    // 판매 시스템 담당자: 이도윤                      // 추가
    // ─────────────────────────────────────────
    [Header("SellSystem")]
    public int leedoyunTodayGold = 0;
    public int leedoyunTotalGold = 0;
    public float orderSpawnTimer = 0f;
    public List<OrderSaveData> activeOrders = new List<OrderSaveData>();

    // ─────────────────────────────────────────
    // 피로도 담당자: 이호승                          // 추가
    // ─────────────────────────────────────────
    [Header("Stamina")]                               // 추가
    public float stamina = 100f; // 현재 피로도 (0~100)  // 추가

    // ─────────────────────────────────────────
    // 곡괭이 시스템 담당자: 이도윤                        // 추가
    // ─────────────────────────────────────────
    [Header("Pickaxe")]                                     // 추가
    public string equippedPickaxeId = "pickaxe_default";    // 현재 장착된 곡괭이 ID // 추가
    public List<string> purchasedPickaxeIds = new List<string>(); // 구매한 곡괭이 ID 목록 // 추가

    // ─────────────────────────────────────────
    // 설정 담당자: 이도윤
    // ─────────────────────────────────────────
    [Header("Settings")]
    public float volumeLevel = 1.0f; // 마스터 볼륨 (0.0 ~ 1.0)

    // ─────────────────────────────────────────
    // 상점 운영 현황 담당자: 박성희
    // ─────────────────────────────────────────
    // ─────────────────────────────────────────
    [Header("Shop")]
    public int totalEarned  = 0;   // 누적 총 수익
    public int todayEarned  = 0;   // 오늘 번 돈
    public List<string> salesHistory = new List<string>();
    // 저장 예시: ["weapon_sword", "juice_grape"] → 오늘 팔린 아이템 목록
}

// ─────────────────────────────────────────
// 아이템 구조 (과일석 / 무기 / 주스 모두 여기서 관리)
//
// 아이템 ID 네이밍 규칙:
//   과일석 원석 → fruitstone_strawberry / fruitstone_grape / fruitstone_lemon
//   제련 무기   → weapon_sword / weapon_dagger
//   착즙 주스   → juice_strawberry / juice_grape / juice_lemon
//   장신구(3단계) → accessory_ring / accessory_necklace
// ─────────────────────────────────────────
[Serializable]
public class OrderSaveData
{
    public string    orderId;
    public string    requestedWeaponId;
    public int       weaponType;
    public string    mainOreId;
    public ulong       rewardGold;
    public float     timeLimit;
    public float     elapsedTime;
}

[Serializable]
public class ItemSaveData
{
    public string itemId;    // 아이템 고유 ID (ItemDatabase 조회 키)
    public int quantity;  // 수량
}
