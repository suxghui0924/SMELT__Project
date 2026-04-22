using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    public int money;

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
    public bool UseMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddMoney(int rewardPoint)
    {
        money += rewardPoint;
        Debug.Log("현재 돈: " + money);
    }
}