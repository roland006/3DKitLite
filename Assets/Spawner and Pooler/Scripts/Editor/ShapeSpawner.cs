using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Indiemount.Pooler;

namespace Indiemount.Spawner
{
    [CustomEditor(typeof(ObjectPooler))]
    public class ShapeSpawner : EditorWindow
    {
        #region Members
        GameObject spawnPrefab;

        Vector3 startPosition;
        Vector3 spacing;

        bool setParent;
        GameObject parent;

        Layout layout;
        enum Layout
        {
            rectangle = 0,
            triangle = 1,
            rightAngleTriangle = 2
        }

        Axis axis;
        enum Axis
        {
            XY = 0,
            XZ = 1,
            XYZ = 2
        }

        int rows = 1;
        int columns = 1;
        int height = 1;

        public int Rows { get => rows; set => rows = Mathf.Max(1, value); }
        public int Columns { get => columns; set => columns = Mathf.Max(1, value); }
        public int Height { get => height; set => height = Mathf.Max(1, value); }
        private void OnValidate() 
        {
            Rows = rows; 
            Columns = columns;
            Height = height;
        }

        int count;
        Transform initial;
        string label;
        bool addToPooler;
        bool canExpand;
        List<GameObject> currInstantiatedGO;

        ObjectPoolerEditor[] ope;
        SerializedProperty newItem;

        ObjectPooler op;
        public ShapeSpawnerSave sss;

        #endregion

        /// <summary>
        /// To Create a Window
        /// </summary>
        [MenuItem("Window/Shape Spawner")]
        static void OpenWindow()
        {
            ShapeSpawner window = (ShapeSpawner)GetWindow(typeof(ShapeSpawner), false, "Shape Spawner", true);
            window.Show();
        }

        private void OnGUI()
        {
            OnValidate();
            GUILayout.Label("Shape Spawn", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            op = FindObjectOfType<ObjectPooler>();

            spawnPrefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Spawn Prefab", "Prefab to Spawn"), spawnPrefab, typeof(GameObject), false);
            startPosition = (Vector3)EditorGUILayout.Vector3Field(new GUIContent("Start Position", "Input the Start Position of the First Spawn Prefab"), startPosition);
            spacing = (Vector3)EditorGUILayout.Vector3Field(new GUIContent("Padding", "Input the Padding between two Prefabs in XYZ axis"), spacing);
            setParent = (bool)EditorGUILayout.Toggle(new GUIContent("Set Parent", "Check if you want to Spawn GameObject as a Child of a another GameObject"), setParent);

            if (setParent)
                parent = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Parent", "Set Parent GameObject"), parent, typeof(GameObject),true);


            layout = (Layout)EditorGUILayout.EnumPopup(new GUIContent("Layout", "Select the Shape Layout for Spawner"), layout);
            axis = (Axis)EditorGUILayout.EnumPopup(new GUIContent("Axis", "Select the Axis for Spawner"), axis);
        
            if (layout == Layout.rectangle)
            {
                rows = (int)EditorGUILayout.IntField(new GUIContent("Rows", "Number of Rows in X axis"), rows);
                columns = (int)EditorGUILayout.IntField(new GUIContent("Columns", "Number of Columns in Y axis"), columns);

                if (axis == Axis.XYZ)
                    height = (int)EditorGUILayout.IntField(new GUIContent("Height", "Size of Height in Z axis"), height);
                
            }
            else if (layout == Layout.triangle)
            {
                rows = (int)EditorGUILayout.IntField(new GUIContent("Base", "Size of Base"), rows);

            }
            else if (layout == Layout.rightAngleTriangle)
            {
                rows = (int)EditorGUILayout.IntField(new GUIContent("Base", "Size of Base"), rows);

            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
        
            GUILayout.Label("Pooler", EditorStyles.boldLabel);
            addToPooler = (bool)EditorGUILayout.Toggle(new GUIContent("Add To Pooler", "Check if you want to Add this element into Object Pooler"), addToPooler); 
            if (addToPooler)
            {
                label = (string)EditorGUILayout.TextField(new GUIContent("Tag", "Tag to Represent element in the Object Pooler"), label); 
                canExpand = (bool)EditorGUILayout.Toggle(new GUIContent("Can Expand", "Check if you want to Expand the Pool of this GameObject at Runtime"), canExpand);
            }

            if (GUILayout.Button(new GUIContent("Generate", "Instantiate the Spawner")))
            {
                initial = spawnPrefab.transform;
                Debug.ClearDeveloperConsole();
                currInstantiatedGO = new List<GameObject>();
                count = 0;

                if (addToPooler)
                {
                    ope = (ObjectPoolerEditor[])Resources.FindObjectsOfTypeAll(typeof(ObjectPoolerEditor));
                    if (ope.Length == 0)
                        Debug.LogError("Select the ObjectPooler GameObject");
                    else
                    {
                        newItem = ope[0].NewItem();
                        Generate();
                        Debug.Log("Generated " + count + " numbers of Prefabs");
                    }
                    GenerateTag();
                    foreach (var item in currInstantiatedGO)
                        item.tag = label;

                    if (ope.Length != 0)
                        ope[0].AddElement(newItem, label, spawnPrefab, count, setParent, parent, canExpand);
                }
                else
                {
                    Generate();
                    Debug.Log("Generated " + count + " numbers of Prefabs");
                }

                if (setParent)
                    SetToParent();

                addToPooler = false;
            }
            EditorUtility.SetDirty(op);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            GUILayout.Label("Save System", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            label = (string)EditorGUILayout.TextField(new GUIContent("File Name", "If the File Name exist it with get Replaced by the recent value. Otherwise, it will Create a New File."), label);
            if (GUILayout.Button("Save"))
            {
                Save();
                EditorUtility.SetDirty(sss); 
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            sss = (ShapeSpawnerSave)EditorGUILayout.ObjectField(new GUIContent("Load Asset", "Load Previously Saved File"),sss,typeof(ShapeSpawnerSave));
            if (GUILayout.Button("Load"))
            {
                Load();
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// To Add a New Tag in the Current Project
        /// </summary>
        void GenerateTag()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            bool found = false;

            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(label)) { found = true; break; }
            }

            if (!found)
            {
                tagsProp.InsertArrayElementAtIndex(0);
                SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
                n.stringValue = label;
                tagManager.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        #region Generate Shape

        /// <summary>
        /// To Generate Layout of the Spawner
        /// </summary>
        void Generate()
        {
            switch (layout)
            {
                case Layout.rectangle:
                    Rectangle();
                    break;

                case Layout.triangle:
                    Triangle();
                    break;

                case Layout.rightAngleTriangle:
                    RightAngleTriangle();
                    break;
            }
        }

        /// <summary>
        /// If setParent is True, make Spawned GameObject as child of Parent
        /// </summary>
        void SetToParent()
        {
            foreach (var item in currInstantiatedGO)
                item.transform.parent = parent.transform;
        }

        /// <summary>
        /// For Generating Rectangle
        /// </summary>
        void Rectangle()
        {
            Transform t = spawnPrefab.transform;
            var p = startPosition;

            switch (axis)
            {
                case Axis.XY:
                    for (int i = 0; i < rows; i++)
                    {
                        p = startPosition + new Vector3(0, i * spacing.y, 0);
                        for (int j = 0; j < columns; j++)
                        {
                            var temp = Instantiate(spawnPrefab, p, Quaternion.identity);

                            t.position += new Vector3(spacing.x, 0, 0);
                            p += new Vector3(spacing.x, 0, 0);
                                count++;
                        
                            currInstantiatedGO.Add(temp);
                        }
                    }
                    break;

                case Axis.XZ:
                    for (int i = 0; i < rows; i++)
                    {
                        p = startPosition + new Vector3(0, 0, i * spacing.z);
                        for (int j = 0; j < columns; j++)
                        {
                            var temp = Instantiate(spawnPrefab,p, Quaternion.identity);
                
                            t.position += new Vector3(spacing.x,0,0);
                            p += new Vector3(spacing.x,0,0);
                                count++;

                            currInstantiatedGO.Add(temp);
                        }
                    }
                    break;

                case Axis.XYZ:
                    for (int k = 0; k < height; k++)
                    {
                        for (int i = 0; i < rows; i++)
                        {
                            p = startPosition + new Vector3(0, k*spacing.y, i * spacing.z);
                            for (int j = 0; j < columns; j++)
                            {
                                var temp = Instantiate(spawnPrefab, p, Quaternion.identity);

                                p += new Vector3(spacing.x, 0, 0);
                                count++;

                                currInstantiatedGO.Add(temp);
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// For Generating Triangle
        /// </summary>
        void Triangle()
        {
            var p = startPosition;
            var x = p;

            var mult = 1;
            var multo = 0;
            int n = 1;
            int h;
            Vector3 nStartPos = startPosition;
            Vector3 oStartPos = startPosition;
            switch (axis)
            {
                case Axis.XY:
                    for (int i = 0; i < rows; i++)
                    {
                        p = startPosition + new Vector3(0, 0, i * spacing.z);
                        x = p;
                        mult = 1;
                        multo = 0;
                        for (int j = 0; j < n; j++)
                        {
                            if (n%2 != 0)
                            {
                                if (j % 2 != 0)
                                {
                                    p = startPosition + new Vector3(-1*multo * spacing.x, i * spacing.y, 0);
                                }
                                else
                                {
                                    p = startPosition + new Vector3(multo * spacing.x, i * spacing.y, 0);
                                    multo++;
                                }
                                var temp = Instantiate(spawnPrefab, p, Quaternion.identity);
                                currInstantiatedGO.Add(temp);
                            }
                            else
                            {
                                if (j % 2 != 0)
                                {
                                    p = startPosition + new Vector3(-1*mult * spacing.x/2, i * spacing.y, 0);
                                    mult = mult + 2;
                                }
                                else
                                {
                                    p = startPosition + new Vector3(mult * spacing.x/2, i * spacing.y, 0);
                                }
                                var temp = Instantiate(spawnPrefab, p, Quaternion.identity);
                                currInstantiatedGO.Add(temp);

                            }
                            count++;    
                        }
                        n++;
                    }
                    break;

                case Axis.XZ:
                    for (int i = 0; i < rows; i++)
                    {
                        p = startPosition + new Vector3(0, 0, i * spacing.z);
                        x = p;
                        mult = 1;
                        multo = 0;
                        for (int j = 0; j < n; j++)
                        {
                            if (n % 2 != 0)
                            {
                                if (j % 2 != 0)
                                {
                                    p = startPosition + new Vector3(-1 * multo * spacing.x, 0, i * spacing.z);
                                }
                                else
                                {
                                    p = startPosition + new Vector3(multo * spacing.x, 0, i * spacing.z);
                                    multo++;
                                }
                                var temp = Instantiate(spawnPrefab, p, Quaternion.identity);
                                currInstantiatedGO.Add(temp);

                            }
                            else
                            {
                                if (j % 2 != 0)
                                {
                                    p = startPosition + new Vector3(-1 * mult * spacing.x / 2, 0, i * spacing.z);
                                    mult = mult + 2;
                                }
                                else
                                {
                                    p = startPosition + new Vector3(mult * spacing.x / 2, 0, i * spacing.z);
                                }
                                var temp = Instantiate(spawnPrefab, p, Quaternion.identity);
                                currInstantiatedGO.Add(temp);

                            }
                            count++;
                        }
                        n++;
                    }
                    break;

                case Axis.XYZ:
                    h = rows;
                    n = 0;
                    for (int k = 0; k < rows; k++)
                    {
                        oStartPos = nStartPos;
                        for (int i = 0; i < h-(2*n); i++)
                        {
                            p = k == 0? oStartPos + new Vector3(0, 0, i * spacing.z) : oStartPos + new Vector3(0, spacing.y, i * spacing.z);
                            for (int j = 0; j < h - (2*n); j++)
                            {
                                var temp = Instantiate(spawnPrefab, p, Quaternion.identity);

                                if (j == 1 && i == 1)
                                {
                                    nStartPos = p;
                                }

                                p += new Vector3(spacing.x, 0, 0);
                                count++;

                                currInstantiatedGO.Add(temp);
                            }
                        }
                        n++;
                    }
                    //startPosition = currInstantiatedGO[0].transform.position;
                    break;
            }
        }

        /// <summary>
        /// For Generating Right Angled Triangle
        /// </summary>
        void RightAngleTriangle()
        {
            var p = startPosition;
            var x = p;

            int n = 1;
            int h;
            Vector3 nStartPos = startPosition;
            Vector3 oStartPos = startPosition;

            switch (axis)
            {
                case Axis.XY:
                    for (int i = 0; i < rows; i++)
                    {
                        p = startPosition + new Vector3(0, i*spacing.y, 0);
                        for (int j = 0; j < n; j++)
                        {
                            var temp = Instantiate(spawnPrefab, p, Quaternion.identity);
                            p += new Vector3(spacing.x, 0, 0);
                            count++;

                            currInstantiatedGO.Add(temp);
                        }
                        n++;
                    }
                    break;
            
                case Axis.XZ:
                    for (int i = 0; i < rows; i++)
                    {
                        p = startPosition + new Vector3(0, 0, i * spacing.z);
                        for (int j = 0; j < n; j++)
                        {
                            var temp = Instantiate(spawnPrefab, p, Quaternion.identity);
                            p += new Vector3(spacing.x, 0, 0);
                            count++;

                            currInstantiatedGO.Add(temp);
                        }
                        n++;
                    }
                    break;

                case Axis.XYZ:
                    h = rows;
                    n = 0;
                    for (int k = 0; k < rows; k++)
                    {
                        oStartPos = nStartPos;
                        for (int i = 0; i < h - n; i++)
                        {
                            p = k == 0? oStartPos + new Vector3(0, 0, i * spacing.z) : oStartPos + new Vector3(0, spacing.y, i * spacing.z);
                            for (int j = 0; j < h - (n); j++)
                            {
                                var temp = Instantiate(spawnPrefab, p, Quaternion.identity);

                                if (j == 0 && i == 1)
                                {
                                    nStartPos = p;
                                }

                                p += new Vector3(spacing.x, 0, 0);
                                count++;
                        
                                currInstantiatedGO.Add(temp);
                            }
                        }
                        n++;
                    }
                    //startPosition = currInstantiatedGO[0].transform.position;
                    break;
            }
        }

        #endregion

        #region SaveSystem

        /// <summary>
        /// To Save
        /// </summary>
        void Save()
        {
            SearchFile();
            sss.SpawnPrefab = spawnPrefab;
            sss.StartPosition = startPosition;
            sss.Spacing = spacing;
            sss.SetParent = setParent;
            sss.parent = parent;
            sss.LayoutInt = (int)layout;
            sss.AxisInt = (int)axis;
            sss.Rows = Rows;
            sss.Columns = Columns;
            sss.Height = Height;
            sss.Display();
        }

        /// <summary>
        /// To Load
        /// </summary>
        void Load()
        {
            if (sss == null)
                return;

            spawnPrefab = sss.SpawnPrefab;
            startPosition = sss.StartPosition;
            spacing = sss.Spacing;
            setParent = sss.setParent;
            parent = sss.parent;
            layout = (Layout)sss.LayoutInt;
            axis = (Axis)sss.AxisInt;
            Rows = sss.Rows;
            Columns = sss.Columns;
            Height = sss.Height;
            sss.Display();
        }

        /// <summary>
        /// To Search a File in the Resource Folder
        /// </summary>
        void SearchFile()
        {
            var fileInfo = Resources.Load<ShapeSpawnerSave>("Data/ShapeSpawner/"+label);

            if (fileInfo == null)
                CreateMyAsset();
            else
            {
                sss = fileInfo; 
                Debug.Log("File " + label + " Updated! at "+ "Resources/Data/ShapeSpawner/"+label);
            }
        }

        /// <summary>
        /// To Create a New File
        /// </summary>
        void CreateMyAsset()
        {
            if (label == null || label == "")
                label = "New";

            sss = ScriptableObject.CreateInstance<ShapeSpawnerSave>();
            
            int x = 0;
            bool check = true;

            if (!AssetDatabase.IsValidFolder("Assets/Resources/Data/ShapeSpawner"))
            {
                x = 1;
                if (!AssetDatabase.IsValidFolder("Assets/Resources/Data"))
                {
                    x = 2;
                    if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                        x = 3;
                }
                check = false;
            }
            
            if (!check)
            {
                switch (x)
                {
                    case 1:
                        AssetDatabase.CreateFolder("Assets/Resources/Data", "ShapeSpawner");
                        break;

                    case 2:
                        AssetDatabase.CreateFolder("Assets/Resources", "Data");
                        AssetDatabase.CreateFolder("Assets/Resources/Data", "ShapeSpawner");
                        break;

                    case 3:
                        AssetDatabase.CreateFolder("Assets", "Resources");
                        AssetDatabase.CreateFolder("Assets/Resources", "Data");
                        AssetDatabase.CreateFolder("Assets/Resources/Data", "ShapeSpawner");
                        break;
                }
            }

            AssetDatabase.CreateAsset(sss, "Assets/Resources/Data/ShapeSpawner/" + label + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("New File " + label + " Created! at Assets/Resources/Data/ShapeSpawner/" + label);
            Selection.activeObject = sss;
        }
        #endregion
    }
}
