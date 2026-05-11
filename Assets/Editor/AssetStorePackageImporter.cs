using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class AssetStorePackageImporter
{
    private const string PackageDirectory = "AssetStorePackages";
    private const string ResourceDirectory = "Assets/Resources/OrbitShare";
    private const string ReportPath = "asset-store-import-report.md";

    [MenuItem("OrbitShare/Asset Store/Import UnityPackages")]
    public static void ImportUnityPackages()
    {
        Directory.CreateDirectory(PackageDirectory);
        string[] packages = Directory.GetFiles(PackageDirectory, "*.unitypackage", SearchOption.AllDirectories);
        if (packages.Length == 0)
        {
            Debug.LogWarning("No .unitypackage files found in " + Path.GetFullPath(PackageDirectory));
            WriteReport("No .unitypackage files found. Download Asset Store packages into AssetStorePackages first.");
            return;
        }

        foreach (string packagePath in packages)
        {
            Debug.Log("Importing Asset Store package: " + packagePath);
            AssetDatabase.ImportPackage(packagePath, false);
        }

        AssetDatabase.Refresh();
        WriteReport("Imported " + packages.Length + " package file(s). Move selected imported assets into " + ResourceDirectory + " using the expected names.");
    }

    [MenuItem("OrbitShare/Asset Store/Validate Resource Hooks")]
    public static void ValidateResourceHooks()
    {
        Directory.CreateDirectory(ResourceDirectory);
        StringBuilder report = new StringBuilder();
        report.AppendLine("# OrbitShare Asset Store Resource Report");
        report.AppendLine();
        report.AppendLine("Resource directory: `" + ResourceDirectory + "`");
        report.AppendLine();
        report.AppendLine("## Expected Runtime Assets");
        AppendAsset(report, "Satellite prefab", ResourceDirectory + "/SatelliteModel.prefab");
        AppendAsset(report, "Space station prefab", ResourceDirectory + "/SpaceStation.prefab");
        AppendAsset(report, "Earth material", ResourceDirectory + "/PremiumEarthMaterial.mat");
        AppendAsset(report, "Space skybox", ResourceDirectory + "/SpaceSkybox.mat");
        AppendAsset(report, "Reserve SFX", ResourceDirectory + "/SFX_Reserve.wav");
        AppendAsset(report, "Start SFX", ResourceDirectory + "/SFX_Start.wav");
        AppendAsset(report, "Observe SFX", ResourceDirectory + "/SFX_Observe.wav");
        AppendAsset(report, "Transmit SFX", ResourceDirectory + "/SFX_Transmit.wav");
        AppendAsset(report, "Complete SFX", ResourceDirectory + "/SFX_Complete.wav");
        report.AppendLine();
        report.AppendLine("Assets marked MISSING are optional. OrbitShare falls back to procedural visuals and generated tones.");
        File.WriteAllText(ReportPath, report.ToString());
        AssetDatabase.Refresh();
        Debug.Log("Asset Store resource report saved: " + Path.GetFullPath(ReportPath));
    }

    public static void ImportAndValidateForBatch()
    {
        ImportUnityPackages();
        ValidateResourceHooks();
        EditorApplication.Exit(0);
    }

    private static void AppendAsset(StringBuilder report, string label, string assetPath)
    {
        report.AppendLine("- " + label + ": " + (File.Exists(assetPath) ? "FOUND" : "MISSING") + " `" + assetPath + "`");
    }

    private static void WriteReport(string message)
    {
        File.WriteAllText(ReportPath, "# OrbitShare Asset Store Import\n\n" + message + "\n");
        AssetDatabase.Refresh();
    }
}
