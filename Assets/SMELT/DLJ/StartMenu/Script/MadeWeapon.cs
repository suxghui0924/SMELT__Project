using System.Collections;
using UnityEngine;

public class MadeWeapon : MonoBehaviour
{
    public SparkParticle SparkParticle;
    public GameObject[] weaponsPrefab;

    private void Update()
    {
        if (SparkParticle.amount == 3)
        {
            CreateWeapon();
        } 
    }
    IEnumerator CreateWeapon()
    {
        GameObject CreatedNow = weaponsPrefab[Random.Range(0, weaponsPrefab.Length - 1)];
        CreatedNow.SetActive(true);
        yield return new WaitForSeconds(1);
        CreatedNow.SetActive(false);
    }
}
