using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIngredient : MonoBehaviour
{
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}