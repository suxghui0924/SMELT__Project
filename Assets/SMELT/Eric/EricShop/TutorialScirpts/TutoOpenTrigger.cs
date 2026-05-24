using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutoOpenTrigger : MonoBehaviour
{
        public bool canSell = false;
        public bool isMade = false;
        public bool isMadeFirst = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
                if(gameObject.name == "CraftingZone")
                {
                        if (!TutoManager.Instance.TutoSaying.canGetOrder1) return;
                        TutoManager.Instance.TutoSaying.canGetOrder2 = true;
                        StartCoroutine(
                                TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine
                                        .getOrder2));
                }

                if (gameObject.name == "NextDayZone")
                {
                        StartCoroutine(
                                TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine
                                        .nextDay2));
                }

                if (gameObject.name == "SkillTreeZone")
                {
                        StartCoroutine(
                                TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine
                                        .store2));
                }

                if (gameObject.name == "StoreRadioZone")
                {
                        StartCoroutine(
                                TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine
                                        .changeBgm2));
                }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
            if (other.CompareTag("Player"))
            {
                    if (Keyboard.current.eKey.wasPressedThisFrame)
                    {
                            if (gameObject.name == "TriggerColl"&&canSell)
                            {
                                    StartCoroutine(
                                            TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine
                                                    .sellWeapon2));
                                    string itemId = InventoryManager.Instance._inventory.First().Key;
                                    InventoryManager.Instance.RemoveItem(itemId);
                                    Destroy(transform.parent.gameObject);
                                    canSell = false;
                            }

                            if (gameObject.name == "DoorZone")
                            {
                            }
                    }
            }
    }

    private IEnumerator DoorCoroutine()
    {
            
            StartCoroutine(
                    TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine
                            .openDoor1));
            yield return new WaitForSeconds(1.5f);
            
            StartCoroutine(
                    TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine
                            .getOrder1));
    }

    private void Update()
    {
            if (InventoryManager.Instance._inventory != null&&!isMadeFirst)
            {
                    isMade = true;
                    isMadeFirst = true;
            }

            if (isMade)
            {
                    StartCoroutine(
                            TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine
                                    .sellWeapon1));

                    canSell = true;
                    isMade = false;
            }
    }
}
