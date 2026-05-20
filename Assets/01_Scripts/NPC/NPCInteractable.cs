using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace _01_Scripts.NPC
{
    [RequireComponent(typeof(NPCMovement))]
    public class NPCInteractable : MonoBehaviour
    {
        [Header("설정")]
        [SerializeField] private float _interactRadius = 2f;

        [Header("폰트 (비워두면 에디터에서 자동 로드)")]
        [SerializeField] private TMP_FontAsset _font;
        private const string FONT_PATH = "Assets/SMELT/Suxghui/Galmuri9 SDF.asset";

        // 말풍선 위치: NPC 우측
        private static readonly Vector3 BUBBLE_OFFSET = new Vector3(0.8f, 0.2f, 0f);  // NPC 우측

        private NPCMovement _movement;
        private Transform   _playerTransform;
        private bool        _isInRange;
        private int         _lastIndex = -1;
        private bool        _spriteLoaded;

        private Canvas        _sharedCanvas;
        private RectTransform _bubbleRoot;
        private Image         _weaponIcon;
        private GameObject    _promptGO;

        // ─────────────────────────────────────────
        // 초기화
        // ─────────────────────────────────────────
        private void Start()
        {
#if UNITY_EDITOR
            if (_font == null)
                _font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FONT_PATH);
#endif
            _movement = GetComponent<NPCMovement>();
            TryFindPlayer();
            BuildBubbleUI();
        }

        // ─────────────────────────────────────────
        // 업데이트
        // ─────────────────────────────────────────
        private void Update()
        {
            if (_playerTransform == null) TryFindPlayer();

            // 인덱스 변경되면 이미지 재시도
            if (_movement.Index != _lastIndex)
            {
                _lastIndex    = _movement.Index;
                _spriteLoaded = false;
            }

            // 스프라이트 로드 성공할 때까지 매 프레임 시도
            if (!_spriteLoaded)
                UpdateWeaponDisplay();

            // 플레이어 거리 체크
            bool inRange = _playerTransform != null &&
                           Vector2.Distance(transform.position, _playerTransform.position) <= _interactRadius;

            if (inRange != _isInRange)
                _isInRange = inRange;

            if (_promptGO != null)
                _promptGO.SetActive(_isInRange && CanSell());

            if (_isInRange && Input.GetKeyDown(KeyCode.E))
                Interact();
        }

        // ─────────────────────────────────────────
        // 플레이어 탐색
        // ─────────────────────────────────────────
        private bool CanSell()
        {
            int slotIdx = _movement.Index - 1;
            if (slotIdx < 0) return false;
            var sm = Leedoyun_SellManager.Instance;
            if (sm == null) return false;
            var orders = sm.ActiveOrders;
            if (orders == null || slotIdx >= orders.Count) return false;
            return InventoryManager.Instance != null &&
                   InventoryManager.Instance.HasItem(orders[slotIdx].requestedWeaponId);
        }

        private void TryFindPlayer()
        {
            var byTag = GameObject.FindGameObjectWithTag("Player");
            if (byTag != null) { _playerTransform = byTag.transform; return; }

            var pp = FindFirstObjectByType<PrototypePlayer>();
            if (pp != null) { _playerTransform = pp.transform; return; }

            var pm = FindFirstObjectByType<PlayerMovement>();
            if (pm != null) _playerTransform = pm.transform;
        }

        // ─────────────────────────────────────────
        // 납품
        // ─────────────────────────────────────────
        private void Interact()
        {
            int slotIdx = _movement.Index - 1;
            if (slotIdx < 0) return;

            var sm = Leedoyun_SellManager.Instance;
            if (sm == null) return;

            var orders = sm.ActiveOrders;
            if (slotIdx >= orders.Count) return;

            var order = orders[slotIdx];
            if (!order.IsActive) return;

            var inv = InventoryManager.Instance;
            if (inv == null || !inv.HasItem(order.requestedWeaponId)) return;

            sm.FulfillOrder(order.orderId, order.requestedWeaponId);

            var held = HeldItemController.Instance;
            if (held != null && held.IsHolding)
                held.ClearHeldItem();
        }

        // ─────────────────────────────────────────
        // 무기 이미지 갱신
        // ─────────────────────────────────────────
        private void UpdateWeaponDisplay()
        {
            if (_weaponIcon == null) return;

            int slotIdx = _movement.Index - 1;
            if (slotIdx < 0)
            {
                if (_bubbleRoot != null) _bubbleRoot.gameObject.SetActive(false);
                _spriteLoaded = true;
                return;
            }

            var sm = Leedoyun_SellManager.Instance;
            if (sm == null || slotIdx >= sm.ActiveOrders.Count)
            {
                if (_bubbleRoot != null) _bubbleRoot.gameObject.SetActive(false);
                return;
            }

            if (_bubbleRoot != null) _bubbleRoot.gameObject.SetActive(true);

            // 플레이어가 무기를 들고 있으면 말풍선 이미지 숨김
            bool holding = HeldItemController.Instance != null && HeldItemController.Instance.IsHolding;
            _weaponIcon.enabled = !holding;
            if (holding) { _spriteLoaded = false; return; }

            var craft = WeaponCraftUI.Instance;
            Sprite spr = craft != null
                ? craft.GetWeaponSprite(sm.ActiveOrders[slotIdx].requestedWeaponId)
                : null;

            _weaponIcon.sprite = spr;
            _weaponIcon.color  = spr != null ? Color.white : new Color(0.6f, 0.6f, 0.6f, 0.5f);

            if (spr != null) _spriteLoaded = true;
        }

        // ─────────────────────────────────────────
        // 말풍선 UI 빌드 (NPC 우측, 꼬리 왼쪽)
        // ─────────────────────────────────────────
        private void OnDestroy()
        {
            if (_bubbleRoot != null)
                Destroy(_bubbleRoot.gameObject);
        }

        private void LateUpdate()
        {
            if (_bubbleRoot == null || _sharedCanvas == null || Camera.main == null) return;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + BUBBLE_OFFSET);
            if (screenPos.z < 0) return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _sharedCanvas.GetComponent<RectTransform>(),
                new Vector2(screenPos.x, screenPos.y),
                null,
                out Vector2 local);
            _bubbleRoot.anchoredPosition = local;
        }

        private void BuildBubbleUI()
        {
            // 기존 ScreenSpaceOverlay 캔버스를 공유해서 사용
            _sharedCanvas = FindFirstObjectByType<Canvas>();
            if (_sharedCanvas == null)
            {
                var cGO = new GameObject("Canvas");
                _sharedCanvas = cGO.AddComponent<Canvas>();
                _sharedCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                cGO.AddComponent<CanvasScaler>();
                cGO.AddComponent<GraphicRaycaster>();
            }

            // 버블 컨테이너 — 공유 캔버스의 자식으로 생성
            var containerGO = new GameObject("NPCBubble");
            containerGO.transform.SetParent(_sharedCanvas.transform, false);
            _bubbleRoot = containerGO.AddComponent<RectTransform>();
            _bubbleRoot.anchorMin = _bubbleRoot.anchorMax = new Vector2(0.5f, 0.5f);
            _bubbleRoot.pivot     = new Vector2(0.5f, 0.5f);
            _bubbleRoot.sizeDelta = new Vector2(130f, 160f);

            // 정렬 순서를 OrderHUD(10)보다 높게
            var subCanvas = containerGO.AddComponent<Canvas>();
            subCanvas.overrideSorting = true;
            subCanvas.sortingOrder    = 20;

            var root = containerGO.transform;

            // ── 말풍선 배경 (우측에 위치) ──────────────
            const float bubbleW = 100f;
            const float bubbleH = 100f;
            const float bubbleX = 15f;   // 캔버스 중심 기준 살짝 오른쪽
            const float bubbleY = 25f;

            var bubbleGO = Rect(root, "Bubble",
                new Vector2(bubbleX, bubbleY), new Vector2(bubbleW, bubbleH));
            bubbleGO.AddComponent<Image>().color = new Color(1f, 1f, 0.92f, 0.97f);

            // ── 꼬리 (말풍선 왼쪽, NPC 방향) ──────────
            var tailGO = Rect(root, "BubbleTail",
                new Vector2(bubbleX - bubbleW * 0.5f - 5f, bubbleY - 10f),
                new Vector2(18f, 18f));
            tailGO.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            tailGO.AddComponent<Image>().color = new Color(1f, 1f, 0.92f, 0.97f);

            // ── 무기 아이콘 ─────────────────────────────
            var iconGO = Rect(bubbleGO.transform, "WeaponIcon",
                Vector2.zero, new Vector2(82f, 82f));
            _weaponIcon = iconGO.AddComponent<Image>();
            _weaponIcon.preserveAspect = true;
            _weaponIcon.color = new Color(0.6f, 0.6f, 0.6f, 0.5f);

            // ── [E] 프롬프트 (말풍선 아래) ─────────────
            var promptGO = Rect(root, "EPrompt",
                new Vector2(bubbleX, bubbleY - bubbleH * 0.5f - 22f),
                new Vector2(bubbleW + 30f, 30f));
            promptGO.AddComponent<Image>().color = new Color(0.05f, 0.05f, 0.05f, 0.85f);

            var txtGO = Rect(promptGO.transform, "Text",
                Vector2.zero, new Vector2(bubbleW + 26f, 26f));
            var tmp = txtGO.AddComponent<TextMeshProUGUI>();
            if (_font != null) tmp.font = _font;
            tmp.text           = "[E] 무기 판매하기";
            tmp.fontSize       = 11f;
            tmp.fontStyle      = FontStyles.Bold;
            tmp.alignment      = TextAlignmentOptions.Center;
            tmp.color          = new Color(1f, 0.95f, 0.3f);
            tmp.raycastTarget  = false;

            _promptGO = promptGO;
            _promptGO.SetActive(false);
        }

        // ─────────────────────────────────────────
        // 헬퍼
        // ─────────────────────────────────────────
        private static GameObject Rect(Transform parent, string name, Vector2 pos, Vector2 size)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot            = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos;
            rt.sizeDelta        = size;
            return go;
        }
    }
}
