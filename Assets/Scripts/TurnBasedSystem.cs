using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace D6Dice
{
    public enum GameState
    {
        Start,
        PlayerA,
        PlayerB,
        End
    }
    public class TurnBasedSystem : MonoBehaviour
    {
        #region Variables
        [ReadOnly] public GameState GameState = GameState.Start;

        [Header("GameObject List")]
        [ReadOnly] public List<GameObject> PlayerA;
        [ReadOnly] public List<GameObject> PlayerB;
        [ReadOnly] public List<GameObject> Powerups;
        public GameObject PlayerA_UI;
        public GameObject PlayerB_UI;
        public GameObject EndScreen;
        public GameObject Header;
        public AudioSource AudioSource;
        public AudioSource BackgroundMusic;
        public AudioSource TurnTransitionSound;
        public Slider MusicSlider;
        public Slider SoundSlider;

        private bool _isCoroutineRunning = false;

        #endregion

        void Start()
        {
            SoundSlider.value = MenuController.soundVolume;
            MusicSlider.value = MenuController.musicVolume;
            Header.GetComponentInChildren<TextMeshProUGUI>().text = "Duel Start";
            Header.SetActive(true);

            StartCoroutine(InitializeGame());

            IEnumerator InitializeGame()
            {
                while (!AllStationary())
                    yield return null;

                GameState = GameState.PlayerA;
            }
        }

        void Update()
        {
            UIHandler();
            IsActionDepleted();

            HasEnded();
            VolumeUpdate();
        }

        #region In Game
        public void Setup()
        {
            PlayerA = new List<GameObject>();
            GameObject[] playerList = GameObject.FindGameObjectsWithTag("PlayerA");
            foreach (var player in playerList)
            {
                PlayerA.Add(player);
            }

            PlayerB = new List<GameObject>();
            GameObject[] opponentList = GameObject.FindGameObjectsWithTag("PlayerB");
            foreach (var opponent in opponentList)
            {
                PlayerB.Add(opponent);
            }

            Powerups = new List<GameObject>();
            GameObject[] powerupList = GameObject.FindGameObjectsWithTag("PowerUps");
            foreach (var powerup in powerupList)
            {
                Powerups.Add(powerup);
            }
        }
        public bool AllStationary()
        {
            bool isStationary = true;
            if (PlayerA != null)
            {
                foreach (var player in PlayerA)
                {
                    if (!player.GetComponent<Rigidbody>().IsSleeping())
                        isStationary = false;
                }
            }
            if (PlayerB != null)
            {
                foreach (var opponent in PlayerB)
                {
                    if (!opponent.GetComponent<Rigidbody>().IsSleeping())
                        isStationary = false;
                }
            }
            if (Powerups != null)
            {
                foreach (var powerup in Powerups)
                {
                    if (!powerup.GetComponent<Rigidbody>().IsSleeping())
                        isStationary = false;
                }
            }
            return isStationary;
        }
        public void EndTurn()
        {
            PlayerA_UI.SetActive(false);
            PlayerB_UI.SetActive(false);
            if (GameState == GameState.PlayerA)
            {
                GameState = GameState.PlayerB;
            }
            else if (GameState == GameState.PlayerB)
            {
                GameState = GameState.PlayerA;
            }
            StartCoroutine(TurnTransition());
        }
        public void IsActionDepleted()
        {
            if (!AllStationary()) return;
            List<GameObject> playerCurrent = GameState == GameState.PlayerA ? PlayerA : PlayerB;
            bool isAllLaunched = true;
            foreach (var player in playerCurrent)
            {
                if (!player.GetComponent<Dice>().isLaunched)
                {
                    isAllLaunched = false;
                    return;
                }
            }
            if (isAllLaunched)
            {
                EndTurn();
            }
        }
        private void UIHandler()
        {
            if (_isCoroutineRunning) return;
            if (AllStationary())
            {
                Header.SetActive(false);
                if (GameState == GameState.PlayerA)
                {
                    PlayerA_UI.SetActive(true);
                    PlayerB_UI.SetActive(false);
                }
                else if (GameState == GameState.PlayerB)
                {
                    PlayerA_UI.SetActive(false);
                    PlayerB_UI.SetActive(true);
                }
                else
                {
                    PlayerA_UI.SetActive(false);
                    PlayerB_UI.SetActive(false);
                }
            }
            else if (!AllStationary())
            {
                PlayerA_UI.SetActive(false);
                PlayerB_UI.SetActive(false);
            }
        }

        private IEnumerator TurnTransition()
        {
            if (GameState == GameState.End) yield break;
            _isCoroutineRunning = true;

            TurnTransitionSound.Play();

            if (GameState == GameState.PlayerA)
                Header.GetComponentInChildren<TextMeshProUGUI>().text = "Player A's\nTurn";
            else if (GameState == GameState.PlayerB)
                Header.GetComponentInChildren<TextMeshProUGUI>().text = "Player B's\nTurn";

            Header.SetActive(true);
            yield return new WaitForSeconds(2f);
            Header.SetActive(false);
            
            _isCoroutineRunning = false;
        }
        #endregion

        #region End Game
        public void HasEnded()
        {
            if (PlayerA.Count == 0 || PlayerB.Count == 0)
            {
                BackgroundMusic.Stop();
                if(!AudioSource.isPlaying)
                    AudioSource.Play();

                GameState = GameState.End;
                EndScreen.SetActive(true);
                TextMeshProUGUI victoryTitle = EndScreen.transform.Find("Victory Title").GetComponent<TextMeshProUGUI>();
                if (PlayerA.Count != 0)
                    victoryTitle.text = "Player A\nVictory";
                else
                    victoryTitle.text = "Player B\nVictory";
            }
        }
        #endregion

        #region Volume
        public void VolumeUpdate()
        {
            AudioSource.volume = MenuController.soundVolume;
            BackgroundMusic.volume = MenuController.musicVolume;
            TurnTransitionSound.volume = MenuController.soundVolume * 0.9f;
        }
        #endregion

    }
}

