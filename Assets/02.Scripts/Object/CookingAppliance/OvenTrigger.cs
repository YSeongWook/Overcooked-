using UnityEngine;

public class OvenTrigger : MonoBehaviour
{
    //public bool onSomething = false;

    public ObjectHighlight ObjectHighlight;

    private void OnTriggerEnter(Collider other)
    {
        PotOnStove pot = other.GetComponent<PotOnStove>();
        if (pot != null && pot.inSomething && ObjectHighlight.onSomething)
        {
            pot.isOnStove = true;
            pot.StartCooking();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PotOnStove pot = other.GetComponent<PotOnStove>();
        if (pot != null)
        {
            pot.isOnStove = false;
            pot.OffSlider();
        }
    }
}
