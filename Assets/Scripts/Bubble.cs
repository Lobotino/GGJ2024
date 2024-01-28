using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Bubble : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI textInBubble;
    [SerializeField] public float delayBetweenMessagesInSeconds = 0.05f;

    [SerializeField] public List<string> allCharacterDialogs = new List<string>();

    public bool isTalkNow = false;

    private Coroutine currentTalk = null;

    public void ShowAllTexts()
    {
        isTalkNow = true;
        this.GameObject().SetActive(true);
        StartCoroutine(StartShowAllTexts());
    }

    public void ShowCurrentDialog(int dialogNumber)
    {
        this.GameObject().SetActive(true);
        StartCoroutine(allCharacterDialogs[dialogNumber]);
    }

    public void ShowCurrentDialog(string text, float endTextDelay)
    {
        StartCoroutine(ShowTextWithDelay(text, endTextDelay));
    }

    public void ShowTextOfEvidence(string text)
    {
        this.GameObject().SetActive(true);
        if (currentTalk != null)
        {
            StopCoroutine(currentTalk);
        }
        currentTalk = StartCoroutine(ShowTextWithDelay(text, 4));
    }
    
    private IEnumerator StartShowAllTexts()
    {
        foreach (var text in allCharacterDialogs)
        {
            textInBubble.text = "";
            yield return ShowText(text);
            yield return new WaitForSeconds(2);
        }
        this.GameObject().SetActive(false);
        isTalkNow = false;
    }

    public IEnumerator ShowTextWithDelay(string text, float delay)
    {
        textInBubble.text = "";
        this.GameObject().SetActive(true);
        yield return ShowText(text);
        yield return new WaitForSeconds(delay);
        this.GameObject().SetActive(false);
    }
    
    public IEnumerator ShowTextWithDelay(int dialogNumber, float delay)
    {
        textInBubble.text = "";
        this.GameObject().SetActive(true);
        yield return ShowText(allCharacterDialogs[dialogNumber]);
        yield return new WaitForSeconds(delay);
        this.GameObject().SetActive(false);
    }
    
    private IEnumerator ShowText(string text)
    {
        foreach (var ch in text)
        {
            AddNextCharacter(ch);
            yield return new WaitForSeconds(delayBetweenMessagesInSeconds);
        }
    }
    
    private void AddNextCharacter(char a_Character)
    {
        textInBubble.text += a_Character;
    }
}