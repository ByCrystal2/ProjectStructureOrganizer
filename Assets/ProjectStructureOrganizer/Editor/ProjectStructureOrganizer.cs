using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class ProjectStructureOrganizer : EditorWindow
{
    private Vector2 scrollPos;
    private string baseFolderName = "";
    private readonly string assetsRoot = "Assets";

    // Define desired folder structure
    // Keys: folder names; Values: subfolders
    private readonly Dictionary<string, string[]> structure = new Dictionary<string, string[]>()
    {
        {"Art", new string[]{"Characters", "Environment", "UI"}},
        {"Audio", new string[]{}},
        {"Materials", new string[]{}},
        {"Prefabs", new string[]{"UI", "Characters", "Environment", "Props", "Effects"}},
        {"Scenes", new string[]{"Levels", "UI"}},
        {"Scripts", new string[]{"Core", "Game", "Managers", "UI", "Systems"}},
        {"UI", new string[]{"HUD", "Popups", "Settings"}},
        {"Resources", new string[]{}},
        {"Plugins", new string[]{"ThirdParty", "Custom"}},
        {"StreamingAssets", new string[]{}},
        {"Testing", new string[]{"EditorTests", "RuntimeTests"}},
        {"Sandbox", new string[]{}}
    };

    // Folders that should always be created under Assets, not under base folder
    private readonly HashSet<string> rootFolders = new HashSet<string>()
    {
        "Plugins", "Sandbox", "StreamingAssets", "Resources"
    };

    private List<string> validationMessages = new List<string>();
    private List<string> unexpectedFolders = new List<string>();

    [MenuItem("Tools/Project Structure Organizer")]
    public static void ShowWindow()
    {
        GetWindow<ProjectStructureOrganizer>("Project Structure Organizer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Project Folder Structure Organizer", EditorStyles.boldLabel);
        GUILayout.Space(5);

        GUILayout.Label("Base Folder (under Assets):", EditorStyles.label);
        baseFolderName = EditorGUILayout.TextField(baseFolderName);
        GUILayout.Space(10);

        EditorGUI.BeginDisabledGroup(string.IsNullOrWhiteSpace(baseFolderName));
        if (GUILayout.Button("1. Create/Update Folder Structure", GUILayout.Height(30)))
        {
            CreateStructure();
        }
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(10);
        GUILayout.Label("2. Validate Current Structure", EditorStyles.boldLabel);
        if (GUILayout.Button("Validate Structure", GUILayout.Height(30)))
        {
            ValidateStructure();
        }

        GUILayout.Space(10);
        if (validationMessages.Count > 0)
        {
            GUILayout.Label("Validation Results:", EditorStyles.boldLabel);
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));
            foreach (var msg in validationMessages)
            {
                GUILayout.Label(msg, EditorStyles.helpBox);
            }
            GUILayout.EndScrollView();
            GUILayout.Space(5);

            if (unexpectedFolders.Count > 0)
            {
                if (GUILayout.Button("3. Move Unexpected Folders to Plugins/ThirdParty", GUILayout.Height(30)))
                {
                    MoveUnexpectedFolders();
                }
            }
        }
    }

    private void CreateStructure()
    {
        // Ensure base folder under Assets
        string basePath = $"{assetsRoot}/{baseFolderName}";
        CreateFolderIfNotExists(assetsRoot, baseFolderName);
        CreateGitkeep(basePath);

        foreach (var kvp in structure)
        {
            string key = kvp.Key;
            string parent;
            if (rootFolders.Contains(key))
            {
                // Under Assets root
                parent = $"{assetsRoot}/{key}";
            }
            else
            {
                // Under base folder
                parent = $"{basePath}/{key}";
            }

            string parentDir = Path.GetDirectoryName(parent).Replace("\\", "/");
            string folderName = Path.GetFileName(parent);
            CreateFolderIfNotExists(parentDir, folderName);
            CreateGitkeep(parent);

            foreach (string child in kvp.Value)
            {
                string childPath = $"{parent}/{child}";
                CreateFolderIfNotExists(parent, child);
                CreateGitkeep(childPath);
            }
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Structure Created/Updated", "Project folders have been created/updated successfully.", "OK");
    }

    private void CreateFolderIfNotExists(string parentDir, string folderName)
    {
        string fullPath = $"{parentDir}/{folderName}";
        if (!AssetDatabase.IsValidFolder(fullPath))
        {
            AssetDatabase.CreateFolder(parentDir, folderName);
            Debug.Log($"Created folder: {fullPath}");
        }
    }

    private void CreateGitkeep(string folderPath)
    {
        string fullPath = $"{folderPath}/.gitkeep";
        if (!File.Exists(fullPath))
        {
            File.WriteAllText(fullPath, string.Empty);
            AssetDatabase.ImportAsset(fullPath);
        }
    }

    private void ValidateStructure()
    {
        validationMessages.Clear();
        unexpectedFolders.Clear();
        string basePath = $"{assetsRoot}/{baseFolderName}";

        // Validate base folder
        if (!AssetDatabase.IsValidFolder(basePath))
        {
            validationMessages.Add($"Missing base folder: {basePath}");
        }

        // Check for missing folders
        foreach (var kvp in structure)
        {
            string key = kvp.Key;
            string parent;
            if (rootFolders.Contains(key))
            {
                parent = $"{assetsRoot}/{key}";
            }
            else
            {
                parent = $"{basePath}/{key}";
            }

            if (!AssetDatabase.IsValidFolder(parent))
            {
                validationMessages.Add($"Missing folder: {parent}");
            }
            else
            {
                foreach (string child in kvp.Value)
                {
                    string childPath = $"{parent}/{child}";
                    if (!AssetDatabase.IsValidFolder(childPath))
                    {
                        validationMessages.Add($"Missing subfolder: {childPath}");
                    }
                }
            }
        }

        // Check for unexpected top-level folders under Assets
        string[] topFolders = Directory.GetDirectories(assetsRoot);
        // Build expected set with proper forward slashes
        HashSet<string> expected = new HashSet<string>();
        expected.Add(basePath);
        foreach (var root in rootFolders)
        {
            expected.Add($"{assetsRoot}/{root}");
        }

        foreach (string folder in topFolders)
        {
            string normalized = folder.Replace("\\", "/");
            if (!expected.Contains(normalized))
            {
                validationMessages.Add($"Unexpected top-level folder: {normalized}");
                unexpectedFolders.Add(normalized);
            }
        }

        if (validationMessages.Count == 0)
        {
            validationMessages.Add("All folders are correctly set up! 🎉");
        }
    }

    private void MoveUnexpectedFolders()
    {
        if (unexpectedFolders.Count == 0)
        {
            EditorUtility.DisplayDialog("No Folders to Move", "There are no unexpected folders to move.", "OK");
            return;
        }

        // Ensure Plugins/ThirdParty exists
        string thirdPartyPath = $"{assetsRoot}/Plugins/ThirdParty";
        CreateFolderIfNotExists($"{assetsRoot}/Plugins", "ThirdParty");
        CreateGitkeep(thirdPartyPath);

        foreach (string folder in unexpectedFolders)
        {
            string folderName = Path.GetFileName(folder);
            string destination = $"{thirdPartyPath}/{folderName}";
            string moveAssetResult = AssetDatabase.MoveAsset(folder, destination);
            if (string.IsNullOrEmpty(moveAssetResult))
            {
                Debug.Log($"Moved {folder} to {destination}");
            }
            else
            {
                Debug.LogError($"Failed to move {folder} to {destination}: {moveAssetResult}");
            }
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Move Complete", "Unexpected folders moved to Plugins/ThirdParty.", "OK");

        // Re-validate to update messages
        ValidateStructure();
    }
}
