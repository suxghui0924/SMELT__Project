using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _01_Scripts.Interface.Radio
{
    public class RadioController : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown dropdown;
        [SerializeField] private Slider _progressSlider;
        [SerializeField] private TextMeshProUGUI _timeText;

        private string[] songNames = {"game", "game2", "game3"};

        private void Start()
        {
            if (_progressSlider != null && _progressSlider.fillRect != null)
            {
                var fill = _progressSlider.fillRect.GetComponent<Image>();
                if (fill != null) fill.color = new Color(1f, 0.85f, 0f);
            }
        }

        private void OnEnable()
        {
            dropdown.onValueChanged.AddListener(HandleValueChagned);
        }

        private void OnDisable()
        {
            dropdown.onValueChanged.RemoveListener(HandleValueChagned);
        }

        private void Update()
        {
            if (SoundManager.instance == null) return;

            if (_progressSlider != null)
                _progressSlider.value = SoundManager.instance.BgmProgress;

            if (_timeText != null)
            {
                float cur   = SoundManager.instance.BgmCurrentTime;
                float total = SoundManager.instance.BgmDuration;
                _timeText.text = $"{FormatTime(cur)} / {FormatTime(total)}";
            }
        }

        private void HandleValueChagned(int arg0)
        {
            SoundManager.instance.SetHouseSong(songNames[arg0]);
            SoundManager.instance.PlayBGM(SoundManager.instance.currentHouseSong);
        }

        public void OnButtonClose()
        {
            UICanvasManager.instance.ControlObject(ObjectType.Radio,false);
        }

        private static string FormatTime(float seconds)
        {
            int m = Mathf.FloorToInt(seconds / 60);
            int s = Mathf.FloorToInt(seconds % 60);
            return $"{m}:{s:00}";
        }
    }
}