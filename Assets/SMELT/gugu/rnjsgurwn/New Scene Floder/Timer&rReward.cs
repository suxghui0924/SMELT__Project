using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TimerAndReward : MonoBehaviour
{
    public float clearTime = 60f;
    public int rewardPoint = 100;

    void Start()
    {
        StartCoroutine(DungeonTimer());
    }

    IEnumerator DungeonTimer()
    {
        yield return new WaitForSeconds(clearTime);

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

        SceneManager.LoadScene("Field");
    }
}
