using System;
using System.Collections;
using System.Collections.Generic;
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

        animController.SetTrigger("Disappear");
        if ( TutoManager.Instance.TutoSaying.canGetOrder2)
        {
            TutoManager.Instance.TutoSaying.canGetOrder2 = false;
        }
        if (!TutoManager.Instance.TutoSaying.tuRAttack && !TutoManager.Instance.TutoSaying.tuLAttack)
        {
            canMove = true;
        }
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
                animController.SetTrigger("Disappear");
                if ( TutoManager.Instance.TutoSaying.canGetOrder2)
                {
                    TutoManager.Instance.TutoSaying.canGetOrder2 = false;
                }
                if (!TutoManager.Instance.TutoSaying.tuRAttack && !TutoManager.Instance.TutoSaying.tuLAttack)
                {
                    canMove = true;
                }
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