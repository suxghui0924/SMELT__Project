using UnityEngine;

public class textAnimation : MonoBehaviour
{
    public Transform[] loadTransform;
    public float speed = 3f; // 순서가 넘어가는 속도

    private float[] startY;
    void Start()
    {
        startY = new float[loadTransform.Length];
        for (int i = 0; i < loadTransform.Length; i++)
        {
            startY[i] = loadTransform[i].position.y;
        }
    }

    void Update()
    {
        // 핵심! 시간에 따라 0, 1, 2, 1, 0 숫자를 무한 반복해서 만들어냅니다.
        int targetIndex = Mathf.FloorToInt(Mathf.PingPong(Time.time * speed, loadTransform.Length - 0.01f));

        for (int i = 0; i < loadTransform.Length; i++)
        {
            // 내 차례(targetIndex)면 위로 0.5만큼, 아니면 원래 위치(0)로 목표를 잡습니다.
            float targetY = startY[i] + (i == targetIndex ? 0.05f : 0f);

            // Lerp를 사용해 목표 위치로 "부드럽게" 끌어당깁니다.
            Vector3 pos = loadTransform[i].position;
            pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * 10f);
            loadTransform[i].position = pos;
        }
    }
}
