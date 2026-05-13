using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OreLightScript : MonoBehaviour
{
    private Light2D _light2D;
    [Header("빛 밝기 범위 설정")]
    [SerializeField] private float minValue = 1f;
    [SerializeField] private float maxValue = 3f;
    [SerializeField] private float speed = 2.0f;
    void Start()
    {
        _light2D = GetComponent<Light2D>();
    }

    void Update()
    {
        if (_light2D == null) return;
        float timer = Time.time * speed;
        float lerpTarget = (Mathf.Sin(timer) + 1.0f) / 2.0f;
        
        _light2D.intensity = Mathf.Lerp(minValue, maxValue, lerpTarget);
    }
}
