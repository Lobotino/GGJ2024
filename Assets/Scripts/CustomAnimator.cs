using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CustomAnimator : MonoBehaviour
{
    [SerializeField] public RawImage imageToAnimate;
    [SerializeField] public List<Texture2D> images;
    [SerializeField] public List<Texture2D> good_images;
    [SerializeField] public List<Texture2D> bad_images;
    [SerializeField] public float animateSpeed = 1f;

    private float currentTime = 0f;

    private int lastImage = 0;

    private bool needToAnimate = true;

    void Update()
    {
        if (!needToAnimate) return;

        currentTime += Time.deltaTime * animateSpeed;
        if (!(currentTime >= 10f)) return;
        currentTime = 0f;
        lastImage++;


        var imagesToShow = images;

        switch (PlayerPrefs.GetInt("last win"))
        {
            case 1:
                imagesToShow = good_images;
                break;

            case 2:
                imagesToShow = bad_images;
                break;
        }

        if (lastImage >= imagesToShow.Count)
        {
            needToAnimate = false;
            return;
        }

        imageToAnimate.texture = imagesToShow[lastImage];
    }
}