using System.Collections;
using UnityEngine;

public class MelonOverride : EnemyBase
{
    protected override IEnumerator KnockbackRoutine()
    {
        _canMove = false;
        float timer = 0;
        Vector3 knockbackDir = (transform.parent.position - _playerTransform.position);
        knockbackDir.y = 0;
        knockbackDir.Normalize();
        float knockbackForce = knockbackPower * 4f;

        _melonRot = 0;
        while (timer <= knockbackTimer)
        {
            float progress = timer / knockbackTimer;
            float currentForce = Mathf.Lerp(knockbackForce, 0, progress);
            transform.parent.position += knockbackDir * (currentForce * Time.deltaTime);
            
            timer += Time.deltaTime;
            yield return null;
        }
        
        _canMove = true;
    }
}