using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyAI))]
public class enemyEditor : Editor
{
    public override void OnInspectorGUI()
    {              
        EnemyAI stats = (EnemyAI)target; // target is the object that will be inspected. Meaning the enemy, And we cast the custom editor to it.

        // All the fields for the enemy
        SerializedProperty HP = serializedObject.FindProperty("HP");
        SerializedProperty model = serializedObject.FindProperty("model");
        SerializedProperty FTS = serializedObject.FindProperty("faceTargetSpeed");
        SerializedProperty targets = serializedObject.FindProperty("target");
        SerializedProperty enemyType = serializedObject.FindProperty("EnemyType");
       
        SerializedProperty shootPoint = serializedObject.FindProperty("shootPos");
        SerializedProperty projectile = serializedObject.FindProperty("bullet");
        SerializedProperty fireRate = serializedObject.FindProperty("shootRate");
       
        SerializedProperty attackSpeed = serializedObject.FindProperty("attackSpeed");
        SerializedProperty weapon = serializedObject.FindProperty("weapon");
        SerializedProperty hitPoint = serializedObject.FindProperty("hitPos");
       
        //sync all the objects
        serializedObject.Update();

        //drawing the enum
        EditorGUILayout.PropertyField(enemyType);
       
        //apply changes ASAP
        serializedObject.ApplyModifiedProperties();
        
        // resync changes
        serializedObject.Update();

        // var to hold the enum
        var type = (EnemyAI.EnemyTypes)enemyType.enumValueIndex;

        // apply range enemy attributes
        if (type == EnemyAI.EnemyTypes.Ranged)
        {
            EditorGUILayout.PropertyField(HP);
            EditorGUILayout.PropertyField(model);
            EditorGUILayout.PropertyField(FTS);
            EditorGUILayout.PropertyField(targets);
            EditorGUILayout.PropertyField(shootPoint);
            EditorGUILayout.PropertyField(projectile);
            EditorGUILayout.PropertyField(fireRate);
        }
        // apply melee enemy attributes
        else if (type == EnemyAI.EnemyTypes.Melee)
        {
            EditorGUILayout.PropertyField(HP);
            EditorGUILayout.PropertyField(model);
            EditorGUILayout.PropertyField(FTS);
            EditorGUILayout.PropertyField(targets);
           
            EditorGUILayout.PropertyField(attackSpeed);
            EditorGUILayout.PropertyField(weapon);
            EditorGUILayout.PropertyField(hitPoint);
        }

        // save these changes
        serializedObject.ApplyModifiedProperties();
    } 
    
   
}
