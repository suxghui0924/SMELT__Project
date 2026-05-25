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
        public bool ui1;

        public bool canExitDungeon;
        public bool makeWeapon1 = true;
        public bool canGetOrder1 = true;
        public bool canGetOrder2 = true;

        public bool weaponCrafted;
        public bool weaponSold;

        public string[] str;

        public GameObject dungeon;

        private void OnEnable()
        {
                WeaponCraftManager.OnWeaponCrafted += HandleWeaponCrafted;
        }

        private void OnDisable()
        {
                WeaponCraftManager.OnWeaponCrafted -= HandleWeaponCrafted;
                if (Leedoyun_SellManager.Instance != null)
                        Leedoyun_SellManager.Instance.OnOrderFulfilled -= HandleOrderFulfilled;
        }

        private void HandleWeaponCrafted(string weaponItemId)
        {
                weaponCrafted = true;
        }

        private void HandleOrderFulfilled(Leedoyun_CustomerOrder order, int gold)
        {
                weaponSold = true;
        }

        private void Start()
        {
                TutoManager.Instance.Dialogue.canMakePoint = true;
                StartCoroutine(SayingCoroutine(TutoManager.Instance.TutoLine.start));
                if (Leedoyun_SellManager.Instance != null)
                        Leedoyun_SellManager.Instance.OnOrderFulfilled += HandleOrderFulfilled;
        }

        public IEnumerator SayingCoroutine(string[] texts)
        {
                str = texts;
                TutoManager.Instance.Dialogue.canMove = false;
                Time.timeScale = 0f;

                if (texts == TutoManager.Instance.TutoLine.store4 ||
                    texts == TutoManager.Instance.TutoLine.changeBgm2 ||
                    texts == TutoManager.Instance.TutoLine.nextDay2)
                        TutoManager.Instance.Dialogue.last = true;
                yield return new WaitForSecondsRealtime(0.5f);

                lines = texts;
                TutoManager.Instance.Dialogue.typingSpeed = typingSpeed;
                TutoManager.Instance.Dialogue.Say(lines, characterName, delayBetweenLines);
        }

        private void Update()
        {
                if (ui1)
                {
                        TutoManager.Instance.Triggered();
                        ui1 = false;
                        TutoManager.Instance.Dialogue.canMakePoint = true;
                        StartCoroutine(
                                TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine.ui1));
                }
                if (Input.GetMouseButtonDown(0))
                {
                        TutoManager.Instance.Dialogue.Skip();
                        
                        TutoManager.Instance.Dialogue.canMove = true;
                }

                if (Keyboard.current.aKey.wasPressedThisFrame&&tuRAttack||Keyboard.current.dKey.wasPressedThisFrame&&tuRAttack)
                {
                        TutoManager.Instance.Triggered();
                        tuRAttack = false;
                        TutoManager.Instance.Dialogue.canMove = true;
                        TutoManager.Instance.Dialogue.Skip();
                        TutoManager.Instance.Dialogue.canMakePoint = true;
                        StartCoroutine(
                                TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine
                                        .dungeon5));
                }
        }
}
