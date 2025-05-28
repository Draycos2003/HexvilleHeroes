using UnityEngine;

public class ShowIfAttribute : PropertyAttribute
{
    public string ConditionField;
    public object DesiredValue;

    public ShowIfAttribute(string conditionField, object desiredValue)
    {
        ConditionField = conditionField;
        DesiredValue = desiredValue;
    }
}
