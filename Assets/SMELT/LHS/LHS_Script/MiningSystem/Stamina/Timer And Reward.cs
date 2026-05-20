using System;
using _01_Scripts._Core._States;
using UnityEngine;
using UnityEngine.Events;

namespace SMELT.LHS.LHS_Script.MiningSystem.Stamina
{
    public class TimerAndReward : MonoBehaviour,ISaveable
    {
        public static TimerAndReward Instance;
        [HideInInspector]
        public int maxFatigue = 100;
        [HideInInspector]
        public float currentFatigue;
        //public Image _mp;
        float timer = 0f;
    
        [HideInInspector]
        public UnityEvent<float> OnFatigueChange;

        private bool db = true;
        
        
        public void OnSave(SaveData data)
        {
            data.stamina=currentFatigue;
        }

        public void OnLoad(SaveData data)
        {
            currentFatigue = data.stamina;
        }
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

        private void OnEnable()
        {
            db = true;
        }

        void Start()
        {
            currentFatigue = maxFatigue;
            //StartCoroutine(DungeonTimer());
        
        }

        void Update()
        {
            if(db)
            {
                timer += Time.deltaTime;
                if (timer >= 1)
                {
                    ReduceFatigue(2f);
                    timer = 0;
                }
            }
        }
    
        private bool isGameOver = false;

        public void ReduceFatigue(float amount)
        {
            currentFatigue -= amount;
            currentFatigue = Mathf.Clamp(currentFatigue,0,maxFatigue);
            Debug.Log($"피로도 감소: {amount}, 현재 피로도: {currentFatigue}");
            OnFatigueChange?.Invoke(currentFatigue);
            if (currentFatigue <= 0)
            {
                currentFatigue = 0;
                GameOver();
            }
        }
    
    
        private void GameOver()
        {
            db = false;
            Debug.Log("피로도 0 → 게임 오버");
            GameManager.instance.ChangeState(new GameDieState());
        }
    
    }
}