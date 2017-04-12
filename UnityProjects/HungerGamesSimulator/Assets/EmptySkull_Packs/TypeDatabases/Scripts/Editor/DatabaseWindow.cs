using System;
using System.Linq;
using EmptySkull.Utilities;
using UnityEditor;
using UnityEngine;

namespace EmptySkull.TypeDatabases.Internal
{
    public class DatabaseWindow : EditorWindow
    {
        private static Database _currentDatabase;
        public static Database CurrentDatabase
        {
            get
            {
                if (_currentDatabase != null)
                    return _currentDatabase;
                if (CurrentDatabaseIndex < 0 || CurrentDatabaseIndex > DatabaseWriter.GetAllDatabases().Length - 1)
                    return null;
                _currentDatabase = DatabaseWriter.GetAllDatabases()[CurrentDatabaseIndex];
                return _currentDatabase;
            }
        }

        public static int CurrentDatabaseIndex
        {
            set { EditorPrefs.SetInt("typedatabases_SelectedDatabaseId", value); }
            get { return EditorPrefs.GetInt("typedatabases_SelectedDatabaseId"); }
        }

        private Vector2 _scrollPos;
        private int _currentlySelected = -1;
        private int _currentlySelectedIndex = -1;

        private Color ErrorColor
        {
            get
            {
                if (CurrentDatabase == null)
                    return Color.black;
                return CurrentDatabase.Settings.AutoUpdateEnum ? new Color(1f, 0.3f, 0.3f) : new Color(1f, 0.6f, 0.6f);
            }
        }

        [MenuItem("Window/Databases/Inspector")]
        public static void Initialize()
        {
            DatabaseWindow window = (DatabaseWindow)GetWindow(typeof(DatabaseWindow));
            window.Show();

            //Force-reset Database-Property to recalculate by next access
            _currentDatabase = null;

            //Updates the assignable types when window is called
            UpdateAssignableTypes();
        }

        //Called by Unity
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnusedMember.Local
        private void OnGUI()
        {
            Color defaultColor = GUI.contentColor;

            if (CurrentDatabase == null)
            {
                EditorGUILayout.LabelField("No database selected");
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("", GUILayout.Width(16));
                Rect lastRect = GUILayoutUtility.GetLastRect();
                if (GUI.Button(new Rect(lastRect.x, lastRect.y + 4, lastRect.width, lastRect.width), "", "AC LeftArrow"))
                {
                    DatabasesOverview.Initialize();
                }
                GUILayout.Label(CurrentDatabase.Name, EditorStyles.boldLabel);

                if (GUILayout.Button("", new GUIStyle(GUI.skin.FindStyle("OL Plus")), GUILayout.Width(20)))
                {
                    DatabaseSettingWindow.Initialize(ref _currentDatabase);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            {
                DatabaseAsset[] sortedDbAssets = CurrentDatabase.Assets.ToArray();

                switch (CurrentDatabase.Settings.SortingBehaviour)
                {
                    case DatabaseSortingBehaviour.OrderByIndex:
                        sortedDbAssets = sortedDbAssets.OrderBy(t => t.Index).ToArray();
                        break;
                    case DatabaseSortingBehaviour.OrderByName:
                        sortedDbAssets = sortedDbAssets.OrderBy(t => t.Name).ToArray();
                        break;
                }

                for (int i = 0; i < sortedDbAssets.Length; i++)
                {
                    DatabaseAsset asset = sortedDbAssets[i];

                    EditorGUILayout.BeginHorizontal();
                    {
                        if (_currentlySelectedIndex == i)
                        {
                            int oldIndex = asset.Index;
                            int newIndex = EditorGUILayout.DelayedIntField(oldIndex, GUILayout.Width(40));

                            if (newIndex > 0 && newIndex != oldIndex)
                                DatabaseWriter.ChangeItemsIndex(CurrentDatabase, asset.Index, newIndex);

                            if (oldIndex != asset.Index)
                            {
                                _currentlySelectedIndex = -1;
                                _currentlySelected = asset.Index;
                            }
                        }
                        else
                        {
                            if (GUILayout.Button(string.Format("#{0:000}", asset.Index), GUILayout.Width(40)))
                            {
                                _currentlySelected = -1;
                                _currentlySelectedIndex = i;
                            }
                        }

                        if (!AssetNameIsValid(asset) || !AssetNameIsUnique(asset))
                            GUI.backgroundColor = ErrorColor;

                        if (GUILayout.Button(asset.Name))
                        {
                            //TODO This error-managemant causes unexpected behaviour : Refactor
                            bool nameFieldSelected = GUI.GetNameOfFocusedControl() == "nameField";
                            GUI.FocusControl(null);
                            if (nameFieldSelected)
                                return;
                            // -----

                            if (_currentlySelected == asset.Index)
                                _currentlySelected = -1;
                            else
                            {
                                _currentlySelected = asset.Index;
                                _currentlySelectedIndex = -1;
                            }
                        }
                        GUI.backgroundColor = defaultColor;

                        if (GUILayout.Button("X", GUILayout.Width(40)) &&
                            EditorUtility.DisplayDialog("Detete " + asset.Name,
                            string.Format("Do you want to delete the asset: #{0:000} {1} " +
                                          "from the Database: {2}?", asset.Index, asset.Name,
                                          CurrentDatabase.Name),
                            "Yes", "Cancel"))
                        {
                            DatabaseWriter.RemoveAssetFromDatabase(i, CurrentDatabase);
                            if (_currentlySelected == asset.Index)
                                _currentlySelected = -1;

                            if (_currentDatabase.Settings.AutoUpdateEnum)
                                DatabaseWriter.CreateDatabaseEnum(_currentDatabase);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    if (_currentlySelected == asset.Index)
                    {
                        EditorGUILayout.BeginVertical("Box");
                        {
                            EditorGUIUtility.labelWidth = 100;

                            EditorGUILayout.LabelField("< " + asset.GetType().Name + " >");

                            bool nameInvalid = !AssetNameIsValid(asset);
                            bool nameNotUnique = !AssetNameIsUnique(asset);
                            if (nameInvalid || nameNotUnique)
                                GUI.backgroundColor = ErrorColor;

                            string oldName = asset.Name;

                            GUI.SetNextControlName("nameField");
                            string newName = EditorGUILayout.DelayedTextField("Name", oldName);

                            asset.Name = newName;

                            if (oldName != newName && _currentDatabase.Settings.AutoUpdateEnum
                                && AssetNameIsValid(asset) && AssetNameIsUnique(asset))
                                DatabaseWriter.CreateDatabaseEnum(_currentDatabase);

                            GUI.backgroundColor = defaultColor;

                            MessageType mType = CurrentDatabase.Settings.AutoUpdateEnum
                                ? MessageType.Warning
                                : MessageType.Info;

                            if (nameInvalid)
                                EditorGUILayout.HelpBox("This name is invalid for enum-generation", mType);

                            if (nameNotUnique)
                                EditorGUILayout.HelpBox("This name is not unique for enum-generation", mType);

                            SerializedObject sObj = new SerializedObject(asset);

                            //if (GUILayout.Button("Select in assets")) //Does work ... place elsewhere
                            //    Selection.activeObject = sObj.targetObject;

                            SerializedProperty sProp = sObj.GetIterator();
                            while (sProp.NextVisible(true))
                            {
                                //Dont display "Name" cause its displayed before
                                if (sProp.displayName == "Name")
                                    continue;

                                //Dont display "Index" cause its displayed in list
                                if (sProp.displayName == "Index")
                                    continue;

                                //Dont display "Script"-Referenc
                                if (sProp.displayName == "Script")
                                    continue;

                                //Dont display in-Array-Propertys (again)
                                if (sProp.propertyPath.Contains("Array"))
                                    continue;

                                //Display current property as default (or by attribute set) field
                                EditorGUILayout.PropertyField(sProp, true);
                            }

                            sObj.ApplyModifiedProperties();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
            }
            EditorGUILayout.EndScrollView();

            if (CurrentDatabase.Settings.HasMultipleChoices)
            {
                EditorGUILayout.LabelField(string.Empty, GUILayout.Height(18));
                Rect r = GUILayoutUtility.GetLastRect();

                int optionPanelWidth = 20;
                Rect addButtonRect = GUILayoutUtility.GetLastRect();
                addButtonRect.x += addButtonRect.width - optionPanelWidth;
                addButtonRect.width = optionPanelWidth;

                if (GUI.Button(addButtonRect, string.Empty))
                {
                    GenericMenu gMenu = new GenericMenu();

                    foreach (Type t in CurrentDatabase.GetAssignableTypes())
                    {
                        if (CurrentDatabase.Settings.DefaultType == t)
                            continue;

                        Type temp = t;
                        gMenu.AddItem(new GUIContent(t.Name), false, () => AddItemOfType(temp));
                    }
                    gMenu.ShowAsContext();
                }

                if (GUI.Button(r, "Create new " + CurrentDatabase.Settings.DefaultType.Name, "DropDownButton"))
                {
                    AddItemOfType(CurrentDatabase.Settings.DefaultType);
                }
            }
            else
            {
                if (CurrentDatabase.Settings.DefaultType != null)
                {
                    if (GUILayout.Button("Create new " + CurrentDatabase.Settings.DefaultType.Name))
                    {
                        AddItemOfType(CurrentDatabase.Settings.DefaultType);
                    }
                }
                else
                {
                    if (GUILayout.Button("No default type set"))
                    {
                        EditorPrefs.SetBool("typeFoldout", true);
                        DatabaseSettingWindow.Initialize(ref _currentDatabase);
                    }
                }
            }

            if (GUI.changed)
                EditorUtility.SetDirty(_currentDatabase);

            GUI.backgroundColor = defaultColor;
        }

        private void AddItemOfType(Type type)
        {
            DatabaseWriter.AddNewAssetToDatabase(type, CurrentDatabase);

            _currentlySelected = CurrentDatabase.Assets.Last().Index;
            GUI.FocusControl(null);

            if (_currentDatabase.Settings.AutoUpdateEnum)
                DatabaseWriter.CreateDatabaseEnum(_currentDatabase);
        }

        private bool AssetNameIsUnique(DatabaseAsset asset)
        {
            string[] enumNames = CurrentDatabase.Assets.Select(AsEnumName).ToArray();
            return enumNames.Count(t => t == AsEnumName(asset)) == 1;
        }

        private bool AssetNameIsValid(DatabaseAsset asset)
        {
            return EnumGenerator.IsEnumValid(AsEnumName(asset));
        }

        private string AsEnumName(DatabaseAsset asset)
        {
            int zeros = CurrentDatabase.Assets.Max(t => t.Index).ToString().Length;
            return DatabaseWriter.GetEnumEntryString(CurrentDatabase.Settings, asset, zeros);
        }

        private static void UpdateAssignableTypes()
        {
            for (int i = 0; i < CurrentDatabase.Settings.TypeInfos.Count; i++)
            {
                StorableTypeInfo info = CurrentDatabase.Settings.TypeInfos[i];

                //Remove "dead" types //TODO Implement way to "fix" dead types by choose an other type (?)
                if (!info.TypeNameExists)
                {
                    //TODO This might change the default type if i == 0. Prevent(?)
                    CurrentDatabase.Settings.TypeInfos.RemoveAt(i);
                    i--;
                }
            }

            //Itterate all inheritad types from marked types (auto-including abstract types)
            Type[] allForceAddTypes = CurrentDatabase.Settings.TypeInfos.Where(t => t.IncludeInheritance).Select(t => t.StorableType).ToArray();
            foreach (Type type in allForceAddTypes)
            {
                //Itterate all subclass-types of processed types
                Type temp = type;
                foreach (Type subType in Database.GetStorableTypes().Where(t => t.IsSubclassOf(temp)))
                {
                    //Don't add a type twice
                    if (CurrentDatabase.Settings.ContainsType(subType))
                        continue;

                    //Add type
                    CurrentDatabase.Settings.TypeInfos.Add(new StorableTypeInfo(subType, true));
                }
            }
        }
    }
}
