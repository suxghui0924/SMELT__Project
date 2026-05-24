using System;
using _01_Scripts._Core._States;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace SMELT.LHS.LHS_Script.MiningSystem.Stamina
{
    public class TimerAndReward : MonoBehaviour,ISaveable
    {
        private HitBoxOnTrigger hitBoxOnTrigger1;
        private HitBoxOnTrigger hitBoxOnTrigger2;
        private PlayerHitBox playerHitBox;
        private PlayerAttack playerAttack;
        public static TimerAndReward Instance;
        [HideInInspector]
        public int maxFatigue = 100;
        [HideInInspector]
        public float currentFatigue = 100;
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
                //DontDestroyOnLoad(gameObject);
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
                playerAttack = new PlayerAttack();
                hitBoxOnTrigger1 = new HitBoxOnTrigger();
                hitBoxOnTrigger2 = new HitBoxOnTrigger();
                playerHitBox = new PlayerHitBox();
                playerAttack = GameObject.Find("PlayerVisual").GetComponent<PlayerAttack>();
                playerHitBox = GameObject.Find("HitBox").GetComponent<PlayerHitBox>();
                hitBoxOnTrigger1 = GameObject.Find("LeftHitBox").GetComponent<HitBoxOnTrigger>();
                hitBoxOnTrigger2 = GameObject.Find("RightHitBox").GetComponent<HitBoxOnTrigger>();
                if (PickaxeManager.Instance.EquippedPickaxe != null)
                {
                    hitBoxOnTrigger1.GetPickaxeData(PickaxeManager.Instance.EquippedPickaxe);
                    hitBoxOnTrigger2.GetPickaxeData(PickaxeManager.Instance.EquippedPickaxe);
                }

                SaveData data = SaveManager.Instance.CurrentData;
               float parry = data.parryRange;
               float attackSpeed = data.attackSpeed;
               if(playerHitBox!=null) playerHitBox.HitboxUpdate(parry);
               if(playerAttack!=null) playerAttack.SkillCooldownUpdate(attackSpeed);
               
                // 저장 파일이 있으면 저장된 스태미나 값으로 복원, 없으면 최대치로 초기화
                if (SaveManager.Instance != null && SaveManager.Instance.HasSaveData())
                {
                    currentFatigue = SaveManager.Instance.CurrentData.stamina;
                    canEnter = true;
                    db = true;
                    OnFatigueChange?.Invoke(currentFatigue);
                }
                else
                {
                    FatigueReset();
                    db = true;
                }
            }
            else
            {
                db = false; // 채광 씬이 아니면 스태미나 감소 중지
            }
        }
        
        public void FatigueReset()
        {
            currentFatigue = maxFatigue;
            OnFatigueChange?.Invoke(currentFatigue);
            Debug.Log("스테미나 초기화: "+currentFatigue);
            db = false;
        }
    
        void Update()
        {
            if(db&&canEnter)
            {
                timer += Time.deltaTime;
                if (timer >= 1)
                {
                    ReduceFatigue(1f);
                    timer = 0;
                }
            }
        }
    
        public void ReduceFatigue(float amount)
        {
            currentFatigue -= amount;
            currentFatigue = Mathf.Clamp(currentFatigue, 0, maxFatigue);
                    Debug.Log($"스테미나 감소: {amount}, 현재 스테미나: {currentFatigue}");
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