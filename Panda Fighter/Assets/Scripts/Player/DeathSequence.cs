using UnityEngine;

public class DeathSequence : CentralDeathSequence
{
    public RespawningText respawnText;
    public GameObject inventory;

    protected override void uponDying()
    {
        base.uponDying();
        inventory.SetActive(false);
        respawnText.StartRespawnCountdown(respawnTime);
    }

    protected override void rightBeforeRespawning()
    {
        inventory.SetActive(true);
        base.rightBeforeRespawning();
    }
}