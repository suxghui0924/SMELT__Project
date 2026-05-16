using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using Random = UnityEngine.Random;

public class BossSkillManager : MonoBehaviour
{
    public static BossSkillManager Instance;

    [SerializeField] private skillSystem _skillSystem;
    public DrowOre drawOre;
    public pressjuice _prejuice;
    public BossJump _bossJump;
    private CinemachineImpulseSource _impulseSource;
    public float timer = 0;
    int skillNum = 0;
    private bool db = false;
    
    
    private void Awake()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        Instance = this;
    }
    private void Start()
    {
        timer = 3f;

    }

    private void FixedUpdate()
    {
        
        StartCoroutine(Manager());
    }

    private IEnumerator Manager()
    {
       
        
        if (db)
        {
            for (int i = 0; i < 1; i++)
            {
                _impulseSource.GenerateImpulseWithVelocity(new Vector3(0, 0.3f, 0));
                yield return new WaitForSeconds(0.01f);
                _impulseSource.GenerateImpulseWithVelocity(new Vector3(0, -0.3f, 0));
            }
   
        }
        
        timer -= Time.deltaTime;

        if (timer < 0 && !db)
        {
            timer = 3f;
            yield return new WaitForSeconds(2f);
            Skills(Random.Range(0, 3));
        }
    }
    private void Skills(int skill)
    {
        if (_skillSystem.IsAlive == false) return;
        StartCoroutine(SkillDelay(skill));
    }

    IEnumerator SkillDelay(int skill)
    {
        if (skill == 0)
        {
            db = true;
            yield return new WaitForSeconds(0.5f);

            StartCoroutine(drawOre.SpawnOre());
            db = false;
        }
        else if (skill == 1)
        {
            
            db = true;

            yield return new WaitForSeconds(1f);
            

            _prejuice.SpawnJuice();
            db = false;
            
        }
        else if (skill == 2)
        {
           
            db = true;

            yield return new WaitForSeconds(2f);
            
            StartCoroutine(_bossJump.JumpTo());
            db = false;
        }
    }
}
