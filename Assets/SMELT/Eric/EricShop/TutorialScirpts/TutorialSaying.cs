using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSaying : MonoBehaviour
{
        public Dialogue dialogue;
        [SerializeField] private float typingSpeed;
        [SerializeField] private string characterName;
        [SerializeField] private float delayBetweenLines;
        [SerializeField] private string[] lines;

        private void Start()
        {
                StartCoroutine(SayingCoroutine(TutorialLine.Instance.enterDungeon1));
        }

        public IEnumerator SayingCoroutine(string[] texts)
        {
                yield return new WaitForSecondsRealtime(0.5f);
                Time.timeScale = 0f;
                lines = texts;
                dialogue.typingSpeed = typingSpeed;
                dialogue.Say(lines, characterName, delayBetweenLines);
        }
}
