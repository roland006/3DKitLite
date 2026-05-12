using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using Indiemount.Spawner;

[CustomEditor(typeof(WaveSpawner))]
public class WaveSpawnerEditor : Editor
{
    #region Private Member
    WaveSpawner ws;
    SerializedObject GetTarget;
    SerializedProperty WaveSettingsList;

    List<bool> property;
    List<float> total;

    Vector2 scrollPosition;
    #endregion

    #region SerializedProperty
    SerializedProperty ListRef;
    SerializedProperty maxCountObject;
    SerializedProperty totalWaveCount;
    SerializedProperty timeBetweenWaves;
    SerializedProperty timeBetweenSpawn;
    SerializedProperty spawnList;
    SerializedProperty waveType;

    SerializedProperty spawnRef;
    SerializedProperty tagOfObjects;
    SerializedProperty spawnPercentage;
    #endregion

    private void OnEnable()
    {
        ws = (WaveSpawner)target;
        GetTarget = new SerializedObject(ws);

        WaveSettingsList = GetTarget.FindProperty("waveSettings");
        property = new List<bool>();
        total = new List<float>();
        for (int i = 0; i < WaveSettingsList.arraySize; i++)
        {
            int x = PlayerPrefs.GetInt("ToogleBoolWSE" + i, 0);
            property.Add(x == 0 ? false : true);
            total.Add(0);

            ListRef = WaveSettingsList.GetArrayElementAtIndex(i);
            totalWaveCount = ListRef.FindPropertyRelative("totalWaveCount");
            maxCountObject = ListRef.FindPropertyRelative("maxCountObjects");
            timeBetweenSpawn = ListRef.FindPropertyRelative("timeBetweenSpawn");
            timeBetweenWaves = ListRef.FindPropertyRelative("timeBetweenWaves");
            spawnList = ListRef.FindPropertyRelative("spawn");
            waveType = ListRef.FindPropertyRelative("waveType");

            for (int j = 0; j < spawnList.arraySize; j++)
            {

                spawnRef = spawnList.GetArrayElementAtIndex(j);
                tagOfObjects = spawnRef.FindPropertyRelative("tagOfObject");
                spawnPercentage = spawnRef.FindPropertyRelative("spawnPercentage");
            }
        }
    }

    private void OnDisable()
    {
        int x = 0;
        foreach (var item in property)
        {
            PlayerPrefs.SetInt("ToogleBoolWSE" + x, item == false ? 0 : 1);
            x++;
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GetTarget.Update();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Wave Settings", EditorStyles.boldLabel);

        if (GUILayout.Button(new GUIContent("Add", "Add new element in Wave Setting")))
        {
            WaveSettingsList.arraySize++;
            PlayerPrefs.SetInt("ToogleBoolWSE" + WaveSettingsList.arraySize, 1);
            property.Add(true);
            total.Add(0);
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        for (int i = 0; i < WaveSettingsList.arraySize; i++)
        {
            GUI.backgroundColor = Color.white;
            property[i] = EditorGUILayout.BeginFoldoutHeaderGroup(property[i], new GUIContent("Element " + i.ToString()));

            if (property[i])
            {

                EditorGUILayout.Space();
                ListRef = WaveSettingsList.GetArrayElementAtIndex(i);
                totalWaveCount = ListRef.FindPropertyRelative("totalWaveCount");
                maxCountObject = ListRef.FindPropertyRelative("maxCountObjects");
                timeBetweenSpawn = ListRef.FindPropertyRelative("timeBetweenSpawn");
                timeBetweenWaves = ListRef.FindPropertyRelative("timeBetweenWaves");
                spawnList = ListRef.FindPropertyRelative("spawn");
                waveType = ListRef.FindPropertyRelative("waveType");

                EditorGUILayout.PropertyField(waveType);

                if (waveType.enumValueIndex == 0)
                {
                    EditorGUILayout.PropertyField(totalWaveCount);
                    EditorGUILayout.PropertyField(maxCountObject);
                    EditorGUILayout.PropertyField(timeBetweenSpawn);
                    EditorGUILayout.PropertyField(timeBetweenWaves);
                }
                else
                {
                    EditorGUILayout.PropertyField(timeBetweenSpawn);
                    totalWaveCount.intValue = 1;
                    maxCountObject.intValue = 1;
                    timeBetweenWaves.floatValue = 1;
                }

                EditorGUILayout.LabelField("_______________________________________________________________________________________________________________");
                EditorGUILayout.Space();
                GUI.backgroundColor = new Color(.8f, .8f, .8f);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Spawner Elements", EditorStyles.boldLabel);
                if (GUILayout.Button(new GUIContent("Add", "Add new element to Spawner"), GUILayout.Width(100)))
                    spawnList.arraySize++;

                EditorGUILayout.EndHorizontal();

                total[i] = 0;
                for (int j = 0; j < spawnList.arraySize; j++)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Element " + j, EditorStyles.boldLabel);
                    if (GUILayout.Button(new GUIContent("Remove", "Remove from the Pool"), GUILayout.Width(100)))
                        spawnList.DeleteArrayElementAtIndex(j);

                    EditorGUILayout.EndHorizontal();
                    spawnRef = spawnList.GetArrayElementAtIndex(j);
                    tagOfObjects = spawnRef.FindPropertyRelative("tagOfObject");
                    spawnPercentage = spawnRef.FindPropertyRelative("spawnPercentage");

                    EditorGUILayout.PropertyField(tagOfObjects);
                    EditorGUILayout.PropertyField(spawnPercentage);
                    total[i] += spawnPercentage.floatValue;


                }

                EditorGUILayout.LabelField("_______________________________________________________________________________________________________________");

                GUI.backgroundColor = Color.white;
                if (GUILayout.Button(new GUIContent("Remove", "Remove this element from Wave Settings")))
                {
                    WaveSettingsList.DeleteArrayElementAtIndex(i);
                    property.RemoveAt(i);
                    total.RemoveAt(i);
                }
                EditorGUILayout.Space();
                EditorGUILayout.Space();

            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        GetTarget.ApplyModifiedProperties();
    }


    /// <summary>
    /// To Calculate the Percentage of individual Spawner
    /// </summary>
    float Calculate(int i, float spawnRates)
    {
        if (spawnRates == -1)
            return 0;
        else
            return spawnRates / total[i];
    }

    void OnSceneGUI()
    {
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(20, 20, 180, 300));
        var rect = EditorGUILayout.BeginVertical();
        GUI.Box(rect, GUIContent.none);
        GUILayout.Label("Wave Settings Data", EditorStyles.boldLabel);
        GUI.color = Color.white;

        EditorGUILayout.LabelField("______________________");
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.Width(180), GUILayout.Height(250));

        for (int i = 0; i < WaveSettingsList.arraySize; i++)
        {
            SerializedProperty ListRef = WaveSettingsList.GetArrayElementAtIndex(i);
            totalWaveCount = ListRef.FindPropertyRelative("totalWaveCount");
            maxCountObject = ListRef.FindPropertyRelative("maxCountObjects");
            timeBetweenSpawn = ListRef.FindPropertyRelative("timeBetweenSpawn");
            timeBetweenWaves = ListRef.FindPropertyRelative("timeBetweenWaves");
            waveType = ListRef.FindPropertyRelative("waveType");
            spawnList = ListRef.FindPropertyRelative("spawn");

            float multSpawn = (float)((maxCountObject.intValue - 1) * timeBetweenSpawn.floatValue);
            float multWave = (float)((totalWaveCount.intValue - 1) * timeBetweenWaves.floatValue) + (float)(totalWaveCount.intValue * multSpawn);
            
            GUILayout.Label("Element: " + i, EditorStyles.boldLabel);
            if (waveType.enumValueIndex == 0)
            {
                GUILayout.Label("Time/Wave " + (int)multSpawn/60 +":"+ (int)(multSpawn % 60 )+ " min");
                GUILayout.Label("Total Time " + (int)(multSpawn + multWave)/60 + ":" + ((int)(multSpawn + multWave) % 60) + " min");
            }
            EditorGUILayout.LabelField("___________________");

            float y = Calculate(i, -1);
            for (int j = 0; j < spawnList.arraySize; j++)
            {
                SerializedProperty spawnRef = spawnList.GetArrayElementAtIndex(j);
                SerializedProperty tagOfObjects = spawnRef.FindPropertyRelative("tagOfObject");
                SerializedProperty spawnPercentage = spawnRef.FindPropertyRelative("spawnPercentage");

                y = Calculate(i, spawnPercentage.floatValue);
                GUILayout.Label(tagOfObjects.stringValue + ": " + Math.Round(y * 100, 2) + "%");
            }
            EditorGUILayout.LabelField("___________________");
        }

        GUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
        GUILayout.EndArea();
        Handles.EndGUI();
    }
}

