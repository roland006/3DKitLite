using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Indiemount.Pooler;

[CustomEditor(typeof(ObjectPooler))]
public class ObjectPoolerEditor : Editor
{
    #region Member
    ObjectPooler op;
    SerializedObject GetTarget;
    SerializedProperty ObjectPoolerList;
    int ListSize;
    List<bool> property;
    #endregion

    private void OnEnable()
    {
        op = (ObjectPooler)target;
        GetTarget = new SerializedObject(op);
        ObjectPoolerList = GetTarget.FindProperty("objectPools");

        property = new List<bool>();
        for (int i = 0; i < ObjectPoolerList.arraySize; i++)
        {
            int x = PlayerPrefs.GetInt("ToogleBoolOPE" + i, 0);
            property.Add(x == 0 ? false : true);
        }

        if (ObjectPoolerList == null)
            Debug.Log("Error");

    }

    private void OnDisable()
    {
        int x = 0;
        foreach (var item in property)
        {
            PlayerPrefs.SetInt("ToogleBoolOPE" + x, item == false ? 0 : 1);
            x++;
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GetTarget.Update();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        ListSize = op.objectPools.Count;
        EditorGUILayout.LabelField("List Size: " + ListSize);

        if (ListSize != ObjectPoolerList.arraySize)
        {
            while (ListSize > ObjectPoolerList.arraySize)
            {
                ObjectPoolerList.InsertArrayElementAtIndex(ObjectPoolerList.arraySize);
            }
            while (ListSize < ObjectPoolerList.arraySize)
            {
                ObjectPoolerList.DeleteArrayElementAtIndex(ObjectPoolerList.arraySize - 1);
            }
        }

        if (GUILayout.Button("Add New Element"))
        {
            NewItem();
        }
        
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField("_______________________________________________________________________________________________________________");

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        for (int i = 0; i < ObjectPoolerList.arraySize; i++)
        {

            property[i] = EditorGUILayout.BeginFoldoutHeaderGroup(property[i], new GUIContent("Element " + i.ToString()));

            EditorGUILayout.BeginHorizontal();

            SerializedProperty MyListRef = ObjectPoolerList.GetArrayElementAtIndex(i);
            SerializedProperty tag = MyListRef.FindPropertyRelative("tag");
            EditorGUILayout.PropertyField(tag, new GUIContent("Tag", "Tag for this Pool of GameObjects"));

            EditorGUILayout.EndHorizontal();

            if (property[i])
            {
                SerializedProperty prefab = MyListRef.FindPropertyRelative("prefab");
                SerializedProperty maxCount = MyListRef.FindPropertyRelative("maxCount");
                SerializedProperty setParent = MyListRef.FindPropertyRelative("setParent");
                SerializedProperty parent = MyListRef.FindPropertyRelative("parent");
                SerializedProperty canExpand = MyListRef.FindPropertyRelative("canExpand");
                SerializedProperty poolingMethod = MyListRef.FindPropertyRelative("poolingMethod");

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(prefab);
                EditorGUILayout.PropertyField(maxCount);
                EditorGUILayout.PropertyField(poolingMethod);
                EditorGUILayout.PropertyField(canExpand);
                EditorGUILayout.PropertyField(setParent);

                if (setParent.boolValue)
                {
                    EditorGUILayout.PropertyField(parent);
                    EditorGUILayout.Space();
                }

                if (GUILayout.Button(new GUIContent("Remove", "Remove from the Pool")))
                    ObjectPoolerList.DeleteArrayElementAtIndex(i);

                EditorGUILayout.LabelField("_______________________________________________________________________________________________________________");
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GetTarget.ApplyModifiedProperties();
    }

    public SerializedProperty NewItem()
    {
        ObjectPoolerList.arraySize++;
        PlayerPrefs.SetInt("ToogleBoolOPE" + ListSize, 1);
        property.Add(true);

        return  ObjectPoolerList.GetArrayElementAtIndex(ObjectPoolerList.arraySize-1);
    }

    public void AddElement(SerializedProperty newItem, string newtag, GameObject newprefab, int newMaxCount, bool newSetParent, GameObject newParent, bool newCanExpand)
    {
        SerializedProperty MyListRef = ObjectPoolerList.GetArrayElementAtIndex(ObjectPoolerList.arraySize-1);
        SerializedProperty tag = MyListRef.FindPropertyRelative("tag");

        if (property[ObjectPoolerList.arraySize-1])
        {
            SerializedProperty prefab = MyListRef.FindPropertyRelative("prefab");
            SerializedProperty maxCount = MyListRef.FindPropertyRelative("maxCount");
            SerializedProperty setParent = MyListRef.FindPropertyRelative("setParent");
            SerializedProperty parent = MyListRef.FindPropertyRelative("parent");
            SerializedProperty canExpand = MyListRef.FindPropertyRelative("canExpand");
            SerializedProperty poolingMethod = MyListRef.FindPropertyRelative("poolingMethod");

            tag.stringValue = newtag;
            prefab.objectReferenceValue = newprefab;
            maxCount.intValue = newMaxCount;
            setParent.boolValue = newSetParent;
            parent.objectReferenceValue = newParent;
            canExpand.boolValue = newCanExpand;
            poolingMethod.enumValueIndex = 0;

            GetTarget.ApplyModifiedProperties();
        }
    }
}