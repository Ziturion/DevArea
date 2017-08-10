using UnityEngine;
using Ziturion.BehaviourTree;

public class BTDebugger : MonoBehaviour
{
    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            BehaviourTree bt = new BehaviourTree(CultureManager.Instance.Cultures[0]);
            BT_Selector selector = new BT_WeightedSelector("Start Selector", 
                new BT_WeightedSelector.WeightedPair(
                    CultureManager.Instance.Cultures[0].GetParameterValue("Behaviour"), 
                    new BT_Selector("Behaviour", new BT_Kill("Kill"), new BT_Claim("Claim: Behaviour",CultureType.Behaviour))),
                new BT_WeightedSelector.WeightedPair(
                    CultureManager.Instance.Cultures[0].GetParameterValue("Communication"), 
                    new BT_Selector("Communication", new BT_Trade("Trade"), new BT_Reputation("Reputation: Communication",CultureType.Communication), new BT_Claim("Claim: Communication",CultureType.Communication))),
                new BT_WeightedSelector.WeightedPair(
                    CultureManager.Instance.Cultures[0].GetParameterValue("Economics"), 
                    new BT_Selector("Economics", new BT_Claim("Claim: Economics",CultureType.Economics), new BT_Reputation("Reputation: Economics",CultureType.Economics))),
                new BT_WeightedSelector.WeightedPair(
                    CultureManager.Instance.Cultures[0].GetParameterValue("Sociology"), 
                    new BT_Selector("Sociology", new BT_Reputation("Reputation: Sociology",CultureType.Sociology))));
            bt.Add(selector);
            Debug.Log("X");
            bt.Run();
        }
    }
}
