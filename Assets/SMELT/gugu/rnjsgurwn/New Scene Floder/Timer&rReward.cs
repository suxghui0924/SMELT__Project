using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TimerAndReward : MonoBehaviour
{
    public float clearTime = 5f;
    public int rewardPoint = 100;

    
    void Start()
    {
        Debug.Log("Timer 시작됨");
        StartCoroutine(DungeonTimer());
    }
   
    IEnumerator DungeonTimer()
    {
        Debug.Log("타이머 시작");

        float timer = 0f;

        while (timer < clearTime)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        DungeonClear();
    }

    void DungeonClear()
    {
        Debug.Log("던전 클리어!");
        MoneyManager.Instance.AddMoney(rewardPoint);

        StartCoroutine(ReturnToField());
    }

    IEnumerator ReturnToField()
    {
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("Work_CreaftSystem_gurwn");
    }
}
