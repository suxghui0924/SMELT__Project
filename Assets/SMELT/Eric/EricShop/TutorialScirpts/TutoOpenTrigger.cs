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
        public bool door = false;

        [SerializeField] private GameObject skillTreeObject;

        public int count;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
                if(gameObject.name == "CraftingZone")
                {
                        if (!TutoManager.Instance.TutoSaying.canGetOrder1&&TutoManager.Instance.TutoSaying.canGetOrder2) return;
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
                                    skillTreeObject.SetActive(true);
                                    Destroy(transform.parent.gameObject);
                                    canSell = false;
                            }

                            if (gameObject.name == "DoorZone")
                            {
                                    door = true;
                            }
                            if (gameObject.name == "SkillTreeZone")
                            {
                                    StartCoroutine(
                                            TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine
                                                    .store3));
                            }
                    }
            }
    }

    private void Start()
    {
            count = InventoryManager.Instance._inventory.Count;
    }

    private IEnumerator DoorCoroutine()
    {
            yield return null;
            
            StartCoroutine(
                    TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine
                            .getOrder1));
    }

    private void Update()
    {

            if (door)
            {
                    StartCoroutine(DoorCoroutine());
                    door = false;
            }
            if (InventoryManager.Instance._inventory.Count > count&&!isMadeFirst)
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
