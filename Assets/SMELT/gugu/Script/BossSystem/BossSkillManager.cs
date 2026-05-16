using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Collections;

public class BossSkillManager : MonoBehaviour
{
    public static BossSkillManager Instance;

    public DrowOre drawOre;
    public pressjuice _prejuice;
    public BossJump _bossJump;
    public float timer = 0;
    int skillNum = 0;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        timer = 3f;

    }
    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            timer = 3f;

            Skills(Random.Range(0, 3));
        }
    }
    private void Skills(int skill)
    {
        StartCoroutine(SkillDelay(skill));
    }

    IEnumerator SkillDelay(int skill)
    {
        if (skill == 0)
        {
            StartCoroutine(CameraShake.Instance.Shake(0.3f, 0.2f));

           

            StartCoroutine(drawOre.SpawnOre());
        }
        else if (skill == 1)
        {
            StartCoroutine(CameraShake.Instance.Shake(0.7f, 0.4f));

            yield return new WaitForSeconds(1f);

            _prejuice.SpawnJuice();
        }
        else if (skill == 2)
        {
            StartCoroutine(CameraShake.Instance.Shake(1f, 0.5f));

            yield return new WaitForSeconds(0.2f);

            StartCoroutine(_bossJump.JumpTo());
        }
    }
}
