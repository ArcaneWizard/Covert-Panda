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

    // Note: the pivot (in this case the body) should be fixed regardless of which direction the creature faces
    // this ensures body flickering won't occur when looking up with the mouse directly
    protected override void updateDirectionBodyFaces()
    {
        if (Input.mousePosition.x >= camera.WorldToScreenPoint(bodyMidLine.position).x)
            body.localRotation = Quaternion.Euler(0, 0, 0);
        else
            body.localRotation = Quaternion.Euler(0, 180, 0);
    }
}