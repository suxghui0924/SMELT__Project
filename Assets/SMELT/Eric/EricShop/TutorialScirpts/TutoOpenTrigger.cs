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
        public string name;

        [SerializeField] private GameObject skillTreeObject;


        public int count;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
                if(gameObject.name == "CraftingZone")
                {
                        if (!TutoManager.Instance.TutoSaying.canGetOrder1&&TutoManager.Instance.TutoSaying.canGetOrder2) return;
                        TutoManager.Instance.Triggered();
                        TutoManager.Instance.TutoSaying.canGetOrder2 = true;
                        StartCoroutine(
                                TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine
                                        .getOrder2));
                }


                if (gameObject.name == "SkillTreeZone")
                {
                        TutoManager.Instance.Triggered();
                        StartCoroutine(
                                TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine
                                        .store2));
                }

        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {

                    if (Keyboard.current.eKey.wasPressedThisFrame)
                    {



                            if (name == "SkillTreeZone")
                            {
                                    StartCoroutine(
                                            TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine
                                                    .store3));
                            }
            }
    }
    
    

    private void Start()
    {
            count = InventoryManager.Instance._inventory.Count;
    }




    private void Update()
    {                   
            if (TutoManager.Instance.TutoSaying.weaponSold)
            {
                    TutoManager.Instance.Triggered();
                    TutoManager.Instance.Dialogue.canMakePoint = true;

                    StartCoroutine(
                            TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine
                                    .sellWeapon2));
                    string itemId = InventoryManager.Instance._inventory.First().Key;
                    TutoManager.Instance.TutoSaying.weaponSold=false;
                    InventoryManager.Instance.RemoveItem(itemId);
                    skillTreeObject.SetActive(true);
                    Destroy(transform.parent.gameObject);
                    canSell = false;
            }

            if (InventoryManager.Instance._inventory.Count > count&&!isMadeFirst)
            {
                    isMade = true;
                    isMadeFirst = true;
            }

            if (TutoManager.Instance.TutoSaying.weaponCrafted)
            {
                    TutoManager.Instance.Dialogue.canMakePoint = true;
                    StartCoroutine(
                            TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine
                                    .sellWeapon1));
                    TutoManager.Instance.TutoSaying.weaponCrafted = false;
                    canSell = true;
                    isMade = false;
                    isMade = false;
            }
    }
}
