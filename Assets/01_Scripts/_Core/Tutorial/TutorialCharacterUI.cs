using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCharacterUI : MonoBehaviour
{
    [SerializeField] private TMP_FontAsset _font;
    [SerializeField] private Sprite _characterSprite;
    [SerializeField] private AnimationClip _talkAnimation;
    [SerializeField] private float _typingSpeed = 0.04f;

    public bool IsTypingComplete { get; private set; }

    private Animation _charAnimation;
    private GameObject _panel;
    private TextMeshProUGUI _nameText;
    private TextMeshProUGUI _dialogueText;
    private TextMeshProUGUI _clickIndicator;
    private Coroutine _typingCoroutine;
    private string _currentLine;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        BuildUI();
    }

    private void BuildUI()
    {
        var canvasGO = new GameObject("TutorialCanvas");
        DontDestroyOnLoad(canvasGO);

        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // 하단 대화창 패널
        _panel = new GameObject("TutorialPanel");
        _panel.transform.SetParent(canvasGO.transform, false);
        var bg = _panel.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.85f);
        var panelRect = _panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0f, 0f);
        panelRect.anchorMax = new Vector2(1f, 0.13f);
        panelRect.offsetMin = panelRect.offsetMax = Vector2.zero;

        // 이름 텍스트
        var nameGO = new GameObject("NameText");
        nameGO.transform.SetParent(_panel.transform, false);
        _nameText = nameGO.AddComponent<TextMeshProUGUI>();
        _nameText.fontSize = 22;
        _nameText.fontStyle = FontStyles.Bold;
        _nameText.color = Color.yellow;
        if (_font != null) _nameText.font = _font;
        var nameRect = nameGO.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0.02f, 0.65f);
        nameRect.anchorMax = new Vector2(0.5f, 1f);
        nameRect.offsetMin = nameRect.offsetMax = Vector2.zero;

        // 대사 텍스트
        var dialogueGO = new GameObject("DialogueText");
        dialogueGO.transform.SetParent(_panel.transform, false);
        _dialogueText = dialogueGO.AddComponent<TextMeshProUGUI>();
        _dialogueText.fontSize = 24;
        _dialogueText.color = Color.white;
        if (_font != null) _dialogueText.font = _font;
        var dialogueRect = dialogueGO.GetComponent<RectTransform>();
        dialogueRect.anchorMin = new Vector2(0.02f, 0.05f);
        dialogueRect.anchorMax = new Vector2(0.98f, 0.65f);
        dialogueRect.offsetMin = dialogueRect.offsetMax = Vector2.zero;

        // 클릭 인디케이터 (우측 하단 ▼)
        var indicatorGO = new GameObject("ClickIndicator");
        indicatorGO.transform.SetParent(_panel.transform, false);
        _clickIndicator = indicatorGO.AddComponent<TextMeshProUGUI>();
        _clickIndicator.text = "▼";
        _clickIndicator.fontSize = 20;
        _clickIndicator.color = Color.yellow;
        _clickIndicator.alignment = TextAlignmentOptions.BottomRight;
        if (_font != null) _clickIndicator.font = _font;
        var indicatorRect = indicatorGO.GetComponent<RectTransform>();
        indicatorRect.anchorMin = new Vector2(0.85f, 0f);
        indicatorRect.anchorMax = new Vector2(1f, 0.4f);
        indicatorRect.offsetMin = indicatorRect.offsetMax = Vector2.zero;
        _clickIndicator.gameObject.SetActive(false);

        // 캐릭터 이미지 (대화창 왼쪽 위)
        if (_characterSprite != null)
        {
            var charImgGO = new GameObject("CharacterImage");
            charImgGO.transform.SetParent(canvasGO.transform, false);
            var charImg = charImgGO.AddComponent<Image>();
            charImg.sprite = _characterSprite;
            charImg.preserveAspect = true;
            var charRect = charImgGO.GetComponent<RectTransform>();
            charRect.anchorMin = new Vector2(0f, 0.13f);
            charRect.anchorMax = new Vector2(0.13f, 0.41f);
            charRect.offsetMin = charRect.offsetMax = Vector2.zero;

            if (_talkAnimation != null)
            {
                _charAnimation = charImgGO.AddComponent<Animation>();
                _talkAnimation.wrapMode = WrapMode.Loop;
                _charAnimation.AddClip(_talkAnimation, _talkAnimation.name);
                _charAnimation.clip = _talkAnimation;
                _charAnimation.Play();
            }
        }

        _panel.SetActive(false);
    }

    public void Show(string charName)
    {
        _panel.SetActive(true);
        _nameText.text = charName;
    }

    public void Hide()
    {
        _panel.SetActive(false);
        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
    }

    public void StartLine(string line)
    {
        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
        IsTypingComplete = false;
        _currentLine = line;
        _clickIndicator.gameObject.SetActive(false);
        _typingCoroutine = StartCoroutine(TypeRoutine(line));
    }

    public void SkipToEnd()
    {
        if (IsTypingComplete) return;
        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
        _dialogueText.text = _currentLine;
        _clickIndicator.gameObject.SetActive(true);
        IsTypingComplete = true;
        _typingCoroutine = null;
    }

    private IEnumerator TypeRoutine(string line)
    {
        _clickIndicator.gameObject.SetActive(false);
        _dialogueText.text = "";

        foreach (char c in line)
        {
            _dialogueText.text += c;
            yield return new WaitForSecondsRealtime(_typingSpeed);
        }

        _clickIndicator.gameObject.SetActive(true);
        IsTypingComplete = true;
        _typingCoroutine = null;
    }
}
