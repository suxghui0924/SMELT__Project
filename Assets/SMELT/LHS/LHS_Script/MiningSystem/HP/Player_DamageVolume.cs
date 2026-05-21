using UnityEngine;
[DefaultExecutionOrder(10)]
public class Player_DamageVolume : MonoBehaviour
{
    [SerializeField] Player_HpScript _playerHpScript;
    void Start()
    {
        /*        BillboardUI.Instance.AddTarget(_gameObject, _rectTransform, 0.85f);
                Debug.Log("시작됨");*/
        //VolumeManager.instance.VolumeStart("ui",0.25f);
       // _playerHpScript = GetComponent<Player_HpScript>();
    }

    void Update()
    {
        /*if (_playerHpScript.IsPlayerDead == false &&_playerHpScript.IsPlayerInvincible == true)
        {
            VolumeManager.instance.VolumeStart("damage", "right", 0.15f);
        }*/
    }
}