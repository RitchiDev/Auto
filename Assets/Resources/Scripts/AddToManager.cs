using UnityEngine;
using UnityEngine.UI;
public class AddToManager :MonoBehaviour
{
    private ColorManager Manager;
    private void Start()
    {
        Manager =ColorManager.m_instance;    
    }
    public void OnColorButtonPressed()
    {
        if (!Manager.m_ChosenStance)
            Manager.m_Prim = GetComponent<Image>().color;
        else
            Manager.m_Second = GetComponent<Image>().color;
    }
    public void OnToggleButtonPressed()
    {
        if (!Manager.m_ChosenStance)
        {
            GetComponentInChildren<Text>().text = "Secondary Color";
            Manager.m_ChosenStance = true;
        }
        else
        {
            GetComponentInChildren<Text>().text = "Primary Color";
            Manager.m_ChosenStance = false;
        }
    }

}
