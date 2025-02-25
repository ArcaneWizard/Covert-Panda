
public class DeathSequence : CentralDeathSequence
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void uponDying()
    {
        base.uponDying();
        References.Instance.InventoryCanvas.SetActive(false);
        References.Instance.RespawnText.StartRespawnCountdown(RESPAWN_TIME);
    }

    protected override void rightBeforeRespawning()
    {
        References.Instance.InventoryCanvas.SetActive(true);
        base.rightBeforeRespawning();
    }
}
