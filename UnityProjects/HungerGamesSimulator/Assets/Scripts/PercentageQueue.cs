using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;

public class PercentageQueue<T>
{
    public int Count { get { return _percentObjs.Count; } }

    private readonly List<PercentObj> _percentObjs;

    public PercentageQueue()
    {
        _percentObjs = new List<PercentObj>();
    }

    public void Add(T obj, float percentage)
    {
        float pc = Math.Max(Math.Min(percentage, 1), 0);
        _percentObjs.Add(new PercentObj(obj, pc));
    }

    public void Add(T obj, Func<float> percentageFunc)
    {
        Add(obj,percentageFunc.Invoke());
    }

    public void Remove(T obj)
    {
        int removeIndex = _percentObjs.FindIndex(t => t.Object.Equals(obj));
        _percentObjs.RemoveAt(removeIndex);
    }

    public void Clear()
    {
        _percentObjs.Clear();
    }

    public bool Contains(T obj)
    {
        return _percentObjs.Any(t => t.Object.Equals(obj));
    }

    public int IndexOf(T obj)
    {
        return _percentObjs.FindIndex(t => t.Object.Equals(obj));
    }

    public void Insert(int index, T obj, float percentage)
    {
        float pc = Math.Max(Math.Min(percentage, 1), 0);
        _percentObjs.Insert(index, new PercentObj(obj, pc));
    }

    public void RemoveAt(int index)
    {
        _percentObjs.RemoveAt(index);
    }

    public T Get()
    {
        PercentObj[] sortedItems = _percentObjs.OrderBy(t => t.Percentage).ToArray();
        for (int i = 0; i < sortedItems.Length; i++)
        {
            if (sortedItems[i].Percentage > Random.Range(0f, 1f))
            {
                return sortedItems[i].Object;
            }
        }
        return sortedItems.First().Object; //return first object Fallback
    }

    public void Change(T obj, float newPercentage)
    {
        int removeIndex = _percentObjs.FindIndex(t => t.Object.Equals(obj));
        if (removeIndex == -1)
            throw new IndexOutOfRangeException(string.Format("The object ({0}) could not be found.", obj));

        float pc = Math.Max(Math.Min(newPercentage, 1), 0);
        _percentObjs[removeIndex] = new PercentObj(obj, pc);
    }

    public void Change(T obj, Func<float> newPercentageFunc)
    {
        Change(obj,newPercentageFunc.Invoke());
    }

    public override string ToString()
    {
        StringBuilder name = new StringBuilder("Type : " + typeof(T));
        foreach (PercentObj pObj in _percentObjs)
        {
            name.Append(Environment.NewLine);
            name.Append(pObj);
        }
        return name.ToString();
    }

    private struct PercentObj
    {
        public T Object;
        public float Percentage;

        public PercentObj(T o, float percentage)
        {
            Object = o;
            Percentage = percentage;
        }

        public override string ToString()
        {
            StringBuilder name = new StringBuilder("[");
            name.Append(Percentage);
            name.Append(", ");
            name.Append(Object);
            name.Append("]");
            return name.ToString();
        }
    }
}
