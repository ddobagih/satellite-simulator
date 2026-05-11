using System.IO;
using UnityEditor;

public static class BuildOrbitShare
{
    [MenuItem("OrbitShare/Build/Linux")]
    public static void BuildLinux()
    {
        Build(BuildTarget.StandaloneLinux64, "Builds/Linux/OrbitShare.x86_64");
    }

    [MenuItem("OrbitShare/Build/Windows")]
    public static void BuildWindows()
    {
        Build(BuildTarget.StandaloneWindows64, "Builds/Windows/OrbitShare.exe");
    }

    [MenuItem("OrbitShare/Prepare Submission Folder")]
    public static void PrepareSubmissionFolder()
    {
        string root = "Submission/OrbitShare";
        Directory.CreateDirectory(root);
        CopyIfExists("README.md", Path.Combine(root, "README.md"));
        CopyIfExists("ASSET_STORE_IMPORT.md", Path.Combine(root, "ASSET_STORE_IMPORT.md"));
        CopyIfExists("orbitshare_shared_satellite_agent.md", Path.Combine(root, "assignment_brief.md"));
        File.WriteAllText(
            Path.Combine(root, "SUBMISSION_CHECKLIST.txt"),
            "OrbitShare submission checklist\n" +
            "- Unity project folder\n" +
            "- Built executable from OrbitShare > Build\n" +
            "- Presentation PPT\n" +
            "- Demo video\n" +
            "- README.md\n" +
            "- ASSET_STORE_IMPORT.md if optional assets were used\n");
        AssetDatabase.Refresh();
    }

    private static void Build(BuildTarget target, string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
        BuildPipeline.BuildPlayer(
            new[] { "Assets/Scenes/MainScene.unity" },
            outputPath,
            target,
            BuildOptions.None);
    }

    private static void CopyIfExists(string source, string destination)
    {
        if (File.Exists(source))
        {
            File.Copy(source, destination, true);
        }
    }
}
