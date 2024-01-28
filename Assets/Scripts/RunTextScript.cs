using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RunTextScript : MonoBehaviour
{
    public int maxTextCharacters = 6;

    public float changeSpeed = 0.3f;
    
    public Text text;
    
    


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(nextSlide());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator nextSlide()
    {
        int currentStartSlide = 0;
        string resultString = "";
        while (true)
        {
            currentStartSlide++;
            char currentChar;
            if (currentStartSlide >= text.text.Length)
            {
                currentStartSlide = 0;
            }

            Debug.Log("currentStartSlide" + currentStartSlide);
            currentChar = text.text[currentStartSlide];
            if (resultString.Length >= maxTextCharacters)
            {
                resultString = resultString.Substring(1, resultString.Length) + currentChar;
            }

            text.text = resultString;
            yield return new WaitForSeconds(changeSpeed);
        }
    }
}