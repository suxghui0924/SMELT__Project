

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour, ISaveable
{
    public static AchievementManager Instance;

    [field: SerializeField] public AchievementSO[] AchievementSO { get; private set; }

    public Dictionary<Achievements, AchievementSO> AchievementStateDic = new Dictionary<Achievements, AchievementSO>();

    public TextMeshProUGUI[] achievementTitleAndDes;

    [SerializeField] private AchievementBannerSizer bannerSizer;

    [SerializeField] private Image achievementImage;

    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            InitDictionary();
        }

        else
            Destroy(gameObject);
    }

    private void Start()
    {
        SaveManager.Instance.Register(this);
    }
    private void InitDictionary()
    {
        AchievementStateDic.Clear();
        foreach(var achievement in AchievementSO)
        {
            if (achievement == null)
            {
                continue;
            }

            if (AchievementStateDic.ContainsKey(achievement.achievementState))
            {
                Debug.LogError($"has same achievement : {achievement.achievementState}");
                continue;
            }
            AchievementStateDic.Add(achievement.achievementState, achievement);
            Debug.Log("saved");
        }
    }

    public void AchievementClear(Achievements achievements)
    { 
        AchievementStateDic[achievements].clear = true;
    }

    public void OnSave(SaveData data)
    {
        data.clearedAchievements.Clear();

        foreach (var pair in AchievementStateDic)
        {
            if (pair.Value.clear)
            {
                data.clearedAchievements.Add(pair.Value.achievementID);
            }
        }
    }

    public void OnLoad(SaveData data)
    {
        foreach (var achievement in AchievementStateDic.Values)
        {
            achievement.clear = false;
        }

        foreach (int achievementID in data.clearedAchievements)
        {
            foreach (var achievement in AchievementStateDic.Values)
                if (achievement.achievementID == achievementID)
                {
                    achievement.clear = true;
                    Debug.Log(achievementID);
                    break;
                }
        }
    }

    public void AchPopUp(AchievementSO achievementSO)
    {
        AlarmSound();
        bannerSizer.ChangeSize(0, 1248.5f, 191f, 0.5f);
        bannerSizer.ChangeInsideSize(0, 1, 0.5f);
        achievementImage.sprite = achievementSO.achievementSprite;
        achievementTitleAndDes[0].text = achievementSO.achievementDisplayName;
        achievementTitleAndDes[1].text = achievementSO.achievementDescription;
    }

    public void AlarmPopUp(string title, string description)
    {
        AlarmSound();
        bannerSizer.ChangeSize(0, 1248.5f, 191f, 0.5f);
        bannerSizer.ChangeInsideSize(0, 1, 0.5f);
        achievementTitleAndDes[0].text = title;
        achievementTitleAndDes[1].text = description;
    }

    private void AlarmSound()
    {
        SoundManager.instance.PlaySFX("Alarm");
    }
}
