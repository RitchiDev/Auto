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
        StartCoroutine(EliminateCountdown());
    }

    private IEnumerator EliminateCountdown()
    {
        float totalTime = m_MaxEliminationTime;
        while (totalTime >= 0)
        {
            m_CountDownImage.fillAmount = totalTime / m_MaxEliminationTime;
            totalTime -= Time.deltaTime;
            m_CountDownText.text = totalTime.ToString("0");
            yield return null;
        }

        EliminatePlayer();
        StartCoroutine(EliminateCountdown());
    }

    public void AddAlivePlayer(EliminationPlayerController playerController)
    {
        m_AlivePlayers.Add(playerController);
    }

    private void EliminatePlayer()
    {
        foreach (EliminationPlayerController playerController in m_AlivePlayers)
        {
            playerController.Eliminate();
        }
    }
}
