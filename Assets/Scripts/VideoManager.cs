using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    [ReadOnly] public string LaunchTutorial;
    [ReadOnly] public string EliminateTutorial;
    [ReadOnly] public string BoostTutorial;
    [ReadOnly] public string TeamBoostTutorial;

    public VideoPlayer VideoPlayer;

    public TextMeshProUGUI DescriptionText;

    private void Start()
    {
        LaunchTutorial = Application.streamingAssetsPath + "/LaunchTutorial.mp4";
        EliminateTutorial = Application.streamingAssetsPath + "/EliminateTutorial.mp4";
        BoostTutorial = Application.streamingAssetsPath + "/BoostTutorial.mp4";
        TeamBoostTutorial = Application.streamingAssetsPath + "/TeamBoostTutorial.mp4";

        VideoPlayer.url = LaunchTutorial;
    }

    private void Update()
    {
        Description();
    }

    public void Next()
    {
        if (VideoPlayer.url == LaunchTutorial)
        {
            VideoPlayer.url = EliminateTutorial;
        }
        else if (VideoPlayer.url == EliminateTutorial)
        {
            VideoPlayer.url = BoostTutorial;
        }
        else if (VideoPlayer.url == BoostTutorial)
        {
            VideoPlayer.url = TeamBoostTutorial;
        }
        else if (VideoPlayer.url == TeamBoostTutorial)
        {
            VideoPlayer.url = LaunchTutorial;
        }
    }

    public void Previous()
    {
        if (VideoPlayer.url == LaunchTutorial)
        {
            VideoPlayer.url = TeamBoostTutorial;
        }
        else if (VideoPlayer.url == EliminateTutorial)
        {
            VideoPlayer.url = LaunchTutorial;
        }
        else if (VideoPlayer.url == BoostTutorial)
        {
            VideoPlayer.url = EliminateTutorial;
        }
        else if (VideoPlayer.url == TeamBoostTutorial)
        {
            VideoPlayer.url = BoostTutorial;
        }
    }

    public void Description()
    {
        if (VideoPlayer.url == LaunchTutorial)
        {
            DescriptionText.text = "Drag over dice to launch. Dice value decides maximum launch distance";
        }
        else if (VideoPlayer.url == EliminateTutorial)
        {
            DescriptionText.text = "Land on opponent's dice to eliminate it. Dice gain momentum on elimination.";
        }
        else if (VideoPlayer.url == BoostTutorial)
        {
            DescriptionText.text = "Alternatively, colliding with booster dice will also boost dice momentum.";
        }
        else if (VideoPlayer.url == TeamBoostTutorial)
        {
            DescriptionText.text = "Landing on teammates' dice also boost momentum.";
        }
    }
}
