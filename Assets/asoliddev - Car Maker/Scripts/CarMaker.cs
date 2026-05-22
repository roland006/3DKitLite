using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*! \mainpage Car Maker documentation
 * 
* <b>Thank you for purchasing Car Maker.</b><br>
* <br>For any question don't hesitate to contact me at : asoliddev@gmail.com
* <br>AssetStore Profile : https://assetstore.unity.com/publishers/38620 
* <br>ArtStation Profile : https://asoliddev.artstation.com/
* 
*  \subsection Basics
* Car maker is a easy and simple tool to create physics based veicles with a few steps. <br>
* Video how to : https://www.youtube.com/watch?v=maPDrjUBH8o&feature=youtu.be <br>
* How to use : <br>
* 1. Go to top menu and open Car Maker. <br>
* 2. Drag and drop a Car GameObject you wan't to use Car Maker on. <br>
* 3. Drag and drop Car Body mesh (GameObject with the car mesh on it). <br>
* 4. Click Auto Detect wheels or Select them manually. <br>
* 5. Adjust the corrent Car Body mesh rotation, to align with Unity's default Forward direction. <br>
* 6. Click Make Car button. <br>
* 7. Done. <br>
* 
* Car Controlls :  Arrows or W,S,A,D <br>
* Camera view : F1
* 
*/


[ExecuteInEditMode]
public class CarMaker : EditorWindow
{
    /// <summary>
    /// Car Object to make
    /// </summary>
    private GameObject carObject;

    /// <summary>
    /// Car settings of the car
    /// </summary>
    private CarSettings carSettings;

    /// <summary>
    /// Gameobject that contains the body mesh of the car
    /// </summary>
    private GameObject carBodyMesh;

    /// <summary>
    /// Front left Wheel mesh gameobject
    /// </summary>
    private GameObject wheelFrontLeft;

    /// <summary>
    /// Front right Wheel mesh gameobject
    /// </summary>
    private GameObject wheelFrontRight;

    /// <summary>
    /// back left Wheel mesh gameobject
    /// </summary>
    private GameObject wheelBackLeft;

    /// <summary>
    /// back right Wheel mesh gameobject
    /// </summary>
    private GameObject wheelBackRight;


   
    /// <summary>
    /// show advanced settings of the menu
    /// </summary>
    private bool showAdvancedSettings = false;

    /// <summary>
    /// Create a camera as well if true
    /// </summary>
    private bool makeCamera = true;

    /// <summary>
    /// Base rotation of car body mesh to match the default wheel forward direction
    /// </summary>
    public float meshRotationY = 0;

    /// <summary>
    /// how slippery the car should be, higher value = more slippery
    /// </summary>
    public float slipperiness = 1;

    /// <summary>
    /// Currently disabled because it throws errors
    /// </summary>
    Editor gameObjectEditor;
    GameObject lastGameObject;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Car Maker/Open")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(CarMaker));
    }

    void OnGUI()
    {
        ///if no car object selected
        if (carObject == null) {

            ///init car settings
            carSettings = new CarSettings();

            GUILayout.Label("Please Select Car GameObject!", EditorStyles.boldLabel);
        }

        carObject = (GameObject)EditorGUILayout.ObjectField(carObject, typeof(GameObject), true);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        /*
        if (carObject != null)
        {
            if (gameObjectEditor == null) {
                gameObjectEditor = Editor.CreateEditor(carObject);
                lastGameObject = carObject;

                carSettings = new CarSettings();
            }

            if (lastGameObject != carObject) {
                gameObjectEditor = Editor.CreateEditor(carObject);
                lastGameObject = carObject;

                carSettings = new CarSettings();
            }


            gameObjectEditor.OnPreviewGUI(GUILayoutUtility.GetRect(200, 200), GUIStyle.none);
        }

        */

        ///display car object preview texture
        if (carObject != null) {
            Texture2D tex2d = AssetPreview.GetAssetPreview(carObject);

            GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                   GUILayout.Label(tex2d);
                GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        ///display settings and buttons
        if (carObject != null)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Car Body Mesh");
            carBodyMesh = EditorGUILayout.ObjectField(carBodyMesh, typeof(Object), true) as GameObject;
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("Auto Detect Wheels"))
            {
                AutoDetectWheels();
            }

            EditorGUILayout.Space();


            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Wheel Front Left");
            wheelFrontLeft = EditorGUILayout.ObjectField(wheelFrontLeft, typeof(Object), true) as GameObject;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Wheel Front Right");
            wheelFrontRight = EditorGUILayout.ObjectField(wheelFrontRight, typeof(Object), true) as GameObject;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Wheel Back Left");
            wheelBackLeft = EditorGUILayout.ObjectField(wheelBackLeft, typeof(Object), true) as GameObject;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Wheel Back Right");
            wheelBackRight = EditorGUILayout.ObjectField(wheelBackRight, typeof(Object), true) as GameObject;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            meshRotationY = EditorGUILayout.FloatField("MeshRotation", meshRotationY);
            makeCamera = EditorGUILayout.Toggle("Make Camera", makeCamera);

            EditorGUILayout.Space();
            EditorGUILayout.Space(); 
            EditorGUILayout.Space();

            showAdvancedSettings = EditorGUILayout.Toggle("Advanced Settings", showAdvancedSettings);
            if (showAdvancedSettings) {
                EditorGUILayout.Space();

                EditorGUI.indentLevel++;

                carSettings.mass = EditorGUILayout.FloatField("mass", carSettings.mass);
                carSettings.drag = EditorGUILayout.FloatField("drag", carSettings.drag);
                carSettings.centerOfMass = EditorGUILayout.Vector3Field("centerOfMass", carSettings.centerOfMass);
                carSettings.motorTorque = EditorGUILayout.FloatField("motorTorque", carSettings.motorTorque);
                carSettings.steeringAngle = EditorGUILayout.FloatField("steeringAngle", carSettings.steeringAngle);

                slipperiness = EditorGUILayout.FloatField("slipperiness", slipperiness);
            }


            EditorGUILayout.Space();
            EditorGUILayout.Space();

            string carMeshBodyError = "ok";

            if (carBodyMesh == null)
                carMeshBodyError = "Please Select Car body Mesh!";

            else if (carBodyMesh.GetComponent<MeshRenderer>() == null)
                carMeshBodyError = "Wrong Car Body Mesh : Please Select the gameobject with car body Meshrenderer!";


            if(carMeshBodyError != "ok")
            {
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.red;
                style.fontSize = 15;
                style.fontStyle = FontStyle.Bold;
                EditorGUILayout.LabelField(carMeshBodyError, style);
            }
            else if (GUILayout.Button("Make Car"))
            {
                GameObject car = MakeCar();

                if (car != null)
                    if (makeCamera)
                        MakeCamera(car);
            }

        }


    }

    /// <summary>
    /// Create car prefab with the current car maker settings
    /// </summary>
    /// <returns></returns>
    private GameObject MakeCar()
    {
        ///get save path
        var path = EditorUtility.SaveFilePanelInProject(
           "Select new car location",
           "New Car",
           "prefab",
           "message");

        GameObject newCar = null;


        ///if car is null
        if (carObject == null) {
            Debug.LogError("Please Assign Car GameObject!");
        }
        else {
            ///init car maker Prefab GameObject
            newCar = new GameObject();
            newCar.transform.position = Vector3.zero;

            ///add rigidbody to root
            Rigidbody rb = newCar.AddComponent<Rigidbody>();
            rb.interpolation = RigidbodyInterpolation.Interpolate;
           
            ///calculate body mesh rotation
            Quaternion meshRot = Quaternion.Euler(Vector3.zero);
            meshRot = Quaternion.Euler(0, meshRotationY, 0);


            ///add car container
            GameObject carContainer = GameObject.Instantiate(carObject, Vector3.zero, meshRot, newCar.transform) as GameObject;
           
            ///get car body mesh
            GameObject carMesh = GetChildByName(carContainer, carBodyMesh.name);

            ///add collider to car body mesh
            Collider collider = carMesh.GetComponent<Collider>();
            if (collider !=null)
            {
                MeshCollider meshCollider = carMesh.GetComponent<MeshCollider>();
                if (meshCollider != null)
                    meshCollider.convex = true;

            }
            else
                carMesh.AddComponent<MeshCollider>().convex = true;



            ///create container for wheels
            GameObject wheelsContainer = new GameObject();
            wheelsContainer.name = "wheelsContainer";
            wheelsContainer.transform.parent = newCar.transform;
            wheelsContainer.transform.localPosition = Vector3.zero;
            wheelsContainer.transform.localRotation = meshRot;

            ///create wheels 
            WheelCollider wheelColliderFrontLeft = AddWheelCollider(wheelsContainer, wheelFrontLeft, "Front Left", true);
            WheelCollider wheelColliderFrontRight = AddWheelCollider(wheelsContainer, wheelFrontRight, "Front Right", true);
            WheelCollider wheelColliderBackLeft = AddWheelCollider(wheelsContainer, wheelBackLeft, "Back Left", false);
            WheelCollider wheelColliderBackRight = AddWheelCollider(wheelsContainer, wheelBackRight, "Back Right", false);


            ///Create the new car Prefab.
            GameObject newPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(newCar, path, InteractionMode.UserAction);
            
            ///set car root name
            newCar.name = newPrefab.name;

            ///add car CarControler
            CarControler carController = newCar.AddComponent<CarControler>();
            
            ///asign car settings
            carController.carSettings = carSettings;

            ///init axleInfos List
            carController.wheelAxleList = new List<WheelAxle>();

            ///create Front Wheels
            WheelAxle axelInfoFront = new WheelAxle();
            axelInfoFront.wheelColliderRight = wheelColliderFrontRight;
            axelInfoFront.wheelColliderLeft = wheelColliderFrontLeft;
            axelInfoFront.wheelMeshLeft = GetChildByName(carContainer,  wheelFrontLeft.name);
            axelInfoFront.wheelMeshRight = GetChildByName(carContainer, wheelFrontRight.name);
            axelInfoFront.motor = true;
            axelInfoFront.steering = true;
            carController.wheelAxleList.Add(axelInfoFront);

            ///create Back Wheels
            WheelAxle axelInfoBack = new WheelAxle();
            axelInfoBack.wheelColliderRight = wheelColliderBackRight;
            axelInfoBack.wheelColliderLeft = wheelColliderBackLeft;
            axelInfoBack.wheelMeshLeft = GetChildByName(carContainer, wheelBackLeft.name);
            axelInfoBack.wheelMeshRight = GetChildByName(carContainer, wheelBackRight.name);
            axelInfoBack.motor = false;
            axelInfoBack.steering = false;
            carController.wheelAxleList.Add(axelInfoBack);

        }


        return newCar;
    }

    /// <summary>
    /// Create Camera
    /// </summary>
    /// <param name="car"></param>
    private void MakeCamera(GameObject car)
    {
        /// Remove Existing cameras
        if (Camera.allCameras.Length > 0)
        {
            for (int i = 0; i < Camera.allCameras.Length; i++)
            {
                DestroyImmediate(Camera.allCameras[i].gameObject);
            }
        }

        ///create camera GameObject
        GameObject newCamera = new GameObject();
        newCamera.name = "CarMakerCamera";

        ///add scripts to GameObject
        newCamera.AddComponent<Camera>();
        newCamera.AddComponent<AudioListener>();
        
        ///add CarCamera script
        CarCamera carCamera = newCamera.AddComponent<CarCamera>();

        ///asign default camera settings
        carCamera.carCameraSettingsList = new List<CarCameraSettings>();
        carCamera.carCameraSettingsList.Add(CarCameraSettings.GetDefaultSettings0());
        carCamera.carCameraSettingsList.Add(CarCameraSettings.GetDefaultSettings1());
        carCamera.carCameraSettingsList.Add(CarCameraSettings.GetDefaultSettings2());

        ///set camera target to follow
        carCamera.target = car.transform;

    }

    /// <summary>
    /// Automaticly try to detect wheels of the car GameObject
    /// </summary>
    private void AutoDetectWheels() {
        ///get all childtren transforms
        Transform[] allChildren = carObject.GetComponentsInChildren<Transform>(true);

        ///iterate all children
        for (int i = 0; i < allChildren.Length; i++)
        {
            ///ignore parent object
            if (allChildren[i] == carObject.transform)
                continue;

            ///front left
            if (FindChildrenWithMatch(allChildren[i], searchWheelFrontLeft))
                wheelFrontLeft = allChildren[i].gameObject;

            ///front right
            if (FindChildrenWithMatch(allChildren[i], searchWheelFrontRight))
                wheelFrontRight = allChildren[i].gameObject;

            ///back left
            if (FindChildrenWithMatch(allChildren[i], searchWheelBackLeft))
                wheelBackLeft = allChildren[i].gameObject;

            ///back right
            if (FindChildrenWithMatch(allChildren[i], searchWheelBackRight))
                wheelBackRight = allChildren[i].gameObject;
        }

    }

    private static string[][] searchWheelFrontLeft = new string[][]{
        new string[] { "front", "left"},
        new string[] { "fl"}
    };

    private static string[][] searchWheelFrontRight = new string[][]{
        new string[] { "front", "right"},
        new string[] { "fr"}
    };

    private static string[][] searchWheelBackLeft = new string[][]{
        new string[] { "back", "left"},
        new string[] { "rear", "left"},
        new string[] { "bl"}
    };

    private static string[][] searchWheelBackRight = new string[][]{
        new string[] { "back", "right"},
        new string[] { "rear", "right"},
        new string[] { "br"}
    };

    /// <summary>
    /// Try to match transform name string with multiple strings
    /// </summary>
    /// <param name="childTransform"></param>
    /// <param name="criteria"></param>
    /// <returns></returns>
    private bool FindChildrenWithMatch(Transform childTransform, string[][] criteria)
    {
        bool b = false;

        ///iterate all possible string matches
        for (int i = 0; i < criteria.Length; i++)
        {
            ///create bool array for the matches
            bool[] matchAaray = new bool[criteria[i].Length];

            ///iterate all strings
            for (int i1 = 0; i1 < criteria[i].Length; i1++)
            {
                ///set match array to true if there is string match
                if (childTransform.name.ToLower().Contains(criteria[i][i1]) == true)
                    matchAaray[i1] = true;

               
            }

            bool allMatch = true;

            ///check if all matches are true
            foreach (bool match in matchAaray)
            {
                if (match == false)
                    allMatch = false;
            }

            if (allMatch == true)
                b = true;
        }

        return b;
    }

    /// <summary>
    /// Create and add WheelCollider to given parent GameObject
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="wheelMeshGO"></param>
    /// <param name="name"></param>
    /// <param name="isFrontWheel"></param>
    /// <returns></returns>
    private WheelCollider AddWheelCollider(GameObject parent, GameObject wheelMeshGO, string name, bool isFrontWheel)
    {
        ///create wheel GameObject and set position and rotation
        GameObject wheel = new GameObject();
        wheel.transform.parent = parent.transform;
        wheel.transform.localPosition = wheelMeshGO.transform.localPosition;
        wheel.transform.localRotation = Quaternion.identity;
        wheel.name = name;

        ///add wheel collider GameObject
        WheelCollider wheelCollider = wheel.AddComponent<WheelCollider>();
        wheelCollider.mass = 40.0f;

        ///adjust wheel collider forward friction settings
        WheelFrictionCurve wfcForward = wheelCollider.forwardFriction;
        wfcForward.extremumSlip = 0.05f;
        wfcForward.extremumValue = 1.0f;
        wfcForward.asymptoteSlip = 2.0f;
        wfcForward.asymptoteValue = 2.0f;
        wheelCollider.forwardFriction = wfcForward;

        ///adjust wheel collider sideways friction settings
        WheelFrictionCurve wfcSideways = wheelCollider.forwardFriction;
        wfcSideways.extremumSlip = 0.05f;
        wfcSideways.extremumValue = 1.0f;

        ///if this is back wheel
        if (isFrontWheel == false)
        {    
            wfcSideways.extremumSlip = 0.1f;
            wfcSideways.extremumValue = 1.2f / slipperiness;
        }


        wheelCollider.sidewaysFriction = wfcSideways;

        ///try to get mesh render of the wheel
        Renderer wheelRenderer = wheelMeshGO.GetComponent<Renderer>();

        if (wheelRenderer) {
            ///calculate size of the wheelcollider based on the renderer size
            wheelCollider.radius = wheelRenderer.bounds.size.y / 2;
        }


        return wheelCollider;
    }

    /// <summary>
    /// returns child GameObject if there is a name match
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    private GameObject GetChildByName(GameObject parent, string name)
    {
        GameObject childGO = null;
        Transform[] transformArray = parent.GetComponentsInChildren<Transform>();

        ///iterate all child transform
        foreach (Transform t in transformArray)
        {
            ///fix name if it was a copy
            t.name = t.name.Replace("(Clone)", "");

            ///check for match
            if (t.name == name)
                childGO = t.gameObject;
        }

        return childGO;
    }
}
