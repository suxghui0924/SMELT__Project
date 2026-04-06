using UnityEngine;

public class StartBtn : MonoBehaviour
{
    public void PickerOn()
    {
        gameObject.SetActive(true);
    }
    public void PickerOff()
    {
        gameObject.SetActive(false);
    }
}
