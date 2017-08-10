using UnityEngine;

namespace Ziturion.BehaviourTree
{
    public class BT_Trade : BT_Primitive
    {
        public BT_Trade(string name) : base(name) { }

        public override BT_Callback CallbackState(BT_CallbackInfo info)
        {
            Culture tradePartner = CultureManager.Instance.RandomCulture(info.CultureInfo);

            //TODO DistanceMod
            if(tradePartner == null)
                return BT_Callback.False;

            if (tradePartner.Variables.Reputation / info.CultureInfo.Variables.Reputation <= 0.7f) //1 means identical rep... >1 tradepartner has less rep
                return BT_Callback.False;
            if(Random.Range(0f,1f) < 0.4f + tradePartner.GetParameterValue("Communication")) //if the trade partner has more communication == more chances
                return BT_Callback.False;
            if(info.CultureInfo.Variables.Production < 10)
                return BT_Callback.False;

            float productionUsed = info.CultureInfo.Variables.Production * Random.Range(0.3f,0.6f);

            if(productionUsed <= 0)
                return BT_Callback.False;

            info.CultureInfo.Variables.Reputation += Mathf.RoundToInt(productionUsed / 20);
            info.CultureInfo.Variables.Production -= Mathf.RoundToInt(productionUsed);

            tradePartner.Variables.Production -= tradePartner.Variables.Production * Random.Range(0.01f, 0.2f);
            tradePartner.Variables.Reputation++;

            //Debug.Log(string.Format("A Culture ({0}) traded with: {1} and lost {2} Production",info.CultureInfo.Name,tradePartner.Name,productionUsed));


            return BT_Callback.True;
        }
    }
}
