using UnityEngine;

public class TutoExitDungeon : MonoBehaviour
{
    [SerializeField] private GameObject[] dungeon;
    [SerializeField] private GameObject[] house;

    public void ExitDungeon()
    {
        if (!TutoManager.Instance.TutoSaying.canExitDungeon) return;
        dungeon[0].SetActive(false);
        dungeon[1].SetActive(false);
        house[0].SetActive(true);
        house[1].SetActive(true);
    }
}
