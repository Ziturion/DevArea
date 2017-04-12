using UnityEditor;

namespace EmptySkull.TypeDatabases.Internal
{
    public static class DatabaseUtilities
    {
        public const string DefaultDatabasesPath = @"Assets/Resources/Databases";
        public const string DefaultEnumsPath = @"Assets/TypeDatabases/Scripts/GeneratedDatabaseEnums";

        public static string DatabasesPath
        {
            get
            {
                string temp = EditorPrefs.GetString("typedatabases_DatabasePath");
                if (string.IsNullOrEmpty(temp))
                {
                    EditorPrefs.SetString("typedatabases_DatabasePath", DefaultDatabasesPath);
                    return DefaultDatabasesPath;
                }
                return temp;
            }
            set { EditorPrefs.SetString("typedatabases_DatabasePath", value); }
        }
        public static string DatabaseBuildPath
        {
            get
            {
                return DatabasesPath
                    .Replace("Assets/", string.Empty)
                    .Replace("Resources/", string.Empty);
            }
        }

        public static string DatabaseEnumsPath
        {
            get
            {
                string temp = EditorPrefs.GetString("typedatabases_DatabaseEnumsPath");
                if (string.IsNullOrEmpty(temp))
                {
                    EditorPrefs.SetString("typedatabases_DatabaseEnumsPath", DefaultEnumsPath);
                    return DefaultEnumsPath;
                }
                return temp;
            }
            set { EditorPrefs.SetString("typedatabases_DatabaseEnumsPath", value); }
        }

        public static string GetDatabasePath(string databaseName)
        {
            return DatabasesPath + @"/" + databaseName + ".asset";
        }

        public static string GetResourcePath(string databaseName)
        {
            return DatabaseBuildPath + @"/" + databaseName;
        }

        public static string GetEnumPath(string databaseName)
        {
            return DatabaseEnumsPath + @"/" + databaseName.Replace(" ", string.Empty).Replace("-", string.Empty) + "_Enum.cs";
        }
    }
}