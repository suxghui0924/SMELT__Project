using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AchievementManager : MonoBehaviour, ISaveable
{
    public static AchievementManager Instance;

    [field: SerializeField] public AchievementSO[] AchievementSO { get; private set; }

    public Dictionary<Achievements, AchievementSO> AchievementStateDic = new Dictionary<Achievements, AchievementSO>();

    public TextMeshProUGUI[] achievementTitleAndDes;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitDictionary();
        }

        else
            Destroy(gameObject);
    }

    private void Start()
    {
        SaveManager.Instance.Register(this);
        AchievementClear(Achievements.FirstJoined);
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

        foreach (string achievementID in data.clearedAchievements)
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
    public bool GetAchievementState(Achievements achievements)
    {
        return AchievementStateDic.TryGetValue(achievements, out AchievementSO achievement) && achievement.clear;
    }

    public void AchPopUp(AchievementSO achievementSO)
    {
        achievementTitleAndDes[0].text = achievementSO.achievementDisplayName;
        achievementTitleAndDes[1].text = achievementSO.achievementDescription;
    }
}
