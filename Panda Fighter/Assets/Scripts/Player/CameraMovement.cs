using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform cameraTarget;
    public Transform centerOfMap;

    [Range(0, 1)]
    public float mapLocationOffsetMultiplier;

    private Vector3 cameraOffset;
    private float cameraPosX;
    private float cameraPosY;
    private float cameraVelocityX = 0.0f;
    private float cameraVelocityY = 0.0f;
    private float smoothTimeX = 0.15f;
    private float smoothTimeY = 0.4f;
    private float mouseDistanceX;
    private float mouseDistanceY;
    private float locationBasedOffset;

    void Start()
    {
        cameraOffset = transform.position - cameraTarget.transform.position;
    }

    void FixedUpdate()
    {
        cameraMovement();
    }

    private void cameraMovement()
    {
        mouseDistanceX = (Input.mousePosition.x - (float)Screen.width / 2f) / (float)Screen.width;
        mouseDistanceY = (Input.mousePosition.y - (float)Screen.height / 2f) / (float)Screen.height;

        if (mouseDistanceX > 0.5f || mouseDistanceX < -0.5f)
            mouseDistanceX = 0.5f * Mathf.Sign(mouseDistanceX);

        if (mouseDistanceY > 0.5f || mouseDistanceY < -0.5f)
            mouseDistanceY = 0.5f * Mathf.Sign(mouseDistanceY);

        locationBasedOffset = (centerOfMap.position.y - cameraTarget.position.y) * mapLocationOffsetMultiplier;

        cameraPosX = Mathf.SmoothDamp(transform.position.x, cameraTarget.position.x + cameraOffset.x + mouseDistanceX * 10f, ref cameraVelocityX, smoothTimeX);
        cameraPosY = Mathf.SmoothDamp(transform.position.y, cameraTarget.position.y + cameraOffset.y + mouseDistanceY * 8f + locationBasedOffset, ref cameraVelocityY, smoothTimeY);

        transform.position = new Vector3(cameraPosX, cameraPosY, transform.position.z);
    }

}
