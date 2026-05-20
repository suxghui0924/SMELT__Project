using System;
using UnityEngine;
using UnityEngine.UI;

public class GaugeBar : MonoBehaviour
{
    private LoadNum Num;
    private Image image;

    private void Awake()
    {
        Num = GetComponentInChildren<LoadNum>();
        image = GetComponent<Image>();
    }
    private void Start()
    {
        Num.StartCoroutine(Num.UpdateNum());
    }
    private void Update()
    {
        image.fillAmount = Num.saveNum / 100f;
        if (image.fillAmount >= 0.99f)
        {
            GameManager.instance.ChangeState(new HouseState());
            UICanvasManager.instance.ControlObject(ObjectType.Loading, false);
        }
    }
}