using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private bool m_FindMenus;
    [SerializeField] private List<Menu> m_Menus;

    public static MenuManager Instance { get; private set; }

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

        if(m_FindMenus)
        {
            Menu[] menus = FindObjectsOfType<Menu>();
            foreach (Menu menu in menus)
            {
                m_Menus.Add(menu);
            }
        }
    }

    public void OpenMenu(MenuName menuName)
    {
        foreach (Menu menu in m_Menus)
        {
            if (menu.MenuName == menuName)
            {
                menu.Open();
            }
            else if (menu.IsOpen)
            {
                CloseMenu(menu);
            }
        }
    }

    public void OpenMenu(Menu gameMenu)
    {
        foreach (Menu menu in this.m_Menus)
        {
            if (menu.IsOpen)
            {
                CloseMenu(menu);
            }
        }
        gameMenu.Open();
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }

    public void SaveGameSettings()
    {
        //if(Application.isEditor)
        //{
        //    return;
        //}

        PlayerPrefs.Save();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
