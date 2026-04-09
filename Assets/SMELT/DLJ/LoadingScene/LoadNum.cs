using System.Collections;
using TMPro;
using UnityEngine;

public class LoadNum : MonoBehaviour
{
    private TextMeshProUGUI textmesh;
    public int saveNum;
    private void Awake()
    {
        textmesh = GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        StartCoroutine(UpdateNum());
    }
    public IEnumerator UpdateNum()
    {
        int i = 0;
        while (saveNum <= 100)
        {
            int target = Random.Range(saveNum, 101);
            for (i = saveNum; i <= target; i++)
            {
                textmesh.text = $"로딩 중 ... ( {i} % )";
                yield return new WaitForSeconds(0.05f);
                saveNum = i;

            }
            if (saveNum > 100)
            {
                saveNum = 100;
            }
            yield return new WaitForSeconds(Random.Range(1, 2));
        }
    }
}
