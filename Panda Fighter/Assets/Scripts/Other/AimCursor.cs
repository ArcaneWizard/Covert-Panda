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
    private Transform player;

    private float zoom = 0.17f;
    public float defaultZoom = 0.3f;
    public float accuracyDistance = 9f;
    public float accuracyFallRate = 0.9f;

    void Awake()
    {
        zoom = defaultZoom;
        player = transform.parent.GetChild(0);
    }

    void Update()
    {
        upArrow.transform.localPosition = new Vector2(0, zoom);
        downArrow.transform.localPosition = new Vector2(0, -zoom);
        rightArrow.transform.localPosition = new Vector2(zoom, 0);
        leftArrow.transform.localPosition = new Vector2(-zoom, 0);

        Vector3 pos = camera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(pos.x, pos.y, 0);

        zoom = defaultZoom + Mathf.Pow(magnitude2D(pos, player.position) / accuracyDistance, accuracyFallRate);
    }

    private float magnitude2D(Vector3 a, Vector3 b) => new Vector2(a.x - b.x, a.y - b.y).magnitude;
}
