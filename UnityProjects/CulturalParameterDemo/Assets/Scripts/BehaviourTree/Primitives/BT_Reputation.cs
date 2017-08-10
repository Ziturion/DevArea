using UnityEngine;

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
            if(info.CultureInfo.Name == "Red Culture")
                Debug.Log(Name);
            //TODO Logic
            return BT_Callback.False;
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
