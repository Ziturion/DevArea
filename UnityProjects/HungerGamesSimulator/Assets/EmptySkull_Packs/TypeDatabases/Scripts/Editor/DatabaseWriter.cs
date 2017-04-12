using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EmptySkull.Utilities;
using UnityEditor;
using UnityEngine;

namespace EmptySkull.TypeDatabases.Internal
{
    public static class DatabaseWriter
    {
        public static void CreateNewDatabase(DatabaseSettings settings)
        {
            Database database = ScriptableObject.CreateInstance<Database>();

            database.Settings = settings;

            string assetPath = DatabaseUtilities.GetDatabasePath(database.Name);

            //Handle existing databases
            AssetDatabase.DeleteAsset(assetPath);
            AssetDatabase.Refresh();

            AssetDatabase.CreateAsset(database, assetPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        public static void RenameDatabase(string newDatabaseName, Database database)
        {
            AssetDatabase.RenameAsset(DatabaseUtilities.GetDatabasePath(database.Name), newDatabaseName);

            database.Name = newDatabaseName;

            AssetDatabase.Refresh();
        }

        //public static void RenameDatabase(string newDatabaseName, int databaseIndex)
        //{
        //    RenameDatabase(newDatabaseName, GetDatabase(databaseIndex).Name);
        //}


        public static void RemoveDatabase(Database database)
        {
            string path = DatabaseUtilities.GetDatabasePath(database.Name);

            AssetDatabase.DeleteAsset(path);
            AssetDatabase.Refresh();
        }

        public static void AddNewAssetToDatabase(Type type, Database database)
        {
            if (!typeof(DatabaseAsset).IsAssignableFrom(type))
            {
                Debug.LogWarning("You cant assigne a not-databaseasset-type to a database");
                return;
            }

            DatabaseAsset asset = ScriptableObject.CreateInstance(type) as DatabaseAsset;

            //TODO if(asset == null) -> Type not allowed exception (?)

            asset.name = type.Name + "_Asset";
            asset.Name = GetItteratedValidString("NewEntry", database.Assets.Select(t => t.Name).ToArray(), 2);
            asset.Index = GetFirstUniqueIndex(database);

            AddAssetToDatabase(asset, database);
        }

        private static void AddAssetToDatabase(DatabaseAsset asset, Database database)
        {
            AssetDatabase.AddObjectToAsset(asset, database);
            EditorUtility.SetDirty(database);

            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(asset));

            database.Assets.Add(asset);
        }

        public static void RemoveAssetFromDatabase(int assetId, Database database)
        {
            DatabaseAsset asset = database.Assets[assetId];

            UnityEngine.Object.DestroyImmediate(asset, true);

            EditorUtility.SetDirty(database);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(database));

            database.Assets.RemoveAt(assetId);
        }

        public static void ChangeItemsIndex(Database database, int itemIndex, int setIndex)
        {
            DatabaseAsset item = database.Get(itemIndex);

            if (item == null)
            {
                //TODO Log an Error!
                return;
            }

            if (database.Assets.Any(t => t.Index == setIndex))
            {
                DatabaseAsset changeItem = database.Assets.First(t => t.Index == setIndex);

                switch (database.Settings.IndexBehaviour)
                {
                    case DatabaseIndexBehaviour.SwitchEntrys:
                        changeItem.Index = item.Index;
                        break;
                    case DatabaseIndexBehaviour.IncreseBelow:
                        ChangeItemsIndex(database, changeItem.Index, setIndex + 1);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            item.Index = setIndex;
        }

        public static void CleanDatabaseIndecies(Database database)
        {
            DatabaseAsset[] allItemSorted = database.Assets.OrderBy(t => t.Index).ToArray();
            int currentIndex = 1;
            foreach (DatabaseAsset item in allItemSorted)
            {
                item.Index = currentIndex;
                currentIndex++;
            }
        }


        public static void CreateDatabaseEnum(Database database, DatabaseSettings otherSettings = null)
        {
            string path = DatabaseUtilities.GetEnumPath(database.Name);

            if (!string.IsNullOrEmpty(database.EnumPath) && path != database.EnumPath)
                RemoveDatabaseEnum(database);

            string name;
            string[] entries;
            int[] indecies;

            PrepareEnumValues(database, out name, out entries, out indecies, otherSettings);

            EnumGenerator.WriteEnum(name, path, entries, indecies);

            database.EnumPath = path;
            database.EnumTypeName = name; //Connects the generated Enum with the Database
        }

        public static void RemoveDatabaseEnum(Database database)
        {
            if (File.Exists(database.EnumPath))
            {
                File.Delete(database.EnumPath);
                AssetDatabase.Refresh();
            }

            database.EnumPath = string.Empty;
        }

        private static void PrepareEnumValues(Database database, out string name, out string[] entries, out int[] indecies, DatabaseSettings otherSettings = null)
        {
            DatabaseAsset[] sortedAssets = database.Assets.OrderBy(t => t.Index).ToArray();
            DatabaseSettings settings = otherSettings == null ? database.Settings : otherSettings;
            StringBuilder nameBuilder = new StringBuilder();

            nameBuilder.Append(settings.EnumTypeNamePrefix);
            nameBuilder.Append(settings.Name);
            nameBuilder.Append(settings.EnumTypeNameSuffix);

            name = nameBuilder.ToString();

            List<string> entriesList = new List<string>();
            List<int> indeciesList = new List<int>();

            int maxIndexLenght = database.Assets.Max(t => t.Index).ToString().Length;

            if (settings.GenerateNoneEntry)
            {
                entriesList.Add(GetEnumEntryString(settings, "None", 0, maxIndexLenght));
                indeciesList.Add(0);
            }

            entriesList.AddRange(sortedAssets.Select(t => GetEnumEntryString(settings, t, maxIndexLenght)).ToArray());
            indeciesList.AddRange(sortedAssets.Select(t => t.Index).ToArray());

            entries = entriesList.ToArray();
            indecies = indeciesList.ToArray();
        }

        public static string GetEnumEntryString(DatabaseSettings settings, DatabaseAsset item, int maxIndexLenght = -1)
        {
            return GetEnumEntryString(settings, item.Name, item.Index, maxIndexLenght);
        }

        private static string GetEnumEntryString(DatabaseSettings settings, string entry, int index, int maxIndexLenght = -1)
        {
            string simplified = GetSimplifiedString(entry);

            StringBuilder entryBuilder = new StringBuilder();

            if (settings.EnumSettings == DatabaseEnumSettings.IndexesAndNames)
            {
                entryBuilder.Append(settings.IndexPrefix);
                if (settings.UseLeadingZeros)
                {
                    int zeroCount = maxIndexLenght - index.ToString().Length;
                    for (int i = 0; i < zeroCount; i++)
                    {
                        entryBuilder.Append("0");
                    }
                }
                entryBuilder.Append(index);
                if (settings.UseSeparator)
                    entryBuilder.Append("_");
            }
            entryBuilder.Append(simplified);

            return entryBuilder.ToString();
        }


        private static string GetSimplifiedString(string text)
        {
            return Path.GetInvalidFileNameChars() //TODO Find better collection of invalid chars
                .Aggregate(text, (current, invalidSymbol) => current.Replace(invalidSymbol.ToString(), string.Empty))
                .Replace(" ", string.Empty);
        }

        private static Database GetDatabase(string databaseName)
        {
            return GetAllDatabases().Single(t => t.Name == databaseName);
        }

        private static Database GetDatabase(int databaseIndex)
        {
            return GetAllDatabases()[databaseIndex];
        }

        public static Database[] GetAllDatabases()
        {
            DirectoryInfo df = new DirectoryInfo(DatabaseUtilities.DatabasesPath);
            string[] fileNames = df.GetFiles().Select(t => t.Name).ToArray();

            return fileNames
                .Select(name => AssetDatabase.LoadAssetAtPath<Database>(DatabaseUtilities.DatabasesPath + "/" + name))
                .Where(db => db != null).ToArray();
        }


        private static int GetFirstUniqueIndex(Database database)
        {
            int index = 0;
            int temp = index;
            while (index <= 0)
            {
                temp++;
                if (database.Assets.All(t => t.Index != temp))
                    index = temp;

                //TODO (if temp > 100000000) -> Break?
            }
            return index;
        }

        public static bool ContinuallyIndecies(Database database)
        {
            for (int i = 0; i < database.Assets.Count; i++)
            {
                if (database.Assets.All(t => t.Index != i + 1))
                    return false;
            }
            return true;
        }


        public static string GetItteratedValidString(string baseString, string[] otherStrings, int minIndexLenght = 0)
        {
            string simplyfied = GetSimplifiedString(baseString);
            string[] otherSimplyfied = otherStrings.Select(GetSimplifiedString).ToArray();

            if (!otherSimplyfied.Contains(simplyfied))
                return baseString;

            for (int i = 2; i < 10000; i++)
            {
                if (!otherSimplyfied.Contains(simplyfied + i.ToString("D" + minIndexLenght)))
                    return baseString + i.ToString("D" + minIndexLenght);
            }

            return "<StringGenerationFailed>";
        }
    }
}