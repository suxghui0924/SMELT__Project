using UnityEngine;

namespace _01_Scripts.Player.Portal
{
    public class PortalSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _portalObject;
        private void OnEnable()
        {
            InventoryManager.Instance.OnDayChanged += HandleDayChanged;
        }

        private void OnDisable()
        {
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