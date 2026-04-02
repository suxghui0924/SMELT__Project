using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject craftUI;

    public void CloseCraftUI()
    {
        craftUI.SetActive(false);
    }
}