namespace Ziturion.BehaviourTree
{
    public interface I_BT_Callback
    {
        string Name { get; set; }

        BT_Callback CallbackState(BT_CallbackInfo info);
    }

    public enum BT_Callback
    {
        True,
        False,
        Running
    }

    public struct BT_CallbackInfo
    {
        public Culture CultureInfo;

        public BT_CallbackInfo(Culture cultureInfo)
        {
            CultureInfo = cultureInfo;
        }
    }
}
