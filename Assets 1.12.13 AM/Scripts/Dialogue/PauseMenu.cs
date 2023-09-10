using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://youtu.be/JivuXdrIHK0

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    [SerializeField] private GameObject settingsUI;
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject popUpUI;
    [SerializeField] private GameObject gameCreaterUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (!GameMenu.IsActive())
            {
                menuUI.SetActive(true);
                return;
            }

            if (!menuUI.activeInHierarchy && !popUpUI.activeInHierarchy)
                return;

            if (!GameIsPaused)
            {
                Pause();
            }
            else
            {
                Resume();
            }
            
        }
            
    }

    // Continue forth through gameplay
    public void Resume()
    {
        settingsUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    // Bring up settings menu, give user option to quit or read rules of game
    void Pause()
    {
        settingsUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
}
