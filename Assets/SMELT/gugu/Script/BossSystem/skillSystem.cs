using System;
using UnityEngine;
using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class skillSystem : MonoBehaviour
{
    
    [SerializeField]private bool isAlive = true;
    private float Damage = 10f;
    private bool shakes = false;
    public bool Weccs = false;
    public BossSkillManager _BSM;
    public bool CanbuyPickaxe;
    
    public bool IsAlive => isAlive;
    private StartBtn _startBtn;
    [SerializeField]private GameObject boss;
    [SerializeField]private GameObject orePrefab ;
    [SerializeField]private GameObject juicePrefab;
    [SerializeField]private CinemachineImpulseSource impulseSource;
    [SerializeField]private TextMeshProUGUI _text;
    [SerializeField] private Button homeBtn;
    [SerializeField] private Image homeBtnImage;
    private void Start()
    {
        _text.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (shakes)
        {
            StartCoroutine(Managers());
        }
        
    }

    private IEnumerator Managers()
    {
        if (shakes)
        {
            for (int i = 0; i < 1; i++)
            {
                impulseSource.GenerateImpulseWithVelocity(new Vector3(0, 0.5f, 0));
                yield return new WaitForSeconds(0.01f);
                impulseSource.GenerateImpulseWithVelocity(new Vector3(0, -0.5f, 0));
            }
            yield return  new WaitForSeconds(3f);
            shakes = false;
        }
    }
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAlive == false) return;
        
        ore oreScript = collision.GetComponent<ore>();
        juice juiceScript = collision.GetComponent<juice>();

        
        if (oreScript != null && oreScript.canHitBoss)
        {
            
            if (collision.gameObject.CompareTag("ore"))
            {
                BossSystem.Instance._Hp.fillAmount -= Damage / 200f;
                Destroy(collision.gameObject);

            }
        }

        if (juiceScript != null && juiceScript.canHitBoss)
        {
            if (collision.gameObject.CompareTag("juice"))
            {

                BossSystem.Instance._Hp.fillAmount -= Damage / 100f;
                Destroy(collision.gameObject);       
            
    
            }
        }
        
        

        if (BossSystem.Instance._Hp.fillAmount <= 0 )
        {
            DestroyAllSkills();
            Debug.Log("보스가 죽었습니다!");
            isAlive = false;
            StartCoroutine(PlayDeathAnimation());
        }
    }
   
    private IEnumerator PlayDeathAnimation()
    {

        shakes = true;
        BossSkillManager.Instance.stack = false;
        yield return new WaitForSeconds(3f);
        
        OnAnimationEnd();
    }
    public void OnAnimationEnd()
    {
        Destroy(boss);
        _text.gameObject.SetActive(true);   
        StartCoroutine(Managers());
        Debug.Log("5만 골드 지급, ??? 획득");
        InventoryManager.Instance.AddGold(50000);
        Debug.Log("지급 완료");
        Weccs = true;
        homeBtn.enabled = true;
        homeBtnImage.enabled = true;
        InventoryManager.Instance.AddGold(50000);
        if(!CanbuyPickaxe)PickaxeDataSend();
        
    }

    private void PickaxeDataSend()
    {
        CanbuyPickaxe = true;
    }
    private void DestroyAllSkills()
    {
        GameObject[] ores = GameObject.FindGameObjectsWithTag("ore");
        
        foreach (GameObject ore in ores)
        {
            Destroy(ore);
        }

        GameObject[] juices = GameObject.FindGameObjectsWithTag("juice");

        foreach (GameObject juice in juices)
        {
            Destroy(juice);
        }
    }
    
}
