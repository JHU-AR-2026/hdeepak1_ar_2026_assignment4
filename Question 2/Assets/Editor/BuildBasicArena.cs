using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Creates the small starter arena requested for the first homework milestone.
/// </summary>
public static class BuildBasicArena
{
    private const string ScenePath = "Assets/Scenes/SampleScene.unity";

    [MenuItem("Assignment 4/Build Basic Arena")]
    public static void Build()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        Material floorMaterial = CreateMaterial("Floor Material", new Color(0.28f, 0.76f, 0.72f));
        Material wallMaterial = CreateMaterial("Wall Material", new Color(0.94f, 0.96f, 0.98f));
        Material ballMaterial = CreateMaterial("Ball Material", new Color(0.92f, 0.08f, 0.08f));
        Material collectibleMaterial = CreateMaterial("Collectible Material", new Color(1f, 0.78f, 0.02f));
        Material powerUpMaterial = CreateMaterial("Power-Up Material", new Color(0.95f, 0.2f, 0.85f));
        Material breakableWallMaterial = CreateMaterial("Breakable Wall Material", new Color(0.38f, 0.17f, 0.07f));
        Material goalMaterial = CreateTransparentMaterial("Goal Globe Material", new Color(0.5f, 0.45f, 1f, 0.28f));

        // Build two equal 26-by-26 platforms with a five-unit jump gap between them.
        CreateCube("Starting Platform", new Vector3(0f, -0.25f, 0f), new Vector3(26f, 0.5f, 26f), floorMaterial);
        CreateCube("Finish Platform", new Vector3(0f, -0.25f, 31f), new Vector3(26f, 0.5f, 26f), floorMaterial);

        // Enclose the starting platform. Its north wall contains the breakable brown doorway.
        CreateCube("Starting Wall South", new Vector3(0f, 1.25f, -13f), new Vector3(26.5f, 2.5f, 0.5f), wallMaterial);
        CreateCube("Starting Wall East", new Vector3(13f, 1.25f, 0f), new Vector3(0.5f, 2.5f, 26.5f), wallMaterial);
        CreateCube("Starting Wall West", new Vector3(-13f, 1.25f, 0f), new Vector3(0.5f, 2.5f, 26.5f), wallMaterial);
        CreateCube("Starting Back Wall Left", new Vector3(-8f, 1.25f, 13f), new Vector3(10f, 2.5f, 0.7f), wallMaterial);
        CreateCube("Starting Back Wall Right", new Vector3(8f, 1.25f, 13f), new Vector3(10f, 2.5f, 0.7f), wallMaterial);

        // The finish platform has no front wall, leaving a clear landing after the jump.
        CreateCube("Finish Wall East", new Vector3(13f, 1.25f, 31f), new Vector3(0.5f, 2.5f, 26.5f), wallMaterial);
        CreateCube("Finish Wall West", new Vector3(-13f, 1.25f, 31f), new Vector3(0.5f, 2.5f, 26.5f), wallMaterial);
        CreateCube("Finish Wall North", new Vector3(0f, 1.25f, 44f), new Vector3(26.5f, 2.5f, 0.5f), wallMaterial);

        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.name = "Red Ball";
        ball.transform.position = new Vector3(0f, 0.6f, 0f);
        ball.GetComponent<Renderer>().sharedMaterial = ballMaterial;
        Rigidbody rigidbody = ball.AddComponent<Rigidbody>();
        rigidbody.mass = 1f;
        rigidbody.linearDamping = 0.35f;
        rigidbody.angularDamping = 0.05f;
        ball.AddComponent<BallMovement>();

        // Place six pickups in a loose path that encourages movement across the larger arena.
        Vector3[] collectiblePositions =
        {
            new Vector3(-7f, 0.8f, -6f),
            new Vector3(0f, 1.2f, -5f),
            new Vector3(7f, 1.6f, -3f),
            new Vector3(-6f, 2f, 4f),
            new Vector3(2f, 2.4f, 7f),
            new Vector3(8f, 2.8f, 11f)
        };

        for (int index = 0; index < collectiblePositions.Length; index++)
        {
            CreateCollectible(index + 1, collectiblePositions[index], collectibleMaterial);
        }

        GameObject powerUp = CreatePowerUp(powerUpMaterial);
        CreateBreakableWall(breakableWallMaterial);
        CreateGoalZone(goalMaterial);

        Text statusText = CreateHud();
        GameObject managerObject = new GameObject("Game Manager");
        GameManager gameManager = managerObject.AddComponent<GameManager>();
        gameManager.Configure(collectiblePositions.Length, powerUp, statusText);

        CreateCamera(ball.transform);
        CreateLight();

        EditorSceneManager.SaveScene(scene, ScenePath);
        Selection.activeGameObject = ball;
        Debug.Log("Basic arena created. Press Play and use WASD or the arrow keys to move the red ball.");
    }

    private static GameObject CreateCube(string name, Vector3 position, Vector3 scale, Material material)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetPositionAndRotation(position, Quaternion.identity);
        cube.transform.localScale = scale;
        cube.GetComponent<Renderer>().sharedMaterial = material;
        return cube;
    }

    private static void CreateCollectible(int number, Vector3 position, Material material)
    {
        GameObject collectible = CreateCube(
            $"Collectible {number}",
            position,
            new Vector3(1.1f, 1.1f, 1.1f),
            material);

        BoxCollider collider = collectible.GetComponent<BoxCollider>();
        collider.isTrigger = true;
        collectible.AddComponent<Collectible>();
    }

    private static GameObject CreatePowerUp(Material material)
    {
        GameObject powerUp = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        powerUp.name = "Power-Up Capsule";
        // Spawn the reward at the center of the starting platform after all cubes are collected.
        powerUp.transform.position = new Vector3(0f, 1f, 0f);
        powerUp.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        powerUp.GetComponent<Renderer>().sharedMaterial = material;
        powerUp.GetComponent<CapsuleCollider>().isTrigger = true;
        powerUp.AddComponent<PowerUp>();
        return powerUp;
    }

    private static void CreateBreakableWall(Material material)
    {
        GameObject wall = CreateCube(
            "Brown Breakable Wall",
            new Vector3(0f, 1.25f, 13f),
            new Vector3(6f, 2.5f, 0.7f),
            material);
        wall.AddComponent<BreakableWall>();
    }

    private static void CreateGoalZone(Material material)
    {
        GameObject goal = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        goal.name = "Transparent Goal Globe";
        // Sink half of the enlarged sphere below the opaque floor so it reads as a dome.
        goal.transform.position = new Vector3(0f, 0f, 31f);
        goal.transform.localScale = new Vector3(10f, 10f, 10f);
        goal.GetComponent<Renderer>().sharedMaterial = material;
        goal.GetComponent<SphereCollider>().isTrigger = true;
        goal.AddComponent<GoalZone>();
    }

    private static Text CreateHud()
    {
        GameObject canvasObject = new GameObject("HUD Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasObject.AddComponent<GraphicRaycaster>();

        GameObject textObject = new GameObject("Status Text");
        textObject.transform.SetParent(canvasObject.transform, false);
        Text text = textObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 34;
        text.fontStyle = FontStyle.Bold;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleLeft;

        RectTransform rect = text.rectTransform;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(30f, -25f);
        rect.sizeDelta = new Vector2(-60f, 70f);

        GameObject startButtonObject = CreateStartButton(canvasObject.transform);

        // The EventSystem routes pointer input from the new Input System to the UI button.
        GameObject eventSystemObject = new GameObject("EventSystem");
        EventSystem eventSystem = eventSystemObject.AddComponent<EventSystem>();
        eventSystem.firstSelectedGameObject = startButtonObject;
        eventSystemObject.AddComponent<InputSystemUIInputModule>();
        return text;
    }

    private static GameObject CreateStartButton(Transform canvasTransform)
    {
        GameObject buttonObject = new GameObject("Start Button");
        buttonObject.transform.SetParent(canvasTransform, false);
        Image background = buttonObject.AddComponent<Image>();
        background.color = new Color(0.03f, 0.28f, 0.32f, 0.96f);
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = background;
        buttonObject.AddComponent<StartGameButton>();

        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = Vector2.zero;
        buttonRect.sizeDelta = new Vector2(280f, 95f);

        GameObject labelObject = new GameObject("Label");
        labelObject.transform.SetParent(buttonObject.transform, false);
        Text label = labelObject.AddComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.text = "START";
        label.fontSize = 42;
        label.fontStyle = FontStyle.Bold;
        label.color = Color.white;
        label.alignment = TextAnchor.MiddleCenter;

        // Stretch the label across the button while keeping clicks on the parent control.
        label.raycastTarget = false;
        RectTransform labelRect = label.rectTransform;
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        return buttonObject;
    }

    private static Material CreateMaterial(string name, Color color)
    {
        string folder = "Assets/Materials";
        if (!AssetDatabase.IsValidFolder(folder))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }

        string path = $"{folder}/{name}.mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            material = new Material(shader);
            AssetDatabase.CreateAsset(material, path);
        }

        material.color = color;
        EditorUtility.SetDirty(material);
        return material;
    }

    private static Material CreateTransparentMaterial(string name, Color color)
    {
        Material material = CreateMaterial(name, color);

        // Configure the URP Lit material for alpha blending instead of an opaque surface.
        material.SetFloat("_Surface", 1f);
        material.SetFloat("_Blend", 0f);
        material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetFloat("_ZWrite", 0f);
        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        material.renderQueue = (int)RenderQueue.Transparent;
        EditorUtility.SetDirty(material);
        return material;
    }

    private static void CreateCamera(Transform ball)
    {
        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        Camera camera = cameraObject.AddComponent<Camera>();
        cameraObject.AddComponent<AudioListener>();
        cameraObject.transform.SetPositionAndRotation(
            new Vector3(0f, 11f, -12f),
            Quaternion.Euler(32f, 0f, 0f));
        camera.fieldOfView = 55f;
        camera.clearFlags = CameraClearFlags.Skybox;

        // Follow the ball while retaining the angled perspective shown in the reference.
        CameraFollow follow = cameraObject.AddComponent<CameraFollow>();
        follow.SetTarget(ball);
    }

    private static void CreateLight()
    {
        GameObject lightObject = new GameObject("Directional Light");
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.25f;
        light.shadows = LightShadows.Soft;
        lightObject.transform.rotation = Quaternion.Euler(48f, -30f, 0f);

        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.58f, 0.62f, 0.68f);
    }
}
