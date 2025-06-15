using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Linq;

[CustomEditor(typeof(MonoBehaviour), true)]
public class ShowIfEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var targetObj = target;
        var objectType = targetObj.GetType();
        var fields = objectType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var field in fields)
        {
            // Skip hidden or non-serialized fields
            if (field.IsDefined(typeof(HideInInspector), true)) continue;
            if (field.IsNotSerialized) continue;

            // Check for ShowIf attribute
            var showIf = field.GetCustomAttribute<ShowIfAttribute>();
            if (showIf != null)
            {
                // Get the control field info
                var controlField = objectType.GetField(showIf.FieldName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (controlField != null)
                {
                    var controlValue = controlField.GetValue(targetObj);
                    // Only show if any compare value matches the control value
                    bool shouldShow = showIf.CompareValues
                        .Any(val => Equals(controlValue, val));
                    if (!shouldShow)
                        continue;
                }
            }

            // Draw the property
            var property = serializedObject.FindProperty(field.Name);
            if (property != null)
                EditorGUILayout.PropertyField(property, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
