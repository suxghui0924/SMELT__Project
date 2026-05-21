using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace _01_Scripts.Player
{
    public class SaveMiningController : MonoBehaviour
    {
        [SerializeField] private Button _saveButton;

        private void Start()
        {
            _saveButton.onClick.AddListener(HandleSaveButtonClicked);
        }

        private void OnEnable()
        {
            OnMiningDataLoad();
        }

        private void HandleSaveButtonClicked()
        {
            SaveManager.Instance.Save();
        }
        private void OnMiningDataLoad()
        {
            SaveManager.Instance.Load();
        }

    }
}