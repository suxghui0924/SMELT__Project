using UnityEngine;

public class BtnPicker : MonoBehaviour
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
