using System.Linq;
using System.Text;
using Ink.UnityIntegration;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif

internal class InkPreBuildValidationCheck :
#if UNITY_2018_1_OR_NEWER
    IPreprocessBuildWithReport
#else
IPreprocessBuild
#endif
{
    public int callbackOrder => 0;

#if UNITY_2018_1_OR_NEWER
    public void OnPreprocessBuild(BuildReport report) {
        PreprocessValidationStep();
    }
#else
    public void OnPreprocessBuild(BuildTarget target, string path) {
		PreprocessValidationStep();
	}
#endif

    private static void PreprocessValidationStep() {
        AssertNotCompiling();
        CheckForInvalidFiles();
    }

    private static void AssertNotCompiling() {
        if (InkCompiler.compiling) {
            var sb = new StringBuilder("Ink is currently compiling!");
            var errorString = sb.ToString();
            InkCompiler.buildBlocked = true;
            if (EditorUtility.DisplayDialog("Ink Build Error!", errorString, "Ok")) Debug.LogError(errorString);
        }
    }

    // When syncronous compilation is allowed we should try to replace this error with a compile.
    private static void CheckForInvalidFiles() {
        var filesToRecompile = InkLibrary.GetFilesRequiringRecompile();
        if (filesToRecompile.Any()) {
            var sb = new StringBuilder();
            sb.AppendLine(
                "There are Ink files which should be compiled, but appear not to be. You can resolve this by either:");
            sb.AppendLine(" - Compiling your files via 'Assets/Recompile Ink'");
            var resolveStep = " - Disabling 'Compile Automatically' " + (InkSettings.instance.compileAutomatically
                ? "in your Ink Settings file"
                : "for each of the files listed below");
            sb.AppendLine(resolveStep);
            sb.AppendLine();
            sb.AppendLine("Files:");
            var filesAsString = string.Join(", ", filesToRecompile.Select(x => x.filePath).ToArray());
            sb.AppendLine(filesAsString);
            var errorString = sb.ToString();
            if (!EditorUtility.DisplayDialog("Ink Build Error!", errorString, "Build anyway", "Cancel build"))
                Debug.LogError(errorString);
            else
                Debug.LogWarning(errorString);
        }
    }
}