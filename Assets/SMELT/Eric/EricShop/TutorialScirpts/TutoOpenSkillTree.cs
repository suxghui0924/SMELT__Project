using System;
using UnityEngine;

public class TutoOpenSkillTree : MonoBehaviour
{
    [SerializeField] private bool tree = true;

    private void Update()
    {
        if (TutoManager.Instance.skillTreeUI == null) return;
        if (TutoManager.Instance.skillTreeUI.activeSelf&&tree)
        {
            tree = false;
         StartCoroutine (TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine.store3));
        }
    }
}
