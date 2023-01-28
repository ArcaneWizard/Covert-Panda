using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAround : CentralLookAround
{
    protected override void figureOutDirectionToLookIn()
    {
        Vector3 weaponPivotPos = weaponSystem.CurrentWeaponConfiguration.WeaponPivot.position;
        directionToLook = (Input.mousePosition - camera.WorldToScreenPoint(weaponPivotPos)).normalized;
    }

    protected override void updateDirectionCreatureFaces() 
    {
        if (Input.mousePosition.x >= camera.WorldToScreenPoint(transform.position).x && body.localRotation.y != 0) 
        {
            body.localRotation = Quaternion.Euler(0, 0, 0);
            controller.UpdateTiltInstantly();
        }
        else if (Input.mousePosition.x < camera.WorldToScreenPoint(transform.position).x && body.localRotation.y == 0) 
        {
            body.localRotation = Quaternion.Euler(0, 180, 0);
            controller.UpdateTiltInstantly();
        }
    }

}
