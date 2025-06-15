using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class ShowIfAttribute : PropertyAttribute
{
    public string FieldName { get; }
    public object[] CompareValues { get; }
    public ShowIfAttribute(string fieldName, params object[] compareValues)
    {
        FieldName = fieldName;
        CompareValues = compareValues;
    }
}
