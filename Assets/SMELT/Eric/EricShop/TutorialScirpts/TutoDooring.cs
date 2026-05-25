using System.Threading.Tasks;
using UnityEngine;

public class TutoDooring : MonoBehaviour
{

        [SerializeField] private GameObject door;

        [SerializeField] private bool canDisable;

        private void OnEnable()
        {
                canDisable = true;
        }

        private async void OnDisable()
        {
                // 1. 유니티가 현재 오브젝트를 비활성화하는 작업을 완전히 끝낼 때까지 대기합니다. (무한 루프 방지)
                await Task.Yield(); 

                // 2. 대기가 끝난 후, 상위 오브젝트가 존재하고 켜져 있다면 그때 끕니다.
                if (door != null && door.activeSelf)
                {
                        door.SetActive(false);
                }
        }
}
