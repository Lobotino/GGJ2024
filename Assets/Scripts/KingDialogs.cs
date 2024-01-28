using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KingDialogs : MonoBehaviour
{
    [SerializeField] public Bubble kingBubble;
    [SerializeField] public GameController gameController;

    private static readonly int KingFun = Animator.StringToHash("kingFun");

    private bool isBeginDialogStarted, isEndingDialogStarted;

    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void StartBeginDialog()
    {
        if (isBeginDialogStarted) return;
        isBeginDialogStarted = true;
        StartCoroutine(BeginDialog());
    }

    public void StartGoodEndingDialog()
    {
        if (isEndingDialogStarted) return;
        isEndingDialogStarted = true;
        StartCoroutine(GoodEndingDialog());
    }

    public void StartBadEndingDialog()
    {
        if (isEndingDialogStarted) return;
        isEndingDialogStarted = true;
        StartCoroutine(BadEndingDialog());
    }

    private void Update()
    {
        _animator.SetFloat(KingFun, gameController.localKingFun);
    }

    private IEnumerator BeginDialog()
    {
        yield return kingBubble.ShowTextWithDelay("Хо-хо-хо", 2f);
        yield return kingBubble.ShowTextWithDelay(
            "Шуты, поужинаем ли мы сегодня смехом, или же в меню будут ваши головы?", 3f);
        yield return kingBubble.ShowTextWithDelay(
            "Рассмешите же меня!", 2f);
        yield return new WaitForSeconds(1);
        gameController.ShowKingFunView();
        PlayerPrefs.SetInt("GameStarted", 1);
    }

    private IEnumerator GoodEndingDialog()
    {
        yield return kingBubble.ShowTextWithDelay("Ха! Смешные у вас шутки! В этот раз можете быть свободны...", 3f);
        gameController.needToDark = true;
        yield return new WaitForSeconds(3);
        PlayerPrefs.SetInt("last win", 1);
        gameController.Disconnect();
        SceneManager.LoadScene("MENU_BEATY");
    }

    private IEnumerator BadEndingDialog()
    {
        yield return kingBubble.ShowTextWithDelay("Всё, с меня хватит. Жаль, что это были последние ваши шутки!", 2f);
    }
}