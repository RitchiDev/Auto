using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class UsernameDisplay : MonoBehaviour
{
    [SerializeField] private PhotonView m_PhotonView;
    [SerializeField] private TMP_Text m_Username;

    private void Start()
    {
        if(m_PhotonView.IsMine)
        {
            gameObject.SetActive(false);
        }
        m_Username.text = m_PhotonView.Owner.NickName;
    }
}
