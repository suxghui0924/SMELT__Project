using TMPro;
using UnityEngine;

/// <summary>
/// 코드 생성 UI용 폰트 자동 로더.
/// 에디터에서 AssetDatabase로 찾아 캐시합니다.
/// </summary>
public static class FontLoader
{
    private static TMP_FontAsset _galmuri9;
    private static TMP_FontAsset _gmarket;

    /// <summary>Galmuri9 SDF — NPC 말풍선, OrderHUD, GoldPopup 등</summary>
    public static TMP_FontAsset Galmuri9 =>
        _galmuri9 != null ? _galmuri9 : (_galmuri9 = Load("Galmuri9 SDF"));

    /// <summary>GmarketSansTTFMedium SDF — PrototypeHUD 등</summary>
    public static TMP_FontAsset GmarketMedium =>
        _gmarket != null ? _gmarket : (_gmarket = Load("GmarketSansTTFMedium SDF"));

    private static TMP_FontAsset Load(string fontName)
    {
#if UNITY_EDITOR
        var guids = UnityEditor.AssetDatabase.FindAssets($"t:TMP_FontAsset {fontName}");
        foreach (var guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            var font = UnityEditor.AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            if (font != null) return font;
        }
#endif
        // 빌드용: Resources 폴더에 폰트를 넣으면 여기서 로드
        return Resources.Load<TMP_FontAsset>($"Fonts/{fontName}");
    }
}
