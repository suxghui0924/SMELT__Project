using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;



public class TimerAndReward : MonoBehaviour
{
    [SerializeField] private TimerAndReward instance;
    public int stage = 1;
    public float baseTime = 20f;
    public int rewardPoint = 100;

    
    void Start()
    {
        StartCoroutine(DungeonTimer());
    }
   
    IEnumerator DungeonTimer()
    {
        Debug.Log("타이머 시작");

        float timer = 0f;
        float stageTime = baseTime + (stage - 1) * 5f;
        while (timer < baseTime)
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
        
        StartCoroutine(ChangeStage());
    }

    IEnumerator ChangeStage()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
   
}
