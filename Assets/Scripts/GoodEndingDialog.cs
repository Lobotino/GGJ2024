using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodEndingDialog : MonoBehaviour
{
    [SerializeField] public Bubble redBubble;
    [SerializeField] public Bubble blueBubble;
    [SerializeField] public SoundManager soundManager;
    [SerializeField] public KingDialogs kingDialogs;
    [SerializeField] public Animator kingAnimator;

    void Start()
    {
        StartCoroutine(BeginDialog());
    }

    private IEnumerator BeginDialog()
    {
        yield return redBubble.ShowTextWithDelay("А теперь, смертельный номер!", 2f);
        yield return blueBubble.ShowTextWithDelay(
            "Анекдот!!!", 2f);
        yield return redBubble.ShowTextWithDelay(
            "Что сказал каннибал, когда съел клоуна?", 2f);
        yield return blueBubble.ShowTextWithDelay(
            "Весело и вкусно", 1f);
        soundManager.PlayHaha();
        yield return new WaitForSeconds(1);
        kingDialogs.StartGoodEndingDialog();
        soundManager.PlayGoodEnding();
        kingAnimator.Play("good_ending");
    }
}