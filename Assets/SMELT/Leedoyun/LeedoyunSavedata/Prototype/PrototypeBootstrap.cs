using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 프로토타입 씬 자동 빌드.
/// 플레이 버튼만 누르면 자동 실행됩니다. 씬에 아무것도 없어도 됩니다.
///
/// [씬 구성]
///   왼쪽 (갈색)  : 광석 채취 구역 — 머물면 자동 채집
///   중앙 (파란색): 대장간          — E키로 무기 제작
///   오른쪽 (초록): 상점            — E키로 무기 판매
///
/// [이동] WASD 또는 방향키
/// [상호작용] E
/// </summary>
public class PrototypeBootstrap : MonoBehaviour
{
    // 이 Bootstrap이 동작할 씬 이름
    private const string TargetSceneName = "Work_Leedoyun_SaveData";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoStart()
    {
        // Prototype 씬이 아니면 실행하지 않음
        if (SceneManager.GetActiveScene().name != TargetSceneName) return;

        // 이미 씬에 Bootstrap이 있으면 중복 실행 방지
        if (FindAnyObjectByType<PrototypeBootstrap>() != null) return;

        var go = new GameObject("[PrototypeBootstrap]");
        go.AddComponent<PrototypeBootstrap>();
    }

    private void Awake()
    {
        Ensure<SaveManager>();
        Ensure<InventoryManager>();
        Ensure<WeaponCraftManager>();
        Ensure<AutoGatherManager>();
        Ensure<Leedoyun_SellManager>();
        Ensure<OrderHUD>();
    }

    private void Start()
    {
        SetupCamera();

        // HUD 먼저 생성 (Zone / Player가 HUD를 참조하므로)
        new GameObject("HUD").AddComponent<PrototypeHUD>();

        CreateBackground();
        CreateZone("CraftingZone", new Vector3(-6.5f, 5f, 0f), new Vector2(5f, 5f),
            ZoneType.Crafting, new Color(0.38f, 0.26f, 0.14f, 0f));

        // 플레이어: 광석 채취 구역 하단에 시작
        CreatePlayer(new Vector3(-6.5f, -3.5f, -1f));
    }

    // ─────────────────────────────────────────
    // 씬 빌드
    // ─────────────────────────────────────────
    private static void SetupCamera()
    {
        var cam = Camera.main;
        if (cam == null) return;
        cam.orthographic      = true;
        cam.orthographicSize  = 6.5f;
        cam.backgroundColor  = new Color(0.08f, 0.08f, 0.12f);
        cam.clearFlags       = CameraClearFlags.SolidColor;
        cam.transform.position = new Vector3(0f, 1f, -10f);
    }

    private static void CreateBackground()
    {
        MakeSprite("Background", new Vector3(0f, 0f, 1f), new Vector2(22f, 14f),
            new Color(0.10f, 0.10f, 0.16f));
    }

    private static void CreateZone(string name, Vector3 pos, Vector2 size, ZoneType type, Color color)
    {
        var go  = MakeSprite(name, pos, size, color);
        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size      = Vector2.one; // world size = localScale × size
        var zone      = go.AddComponent<PrototypeZone>();
        zone.ZoneType = type;
    }

    private static void CreatePlayer(Vector3 pos)
    {
        var go  = MakeSprite("Player", pos, new Vector2(0.8f, 1.0f), new Color(0.95f, 0.65f, 0.20f));
        var rb  = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints  = RigidbodyConstraints2D.FreezeRotation;
        var col = go.AddComponent<BoxCollider2D>();
        col.size = Vector2.one;
        go.AddComponent<PrototypePlayer>();
    }

    // ─────────────────────────────────────────
    // 유틸
    // ─────────────────────────────────────────
    private static GameObject MakeSprite(string name, Vector3 pos, Vector2 size, Color color)
    {
        var go = new GameObject(name);
        go.transform.position   = pos;
        go.transform.localScale = new Vector3(size.x, size.y, 1f);
        go.AddComponent<SpriteRenderer>().sprite = MakeSolidSprite(color);
        return go;
    }

    private static Sprite MakeSolidSprite(Color color)
    {
        var tex = new Texture2D(2, 2) { filterMode = FilterMode.Point };
        tex.SetPixels(new[] { color, color, color, color });
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f), 2f);
    }

    private static void Ensure<T>() where T : MonoBehaviour
    {
        if (FindAnyObjectByType<T>() == null)
            new GameObject($"[{typeof(T).Name}]").AddComponent<T>();
    }
}
