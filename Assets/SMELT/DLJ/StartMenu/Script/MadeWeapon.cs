using System.Collections;
using UnityEngine;

public class MadeWeapon : MonoBehaviour
{
    public SparkParticle SparkParticle;
    public StarParticle starParticle;
    public GameObject[] weaponsPrefab;

    public bool isCreating = false;

    private void Update()
    {
        if (SparkParticle.amount == 3 && isCreating != true)
        {
            StartCoroutine(CreateWeapon());
            StartCoroutine(starParticle.PlayParticle());

        } 
    }
    IEnumerator CreateWeapon()
    {
        isCreating = true;
        yield return new WaitForSeconds(2);
        GameObject CreatedNow = Instantiate(weaponsPrefab[Random.Range(0, weaponsPrefab.Length)]);
        Debug.Log(CreatedNow);
        yield return new WaitForSeconds(1.3f);
        isCreating = false;
        Destroy(CreatedNow);
    }
}
