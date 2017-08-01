using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CultureGenerator : Singleton<CultureGenerator>
{
    public CultureUI CultureUIPrefab;
    public Transform CanvasParent;

    public CultureParameter[] ParameterPool;
    public Vector2 ParameterCount;
    public CultureStats[] Cultures;

    public List<Culture> AllCultures { get; private set; }
    public float PointValue = 1.75f;

    public List<CultureUI> AllCultureUIs = new List<CultureUI>();

    protected void Awake()
    {
        AllCultures = new List<Culture>();
        TimeManager.OnStartSimulation += GenerateCultures;
    }

    private void GenerateCultures()
    {
        Debug.Log("Generating Cultures...");
        foreach (CultureStats cStats in Cultures)
        {
            AllCultures.Add(GenerateCulture(cStats, PointValue)); //TODO Generate per algorithm
        }
        Debug.Log("Generating Culture UI...");
        GenerateUIs();
        Debug.Log("Cultures Generated.");
    }

    private Culture GenerateCulture(string cultureName, Color cultureColor, Vector2 position, float pointValue)
    {
        List<CultureParameter> parameters = new List<CultureParameter>();

        for (int paramIndex = 0; paramIndex < Random.Range(ParameterCount.x, ParameterCount.y); paramIndex++)
        {
            //for this example the paramIndex selects the corresponding Parameter in the pool for simplicity
            //you could also make a random selection from the pool and delete duplicates
            //TODO set Value
            parameters.Add(ParameterPool[paramIndex]);
            parameters[paramIndex].Value = Random.Range(0f, 1f);
        }

        float sum = parameters.Sum(t => t.Value);
        float mod = pointValue / sum;

        foreach (CultureParameter parameter in parameters)
        {
            parameter.Value = parameter.Value * mod;
        }

        Culture newCulture = new Culture(cultureName, cultureColor, position, parameters.ToArray());

        return newCulture;
    }

    private Culture GenerateCulture(CultureStats cStats, float pointValue)
    {
        return GenerateCulture(cStats.CultureName, cStats.CultureColor, cStats.StartPosition, pointValue);
    }

    private void GenerateUIs()
    {
        //Hardcoded because the corners in a ui are limited
        GenerateUI(AllCultures[0], AnchorPosition.LeftDownAnchor());
        GenerateUI(AllCultures[1], AnchorPosition.RightUpAnchor());
        GenerateUI(AllCultures[2], AnchorPosition.RightDownAnchor());
    }

    private void GenerateUI(Culture culture, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
    {
        CultureUI newUi = Instantiate(CultureUIPrefab, CanvasParent);
        newUi.Initialize(culture.Name, culture.CultureColor, culture.Parameter);
        newUi.name = culture.Name + "_UI";
        RectTransform rect = newUi.GetComponent<RectTransform>();

        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;

        AllCultureUIs.Add(newUi);
    }

    private void GenerateUI(Culture culture, AnchorPosition position)
    {
        GenerateUI(culture, position.AnchorMin, position.AnchorMax, position.Pivot);
    }

    private struct AnchorPosition
    {
        public readonly Vector2 AnchorMin;
        public readonly Vector2 AnchorMax;
        public readonly Vector2 Pivot;

        private AnchorPosition(Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
        {
            AnchorMin = anchorMin;
            AnchorMax = anchorMax;
            Pivot = pivot;
        }

        public static AnchorPosition LeftDownAnchor()
        {
            return new AnchorPosition(new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0));
        }

        public static AnchorPosition RightDownAnchor()
        {
            return new AnchorPosition(new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0));
        }

        public static AnchorPosition RightUpAnchor()
        {
            return new AnchorPosition(new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1));
        }
    }

    [System.Serializable]
    public struct CultureStats
    {
        public string CultureName;
        public Color CultureColor;
        public Vector2 StartPosition;

        public CultureStats(string cultureName, Color cultureColor, Vector2 startPosition)
        {
            CultureName = cultureName;
            CultureColor = cultureColor;
            StartPosition = startPosition;
        }
    }
}
