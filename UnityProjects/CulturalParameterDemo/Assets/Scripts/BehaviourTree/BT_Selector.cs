namespace Ziturion.BehaviourTree
{
    public class BT_Selector : BT_Composite
    {
        public BT_Selector(string name, params I_BT_Callback[] startElements) : base(name,startElements) { }

        public override BT_Callback CallbackState(BT_CallbackInfo info)
        {
            foreach (I_BT_Callback btCallback in Childs)
            {
                if (btCallback.CallbackState(info) == BT_Callback.True)
                {
                    //if the callback did run => selector calls back
                    return BT_Callback.True;
                }
            }
            return BT_Callback.False;
        }
    }
}
