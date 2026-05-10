using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private GameObject buttonUp;
    [SerializeField] private GameObject buttonDown;
    [SerializeField] private GameObject objectShop;
    [SerializeField] private GameObject objectSkillTree;

    [Range(1, 2)]
    [SerializeField] int count = 1;

    void OnEnable()
    {
        if (objectShop.activeSelf)
            count = 1;
        else if (objectSkillTree.activeSelf)
            count = 2;
        else
            Debug.LogWarning("Not Found objectUI Or objectActiveSelf ");
    }
    public void OnButton(GameObject prev)
    {
        if (prev.name == buttonUp.name)
        {
            if (count == 1)
            {
                count++;
                objectShop.SetActive(false);
                objectSkillTree.SetActive(true);
            }
            else if (count == 2)
            {
                count--;
                objectShop.SetActive(true);
                objectSkillTree.SetActive(false);
            }
        }
        else if (prev.name == buttonDown.name)
        {
            if (count == 1)
            {
                count = 2;
                objectShop.SetActive(false);
                objectSkillTree.SetActive(true);
            }
            else if (count == 2)
            {
                count--;
                objectShop.SetActive(true);
                objectSkillTree.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("Not Found object Compare other");
        }
    }
}