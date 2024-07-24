using System;
using UnityEngine;

public class IconManager_Net : Singleton<IconManager_Net>
{
    public Sprite[] icons; // 재료 아이콘 배열

    public Sprite GetIcon(Ingredient_Net.IngredientType type)
    {
        // 재료 타입에 맞는 아이콘을 반환합니다.
        return type switch
        {
            Ingredient_Net.IngredientType.Fish => icons[0],
            Ingredient_Net.IngredientType.Shrimp => icons[1],
            Ingredient_Net.IngredientType.Tomato => icons[2],
            Ingredient_Net.IngredientType.Lettuce => icons[3],
            Ingredient_Net.IngredientType.Cucumber => icons[4],
            Ingredient_Net.IngredientType.Potato => icons[5],
            Ingredient_Net.IngredientType.Chicken => icons[6],
            Ingredient_Net.IngredientType.SeaWeed => icons[7],
            Ingredient_Net.IngredientType.Tortilla => icons[8],
            Ingredient_Net.IngredientType.Rice => icons[9],
            Ingredient_Net.IngredientType.Pepperoni => icons[10],
            Ingredient_Net.IngredientType.Meat => icons[11],
            Ingredient_Net.IngredientType.Dough => icons[12],
            Ingredient_Net.IngredientType.Cheese => icons[13],
            Ingredient_Net.IngredientType.SushiRice => icons[9],
            Ingredient_Net.IngredientType.SushiFish => icons[0],
            Ingredient_Net.IngredientType.SushiCucumber => icons[4],
            Ingredient_Net.IngredientType.PizzaTomato => icons[2],
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}