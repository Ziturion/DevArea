using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EmptySkull.TypeDatabases.Internal
{
    public class Database : ScriptableObject
    {
        public List<DatabaseAsset> Assets = new List<DatabaseAsset>();

        public string Name
        {
            get { return Settings.Name; }
            set { Settings.Name = value; }
        }

        public DatabaseSettings Settings;

        public string EnumPath { get; set; }
        public string EnumTypeName { set; private get; }

        private Type _enumType;
        public Type EnumType
        {
            get
            {
                if (_enumType != null)
                    return _enumType;
                _enumType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(t => t.GetTypes())
                    .Where(t => t.IsEnum)
                    .SingleOrDefault(t => t.Name == EnumTypeName);
                return _enumType;
            }
        }

        public DatabaseAsset Get(int index)
        {
            if (Assets.Any(t => t.Index == index))
                return Assets.Single(t => t.Index == index);
            return null;
        }

        public Type[] GetAssignableTypes()
        {
            return Settings.TypeInfos.Select(t => t.StorableType).Where(t => t != null && !t.IsAbstract).ToArray();
        }

        public static Type[] GetStorableTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .Where(t => t.IsClass && t.IsSubclassOf(typeof(DatabaseAsset)))
                .ToArray();
        }
    }

    [Serializable]
    public class DatabaseSettings
    {
        public string Name;
        [SerializeField]
        public List<StorableTypeInfo> TypeInfos = new List<StorableTypeInfo>();
        public Type DefaultType
        {
            get
            {
                return HasTypes
                    ? TypeInfos[0].StorableType
                    : null;
            }
        }
        public bool HasMultipleChoices
        {
            get { return TypeInfos.Count > 1; }
        }
        public bool HasTypes
        {
            get { return TypeInfos.Count > 0; }
        }

        public DatabaseSortingBehaviour SortingBehaviour = DatabaseSortingBehaviour.OrderByIndex;
        public DatabaseIndexBehaviour IndexBehaviour = DatabaseIndexBehaviour.SwitchEntrys;

        public string EnumTypeNamePrefix = "";
        public string EnumTypeNameSuffix = "_Enum";

        public bool AutoUpdateEnum;
        public DatabaseEnumSettings EnumSettings;

        public bool GenerateNoneEntry;
        public string IndexPrefix = "Id";

        public bool UseLeadingZeros;
        public bool UseSeparator = true;

        public DatabaseSettings(string name, Type[] usedTypes = null, bool autoupdateEnum = false, DatabaseEnumSettings enumSettings = DatabaseEnumSettings.NamesOnly)
        {
            Name = name;
            if (usedTypes == null)
                return;
            TypeInfos = new List<StorableTypeInfo>();
            foreach (Type type in usedTypes)
            {
                TypeInfos.Add(new StorableTypeInfo(type));
            }

            AutoUpdateEnum = autoupdateEnum;
            EnumSettings = enumSettings;
        }

        public DatabaseSettings(DatabaseSettings other)
        {
            Name = other.Name;
            if (other.TypeInfos != null)
            {
                TypeInfos = new List<StorableTypeInfo>();
                for (int i = 0; i < other.TypeInfos.Count; i++)
                {
                    TypeInfos.Add(other.TypeInfos[i]);
                }
            }
            //TypeInfos = other.TypeInfos; //<- Test this?

            SortingBehaviour = other.SortingBehaviour;
            IndexBehaviour = other.IndexBehaviour;

            EnumTypeNamePrefix = other.EnumTypeNamePrefix;
            EnumTypeNameSuffix = other.EnumTypeNameSuffix;

            AutoUpdateEnum = other.AutoUpdateEnum;
            EnumSettings = other.EnumSettings;

            GenerateNoneEntry = other.GenerateNoneEntry;
            IndexPrefix = other.IndexPrefix;

            UseLeadingZeros = other.UseLeadingZeros;
            UseSeparator = other.UseSeparator;
        }

        public bool ContainsType(Type type)
        {
            return TypeInfos.Select(t => t.StorableType).Contains(type);
        }
    }

    [Serializable]
    public class StorableTypeInfo
    {
        public string TypeName;

        private bool _includeSubtypes;
        public bool IncludeInheritance
        {
            get { return _includeSubtypes || StorableType.IsAbstract; }
            set { _includeSubtypes = value; }
        }

        private Type _storableType;
        public Type StorableType
        {
            get
            {
                if (_storableType != null)
                    return _storableType;
                Type[] allStorableTypes = Database.GetStorableTypes();
                _storableType = allStorableTypes.SingleOrDefault(t => t.Name == TypeName);
                return _storableType;
            }
        }

        public bool TypeNameExists
        {
            get { return Database.GetStorableTypes().Count(t => t.Name == TypeName) == 1; }
        }

        public StorableTypeInfo(Type type, bool includeInheritance = false)
        {
            if (!Database.GetStorableTypes().Contains(type))
            {
                Debug.LogWarning("The assigned Type does not implements IDatabaseStorable");
                return;
            }
            TypeName = type.Name;
            IncludeInheritance = includeInheritance;
        }
    }

    public enum DatabaseSortingBehaviour
    {
        NoSorting, OrderByIndex, OrderByName
    }

    public enum DatabaseIndexBehaviour
    {
        SwitchEntrys, IncreseBelow
    }

    public enum DatabaseEnumSettings
    {
        NamesOnly,
        IndexesAndNames
    }
}