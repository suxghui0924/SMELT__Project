using UnityEngine;

public class BossSceneHomeButton : MonoBehaviour
{
    
    public void OnClickHomeButton()
    {
        GameManager.instance.ChangeState(new HouseState());
    }
}
