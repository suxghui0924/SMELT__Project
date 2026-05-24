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
        private static readonly Vector3 BUBBLE_OFFSET = new Vector3(0.9f, 0.2f, 0f);  // NPC 우측

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

            bool showPrompt = _isInRange && CanSell();
            if (_promptGO != null)
            {
                _promptGO.SetActive(showPrompt);
                // 프롬프트가 보일 때 이 NPC의 컨테이너를 최상위 형제로 올려
                // 다른 NPC의 말풍선에 가리지 않도록 함
                if (showPrompt && _bubbleRoot != null)
                    _bubbleRoot.transform.SetAsLastSibling();
            }

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

            int goldEarned = sm.FulfillOrder(order.orderId, order.requestedWeaponId);
            if (goldEarned >= 0)
                GoldPopup.Show(transform.position + Vector3.up * 0.5f, goldEarned);

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
                return;  // 주문 수 불일치는 일시적 상태 — 버블 숨기지 않고 재시도 대기

            if (_bubbleRoot != null) _bubbleRoot.gameObject.SetActive(true);

            // 플레이어가 무기를 들고 있으면 말풍선 이미지 숨김
            bool holding = HeldItemController.Instance != null && HeldItemController.Instance.IsHolding;
            _weaponIcon.enabled = !holding;
            if (holding) { _spriteLoaded = false; return; }

            var craft = WeaponCraftUI.Instance;
            Sprite spr = craft != null
                ? craft.GetWeaponSprite(sm.ActiveOrders[slotIdx].requestedWeaponId)
                : null;

            if (spr != null)
            {
                _weaponIcon.sprite  = spr;
                _weaponIcon.color   = Color.white;
                _weaponIcon.enabled = true;
                _spriteLoaded = true;
            }
            else
            {
                // 스프라이트 아직 로드 안 됨 — 아이콘 숨기고 다음 프레임 재시도
                _weaponIcon.enabled = false;
                _spriteLoaded = false;
            }
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

            // Canvas 컴포넌트를 붙이지 않음 — 붙이면 FindFirstObjectByType<Canvas>()로
            // 다른 NPC가 이 컨테이너를 캔버스로 잡아 자식이 되고, NPC 파괴 시 같이 파괴됨.
            // 대신 최상위 형제로 배치해 OrderHUD 위에 렌더링되게 함.
            containerGO.transform.SetAsLastSibling();

            var root = containerGO.transform;

            // ── 말풍선 배경 (우측에 위치) ──────────────
            const float bubbleW = 100f;
            const float bubbleH = 100f;
            const float bubbleX = 15f;   // 캔버스 중심 기준 살짝 오른쪽
            const float bubbleY = 25f;

            // 꼬리를 먼저 생성 → 배경이 위에 렌더링되어 꼬리 안쪽을 덮음
            var tailGO = Rect(root, "BubbleTail",
                new Vector2(bubbleX - bubbleW * 0.5f + 6f, bubbleY),
                new Vector2(18f, 18f));
            tailGO.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            tailGO.AddComponent<Image>().color = new Color(1f, 1f, 0.92f, 0.97f);

            var bubbleGO = Rect(root, "Bubble",
                new Vector2(bubbleX, bubbleY), new Vector2(bubbleW, bubbleH));
            bubbleGO.AddComponent<Image>().color = new Color(1f, 1f, 0.92f, 0.97f);

            // ── 무기 아이콘 ─────────────────────────────
            var iconGO = Rect(bubbleGO.transform, "WeaponIcon",
                Vector2.zero, new Vector2(82f, 82f));
            iconGO.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0f);

            var iconSpriteGO = new GameObject("Sprite");
            iconSpriteGO.transform.SetParent(iconGO.transform, false);
            var sprRt = iconSpriteGO.AddComponent<RectTransform>();
            sprRt.anchorMin = Vector2.zero;
            sprRt.anchorMax = Vector2.one;
            sprRt.offsetMin = new Vector2(4f, 4f);
            sprRt.offsetMax = new Vector2(-4f, -4f);
            _weaponIcon = iconSpriteGO.AddComponent<Image>();
            _weaponIcon.preserveAspect = true;
            _weaponIcon.color = new Color(0f, 0f, 0f, 0f);

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
