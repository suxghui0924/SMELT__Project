using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Triggering : MonoBehaviour
{
        public bool door = false;
        [SerializeField] private GameObject doors;
        [SerializeField] private Collider2D coll;
        
        private void AddOre()
        {
                var inv = InventoryManager.Instance;

                inv.AddGold(100_000_000);
                inv.AddItem("fruitstone_apple", 999);
                inv.AddItem("fruitstone_melon", 999);
                inv.AddItem("fruitstone_orange", 999);
                inv.AddItem("fruitstone_lemon", 999);
                inv.AddItem("fruitstone_grape", 999);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
                if(other.CompareTag("Player"))
                {
                        if (gameObject.name == "123")
                        {
                                doors.SetActive(true);
                                gameObject.SetActive(false);
                                return;
                        }

                        if (name == "DoorZone")
                        {
                                door = true;
                        }
                }


        }

        private void Update()
        {
                Debug.Log(name);

                if (door)
                {
                        Leedoyun_SellManager.Instance.ForceSpawnTutorialOrder(WeaponType.Sword, "fruitstone_apple");
                        TutoManager.Instance.Triggered();
                        door = false;
                        TutoManager.Instance.Dialogue.canMakePoint = true;
                        StartCoroutine(
                                TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine
                                        .getOrder1));
                        AddOre();
                        coll.enabled = false;
                }
        }
}
