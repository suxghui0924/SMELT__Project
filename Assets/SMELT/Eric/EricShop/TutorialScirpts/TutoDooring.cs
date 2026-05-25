using System;
using UnityEngine;

public class TutoDooring : MonoBehaviour
{
        private void OnDisable()
        {
                transform.parent.gameObject.SetActive(false);
        }
}
