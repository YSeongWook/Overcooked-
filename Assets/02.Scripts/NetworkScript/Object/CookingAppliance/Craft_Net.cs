using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Craft_Net : MonoBehaviour
{
    [SerializeField] private Animator CraftAnim;
    public enum FoodType { Fish, Shrimp, Lettuce, Cucumber, Tomato, Chicken, Potato }
    public FoodType food;
    public GameObject foodPrefabs;

    public void OpenCraftPlayer1()
    {
        OpenCraft(FindObjectOfType<PlayerInteractController_Net>());
    }

    public void OpenCraftPlayer2()
    {
        OpenCraft(FindObjectOfType<Player2InteractController_Net>());
    }

    public void OpenCraftPlayer3()
    {
        OpenCraft(FindObjectOfType<Player3InteractController_Net>());
    }

    public void OpenCraftPlayer4()
    {
        OpenCraft(FindObjectOfType<Player4InteractController_Net>());
    }

    private void OpenCraft(PlayerInteractController_Net player)
    {
        CraftAnim.SetTrigger("Open");
        GameObject newFood = Instantiate(foodPrefabs, Vector3.zero, Quaternion.identity);
        player.isHolding = true;
        newFood.transform.GetChild(0).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        newFood.transform.GetChild(0).transform.GetChild(0).GetComponent<Ingredient_Net>().HandleIngredient(player.transform, ConvertFoodTypeToHandleType(food), true);
    }
    private void OpenCraft(Player2InteractController_Net player)
    {
        CraftAnim.SetTrigger("Open");
        GameObject newFood = Instantiate(foodPrefabs, Vector3.zero, Quaternion.identity);
        player.isHolding = true;
        newFood.transform.GetChild(0).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        newFood.transform.GetChild(0).transform.GetChild(0).GetComponent<Ingredient_Net>().HandleIngredient(player.transform, ConvertFoodTypeToHandleType(food), true);
    }
    private void OpenCraft(Player3InteractController_Net player)
    {
        CraftAnim.SetTrigger("Open");
        GameObject newFood = Instantiate(foodPrefabs, Vector3.zero, Quaternion.identity);
        player.isHolding = true;
        newFood.transform.GetChild(0).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        newFood.transform.GetChild(0).transform.GetChild(0).GetComponent<Ingredient_Net>().HandleIngredient(player.transform, ConvertFoodTypeToHandleType(food), true);
    }
    private void OpenCraft(Player4InteractController_Net player)
    {
        CraftAnim.SetTrigger("Open");
        GameObject newFood = Instantiate(foodPrefabs, Vector3.zero, Quaternion.identity);
        player.isHolding = true;
        newFood.transform.GetChild(0).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        newFood.transform.GetChild(0).transform.GetChild(0).GetComponent<Ingredient_Net>().HandleIngredient(player.transform, ConvertFoodTypeToHandleType(food), true);
    }
    private Ingredient_Net.IngredientType ConvertFoodTypeToHandleType(FoodType food)
    {
        switch (food)
        {
            case FoodType.Fish:
                return Ingredient_Net.IngredientType.Fish;
            case FoodType.Shrimp:
                return Ingredient_Net.IngredientType.Shrimp;
            case FoodType.Lettuce:
                return Ingredient_Net.IngredientType.Lettuce;
            case FoodType.Cucumber:
                return Ingredient_Net.IngredientType.Cucumber;
            case FoodType.Tomato:
                return Ingredient_Net.IngredientType.Tomato;
            case FoodType.Chicken:
                return Ingredient_Net.IngredientType.Chicken;
            case FoodType.Potato:
                return Ingredient_Net.IngredientType.Potato;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(food), "Invalid food type");
        }
    }
}
