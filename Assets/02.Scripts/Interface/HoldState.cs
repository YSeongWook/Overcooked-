public class HoldState : IPlayerState
{
    private PlayerInteractController playerController;

    public HoldState(PlayerInteractController playerController)
    {
        this.playerController = playerController;
    }

    public void CatchOrKnockback()
    {
        playerController.CatchOrKnockback();
    }

    public void CookOrThrow()
    {
        playerController.CookOrThrow();
    }

    public void PickupOrPlace()
    {
        playerController.PickupOrPlace();
    }
}
