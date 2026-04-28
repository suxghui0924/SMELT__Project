using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckSum : MonoBehaviour
{
    Image _bar;
    void Awake()
    {
        _bar = GetComponent<Image>();
    }
    void Update()
    {
        if(_bar.fillAmount >= 0.99f)
        {
            GameManager.instance.ChangeState(GameDataSO.GameState.House);
            UICanvasManager.instance.ControlObject("System", 0, false);
        }
    }
}
