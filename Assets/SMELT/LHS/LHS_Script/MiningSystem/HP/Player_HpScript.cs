using System;
using System.Collections;
using SMELT.LHS.LHS_Script.MiningSystem.Stamina;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Player_HpScript : MonoBehaviour
{

    [Header("무적 작동시간 설정")]
    public float _playerInvincibleDuration;

    public float PlayerCurrentHp { get; private set; }

    public bool IsPlayerDead { get; private set; }
    public bool IsPlayerInvincible {get; private set; }
    
    private SpriteRenderer _playerSpriteRenderer;
    private void Start()
    {
        _playerSpriteRenderer = transform.parent.GetComponentInChildren<SpriteRenderer>();
        PlayerCurrentHp = TimerAndReward.Instance.currentFatigue;
        IsPlayerDead = false;
        IsPlayerInvincible = false;
    }

    private void Update()
    {
        PlayerCurrentHp = TimerAndReward.Instance.currentFatigue;
    }

    public void TakeDamage(float damageValue,string dir)
    {
        if (IsPlayerDead == true || IsPlayerInvincible == true) return;
        VolumeManager.instance.VolumeStart("damage",dir,0.1f);
        TimerAndReward.Instance.ReduceFatigue(damageValue);
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

    public void PlayerGameOver()
    {
        IsPlayerDead = true;
    }
}

