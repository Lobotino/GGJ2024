using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScaleScript : MonoBehaviour
{
    [SerializeField] public float rate = 0.02f;
    [SerializeField] public float ratio = 0.006f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        PingPong();
    }

    void PingPong()
    {
        transform.localScale = Mathf.PingPong(Time.time * rate, ratio) * Vector2.one;
        if (transform.localScale.x <= 0.003f)
        {
            transform.localScale = new Vector2(0.0035f, 0.0035f);
        }
    }
}