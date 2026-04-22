using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using JetBrains.Annotations;

public class TimerAndReward : MonoBehaviour
{
    public static TimerAndReward Instance;
    //public float baseTime = 1f;
    public int rewardPoint = 100;
    public int minFatigue = 0;
    public int maxFatigue = 100;
    public int currentFatigue;
    Coroutine timerCoroutine;

    void Start()
    {
        currentFatigue = maxFatigue;
        if (timerCoroutine == null)
        {
            timerCoroutine = StartCoroutine(DungeonTimer());
        }
    }
     /*IEnumerator DungeonTimer()
    {
        float timer = 0f;

        float stageTime = baseTime + (MoneyManager.Instance.stage - 1) * 0.1f;

        Debug.Log("시간: " + stageTime);
        Debug.Log("현재 스테이지: " + MoneyManager.Instance.stage);

        while (timer < stageTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }                                                   // 타이머 종료 후 보상 지급 및 씬 리로드

        DungeonClear();
    }

    void DungeonClear()
    {
        int reward = rewardPoint;
        if (MoneyManager.Instance.stage >= 5)
        {
            reward = 100 + ((MoneyManager.Instance.stage - 1) / 5) * 50;
        }
        MoneyManager.Instance.AddMoney(reward);
        MoneyManager.Instance.stage++;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}*/

    bool isGameOver = false;

    IEnumerator DungeonTimer()
    {
        float timer = 0f;
        float moneyTimer = 0f;

        while (!isGameOver)
        {
            timer += Time.deltaTime;
            moneyTimer += Time.deltaTime;

            if (moneyTimer >= 1f)
            {
                int rewardPerSecond = Mathf.FloorToInt(timer / 60f) + 1;         // 1초마다 돈 지금 근데 1분마다 초당 돈지급량 증가
                MoneyManager.Instance.AddMoney(rewardPerSecond);
                moneyTimer = 0f;
            }

            yield return null;
        }

        DungeonClear(); // 죽으면 클리어 처리 or 실패 처리
    }

    void DungeonClear()
    {
        int reward = rewardPoint;
        if (MoneyManager.Instance.stage >= 5)
        {
            reward = 100 + ((MoneyManager.Instance.stage - 1) / 5) * 50;
        }
        MoneyManager.Instance.AddMoney(reward);
        MoneyManager.Instance.stage++;

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

        StopAllCoroutines(); // 타이머 멈춤

        // 여기서 씬 이동 or UI 띄우기
    }
}