using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _01_Scripts.Interface.Radio
{
    public class RadioController : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown dropdown;
        private string[] songNames = {"game", "game2", "game3"};        
        private void OnEnable()
        {   
            dropdown.onValueChanged.AddListener(HandleValueChagned);
        }

        private void OnDisable()
        {
            dropdown.onValueChanged.RemoveListener(HandleValueChagned);
        }
        
        private void HandleValueChagned(int arg0)
        {
            SoundManager.instance.PlayBGM(songNames[arg0]);
        }

        
        
        public void OnButtonClose()
        {
            UICanvasManager.instance.ControlObject(ObjectType.Radio,false);
        }
    }
}