using UnityEngine;

public class Craft : MonoBehaviour
{
    [SerializeField] private Animator CraftAnim;
    public enum FoodType { Fish, Shrimp, Lettuce, Cucumber, Tomato, Chicken, Potato, Meat, Tortilla, Rice, SeaWeed, Pepperoni, Cheese, Dough, PizzaTomato, SushiRice, SushiFish, SushiCucumber }
    public FoodType food;
    public GameObject foodPrefabs;

    public void OpenCraftPlayer1()
    {
        OpenCraft(FindObjectOfType<PlayerInteractController>());
    }

    public void OpenCraftPlayer2()
    {
        OpenCraft(FindObjectOfType<Player2InteractController>());
    }

    private void OpenCraft(PlayerInteractController player)
    {
        CraftAnim.SetTrigger("Open");
        GameObject newFood = Instantiate(foodPrefabs, Vector3.zero, Quaternion.identity);
        player.isHolding = true;
        newFood.transform.GetChild(0).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        newFood.transform.GetChild(0).transform.GetChild(0).GetComponent<Ingredient>().HandleIngredient(player.transform, ConvertFoodTypeToHandleType(food), true);
    }
    private void OpenCraft(Player2InteractController player)
    {
        CraftAnim.SetTrigger("Open");
        GameObject newFood = Instantiate(foodPrefabs, Vector3.zero, Quaternion.identity);
        player.isHolding = true;
        newFood.transform.GetChild(0).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        newFood.transform.GetChild(0).transform.GetChild(0).GetComponent<Ingredient>().HandleIngredient(player.transform, ConvertFoodTypeToHandleType(food), true);
    }
    private Ingredient.IngredientType ConvertFoodTypeToHandleType(FoodType food)
    {
        switch (food)
        {
            case FoodType.Fish:
                return Ingredient.IngredientType.Fish;
            case FoodType.Shrimp:
                return Ingredient.IngredientType.Shrimp;
            case FoodType.Lettuce:
                return Ingredient.IngredientType.Lettuce;
            case FoodType.Cucumber:
                return Ingredient.IngredientType.Cucumber;
            case FoodType.Tomato:
                return Ingredient.IngredientType.Tomato;
            case FoodType.Chicken:
                return Ingredient.IngredientType.Chicken;
            case FoodType.Potato:
                return Ingredient.IngredientType.Potato;
            case FoodType.Tortilla:
                return Ingredient.IngredientType.Tortilla;
            case FoodType.SeaWeed:
                return Ingredient.IngredientType.SeaWeed;
            case FoodType.Rice:
                return Ingredient.IngredientType.Rice;
            case FoodType.Pepperoni:
                return Ingredient.IngredientType.Pepperoni;
            case FoodType.Cheese:
                return Ingredient.IngredientType.Cheese;
            case FoodType.Dough:
                return Ingredient.IngredientType.Dough;
            case FoodType.Meat:
                return Ingredient.IngredientType.Meat;
            case FoodType.PizzaTomato:
                return Ingredient.IngredientType.PizzaTomato;
            case FoodType.SushiRice:
                return Ingredient.IngredientType.SushiRice;
            case FoodType.SushiFish:
                return Ingredient.IngredientType.SushiFish;
            case FoodType.SushiCucumber:
                return Ingredient.IngredientType.SushiCucumber;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(food), "Invalid food type");
        }
    }
}
