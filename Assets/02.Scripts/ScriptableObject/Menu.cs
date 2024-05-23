using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Ingredient;

[CreateAssetMenu(fileName = "Menu Data", menuName = "Scriptable Object/Menu data", order = int.MaxValue)]

public class Menu : ScriptableObject
{
    // 레벨 스시 3, 부리또 4, 피자 5로 해뒀습니다
    public int Level;
    public string MenuName;
    public List<Ingredient.IngredientType> Ingredient;
    public float LimitTime;
    public Sprite MenuIcon;
    public List<Sprite> IngredientIcon;
    public int Price;
}
