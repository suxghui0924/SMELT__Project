using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TimerAndReward : MonoBehaviour
{
    public float baseTime = 5f;
    public int rewardPoint = 100;

    void Start()
    {
        StartCoroutine(DungeonTimer());
    }

    IEnumerator DungeonTimer()
    {
        float timer = 0f;

        float stageTime = baseTime + (MoneyManager.Instance.stage - 1) * 5f;

        Debug.Log("현재 스테이지: " + MoneyManager.Instance.stage);
        Debug.Log("시간: " + stageTime);

        while (timer < stageTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        DungeonClear();
    }

    void DungeonClear()
    {
        MoneyManager.Instance.AddMoney(rewardPoint);

        MoneyManager.Instance.stage++;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}