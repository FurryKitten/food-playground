using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeInfo", menuName = "ScriptableObjects/UpgradeInfo", order = 1)]
public class UpgradeInfoSO : ScriptableObject
{
    [SerializeField] private int[] _tier1Costs;
    [SerializeField] private int[] _tier2Costs;
    [SerializeField] private int[] _tier3Costs;

    public int[] Tier1Costs => _tier1Costs;
    public int[] Tier2Costs => _tier2Costs;
    public int[] Tier3Costs => _tier3Costs;
    //public int[][] TiersCosts = new int[][] 

    public int[][] getTierCosts()
    {
        return new int[][]
        {
            _tier1Costs, _tier2Costs, _tier3Costs
        };
    }

    
}
