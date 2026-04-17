using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;
using System.IO;
using System.Reflection;

public class VerticalSliceSetup : EditorWindow
{
    static TMP_FontAsset tmpFont;

    [MenuItem("Tools/Vertical Slice Setup")]
    static void ShowWindow()
    {
        GetWindow<VerticalSliceSetup>("VS Setup");
    }

    void OnGUI()
    {
        GUILayout.Label("Vertical Slice Scene Setup", EditorStyles.boldLabel);
        GUILayout.Space(5);
        GUILayout.Label("IMPORTANT: First click 'Clean ALL Scenes' if re-running.", EditorStyles.miniLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Clean ALL Scenes (removes generated UI)", GUILayout.Height(30)))
        {
            CleanScene("Assets/Scenes/MainMenu.unity", "MainMenuCanvas");
            CleanScene("Assets/Scenes/Garage.unity", "GarageCanvas");
            CleanScene("Assets/Scenes/Map1.unity", "RaceCanvas");
            CleanScene("Assets/Scenes/Map2.unity", "RaceCanvas");
            CleanScene("Assets/Scenes/Map3.unity", "RaceCanvas");
            CleanScene("Assets/Scenes/WinScene.unity", "WinCanvas");
            Debug.Log("All scenes cleaned!");
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Setup ALL Scenes + Build Settings", GUILayout.Height(50)))
        {
            LoadTMPFont();
            SetupMainMenu();
            SetupGarage();
            SetupRaceScene("Map1");
            SetupRaceScene("Map2");
            SetupRaceScene("Map3");
            SetupWinScene();
            AddAllScenesToBuildSettings();
            Debug.Log("=== ALL SCENES SET UP SUCCESSFULLY ===");
        }

        GUILayout.Space(10);
        GUILayout.Label("Individual Scenes", EditorStyles.boldLabel);

        if (GUILayout.Button("MainMenu")) { LoadTMPFont(); SetupMainMenu(); }
        if (GUILayout.Button("Garage")) { LoadTMPFont(); SetupGarage(); }
        if (GUILayout.Button("Map1")) { LoadTMPFont(); SetupRaceScene("Map1"); }
        if (GUILayout.Button("Map2")) { LoadTMPFont(); SetupRaceScene("Map2"); }
        if (GUILayout.Button("Map3")) { LoadTMPFont(); SetupRaceScene("Map3"); }
        if (GUILayout.Button("WinScene")) { LoadTMPFont(); SetupWinScene(); }

        GUILayout.Space(10);
        GUILayout.Label("Extras", EditorStyles.boldLabel);

        if (GUILayout.Button("Add All Scenes to Build Settings"))
            AddAllScenesToBuildSettings();

        if (GUILayout.Button("Create MusicManager Prefab"))
            CreateMusicManagerPrefab();

        if (GUILayout.Button("Create UIAudioManager Prefab"))
            CreateUIAudioManagerPrefab();
    }

    // ─────────────────────────────────────────────
    // FONT
    // ─────────────────────────────────────────────
    static void LoadTMPFont()
    {
        // Try to find any TMP font asset in the project
        string[] guids = AssetDatabase.FindAssets("t:TMP_FontAsset");
        if (guids.Length > 0)
        {
            // Prefer LiberationSans or any SDF font
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("LiberationSans"))
                {
                    tmpFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
                    break;
                }
            }
            // fallback to first found
            if (tmpFont == null)
            {
                tmpFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                    AssetDatabase.GUIDToAssetPath(guids[0]));
            }
        }

        if (tmpFont != null)
            Debug.Log("Using TMP font: " + tmpFont.name);
        else
            Debug.LogWarning("No TMP font found! Text may be invisible. Import TMP Essentials via Window > TextMeshPro > Import TMP Essential Resources.");
    }

    // ─────────────────────────────────────────────
    // CLEAN
    // ─────────────────────────────────────────────
    static void CleanScene(string scenePath, string canvasName)
    {
        if (!File.Exists(scenePath)) return;
        var scene = EditorSceneManager.OpenScene(scenePath);

        GameObject[] roots = scene.GetRootGameObjects();

        // Remove generated canvas by name (also catch hand-made "Canvas" roots)
        foreach (var root in roots)
        {
            if (root == null) continue;
            if (root.name == canvasName || root.name == "MinimapCamera")
                Object.DestroyImmediate(root);
            else if (root.name == "Canvas" && root.GetComponent<Canvas>() != null)
                Object.DestroyImmediate(root);
        }

        // Also clean generated children from ANY existing canvas
        foreach (var root in roots)
        {
            if (root == null) continue;

            // Find all canvases (including pre-existing ones)
            foreach (var canvas in root.GetComponentsInChildren<Canvas>(true))
            {
                // Remove generated panels and UI elements
                string[] generatedNames = {
                    "PausePanel", "HowToPlayPanel", "GameOverPanel",
                    "TitleText", "GarageTitle", "WinTitle", "FinalTimeText",
                    "PlayButton", "HowToPlayButton", "QuitButton",
                    "MenuButton", "ReplayButton", "MusicPlaylistWidget",
                    "DriverLabel", "DriverNameText", "PrevDriverButton", "NextDriverButton",
                    "CarLabel", "CarNameText", "PrevCarButton", "NextCarButton",
                    "MapNameText", "RaceButton", "BackButton",
                    "PauseTitle", "ResumeButton", "HTPPauseButton", "RestartButton",
                    "EventSystem",
                    "CountdownText", "TimerText", "LapText",
                    "TurboBarBackground", "MinimapImage", "FadeOverlay"
                };

                foreach (string childName in generatedNames)
                {
                    Transform child = canvas.transform.Find(childName);
                    if (child != null)
                        Object.DestroyImmediate(child.gameObject);
                }
            }

            // Remove added components
            foreach (var comp in root.GetComponentsInChildren<PauseMenu>(true))
                Object.DestroyImmediate(comp);
            foreach (var comp in root.GetComponentsInChildren<GameOverScreen>(true))
                Object.DestroyImmediate(comp);
            foreach (var comp in root.GetComponentsInChildren<HowToPlayScreen>(true))
                Object.DestroyImmediate(comp);
            foreach (var comp in root.GetComponentsInChildren<GarageManager>(true))
                Object.DestroyImmediate(comp);
            foreach (var comp in root.GetComponentsInChildren<TireSmoke>(true))
                Object.DestroyImmediate(comp);
            foreach (var comp in root.GetComponentsInChildren<MusicPlaylistUI>(true))
                Object.DestroyImmediate(comp);
            foreach (var comp in root.GetComponentsInChildren<UIButtonSounds>(true))
                Object.DestroyImmediate(comp);
            foreach (var comp in root.GetComponentsInChildren<UIButtonGlow>(true))
                Object.DestroyImmediate(comp);
            foreach (var comp in root.GetComponentsInChildren<QuitHelper>(true))
                Object.DestroyImmediate(comp);
            foreach (var comp in root.GetComponentsInChildren<MainMenuUI>(true))
                Object.DestroyImmediate(comp);
            foreach (var comp in root.GetComponentsInChildren<UIAudioManager>(true))
                Object.DestroyImmediate(comp);
            foreach (var comp in root.GetComponentsInChildren<MusicManager>(true))
                Object.DestroyImmediate(comp);
            foreach (var comp in root.GetComponentsInChildren<CountdownUI>(true))
                Object.DestroyImmediate(comp);
            foreach (var comp in root.GetComponentsInChildren<RaceTimer>(true))
                Object.DestroyImmediate(comp);
            foreach (var comp in root.GetComponentsInChildren<TurboCooldownUI>(true))
                Object.DestroyImmediate(comp);

            // Clean nested MusicPlaylistWidget inside PausePanel
            foreach (var canvas in root.GetComponentsInChildren<Canvas>(true))
            {
                Transform pp = canvas.transform.Find("PausePanel");
                if (pp != null)
                {
                    Transform nested = pp.Find("MusicPlaylistWidget");
                    if (nested != null) Object.DestroyImmediate(nested.gameObject);
                }
            }
        }

        EditorSceneManager.SaveScene(scene);
    }

    // ─────────────────────────────────────────────
    // MAIN MENU
    // ─────────────────────────────────────────────
    static void SetupMainMenu()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        Canvas canvas = FindOrCreateCanvas("MainMenuCanvas");
        GameObject canvasGO = canvas.gameObject;

        // ensure singletons exist in this scene
        EnsureSingleton<UIAudioManager>(scene, "UIAudioManager");
        EnsureSingleton<MusicManager>(scene, "MusicManager");

        // Title
        CreateText(canvasGO.transform, "TitleText", "THE PAST AND THE FURIOUS",
            new Vector2(0, 200), 56, TextAlignmentOptions.Center, FontStyles.Bold);

        // Play -> Garage
        var playBtn = CreateButton(canvasGO.transform, "PlayButton", "PLAY",
            new Vector2(0, 50), new Vector2(300, 65));
        var playSceneChanger = EnsureComponent<SceneChange>(playBtn);
        playSceneChanger.sceneName = "Garage";
        WirePersistentClick(playBtn, playSceneChanger, "LoadScene");

        // How To Play
        var howToPlayBtn = CreateButton(canvasGO.transform, "HowToPlayButton", "HOW TO PLAY",
            new Vector2(0, -25), new Vector2(300, 65));

        // Quit
        var quitBtn = CreateButton(canvasGO.transform, "QuitButton", "QUIT",
            new Vector2(0, -100), new Vector2(300, 65));
        var quitHelper = EnsureComponent<QuitHelper>(quitBtn);
        WirePersistentClick(quitBtn, quitHelper, "DoQuit");

        // How To Play Panel
        GameObject htpPanel = CreatePanel(canvasGO.transform, "HowToPlayPanel", new Color(0, 0, 0, 0.92f));
        htpPanel.SetActive(false);

        CreateText(htpPanel.transform, "HTPTitle", "HOW TO PLAY",
            new Vector2(0, 250), 40, TextAlignmentOptions.Center, FontStyles.Bold);

        CreateText(htpPanel.transform, "HTPControls",
            "W / Up Arrow  -  Accelerate\n" +
            "S / Down Arrow  -  Brake / Reverse\n" +
            "A / D  -  Steer Left / Right\n" +
            "Space  -  Turbo Boost\n\n" +
            "Complete all laps to win!\n" +
            "Use turbo wisely - it has a cooldown.",
            new Vector2(0, 20), 26, TextAlignmentOptions.Center, FontStyles.Normal);

        var closeHTPBtn = CreateButton(htpPanel.transform, "CloseHTPButton", "BACK",
            new Vector2(0, -250), new Vector2(220, 55));

        // Wire HowToPlayScreen
        var htpScreen = EnsureComponent<HowToPlayScreen>(canvasGO);
        htpScreen.howToPlayPanel = htpPanel;

        // MainMenuUI (self-wires buttons at runtime)
        var mainMenuUI = EnsureComponent<MainMenuUI>(canvasGO);
        mainMenuUI.howToPlayScreen = htpScreen;

        // Music playlist widget
        CreateMusicPlaylistWidget(canvasGO.transform, new Vector2(20, 20), new Vector2(320, 130));

        MarkAllDirty(canvasGO);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("MainMenu scene set up!");
    }

    // ─────────────────────────────────────────────
    // GARAGE
    // ─────────────────────────────────────────────
    static void SetupGarage()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Garage.unity");
        Canvas canvas = FindOrCreateCanvas("GarageCanvas");
        GameObject canvasGO = canvas.gameObject;

        // Title
        CreateText(canvasGO.transform, "GarageTitle", "GARAGE",
            new Vector2(0, 300), 48, TextAlignmentOptions.Center, FontStyles.Bold);

        // --- Driver Selection ---
        CreateText(canvasGO.transform, "DriverLabel", "SELECT DRIVER",
            new Vector2(0, 210), 28, TextAlignmentOptions.Center, FontStyles.Bold);

        var prevDriverBtn = CreateButton(canvasGO.transform, "PrevDriverButton", "<",
            new Vector2(-200, 160), new Vector2(65, 65));

        CreateText(canvasGO.transform, "DriverNameText", "Driver 1",
            new Vector2(0, 160), 30, TextAlignmentOptions.Center, FontStyles.Normal);

        var nextDriverBtn = CreateButton(canvasGO.transform, "NextDriverButton", ">",
            new Vector2(200, 160), new Vector2(65, 65));

        // --- Car Selection ---
        CreateText(canvasGO.transform, "CarLabel", "SELECT CAR",
            new Vector2(0, 70), 28, TextAlignmentOptions.Center, FontStyles.Bold);

        var prevCarBtn = CreateButton(canvasGO.transform, "PrevCarButton", "<",
            new Vector2(-200, 20), new Vector2(65, 65));

        CreateText(canvasGO.transform, "CarNameText", "Car 1",
            new Vector2(0, 20), 30, TextAlignmentOptions.Center, FontStyles.Normal);

        var nextCarBtn = CreateButton(canvasGO.transform, "NextCarButton", ">",
            new Vector2(200, 20), new Vector2(65, 65));

        // Map name
        CreateText(canvasGO.transform, "MapNameText", "Map1",
            new Vector2(0, -40), 22, TextAlignmentOptions.Center, FontStyles.Italic);

        // Race & Back
        var raceBtn = CreateButton(canvasGO.transform, "RaceButton", "RACE!",
            new Vector2(0, -130), new Vector2(320, 75));
        var backBtn = CreateButton(canvasGO.transform, "BackButton", "BACK",
            new Vector2(0, -220), new Vector2(220, 55));

        // GarageManager
        var gm = EnsureComponent<GarageManager>(canvasGO);
        gm.carNameText = canvasGO.transform.Find("CarNameText").GetComponent<TextMeshProUGUI>();
        gm.driverNameText = canvasGO.transform.Find("DriverNameText").GetComponent<TextMeshProUGUI>();
        gm.mapNameText = canvasGO.transform.Find("MapNameText").GetComponent<TextMeshProUGUI>();

        WirePersistentClick(prevCarBtn, gm, "PreviousCar");
        WirePersistentClick(nextCarBtn, gm, "NextCar");
        WirePersistentClick(prevDriverBtn, gm, "PreviousDriver");
        WirePersistentClick(nextDriverBtn, gm, "NextDriver");
        WirePersistentClick(raceBtn, gm, "ConfirmAndRace");
        WirePersistentClick(backBtn, gm, "BackToMenu");

        // Music playlist widget
        CreateMusicPlaylistWidget(canvasGO.transform, new Vector2(20, 20), new Vector2(320, 130));

        MarkAllDirty(canvasGO);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Garage scene set up!");
    }

    // ─────────────────────────────────────────────
    // RACE SCENES
    // ─────────────────────────────────────────────
    static void SetupRaceScene(string sceneName)
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/" + sceneName + ".unity");
        Canvas canvas = FindOrCreateCanvas("RaceCanvas");
        GameObject canvasGO = canvas.gameObject;

        // --- PAUSE PANEL ---
        GameObject pausePanel = CreatePanel(canvasGO.transform, "PausePanel", new Color(0, 0, 0, 0.88f));
        pausePanel.SetActive(false);

        CreateText(pausePanel.transform, "PauseTitle", "PAUSED",
            new Vector2(0, 200), 48, TextAlignmentOptions.Center, FontStyles.Bold);

        var resumeBtn = CreateButton(pausePanel.transform, "ResumeButton", "RESUME",
            new Vector2(0, 70), new Vector2(300, 60));
        var htpPauseBtn = CreateButton(pausePanel.transform, "HTPPauseButton", "HOW TO PLAY",
            new Vector2(0, 0), new Vector2(300, 60));
        var restartBtn = CreateButton(pausePanel.transform, "RestartButton", "RESTART",
            new Vector2(0, -70), new Vector2(300, 60));
        var quitBtn = CreateButton(pausePanel.transform, "QuitButton", "QUIT TO MENU",
            new Vector2(0, -140), new Vector2(300, 60));

        // --- HOW TO PLAY PANEL ---
        GameObject htpPanel = CreatePanel(canvasGO.transform, "HowToPlayPanel", new Color(0, 0, 0, 0.92f));
        htpPanel.SetActive(false);

        CreateText(htpPanel.transform, "HTPTitle", "HOW TO PLAY",
            new Vector2(0, 250), 40, TextAlignmentOptions.Center, FontStyles.Bold);

        CreateText(htpPanel.transform, "HTPText",
            "W / Up Arrow  -  Accelerate\n" +
            "S / Down Arrow  -  Brake / Reverse\n" +
            "A / D  -  Steer Left / Right\n" +
            "Space  -  Turbo Boost\n\n" +
            "Complete all laps to win!\n" +
            "Use turbo wisely - it has a cooldown.",
            new Vector2(0, 20), 26, TextAlignmentOptions.Center, FontStyles.Normal);

        var closeHTPBtn = CreateButton(htpPanel.transform, "CloseHTPButton", "BACK",
            new Vector2(0, -250), new Vector2(220, 55));

        // --- GAME OVER PANEL ---
        GameObject gameOverPanel = CreatePanel(canvasGO.transform, "GameOverPanel", new Color(0.15f, 0, 0, 0.92f));
        gameOverPanel.SetActive(false);

        CreateText(gameOverPanel.transform, "GameOverTitle", "GAME OVER",
            new Vector2(0, 150), 52, TextAlignmentOptions.Center, FontStyles.Bold);
        CreateText(gameOverPanel.transform, "ReasonText", "",
            new Vector2(0, 70), 30, TextAlignmentOptions.Center, FontStyles.Normal);

        var retryBtn = CreateButton(gameOverPanel.transform, "RetryButton", "RETRY",
            new Vector2(0, -30), new Vector2(280, 60));
        var goQuitBtn = CreateButton(gameOverPanel.transform, "GOQuitButton", "QUIT TO MENU",
            new Vector2(0, -100), new Vector2(280, 60));

        // --- WIRE PAUSE MENU ---
        var pauseMenu = EnsureComponent<PauseMenu>(canvasGO);
        pauseMenu.pausePanel = pausePanel;
        pauseMenu.howToPlayPanel = htpPanel;

        // NOTE: Do NOT add persistent listeners here — PauseMenu.Start() wires
        // buttons at runtime via WireButton(). Adding persistent listeners too would
        // cause double-firing (resume immediately re-pauses, etc).

        // --- MUSIC WIDGET IN PAUSE PANEL ---
        CreateMusicPlaylistWidget(pausePanel.transform, new Vector2(20, 20), new Vector2(300, 130), true);

        // --- WIRE GAME OVER ---
        var gameOverScreen = EnsureComponent<GameOverScreen>(canvasGO);
        gameOverScreen.gameOverPanel = gameOverPanel;
        gameOverScreen.reasonText = gameOverPanel.transform.Find("ReasonText").GetComponent<TextMeshProUGUI>();

        WirePersistentClick(retryBtn, gameOverScreen, "OnRetryButton");
        WirePersistentClick(goQuitBtn, gameOverScreen, "OnQuitToMenuButton");

        // --- WIRE RACE MANAGER ---
        RaceManager rm = Object.FindFirstObjectByType<RaceManager>();
        if (rm != null)
        {
            rm.gameOverScreen = gameOverScreen;
            rm.nextSceneName = "WinScene";
            EditorUtility.SetDirty(rm);
        }

        // --- COUNTDOWN ---
        var countdownTextGO = CreateText(canvasGO.transform, "CountdownText", "",
            Vector2.zero, 120, TextAlignmentOptions.Center, FontStyles.Bold);
        var countdownRect = countdownTextGO.GetComponent<RectTransform>();
        countdownRect.anchorMin = new Vector2(0.5f, 0.5f);
        countdownRect.anchorMax = new Vector2(0.5f, 0.5f);
        countdownRect.sizeDelta = new Vector2(400, 200);

        var countdownUI = EnsureComponent<CountdownUI>(canvasGO);
        countdownUI.countdownText = countdownTextGO.GetComponent<TextMeshProUGUI>();

        // --- RACE TIMER ---
        var timerTextGO = CreateText(canvasGO.transform, "TimerText", "00:00.00",
            new Vector2(20, -20), 36, TextAlignmentOptions.TopLeft, FontStyles.Normal);
        var timerRect = timerTextGO.GetComponent<RectTransform>();
        timerRect.anchorMin = new Vector2(0, 1);
        timerRect.anchorMax = new Vector2(0, 1);
        timerRect.pivot = new Vector2(0, 1);
        timerRect.sizeDelta = new Vector2(300, 50);

        var raceTimer = EnsureComponent<RaceTimer>(canvasGO);
        raceTimer.timerText = timerTextGO.GetComponent<TextMeshProUGUI>();

        // --- LAP TEXT ---
        var lapTextGO = CreateText(canvasGO.transform, "LapText", "Lap 1 / 3",
            new Vector2(20, -75), 28, TextAlignmentOptions.TopLeft, FontStyles.Normal);
        var lapRect = lapTextGO.GetComponent<RectTransform>();
        lapRect.anchorMin = new Vector2(0, 1);
        lapRect.anchorMax = new Vector2(0, 1);
        lapRect.pivot = new Vector2(0, 1);
        lapRect.sizeDelta = new Vector2(300, 40);

        // --- TURBO BAR ---
        Transform existingTurboBG = canvasGO.transform.Find("TurboBarBackground");
        if (existingTurboBG != null) Object.DestroyImmediate(existingTurboBG.gameObject);
        GameObject turboBarBG = new GameObject("TurboBarBackground");
        turboBarBG.transform.SetParent(canvasGO.transform, false);
        var turboBGRect = turboBarBG.AddComponent<RectTransform>();
        turboBGRect.anchorMin = new Vector2(0.5f, 0);
        turboBGRect.anchorMax = new Vector2(0.5f, 0);
        turboBGRect.pivot = new Vector2(0.5f, 0);
        turboBGRect.anchoredPosition = new Vector2(0, 30);
        turboBGRect.sizeDelta = new Vector2(200, 20);
        var turboBGImg = turboBarBG.AddComponent<Image>();
        turboBGImg.color = new Color(0.15f, 0.15f, 0.15f, 0.8f);

        GameObject turboBarFill = new GameObject("TurboBarFill");
        turboBarFill.transform.SetParent(turboBarBG.transform, false);
        var turboFillRect = turboBarFill.AddComponent<RectTransform>();
        turboFillRect.anchorMin = Vector2.zero;
        turboFillRect.anchorMax = Vector2.one;
        turboFillRect.offsetMin = Vector2.zero;
        turboFillRect.offsetMax = Vector2.zero;
        var turboFillImg = turboBarFill.AddComponent<Image>();
        turboFillImg.color = Color.green;
        turboFillImg.type = Image.Type.Filled;
        turboFillImg.fillMethod = Image.FillMethod.Horizontal;

        var turboCooldownUI = EnsureComponent<TurboCooldownUI>(canvasGO);
        turboCooldownUI.fillBar = turboFillImg;

        // --- MINIMAP ---
        // Create RenderTexture asset if it doesn't exist
        string rtPath = "Assets/Resources/MinimapRT.renderTexture";
        RenderTexture minimapRT = AssetDatabase.LoadAssetAtPath<RenderTexture>(rtPath);
        if (minimapRT == null)
        {
            minimapRT = new RenderTexture(256, 256, 16);
            minimapRT.name = "MinimapRT";
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            AssetDatabase.CreateAsset(minimapRT, rtPath);
            AssetDatabase.SaveAssets();
        }

        // Minimap RawImage on the canvas
        if (canvasGO.transform.Find("MinimapImage") == null)
        {
            GameObject minimapImgGO = new GameObject("MinimapImage");
            minimapImgGO.transform.SetParent(canvasGO.transform, false);
            var mmRect = minimapImgGO.AddComponent<RectTransform>();
            mmRect.anchorMin = new Vector2(1, 0);
            mmRect.anchorMax = new Vector2(1, 0);
            mmRect.pivot = new Vector2(1, 0);
            mmRect.anchoredPosition = new Vector2(-20, 20);
            mmRect.sizeDelta = new Vector2(180, 180);
            var rawImg = minimapImgGO.AddComponent<UnityEngine.UI.RawImage>();
            rawImg.texture = minimapRT;
            rawImg.raycastTarget = false;
        }

        // MinimapCamera GameObject in the scene
        MinimapCamera existingMiniCam = Object.FindFirstObjectByType<MinimapCamera>();
        if (existingMiniCam == null)
        {
            GameObject miniCamGO = new GameObject("MinimapCamera");
            var miniCam = miniCamGO.AddComponent<Camera>();
            miniCam.orthographic = true;
            miniCam.clearFlags = CameraClearFlags.SolidColor;
            miniCam.backgroundColor = Color.black;
            miniCam.targetTexture = minimapRT;
            // Remove AudioListener if one was added
            AudioListener listener = miniCamGO.GetComponent<AudioListener>();
            if (listener != null) Object.DestroyImmediate(listener);

            var miniMapScript = miniCamGO.AddComponent<MinimapCamera>();
            existingMiniCam = miniMapScript;
            EditorUtility.SetDirty(miniCamGO);
        }
        else
        {
            var cam = existingMiniCam.GetComponent<Camera>();
            if (cam != null) cam.targetTexture = minimapRT;
        }

        // --- CAR SETUP ---
        CarController car = Object.FindFirstObjectByType<CarController>();

        // Wire minimap + countdown + RaceManager to the car
        if (car != null)
        {
            existingMiniCam.target = car.transform;
            EditorUtility.SetDirty(existingMiniCam);

            PlayerInput pi = car.GetComponent<PlayerInput>();
            if (pi != null)
                countdownUI.playerInput = pi;

            turboCooldownUI.carController = car;
        }

        // --- CAR AUDIO ---
        if (car != null && car.GetComponent<CarAudio>() == null)
        {
            car.gameObject.AddComponent<CarAudio>();
            EditorUtility.SetDirty(car.gameObject);
        }

        // --- TIRE SMOKE ---
        if (car != null && car.GetComponent<TireSmoke>() == null)
        {
            var smoke = car.gameObject.AddComponent<TireSmoke>();
            smoke.carController = car;
            EditorUtility.SetDirty(car.gameObject);
        }

        // --- WHEEL COLLIDERS ---
        if (car != null && car.GetComponent<WheelColliders>() == null)
        {
            car.gameObject.AddComponent<WheelColliders>();
            EditorUtility.SetDirty(car.gameObject);
        }

        // --- WIRE RACE MANAGER (extended) ---
        if (rm != null && car != null)
        {
            PlayerInput pi = car.GetComponent<PlayerInput>();
            if (pi != null) rm.playerInput = pi;
            rm.carController = car;
            rm.carRigidbody = car.rb;
            rm.carTransform = car.transform;
            rm.lapText = lapTextGO.GetComponent<TextMeshProUGUI>();
            EditorUtility.SetDirty(rm);
        }

        // --- FADE GROUP ---
        if (rm != null && rm.fadeGroup == null)
        {
            GameObject fadeGO = new GameObject("FadeOverlay");
            fadeGO.transform.SetParent(canvasGO.transform, false);
            var fadeRect = fadeGO.AddComponent<RectTransform>();
            fadeRect.anchorMin = Vector2.zero;
            fadeRect.anchorMax = Vector2.one;
            fadeRect.offsetMin = Vector2.zero;
            fadeRect.offsetMax = Vector2.zero;
            var fadeImg = fadeGO.AddComponent<Image>();
            fadeImg.color = Color.black;
            fadeImg.raycastTarget = false;
            var fadeCanvasGroup = fadeGO.AddComponent<CanvasGroup>();
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
            rm.fadeGroup = fadeCanvasGroup;
            // Move to last sibling so it renders on top
            fadeGO.transform.SetAsLastSibling();
            EditorUtility.SetDirty(rm);
        }

        MarkAllDirty(canvasGO);
        EditorSceneManager.SaveScene(scene);
        Debug.Log(sceneName + " scene set up!");
    }

    // ─────────────────────────────────────────────
    // WIN SCENE
    // ─────────────────────────────────────────────
    static void SetupWinScene()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/WinScene.unity");
        Canvas canvas = FindOrCreateCanvas("WinCanvas");
        GameObject canvasGO = canvas.gameObject;

        CreateText(canvasGO.transform, "WinTitle", "YOU WIN!",
            new Vector2(0, 200), 60, TextAlignmentOptions.Center, FontStyles.Bold);

        CreateText(canvasGO.transform, "FinalTimeText", "Time: 00:00.00",
            new Vector2(0, 100), 36, TextAlignmentOptions.Center, FontStyles.Normal);

        // Main Menu button
        var menuBtn = CreateButton(canvasGO.transform, "MenuButton", "MAIN MENU",
            new Vector2(0, -20), new Vector2(320, 65));
        var menuChanger = EnsureComponent<SceneChange>(menuBtn);
        menuChanger.sceneName = "MainMenu";
        WirePersistentClick(menuBtn, menuChanger, "LoadScene");

        // Play Again button
        var replayBtn = CreateButton(canvasGO.transform, "ReplayButton", "PLAY AGAIN",
            new Vector2(0, -100), new Vector2(320, 65));
        var replayChanger = EnsureComponent<SceneChange>(replayBtn);
        replayChanger.sceneName = "Garage";
        WirePersistentClick(replayBtn, replayChanger, "LoadScene");

        // WinScreenUI
        WinScreenUI existingWin = Object.FindFirstObjectByType<WinScreenUI>();
        if (existingWin == null)
        {
            var winUI = EnsureComponent<WinScreenUI>(canvasGO);
            winUI.finalTimeText = canvasGO.transform.Find("FinalTimeText").GetComponent<TextMeshProUGUI>();
        }
        else
        {
            existingWin.finalTimeText = canvasGO.transform.Find("FinalTimeText").GetComponent<TextMeshProUGUI>();
            EditorUtility.SetDirty(existingWin);
        }

        MarkAllDirty(canvasGO);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("WinScene set up!");
    }

    // ─────────────────────────────────────────────
    // BUILD SETTINGS
    // ─────────────────────────────────────────────
    static void AddAllScenesToBuildSettings()
    {
        string[] scenePaths = {
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/Garage.unity",
            "Assets/Scenes/Map1.unity",
            "Assets/Scenes/Map2.unity",
            "Assets/Scenes/Map3.unity",
            "Assets/Scenes/WinScene.unity"
        };

        var buildScenes = new EditorBuildSettingsScene[scenePaths.Length];
        for (int i = 0; i < scenePaths.Length; i++)
            buildScenes[i] = new EditorBuildSettingsScene(scenePaths[i], true);

        EditorBuildSettings.scenes = buildScenes;
        Debug.Log("Build settings updated with " + scenePaths.Length + " scenes.");
    }

    // ─────────────────────────────────────────────
    // PREFABS
    // ─────────────────────────────────────────────
    static void CreateMusicManagerPrefab()
    {
        string path = "Assets/Prefabs/MusicManager.prefab";
        if (File.Exists(path)) { Debug.Log("Already exists: " + path); return; }

        GameObject go = new GameObject("MusicManager");
        go.AddComponent<MusicManager>();
        PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        Debug.Log("Created: " + path);
    }

    static void CreateUIAudioManagerPrefab()
    {
        string path = "Assets/Prefabs/UIAudioManager.prefab";
        if (File.Exists(path)) { Debug.Log("Already exists: " + path); return; }

        GameObject go = new GameObject("UIAudioManager");
        go.AddComponent<UIAudioManager>();
        PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        Debug.Log("Created: " + path);
    }

    // ═════════════════════════════════════════════
    // HELPERS
    // ═════════════════════════════════════════════

    static Canvas FindOrCreateCanvas(string name)
    {
        Canvas[] allCanvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (var c in allCanvases)
        {
            if (c.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                // Always fix the CanvasScaler on existing canvases
                var existingScaler = c.GetComponent<CanvasScaler>();
                if (existingScaler != null)
                {
                    existingScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    existingScaler.referenceResolution = new Vector2(1920, 1080);
                    existingScaler.matchWidthOrHeight = 0.5f;
                    EditorUtility.SetDirty(existingScaler);
                }
                return c;
            }
        }

        GameObject canvasGO = new GameObject(name);
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        return canvas;
    }

    static GameObject CreateText(Transform parent, string name, string content,
        Vector2 position, int fontSize, TextAlignmentOptions alignment, FontStyles style)
    {
        Transform existing = parent.Find(name);
        if (existing != null)
        {
            // update existing text
            var existTMP = existing.GetComponent<TextMeshProUGUI>();
            if (existTMP != null)
            {
                existTMP.text = content;
                existTMP.fontSize = fontSize;
                if (tmpFont != null) existTMP.font = tmpFont;
            }
            return existing.gameObject;
        }

        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);

        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchoredPosition = position;

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = content;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.fontStyle = style;
        tmp.color = Color.white;
        tmp.raycastTarget = false;

        if (tmpFont != null)
            tmp.font = tmpFont;

        // size based on content
        if (content.Contains("\n"))
            rect.sizeDelta = new Vector2(800, 400);
        else
            rect.sizeDelta = new Vector2(800, fontSize * 1.5f);

        return go;
    }

    static GameObject CreateButton(Transform parent, string name, string label,
        Vector2 position, Vector2 size)
    {
        Transform existing = parent.Find(name);
        if (existing != null) return existing.gameObject;

        GameObject btnGO = new GameObject(name);
        btnGO.transform.SetParent(parent, false);

        RectTransform rect = btnGO.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Image img = btnGO.AddComponent<Image>();
        img.color = new Color(0.15f, 0.15f, 0.15f, 0.9f);

        Button btn = btnGO.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor = new Color(0.15f, 0.15f, 0.15f, 0.9f);
        cb.highlightedColor = new Color(0.3f, 0.1f, 0.1f, 1f);
        cb.pressedColor = new Color(0.5f, 0.1f, 0.1f, 1f);
        cb.selectedColor = new Color(0.25f, 0.25f, 0.25f, 1f);
        btn.colors = cb;

        // button text child
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(btnGO.transform, false);

        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(5, 2);
        textRect.offsetMax = new Vector2(-5, -2);

        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = Mathf.Max(18, size.y * 0.4f);
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.raycastTarget = false;

        if (tmpFont != null)
            tmp.font = tmpFont;

        // add sound + glow
        AddButtonSound(btnGO, label);
        AddButtonGlow(btnGO);

        return btnGO;
    }

    static GameObject CreatePanel(Transform parent, string name, Color bgColor)
    {
        Transform existing = parent.Find(name);
        if (existing != null) return existing.gameObject;

        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image img = panel.AddComponent<Image>();
        img.color = bgColor;

        return panel;
    }

    static void AddButtonSound(GameObject btnGO, string label)
    {
        if (btnGO.GetComponent<UIButtonSounds>() != null) return;

        var sounds = btnGO.AddComponent<UIButtonSounds>();
        string upper = label.ToUpper();
        if (upper.Contains("QUIT") || upper.Contains("BACK") || upper.Contains("CLOSE"))
            sounds.clickType = UIButtonSounds.ClickType.Back;
        else if (upper.Contains("RACE") || upper.Contains("PLAY") || upper.Contains("CONFIRM") || upper.Contains("START"))
            sounds.clickType = UIButtonSounds.ClickType.Confirm;
        else
            sounds.clickType = UIButtonSounds.ClickType.Normal;
    }

    static void AddButtonGlow(GameObject btnGO)
    {
        if (btnGO.GetComponent<UIButtonGlow>() == null)
            btnGO.AddComponent<UIButtonGlow>();
    }

    /// <summary>
    /// Adds a PERSISTENT onClick listener that survives scene save/load.
    /// </summary>
    static void WirePersistentClick(GameObject btnGO, Object target, string methodName)
    {
        Button btn = btnGO.GetComponent<Button>();
        if (btn == null) return;

        // check if already wired
        for (int i = 0; i < btn.onClick.GetPersistentEventCount(); i++)
        {
            if (btn.onClick.GetPersistentTarget(i) == target &&
                btn.onClick.GetPersistentMethodName(i) == methodName)
                return;
        }

        UnityAction action = System.Delegate.CreateDelegate(
            typeof(UnityAction), target, methodName) as UnityAction;

        if (action != null)
        {
            UnityEventTools.AddPersistentListener(btn.onClick, action);
            EditorUtility.SetDirty(btnGO);
        }
        else
        {
            Debug.LogWarning("Could not wire " + methodName + " on " + target.GetType().Name);
        }
    }

    static T EnsureComponent<T>(GameObject go) where T : Component
    {
        T comp = go.GetComponent<T>();
        if (comp == null) comp = go.AddComponent<T>();
        return comp;
    }

    static void EnsureSingleton<T>(UnityEngine.SceneManagement.Scene scene, string name) where T : Component
    {
        if (Object.FindFirstObjectByType<T>() != null) return;

        GameObject go = new GameObject(name);
        go.AddComponent<T>();
        EditorUtility.SetDirty(go);
    }

    static void MarkAllDirty(GameObject root)
    {
        EditorUtility.SetDirty(root);
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            EditorUtility.SetDirty(child.gameObject);
    }

    static void CreateMusicPlaylistWidget(Transform parent, Vector2 position, Vector2 size, bool alwaysVisible = false)
    {
        string name = "MusicPlaylistWidget";
        if (parent.Find(name) != null) return;

        GameObject widget = new GameObject(name);
        widget.transform.SetParent(parent, false);

        RectTransform widgetRect = widget.AddComponent<RectTransform>();
        widgetRect.anchorMin = new Vector2(0, 0);
        widgetRect.anchorMax = new Vector2(0, 0);
        widgetRect.pivot = new Vector2(0, 0);
        widgetRect.anchoredPosition = position;
        widgetRect.sizeDelta = size;

        Image bg = widget.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.75f);

        var songText = CreateText(widget.transform, "SongNameText", "No Track",
            new Vector2(0, 28), 20, TextAlignmentOptions.Center, FontStyles.Normal);
        songText.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 30);

        var trackText = CreateText(widget.transform, "TrackNumberText", "0 / 0",
            new Vector2(0, 5), 16, TextAlignmentOptions.Center, FontStyles.Normal);
        trackText.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 24);

        var prevBtn = CreateButton(widget.transform, "PrevTrackBtn", "<<",
            new Vector2(-110, -25), new Vector2(55, 35));
        var playPauseBtn = CreateButton(widget.transform, "PlayPauseBtn", "||",
            new Vector2(0, -25), new Vector2(55, 35));
        var nextBtn = CreateButton(widget.transform, "NextTrackBtn", ">>",
            new Vector2(110, -25), new Vector2(55, 35));

        // --- Volume Slider ---
        GameObject sliderGO = new GameObject("VolumeSlider");
        sliderGO.transform.SetParent(widget.transform, false);
        RectTransform sliderRect = sliderGO.AddComponent<RectTransform>();
        sliderRect.anchoredPosition = new Vector2(0, -55);
        sliderRect.sizeDelta = new Vector2(260, 16);

        // Background
        GameObject sliderBG = new GameObject("Background");
        sliderBG.transform.SetParent(sliderGO.transform, false);
        var bgImg = sliderBG.AddComponent<Image>();
        bgImg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        var bgRect = sliderBG.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero; bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero; bgRect.offsetMax = Vector2.zero;

        // Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderGO.transform, false);
        var fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0, 0.25f);
        fillAreaRect.anchorMax = new Vector2(1, 0.75f);
        fillAreaRect.offsetMin = new Vector2(5, 0);
        fillAreaRect.offsetMax = new Vector2(-5, 0);

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        var fillImg = fill.AddComponent<Image>();
        fillImg.color = new Color(0.8f, 0.2f, 0.2f, 1f);
        var fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero; fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero; fillRect.offsetMax = Vector2.zero;

        // Handle Slide Area
        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(sliderGO.transform, false);
        var handleAreaRect = handleArea.AddComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero; handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.offsetMin = new Vector2(10, 0);
        handleAreaRect.offsetMax = new Vector2(-10, 0);

        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        var handleImg = handle.AddComponent<Image>();
        handleImg.color = Color.white;
        var handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(16, 16);

        Slider slider = sliderGO.AddComponent<Slider>();
        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImg;
        slider.wholeNumbers = false;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0.5f;
        slider.direction = Slider.Direction.LeftToRight;

        // --- Wire MusicPlaylistUI ---
        var playlistUI = widget.AddComponent<MusicPlaylistUI>();
        playlistUI.songNameText = songText.GetComponent<TextMeshProUGUI>();
        playlistUI.trackNumberText = trackText.GetComponent<TextMeshProUGUI>();
        playlistUI.volumeSlider = slider;
        playlistUI.panel = widget;
        playlistUI.alwaysVisible = alwaysVisible;

        // NOTE: Do NOT add persistent listeners here — MusicPlaylistUI.Start() wires
        // buttons at runtime via WireButton(). Adding persistent listeners too would
        // cause double-firing (skip 2 songs, etc).
    }
}
