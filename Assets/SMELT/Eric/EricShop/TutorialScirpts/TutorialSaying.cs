using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialSaying : MonoBehaviour
{
        public Dialogue dialogue;
        [SerializeField] private float typingSpeed;
        [SerializeField] private string characterName;
        [SerializeField] private float delayBetweenLines;
        [SerializeField] private string[] lines;
        [SerializeField] private bool attack;

        private void Start()
        {
                StartCoroutine(SayingCoroutine(TutorialLine.Instance.dungeon3));
        }

        public IEnumerator SayingCoroutine(string[] texts)
        {
                dialogue.canMove = false;
      
                yield return new WaitForSecondsRealtime(0.5f);
                Time.timeScale = 0f;        
                if(TutorialLine.Instance.dungeon3 == texts)
                {
                        attack = true;
                }
                lines = texts;
                dialogue.typingSpeed = typingSpeed;
                dialogue.Say(lines, characterName, delayBetweenLines);
        }

        private void Update()
        {
                if (Input.GetMouseButtonDown(0))
                {
                        dialogue.Skip();
                }

                if(attack)
                {
                        if (Keyboard.current.dKey.wasPressedThisFrame)
                        {
                                dialogue.canMove = true;
                                attack = false;
                                dialogue.Skip();
                        }
                }
        }
}
