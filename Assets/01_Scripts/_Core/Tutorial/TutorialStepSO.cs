using UnityEngine;

public enum TutorialAdvanceCondition { Click, KeyPress, Custom, Auto }

[CreateAssetMenu(fileName = "TutorialStep", menuName = "Tutorial/Step")]
public class TutorialStepSO : ScriptableObject
{
    [Header("캐릭터")]
    public string characterName;

    [Header("대사")]
    [TextArea(2, 5)]
    public string[] lines;

    [Header("단계 완료 조건")]
    public TutorialAdvanceCondition condition = TutorialAdvanceCondition.Click;
    [Tooltip("condition이 KeyPress일 때 사용")]
    public KeyCode waitKey;
    [Tooltip("condition이 Custom일 때 CompleteCustomCondition()에 넘길 ID")]
    public string customConditionId;
    [Tooltip("condition이 Auto일 때 대기 시간 (초)")]
    public float autoAdvanceDelay = 1.5f;

    [Header("옵션")]
    public bool freezeTime = false;
}
