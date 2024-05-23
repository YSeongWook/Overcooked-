using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Star Limit Net", menuName = "Scriptable Object Net/Star Limit Net", order = int.MaxValue)]

public class StarLimit_Net : ScriptableObject
{
    public int Stage;
    public int oneStarLimit;
    public int twoStarLimit;
    public int threeStarLimit;
    public int successMoney;
}

