using UnityEngine;

[CreateAssetMenu(fileName = "TutorialSequence", menuName = "Tutorial/Sequence")]
public class TutorialSequenceSO : ScriptableObject
{
    public TutorialStepSO[] steps;
}
