using UnityEngine;
[DefaultExecutionOrder(5)]
public class Test : MonoBehaviour
{
    [SerializeField] private Transform _gameObject;
    [SerializeField] private RectTransform _rectTransform;
    void Start()
    {
/*        BillboardUI.Instance.AddTarget(_gameObject, _rectTransform, 0.85f);
        Debug.Log("시작됨");*/
        VolumeManager.instance.VolumeStart("damage", 0.25f);
        //VolumeManager.instance.VolumeStart("ui",0.25f);
    }
}