using System.Runtime.CompilerServices;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;
    public float playTime  { private set; get; }
    public int money { private set; get; }
    public int stage;

    void Awake()
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
    private void Start()
    {
        stage = 1;
    }
    void Update()
    {
        playTime += Time.deltaTime;
    }
    public void AddMoney(int amount)
    {
        money += amount;
    }
}