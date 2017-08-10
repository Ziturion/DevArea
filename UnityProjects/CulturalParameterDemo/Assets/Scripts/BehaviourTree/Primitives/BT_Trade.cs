using UnityEngine;

namespace Ziturion.BehaviourTree
{
    public class BT_Trade : BT_Primitive
    {
        public BT_Trade(string name) : base(name) { }

        public override BT_Callback CallbackState(BT_CallbackInfo info)
        {
            //if (info.CultureInfo.Name == "Red Culture")
            //    Debug.Log(Name);
            //TODO Logic
            return BT_Callback.False;
        }
    }
}
