using UnityEngine;

public class Teleportation : MonoBehaviour
{
    public Transform teleportationZones;
    private CentralController controller;
    private System.Random random;

    //how long you have to wait to teleport again (after just teleporting)
    private float reloadTime = 1f;
    private float reloadTimer;

    void Awake()
    {
        random = new System.Random();
        controller = transform.GetComponent<CentralController>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.F) && reloadTimer <= 0f) {
            reloadTimer = reloadTime;
            teleport();
        }

        if (reloadTimer > 0f)
            reloadTimer -= Time.deltaTime;
    }

    private void teleport()
    {
        Vector3 nextLocation = teleportationZones.transform.GetChild(random.Next(0, teleportationZones.childCount)).position;
        transform.position = new Vector3(nextLocation.x, nextLocation.y, transform.position.z);
        controller.UpdateTiltInstantly();
    }
}
