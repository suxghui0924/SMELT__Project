using System.Collections;
using UnityEngine;

public class SkillProjectile : MonoBehaviour
{
    private Collider2D myCol;

    private void Awake()
    {
        myCol = GetComponent<Collider2D>();
    }

    public void SetBoss(Collider2D bossCol)
    {
        
        Physics2D.IgnoreCollision(myCol, bossCol, true);

        
        StartCoroutine(EnableCollision(bossCol));
    }

    IEnumerator EnableCollision(Collider2D bossCol)
    {
        yield return new WaitForSeconds(0.3f);

        Physics2D.IgnoreCollision(myCol, bossCol, false);
    }
}