using System.Collections;
using UnityEngine;

public class SparkParticle : MonoBehaviour
{
    public ParticleSystem ParticleSystem;
    public Animator Animator;
    public int amount = 0;

    private Coroutine _coroutine;

    private void Awake()
    {
        ParticleSystem = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        _coroutine = StartCoroutine(PlayParticle());
    }

    IEnumerator PlayParticle()
    {
        while (amount < 3)
        {
            ParticleSystem.Play();
            amount++;
            yield return new WaitForSeconds(1.32f);
        }

        ParticleSystem.Stop();
        Animator.SetInteger("Smithing", amount);
        amount = 0;
        yield return new WaitForSeconds(2);
        Animator.SetInteger("Smithing", amount);
        StartCoroutine(PlayParticle());
    }
}
