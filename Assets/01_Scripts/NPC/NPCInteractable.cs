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
        [SerializeField] private Vector3 _bubbleOffset = new Vector3(0f, 1.6f, 0f);

        [Header("폰트 (비워두면 기본 폰트)")]
        [SerializeField] private TMP_FontAsset _font;
        private const string FONT_PATH = "Assets/SMELT/Suxghui/Galmuri9 SDF.asset";

        private NPCMovement _movement;
        private Transform _playerTransform;
        private bool _isInRange;
        private int _lastIndex = -1;

        private Canvas _worldCanvas;
        private Image _weaponIcon;
        private GameObject _promptGO;

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
            UpdateWeaponDisplay();
        }

        // ─────────────────────────────────────────
        // 업데이트
        // ─────────────────────────────────────────
        private void Update()
        {
            if (_playerTransform == null) TryFindPlayer();

            if (_movement.Index != _lastIndex)
            {
                _lastIndex = _movement.Index;
                UpdateWeaponDisplay();
            }

            bool inRange = _playerTransform != null &&
                           Vector2.Distance(transform.position, _playerTransform.position) <= _interactRadius;

            if (inRange != _isInRange)
            {
                _isInRange = inRange;
                if (_promptGO != null) _promptGO.SetActive(_isInRange);
            }

            if (_isInRange && Input.GetKeyDown(KeyCode.E))
                Interact();
        }

        // ─────────────────────────────────────────
        // 플레이어 탐색
        // ─────────────────────────────────────────
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
                if (_worldCanvas != null) _worldCanvas.gameObject.SetActive(false);
                return;
            }

            var sm = Leedoyun_SellManager.Instance;
            if (sm == null || slotIdx >= sm.ActiveOrders.Count)
            {
                if (_worldCanvas != null) _worldCanvas.gameObject.SetActive(false);
                return;
            }

            if (_worldCanvas != null) _worldCanvas.gameObject.SetActive(true);

            var itemData = ItemDatabase.Instance != null
                ? ItemDatabase.Instance.Get(sm.ActiveOrders[slotIdx].requestedWeaponId)
                : null;
            Sprite spr = itemData != null ? itemData.icon : null;

            _weaponIcon.sprite = spr;
            _weaponIcon.color  = spr != null ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.4f);
        }

        // ─────────────────────────────────────────
        // 말풍선 UI 빌드
        // ─────────────────────────────────────────
        private void BuildBubbleUI()
        {
            var canvasGO = new GameObject("NPCBubble");
            canvasGO.transform.SetParent(transform);
            canvasGO.transform.localPosition = _bubbleOffset;
            canvasGO.transform.localScale    = Vector3.one * 0.01f;

            _worldCanvas = canvasGO.AddComponent<Canvas>();
            _worldCanvas.renderMode   = RenderMode.WorldSpace;
            _worldCanvas.sortingOrder = 50;
            canvasGO.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 130f);

            var root = canvasGO.transform;

            // 말풍선 배경
            var bubble = Rect(root, "Bubble", new Vector2(0f, 25f), new Vector2(92f, 92f));
            bubble.AddComponent<Image>().color = new Color(1f, 1f, 0.92f, 0.97f);

            // 무기 아이콘
            var iconGO = Rect(bubble.transform, "WeaponIcon", Vector2.zero, new Vector2(74f, 74f));
            _weaponIcon = iconGO.AddComponent<Image>();
            _weaponIcon.preserveAspect = true;
            _weaponIcon.color = new Color(0.5f, 0.5f, 0.5f, 0.4f);

            // 말풍선 꼬리
            var tail = Rect(root, "BubbleTail", new Vector2(0f, -22f), new Vector2(14f, 14f));
            tail.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            tail.AddComponent<Image>().color = new Color(1f, 1f, 0.92f, 0.97f);

            // [E] 프롬프트
            var prompt = Rect(root, "EPrompt", new Vector2(0f, -55f), new Vector2(200f, 28f));
            prompt.AddComponent<Image>().color = new Color(0.05f, 0.05f, 0.05f, 0.82f);

            var txtGO = Rect(prompt.transform, "Text", Vector2.zero, new Vector2(196f, 24f));
            var tmp = txtGO.AddComponent<TextMeshProUGUI>();
            if (_font != null) tmp.font = _font;
            tmp.text      = "[E] 무기 판매하기";
            tmp.fontSize  = 16f;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color     = new Color(1f, 0.95f, 0.3f);
            tmp.raycastTarget = false;

            _promptGO = prompt;
            _promptGO.SetActive(false);
        }

        // ─────────────────────────────────────────
        // 헬퍼 — GameObject 반환
        // ─────────────────────────────────────────
        private static GameObject Rect(Transform parent, string name, Vector2 pos, Vector2 size)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot     = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos;
            rt.sizeDelta        = size;
            return go;
        }
    }
}
