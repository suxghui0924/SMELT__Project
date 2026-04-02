using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Player_HpScript : MonoBehaviour
{
    [Header("최대 HP 설정")]
    public int _playerMaxHp;

    [Header("이벤트")]
    public UnityEvent<int> _playerHpChanged;
    public UnityEvent _playerDead;

    [Header("무적 작동시간 설정")]
    public float _playerInvincibleDuration;

    public int _playerCurrentHp { get; private set; }
    private bool _isPlayerDead;
    private bool _isPlayerInvincible;
    private SpriteRenderer _playerSpriteRenderer;
   [SerializeField] private UI_HpChangingScript _hpChangingUIScript;
    private void Start()
    {
        _playerSpriteRenderer = GetComponentInParent<SpriteRenderer>();
        _playerCurrentHp = _playerMaxHp;
    }

    private void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)TakeDamage(1);
    }
    public void TakeDamage(int damageValue)
    {
        if (_isPlayerDead == true || _isPlayerInvincible == true) return;
        _playerCurrentHp -= damageValue;
        _playerCurrentHp = Mathf.Clamp(_playerCurrentHp, 0, _playerMaxHp);
        _playerHpChanged?.Invoke(_playerCurrentHp);
        if (_playerCurrentHp > 0)
        {
            _hpChangingUIScript.HealthViewUpdate(_playerCurrentHp);
            StartCoroutine(InvisiblePlayer());
        }

        else if (_playerCurrentHp <= 0) PlayerGameOver();
    }

    IEnumerator InvisiblePlayer()
    {
        _isPlayerInvincible = true;
        
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
        _isPlayerInvincible = false;
        timer = 0f;
    }

    public void PlayerGameOver()
    {
        Debug.Log("플레이어 사망");
        _isPlayerDead = true;
    }
}

