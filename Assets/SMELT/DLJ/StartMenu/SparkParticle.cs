using System.Collections;
using UnityEngine;

public class SparkParticle : MonoBehaviour
{
    public ParticleSystem ParticleSystem;
    private void Awake()
    {
        ParticleSystem = GetComponent<ParticleSystem>();
    }
    private void Start()
    {
        StartCoroutine(PlayParticle());
    }

    IEnumerator PlayParticle()
    {
        while (true)
        {
            ParticleSystem.Play();
            yield return new WaitForSeconds(0.587f);
        }
    }
}
