using UnityEditor;

[CustomEditor(typeof(pickupItemStats))]
public class CustomItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        pickupItemStats stats = (pickupItemStats)target;

        SerializedProperty IType = serializedObject.FindProperty("IType");

        SerializedProperty tile = serializedObject.FindProperty("tile");
        SerializedProperty model = serializedObject.FindProperty("model");

        SerializedProperty description = serializedObject.FindProperty("description");
        SerializedProperty stackable = serializedObject.FindProperty("stackable");
        SerializedProperty maxStack = serializedObject.FindProperty("maxStack");
        SerializedProperty icon = serializedObject.FindProperty("icon");

        SerializedProperty hitFX = serializedObject.FindProperty("hitFX");
        SerializedProperty hitSound = serializedObject.FindProperty("hitSound");
        SerializedProperty volume = serializedObject.FindProperty("volume");

        SerializedProperty damgageAmount = serializedObject.FindProperty("damgageAmount");
        SerializedProperty shootRange = serializedObject.FindProperty("shootRange");
        SerializedProperty shootRate = serializedObject.FindProperty("shootRate");

        SerializedProperty buffAmount = serializedObject.FindProperty("buffAmount");

        //sync all the objects
        serializedObject.Update();

        //drawing the enum
        EditorGUILayout.PropertyField(IType);

        //apply changes ASAP
        serializedObject.ApplyModifiedProperties();

        // resync changes
        serializedObject.Update();

        // var to hold the enum
        var itemType = (pickupItemStats.ItemType)IType.enumValueIndex;

        if (itemType == pickupItemStats.ItemType.melee)
        {
            EditorGUILayout.PropertyField(tile);
            EditorGUILayout.PropertyField(model);

            EditorGUILayout.PropertyField(description);
            EditorGUILayout.PropertyField(stackable);
            EditorGUILayout.PropertyField(maxStack);
            EditorGUILayout.PropertyField(icon);

            EditorGUILayout.PropertyField(hitFX);
            EditorGUILayout.PropertyField(hitSound);
            EditorGUILayout.PropertyField(volume);

            EditorGUILayout.PropertyField(damgageAmount);
        }
        // apply melee enemy attributes
        else if (itemType == pickupItemStats.ItemType.ranged)
        {
            EditorGUILayout.PropertyField(tile);
            EditorGUILayout.PropertyField(model);

            EditorGUILayout.PropertyField(description);
            EditorGUILayout.PropertyField(stackable);
            EditorGUILayout.PropertyField(maxStack);
            EditorGUILayout.PropertyField(icon);

            EditorGUILayout.PropertyField(hitFX);
            EditorGUILayout.PropertyField(hitSound);
            EditorGUILayout.PropertyField(volume);

            EditorGUILayout.PropertyField(damgageAmount);
            EditorGUILayout.PropertyField(shootRange);
            EditorGUILayout.PropertyField(shootRate);
        }
        else if (itemType == pickupItemStats.ItemType.consumable)
        {
            EditorGUILayout.PropertyField(tile);
            EditorGUILayout.PropertyField(model);

            EditorGUILayout.PropertyField(description);
            EditorGUILayout.PropertyField(stackable);
            EditorGUILayout.PropertyField(maxStack);
            EditorGUILayout.PropertyField(icon);

            EditorGUILayout.PropertyField(hitFX);
            EditorGUILayout.PropertyField(hitSound);
            EditorGUILayout.PropertyField(volume);

            EditorGUILayout.PropertyField(buffAmount);
        }
        // save these changes
        serializedObject.ApplyModifiedProperties();
    }
}

