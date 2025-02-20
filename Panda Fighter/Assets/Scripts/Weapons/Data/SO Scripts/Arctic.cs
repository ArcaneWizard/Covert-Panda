using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "Weapon/SingleFireGunWithExplosion")]
public class Arctic : ScriptableObject, ISingleFireGunData, IExplosionData
{
    public string Name { get; set; }

    [field: SerializeField] public HowGunIsHeld HowGunIsHeld { get; set; }
    [field: SerializeField] public int DmgPerShot { get; set; }
    [field: SerializeField] public int ShotsPerSecond { get; set; }
    [field: SerializeField] public int BulletSpeed { get; set; }
    [field: SerializeField] public int ReloadDuration { get; set; }
    [field: SerializeField] public int StartingAmmo { get; set; }
    [field: SerializeField] public int ExplosionDmg { get; set; }
    [field: SerializeField] public int MaxRangeByAI { get; set; }
}
