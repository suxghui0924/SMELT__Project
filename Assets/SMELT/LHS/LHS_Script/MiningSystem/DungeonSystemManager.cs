using System;
using UnityEngine;

public class DungeonSystemManager : MonoBehaviour
{
   /// <summary>
   /// 피로도 0이 됐을 때, 게임 내 획득한 쟤료 수치, 남은 피로도 등을 관리하는 매니저입니다
   /// </summary>
   public static DungeonSystemManager instance;
   
   
   private void Awake()
   {
      if(instance == null)
      instance = this;
   }

   public void OnDead()
   {
      GameManager.instance.ChangeState(new GameOverState());
   }
   
}
