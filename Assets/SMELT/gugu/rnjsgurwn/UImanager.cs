using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject CraftUI;
    public GameObject closebutton;
    public GameObject Creaft;

    public void CloseButton()
    {
        CraftUI.SetActive(false);
        closebutton.SetActive(false);
        Creaft.SetActive(false);

        Debug.Log("Crafting UI closed.");
    }
}