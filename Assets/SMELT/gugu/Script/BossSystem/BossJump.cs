using UnityEngine;
using System.Collections;

public class BossJump : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    private float leftX = -6f;
    private float rightX = 6f;

    public IEnumerator JumpTo()
    {
        Vector3 startPos = transform.position;

        float targetX;

        // 현재 오른쪽이면 왼쪽으로
        if (transform.position.x > 0)
        {
            targetX = leftX;
            spriteRenderer.flipX = true;
        }
        // 현재 왼쪽이면 오른쪽으로
        else
        {
            targetX = rightX;
            spriteRenderer.flipX = false;
        }

        Vector3 targetPos = new Vector3(targetX, startPos.y, startPos.z);

        float duration = 1f;
        float height = 3f;

        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;

            Vector3 pos = Vector3.Lerp(startPos, targetPos, t);

            // 포물선
            pos.y += height * 4f * (t - t * t);

            transform.position = pos;

            time += Time.deltaTime;

            yield return null;
        }

        transform.position = targetPos;
    }
}