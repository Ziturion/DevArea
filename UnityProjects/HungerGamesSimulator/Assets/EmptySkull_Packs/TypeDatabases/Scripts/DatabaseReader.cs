using EmptySkull.TypeDatabases.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EmptySkull.TypeDatabases
{
    /// <summary>
    /// The database-reader is used to accsess the database-items.
    /// </summary>
    public static class DatabaseReader
    {
        /// <summary>
        /// True when the database-collection is loaded.
        /// </summary>
        public static bool IsLoaded { get { return _allDatabases != null; } }

        private static Database[] _allDatabases;
        private static Database[] AllDatabases
        {
            get
            {
                if (!IsLoaded)
                    Load();
                return _allDatabases;
            }
        }

        private static Dictionary<string, Database> _databasesByNames;
        private static Dictionary<string, Database> DatabasesByNames
        {
            get
            {
                if (_databasesByNames != null)
                    return _databasesByNames;
                LoadDictionarys();
                return _databasesByNames;
            }
        }

        private static Dictionary<Type, Database> _databasesByEnumTypes;
        private static Dictionary<Type, Database> DatabasesByEnumTypes
        {
            get
            {
                if (_databasesByEnumTypes != null)
                    return _databasesByEnumTypes;
                LoadDictionarys();
                return _databasesByEnumTypes;
            }
        }

        /// <summary>
        /// Returns an array of all database-names.
        /// </summary>
        public static string[] DatabaseNames
        {
            get { return AllDatabases.Select(t => t.Name).ToArray(); }
        }

        /// <summary>
        /// Enforces loading the databases. This is not nesscessary since the databases are lazy initialized,
        /// but it gives more controll and should be called manually for large databases.
        /// </summary>
        public static void Load()
        {
            _allDatabases = Resources.FindObjectsOfTypeAll<Database>();
            LoadDictionarys();
        }

        /// <summary>
        /// Access a database by its name and returns the item with the given index.
        /// </summary>
        /// <param name="databaseName">
        /// The name-string of the database (the string must exactly equal the databases name)
        /// </param>
        /// <param name="itemIndex">
        /// The index of the searched item
        /// </param>
        /// <returns>
        /// The searched item as object-type. Will be 'null' if the string or index does not exist.
        ///  </returns>
        public static object GetItem(string databaseName, int itemIndex)
        {
            if (!DatabasesByNames.ContainsKey(databaseName))
            {
                Debug.LogError("Database-Name " + databaseName + " does not exist!");
                return null;
            }
            return GetItem(DatabasesByNames[databaseName], itemIndex);
        }
        /// <summary>
        /// Access a database by its name and returns the item with the given index.
        /// </summary>
        /// <param name="databaseName">
        /// The name-string of the database (the string must exactly equal the databases name)
        /// </param>
        /// <param name="itemIndex">
        /// The index of the searched item
        /// </param>
        /// <returns>
        /// The searched item as the given type. Will be 'null' if the string or index does not exist
        /// or if the item is not of the given type.
        ///  </returns>
        public static T GetItem<T>(string databaseName, int itemIndex) where T : DatabaseAsset
        {
            return GetItem(databaseName, itemIndex) as T;
        }

        /// <summary>
        /// Access a database by its name and returns the item with the given index.
        /// </summary>
        /// <param name="databaseName">
        /// The name-string of the database (the string must exactly equal the databases name)
        /// </param>
        /// <param name="itemName">
        /// The name of the searched item.
        /// </param>
        /// <returns>
        /// The searched item as object-type. Will be 'null' if the string or item-name does not exist.
        /// It will also return null if the given item-name is not unique in the database!
        ///  </returns>
        public static object GetItem(string databaseName, string itemName)
        {
            if (!DatabasesByNames.ContainsKey(databaseName))
            {
                Debug.LogError("Database-Name " + databaseName + " does not exist!");
                return null;
            }
            return GetItem(DatabasesByNames[databaseName], itemName);
        }
        /// <summary>
        /// Access a database by its name and returns the item with the given index.
        /// </summary>
        /// <param name="databaseName">
        /// The name-string of the database (the string must exactly equal the databases name)
        /// </param>
        /// <param name="itemName">
        /// The name of the searched item
        /// </param>
        /// <returns>
        /// The searched item as the given type. Will be 'null' if the string or item-name does not exist
        /// or if the item is not of the given type.
        /// It will also return null if the given item-name is not unique in the database!
        ///  </returns>
        public static T GetItem<T>(string databaseName, string itemName) where T : DatabaseAsset
        {
            return GetItem(databaseName, itemName) as T;
        }

        /// <summary>
        /// Access a database by an database-enum (automaticaly generated by the type-database-system).
        /// </summary>
        /// <param name="databaseEnum">
        /// The (automaticaly generaded) database-enum
        /// </param>
        /// <returns>
        /// The searched item as object-type. Will be 'null' if the used enum is not a database-enum.
        ///  </returns>
        public static object GetItem(Enum databaseEnum)
        {
            if (!DatabasesByEnumTypes.ContainsKey(databaseEnum.GetType()))
            {
                Debug.LogWarning("Given Type is not a connected database-enum-type");
                return null;
            }

            return GetItem(DatabasesByEnumTypes[databaseEnum.GetType()], Convert.ToInt32(databaseEnum));
        }
        /// <summary>
        /// Access a database by an database-enum (automaticaly generated by the type-database-system).
        /// </summary>
        /// <param name="databaseEnum">
        /// The (automaticaly generaded) database-enum
        /// </param>
        /// <returns>
        /// The searched item as the given type. Will be 'null' if the used enum is not a database-enum
        /// or if the item is not of the given type.
        ///  </returns>
        public static T GetItem<T>(Enum databaseEnum) where T : DatabaseAsset
        {
            return GetItem(databaseEnum) as T;
        }

        /// <summary>
        /// Retruns all items of a database.
        /// </summary>
        /// <param name="databaseName">
        /// The name-string of the database (the string must exactly equal the databases name)
        /// </param>
        /// <returns>
        /// All items as an array of objects.
        /// </returns>
        public static object[] GetAllItems(string databaseName)
        {
            if (!DatabasesByNames.ContainsKey(databaseName))
            {
                Debug.LogError("Database-Name " + databaseName + " does not exist!");
                return null;
            }
            object[] result = GetAllItems(DatabasesByNames[databaseName]);
            return result;
        }
        /// <summary>
        /// Retruns multiple items of a database, that can be filtered by a predicate.
        /// </summary>
        /// <typeparam name="T">
        /// The item-type (inherits from database-asset-type)
        /// </typeparam>
        /// <param name="databaseName">
        /// The name-string of the database (the string must exactly equal the databases name)
        /// </param>
        /// <param name="filterPredicate">
        /// The filter-predicate to select or exclude items.
        /// </param>
        /// <param name="includeOtherTypesAsNull">
        /// Usually items of an not-matching type are automaticaly excluded. To include them as 'null'-references,
        /// this must be set to true.
        /// </param>
        /// <returns>
        /// All filtered items as an array of the given type.
        /// </returns>
        public static T[] GetAllItems<T>(string databaseName, Func<T, bool> filterPredicate = null, bool includeOtherTypesAsNull = false) where T : DatabaseAsset
        {
            object[] allItems = GetAllItems(databaseName);
            if (allItems == null)
                return null;
            T[] result = allItems.Select(t => t as T).Where(t => t != null || includeOtherTypesAsNull).ToArray();
            if (filterPredicate != null)
                result = result.Where(filterPredicate).ToArray();
            return result;
        }


        private static object GetItem(Database database, int itemIndex)
        {
            return database.Get(itemIndex);
        }
        private static object GetItem(Database database, string itemName)
        {
            return database.Assets.SingleOrDefault(t => t.Name == itemName);
        }

        private static object[] GetAllItems(Database database)
        {
            return database.Assets.Select(t => (object)t).ToArray();
        }

        private static void LoadDictionarys()
        {
            _databasesByNames = new Dictionary<string, Database>();
            _databasesByEnumTypes = new Dictionary<Type, Database>();

            foreach (Database database in AllDatabases)
            {
                _databasesByNames.Add(database.Name, database);

                if (database.EnumType != null)
                    _databasesByEnumTypes.Add(database.EnumType, database);
            }
        }
    }
}