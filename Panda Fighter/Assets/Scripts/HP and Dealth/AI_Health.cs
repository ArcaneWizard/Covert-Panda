using UnityEngine;

public class AI_Health : Health
{
    protected ICentralAbilityHandler abilityHandler;

    protected override void Awake()
    {
        base.Awake();
        abilityHandler = transform.GetComponent<ICentralAbilityHandler>();
        MaxHP = 400;
    }

    public override void InflictDamage(int damage, Transform attacker = null)
    {
        if (abilityHandler.IsInvulnerable)
            return;

        base.InflictDamage(damage, attacker);
    }
}
