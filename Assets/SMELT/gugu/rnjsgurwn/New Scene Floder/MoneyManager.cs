using UnityEngine;
using UnityEngine.SceneManagement;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;
    public int money;

    private void Awake()
    {
        Instance = this;
    }

    public void AddMoney(int amount)
    {
        money += amount;
        Debug.Log("돈: " + money);
    }
}
