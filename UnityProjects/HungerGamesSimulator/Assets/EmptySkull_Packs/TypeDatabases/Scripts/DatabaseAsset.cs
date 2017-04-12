using UnityEngine;

namespace EmptySkull.TypeDatabases
{
    /// <summary>
    /// The database-asset is the abstract basic class for all database-objects.
    /// It is stored and managed as a scriptable object inside the unity-assets.
    /// </summary>
    [System.Serializable]
    public abstract class DatabaseAsset : ScriptableObject
    {
        /// <summary>
        /// The default name when the name is not set.
        /// </summary>
        const string DefaultName = "NameMissing";

        [SerializeField]
        private string _name;

        /// <summary>
        /// The name of the database-asset.
        /// </summary>
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                    return DefaultName;
                return _name;
            }
            set { _name = value; }
        }

        /// <summary>
        /// The unique Index of the database-asset. This always equals the value of the enum-entry, when the
        /// automatically generated enums are used.
        /// </summary>
        public int Index;
    }
}

