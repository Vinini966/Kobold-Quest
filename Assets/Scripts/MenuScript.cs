//Code by Vincent Kyne

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    [Header("Menu")]
    public bool isOpen = false;

    [Header("Settings")]
    public GameObject settingsObject;
    bool windowOpen = false;

    public delegate void MenuChange(bool status);
    public MenuChange onMenuChange;

    public void OpenMenu()
    {
        if (isOpen)
        {
            isOpen = false;
            gameObject.SetActive(false);
            
        }
        else
        {
            isOpen = true;
            gameObject.SetActive(true);
        }
        if (onMenuChange != null)
            onMenuChange(isOpen);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Title");
    }

    public void Settings()
    {
        if (windowOpen)
        {
            windowOpen = false;
            settingsObject.SetActive(false);
        }
        else
        {
            windowOpen = true;
            settingsObject.SetActive(true);
        }
        if (onMenuChange != null)
            onMenuChange(windowOpen);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
