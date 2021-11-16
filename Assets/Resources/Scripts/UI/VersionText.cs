using UnityEngine;
using TMPro;
public class VersionText : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<TMP_Text>().text = "Game Version: " + Application.version;
    }
}
