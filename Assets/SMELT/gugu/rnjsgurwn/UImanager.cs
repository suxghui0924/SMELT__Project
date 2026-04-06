using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject CraftUI;
    public GameObject button;

    public void CloseButton()
    {
        CraftUI.SetActive(false);
        button.SetActive(false);

        Debug.Log("Crafting UI closed.");
    }
}