using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAround : CentralLookAround
{
    [SerializeField] private Transform bodyMidLine;

    protected override void figureOutDirectionToLookIn()
    {
        Vector3 weaponPivotPos = weaponSystem.CurrentWeaponConfiguration.WeaponPivot.position;
        directionToLook = (Input.mousePosition - camera.WorldToScreenPoint(weaponPivotPos)).normalized;
    }
}