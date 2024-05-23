using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientUI : MonoBehaviour
{
    public Transform Target;
    [SerializeField] private new Vector3 pos = Vector3.zero;
    private void Update()
    {
        if (Target != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(Target.position + pos);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
