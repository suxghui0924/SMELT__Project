using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class StarParticle : MonoBehaviour
{
    public ParticleSystem ParticleSystem;

    public IEnumerator PlayParticle()
    {
        yield return new WaitForSeconds(2);
        ParticleSystem.Play();
    }
}
