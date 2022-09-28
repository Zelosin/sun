using UnityEditor;
using UnityEngine;

namespace Ink.UnityIntegration {
    [InitializeOnLoad]
    public class InkUnityIntegrationStartupWindow : EditorWindow {
        private const string editorPrefsKeyForVersionSeen = "Ink Unity Integration Startup Window Version Confirmed";
        private const int announcementVersion = 2;
        private static int announcementVersionPreviouslySeen;

        private static Texture2D _logoIcon;

        private Vector2 scrollPosition;

        static InkUnityIntegrationStartupWindow() {
            EditorApplication.delayCall += TryCreateWindow;
        }

        public static Texture2D logoIcon {
            get {
                if (_logoIcon == null) _logoIcon = Resources.Load<Texture2D>("InkLogoIcon");
                return _logoIcon;
            }
        }

        private void OnGUI() {
            EditorGUILayout.BeginVertical();
            var areaSize = new Vector2(90, 90);
            GUILayout.BeginArea(new Rect((position.width - areaSize.x) * 0.5f, 15, areaSize.x, areaSize.y));
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(new GUIContent(logoIcon), GUILayout.Width(areaSize.x),
                GUILayout.Height(areaSize.x * ((float)logoIcon.height / logoIcon.width)));
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Version " + InkLibrary.unityIntegrationVersionCurrent,
                EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.LabelField("Ink version " + InkLibrary.inkVersionCurrent,
                EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.EndVertical();
            GUILayout.EndArea();

            GUILayout.Space(20 + areaSize.y);

            if (announcementVersionPreviouslySeen == -1) {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField("New to ink?", EditorStyles.boldLabel);
                EditorGUILayout.EndVertical();
            }

            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("About Ink")) Application.OpenURL("https://www.inklestudios.com/ink/");
                if (GUILayout.Button("❤️Support Us!❤️")) Application.OpenURL("https://www.patreon.com/inkle");
                if (GUILayout.Button("Close")) Close();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                {
                    // 1.0.0
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUILayout.LabelField("🎉Version 1.0.0🎉:", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("• Update ink to 1.0.0", EditorStyles.wordWrappedLabel);
                    EditorGUILayout.LabelField("• Ink Editor Window: Allow resizing (some) panels",
                        EditorStyles.wordWrappedLabel);
                    EditorGUILayout.LabelField("• Ink Editor Window: Named content panel ",
                        EditorStyles.wordWrappedLabel);
                    EditorGUILayout.LabelField("• Ink Editor Window: Improved performance for large stories",
                        EditorStyles.wordWrappedLabel);
                    EditorGUILayout.LabelField(
                        "• Allow compiling include files that don't have the .ink file extension",
                        EditorStyles.wordWrappedLabel);
                    EditorGUILayout.LabelField("• Remove ability to use a custom inklecate (legacy compiler)",
                        EditorStyles.wordWrappedLabel);
                    EditorGUILayout.LabelField("• Fixes settings menu on 2020+", EditorStyles.wordWrappedLabel);
                    EditorGUILayout.LabelField("• Improved migration from earlier versions",
                        EditorStyles.wordWrappedLabel);
                    EditorGUILayout.LabelField(
                        "• Moved persistent compilation tracking code from InkLibrary into InkCompiler",
                        EditorStyles.wordWrappedLabel);
                    EditorGUILayout.LabelField(
                        "• Use Unity's new ScriptableSingleton for InkLibrary, InkSettings and InkCompiler on 2020+",
                        EditorStyles.wordWrappedLabel);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    // 0.9.71
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUILayout.LabelField("Version 0.9.71:", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("• Resolves some compilation issues.", EditorStyles.wordWrappedLabel);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    // 0.9.60
                    EditorGUILayout.LabelField("Version 0.9.60:", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField(
                        "• Moved InkLibrary and InkSettings from Assets into Library and ProjectSettings.",
                        EditorStyles.wordWrappedLabel);
                    EditorGUILayout.LabelField("   ‣ InkLibrary should no longer be tracked in source control.",
                        EditorStyles.wordWrappedLabel);
                    EditorGUILayout.LabelField("   ‣ Changes to InkSettings must be migrated manually.",
                        EditorStyles.wordWrappedLabel);
                    EditorGUILayout.LabelField(
                        "   ‣ The InkLibrary and InkSettings files in your project folder should be deleted.",
                        EditorStyles.wordWrappedLabel);
                    EditorGUILayout.LabelField(
                        "• Added a divertable list of knots, stitches and other named content to the Ink Editor Window, replacing the Diverts subpanel.",
                        EditorStyles.wordWrappedLabel);
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.Space();

            EditorGUILayout.EndVertical();
        }

        private static void TryCreateWindow() {
            announcementVersionPreviouslySeen = EditorPrefs.GetInt(editorPrefsKeyForVersionSeen, -1);
            if (announcementVersion != announcementVersionPreviouslySeen) ShowWindow();
        }

        public static void ShowWindow() {
            var window =
                GetWindow(typeof(InkUnityIntegrationStartupWindow), true,
                    "Ink Update " + InkLibrary.unityIntegrationVersionCurrent,
                    true) as InkUnityIntegrationStartupWindow;
            window.minSize = new Vector2(200, 200);
            var size = new Vector2(520, 320);
            window.position = new Rect((Screen.currentResolution.width - size.x) * 0.5f,
                (Screen.currentResolution.height - size.y) * 0.5f, size.x, size.y);
            EditorPrefs.SetInt(editorPrefsKeyForVersionSeen, announcementVersion);
        }
    }
}