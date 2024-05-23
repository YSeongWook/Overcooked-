using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Star Limit", menuName = "Scriptable Object/Star Limit", order = int.MaxValue)]

public class StarLimit : ScriptableObject
{
    public int Stage;
    public int oneStarLimit;
    public int twoStarLimit;
    public int threeStarLimit;
    public int successMoney;
}

