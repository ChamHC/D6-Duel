using D6Dice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    public GameObject Settings;
    public GameObject SettingsUI;
    public GameObject OptionsUI;
    public TurnBasedSystem game;

    public void Update()
    {
        if (game.GameState == GameState.End)
            Settings.SetActive(false);
    }

    public void SettingsButton()
    {
        Settings.SetActive(false);
        SettingsUI.SetActive(true);
        Time.timeScale = 0f;
    }
    public void ResumeButton()
    {
        Settings.SetActive(true);
        SettingsUI.SetActive(false);
        Time.timeScale = 1f;
    }
    public void OptionsButton()
    {
        SettingsUI.SetActive(false);
        OptionsUI.SetActive(true);
    }
    public void ExitButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
    public void BackButton()
    {
        SettingsUI.SetActive(true);
        OptionsUI.SetActive(false);
    }
    
}
