using UnityEngine;

public class StoveTrigger : MonoBehaviour
{
    //public bool onSomething = false;

    public ObjectHighlight ObjectHighlight;

    private void OnTriggerStay(Collider other)
    {
        PotOnStove pot = other.GetComponent<PotOnStove>();
        if (pot != null && pot.inSomething && ObjectHighlight.onSomething)
        {
            pot.isOnStove = true;
            pot.StartCooking();
        }

        PanOnStove pan = other.GetComponent<PanOnStove>();
        if (pan != null && pan.inSomething && ObjectHighlight.onSomething)
        {
            pan.isOnStove = true;
            pan.StartCooking();
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

        PanOnStove pan = other.GetComponent<PanOnStove>();
        if (pan != null)
        {
            pan.isOnStove = false;
            pan.OffSlider();
        }
    }
}
