# Project Structure Organizer

![License](https://img.shields.io/badge/license-MIT-blue.svg)  
![Unity Version](https://img.shields.io/badge/unity-2019.4%2B-green.svg)

Project Structure Organizer is a Unity Editor extension that helps you keep your project’s folder hierarchy clean and consistent. It allows you to:

1. Create a custom **Base Folder** under `Assets/` and automatically generate subfolders (Art, Scripts, Scenes, Prefabs, etc.).  
2. Validate existing folders, reporting any missing or “unexpected” top-level directories.  
3. Move unexpected top-level folders (e.g., Asset Store imports) into `Assets/Plugins/ThirdParty` with a single click.

---

## 🔍 Features

- **One-Click Setup**: Define a `Base Folder` name and create the entire folder structure under `Assets/BaseFolder` in one go.  
- **Validation**: Identify missing subfolders or any extra top-level folders that aren’t part of the template.  
- **Auto-Cleanup**: Instantly move all unexpected top-level folders into `Assets/Plugins/ThirdParty`.  
- **Easy Customization**: Update the `structure` dictionary in the C# source to add or remove any root/subfolders.  

---

## 🚀 Quick Start

### Unity Package Manager (UPM) Installation

1. Open your Unity project (Unity 2019.4 or newer).  
2. Go to **Window > Package Manager**.  
3. Click the “+” icon in the top left and select **“Add package from git URL…”**.  
4. Paste your repository URL plus the desired Git tag, for example:  
https://github.com/ByCrystal2/ProjectStructureOrganizer/releases/tag/V1.0.0
5. Click **Add** and wait for the package to finish importing.

### Manual Installation (Copy Files)

1. Clone or download the repo:  
```bash
git clone https://github.com/ByCrystal2/ProjectStructureOrganizer.git
2. Copy the folder Assets/Plugins/ProjectStructureOrganizer from the downloaded repo into your Unity project’s Assets/Plugins/ directory.

3. Switch back to Unity and let it compile. You should now see the menu item under Tools > Project Structure Organizer.

🛠️ Usage Instructions
1. Open the Tool Window
In the Unity Editor, navigate to:
Tools > Project Structure Organizer
2. Enter a Base Folder Name
In the field labeled “Base Folder (under Assets):”, type the name you want to use (e.g., MyGame). This will create Assets/MyGame and place all required subfolders inside.

3. Create or Update Folder Structure
Click “1. Create/Update Folder Structure”.

- If Assets/<BaseFolderName> does not exist, it will be created automatically, along with all configured subfolders.

- Folders listed in the rootFolders set (Plugins, Sandbox, StreamingAssets, Resources) will always be created directly under Assets/.

4. Validate Your Project Structure
Click “Validate Structure”.

- Any missing required folders will appear as “Missing folder: Assets/…” messages.

- Any top-level folder under Assets/ that is not part of the defined template (including your Base Folder and the rootFolders) will show up as “Unexpected top-level folder: Assets/…”.

5. Move Unexpected Folders
If any “Unexpected top-level folder” messages appear, a third button will become active: “3. Move Unexpected Folders to Plugins/ThirdParty”.

- Clicking this will create Assets/Plugins/ThirdParty (if not already present) and relocate all listed unexpected folders there.

- After moving, the tool re-validates the folder structure to confirm the changes.

⚙️ Configuration & Customization
- Modify the Folder Template
In ProjectStructureOrganizer.cs, locate the structure dictionary:
private readonly Dictionary<string, string[]> structure = new Dictionary<string, string[]>()
{
    {"Art",        new string[]{"Characters", "Environment", "UI"}},
    {"Audio",      new string[]{}},
    {"Materials",  new string[]{}},
    // … others …
};
- To add a new root folder (e.g., Localization), insert:
{"Localization", new string[]{"Languages", "Fonts"}},
- This will ensure Assets/<BaseFolder>/Localization/Languages and Assets/<BaseFolder>/Localization/Fonts get created.

- Adjust the Root-Level Exceptions
The rootFolders set determines which top-level folders always remain directly under Assets/. By default:
private readonly HashSet<string> rootFolders = new HashSet<string>()
{
    "Plugins", "Sandbox", "StreamingAssets", "Resources"
};
- If you want additional root-level directories (e.g. ThirdParty, EditorResources), add them:
rootFolders.Add("ThirdParty");
- .gitkeep File Behavior
By default, each created folder receives a .gitkeep file so that empty directories are preserved in Git.
	- If you prefer not to include .gitkeep, remove or comment out calls to CreateGitkeep(folderPath).

- Localization / Language Updates
If you want to change dialog or log messages from English to another language:
	- Search for EditorUtility.DisplayDialog or Debug.Log in the code and replace the strings accordingly.

⚠️ Warnings & Best Practices
1. Base Folder Name Is Mandatory
	- The “Base Folder (under Assets)” field cannot be left blank.
	- Use alphanumeric names without spaces or special characters to avoid path issues.
2. Existing Folders Might Be Overwritten
	- If a folder already exists but its subfolders differ from the template, running “Create/Update Folder Structure” will add any missing subfolders and .gitkeep files.
	- It will not delete any user-added files inside existing directories—but always backup your project before making sweeping structural changes.
3. Asset Store or Third-Party Packages
	- Imported Asset Store packages often create their own top-level folder under Assets/.
	- The tool will mark these as “Unexpected” unless you move them to Assets/Plugins/ThirdParty (recommended) or add their folder name to rootFolders.
4. Unity Version Compatibility
	- This tool was built and tested on Unity 2019.4 and later.
	- It uses APIs (e.g., AssetDatabase.CreateFolder) that are unavailable in very old Unity versions (prior to Unity 5). Consider upgrading if you encounter errors.
5. Version Control
	- The .gitkeep files ensure empty folders remain in your Git repository. If you use Perforce, SVN, or another VCS, adjust accordingly (e.g., place .p4ignore or other placeholders).

🤝 Contributing
We welcome feedback, bug reports, and pull requests! To contribute:
1. Fork the repository and create a feature/bugfix branch:
git checkout -b feature/new-folder-template
2. Make your changes, add any new tests or examples, and commit using clear messages:
git commit -m "Add Localization folder under Base Folder"
3. Push your branch and open a Pull Request on GitHub.
4. Ensure your code compiles in Unity and passes any existing tests.

Branch Strategy
- main (or master): Always contains stable, release-ready code.
- feature/: New features or enhancements.
- bugfix/: Patches or corrections to existing features.

Please follow standard GitHub Flow and ensure your PR description explains what problem you’re solving or feature you’re adding.

📜 Changelog
See CHANGELOG.md for a chronological list of changes. A sample entry might look like:
## [1.0.0] – 2025-06-02
### Added
- Initial release of Project Structure Organizer.
- One-click creation of Base Folder and subfolders.
- Validate Structure button.
- Move Unexpected Folders functionality.

### Changed
- N/A

### Fixed
- N/A

📄 License
This project is licensed under the MIT License. See LICENSE for details.
	- In short, you are free to use, copy, modify, and distribute this software provided that you include the original copyright and license notice.

Acknowledgments & References
- Unity Editor Scripting Documentation:
	https://docs.unity3d.com/ScriptReference/EditorWindow.html
- Unity Project Organization Best Practices:
	- https://gamedevbeginner.com/how-to-structure-your-unity-project-best-practice-tips/
	- https://github.com/justinwasilenko/Unity-Style-Guide
- Community Contributions:
	This tool was inspired by various open-source Unity folder-structure utilities.

-----> Author:
Ahmet Burak: https://github.com/ByCrystal2