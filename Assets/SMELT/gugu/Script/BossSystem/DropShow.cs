using UnityEngine;
using System.Collections;
using DG.Tweening;
using Unity.Android.Types;
using Unity.Cinemachine;

public class DropShow : MonoBehaviour
{
    private bool shake = false;
    [SerializeField]private CinemachineImpulseSource _impulseSource;
    [SerializeField] private Transform target;
    
    Sequence mySequence;

    private void Start()
    {
        shake = true;
        
        if(gameObject.name == "Player")
            mySequence.Append(transform.DOMoveY(-2, 1.5f).SetEase(Ease.OutBounce));
        else
            mySequence.Append(transform.DOMoveY(target.position.y, 1.5f).SetEase(Ease.OutBounce));
    }
    private void FixedUpdate()
    {
        if (shake)
        {
            StartCoroutine(Managers());
        }
        
    }

    private IEnumerator Managers()
    {
        if (shake)
        {
            for (int i = 0; i < 1; i++)
            {
                _impulseSource.GenerateImpulseWithVelocity(new Vector3(0, 0.2f, 0));
                yield return new WaitForSeconds(0.01f);
                _impulseSource.GenerateImpulseWithVelocity(new Vector3(0, -0.2f, 0));
            }
            yield return  new WaitForSeconds(1.7f);
            shake = false;
        }
    }
}