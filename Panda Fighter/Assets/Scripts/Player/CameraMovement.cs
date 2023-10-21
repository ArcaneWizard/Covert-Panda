using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

///<summary> Handles the main camera's movement </summary>
[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform cameraTargetBeforePlayerDeath;
    [SerializeField] private Transform cameraTargetAfterPlayerDeath;
    [SerializeField] private Transform centerOfMap;
    [SerializeField] private PlayerHealth health;

    // The player/creature the camera follows
    private Transform cameraTarget;

    // How much the camera overshoots/undershoots as the creature gets closer to the top/bottom edge of the map
    [Range(0, 1)]
    private float mapOffsetMultiplier = 0f;

    // Camera movement settings
    private float smoothTimeX = 0.15f;
    private float smoothTimeY = 0.4f;

    // Camera shake settings
    public float shakeDuration = 0.2f;
    public float shakeSpeed = 1f;
    public float shakeMaxMagnitude = 1f;

    // Camera sway settings
    public float swaySpeed = 1f;
    public float swayMaxMagnitude = 1f;

    private float cameraVelocityX = 0.0f;
    private float cameraVelocityY = 0.0f;
    private Vector3 cameraOffset;

    private bool executeCameraShake;
    private float shakeTimer;
    private Vector2 shakeOffset;
    private Vector2 swayOffset;
    private float random;

    public float d;

    void Start()
    {
        cameraTarget = cameraTargetAfterPlayerDeath;
        cameraOffset = transform.position - cameraTarget.transform.position;
    }

    void FixedUpdate()
    {
        updateCameraShake();
        updateCameraSway();
        updateCameraMovement();

        // when timer is over, end camera shake
        if (shakeTimer > 0f)
            shakeTimer -= Time.deltaTime;
        else
            executeCameraShake = false;
    }

    public void ExecuteCameraShake(float maxMagnitude)
    {
        executeCameraShake = true;

        shakeMaxMagnitude = maxMagnitude;
        shakeTimer = shakeDuration;
        random = UnityEngine.Random.Range(0, 100000);
    }

    private void updateCameraMovement()
    {
        // determine mouse position relative to the center of screen
        float mouseDistanceX = (Input.mousePosition.x - Screen.width / 2f) / (float)Screen.width;
        float mouseDistanceY = (Input.mousePosition.y - Screen.height / 2f) / (float)Screen.height;

        if (mouseDistanceX > 0.5f || mouseDistanceX < -0.5f)
            mouseDistanceX = 0.5f * Mathf.Sign(mouseDistanceX);

        if (mouseDistanceY > 0.5f || mouseDistanceY < -0.5f)
            mouseDistanceY = 0.5f * Mathf.Sign(mouseDistanceY);

        // camera should undershoot/overshoot the further the player is from the center of the map on the y-axis
        float locationBasedOffset = (centerOfMap.position.y - cameraTarget.position.y) * mapOffsetMultiplier;

        // camera smoothly follows the moving player and extends slightly in the direction of the mouse cursor.
        float cameraPosX = Mathf.SmoothDamp(transform.position.x, cameraTarget.position.x + swayOffset.x + cameraOffset.x + mouseDistanceX * 10f, ref cameraVelocityX, smoothTimeX);
        float cameraPosY = Mathf.SmoothDamp(transform.position.y, cameraTarget.position.y + swayOffset.x + cameraOffset.y + mouseDistanceY * 8f + locationBasedOffset, ref cameraVelocityY, smoothTimeY);

        // camera is capable of swaying slightly or shaking violently
        cameraPosX += shakeOffset.x;
        cameraPosY += shakeOffset.y;

        transform.position = new Vector3(cameraPosX, cameraPosY, transform.position.z);
    }

    // Update a Vector2 camera shake offset to apply to the camera's position
    private void updateCameraShake()
    {
        if (executeCameraShake)
        {
            float x = (Mathf.PerlinNoise(random, Time.time * shakeSpeed) * 2f - 1f) * shakeMaxMagnitude;
            float y = (Mathf.PerlinNoise(random + 20, Time.time * shakeSpeed) * 2f - 1f) * shakeMaxMagnitude;
            shakeOffset = new Vector2(x, y);
        }

        else
            shakeOffset = Vector2.zero;
    }

    // Update a Vector2 sway offset to apply to the camera's position
    private void updateCameraSway()
    {
        float x = (Mathf.PerlinNoise(420f, Time.time * swaySpeed) * 2f - 1f) * swayMaxMagnitude;
        float y = (Mathf.PerlinNoise(420420f, Time.time * swaySpeed) * 2f - 1f) * swayMaxMagnitude;
        swayOffset = new Vector2(x, y);
    }
}