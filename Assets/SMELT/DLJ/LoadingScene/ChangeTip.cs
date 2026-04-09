using TMPro;
using UnityEngine;

public class ChangeTip : MonoBehaviour
{
    private TextMeshProUGUI textmesh;
    private void Awake()
    {
        textmesh = GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        int chooseTip = Random.Range(0, 5);
        if (chooseTip == 0)
        {
            textmesh.text = "TIP - 과일석은 종류마다 다양한 기믹이 있습니다.";
        }
        else if (chooseTip ==1)
        {
            textmesh.text = "TIP - 저희 동아리 이름은 밀랍칠한 약간 녹슨 깎인 구리 반블럭입니다.";
        }
        else if (chooseTip == 2)
        {
            textmesh.text = "TIP - 곡괭이를 업그레이드하면 공격속도가 빨라집니다.";
        }
        else if (chooseTip == 3)
        {
            textmesh.text = "TIP - 스킬 트리를 잘 분배해서 찍으면 기분이 좋습니다.";
        }
        else if (chooseTip == 4)
        {
            textmesh.text = "TIP - 밀랍칠한 약간 녹슨 깎인 구리 반블럭은 최강입니다";
        }
    }
}
