using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace _01_Scripts.Player.Manager
{
    public class NPCManager : MonoBehaviour
    {
        public static NPCManager Instance;
        [SerializeField] private GameObject _context;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if(scene.name == "House")
                SetChildrenActive(true);
            else if(scene.name == "Lobby")
                SetChildrenDestroy();
            else
                SetChildrenActive(false);
        }

        private void SetChildrenActive(bool active)
        {
            Debug.Assert(_context != null, "Context cannot be null.");
            for (int _ = 0; _ < _context.transform.childCount ; _++)
            {
                GameObject child = _context.transform.GetChild(_).gameObject;
                Debug.Assert(child != null, "_context.transform.GetChild(_) != null");
                child.SetActive(active);
            }
        }
        
        private void SetChildrenDestroy()
        {
            Debug.Assert(_context != null, "Context cannot be null.");
            for (int _ = 0; _ < _context.transform.childCount ; _++)
            {
                GameObject child = _context.transform.GetChild(_).gameObject;
                Debug.Assert(child != null, "_context.transform.GetChild(_) != null");
                Destroy(child);
            }
        }
        
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}