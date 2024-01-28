using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] public bool isBiba = false;
    [SerializeField] public Button startButton;
    [SerializeField] public Button exitButton;

    void Start()
    {
        PlayerPrefs.SetInt("GameStarted", 0);
        startButton.onClick.AddListener(StartClick);
        exitButton.onClick.AddListener(ExitClick);
    }

    private void StartClick()
    {
        PlayerPrefs.SetInt("last win", 0);
        PlayerPrefs.SetInt("playerNumber", isBiba ? 1 : 0);
        SceneManager.LoadScene("Main 1");
    }

    private void ExitClick()
    {
        PlayerPrefs.SetInt("last win", 0);
        Application.Quit();
    }
}