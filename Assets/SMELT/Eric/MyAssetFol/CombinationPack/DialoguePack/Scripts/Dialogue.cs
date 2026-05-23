using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


// IMPORTANT Public METHODS and their use

// Say() can take a string or string array, and proceeds to print it one character at a time. It can also take a character name, and a line delay.
// Skip() skips to the end of the line and then running skip again with 1 second of the first takes you to the next line in the index. 
// Clear() clears the stored data and closes the text box. 


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
            }

        }

    }

    IEnumerator TypeMany()
    {
        for (int i = 0; i < sentences.Length+1; i++)
        {
            
            if (index < sentences.Length)
            {
                StartCoroutine(Type());
            }            
            yield return new WaitForSecondsRealtime(duration);
            if (index < sentences.Length)
            {
                index++;
            }
            else
            {
                
                //Debug.Log("Should Shrink");
                animController.SetTrigger("Disappear");
            }
        }
    }

    private void Update()
    {
                if(canMove)  Time.timeScale = 1f;
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
        animController.SetTrigger("Appear");
        string[] phrase = { _text};
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
        canMove = false;
        animController.SetTrigger("Appear");
        sentences = _text;
        index = 0;
        if (_duration>0)
        {
            duration = _duration;
        }
        UpdateName(_characterName);
        StartCoroutine(TypeMany());
    }


    public void Skip()
    {
        if (index < sentences.Length-1)
        {
            if (active)
            {
                active = false;
                StartCoroutine(SkipInvokeRoutine());
            }
            else
            {
                index++;
                StartCoroutine(Type());
                StartCoroutine(SkipInvokeRoutine());
            }
        }
        else
        {
            //Debug.Log("Should Shrink");
            animController.SetTrigger("Disappear");
        }
           
    }
    
    private IEnumerator SkipInvokeRoutine()
    {
        yield return new WaitForSecondsRealtime(1f);
        active = true;
    }

    public void Clear()
    {
        animController.SetTrigger("Disappear");
        sentences = null;
        UpdateName();
        textBox.text = "";
    }
}



