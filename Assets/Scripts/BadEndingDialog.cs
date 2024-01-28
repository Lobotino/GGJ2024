using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BadEndingDialog : MonoBehaviour
{
    [SerializeField] public GameObject redFool;
    [SerializeField] public GameObject blueFool;
    [SerializeField] public KingDialogs kingDialogs;
    [SerializeField] public Animator kingAnimator;
    [SerializeField] public GameController gameController;

    void Start()
    {
        StartCoroutine(BeginDialog());
    }

    private IEnumerator BeginDialog()
    {
        kingDialogs.StartBadEndingDialog();
        yield return new WaitForSeconds(4);
        kingAnimator.Play("bad_ending");
        yield return new WaitForSeconds(2);
        redFool.SetActive(false);
        blueFool.SetActive(false);
        yield return new WaitForSeconds(1);
        gameController.needToDark = true;
        yield return new WaitForSeconds(2);
        PlayerPrefs.SetInt("last win", 2);
        gameController.Disconnect();
        SceneManager.LoadScene("MENU_BEATY");
    }
}