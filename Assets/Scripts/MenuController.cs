using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public static float musicVolume = 1f;
    public static float soundVolume = 1f;

    public Slider soundSlider;
    public Slider musicSlider;

    public AudioSource uiSound;
    public AudioSource bgMusic;

    private bool toLevelOne;

    private void Start()
    {
        musicSlider.value = musicVolume;
        soundSlider.value = soundVolume;
    }

    private void Update()
    {
        UpdateVolume();

        if (toLevelOne)
        {
            StartCoroutine(ToLevelOne());
        }

        IEnumerator ToLevelOne()
        {
            yield return new WaitForSeconds(0.75f);
            toLevelOne = false;
            SceneManager.LoadScene(1);
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void LoadLevelOne()
    {
        toLevelOne = true;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MusicSliderAction()
    {
        musicVolume = musicSlider.value;
    }
    public void SoundSliderAction()
    {
        soundVolume = soundSlider.value;
    }

    public void UpdateVolume()
    {
        if (uiSound == null || bgMusic == null) return;
        uiSound.volume = soundVolume;
        bgMusic.volume = musicVolume;
    }

    public void PlayUISound()
    {
        uiSound.Play();
    }
}
