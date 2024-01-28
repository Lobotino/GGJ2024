using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateOnceScript : MonoBehaviour
{
    private bool isPlayed = false;

    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPlayed)
        {
            _animator.Play("Boom",  -1, 0f);
            isPlayed = true;
        }
    }
}