using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public static class ExtensionUtils
{
    public static bool IsInstalled(string name)
    {
        string path = Path.Combine(Application.dataPath, "Extensions", name);
        return Directory.Exists(path);
    }

    public static string FindRootDirectory(string extractDir, string name)
    {
        var subdirs = Directory.GetDirectories(extractDir);
        if (subdirs.Length == 1 && Path.GetFileName(subdirs[0]).Equals(name, StringComparison.OrdinalIgnoreCase))
            return subdirs[0];
        return extractDir;
    }
}
