using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set;}
    public GameDataSO gameData;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            if (gameData != null)
            {
                gameData.Reset();
            }
        }
        else
        {
            Destroy(gameObject);
        }

    }
}

