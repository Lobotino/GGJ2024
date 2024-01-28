using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : NetworkBehaviour
{
    [SerializeField] public KingDialogs kingDialogs;

    [SerializeField] public GameObject loadingView;
    [SerializeField] public GameObject kingFunView;
    [SerializeField] public TextMeshProUGUI papersCountText;
    [SerializeField] public Slider kingFunProgress;
    [SerializeField] public float kingFunDecreaseSpeed = 1f;
    [SerializeField] public GameObject finalPaper;
    [SerializeField] public GameObject goodEndingObjects;
    [SerializeField] public GameObject badEndingObjects;

    [SerializeField] public GameObject ballsObject;
    [SerializeField] public GameObject balanceObject;

    [SerializeField] public SoundManager soundManager;

    [SerializeField] public Image darkFullScreenImage;

    [SerializeField] public float darkSpeed;

    public bool needToDark;
    [Networked] public float KingFun { get; set; }
    [Networked] public bool GameStarted { get; set; }
    [Networked] public bool GameWin { get; set; }

    [Networked] public int PapersCount { get; set; }

    private ChangeDetector _changeDetector;

    private bool isSpawned = false;

    public float localKingFun = 95f;

    public int localPapersCount = 0;

    public override void Spawned()
    {
        isSpawned = true;
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        KingFun = 95.0f;
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority || !GameStarted) return;

        KingFun -= 0.05f * kingFunDecreaseSpeed;
        if (KingFun <= 0)
        {
            KingFun = 0f;
        }

        if (KingFun >= 80)
        {
            KingFun = 80;
        }

        localKingFun = KingFun;

        UpdateSliderProgress();
    }

    public PlayerRef playerRefMe;

    public void OnPlayerJoined(NetworkRunner networkRunner)
    {
        var players = networkRunner.SessionInfo.PlayerCount;
        Debug.Log("Players: " + players);
        
        // if (HasStateAuthority)
        // {
        //     playerRefMe = playerRef;
        // }

        if (players >= 2)
        {
            PlayerPrefs.SetInt("playerNumber", 1);
            StartGame();
            if (!isSpawned) return;
            GameStarted = true;
        }
        else
        {
            PlayerPrefs.SetInt("playerNumber", 0);
        }
    }

    void Update()
    {
        darkFullScreenImage.color = Color.Lerp(darkFullScreenImage.color, needToDark ? Color.black : Color.clear,
            Time.deltaTime * darkSpeed);

        if (PlayerPrefs.GetInt("GameStarted") == 1 && localKingFun <= 1)
        {
            Debug.Log("Loose game with score " + localKingFun);
            ShowLooseGame();
            return;
        }

        if (PlayerPrefs.GetInt("GameStarted") == 1 && localKingFun >= 98)
        {
            Debug.Log("Win game with score " + localKingFun);
            ShowWinGame();
            return;
        }

        if (_changeDetector == null)
        {
            return;
        }

        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(KingFun):
                    localKingFun = KingFun;
                    UpdateSliderProgress();
                    break;
                case nameof(PapersCount):
                    localPapersCount = PapersCount;
                    UpdatePapersCountUI();
                    break;
                case nameof(GameStarted):
                    if (!HasStateAuthority && GameStarted)
                    {
                        StartGame();
                    }

                    break;
                case nameof(GameWin):
                    if (!HasStateAuthority)
                    {
                        ShowWinGame();
                    }

                    break;
            }
        }
    }

    private void UpdatePapersCountUI()
    {
        papersCountText.text = localPapersCount + "/4";
        if (localPapersCount >= 4)
        {
            finalPaper.SetActive(true);
        }
    }

    private void UpdateSliderProgress()
    {
        kingFunProgress.value = KingFun;
    }

    private void StartGame()
    {
        kingDialogs.StartBeginDialog();
        loadingView.SetActive(false);
    }

    public void ShowKingFunView()
    {
        kingFunView.SetActive(true);
    }

    public void ShowLooseGame()
    {
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            player.SetActive(false);
        }

        ballsObject.SetActive(false);
        balanceObject.SetActive(false);

        Camera.main.GetComponent<CameraScript>().player = null;
        var kingPos = kingDialogs.gameObject.transform.position;
        Camera.main.transform.position = new Vector3(kingPos.x, kingPos.y, -10f);

        if (HasStateAuthority)
        {
            GameStarted = false;
        }

        PlayerPrefs.SetInt("GameStarted", 0);
        soundManager.StopSound();
        soundManager.PlayBadEnding();
        kingFunView.SetActive(false);
        badEndingObjects.SetActive(true);
    }

    public void ShowWinGame()
    {
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            player.SetActive(false);
        }

        ballsObject.SetActive(false);
        balanceObject.SetActive(false);

        Camera.main.GetComponent<CameraScript>().player = null;

        var kingPos = kingDialogs.gameObject.transform.position;
        Camera.main.transform.position = new Vector3(kingPos.x, kingPos.y, -10f);

        if (HasStateAuthority)
        {
            GameStarted = false;
        }

        PlayerPrefs.SetInt("GameStarted", 0);
        soundManager.StopSound();
        kingFunView.SetActive(false);
        goodEndingObjects.SetActive(true);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void IncreaseFunRpc(float value)
    {
        Debug.Log("Success! Increase fun " + value);
        KingFun += value;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DecreaseFunRpc(float value)
    {
        Debug.Log("Fail! Decrease fun " + value);
        KingFun -= value;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void WinGameRpc()
    {
        GameWin = true;
        if (HasStateAuthority)
        {
            ShowWinGame();
        }
    }

    //
    // IEnumerator FadeInFunView()
    // {
    //     for (float i = 0; i <= 1; i += Time.deltaTime)
    //     {
    //         kingFunView.color = new Color(1, 1, 1, i);
    //         yield return null;
    //     }
    // }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void IncrementPapersCountRpc()
    {
        PapersCount++;
        Debug.Log("Current papers count " + PapersCount);
    }

    public void Disconnect()
    {
        Runner.Shutdown();
    }
}