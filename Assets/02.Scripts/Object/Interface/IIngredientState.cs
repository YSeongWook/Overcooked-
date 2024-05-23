public interface IIngredientState
{
    void Processing(Ingredient ingredient, PlayerInteractController playerIC);
    void Cooked(Ingredient ingredient, PlayerInteractController playerIC);
    void Raw(Ingredient ingredient, PlayerInteractController playerIC);
}
