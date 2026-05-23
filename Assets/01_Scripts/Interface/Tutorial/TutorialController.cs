using _01_Scripts._Core._States;
using UnityEngine;

namespace _01_Scripts.Interface.Tutorial
{
    public class TutorialController : MonoBehaviour
    {
        public void OnMouseOpenClick()
        {
            GameManager.instance.ChangeState(new TutorialState());
        }

        public void OnMouseCloseClick()
        {
            UICanvasManager.instance.ControlObject(ObjectType.Tutorial, false);
        }
    }
}