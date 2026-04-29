using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Player_HpScript : MonoBehaviour
{
    [Header("최대 HP 설정")]
    public int _playerMaxHp;

    [FormerlySerializedAs("_playerHpChanged")] [HideInInspector]
    public UnityEvent<int> PlayerHpChanged;
    [FormerlySerializedAs("_playerDead")][HideInInspector] public UnityEvent PlayerDead;

    [Header("무적 작동시간 설정")]
    public float _playerInvincibleDuration;

    public int PlayerCurrentHp { get; private set; }

    public bool IsPlayerDead { get; private set; }
    public bool IsPlayerInvincible {get; private set; }
    
    private SpriteRenderer _playerSpriteRenderer;
   [SerializeField] private UI_HpChangingScript _hpChangingUIScript;
    private void Start()
    {
        _playerSpriteRenderer = transform.parent.GetComponentInChildren<SpriteRenderer>();
        PlayerCurrentHp = _playerMaxHp;
        IsPlayerDead = false;
        IsPlayerInvincible = false;
    }

    private void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)TakeDamage(1);
    }
    public void TakeDamage(int damageValue)
    {
        if (IsPlayerDead == true || IsPlayerInvincible == true) return;
        PlayerCurrentHp -= damageValue;
        PlayerCurrentHp = Mathf.Clamp(PlayerCurrentHp, 0, _playerMaxHp);
        PlayerHpChanged?.Invoke(PlayerCurrentHp);
        if (PlayerCurrentHp > 0)
        {
            _hpChangingUIScript.HealthViewUpdate(PlayerCurrentHp);
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

    public void PlayerGameOver()
    {
        _hpChangingUIScript.HealthViewUpdate(PlayerCurrentHp);
        GameManager.instance.ChangeState(new GameOverState());
        Debug.Log("플레이어 사망");
        IsPlayerDead = true;
    }
}

