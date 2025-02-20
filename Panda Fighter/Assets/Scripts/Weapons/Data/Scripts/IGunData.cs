
public interface IGunData : IWeaponData
{
    HowGunIsHeld HowGunIsHeld { get; set; }
    int StartingAmmo { get; set; }
    int ReloadDuration { get; set; }
    int BulletSpeed { get; set; }
    int MaxRangeByAI { get; set; }
}
