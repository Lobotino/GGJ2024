using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameScript : MonoBehaviour
{
    [SerializeField] private GameController gameController;

    public void FinishGame()
    {
        gameController.WinGameRpc();
    }
}