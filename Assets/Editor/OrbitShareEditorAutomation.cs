using System.Collections;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class OrbitShareEditorAutomation
{
    private const string CaptureFlag = "OrbitShare.CapturePlayScreenshot";
    private const string CapturePathKey = "OrbitShare.CapturePath";
    private const string FastDemoStartedKey = "OrbitShare.FastDemoStarted";
    private const string StartTimeKey = "OrbitShare.CaptureStartTime";
    private const string ScreenshotRequestedKey = "OrbitShare.ScreenshotRequested";
    private const string FrameCaptureFlag = "OrbitShare.CaptureDemoFrames";
    private const string FrameIndexKey = "OrbitShare.FrameIndex";
    private const string LastFrameTimeKey = "OrbitShare.LastFrameTime";
    private const string FrameDirectoryKey = "OrbitShare.FrameDirectory";
    private const string FrameAgentStartedKey = "OrbitShare.FrameAgentStarted";
    private const string ValidationFlag = "OrbitShare.ValidateMissionFlow";
    private const string ValidationPathKey = "OrbitShare.ValidationPath";
    private static int validationErrorLogCount;

    static OrbitShareEditorAutomation()
    {
        EditorApplication.update += Update;
        Application.logMessageReceived += HandleLogMessage;
    }

    public static void CapturePlayScreenshot()
    {
        string path = Path.GetFullPath("orbitshare-playmode.png");
        SessionState.SetBool(CaptureFlag, true);
        SessionState.SetBool(FastDemoStartedKey, false);
        SessionState.SetBool(ScreenshotRequestedKey, false);
        SessionState.SetString(CapturePathKey, path);
        SessionState.SetFloat(StartTimeKey, 0f);

        EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");
        EditorApplication.isPlaying = true;
    }

    public static void CaptureDemoFrames()
    {
        string directory = Path.GetFullPath("DemoFrames");
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, true);
        }
        Directory.CreateDirectory(directory);

        SessionState.SetBool(CaptureFlag, false);
        SessionState.SetBool(FrameCaptureFlag, true);
        SessionState.SetBool(FastDemoStartedKey, false);
        SessionState.SetBool(FrameAgentStartedKey, false);
        SessionState.SetInt(FrameIndexKey, 0);
        SessionState.SetFloat(StartTimeKey, 0f);
        SessionState.SetFloat(LastFrameTimeKey, -1f);
        SessionState.SetString(FrameDirectoryKey, directory);

        EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");
        EditorApplication.isPlaying = true;
    }

    public static void ValidateMissionFlow()
    {
        string path = Path.GetFullPath("orbitshare-validation.md");
        SessionState.SetBool(CaptureFlag, false);
        SessionState.SetBool(FrameCaptureFlag, false);
        SessionState.SetBool(ValidationFlag, true);
        SessionState.SetBool(FastDemoStartedKey, false);
        SessionState.SetFloat(StartTimeKey, 0f);
        SessionState.SetString(ValidationPathKey, path);
        validationErrorLogCount = 0;

        EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");
        EditorApplication.isPlaying = true;
    }

    private static void HandleLogMessage(string condition, string stackTrace, LogType type)
    {
        if (!SessionState.GetBool(ValidationFlag, false))
        {
            return;
        }

        if (type == LogType.Error || type == LogType.Exception)
        {
            validationErrorLogCount++;
        }
    }

    private static void Update()
    {
        if (SessionState.GetBool(FrameCaptureFlag, false))
        {
            UpdateFrameCapture();
            return;
        }

        if (SessionState.GetBool(ValidationFlag, false))
        {
            UpdateValidation();
            return;
        }

        if (!SessionState.GetBool(CaptureFlag, false) || !EditorApplication.isPlaying)
        {
            return;
        }

        if (SessionState.GetFloat(StartTimeKey, 0f) <= 0f)
        {
            SessionState.SetFloat(StartTimeKey, (float)EditorApplication.timeSinceStartup);
        }

        float elapsed = (float)EditorApplication.timeSinceStartup - SessionState.GetFloat(StartTimeKey, 0f);
        string path = SessionState.GetString(CapturePathKey, Path.GetFullPath("orbitshare-playmode.png"));

        MissionManager missionManager = Object.FindObjectOfType<MissionManager>();
        if (elapsed > 1f && !SessionState.GetBool(FastDemoStartedKey, false) && missionManager != null)
        {
            missionManager.RunFastDemo();
            SessionState.SetBool(FastDemoStartedKey, true);
        }

        if (SessionState.GetBool(ScreenshotRequestedKey, false))
        {
            if (File.Exists(path) || elapsed > 8f)
            {
                SessionState.SetBool(CaptureFlag, false);
                EditorApplication.isPlaying = false;
                EditorApplication.delayCall += delegate
                {
                    Debug.Log("OrbitShare screenshot saved: " + path);
                    EditorApplication.Exit(0);
                };
            }
            return;
        }

        if (elapsed < 4.5f)
        {
            return;
        }

        ScreenCapture.CaptureScreenshot(path);
        SessionState.SetBool(ScreenshotRequestedKey, true);
    }

    private static void UpdateFrameCapture()
    {
        if (!EditorApplication.isPlaying)
        {
            return;
        }

        if (!SessionState.GetBool(FrameAgentStartedKey, false))
        {
            string directory = SessionState.GetString(FrameDirectoryKey, Path.GetFullPath("DemoFrames"));
            GameObject agentObject = new GameObject("OrbitShareFrameCaptureAgent");
            OrbitShareFrameCaptureAgent agent = agentObject.AddComponent<OrbitShareFrameCaptureAgent>();
            agent.Initialize(directory);
            SessionState.SetBool(FrameAgentStartedKey, true);
            SessionState.SetFloat(StartTimeKey, (float)EditorApplication.timeSinceStartup);
            return;
        }

        float elapsed = (float)EditorApplication.timeSinceStartup - SessionState.GetFloat(StartTimeKey, (float)EditorApplication.timeSinceStartup);
        if (elapsed > 20f)
        {
            SessionState.SetBool(FrameCaptureFlag, false);
            EditorApplication.isPlaying = false;
            EditorApplication.delayCall += delegate
            {
                Debug.LogError("OrbitShare frame capture timed out before all frames were written.");
                EditorApplication.Exit(1);
            };
        }

        return;
    }

    private static void UpdateValidation()
    {
        if (!EditorApplication.isPlaying)
        {
            return;
        }

        if (SessionState.GetFloat(StartTimeKey, 0f) <= 0f)
        {
            SessionState.SetFloat(StartTimeKey, (float)EditorApplication.timeSinceStartup);
        }

        float elapsed = (float)EditorApplication.timeSinceStartup - SessionState.GetFloat(StartTimeKey, 0f);
        MissionManager missionManager = Object.FindObjectOfType<MissionManager>();
        if (elapsed > 1f && missionManager != null && missionManager.Phase != MissionPhase.Complete)
        {
            missionManager.RunFastDemo();
            SessionState.SetBool(FastDemoStartedKey, true);
        }

        if (elapsed < 10f && (missionManager == null || missionManager.Phase != MissionPhase.Complete))
        {
            return;
        }

        string path = SessionState.GetString(ValidationPathKey, Path.GetFullPath("orbitshare-validation.md"));
        WriteValidationReport(path, elapsed);
        SessionState.SetBool(ValidationFlag, false);
        EditorApplication.isPlaying = false;
        EditorApplication.delayCall += delegate
        {
            Debug.Log("OrbitShare validation report saved: " + path);
            EditorApplication.Exit(0);
        };
    }

    private static void WriteValidationReport(string path, float elapsed)
    {
        MissionManager missionManager = Object.FindObjectOfType<MissionManager>();
        SatelliteManager satelliteManager = Object.FindObjectOfType<SatelliteManager>();
        CameraManager cameraManager = Object.FindObjectOfType<CameraManager>();
        ImmediateUIManager immediateUIManager = Object.FindObjectOfType<ImmediateUIManager>();
        ObservationTarget[] targets = Object.FindObjectsOfType<ObservationTarget>();
        SatelliteOrbit orbit = Object.FindObjectOfType<SatelliteOrbit>();
        GroundStation groundStation = Object.FindObjectOfType<GroundStation>();

        SatelliteData personalSatellite = FindSatellite(satelliteManager, "SAT-P01");
        SatelliteData companySatellite = FindSatellite(satelliteManager, "SAT-C01");
        SatelliteData sharedSatellite = FindSatellite(satelliteManager, "SAT-S01");
        ObservationTarget seoulTarget = FindTarget(targets, "Seoul");
        ObservationTarget jejuTarget = FindTarget(targets, "Jeju");
        ObservationTarget pacificTarget = FindTarget(targets, "Pacific");

        bool hasManagers = missionManager != null && satelliteManager != null && cameraManager != null && immediateUIManager != null;
        bool hasSceneObjects = orbit != null && targets.Length >= 3 && groundStation != null;
        bool bootstrapReferencesConnected = missionManager != null
            && missionManager.satelliteManager != null
            && missionManager.simulationManager != null
            && missionManager.earth != null
            && missionManager.satelliteOrbit != null
            && missionManager.groundStation != null
            && missionManager.connectionLine != null
            && cameraManager != null
            && cameraManager.targetCamera != null
            && cameraManager.earth != null
            && cameraManager.satellite != null
            && cameraManager.missionTarget != null
            && cameraManager.groundStation != null
            && immediateUIManager != null
            && immediateUIManager.simulationManager != null
            && immediateUIManager.satelliteManager != null
            && immediateUIManager.missionManager != null
            && immediateUIManager.cameraManager != null
            && orbit != null
            && orbit.earth != null
            && orbit.simulationManager != null;
        bool missionComplete = missionManager != null && missionManager.Phase == MissionPhase.Complete;
        bool observationComplete = missionManager != null && missionManager.ObservationProgress >= 0.99f;
        bool transferComplete = missionManager != null && missionManager.TransferProgress >= 0.99f;
        bool hasFleet = satelliteManager != null && satelliteManager.Satellites.Count >= 3;
        bool satelliteCoreDataValid = personalSatellite != null
            && companySatellite != null
            && sharedSatellite != null
            && sharedSatellite.ownerType == SatelliteOwnerType.Shared
            && sharedSatellite.availabilityState == SatelliteAvailabilityState.MissionComplete
            && Mathf.Approximately(sharedSatellite.dataStoragePercent, 0f);
        bool targetCoreDataValid = seoulTarget != null
            && jejuTarget != null
            && pacificTarget != null
            && IsTargetDataPositive(seoulTarget)
            && IsTargetDataPositive(jejuTarget)
            && IsTargetDataPositive(pacificTarget)
            && missionManager != null
            && missionManager.SelectedTarget != null;
        bool missionMetricsReady = missionManager != null && missionManager.MissionQualityScore > 0 && missionManager.ServiceReliabilityScore > 0;
        bool scoreRangeValid = missionManager != null
            && IsScoreInRange(missionManager.MissionQualityScore)
            && IsScoreInRange(missionManager.ServiceReliabilityScore);
        bool runtimeLogsClean = validationErrorLogCount == 0;
        bool passed = hasManagers
            && hasSceneObjects
            && bootstrapReferencesConnected
            && missionComplete
            && observationComplete
            && transferComplete
            && hasFleet
            && satelliteCoreDataValid
            && targetCoreDataValid
            && missionMetricsReady
            && scoreRangeValid
            && runtimeLogsClean;

        StringBuilder report = new StringBuilder();
        report.AppendLine("# OrbitShare Automated Validation");
        report.AppendLine();
        report.AppendLine("Result: " + (passed ? "PASS" : "FAIL"));
        report.AppendLine("Elapsed seconds: " + elapsed.ToString("0.00"));
        report.AppendLine("Screen: " + Screen.width + "x" + Screen.height);
        report.AppendLine();
        report.AppendLine("## Checks");
        report.AppendLine("- Managers present: " + Status(hasManagers));
        report.AppendLine("- Scene objects present: " + Status(hasSceneObjects));
        report.AppendLine("- Bootstrap references connected: " + Status(bootstrapReferencesConnected));
        report.AppendLine("- Fleet initialized: " + Status(hasFleet));
        report.AppendLine("- Satellite core data valid: " + Status(satelliteCoreDataValid));
        report.AppendLine("- Target core data valid: " + Status(targetCoreDataValid));
        report.AppendLine("- Fast demo reaches mission complete: " + Status(missionComplete));
        report.AppendLine("- Observation reaches 100%: " + Status(observationComplete));
        report.AppendLine("- Transfer reaches 100%: " + Status(transferComplete));
        report.AppendLine("- Mission metrics calculated: " + Status(missionMetricsReady));
        report.AppendLine("- Score range 0-100: " + Status(scoreRangeValid));
        report.AppendLine("- Runtime Error/Exception logs: " + Status(runtimeLogsClean) + " (" + validationErrorLogCount + ")");
        report.AppendLine();
        report.AppendLine("## Runtime State");
        report.AppendLine("- Mission phase: " + (missionManager != null ? missionManager.Phase.ToString() : "Missing"));
        report.AppendLine("- Target: " + (missionManager != null && missionManager.SelectedTarget != null ? missionManager.SelectedTarget.targetName : "None"));
        report.AppendLine("- Observation progress: " + (missionManager != null ? Mathf.RoundToInt(missionManager.ObservationProgress * 100f).ToString() : "0") + "%");
        report.AppendLine("- Transfer progress: " + (missionManager != null ? Mathf.RoundToInt(missionManager.TransferProgress * 100f).ToString() : "0") + "%");
        report.AppendLine("- Communication available: " + (missionManager != null && missionManager.CommunicationAvailable));
        report.AppendLine("- Mission quality score: " + (missionManager != null ? missionManager.MissionQualityScore.ToString() : "0"));
        report.AppendLine("- Service reliability score: " + (missionManager != null ? missionManager.ServiceReliabilityScore.ToString() : "0"));
        report.AppendLine("- Satellite count: " + (satelliteManager != null ? satelliteManager.Satellites.Count.ToString() : "0"));
        report.AppendLine("- SAT-S01 state: " + (sharedSatellite != null ? sharedSatellite.availabilityState.ToString() : "Missing"));
        report.AppendLine("- SAT-S01 storage: " + (sharedSatellite != null ? sharedSatellite.dataStoragePercent.ToString("0.##") : "Missing"));
        report.AppendLine("- Target count: " + targets.Length);
        report.AppendLine("- Targets present: Seoul=" + (seoulTarget != null) + ", Jeju=" + (jejuTarget != null) + ", Pacific=" + (pacificTarget != null));
        report.AppendLine();
        report.AppendLine("## Notes");
        report.AppendLine("- Unity editor token, REST port, curl, and update-check warnings are environment noise unless accompanied by runtime exceptions.");
        report.AppendLine("- The editor automation intentionally enters Play mode and exits after validation; this is not an application crash.");

        File.WriteAllText(path, report.ToString());
    }

    private static string Status(bool value)
    {
        return value ? "PASS" : "FAIL";
    }

    private static SatelliteData FindSatellite(SatelliteManager satelliteManager, string satelliteName)
    {
        if (satelliteManager == null)
        {
            return null;
        }

        return satelliteManager.Satellites.Find(satellite => satellite.satelliteName == satelliteName);
    }

    private static ObservationTarget FindTarget(ObservationTarget[] targets, string targetName)
    {
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] != null && targets[i].targetName == targetName)
            {
                return targets[i];
            }
        }

        return null;
    }

    private static bool IsTargetDataPositive(ObservationTarget target)
    {
        return target != null
            && target.estimatedDataMb > 0f
            && target.resolutionMeters > 0f
            && target.missionCostCredits > 0;
    }

    private static bool IsScoreInRange(int score)
    {
        return score >= 0 && score <= 100;
    }
}

public class OrbitShareFrameCaptureAgent : MonoBehaviour
{
    private string directory;

    public void Initialize(string targetDirectory)
    {
        directory = targetDirectory;
    }

    private IEnumerator Start()
    {
        if (string.IsNullOrEmpty(directory))
        {
            directory = Path.GetFullPath("DemoFrames");
        }

        yield return new WaitForSecondsRealtime(1f);

        MissionManager missionManager = Object.FindObjectOfType<MissionManager>();
        if (missionManager != null)
        {
            missionManager.RunFastDemo();
        }

        for (int i = 0; i < 13; i++)
        {
            missionManager = Object.FindObjectOfType<MissionManager>();
            if (missionManager != null && missionManager.Phase != MissionPhase.Complete && i > 4)
            {
                missionManager.RunFastDemo();
            }

            yield return new WaitForEndOfFrame();

            Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();
            if (texture != null)
            {
                string framePath = Path.Combine(directory, "frame_" + i.ToString("000") + ".png");
                File.WriteAllBytes(framePath, texture.EncodeToPNG());
                Object.Destroy(texture);
            }

            yield return new WaitForSecondsRealtime(0.7f);
        }

        SessionState.SetBool("OrbitShare.CaptureDemoFrames", false);
        EditorApplication.isPlaying = false;
        EditorApplication.delayCall += delegate
        {
            Debug.Log("OrbitShare demo frames saved: " + directory);
            EditorApplication.Exit(0);
        };
    }
}
