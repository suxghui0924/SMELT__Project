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

    [Header("캐릭터 이미지")]
    [Tooltip("플레이어 스프라이트. 비워두면 기본 주황 사각형 사용.")]
    [SerializeField] private Sprite _playerSprite;
    [Tooltip("스프라이트 크기 (단위: 유닛). 기본값 (0.8, 1.0)")]
    [SerializeField] private Vector2 _playerSize = new Vector2(0.8f, 1.0f);

    [Header("캐릭터 애니메이션")]
    [Tooltip("Animator Controller. 비워두면 애니메이션 없이 동작.")]
    [SerializeField] private RuntimeAnimatorController _animatorController;

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
        Ensure<PlayerStatManager>();
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
        CreatePlayer(new Vector3(-6.5f, -3.5f, -1f), _playerSprite, _playerSize, _animatorController);
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
        cam.transform.position = new Vector3(0f, 2f, -10f);
    }

    private static void CreateBackground()
    {
        var cam = Camera.main;
        float h = cam != null ? cam.orthographicSize * 2f : 14f;
        float w = cam != null ? h * cam.aspect : 17f;
        var center = cam != null
            ? new Vector3(cam.transform.position.x, cam.transform.position.y, 1f)
            : new Vector3(0f, 0f, 1f);
        MakeSprite("Background", center, new Vector2(w, h),
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

    private static void CreatePlayer(Vector3 pos, Sprite sprite, Vector2 size, RuntimeAnimatorController animCtrl)
    {
        GameObject go;
        if (sprite != null)
        {
            go = new GameObject("Player");
            go.transform.position   = pos;
            go.transform.localScale = new Vector3(size.x, size.y, 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite           = sprite;
            sr.sortingLayerName = "Default";
        }
        else
        {
            go = MakeSprite("Player", pos, size, new Color(0.95f, 0.65f, 0.20f));
            go.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        }

        if (animCtrl != null)
        {
            var anim = go.AddComponent<Animator>();
            anim.runtimeAnimatorController = animCtrl;
        }

        var rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints  = RigidbodyConstraints2D.FreezeRotation;
        var col = go.AddComponent<BoxCollider2D>();
        col.size   = new Vector2(1f, 0.25f);
        col.offset = new Vector2(0f, -0.375f);
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
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite           = MakeSolidSprite(color);
        sr.sortingLayerName = "Background";
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
