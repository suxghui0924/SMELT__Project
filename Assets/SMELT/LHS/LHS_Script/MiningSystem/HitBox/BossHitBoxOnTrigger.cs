using System.Collections.Generic;
using UnityEngine;

public class BossHitBoxOnTrigger : MonoBehaviour
{
    private PlayerHitBox _playerHitBox;
    private List<GameObject> _enemies = new List<GameObject>(); //한번 히트박스에서 데미지를 입으면 계속 데미지가 들어오는걸 방지하기 위한 리스트
    public int currentDamage;
    [SerializeField] private ParticleSystem _hitVfx;
    private Collider2D _myTrigger;
    private void Start()
    {
        _playerHitBox = GetComponentInParent<PlayerHitBox>();
        _myTrigger = GetComponent<Collider2D>();
    }
    
    private void OnEnable()
    {
        _enemies.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_playerHitBox._triggerOn)
        {
                    Vector3 myCenter = _myTrigger.bounds.center;

                    Vector3 hitPoint = other.ClosestPoint(myCenter);
                        SoundManager.instance.PlaySFX("Parry");
                        _hitVfx.transform.position = hitPoint;
                        _hitVfx.Play();
        }
                _enemies.Add(other.gameObject);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (_enemies.Contains(other.gameObject))
            {
                _enemies.Remove(other.gameObject);
            }
        }
    }
}
