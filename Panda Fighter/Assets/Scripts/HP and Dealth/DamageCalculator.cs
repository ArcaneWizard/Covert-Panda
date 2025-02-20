using UnityEngine;

public static class DamageCalculator
{
    public static int CalculateDmg(int baseDmg, Transform recipient, Transform attacker = null)
    {
        /* 
           let ODM = offensive damage multiplier
           let DDM = defensive damage multiplier
        
           If attacker is provided:
               dmg = (float)baseDmg * attacker.Perks.ODM * recipient.Perks.DDM * recipient.Armor.DDM

           If attacker isn't provided (ex. dmg inflicted by environment):
               dmg = (float)baseDmg * recipient.Perks.DDM * recipient.Armor.DDM

           Notes:
               - also factor in weapon type (physical, electrical) and creature type (if they're not wearing armor)
               - invulnerability -> recipient.Perks.DDM = 0
               - are perks and abilities separate things in this game?
        */

        return baseDmg;
    }
}
