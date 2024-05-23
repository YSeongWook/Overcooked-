using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIngredient_Net : MonoBehaviour
{
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}