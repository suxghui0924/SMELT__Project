using UnityEngine;
[DefaultExecutionOrder(10)]
public class Test : MonoBehaviour
{
    [SerializeField] private Transform _gameObject;
    [SerializeField] private RectTransform _rectTransform;
    void Start()
    {
        /*        BillboardUI.Instance.AddTarget(_gameObject, _rectTransform, 0.85f);
                Debug.Log("시작됨");*/
        //VolumeManager.instance.VolumeStart("ui",0.25f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            VolumeManager.instance.VolumeStart("damage", "right", 0.15f);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            VolumeManager.instance.VolumeStart("damage", "left", 0.15f);
        }
    }
}