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
    
    private Sequence _moveSequence;
    private bool db = true;
    
    #region OnEnable / Init
    /*void OnEnable()
    {
        if (objectShop.activeSelf)
            count = 2;
        else if (objectSkillTree.activeSelf)
            count = 1;
        else
            Debug.LogWarning("Not Found objectUI Or objectActiveSelf ");
    }*/
    #endregion
    #region Change Group Frame

    public void OnButton(GameObject prev)
    {
        if (db)
        {
            db = false;
            SoundManager.instance.PlaySFX("Slide");
        if (prev.name == buttonUp.name)
        {

            if (count == 1)
            {
                count = 2;
                objectSkillTree.transform.localPosition = new Vector2(0, -1000);
                DoTweenSequence(objectShop, 1000, objectSkillTree, 0);
            }
            else if (count == 2)
            {
                count = 1;
                objectShop.transform.localPosition = new Vector2(0, -1000);
                DoTweenSequence(objectShop, 0, objectSkillTree, 1000);
            }
        }
        else if (prev.name == buttonDown.name)
        {
            if (count == 1)
            {
                count = 2;
                objectSkillTree.transform.localPosition = new Vector2(0, 1000);
                DoTweenSequence(objectShop, -1000, objectSkillTree, 0);
            }
            else if (count == 2)
            {
                count = 1;
                objectShop.transform.localPosition = new Vector2(0, 1000);
                DoTweenSequence(objectShop, 0, objectSkillTree, -1000);
            }
        }
        else
        {
            Debug.LogWarning("Not Found object Compare other");
        }
    }
}
    #endregion
    
    #region DoTween

    private void DoTweenSequence(GameObject upToDowning, float upRange, GameObject downToDowning, float downRange)
    {
        _moveSequence?.Kill();

        _moveSequence = DOTween.Sequence()
            .Append(upToDowning.transform.DOLocalMoveY(upRange, 1).SetEase(Ease.InOutCubic))
            .Join(downToDowning.transform.DOLocalMoveY(downRange, 1).SetEase(Ease.InOutCubic))
            .SetLink(gameObject)
            .OnComplete(() => { db = true; });
    }
    
    #endregion
    
    #region closeUI

    public void OnCloseUI()
    {
        UICanvasManager.instance.ControlObject(ObjectType.ShopASkill, false);
    }
    #endregion
    
}