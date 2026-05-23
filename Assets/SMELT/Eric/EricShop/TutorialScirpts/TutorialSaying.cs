using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialSaying : MonoBehaviour
{
        [SerializeField] private float typingSpeed;
        [SerializeField] private string characterName;
        [SerializeField] private float delayBetweenLines;
        [SerializeField] private string[] lines;
        
        public bool tuRAttack;
        public bool tuLAttack;

        private void Start()
        {
                StartCoroutine(SayingCoroutine(TutoManager.Instance.TutoLine.start));
        }

        public IEnumerator SayingCoroutine(string[] texts)
        {
                TutoManager.Instance.Dialogue.canMove = false;
                Time.timeScale = 0f;
                yield return new WaitForSecondsRealtime(0.5f);
                if(TutoManager.Instance.TutoLine.dungeon3 == texts)
                {
                        tuRAttack = true;
                }
                if(TutoManager.Instance.TutoLine.dungeon4 == texts)
                {
                        tuLAttack = true;
                }
                lines = texts;
                TutoManager.Instance.Dialogue.typingSpeed = typingSpeed;
                TutoManager.Instance.Dialogue.Say(lines, characterName, delayBetweenLines);
        }

        private void Update()
        {
                if (Input.GetMouseButtonDown(0))
                {
                        TutoManager.Instance.Dialogue.Skip();
                }

                if(tuRAttack)
                {
                        if (Keyboard.current.dKey.wasPressedThisFrame)
                        {
                                TutoManager.Instance.Dialogue.canMove = true;
                                tuRAttack = false;
                                TutoManager.Instance.Dialogue.Skip();
                        }
                }
                if(tuLAttack)
                {
                        if (Keyboard.current.aKey.wasPressedThisFrame)
                        {
                                TutoManager.Instance.Dialogue.canMove = true;
                                tuLAttack = false;
                                TutoManager.Instance.Dialogue.Skip();
                        }      
                }
        }
}
