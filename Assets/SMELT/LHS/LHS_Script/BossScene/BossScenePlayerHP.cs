using System.Collections;
using _01_Scripts._Core._States;
using _01_Scripts.Player.GameOver;
using UnityEngine;

public class BossScenePlayerHP : MonoBehaviour
{
    [Header("무적 작동시간 설정")]
    public float _playerInvincibleDuration;

    public float PlayerCurrentHp;

    public bool IsPlayerDead { get; private set; }
    public bool IsPlayerInvincible {get; private set; }

    private float timer = 0f;
    [SerializeField]private skillSystem _skillSystem;
    
    [SerializeField]private BossSceneSceneHpUI _bossSceneSceneHpUI;
    
    private SpriteRenderer _playerSpriteRenderer;
    
    private void Start()
    {
        _playerSpriteRenderer = transform.parent.GetComponentInChildren<SpriteRenderer>();
        IsPlayerDead = false;
        IsPlayerInvincible = false;
    }
    
    private void Update()
    {
        if (!IsPlayerDead&&_skillSystem.IsAlive)
        {
            timer += Time.deltaTime;
            if (timer >= 1)
            {
                ReduceFatigue(1f);
                timer = 0;
            }
        }
    }
    public void TakeDamage(float damageValue)
    {
        
        if (IsPlayerDead == true || IsPlayerInvincible == true) return;
        VolumeManager.instance.VolumeStart("damage", "left",0.2f);
        ReduceFatigue(damageValue);
        if (PlayerCurrentHp > 0)
        {
            StartCoroutine(InvisiblePlayer());
        }
        
        else if (PlayerCurrentHp <= 0) PlayerGameOver();
        
    }
    IEnumerator InvisiblePlayer()
    {
        IsPlayerInvincible = true;
        
        Color _playerAlphaChange=Color.white;
        float timer = 0f;
        while (timer < _playerInvincibleDuration)
        {
            _playerAlphaChange = _playerSpriteRenderer.color;
            _playerAlphaChange.a = Mathf.PingPong(Time.time*0.1f,0.2f);
            _playerSpriteRenderer.color = _playerAlphaChange;

            timer += Time.deltaTime;
            yield return null;
        }
        _playerAlphaChange.a=1f;
        _playerSpriteRenderer.color = _playerAlphaChange;
        IsPlayerInvincible = false;
        timer = 0f;
    }

    private void ReduceFatigue(float amount)
    {
        PlayerCurrentHp -= amount;
        _bossSceneSceneHpUI.HpChanged(amount);   
    }
    
    public void PlayerGameOver()
    {
        GameManager.instance.ChangeState(new GameDieState());
    }
}
