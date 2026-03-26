using UnityEngine;

/// <summary>
/// 아이템 고유 정보를 담는 ScriptableObject.
/// 저장 안 함 - 에셋으로 관리.
/// 
/// [만드는 법]
///   Project 창 우클릭 → Create → SMELT/ItemData
///   itemId는 SaveData의 아이템 ID 네이밍 규칙을 따르세요.
///
/// [아이템 ID 네이밍 규칙]
///   과일석  → fruitstone_strawberry / fruitstone_grape / fruitstone_lemon
///   무기    → weapon_sword / weapon_dagger
///   주스    → juice_strawberry / juice_grape / juice_lemon
///   장신구  → accessory_ring / accessory_necklace
/// </summary>
[CreateAssetMenu(menuName = "SMELT/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("기본 정보")]
    public string itemId;       // 고유 ID  ex) "fruitstone_strawberry"
    public string itemName;     // 이름     ex) "딸기 과일석"
    public Sprite icon;         // 아이콘
    [TextArea]
    public string description;  // 설명

    [Header("가격")]
    public int buyPrice;        // 구매 가격 (0이면 구매 불가)
    public int sellPrice;       // 판매 가격

    [Header("종류")]
    public ItemType itemType;   // 아이템 종류
}

public enum ItemType
{
    FruitStone,     // 과일석 원석
    Weapon,         // 제련 무기
    Juice,          // 착즙 주스
    Accessory,      // 장신구 (3단계)
}
