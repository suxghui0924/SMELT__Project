using UnityEngine;

public class TutoHit : MonoBehaviour
{
    private PlayerAnimation _anim;
    private PlayerAttack _att;

    private void Start()
    {
        _att = GameObject.Find("PlayerVisual").GetComponent<PlayerAttack>();
        _anim = GameObject.Find("PlayerVisual").GetComponent<PlayerAnimation>();
    }
}
