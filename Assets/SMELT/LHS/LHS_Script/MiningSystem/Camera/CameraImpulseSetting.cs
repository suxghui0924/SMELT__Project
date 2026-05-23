using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class CameraImpulseSetting : MonoBehaviour
{
   public static Action OnEnemyHit;

   [SerializeField] private CinemachineImpulseSource impulseSource;
   
   public static void EnemyHit()
   {
      OnEnemyHit?.Invoke();
   }

   private void OnEnable()
   {
      OnEnemyHit+=OnHit;
   }

   private void OnDisable()
   {
      OnEnemyHit-=OnHit;
   }
   private void OnHit()
   {
      impulseSource.GenerateImpulse();
   }
}
