using System;
using System.Collections;
using System.Collections.Generic;
using _01_Scripts._Core._States;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    [Header("Setup")]
    public TextMeshProUGUI textBox;
    public TextMeshProUGUI nameBox;
    public Animator animController;

    [Header("Input")]
    [HideInInspector]
    public string[] sentences;
    private int index;
    public float typingSpeed = 0.02f;
    public float duration = 4f;
    private bool active = true;
    
    
    public bool canMove = true;
    public bool last = false;
    public int pointsNum = 0;
    public bool canMakePoint =false;

    public bool canDo = true;
    public bool next=true;
    public GameObject homeBtn;

    private void Dissapear()
    {
        Check();
        animController.SetTrigger("Disappear");
        if (TutoManager.Instance.TutoSaying.canGetOrder2)  TutoManager.Instance.TutoSaying.canGetOrder2 = false;
        if (!TutoManager.Instance.TutoSaying.tuRAttack && !TutoManager.Instance.TutoSaying.tuLAttack)  canMove = true;
        if (last)
        {
            last = false;
            TutoManager.Instance.canLast = true;
        }
        if(TutoManager.Instance.TutoLine.dungeon3 == TutoManager.Instance.TutoSaying.str&&TutoManager.Instance.TutoSaying.dungeon.activeSelf&&canDo)
        {
            canDo = false;
            TutoManager.Instance.TutoSaying.tuRAttack = true;
        }      
        if(TutoManager.Instance.TutoLine.dungeon5 == TutoManager.Instance.TutoSaying.str)
        {
                  TutoManager.Instance.TutoSaying.canExitDungeon = true;
            
        }
        if(TutoManager.Instance.TutoLine.dungeon1 == TutoManager.Instance.TutoSaying.str&&TutoManager.Instance.TutoSaying.dungeon.activeSelf)
        {
            StartCoroutine(TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine.dungeon3));
        }
        if(TutoManager.Instance.TutoLine.store4 == TutoManager.Instance.TutoSaying.str)
        {
            TutoManager.Instance.Triggered();
            TutoManager.Instance.Dialogue.canMakePoint = true;
            StartCoroutine(TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine.changeBgm2));

        }     
        if(TutoManager.Instance.TutoLine.changeBgm2 == TutoManager.Instance.TutoSaying.str)
        {
            TutoManager.Instance.Triggered();
            TutoManager.Instance.Dialogue.canMakePoint = true;
            StartCoroutine(TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine.nextDay2));

        }
        if(TutoManager.Instance.TutoLine.nextDay2 == TutoManager.Instance.TutoSaying.str)
        {
            TutoManager.Instance.Triggered();
            StartCoroutine(TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine.last));

        }
        if(TutoManager.Instance.TutoLine.last == TutoManager.Instance.TutoSaying.str&&next)
        {
            next = false;
            GameManager.instance.ChangeState(new LobbyState());

        }
    }

    private void Check()
    {
        if (pointsNum >= 9||!canMakePoint) return;
        TutoManager.Instance.TargetPos(pointsNum);
        pointsNum++;
        canMakePoint = false;
    }
    
    IEnumerator Type()
    {
        animController.ResetTrigger("Disappear");
        textBox.text = "";
        
        foreach (var letter in sentences[index].ToCharArray())
        {
            if (active)
            {
                textBox.text += letter;
                yield return new WaitForSecondsRealtime(typingSpeed);
                animController.ResetTrigger("Appear");
            }
            else
            {
                textBox.text = sentences[index];
                break; 
            }
        }
        
        active = false; 
    }

    IEnumerator TypeMany()
    {
        while (index < sentences.Length)
        {
            yield return StartCoroutine(Type()); 
            
            yield return new WaitForSecondsRealtime(duration);
            index++;
        }

        Dissapear();

    }

    private void Update()
    {
        if(canMove) Time.timeScale = 1f;
    }

    //Character Socket
    public void UpdateName(string name = null)
    {
        if (name == null)
        {
            nameBox.gameObject.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            if (!nameBox.gameObject.transform.parent.gameObject.active)
            {
                nameBox.gameObject.transform.parent.gameObject.SetActive(true);
            }
            nameBox.text = name;
        }
    }

    public void Say(string _text, string _characterName = null, float _duration = 0)
    {
        StopAllCoroutines(); 
        active = true; 
        
        animController.SetTrigger("Appear");
        string[] phrase = { _text };
        sentences = phrase;

        index = 0;
        if (_duration > 0)
        {
            duration = _duration;
        }
        UpdateName(_characterName);
        StartCoroutine(Type());
    }

    public void Say(string[] _text, string _characterName = null, float _duration = 0)
    {
        StopAllCoroutines(); 
        active = true;  
        
        canMove = false;
        animController.SetTrigger("Appear");
        sentences = _text;
        index = 0;
        if (_duration > 0)
        {
            duration = _duration;
        }
        UpdateName(_characterName);
        StartCoroutine(TypeMany());
    }

    public void Skip()
    {
        if (sentences == null || sentences.Length == 0) return;

        if (active)
        {
            active = false; 
        }
        else
        {
            if (index < sentences.Length - 1)
            {
                index++;
                StopAllCoroutines(); 
                StartCoroutine(TypeMany());
            }
            else
            {
                StopAllCoroutines();
                Dissapear();
            }
        }
    }

    public void Clear()
    {
        StopAllCoroutines(); 
        animController.SetTrigger("Disappear");
        sentences = null;
        UpdateName();
        textBox.text = "";
    }
    
    
}