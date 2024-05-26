using System.Collections;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [Header("Team Positions")]
    [SerializeField] private TeamReturnPositions redTeamPositions;
    [SerializeField] private TeamReturnPositions blueTeamPositions;

    [Space(10)]
    [SerializeField] private RespawnManager respawnManager;

    public GameObject Plate;
    public GameObject Pan;
    public GameObject Pot;
    public GameObject ingredientUI;
    public Sprite[] icons;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(DeactivateAndRespawnPlayer(other.gameObject));
        }
        else if (other.CompareTag("Plate") || other.CompareTag("Pan") || other.CompareTag("Pot"))
        {
            HandleKitchenware(other);
        }
        else if (other.CompareTag("Ingredient"))
        {
            HandleIngredient(other);
        }
    }

    private void HandleKitchenware(Collider other)
    {
        Ingredient ingredient = other.GetComponent<Ingredient>();
        if (ingredient != null)
        {
            StartCoroutine(DeactivateAndRespawn(ingredient, other.tag));
        }
    }

    private void HandleIngredient(Collider other)
    {
        Ingredient ingredient = other.GetComponent<Ingredient>();
        if (ingredient != null)
        {
            Destroy(ingredient.transform.parent.parent.gameObject);
        }
    }

    private IEnumerator DeactivateAndRespawnPlayer(GameObject player)
    {
        yield return new WaitForSeconds(1f);
        respawnManager.StartRespawnCountdown(player);
    }

    private IEnumerator DeactivateAndRespawn(Ingredient ingredient, string tag)
    {
        yield return new WaitForSeconds(2.0f);

        Transform returnPosition = GetReturnPosition(ingredient.team, tag);
        if (returnPosition != null)
        {
            GameObject prefabToSpawn = GetPrefabToSpawn(tag);
            if (prefabToSpawn != null)
            {
                GameObject newObject = Instantiate(prefabToSpawn, returnPosition.parent);
                CloneValues(ingredient.gameObject, newObject);
                Destroy(ingredient.gameObject);

                ResetTransform(newObject, returnPosition, tag);
                newObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                returnPosition.parent.GetChild(0).GetComponent<ObjectHighlight>().onSomething = true;

                Debug.Log($"{tag}가 반환되었습니다.");
            }
            else
            {
                Debug.LogWarning($"{tag}의 프리팹이 설정되지 않았습니다.");
            }
        }
        else
        {
            Debug.LogWarning($"{tag}의 반환 위치를 찾을 수 없습니다.");
        }
    }

    private GameObject GetPrefabToSpawn(string tag)
    {
        switch (tag)
        {
            case "Plate": return Plate;
            case "Pan": return Pan;
            case "Pot": return Pot;
            default: return null;
        }
    }

    private void ResetTransform(GameObject obj, Transform returnPosition, string tag)
    {
        obj.transform.position = returnPosition.position;
        obj.transform.rotation = Quaternion.identity;
        obj.GetComponent<Rigidbody>().velocity = Vector3.zero;
        obj.transform.SetSiblingIndex(2);

        obj.transform.localPosition = tag == "Plate"
            ? new Vector3(0.072f, 0.006f, 0.024f)
            : new Vector3(0.0f, 0.006f, 0.0f);

        obj.SetActive(true);
    }

    private void CloneValues(GameObject original, GameObject clone)
    {
        if (original.CompareTag("Plate"))
        {
            CopyPlateValues(original, clone);
        }
        else if (original.CompareTag("Pan"))
        {
            CopyPanValues(original, clone);
        }
        else if (original.CompareTag("Pot"))
        {
            CopyPotValues(original, clone);
        }
    }

    private void CopyPlateValues(GameObject original, GameObject clone)
    {
        Plates originalPlates = original.GetComponent<Plates>();
        Plates clonePlates = clone.GetComponent<Plates>();

        if (originalPlates != null && clonePlates != null)
        {
            clonePlates.canvas = originalPlates.canvas;
            clonePlates.ingredientUI = ingredientUI;
            clonePlates.icons = icons;
        }
    }

    private void CopyPanValues(GameObject original, GameObject clone)
    {
        PanOnStove originalPan = original.GetComponent<PanOnStove>();
        PanOnStove clonePan = clone.GetComponent<PanOnStove>();

        if (originalPan != null && clonePan != null)
        {
            clonePan.canvas = originalPan.canvas;
            clonePan.cookingBar = originalPan.cookingBar;
            clonePan.ingredientUI = ingredientUI;
            clonePan.pfxFire = originalPan.pfxFire;
            clonePan.icons = icons;
        }
    }

    private void CopyPotValues(GameObject original, GameObject clone)
    {
        PotOnStove originalPot = original.GetComponent<PotOnStove>();
        PotOnStove clonePot = clone.GetComponent<PotOnStove>();

        if (originalPot != null && clonePot != null)
        {
            clonePot.canvas = originalPot.canvas;
            clonePot.cookingBar = originalPot.cookingBar;
            clonePot.ingredientUI = ingredientUI;
            clonePot.pfxFire = originalPot.pfxFire;
            clonePot.icons = icons;
        }
    }

    private Transform GetReturnPosition(Ingredient.Team team, string objectType)
    {
        TeamReturnPositions teamPositions = team == Ingredient.Team.Red ? redTeamPositions : blueTeamPositions;
        Transform[] positions = objectType switch
        {
            "Pan" => teamPositions.panPositions,
            "Pot" => teamPositions.potPositions,
            "Plate" => teamPositions.platePositions,
            _ => null,
        };

        if (positions != null)
        {
            foreach (Transform position in positions)
            {
                if ((position.parent.childCount == 2 && position.parent.gameObject.name.Contains("Plate")) ||
                    (position.parent.childCount == 3 && (position.parent.gameObject.name.Contains("Pot") || position.parent.gameObject.name.Contains("Pan"))))
                {
                    return position;
                }
            }
        }

        return null;
    }
}
