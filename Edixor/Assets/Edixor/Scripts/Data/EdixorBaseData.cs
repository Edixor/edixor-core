using System.Collections.Generic;
using UnityEngine;
using System.IO; 
using System;  

[CreateAssetMenu(fileName = "EdixorData", menuName = "Edixor/EdixorData", order = 0)]
public class EdixorData : ScriptableObject {
    
    [SerializeField] private List<DataPlagin> plagins;
    [SerializeField] private DataTranslation languages;
    [SerializeField] private string version = "00.00.00";
    [SerializeField] private string mainPath;
    public DataStyle styles = new DataStyle();

    public List<DataPlagin> Plagins => plagins;
    public DataTranslation Languages => languages;
    public string Version => version;
    public string MainPath => mainPath;

    public bool developerMode;

    private void OnEnable() {
        if (string.IsNullOrEmpty(mainPath)) {
            InitializeMainPath();
        }
    }

    public void SetVersion(string version) {
        if (developerMode)
        {
            this.version = version;
        }
    }
    public void SetMainPath(string path) {
        if (developerMode)
        {
            this.mainPath = path;
        }
    }

    public void InitializeMainPath() {
        string folderName = "Edixor";
        string rootPath = Directory.GetDirectoryRoot(Application.dataPath);
        mainPath = FindFolder(rootPath, folderName);

        if (string.IsNullOrEmpty(mainPath)) {
            Debug.LogError($"Folder '{folderName}' not found.");
        } else {
            Debug.Log($"Folder '{folderName}' found at: {mainPath}");
        }
    }

    public void InitializeStyle() {
        
    }

    private string FindFolder(string root, string folderName) {
        try {
            foreach (string dir in Directory.GetDirectories(root)) {
                if (dir.EndsWith(folderName)) {
                    return dir;
                }

                string found = FindFolder(dir, folderName);
                if (!string.IsNullOrEmpty(found)) {
                    return found;
                }
            }
        } catch (UnauthorizedAccessException) {
        } catch (Exception) {
        }
        return null;
    }
}
