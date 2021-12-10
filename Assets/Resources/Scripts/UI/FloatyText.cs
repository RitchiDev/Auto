using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class FloatyText : MonoBehaviour
{
    [SerializeField] private TMP_Text m_PointsText;
    [SerializeField] private float m_ShrinkTime = 3f;
    [SerializeField] private float m_GrowTime = 3f;
    [SerializeField] private Vector3 m_Offset = new Vector3(0.0f, 3f, 0.0f);
    [SerializeField] private Vector3 m_Randomize = new Vector3(0.5f, 0.0f, 0.0f);
    [SerializeField] private Vector3 m_GrowScale = new Vector3(1.5f, 1.5f, 1.5f);
    private RectTransform m_Transform;

    private void Awake()
    {
        m_Transform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        if(!m_Transform)
        {
            return;
        }

        m_Transform.localPosition += m_Offset;
        m_Transform.localPosition += new Vector3(Random.Range(-m_Randomize.x, m_Randomize.x), Random.Range(-m_Randomize.y, m_Randomize.y), Random.Range(-m_Randomize.z, m_Randomize.z));
    }

    public void SetUp(string text, Material material)
    {
        if (!m_Transform)
        {
            return;
        }

        m_PointsText.text = "+" + text;
        m_PointsText.color = material.color;

        Grow();
    }

    private void Shrink()
    {
        if (!m_Transform)
        {
            return;
        }

        m_Transform.DOScale(Vector3.zero, m_ShrinkTime);
        Invoke("DoDestroy", m_ShrinkTime);
    }

    private void Grow()
    {
        if (!m_Transform)
        {
            return;
        }

        m_Transform.DOScale(m_GrowScale, m_GrowTime);
        Invoke("Shrink", m_GrowTime);
    }

    private void DoDestroy()
    {
        if (!m_Transform)
        {
            return;
        }

        Destroy(m_Transform.parent.gameObject);
    }
}
