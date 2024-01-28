using System.Collections;
using UnityEngine;

public class BardsScript : MonoBehaviour
{
    [SerializeField] public GameObject objectToShow;

    void Start()
    {
        StartCoroutine(EnableAfterTimer());
    }

    private IEnumerator EnableAfterTimer()
    {
        yield return new WaitForSeconds(30);
        objectToShow.SetActive(true);
    }
}