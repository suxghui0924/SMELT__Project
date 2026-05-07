using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private InputField _nameInput;    
    
    public void OnButton(GameObject prev, GameObject next)
    {
        prev.SetActive(false);
        next.SetActive(true);
    }
}