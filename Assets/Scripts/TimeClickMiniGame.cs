using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class TimeClickMiniGame : MonoBehaviour
{
    [SerializeField] public string miniGameType = "Balls";

    [SerializeField] public float minTimeBetweenClicks = 0.3f;
    [SerializeField] public float maxTimeBetweenClicks = 1.0f;

    [SerializeField] public float maxIncreaseFunValue = 10f;
    [SerializeField] public float minIncreaseFunValue = 1f;
    [SerializeField] public float maxDecreaseFunValue = 15f;
    [SerializeField] public float minDecreaseFunValue = 5f;

    [SerializeField] [CanBeNull] public NetworkObjectToHide itemToInteract;
    [SerializeField] public GameController gameController;

    [SerializeField] public List<KeyCode> buttonsToClickList;

    [SerializeField] public List<GameObject> viewsHoldersToShowButtonToClick;
    [SerializeField] public List<TextMeshProUGUI> textViewsToShowButtonToClick;

    private bool _isPlayingNow = false;

    private KeyCode _currentButtonToWait;
    private GameObject _currentViewToWait;
    private TextMeshProUGUI _currentTextToWait;
    private bool _clickedSuccessCurrentButton = false;
    private KeyCode _lastSuccessClickedButton;
    private int _lastViewHolderShowedId;

    public Boolean StartPlaying(Transform playerTransform)
    {
        if (!_isPlayingNow)
        {
            gameObject.transform.position = playerTransform.position;
            _isPlayingNow = true;
            if (itemToInteract != null)
            {
                itemToInteract.HideObjectRpc();
            }

            StartCoroutine(Play());
            return true;
        }

        return false;
    }

    public void StopPlaying()
    {
        _isPlayingNow = false;
        foreach (var view in viewsHoldersToShowButtonToClick)
        {
            view.SetActive(false);
        }

        if (itemToInteract != null)
        {
            itemToInteract.ShowObjectRpc();
        }

        if (_currentViewToWait != null)
        {
            _currentViewToWait.SetActive(false);
            _currentViewToWait = null;
            _currentTextToWait = null;
        }
    }

    private IEnumerator Play()
    {
        while (_isPlayingNow)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(minTimeBetweenClicks, maxTimeBetweenClicks));
            if (_isPlayingNow)
            {
                if (_currentButtonToWait != null && _currentTextToWait != null)
                {
                    if (_clickedSuccessCurrentButton)
                    {
                        _clickedSuccessCurrentButton = false;
                        IncreaseFunRpc();
                    }
                    else
                    {
                        _currentTextToWait.color = Color.red;
                        yield return new WaitForSeconds(0.3f);
                        DecreaseFunRpc();
                    }
                }

                while (true)
                {
                    var newButtonToWait =
                        buttonsToClickList[UnityEngine.Random.Range(0, buttonsToClickList.Count)];
                    if (newButtonToWait == _lastSuccessClickedButton) continue;

                    _currentButtonToWait = newButtonToWait;
                    break;
                }

                if (_currentViewToWait != null)
                {
                    _currentViewToWait.SetActive(false);
                    if (_currentTextToWait != null)
                    {
                        _currentTextToWait.color = Color.white;
                    }
                }

                while (true)
                {
                    var viewId = UnityEngine.Random.Range(0, viewsHoldersToShowButtonToClick.Count);
                    if (viewId == +_lastViewHolderShowedId) continue;

                    _currentViewToWait = viewsHoldersToShowButtonToClick[viewId];
                    _currentTextToWait = textViewsToShowButtonToClick[viewId];
                    _currentViewToWait.SetActive(true);
                    textViewsToShowButtonToClick[viewId].text = _currentButtonToWait.ToString();
                    _lastViewHolderShowedId = viewId;
                    break;
                }
            }
        }
    }

    // [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void IncreaseFunRpc()
    {
        var increase = UnityEngine.Random.Range(minIncreaseFunValue, maxIncreaseFunValue);
        gameController.IncreaseFunRpc(increase);
    }

    // [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void DecreaseFunRpc()
    {
        var decrease = UnityEngine.Random.Range(minDecreaseFunValue, maxDecreaseFunValue);
        gameController.DecreaseFunRpc(decrease);
    }

    void Update()
    {
        if (_isPlayingNow && !_clickedSuccessCurrentButton)
        {
            var buttonToWait = _currentButtonToWait;
            if (buttonToWait != null && Input.GetKey(buttonToWait))
            {
                _lastSuccessClickedButton = buttonToWait;
                _clickedSuccessCurrentButton = true;
                _currentTextToWait.color = Color.green;
            }
        }
    }
}