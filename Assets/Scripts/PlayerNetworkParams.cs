using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerNetworkParams : NetworkBehaviour
{
    private PlayerController _playerController;

    [Networked] public int PlayerNumber { get; set; }
    [Networked] public int AnimationState { get; set; }

    private ChangeDetector _changeDetector;

    private Camera _camera;

    private bool _spawned;

    public override void Spawned()
    {
        _playerController = GetComponent<PlayerController>();
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        PlayerNumber = PlayerPrefs.GetInt("playerNumber");

        _spawned = true;
    }

    void Update()
    {
        if (HasStateAuthority)
        {
            if (_camera == null && PlayerPrefs.GetInt("GameStarted") == 1)
            {
                _camera = Camera.main;
                _camera.GetComponent<CameraScript>().player = transform;
            }

            return;
        }

        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(PlayerNumber):
                    _playerController.UpdatePlayerColor(PlayerNumber);
                    break;
                case nameof(AnimationState):
                    _playerController.UpdateAnimationState(AnimationState);
                    break;
            }
        }
    }
}