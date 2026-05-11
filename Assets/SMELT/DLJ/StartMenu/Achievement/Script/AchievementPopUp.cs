using TMPro;
using UnityEngine;

public class AchievementPopUp : MonoBehaviour
{
    public TextMeshProUGUI[] achievementTitleAndDes;
    public void AchPopUp(AchievementSO achievementSO)
    {
        achievementTitleAndDes[0].text = achievementSO.achievementDisplayName;
        achievementTitleAndDes[0].text = achievementSO.achievementDescription;
    }
}
