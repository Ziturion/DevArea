using System.Collections.Generic;

namespace Ziturion.BehaviourTree
{
    public class BehaviourTree
    {
        private readonly List<I_BT_Callback> _childs = new List<I_BT_Callback>();
        private readonly Culture _posessingCulture;

        public BehaviourTree(Culture posessingCulture)
        {
            _posessingCulture = posessingCulture;
        }

        public BT_Callback Run()
        {
            foreach (I_BT_Callback btCallback in _childs)
            {
                btCallback.CallbackState(new BT_CallbackInfo(_posessingCulture));
            }
            return BT_Callback.Running;
        }

        public I_BT_Callback GetChild(int index)
        {
            return _childs[index];
        }

        public void Add(I_BT_Callback element)
        {
            _childs.Add(element);
        }
    }
}
