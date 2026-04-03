using NUnit.Framework;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BillboardUI : MonoBehaviour
{
    [SerializeField] private List<target_Match> m_Matches = new List<target_Match>();
    [SerializeField] private Camera m_Camera;

    public static BillboardUI Instance = null;

    private Canvas canvas;
    [Serializable]
    public class target_Match
    {
        public Transform target;
        public RectTransform ui;
        public float offset;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            canvas = GetComponent<Canvas>();
            m_Camera = Camera.main;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Init()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
           Destroy(gameObject);
        }
    }
    public void AddTarget(Transform target_object, RectTransform target_ui, float target_offset)
    {
        Image BillboardUI = target_ui.GetComponent<Image>();
        BillboardUI = Instantiate(BillboardUI, canvas.transform);

        target_Match newMatch = new target_Match
        {
            target = target_object,
            ui = BillboardUI.rectTransform,
            offset = target_offset
        };

        m_Matches.Add(newMatch);
    }

    private void Start()
    {

        foreach (var match in m_Matches)
        {
            if (match.target != null && match.ui != null)
            {
                Debug.Log(match.ui.name);
                Image billBoard = match.ui.GetComponent<Image>();
                billBoard = Instantiate(billBoard, canvas.transform);
                match.ui = billBoard.GetComponent<RectTransform>();
            }
        }
    }

    private void LateUpdate()
    {
        for (int _ =0; _ < m_Matches.Count; _++)
        {
            if (m_Matches[_].target != null && m_Matches[_].ui != null)
            {
                m_Matches[_].ui.position = m_Matches[_].target.position + Vector3.up * m_Matches[_].offset;
            }
        }
    }
}
