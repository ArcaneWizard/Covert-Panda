using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

[ExecuteAlways]
public class IKTargets : MonoBehaviour
{
    public Transform MeeleePoleAim;
    public Transform ShortBarrelMainAim;
    public Transform ShortBarrelOtherAim;
    public Transform MediumBarrelAim;
    public Transform LongBarrelAim;
    public Transform PistolGripAim;
    public Transform ShoulderRestAim;

    private void StoreInitialFieldValues()
    {
        FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        initialFieldValues.Clear();
        foreach (FieldInfo field in fields) {
            if (field.IsDefined(typeof(SerializeField), false) || field.IsPublic)
                initialFieldValues.Add(field.Name, field.GetValue(this));
        }
    }

    private Dictionary<string, object> initialFieldValues = new Dictionary<string, object>();

    private void LogChangedFields()
    {
        FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (FieldInfo field in fields) {
            if (field.IsDefined(typeof(SerializeField), false) || field.IsPublic) {
                if (initialFieldValues.ContainsKey(field.Name)) {
                    string fieldName = field.Name;
                    object initialValue = initialFieldValues[fieldName];
                    object currentValue = field.GetValue(this);

                    if (!Equals(initialValue, currentValue)) {
                        Debug.Log($"{fieldName} was changed. Initial value: {initialValue}, Current value: {currentValue}");
                    }
                }
            }
        }
    }

    private void OnValidate()
    {
        LogChangedFields();
        StoreInitialFieldValues();
    }
}

