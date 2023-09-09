using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame: MonoBehaviour
{
    private static bool GameIsPaused = false;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject settingsUI;
    [SerializeField] private GameObject howToPlayUI;
    [SerializeField] private GameObject objectivesUI;
    [SerializeField] private GameObject rulesUI;
    [SerializeField] private GameObject initialRoundUI;
    [SerializeField] private GameObject postInitialRoundUI;
    [SerializeField] private GameObject revObjectiveUI;
    [SerializeField] private GameObject spyObjectiveUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (!GameIsPaused)
            {
                Pause();
            }
            else if (howToPlayUI.activeInHierarchy)
            {
                howToPlayUI.SetActive(false);
                settingsUI.SetActive(true);
            }
            else if (rulesUI.activeInHierarchy)
            {
                rulesUI.SetActive(false);
                howToPlayUI.SetActive(true);
            }
            else if (objectivesUI.activeInHierarchy)
            {
                objectivesUI.SetActive(false);
                howToPlayUI.SetActive(true);
            }
            else if (initialRoundUI.activeInHierarchy)
            {
                initialRoundUI.SetActive(false);
                rulesUI.SetActive(true);
            }
            else if (postInitialRoundUI.activeInHierarchy)
            {
                postInitialRoundUI.SetActive(false);
                rulesUI.SetActive(true);
            }
            else if (revObjectiveUI.activeInHierarchy)
            {
                revObjectiveUI.SetActive(false);
                objectivesUI.SetActive(true);
            }
            else if (spyObjectiveUI.activeInHierarchy)
            {
                spyObjectiveUI.SetActive(false);
                objectivesUI.SetActive(true);
            }
            else if (settingsUI.activeInHierarchy)
            {
                Resume();
            }
        }
    }

    // Continue forth through gameplay
    public void Resume()
    {
        settingsUI.SetActive(false);
        pauseButton.SetActive(true);
        GameIsPaused = false;

    }

    // Bring up settings menu, give user option to quit or read rules of game
    public void Pause()
    {
        settingsUI.SetActive(true);
        pauseButton.SetActive(false);
        GameIsPaused = true;
    }

    public static bool IsGamePaused()
    {
        return GameIsPaused;
    }
}
