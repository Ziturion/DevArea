using System.Collections.Generic;

namespace Ziturion.BehaviourTree
{
    public abstract class BT_Composite : I_BT_Callback
    {
        protected List<I_BT_Callback> Childs = new List<I_BT_Callback>();
        public string Name { get; set; }

        protected BT_Composite(string name, params I_BT_Callback[] startElements)
        {
            Childs.AddRange(startElements);
            Name = name;
        }

        public abstract BT_Callback CallbackState(BT_CallbackInfo info);

        public void Add(I_BT_Callback element)
        {
            Childs.Add(element);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
