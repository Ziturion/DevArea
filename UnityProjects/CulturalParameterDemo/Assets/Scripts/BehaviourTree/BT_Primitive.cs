namespace Ziturion.BehaviourTree
{
    public abstract class BT_Primitive : I_BT_Callback
    {
        public string Name { get; set; }

        protected BT_Primitive(string name)
        {
            Name = name;
        }

        public abstract BT_Callback CallbackState(BT_CallbackInfo info);

        public override string ToString()
        {
            return Name;
        }
    }
}
