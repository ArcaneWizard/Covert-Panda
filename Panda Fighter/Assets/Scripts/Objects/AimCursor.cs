using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimCursor : MonoBehaviour
{
    public Transform upArrow;
    public Transform downArrow;
    public Transform rightArrow;
    public Transform leftArrow;

    public Camera camera;
    public Transform player;

    private float zoom = 0.17f;
    public float defaultZoom = 0.17f;
    public float accuracyDistance = 9f;
    public float accuracyFallRate = 1.2f;

    void Start()
    {
        zoom = defaultZoom;
    }

    void Update()
    {
        upArrow.transform.localPosition = new Vector2(0, zoom);
        downArrow.transform.localPosition = new Vector2(0, -zoom);
        rightArrow.transform.localPosition = new Vector2(zoom, 0);
        leftArrow.transform.localPosition = new Vector2(-zoom, 0);

        Vector3 pos = camera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(pos.x, pos.y, 0);

        zoom = defaultZoom * Mathf.Pow((pos - player.position).magnitude / accuracyDistance, accuracyFallRate);
    }
}
