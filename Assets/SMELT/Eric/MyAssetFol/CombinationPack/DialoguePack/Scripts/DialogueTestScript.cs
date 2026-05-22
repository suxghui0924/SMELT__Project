using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTestScript : MonoBehaviour
{
    public Dialogue dialogue;
    public string characterName;
    public string singleLine;
    public string[] multipleLines;
    public float delayBetweenLines = 4;
    public float typingSpeed = 0.02f;

    private void Start()
    {
        Invoke("Test", .5f);
    }

    public void Test()
    {
        dialogue.typingSpeed = typingSpeed;
        if (multipleLines.Length == 0)
        {
            dialogue.Say(singleLine, characterName, delayBetweenLines);
        }
        else
        {
            dialogue.Say(multipleLines, characterName, delayBetweenLines);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dialogue.Skip();
        }
        if (Input.GetMouseButtonDown(1))
        {
            dialogue.Clear();
        }
    }
}
