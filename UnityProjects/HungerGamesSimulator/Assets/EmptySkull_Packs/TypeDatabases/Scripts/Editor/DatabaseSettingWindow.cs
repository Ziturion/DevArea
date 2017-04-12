using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmptySkull.Utilities;
using UnityEditor;
using UnityEngine;

namespace EmptySkull.TypeDatabases.Internal
{
    public class DatabaseSettingWindow : EditorWindow
    {
        private static DatabaseSettings _editSettings;
        private static Database _referenceDatabase;

        private static List<StorableTypeInfo> _selectedTypes = new List<StorableTypeInfo>();
        private static StorableTypeInfo _defaultType;

        private Vector2 _scrollPos;

        public static void Initialize(ref Database database)
        {
            _referenceDatabase = database;

            _editSettings = new DatabaseSettings(database.Settings);

            _selectedTypes = _referenceDatabase.Settings.TypeInfos;
            _defaultType = _selectedTypes.Count > 0 ? _selectedTypes[0] : null;
            _selectedTypes = _selectedTypes.OrderBy(t => t.TypeName).ToList();

            DatabaseSettingWindow window = (DatabaseSettingWindow) GetWindow(typeof (DatabaseSettingWindow));
            window.titleContent = new GUIContent("Settings");
            window.Show();
        }

        //Called by Unity
        private void OnGUI()
        {
            if (_referenceDatabase == null)
            {
                EditorGUILayout.LabelField("No database selected");
                return;
            }

            EditorGUIUtility.labelWidth = 75;
            EditorGUILayout.Space();

            string[] names = DatabaseWriter.GetAllDatabases().Where(t => t != _referenceDatabase).Select(t => t.Name).ToArray();
            bool nameIsNotUnique = names.Contains(_editSettings.Name);
            Color defaultColor = GUI.backgroundColor;
            if (nameIsNotUnique)
                GUI.backgroundColor = new Color(1f, 0.3f, 0.3f);
            _editSettings.Name = EditorGUILayout.TextField("Name", _editSettings.Name);

            GUI.backgroundColor = defaultColor;
            if (nameIsNotUnique)
                EditorGUILayout.HelpBox("This Name is not unique! This will cause errors when trying to access the database.", MessageType.Warning);
            _editSettings.SortingBehaviour =
                (DatabaseSortingBehaviour) EditorGUILayout.EnumPopup("Sorting", _editSettings.SortingBehaviour);

            EditorGUILayout.Space();

            bool typeFoldout = EditorPrefs.GetBool("typedatabases_typeFoldout");
            EditorPrefs.SetBool("typedatabases_typeFoldout", EditorGUILayout.Foldout(typeFoldout, "Type Settings"));
            if (typeFoldout)
            {
                Type[] possibleTypes = Database.GetStorableTypes();

                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, false, GUILayout.MaxHeight(200));
                {
                    EditorGUILayout.BeginVertical("Box");
                    if (_selectedTypes.Count <= 0)
                        EditorGUILayout.LabelField("No type selected.");

                    for (int i = 0; i < _selectedTypes.Count; i++)
                    {
                        StorableTypeInfo selectedType = _selectedTypes[i];
                        EditorGUILayout.BeginHorizontal();
                        {
                            bool typeEnforced = TypeIsEnforced(_selectedTypes[i].StorableType);
                            //Name
                            EditorGUILayout.LabelField(GetTypeLabel(selectedType.StorableType));

                            GUILayout.FlexibleSpace();

                            //Autoinclude Inhariatants
                            if (typeEnforced || _selectedTypes[i].StorableType.IsAbstract)
                                GUI.enabled = false;
                            EditorGUILayout.LabelField("Inh.", GUILayout.Width(30));
                            _selectedTypes[i].IncludeInheritance = EditorGUILayout.Toggle("",
                                _selectedTypes[i].IncludeInheritance, GUILayout.Width(15));
                            GUI.enabled = true;

                            //Default-Type-Select
                            if (_selectedTypes[i] == _defaultType)
                            {
                                EditorGUILayout.LabelField("Is default", GUILayout.Width(75));
                            }
                            else
                            {
                                GUI.enabled = !selectedType.StorableType.IsAbstract;
                                if (GUILayout.Button("Default", GUILayout.Width(75)))
                                {
                                    _defaultType = _selectedTypes[i];
                                }
                                GUI.enabled = true;
                            }

                            //Remove
                            if (typeEnforced)
                                GUI.enabled = false;
                            if (GUILayout.Button("X", GUILayout.Width(25)))
                            {
                                _selectedTypes.Remove(selectedType);
                                i--;

                                if (selectedType == _defaultType)
                                {
                                    _defaultType = _selectedTypes.Count > 0
                                        ? _selectedTypes[0]
                                        : null;
                                }
                                continue;
                            }
                            GUI.enabled = true;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginVertical("Box");
                    if (possibleTypes.Length <= 0)
                        EditorGUILayout.LabelField("No more possible type found.");

                    for (int i = 0; i < possibleTypes.Length; i++)
                    {
                        Type possibleType = possibleTypes[i];
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(GetTypeLabel(possibleType));
                            if (_selectedTypes.Select(t => t.StorableType).Contains(possibleType))
                                GUI.enabled = false;
                            if (GUILayout.Button("Add"))
                            {
                                _selectedTypes.Add(new StorableTypeInfo(possibleType));
                                _selectedTypes = _selectedTypes.OrderBy(t => t.TypeName).ToList();
                                if (_defaultType == null)
                                {
                                    _defaultType = _selectedTypes[0];
                                }
                            }
                            GUI.enabled = true;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.Space();
            }

            bool indexFoldout = EditorPrefs.GetBool("typedatabases_indexFoldout");
            EditorPrefs.SetBool("typedatabases_indexFoldout", EditorGUILayout.Foldout(indexFoldout, "Index Settings"));
            if (indexFoldout)
            {
                float lableWidth01 = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 200;
                _editSettings.IndexBehaviour =
                    (DatabaseIndexBehaviour) EditorGUILayout.EnumPopup("Prevent double-indices by",
                        _editSettings.IndexBehaviour);
                EditorGUIUtility.labelWidth = lableWidth01;

                if (DatabaseWriter.ContinuallyIndecies(_referenceDatabase))
                    GUI.enabled = false;

                if (GUILayout.Button("Clean up indecies"))
                {
                    DatabaseWriter.CleanDatabaseIndecies(_referenceDatabase);
                }
                GUI.enabled = true;

                EditorGUILayout.Space();
            }

            bool enumFoldout = EditorPrefs.GetBool("typedatabases_enumFoldout");
            EditorPrefs.SetBool("typedatabases_enumFoldout", EditorGUILayout.Foldout(enumFoldout, "Enum Settings"));
            if (enumFoldout)
            {
                float lableWidth02 = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 120;

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Enum-Name", GUILayout.Width(115));
                    _editSettings.EnumTypeNamePrefix = EditorGUILayout.TextField(_editSettings.EnumTypeNamePrefix);
                    GUI.enabled = false;
                    EditorGUILayout.TextField(_editSettings.Name, GUILayout.MinWidth(120));
                    GUI.enabled = true;
                    _editSettings.EnumTypeNameSuffix = EditorGUILayout.TextField(_editSettings.EnumTypeNameSuffix);
                }
                EditorGUILayout.EndHorizontal();

                _editSettings.EnumSettings =
                    (DatabaseEnumSettings) EditorGUILayout.EnumPopup("Enum Generation", _editSettings.EnumSettings);

                _editSettings.GenerateNoneEntry = EditorGUILayout.Toggle("None-Entry at Zero",
                    _editSettings.GenerateNoneEntry);

                if (_editSettings.EnumSettings == DatabaseEnumSettings.IndexesAndNames)
                {
                    _editSettings.IndexPrefix = EditorGUILayout.TextField("Index Prefix", _editSettings.IndexPrefix);
                    _editSettings.UseLeadingZeros = EditorGUILayout.Toggle("Use Leading Zeros",
                        _editSettings.UseLeadingZeros);
                    Rect temp = GUILayoutUtility.GetLastRect();
                    _editSettings.UseSeparator = EditorGUILayout.Toggle("Use Separator", _editSettings.UseSeparator);

                    temp.x += lableWidth02 + 50 + 20;
                    temp.height *= 2;
                    temp.width = position.width - (lableWidth02 + 50) - 20 - 8;
                    temp.height -= 4;
                    temp.y += 2;

                    EditorGUI.DrawRect(temp, Color.gray);

                    StringBuilder sB = new StringBuilder();
                    sB.Append(_editSettings.EnumTypeNamePrefix);
                    sB.Append(_editSettings.Name);
                    sB.Append(_editSettings.EnumTypeNameSuffix);
                    sB.Append(".");
                    sB.Append(_editSettings.IndexPrefix);
                    if (_editSettings.UseLeadingZeros)
                        sB.Append("00");
                    sB.Append("7");
                    if (_editSettings.UseSeparator)
                        sB.Append("_");
                    sB.Append("ExampleEntry");

                    int size;
                    if (sB.Length > 30)
                        size = 8;
                    else if (sB.Length > 20)
                        size = 10;
                    else if (sB.Length > 15)
                        size = 14;
                    else
                        size = 18;

                    EditorGUI.LabelField(temp, sB.ToString(),
                        new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontSize = size});
                }

                EditorGUIUtility.labelWidth = lableWidth02;

                bool enumGenerationImpossible = !EnumGenerationCurrentlyPossible();
                if (enumGenerationImpossible)
                {
                    MessageType mType = _editSettings.AutoUpdateEnum ? MessageType.Warning : MessageType.Info;
                    EditorGUILayout.HelpBox("The enum-generation is currently not possible! " +
                                            "Check the entry-names for invalid characters or duplicates.",
                        mType);
                }

                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    _editSettings.AutoUpdateEnum = EditorGUILayout.ToggleLeft("Auto", _editSettings.AutoUpdateEnum,
                        GUILayout.Width(45));

                    EditorGUILayout.LabelField(string.Empty, GUILayout.Height(18));
                    Rect r = GUILayoutUtility.GetLastRect();

                    if (!string.IsNullOrEmpty(_referenceDatabase.EnumPath))
                    {
                        int optionPanelWidth = 20;
                        Rect addButtonRect = GUILayoutUtility.GetLastRect();
                        addButtonRect.x += addButtonRect.width - optionPanelWidth;
                        addButtonRect.width = optionPanelWidth;

                        if (GUI.Button(addButtonRect, string.Empty))
                        {
                            GenericMenu gMenu = new GenericMenu();

                            gMenu.AddItem(new GUIContent("Remove enum"), false,
                                () => DatabaseWriter.RemoveDatabaseEnum(_referenceDatabase));
                            gMenu.ShowAsContext();
                        }
                    }

                    if (enumGenerationImpossible || _editSettings.AutoUpdateEnum)
                        GUI.enabled = false;

                    if (GUI.Button(r, "Update Enum",
                        !string.IsNullOrEmpty(_referenceDatabase.EnumPath) ? "DropDownButton" : "Button"))
                    {
                        DatabaseWriter.CreateDatabaseEnum(_referenceDatabase, _editSettings);
                    }
                    GUI.enabled = true;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }

            if (nameIsNotUnique)
                GUI.enabled = false;
            if (GUILayout.Button("Apply"))
            {
                _editSettings.TypeInfos = _selectedTypes;

                //Apply Settings to database
                if (_referenceDatabase.Name != _editSettings.Name)
                    DatabaseWriter.RenameDatabase(_editSettings.Name, _referenceDatabase);
                _referenceDatabase.Settings = _editSettings;

                //Set default-type to top in database-settings
                if (_defaultType != null && _referenceDatabase.Settings.ContainsType(_defaultType.StorableType))
                {
                    _referenceDatabase.Settings.TypeInfos.Remove(_defaultType);
                    _referenceDatabase.Settings.TypeInfos.Insert(0, _defaultType);
                }

                if (_referenceDatabase.Settings.AutoUpdateEnum)
                    DatabaseWriter.CreateDatabaseEnum(_referenceDatabase);

                DatabaseWindow.Initialize();
                GetWindow(typeof (DatabaseSettingWindow)).Close();
            }
            GUI.enabled = true;

            if (GUI.changed)
                EditorUtility.SetDirty(_referenceDatabase);
        }

        private bool EnumGenerationCurrentlyPossible()
        {
            int zeros = _referenceDatabase.Assets.Count <= 0
                ? 0
                : _referenceDatabase.Assets.Max(t => t.Index).ToString().Length;
            string[] enumNames =
                _referenceDatabase.Assets
                    .Select(t => DatabaseWriter.GetEnumEntryString(_editSettings, t, zeros)).ToArray();

            return enumNames.All(EnumGenerator.IsEnumValid) && enumNames.Length == enumNames.Distinct().Count();
        }

        private string GetTypeLabel(Type type)
        {
            return !type.IsAbstract ? type.Name : type.Name + " (a)";
        }

        private bool TypeIsEnforced(Type type)
        {
            return _selectedTypes.Where(t => t.IncludeInheritance)
                .Any(typeInfo => type.IsSubclassOf(typeInfo.StorableType));
        }
    }
}