using UnityEngine;

public class DeathSequence : CentralDeathSequence
{
    public RespawningText respawnText;
    public GameObject inventory;

    protected override void actionsTriggeredImmediatelyUponDeath()
    {
        base.actionsTriggeredImmediatelyUponDeath();
        inventory.SetActive(false);
        respawnText.StartRespawnCountdown(respawnTime);
    }

    protected override void actionsTriggeredWhenRespawning()
    {
        inventory.SetActive(true);
        base.actionsTriggeredWhenRespawning();
    }
}