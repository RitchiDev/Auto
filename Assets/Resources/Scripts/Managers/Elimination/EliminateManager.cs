using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun.UtilityScripts;
using UnityEngine.UI;
using TMPro;

public class EliminateManager : MonoBehaviour
{
    public static EliminateManager Instance { get; private set; }

    [SerializeField] private List<EliminationPlayerController> m_AlivePlayers = new List<EliminationPlayerController>();

    [SerializeField] private Image m_CountDownImage;
    [SerializeField] private TMP_Text m_CountDownText;
    [SerializeField] private float m_MaxEliminationTime = 60f;
    [SerializeField] private float m_TimeBeforeNextElimination = 5f;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogError("An instance of: " + this.ToString() + " already existed!");
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        StartCoroutine(TimeBeforeEliminateStartsCountdown());
    }

    private IEnumerator TimeBeforeEliminateStartsCountdown()
    {
        m_CountDownText.color = Color.white;

        float totalTime = m_TimeBeforeNextElimination;
        while (totalTime >= 0)
        {
            m_CountDownImage.fillAmount = totalTime / m_MaxEliminationTime;
            totalTime -= Time.deltaTime;
            m_CountDownText.text = totalTime.ToString("0");

            yield return null;
        }

        StartCoroutine(EliminateCountdown());
    }

    private IEnumerator EliminateCountdown()
    {
        m_CountDownText.color = Color.red;

        float totalTime = m_MaxEliminationTime;
        while (totalTime >= 0)
        {
            m_CountDownImage.fillAmount = totalTime / m_MaxEliminationTime;
            totalTime -= Time.deltaTime;
            m_CountDownText.text = totalTime.ToString("0");

            yield return null;
        }

        EliminatePlayer();
        StartCoroutine(TimeBeforeEliminateStartsCountdown());
    }

    public void AddAlivePlayer(EliminationPlayerController playerController)
    {
        m_AlivePlayers.Add(playerController);
    }

    public void RemoveAlivePlayer(EliminationPlayerController playerController)
    {
        m_AlivePlayers.Remove(playerController);
    }

    private void EliminatePlayer()
    {
        foreach (EliminationPlayerController playerController in m_AlivePlayers)
        {
            playerController.Eliminate();
        }
    }
}
