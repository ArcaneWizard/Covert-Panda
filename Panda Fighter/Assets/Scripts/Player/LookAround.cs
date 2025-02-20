using UnityEngine;

public class LookAround : CentralLookAround
{
    protected override void figureOutDirectionToLookIn()
    {
        Vector3 weaponPivotPos = weaponSystem.CurrentWeaponConfiguration.WeaponPivot.position;
        DirectionToLook = (Input.mousePosition - camera.WorldToScreenPoint(weaponPivotPos)).normalized;
    }
}
