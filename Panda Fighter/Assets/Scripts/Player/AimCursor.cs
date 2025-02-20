using UnityEngine;

public class AimCursor : MonoBehaviour
{
    public Transform UpArrow;
    public Transform DownArrow;
    public Transform RightArrow;
    public Transform LeftArrow;

    private new Camera camera;
    private Transform player;

    private float zoom = 0.17f;
    public float DefaultZoom = 0.3f;
    public float AccuracyDistance = 9f;
    public float AccuracyFallRate = 0.9f;

    void Awake()
    {
        zoom = DefaultZoom;
        player = transform.parent.GetChild(0);
        camera = transform.parent.parent.parent.GetComponent<References>().Camera;
    }

    void Update()
    {
        UpArrow.transform.localPosition = new Vector2(0, zoom);
        DownArrow.transform.localPosition = new Vector2(0, -zoom);
        RightArrow.transform.localPosition = new Vector2(zoom, 0);
        LeftArrow.transform.localPosition = new Vector2(-zoom, 0);

        Vector3 pos = camera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(pos.x, pos.y, 0);

        zoom = DefaultZoom + Mathf.Pow(magnitude2D(pos, player.position) / AccuracyDistance, AccuracyFallRate);
    }

    private float magnitude2D(Vector3 a, Vector3 b) => new Vector2(a.x - b.x, a.y - b.y).magnitude;
}
