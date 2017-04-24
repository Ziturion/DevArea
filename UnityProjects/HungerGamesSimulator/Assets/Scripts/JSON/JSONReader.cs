using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using SimpleJSON;
using Random = UnityEngine.Random;

public class JSONReader
{

    private static TextAsset _textAsset;
    private static JSONNode _jsonNodeData;

    public static void LoadAsset(string assetName)
    {
        _textAsset = Resources.Load<TextAsset>(assetName);

        if (_textAsset == null)
            throw new FileLoadException(assetName + " not loaded. Please check if " + assetName +
                                        ".json exists in Asset/Resources.");

        _jsonNodeData = JSON.Parse(_textAsset.text);

        if (_jsonNodeData == null)
            throw new FileLoadException(assetName + " could not be parsed. Please check if " + assetName +
                                        ".json exists in Asset/Resources.");
    }

    public static string GetRawText()
    {
        return _jsonNodeData.Value;
    }

    public static string GetDeathText(PlayerController.Cause cause)
    {
        LoadAsset("deaths");
        List<string> possibleTexts = new List<string>();
        string result = _jsonNodeData[0].Value;
        if (result == null)
            throw new NotImplementedException();

        string selectedPage = "";
        if (cause.Instigator is Trap)
            selectedPage = "Trap";

        foreach (JSONNode node in _jsonNodeData[selectedPage].AsArray)
        {
            if (node["index"].AsInt == cause.Instigator.Index - 1)
            {
                possibleTexts.Add(node["text"]);
            }
        }

        return possibleTexts[Random.Range(0,possibleTexts.Count)];
    }
}
