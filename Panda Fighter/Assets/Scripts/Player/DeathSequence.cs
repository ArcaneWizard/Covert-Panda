using UnityEngine;

public class DeathSequence : CentralDeathSequence
{
    public RespawningText respawnText;
    public GameObject inventory;

    protected override void actionsTriggeredImmediatelyUponDeath()
    {
        inventory.SetActive(false);
        respawnText.StartRespawnCountdown(respawnTime);
        base.actionsTriggeredImmediatelyUponDeath();
    }

    protected override void actionsTriggeredWhenRespawning()
    {
        inventory.SetActive(true);
        base.actionsTriggeredWhenRespawning();
    }
}