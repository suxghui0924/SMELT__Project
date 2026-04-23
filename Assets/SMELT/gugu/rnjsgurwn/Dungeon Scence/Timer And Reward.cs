using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine.UI;

public class TimerAndReward : MonoBehaviour
{
    public static TimerAndReward Instance;
    //public float baseTime = 1f;
    private int rewardPoint = 100;
    private int minFatigue = 0;
    int maxFatigue = 100;
    int currentFatigue;
    float clearTime = 60f;
    [SerializeField] private Image _mp;

   private int _stage = 0;

    


    void Start()
    {
        _stage = 1;
        currentFatigue = maxFatigue;
        StartCoroutine(DungeonTimer());
        
    }


    bool isGameOver = false;

    IEnumerator DungeonTimer()
    {
        float timer = 0f;
        float moneyTimer = 0f;

        while (timer < clearTime)
        {
            timer += Time.deltaTime;
            moneyTimer += Time.deltaTime;

            if (moneyTimer >= 1f)
            {
                int rewardPerSecond = Mathf.FloorToInt(MoneyManager.Instance.playTime / 60f) + 1;

                MoneyManager.Instance.AddMoney(rewardPerSecond);

                moneyTimer = 0f;               
                ReduceFatigue(1);
                _mp.fillAmount -= 0.01f;
               
                    
            }

            yield return null;
        }
        DungeonClear(); // 죽으면 클리어 처리 or 실패 처리
        
    }

    void DungeonClear()
    {
        int reward = rewardPoint;
        if (_stage >= 5)
        {
            reward = 100 + ((_stage - 1) / 5) * 50;
        }
        MoneyManager.Instance.AddMoney(reward);
        _stage++;
        Debug.Log($"다음 스테이지: {MoneyManager.Instance.stage}");

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }

    public void ReduceFatigue(int amount)
    {
        currentFatigue -= amount;
        Debug.Log($"피로도 감소: {amount}, 현재 피로도: {currentFatigue}");
        if (currentFatigue <= 0)
        {
            currentFatigue = 0;
            GameOver();
        }
    }
    void GameOver()
    {
        Debug.Log("피로도 0 → 게임 오버");

        Time.timeScale = 0f;
        // 대충 2초 후에 마을로 돌아갈수 있게 할수 있?
    }
}