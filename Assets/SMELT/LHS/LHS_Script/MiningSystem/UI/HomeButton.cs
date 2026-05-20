using _01_Scripts._Core._States;
using UnityEngine;

public class HomeButton : MonoBehaviour
{
    public void OnClickHomeButton()
    {
        GameManager.instance.ChangeState(new HouseState());
    }
}
