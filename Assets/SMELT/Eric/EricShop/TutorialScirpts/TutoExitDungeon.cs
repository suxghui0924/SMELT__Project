using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _01_Scripts.NPC;
using _01_Scripts.NPC.Data;

public class TutoExitDungeon : MonoBehaviour
{
    [SerializeField] private GameObject[] dungeon;
    [SerializeField] private GameObject[] house;

    private void Start()
    {
        if (house[2] == null) house[2] = GameObject.Find("Player");
        transform.parent.gameObject.SetActive(false);   
    }

    public void ExitDungeon()
    {
        if (!TutoManager.Instance.TutoSaying.canExitDungeon) return;
            UICanvasManager.instance.SetCanvasActive(CanvasType.Hud, true);
            
        TutoManager.Instance.TutoSaying.ui1 = true;
        dungeon[0].SetActive(false);
        dungeon[1].SetActive(false);
        dungeon[2].SetActive(false);
        dungeon[3].SetActive(false);
        house[0].SetActive(true);
        house[1].SetActive(true);
        house[2].SetActive(true);
        house[3].SetActive(true);
        house[4].SetActive(true);
        
        
    }
}
