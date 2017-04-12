using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EmptySkull.TypeDatabases.Internal
{
    public class DatabasesOverview : EditorWindow
    {
        private static Database[] _allDatabases;

        private static Database[] AllDatabases
        {
            get
            {
                if (_allDatabases == null)
                    _allDatabases = DatabaseWriter.GetAllDatabases();
                return _allDatabases;
            }
        }

        private Vector2 _scrollPos;

        [MenuItem("Window/Databases/Overview")]
        public static void Initialize()
        {
            DatabasesOverview window = (DatabasesOverview) GetWindow(typeof (DatabasesOverview));
            window.Show();

            _allDatabases = null;
        }

        //Called by Unity
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnusedMember.Local
        private void OnGUI()
        {
            Color defaultColor = GUI.backgroundColor;

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Databases Overview", EditorStyles.boldLabel);

                if (GUILayout.Button("", new GUIStyle(GUI.skin.FindStyle("OL Plus")), GUILayout.Width(20)))
                {
                    DatabasesWindow.Initialize();
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (AllDatabases.Length <= 0)
                EditorGUILayout.LabelField("No databases found ...");

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            {
                if (AllDatabases != null)
                {
                    foreach (Database database in AllDatabases.OrderBy(t => t.Name))
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            var temp = database;
                            if (AllDatabases.Select(t => t.Name).Count(t => t == temp.Name) > 1)
                                GUI.backgroundColor = new Color(1, 0.3f, 0.3f);

                            if (GUILayout.Button(database.Name))
                            {
                                DatabaseWindow.CurrentDatabaseIndex = Array.IndexOf(AllDatabases, database);
                                DatabaseWindow.Initialize();
                            }

                            GUI.backgroundColor = defaultColor;

                            if (GUILayout.Button("X", GUILayout.Width(40)) &&
                                EditorUtility.DisplayDialog("Detete " + database.Name,
                                    string.Format("Do you want to delete the database: {0}?", database.Name)
                                    , "Yes", "Cancel"))
                            {
                                DatabaseWriter.RemoveDatabase(database);
                                _allDatabases = null;
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            EditorGUILayout.EndScrollView();

            CreateButton();

            GUI.backgroundColor = defaultColor;
        }

        private static void CreateButton()
        {
            if (GUILayout.Button("Create new"))
            {
                DatabaseWriter.CreateNewDatabase(
                    new DatabaseSettings(
                        DatabaseWriter.GetItteratedValidString("NewDatabase", AllDatabases.Select(t => t.Name).ToArray(),
                            2),
                        new Type[0]));
                _allDatabases = null;
            }
        }
    }
}