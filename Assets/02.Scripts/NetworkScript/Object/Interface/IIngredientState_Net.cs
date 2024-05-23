public interface IIngredientState_Net
{
    void Processing(Ingredient ingredient, PlayerInteractController playerIC);
    void Cooked(Ingredient ingredient, PlayerInteractController playerIC);
    void Raw(Ingredient ingredient, PlayerInteractController playerIC);
}
