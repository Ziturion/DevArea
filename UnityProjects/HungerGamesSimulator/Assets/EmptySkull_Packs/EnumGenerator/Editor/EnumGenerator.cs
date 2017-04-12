using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EmptySkull.Utilities
{
    public static class EnumGenerator
    {
        private static readonly CodeDomProvider Prov = CodeDomProvider.CreateProvider("C#");

        public static void WriteEnum(string name, string path, string[] entries, int[] indecies = null)
        {
            if (!IsEnumValid(name) || entries.Any(t => !IsEnumValid(t)))
                throw new EnumGenerationException("The given name or some entries are not valid as an enum-type-name.");

            if (entries.Any(string.IsNullOrEmpty) || entries.Length != entries.Distinct().Count())
                throw new EnumGenerationException("One or more entries are empty or not unique.");

            bool writeIndecies = false;
            if (indecies != null)
            {
                if (indecies.Length != entries.Length)
                    throw new EnumGenerationException("When using custom indecies, " +
                                                  "make sure, the lenght of the indecie-array " +
                                                  "equals the lenght of the entries-array.");
                writeIndecies = true;
            }

            string genPath = Application.dataPath.Replace(@"/Assets", string.Empty) + @"/" + path;

            List<string> pathSplit = genPath.Split('/').ToList();
            pathSplit.RemoveAt(pathSplit.Count - 1);
            string directoryPath = string.Join("/", pathSplit.ToArray());
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            using (StreamWriter file = File.CreateText(genPath))
            {
                //TODO Add namespace!
                file.WriteLine("/// <summary>");
                file.WriteLine("/// This is a generated field! Changing or removing this code");
                file.WriteLine("/// might cause errors!");
                file.WriteLine("/// Any edits or changes will be loss on the next generation!");
                file.WriteLine("/// </summary>");
                file.WriteLine("public enum " + name);
                file.WriteLine("{");
                for (int i = 0; i < entries.Length; i++)
                {
                    string entry = entries[i];
                    StringBuilder entryString = new StringBuilder();

                    entryString.Append("\t" + entry);

                    if (writeIndecies)
                    {
                        int index = indecies[i];

                        entryString.Append(" = ");
                        entryString.Append(index);
                    }

                    if (i < entries.Length)
                        entryString.Append(",");
                    file.WriteLine(entryString.ToString());
                }
                file.WriteLine("}");
            }
            AssetDatabase.Refresh();
        }

        public static bool IsEnumValid(string text)
        {
            return Prov.IsValidIdentifier(text) && !string.IsNullOrEmpty(text);
        }

        [Serializable]
        public class EnumGenerationException : Exception
        {
            //
            // For guidelines regarding the creation of new exception types, see
            //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
            // and
            //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
            //

            public EnumGenerationException()
            {
            }

            public EnumGenerationException(string message) : base(message)
            {
            }

            public EnumGenerationException(string message, Exception inner) : base(message, inner)
            {
            }

            protected EnumGenerationException(
                SerializationInfo info,
                StreamingContext context) : base(info, context)
            {
            }
        }
    }
}