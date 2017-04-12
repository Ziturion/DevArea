using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EmptySkull.TypeDatabases.Internal
{
    public class DatabasesWindow : EditorWindow
    {
        private const int LableWidth = 100;

        private static string _tempDatabasesPath;
        private static string _tempEnumsPath;

        public static void Initialize()
        {
            _tempDatabasesPath = DatabaseUtilities.DatabasesPath;
            _tempEnumsPath = DatabaseUtilities.DatabaseEnumsPath;

            DatabasesWindow window = (DatabasesWindow)GetWindow(typeof(DatabasesWindow));
            window.titleContent = new GUIContent("Databases");
            window.Show();
        }

        //Called by Unity
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Databases Settings", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold }); //TODO Cage?
            EditorGUILayout.Space();

            if (_tempDatabasesPath == null || _tempEnumsPath == null)
            {
                EditorGUILayout.LabelField("Not initialized...");
                return;
            }

            string customDatabasePath;
            string customEnumPath;

            EditorGUILayout.BeginHorizontal();
            {
                string prePath = "Assets/Resources";
                EditorGUILayout.LabelField("Databases-Path", GUILayout.Width(LableWidth));
                GUI.enabled = false;
                EditorGUILayout.TextField(prePath, GUILayout.Width(120));
                GUI.enabled = true;
                customDatabasePath =
                    EditorGUILayout.TextField(_tempDatabasesPath.Replace(prePath, string.Empty));
                _tempDatabasesPath = prePath + customDatabasePath;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                string prePath = "Assets";
                EditorGUILayout.LabelField("Enum-Path", GUILayout.Width(LableWidth));
                GUI.enabled = false;
                EditorGUILayout.TextField(prePath, GUILayout.Width(50));
                GUI.enabled = true;
                customEnumPath =
                    EditorGUILayout.TextField(_tempEnumsPath.Replace(prePath, string.Empty));
                _tempEnumsPath = prePath + customEnumPath;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            if (_tempDatabasesPath == DatabaseUtilities.DefaultDatabasesPath
                && _tempEnumsPath == DatabaseUtilities.DefaultEnumsPath)
                GUI.enabled = false;
            if (GUILayout.Button("Reset paths to default"))
            {
                GUI.FocusControl(null);

                _tempDatabasesPath = DatabaseUtilities.DefaultDatabasesPath;
                _tempEnumsPath = DatabaseUtilities.DefaultEnumsPath;
            }
            GUI.enabled = true;

            if (!CheckIfIsValidPath(_tempDatabasesPath) || !CheckIfIsValidPath(_tempEnumsPath)
                || (customDatabasePath.Length > 0 && customDatabasePath[0] != '/') ||
                (customEnumPath.Length > 0 && customEnumPath[0] != '/'))
            {
                EditorGUILayout.HelpBox("One or more paths are not valid!", MessageType.Warning);
                GUI.enabled = false;
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Apply"))
            {
                ApplyValues();
                GetWindow(typeof(DatabasesWindow)).Close();
            }
            GUI.enabled = true;
        }

        private void ApplyValues()
        {
            DatabaseUtilities.DatabasesPath = _tempDatabasesPath;
            DatabaseUtilities.DatabaseEnumsPath = _tempEnumsPath;
        }

        private bool CheckIfIsValidPath(string path)
        {
            return Path.GetInvalidPathChars().All(invalidFileNameChar => !path.Contains(invalidFileNameChar));
        }
    }
}