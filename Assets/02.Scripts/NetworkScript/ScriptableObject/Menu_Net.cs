using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Ingredient;

[CreateAssetMenu(fileName = "Menu Data Net", menuName = "Scriptable Object Net/Menu data Net", order = int.MaxValue)]

public class Menu_Net : ScriptableObject
{
    public int Level;
    public string MenuName;
    public List<Ingredient_Net.IngredientType> Ingredient;
    public float LimitTime;
    public Sprite MenuIcon;
    public List<Sprite> IngredientIcon;
    public int Price;
}
