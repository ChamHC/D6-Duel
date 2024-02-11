using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    public VideoClip LaunchTutorial;
    public VideoClip EliminateTutorial;
    public VideoClip BoostTutorial;
    public VideoClip TeamBoostTutorial;

    public VideoPlayer VideoPlayer;

    public TextMeshProUGUI DescriptionText;

    public void Update()
    {
        Description();
    }

    public void Next()
    {
        if (VideoPlayer.clip == LaunchTutorial)
        {
            VideoPlayer.clip = EliminateTutorial;
        }
        else if (VideoPlayer.clip == EliminateTutorial)
        {
            VideoPlayer.clip = BoostTutorial;
        }
        else if (VideoPlayer.clip == BoostTutorial)
        {
            VideoPlayer.clip = TeamBoostTutorial;
        }
        else if (VideoPlayer.clip == TeamBoostTutorial)
        {
            VideoPlayer.clip = LaunchTutorial;
        }
    }

    public void Previous()
    {
        if (VideoPlayer.clip == LaunchTutorial)
        {
            VideoPlayer.clip = TeamBoostTutorial;
        }
        else if (VideoPlayer.clip == EliminateTutorial)
        {
            VideoPlayer.clip = LaunchTutorial;
        }
        else if (VideoPlayer.clip == BoostTutorial)
        {
            VideoPlayer.clip = EliminateTutorial;
        }
        else if (VideoPlayer.clip == TeamBoostTutorial)
        {
            VideoPlayer.clip = BoostTutorial;
        }
    }

    public void Description()
    {
        if (VideoPlayer.clip == LaunchTutorial)
        {
            DescriptionText.text = "Drag over dice to launch. Dice value decides maximum launch distance";
        }
        else if (VideoPlayer.clip == EliminateTutorial)
        {
            DescriptionText.text = "Stack to eliminate opponent's dice. Dice gain momentum on elimination.";
        }
        else if (VideoPlayer.clip == BoostTutorial)
        {
            DescriptionText.text = "Alternatively, gain momentum by colliding with yellow dice.";
        }
        else if (VideoPlayer.clip == TeamBoostTutorial)
        {
            DescriptionText.text = "Stacking on teammates' dice also boost momentum.";
        }
    }
}
