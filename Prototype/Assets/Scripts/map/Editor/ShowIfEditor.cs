using UnityEditor;
using UnityEngine;
using System.Reflection;

[CustomEditor(typeof(MonoBehaviour), true)]
public class ShowIfEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var targetObj = target;
        var type = targetObj.GetType();
        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var field in fields)
        {
            if (field.IsDefined(typeof(HideInInspector), true)) continue;
            if (field.IsNotSerialized) continue;

            var property = serializedObject.FindProperty(field.Name);
            if (property == null) continue;

            var showIfAttr = field.GetCustomAttribute<ShowIfAttribute>();
            if (showIfAttr != null)
            {
                var conditionField = type.GetField(showIfAttr.ConditionField, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (conditionField != null)
                {
                    var currentValue = conditionField.GetValue(targetObj);
                    if (!Equals(currentValue, showIfAttr.DesiredValue))
                        continue;
                }
            }

            EditorGUILayout.PropertyField(property, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
