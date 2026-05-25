using System.Collections;
using UnityEngine;

public class TutorialRunner : MonoBehaviour
{
    [SerializeField] private TutorialSequenceSO _sequence;
    [SerializeField] private TutorialCharacterUI _characterUI;

    public static TutorialRunner Instance { get; private set; }

    private int _stepIndex;
    private int _lineIndex;
    private TutorialStepSO _currentStep;
    private string _pendingCustomId;

    private enum TutorialState { Idle, Typing, LineWait, ConditionWait }
    private TutorialState _state = TutorialState.Idle;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (_sequence == null)            { Debug.LogError("[TutorialRunner] Sequence가 null입니다. 인스펙터에서 할당하세요."); return; }
        if (_sequence.steps.Length == 0)  { Debug.LogError("[TutorialRunner] Sequence에 Step이 없습니다. SO 자동 생성을 실행하세요."); return; }
        if (_characterUI == null)         { Debug.LogError("[TutorialRunner] CharacterUI가 null입니다. 인스펙터에서 할당하세요."); return; }
        RunStep(0);
    }

    private void Update()
    {
        switch (_state)
        {
            case TutorialState.Typing:
                if (Input.GetMouseButtonDown(0))
                    _characterUI.SkipToEnd();
                break;

            case TutorialState.LineWait:
                if (Input.GetMouseButtonDown(0))
                    NextLine();
                break;

            case TutorialState.ConditionWait:
                CheckStepCondition();
                break;
        }
    }

    private void RunStep(int index)
    {
        _stepIndex = index;
        _lineIndex = 0;
        _currentStep = _sequence.steps[index];

        if (_currentStep == null) { Debug.LogError($"[TutorialRunner] steps[{index}]가 null입니다. SO 할당을 확인하세요."); return; }
        if (_currentStep.lines == null || _currentStep.lines.Length == 0) { Debug.LogError($"[TutorialRunner] steps[{index}]에 대사가 없습니다."); return; }

        Debug.Log($"[TutorialRunner] Step {index} 시작: {_currentStep.characterName} / {_currentStep.lines[0]}");

        if (_currentStep.freezeTime) Time.timeScale = 0f;

        _characterUI.Show(_currentStep.characterName);
        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        _state = TutorialState.Typing;
        _characterUI.StartLine(_currentStep.lines[_lineIndex]);
        StartCoroutine(WaitForTypingComplete());
    }

    private IEnumerator WaitForTypingComplete()
    {
        yield return new WaitUntil(() => _characterUI.IsTypingComplete);

        bool isLastLine = _lineIndex >= _currentStep.lines.Length - 1;

        if (!isLastLine)
        {
            _state = TutorialState.LineWait;
            yield break;
        }

        if (_currentStep.condition == TutorialAdvanceCondition.Auto)
            StartCoroutine(AutoAdvance());
        else if (_currentStep.condition == TutorialAdvanceCondition.Custom)
        {
            _pendingCustomId = _currentStep.customConditionId;
            _state = TutorialState.ConditionWait;
        }
        else
        {
            _state = TutorialState.ConditionWait;
        }
    }

    private void NextLine()
    {
        _lineIndex++;
        ShowCurrentLine();
    }

    private void CheckStepCondition()
    {
        switch (_currentStep.condition)
        {
            case TutorialAdvanceCondition.Click:
                if (Input.GetMouseButtonDown(0)) AdvanceStep();
                break;
            case TutorialAdvanceCondition.KeyPress:
                if (Input.GetKeyDown(_currentStep.waitKey)) AdvanceStep();
                break;
        }
    }

    private IEnumerator AutoAdvance()
    {
        _state = TutorialState.Idle;
        yield return new WaitForSecondsRealtime(_currentStep.autoAdvanceDelay);
        AdvanceStep();
    }

    // 외부에서 Custom 조건 완료 시 호출 (예: 적 처치, 무기 제작 등)
    public void CompleteCustomCondition(string conditionId)
    {
        if (_state == TutorialState.ConditionWait && _pendingCustomId == conditionId)
            AdvanceStep();
    }

    private void AdvanceStep()
    {
        if (_currentStep.freezeTime) Time.timeScale = 1f;

        int next = _stepIndex + 1;
        if (next >= _sequence.steps.Length)
        {
            EndTutorial();
            return;
        }
        RunStep(next);
    }

    public void Restart()
    {
        _stepIndex = 0;
        _lineIndex = 0;
        _state = TutorialState.Idle;
        RunStep(0);
    }

    private void EndTutorial()
    {
        _characterUI.Hide();
        Time.timeScale = 1f;
        _state = TutorialState.Idle;
        ShopManager.Instance?.CloseShop();
        AchievementClear.instance?.ClearAchievement(Achievements.FirstJoined);
        SaveManager.Instance?.Save();
    }
}
