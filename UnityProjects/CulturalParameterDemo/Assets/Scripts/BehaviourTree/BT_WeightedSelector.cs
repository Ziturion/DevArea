using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ziturion.BehaviourTree
{
    public class BT_WeightedSelector : BT_Selector
    {
        private readonly List<float> _weights = new List<float>();

        public BT_WeightedSelector(string name, params WeightedPair[] startElements) : base(name, startElements.Select(t => t.Element).ToArray())
        {
            _weights.AddRange(startElements.Select(t => t.Weight));
        }

        public void Add(I_BT_Callback element, float weight)
        {
            Add(element);
            _weights.Add(weight);
        }

        public override BT_Callback CallbackState(BT_CallbackInfo info)
        {
            List<WeightedPair> weightedList = Childs.Select((t, i) => new WeightedPair(_weights[i], t)).ToList();
            weightedList = weightedList.OrderByDescending(t => t.Weight).ToList();

            foreach (I_BT_Callback btCallback in weightedList.Select(t => t.Element))
            {
                if (btCallback.CallbackState(info) == BT_Callback.True)
                {
                    //if the callback did run => selector calls back
                    return BT_Callback.True;
                }
            }
            return BT_Callback.False;
        }

        public void ChangeWeight(int index, float value)
        {
            if (index >= _weights.Count || index < 0)
                return;

            _weights[index] = value;
        }

        public struct WeightedPair
        {
            public float Weight;
            public I_BT_Callback Element;

            public WeightedPair(float weight, I_BT_Callback element)
            {
                Weight = weight;
                Element = element;
            }
        }
    }
}
