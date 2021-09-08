using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

#if UNITY_EDITOR

/// <summary>
/// Prebuild event class.
/// Used for copying data from Excel
/// </summary>
[ExecuteInEditMode]
public class Prebuild : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    /// <summary>
    /// Building process started.
    /// All Excel data should be copied to resources by XMLExport app.
    /// </summary>
    /// <param name="report"> not used </param>
    public void OnPreprocessBuild(BuildReport report)
    {
        string curDir = Directory.GetCurrentDirectory();
        string filename = "XMLExport.exe";
        while (!File.Exists(curDir + "\\" + filename))
            curDir = Directory.GetParent(curDir).FullName;
        string strCmdText = "/C " + filename;
        System.Diagnostics.Process.Start("CMD.exe", strCmdText); //Start cmd process

        Debug.Log("xml files copied");
    }
}

#endif
