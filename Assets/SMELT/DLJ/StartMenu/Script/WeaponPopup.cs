using UnityEngine;

public class WeaponPopup : MonoBehaviour
{
    private Transform transform;
    private void Awake()
    {
        transform = GetComponent<Transform>();
    }
    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.7f, 0.7f, 0), 5f * Time.deltaTime);
    }
}
