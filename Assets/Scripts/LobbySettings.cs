using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbySettings : MonoBehaviour
{
    public Slider PlayerA;
    public Slider PlayerB;
    public Slider Boosters;

    public TextMeshProUGUI PlayerAText;
    public TextMeshProUGUI PlayerBText;
    public TextMeshProUGUI BoosterText;

    public static int numOfPlayerA;
    public static int numOfPlayerB;
    public static int numOfBoosters;

    public void Update()
    {
        numOfPlayerA = (int)PlayerA.value;
        numOfPlayerB = (int)PlayerB.value;
        numOfBoosters = (int)Boosters.value;

        PlayerAText.text = PlayerA.value.ToString();
        PlayerBText.text = PlayerB.value.ToString();
        BoosterText.text = Boosters.value.ToString();
    }
}
