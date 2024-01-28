using System;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] public float smoothSpeed = 1f;
    public Transform player;

    private void LateUpdate()
    {
        if (player == null) return;

        SmoothFollow();
    }

    private void SmoothFollow()
    {
        Vector3 targetPos = new Vector3(player.position.x, player.position.y, -10);
        Vector3 smoothFollow = Vector3.Lerp(transform.position,
            targetPos, smoothSpeed);

        transform.position = smoothFollow;
    }
}