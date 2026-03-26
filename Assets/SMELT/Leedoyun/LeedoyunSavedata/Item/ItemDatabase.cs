using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 ItemData 에셋을 등록해두고 ID로 조회하는 데이터베이스.
/// 
/// [설정 방법]
///   1. 빈 GameObject 생성 후 이 스크립트 부착
///   2. Inspector의 items 리스트에 ItemData 에셋을 드래그로 등록
/// 
/// [사용 예시]
///   ItemData data = ItemDatabase.Instance.Get("fruitstone_strawberry");
///   icon.sprite   = data.icon;
///   nameText.text = data.itemName;
/// </summary>
public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    [SerializeField] private List<ItemData> items;  // Inspector에서 에셋 등록

    private Dictionary<string, ItemData> _dict;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 리스트 → 딕셔너리 (빠른 조회용)
        _dict = new Dictionary<string, ItemData>();
        foreach (var item in items)
        {
            if (item == null) continue;
            if (_dict.ContainsKey(item.itemId))
            {
                Debug.LogWarning($"[ItemDatabase] 중복 ID 발견: {item.itemId}");
                continue;
            }
            _dict[item.itemId] = item;
        }
    }

    /// <summary>ID로 아이템 데이터 조회. 없으면 null 반환.</summary>
    public ItemData Get(string itemId)
    {
        if (_dict.TryGetValue(itemId, out var data)) return data;
        Debug.LogWarning($"[ItemDatabase] 아이템 없음: {itemId}");
        return null;
    }

    /// <summary>해당 ID의 아이템이 등록되어 있는지 확인.</summary>
    public bool Contains(string itemId) => _dict.ContainsKey(itemId);
}
