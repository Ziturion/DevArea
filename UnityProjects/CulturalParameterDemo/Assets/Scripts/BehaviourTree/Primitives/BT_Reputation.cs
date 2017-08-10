using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Ziturion.BehaviourTree
{
    /// <summary>
    /// This Class needs a CultureParameterType and then gets the behaviour for Reputation-Needs
    /// </summary>
    public class BT_Reputation : BT_Primitive
    {
        private CultureType _cultureType;
        public BT_Reputation(string name, CultureType cultureType) : base(name)
        {
            _cultureType = cultureType;
        }

        public override BT_Callback CallbackState(BT_CallbackInfo info)
        {
            //if(info.CultureInfo.Name == "Red Culture")
            //    Debug.Log(Name);
            //TODO Logic
            return SelectReputation(info.CultureInfo) ? BT_Callback.True : BT_Callback.False;
        }

        private bool SelectReputation(Culture culture)
        {
            if (Random.Range(0f, 1f) < 0.5f)
                return false; //TODO maybe Redo cause unnecessary Randomness
            culture.Variables.Reputation++;
            switch (_cultureType)
            {
                case CultureType.Behaviour:
                    throw new NotSupportedException();
                case CultureType.Communication:
                    culture.Variables.InteractionRate += Random.Range(0f,1f);
                    break;
                case CultureType.Economics:
                    culture.Variables.Production += Random.Range(10, 25);
                    break;
                case CultureType.Sociology:
                    culture.Variables.PopulationSize += Mathf.RoundToInt(Random.Range(culture.Variables.PopulationSize * 0.01f, culture.Variables.PopulationSize * 0.09f));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return true;
        }
    }



    public enum CultureType
    {
        Behaviour,
        Communication,
        Economics,
        Sociology
    }
}
