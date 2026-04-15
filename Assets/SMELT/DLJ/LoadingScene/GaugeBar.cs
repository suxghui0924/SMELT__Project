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
    }
}
