using System;
using _01_Scripts._Core._States;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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

        public bool db = false;
        [HideInInspector]
        public bool canEnter = true;
        
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
                SceneManager.sceneLoaded += OnSceneChanged;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (SaveManager.Instance != null)
                SaveManager.Instance.Register(this);
        }

        private void OnSceneChanged(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name == "LHS_MiningScene")
            {
                // 저장 파일이 있으면 저장된 스태미나 값으로 복원, 없으면 최대치로 초기화
                if (SaveManager.Instance != null && SaveManager.Instance.HasSaveData())
                {
                    currentFatigue = SaveManager.Instance.CurrentData.stamina;
                    db = true;
                    OnFatigueChange?.Invoke(currentFatigue);
                }
                else
                {
                    FatigueReset();
                }
            }
        }
        
        public void FatigueReset()
        {
            db = true;
            currentFatigue = maxFatigue;
        }
    
        void Update()
        {
            if(db&&canEnter)
            {
                timer += Time.deltaTime;
                if (timer >= 1)
                {
                    ReduceFatigue(2f);
                    timer = 0;
                }
            }
        }
    
        public void ReduceFatigue(float amount)
        {
            currentFatigue -= amount;
            currentFatigue = Mathf.Clamp(currentFatigue, 0, maxFatigue);
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
            canEnter = false;
            Debug.Log("피로도 0 → 게임 오버");
            GameManager.instance.ChangeState(new GameDieState());
        }
    }
}