using UnityEngine;

namespace _01_Scripts.Player.Portal
{
    public class PortalSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _portalObject;
        private void OnEnable()
        {
            if (InventoryManager.Instance == null) return;
            InventoryManager.Instance.OnDayChanged += HandleDayChanged;
            HandleDayChanged(InventoryManager.Instance.CurrentDay, InventoryManager.Instance.MaintenanceCost);
        }

        private void Start()
        {
            // OnEnable 시점에 InventoryManager가 없었을 경우 여기서 재시도
            if (InventoryManager.Instance == null) return;
            InventoryManager.Instance.OnDayChanged -= HandleDayChanged; // 중복 방지
            InventoryManager.Instance.OnDayChanged += HandleDayChanged;
            HandleDayChanged(InventoryManager.Instance.CurrentDay, InventoryManager.Instance.MaintenanceCost);
        }

        private void OnDisable()
        {
            if (InventoryManager.Instance == null) return;
            InventoryManager.Instance.OnDayChanged -= HandleDayChanged;
        }
        
        private void HandleDayChanged(int arg1, ulong arg2)
        {
            if (arg1 >= 7)
            {
                _portalObject.SetActive(true);
            }
        }
    }
}