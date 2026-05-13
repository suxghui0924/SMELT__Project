using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine.UI;

public class TimerAndReward : MonoBehaviour
{
    public static TimerAndReward Instance;
    private int rewardPoint = 100;
    private int minFatigue = 0;
    [HideInInspector]
    public int maxFatigue = 100;
    [HideInInspector]
    public float currentFatigue;
    private float clearTime = 60f;
    public Image _mp;
    float timer = 0f;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        currentFatigue = maxFatigue;
        //StartCoroutine(DungeonTimer());
        
    }

    void Update()
    {
        timer+= Time.deltaTime;
        if (timer >= 1)
        {
            ReduceFatigue(1);
            timer = 0;
        }
        //ReduceFatigue(1);
    }
    
    bool isGameOver = false;

    /*IEnumerator DungeonTimer()
    {
        float timer = 0f;
        float moneyTimer = 0f;

        while (timer < clearTime)
        {
            timer += Time.deltaTime;
            moneyTimer += Time.deltaTime;

            if (moneyTimer >= 1f)
            {
                ReduceFatigue(1);
                _mp.fillAmount -= 0.01f;
               
                    
            }

            yield return null;
        }
        DungeonClear(); // 죽으면 클리어 처리 or 실패 처리
        
    }*/

    void DungeonClear() //미완임 나중에 던전클리어UI완성하면 연결할꺼
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReduceFatigue(float amount)
    {
        _mp.fillAmount -= (amount+0.0f)/100f;
        currentFatigue -= amount;
        Debug.Log($"피로도 감소: {amount}, 현재 피로도: {currentFatigue}");
        if (currentFatigue <= 0)
        {
            currentFatigue = 0;
           //GameOver();
        }
    }
    /*void GameOver()
    {
        Debug.Log("피로도 0 → 게임 오버");
    
        Time.timeScale = 0f;
       //GameManager.instance.ChangeState(GameDataSO.GameState.GameOver);
    }*/
    
}