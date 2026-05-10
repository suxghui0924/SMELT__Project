using UnityEngine;

namespace _01_Scripts.Interface.UserInput
{
    public class ClickerSound : MonoBehaviour
    {
        public void OnButtonClick()
        {
            SoundManager.instance.PlaySFX("UIClick");
        }
    }
}