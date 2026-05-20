using System;
using UnityEngine;

public class BSkillAttack : MonoBehaviour
{
    public float damage;
    public bool canHit = false;
    
    
    public void CanDamage(int damages)
    {
        damage = damages;
        canHit = true;
    }
}
