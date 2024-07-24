using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Plates_Net : MonoBehaviour
{
    public List<Ingredient_Net.IngredientType> containIngredients = new List<Ingredient_Net.IngredientType>();
    public int limit = 1;
    public GameObject Canvas;
    public GameObject canvas;
    [SerializeField] private GameObject IngredientUI;
    [SerializeField] private Sprite[] Icons;
    public GameObject madeUI;

    public bool AddIngredient(Ingredient_Net.IngredientType handleType)
    {
        //Dough가 활성화되어야 하는 재료들
        var requiresDough = new[] {
            Ingredient_Net.IngredientType.Pepperoni,
            Ingredient_Net.IngredientType.Cheese,
            Ingredient_Net.IngredientType.PizzaTomato
        };

        var requiresTortilla = new[] {
            Ingredient_Net.IngredientType.Meat,
            Ingredient_Net.IngredientType.Chicken,
            Ingredient_Net.IngredientType.Rice
        };

        var requiresSeaWeed = new[] {
            Ingredient_Net.IngredientType.SushiRice,
            Ingredient_Net.IngredientType.SushiFish,
            Ingredient_Net.IngredientType.SushiCucumber
        };

        // Dough가 포함되지 않은 상태에서 requiresDough 타입이 들어오면 false 반환
        if (requiresDough.Contains(handleType) && !containIngredients.Contains(Ingredient_Net.IngredientType.Dough))
        {
            return false;
        }

        // Tortilla가 포함되지 않은 상태에서 requiresTortilla 타입이 들어오면 false 반환
        if (requiresTortilla.Contains(handleType) && !containIngredients.Contains(Ingredient_Net.IngredientType.Tortilla))
        {
            return false;
        }

        // SeaWeed가 포함되지 않은 상태에서 requiresTortilla 타입이 들어오면 false 반환
        if (requiresSeaWeed.Contains(handleType) && !containIngredients.Contains(Ingredient_Net.IngredientType.SeaWeed))
        {
            return false;
        }

        if (!CheckOverlap(handleType) && containIngredients.Count < limit)
        {
            containIngredients.Add(handleType);

            switch (handleType)
            {
                case Ingredient_Net.IngredientType.Fish:
                    transform.GetChild(1).gameObject.SetActive(true);
                    break;
                case Ingredient_Net.IngredientType.Shrimp:
                    transform.GetChild(2).gameObject.SetActive(true);
                    break;
                case Ingredient_Net.IngredientType.Tomato:
                    transform.GetChild(3).gameObject.SetActive(true);
                    break;
                case Ingredient_Net.IngredientType.Lettuce:
                    transform.GetChild(4).gameObject.SetActive(true);
                    break;
                case Ingredient_Net.IngredientType.Cucumber:
                    transform.GetChild(5).gameObject.SetActive(true);
                    break;
                case Ingredient_Net.IngredientType.Potato:      // 썰리고 조리 후 올라감
                    transform.GetChild(6).gameObject.SetActive(true);
                    break;
                case Ingredient_Net.IngredientType.SeaWeed:
                case Ingredient_Net.IngredientType.SushiRice:
                case Ingredient_Net.IngredientType.SushiFish:
                case Ingredient_Net.IngredientType.SushiCucumber:
                    UpdateSushiCombinations();
                    break;
                case Ingredient_Net.IngredientType.Tortilla:
                case Ingredient_Net.IngredientType.Meat:
                case Ingredient_Net.IngredientType.Chicken:
                case Ingredient_Net.IngredientType.Rice:
                    UpdateTortillaCombinations();
                    break;
                case Ingredient_Net.IngredientType.Dough:       // 썰리고 조리 전에 올라서 오븐에 들어감
                    transform.GetChild(9).gameObject.SetActive(true);
                    break;
                case Ingredient_Net.IngredientType.Pepperoni:   // 썰리고 조리 전에 올라서 오븐에 들어감
                    transform.GetChild(10).gameObject.SetActive(true);
                    break;
                case Ingredient_Net.IngredientType.Cheese:      // 썰리고 조리 전에 올라서 오븐에 들어감
                    transform.GetChild(11).gameObject.SetActive(true);
                    break;
                case Ingredient_Net.IngredientType.PizzaTomato:      // 썰리고 조리 전에 올라서 오븐에 들어감
                    transform.GetChild(12).gameObject.SetActive(true);
                    break;
                default:
                    // 기본적으로 아무것도 하지 않음
                    break;
            }
            return true;
        }
        return false;
    }

    public void ClearIngredient()
    {
        if (containIngredients.Count == 1)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        containIngredients.RemoveRange(0, containIngredients.Count); //다 지우고
        Destroy(madeUI); //UI도 지우기
    }
    private bool CheckOverlap(Ingredient_Net.IngredientType type)
    {
        for (int i = 0; i < containIngredients.Count; i++)
        {
            if (containIngredients[i].Equals(type))
            {
                return true;
            }
        }
        return false;
    }

    public void InstantiateUI()
    {
        if (containIngredients.Count == 1)
        {
            madeUI = Instantiate(IngredientUI, Vector3.zero, Quaternion.identity, Canvas.transform);
            madeUI.transform.GetChild(1).gameObject.SetActive(false);
            madeUI.transform.GetChild(2).gameObject.SetActive(false);
            madeUI.transform.GetChild(3).gameObject.SetActive(false);
            Image[] images = new Image[1];
            images[0] = madeUI.transform.GetChild(0).GetComponent<Image>();
            AddIconMethod(images);
            madeUI.GetComponent<IngredientUI_Net>().Target = transform;
        }
        else if (containIngredients.Count == 2) //접시에 합쳐진게 2개면
        {
            madeUI.transform.GetChild(0).gameObject.SetActive(false);
            madeUI.transform.GetChild(1).gameObject.SetActive(true);
            madeUI.transform.GetChild(2).gameObject.SetActive(false);
            madeUI.transform.GetChild(3).gameObject.SetActive(false);
            Image[] images = new Image[2];
            images[0] = madeUI.transform.GetChild(1).GetChild(0).GetComponent<Image>();
            images[1] = madeUI.transform.GetChild(1).GetChild(1).GetComponent<Image>();
            AddIconMethod(images);
        }
        else if (containIngredients.Count == 3) //재료가 3개면
        {
            madeUI.transform.GetChild(0).gameObject.SetActive(false);
            madeUI.transform.GetChild(1).gameObject.SetActive(false);
            madeUI.transform.GetChild(2).gameObject.SetActive(true);
            madeUI.transform.GetChild(3).gameObject.SetActive(false);
            Image[] images = new Image[3];
            images[0] = madeUI.transform.GetChild(2).GetChild(0).GetComponent<Image>();
            images[1] = madeUI.transform.GetChild(2).GetChild(1).GetComponent<Image>();
            images[2] = madeUI.transform.GetChild(2).GetChild(2).GetComponent<Image>();
            AddIconMethod(images);
        }
        else if (containIngredients.Count == 4) //재료가 4개면
        {
            madeUI.transform.GetChild(0).gameObject.SetActive(false);
            madeUI.transform.GetChild(1).gameObject.SetActive(false);
            madeUI.transform.GetChild(2).gameObject.SetActive(false);
            madeUI.transform.GetChild(3).gameObject.SetActive(true);
            Image[] images = new Image[4];
            images[0] = madeUI.transform.GetChild(3).GetChild(0).GetComponent<Image>();
            images[1] = madeUI.transform.GetChild(3).GetChild(1).GetComponent<Image>();
            images[2] = madeUI.transform.GetChild(3).GetChild(2).GetComponent<Image>();
            images[3] = madeUI.transform.GetChild(3).GetChild(3).GetComponent<Image>();
            AddIconMethod(images);
        }
    }
    private void AddIconMethod(Image[] images)
    {
        for (int i = 0; i < containIngredients.Count; i++)
        {
            switch (containIngredients[i])
            {
                case Ingredient_Net.IngredientType.Fish:
                    images[i].sprite = Icons[0];
                    break;
                case Ingredient_Net.IngredientType.Shrimp:
                    images[i].sprite = Icons[1];
                    break;
                case Ingredient_Net.IngredientType.Tomato:
                    images[i].sprite = Icons[2];
                    break;
                case Ingredient_Net.IngredientType.Lettuce:
                    images[i].sprite = Icons[3];
                    break;
                case Ingredient_Net.IngredientType.Cucumber:
                    images[i].sprite = Icons[4];
                    break;
                case Ingredient_Net.IngredientType.Potato:
                    images[i].sprite = Icons[5];
                    break;
                case Ingredient_Net.IngredientType.Chicken:
                    images[i].sprite = Icons[6];
                    break;
                case Ingredient_Net.IngredientType.SeaWeed:
                    images[i].sprite = Icons[7];
                    break;
                case Ingredient_Net.IngredientType.Tortilla:
                    images[i].sprite = Icons[8];
                    break;
                case Ingredient_Net.IngredientType.Rice:
                    images[i].sprite = Icons[9];
                    break;
                case Ingredient_Net.IngredientType.Pepperoni:
                    images[i].sprite = Icons[10];
                    break;
                case Ingredient_Net.IngredientType.Meat:
                    images[i].sprite = Icons[11];
                    break;
                case Ingredient_Net.IngredientType.Dough:
                    images[i].sprite = Icons[12];
                    break;
                case Ingredient_Net.IngredientType.Cheese:
                    images[i].sprite = Icons[13];
                    break;
                case Ingredient_Net.IngredientType.SushiRice:
                    images[i].sprite = Icons[9];
                    break;
                case Ingredient_Net.IngredientType.SushiFish:
                    images[i].sprite = Icons[0];
                    break;
                case Ingredient_Net.IngredientType.SushiCucumber:
                    images[i].sprite = Icons[4];
                    break;
                case Ingredient_Net.IngredientType.PizzaTomato:
                    images[i].sprite = Icons[2];
                    break;
                default:
                    // 기본적으로 아무것도 하지 않음
                    break;
            }

        }
    }

    private void UpdateSushiCombinations()
    {
        bool hasSeaWeed = containIngredients.Contains(Ingredient_Net.IngredientType.SeaWeed);
        bool hasSushiFish = containIngredients.Contains(Ingredient_Net.IngredientType.SushiFish);
        bool hasSushiCucumber = containIngredients.Contains(Ingredient_Net.IngredientType.SushiCucumber);
        bool hasSushiRice = containIngredients.Contains(Ingredient_Net.IngredientType.SushiRice);

        // 모든 조합 비활성화
        transform.GetChild(7).gameObject.SetActive(false); // SeaWeed
        transform.GetChild(13).GetChild(0).gameObject.SetActive(false); // SeaWeed + SushiFish
        transform.GetChild(13).GetChild(1).gameObject.SetActive(false); // SeaWeed + SushiCucumber
        transform.GetChild(13).GetChild(2).gameObject.SetActive(false); // SeaWeed + SushiRice
        transform.GetChild(13).GetChild(3).gameObject.SetActive(false); // SeaWeed + SushiFish + SushiCucumber
        transform.GetChild(13).GetChild(4).gameObject.SetActive(false); // SeaWeed + SushiRice + SushiFish
        transform.GetChild(13).GetChild(5).gameObject.SetActive(false); // SeaWeed + SushiRice + SushiCucumber
        transform.GetChild(13).GetChild(6).gameObject.SetActive(false); // SeaWeed + SushiRice + SushiFish + SushiCucumber

        // 적절한 조합 활성화 2
        if (hasSeaWeed)
        {
            if (hasSushiFish && hasSushiCucumber && hasSushiRice)
            {
                transform.GetChild(13).GetChild(6).gameObject.SetActive(true); // SeaWeed + SushiRice + SushiFish + SushiCucumber
            }
            else if (hasSushiFish && hasSushiCucumber)
            {
                transform.GetChild(13).GetChild(3).gameObject.SetActive(true); // SeaWeed + SushiFish + SushiCucumber
            }
            else if (hasSushiRice && hasSushiFish)
            {
                transform.GetChild(13).GetChild(4).gameObject.SetActive(true); // SeaWeed + SushiRice + SushiFish
            }
            else if (hasSushiRice && hasSushiCucumber)
            {
                transform.GetChild(13).GetChild(5).gameObject.SetActive(true); // SeaWeed + SushiRice + SushiCucumber
            }
            else if (hasSushiFish)
            {
                transform.GetChild(13).GetChild(0).gameObject.SetActive(true); // SeaWeed + SushiFish
            }
            else if (hasSushiCucumber)
            {
                transform.GetChild(13).GetChild(1).gameObject.SetActive(true); // SeaWeed + SushiCucumber
            }
            else if (hasSushiRice)
            {
                transform.GetChild(13).GetChild(2).gameObject.SetActive(true); // SeaWeed + SushiRice
            }
            else
            {
                transform.GetChild(7).gameObject.SetActive(true); // SeaWeed만 활성화
            }
        }
    }

    private void UpdateTortillaCombinations()
    {
        bool hasTortilla = containIngredients.Contains(Ingredient_Net.IngredientType.Tortilla);
        bool hasMeat = containIngredients.Contains(Ingredient_Net.IngredientType.Meat);
        bool hasChicken = containIngredients.Contains(Ingredient_Net.IngredientType.Chicken);
        bool hasRice = containIngredients.Contains(Ingredient_Net.IngredientType.Rice);

        // 모든 조합 비활성화
        transform.GetChild(8).gameObject.SetActive(false); // Tortilla
        transform.GetChild(14).GetChild(0).gameObject.SetActive(false); // Tortilla_Meat
        transform.GetChild(14).GetChild(1).gameObject.SetActive(false); // Tortilla_Chicken
        transform.GetChild(14).GetChild(2).gameObject.SetActive(false); // Tortilla_Rice
        transform.GetChild(14).GetChild(3).gameObject.SetActive(false); // Tortilla_Rice_Meat
        transform.GetChild(14).GetChild(4).gameObject.SetActive(false); // Tortilla_Rice_Chicken
        transform.GetChild(14).GetChild(5).gameObject.SetActive(false); // Tortilla_Chicken_Meat
        transform.GetChild(14).GetChild(6).gameObject.SetActive(false); // Tortilla_Rice_Meat_Chicken

        // 적절한 조합 활성화
        if (hasTortilla)
        {
            if (hasMeat && hasChicken && hasRice)
            {
                transform.GetChild(14).GetChild(6).gameObject.SetActive(true); // Tortilla + Rice + Meat + Chicken
            }
            else if (hasChicken && hasMeat)
            {
                transform.GetChild(14).GetChild(5).gameObject.SetActive(true); // Tortilla + Chicken + Meat
            }
            else if (hasRice && hasMeat)
            {
                transform.GetChild(14).GetChild(3).gameObject.SetActive(true); // Tortilla + Rice + Meat
            }
            else if (hasRice && hasChicken)
            {
                transform.GetChild(14).GetChild(4).gameObject.SetActive(true); // Tortilla + Rice + Chicken
            }
            else if (hasMeat)
            {
                transform.GetChild(14).GetChild(0).gameObject.SetActive(true); // Tortilla + Meat
            }
            else if (hasChicken)
            {
                transform.GetChild(14).GetChild(1).gameObject.SetActive(true); // Tortilla + Chicken
            }
            else if (hasRice)
            {
                transform.GetChild(14).GetChild(2).gameObject.SetActive(true); // Tortilla + Rice
            }
            else
            {
                transform.GetChild(8).gameObject.SetActive(true); // Tortilla만 활성화
            }
        }
    }
}



/*

public class Plates_Net : MonoBehaviour
{
    public List<Ingredient_Net.IngredientType> containIngredients = new List<Ingredient_Net.IngredientType>();
    public int limit = 1;
    public GameObject Canvas;
    [SerializeField] private GameObject IngredientUI;
    [SerializeField] private Sprite[] Icons;
    public GameObject madeUI;
    private Transform canvasTransform; // UI가 생성될 부모 Transform

    private void Start()
    {
        //canvasTransform = Camera.main.transform.Find("Canvas");
        //Canvas = canvasTransform.gameObject;
    }

    public bool AddIngredient(Ingredient_Net.IngredientType handleType)
    {
        //Dough가 활성화되어야 하는 재료들
        var requiresDough = new[] {
            Ingredient_Net.IngredientType.Pepperoni,
            Ingredient_Net.IngredientType.Cheese,
            Ingredient_Net.IngredientType.PizzaTomato
        };

        var requiresTortilla = new[] {
            Ingredient_Net.IngredientType.Meat,
            Ingredient_Net.IngredientType.Chicken,
            Ingredient_Net.IngredientType.Rice
        };

        var requiresSeaWeed = new[] {
            Ingredient_Net.IngredientType.SushiRice,
            Ingredient_Net.IngredientType.SushiFish,
            Ingredient_Net.IngredientType.SushiCucumber
        };

        // Dough가 포함되지 않은 상태에서 requiresDough 타입이 들어오면 false 반환
        if (requiresDough.Contains(handleType) && !containIngredients.Contains(Ingredient_Net.IngredientType.Dough))
        {
            return false;
        }

        // Tortilla가 포함되지 않은 상태에서 requiresTortilla 타입이 들어오면 false 반환
        if (requiresTortilla.Contains(handleType) && !containIngredients.Contains(Ingredient_Net.IngredientType.Tortilla))
        {
            return false;
        }

        // SeaWeed가 포함되지 않은 상태에서 requiresTortilla 타입이 들어오면 false 반환
        if (requiresSeaWeed.Contains(handleType) && !containIngredients.Contains(Ingredient_Net.IngredientType.SeaWeed))
        {
            return false;
        }

        if (!CheckOverlap(handleType) && containIngredients.Count < limit)
        {
            containIngredients.Add(handleType);

            switch (handleType)
            {
                case Ingredient_Net.IngredientType.Fish:
                    transform.GetChild(1).gameObject.SetActive(true);
                    break;
                case Ingredient_Net.IngredientType.Shrimp:
                    transform.GetChild(2).gameObject.SetActive(true);
                    break;
                case Ingredient_Net.IngredientType.Tomato:
                    transform.GetChild(3).gameObject.SetActive(true);
                    break;
                case Ingredient_Net.IngredientType.Lettuce:
                    transform.GetChild(4).gameObject.SetActive(true);
                    break;
                case Ingredient_Net.IngredientType.Cucumber:
                    transform.GetChild(5).gameObject.SetActive(true);
                    break;
                case Ingredient_Net.IngredientType.Potato:      // 썰리고 조리 후 올라감
                    transform.GetChild(6).gameObject.SetActive(true);
                    break;
                case Ingredient_Net.IngredientType.SeaWeed:
                case Ingredient_Net.IngredientType.SushiRice:
                case Ingredient_Net.IngredientType.SushiFish:
                case Ingredient_Net.IngredientType.SushiCucumber:
                    UpdateSushiCombinations();
                    break;
                case Ingredient_Net.IngredientType.Tortilla:
                case Ingredient_Net.IngredientType.Meat:
                case Ingredient_Net.IngredientType.Chicken:
                case Ingredient_Net.IngredientType.Rice:
                    UpdateTortillaCombinations();
                    break;
                case Ingredient_Net.IngredientType.Dough:       // 썰리고 조리 전에 올라서 오븐에 들어감
                    transform.GetChild(9).gameObject.SetActive(true);
                    break;
                case Ingredient_Net.IngredientType.Pepperoni:   // 썰리고 조리 전에 올라서 오븐에 들어감
                    transform.GetChild(10).gameObject.SetActive(true);
                    break;
                case Ingredient_Net.IngredientType.Cheese:      // 썰리고 조리 전에 올라서 오븐에 들어감
                    transform.GetChild(11).gameObject.SetActive(true);
                    break;
                case Ingredient_Net.IngredientType.PizzaTomato:      // 썰리고 조리 전에 올라서 오븐에 들어감
                    transform.GetChild(12).gameObject.SetActive(true);
                    break;
                default:
                    // 기본적으로 아무것도 하지 않음
                    break;
            }
            return true;
        }
        return false;
    }

    public void ClearIngredient()
    {
        if (containIngredients.Count == 1)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        containIngredients.RemoveRange(0, containIngredients.Count); //다 지우고
        Destroy(madeUI); //UI도 지우기
    }
    private bool CheckOverlap(Ingredient_Net.IngredientType type)
    {
        for (int i = 0; i < containIngredients.Count; i++)
        {
            if (containIngredients[i].Equals(type))
            {
                return true;
            }
        }
        return false;
    }

    //[Command]
    public void CmdInstantiateUI()
    {
        //RpcInstantiateUI();
    }

    //[ClientRpc]
    public void InstantiateUI()
    {
        OverNetworkRoomManager manager = (OverNetworkRoomManager)NetworkRoomManager.singleton;
        GameObject IngredientUI_ = manager.GetUIObject_RoomManager("IngredientUI_Net");

        if (containIngredients.Count == 1)
        {
            madeUI = Instantiate(IngredientUI_, Vector3.zero, Quaternion.identity, Canvas.transform);
            madeUI.transform.GetChild(1).gameObject.SetActive(false);
            madeUI.transform.GetChild(2).gameObject.SetActive(false);
            madeUI.transform.GetChild(3).gameObject.SetActive(false);
            Image[] images = new Image[1];
            images[0] = madeUI.transform.GetChild(0).GetComponent<Image>();
            AddIconMethod(images);
            madeUI.GetComponent<IngredientUI_Net>().Target = transform;
        }
        else if (containIngredients.Count == 2) //접시에 합쳐진게 2개면
        {
            madeUI.transform.GetChild(0).gameObject.SetActive(false);
            madeUI.transform.GetChild(1).gameObject.SetActive(true);
            madeUI.transform.GetChild(2).gameObject.SetActive(false);
            madeUI.transform.GetChild(3).gameObject.SetActive(false);
            Image[] images = new Image[2];
            images[0] = madeUI.transform.GetChild(1).GetChild(0).GetComponent<Image>();
            images[1] = madeUI.transform.GetChild(1).GetChild(1).GetComponent<Image>();
            AddIconMethod(images);
        }
        else if (containIngredients.Count == 3) //재료가 3개면
        {
            madeUI.transform.GetChild(0).gameObject.SetActive(false);
            madeUI.transform.GetChild(1).gameObject.SetActive(false);
            madeUI.transform.GetChild(2).gameObject.SetActive(true);
            madeUI.transform.GetChild(3).gameObject.SetActive(false);
            Image[] images = new Image[3];
            images[0] = madeUI.transform.GetChild(2).GetChild(0).GetComponent<Image>();
            images[1] = madeUI.transform.GetChild(2).GetChild(1).GetComponent<Image>();
            images[2] = madeUI.transform.GetChild(2).GetChild(2).GetComponent<Image>();
            AddIconMethod(images);
        }
        else if (containIngredients.Count == 4) //재료가 4개면
        {
            madeUI.transform.GetChild(0).gameObject.SetActive(false);
            madeUI.transform.GetChild(1).gameObject.SetActive(false);
            madeUI.transform.GetChild(2).gameObject.SetActive(false);
            madeUI.transform.GetChild(3).gameObject.SetActive(true);
            Image[] images = new Image[4];
            images[0] = madeUI.transform.GetChild(3).GetChild(0).GetComponent<Image>();
            images[1] = madeUI.transform.GetChild(3).GetChild(1).GetComponent<Image>();
            images[2] = madeUI.transform.GetChild(3).GetChild(2).GetComponent<Image>();
            images[3] = madeUI.transform.GetChild(3).GetChild(3).GetComponent<Image>();
            AddIconMethod(images);
        }
    }
    private void AddIconMethod(Image[] images)
    {
        for (int i = 0; i < containIngredients.Count; i++)
        {
            switch (containIngredients[i])
            {
                case Ingredient_Net.IngredientType.Fish:
                    images[i].sprite = Icons[0];
                    break;
                case Ingredient_Net.IngredientType.Shrimp:
                    images[i].sprite = Icons[1];
                    break;
                case Ingredient_Net.IngredientType.Tomato:
                    images[i].sprite = Icons[2];
                    break;
                case Ingredient_Net.IngredientType.Lettuce:
                    images[i].sprite = Icons[3];
                    break;
                case Ingredient_Net.IngredientType.Cucumber:
                    images[i].sprite = Icons[4];
                    break;
                case Ingredient_Net.IngredientType.Potato:
                    images[i].sprite = Icons[5];
                    break;
                case Ingredient_Net.IngredientType.Chicken:
                    images[i].sprite = Icons[6];
                    break;
                case Ingredient_Net.IngredientType.SeaWeed:
                    images[i].sprite = Icons[7];
                    break;
                case Ingredient_Net.IngredientType.Tortilla:
                    images[i].sprite = Icons[8];
                    break;
                case Ingredient_Net.IngredientType.Rice:
                    images[i].sprite = Icons[9];
                    break;
                case Ingredient_Net.IngredientType.Pepperoni:
                    images[i].sprite = Icons[10];
                    break;
                case Ingredient_Net.IngredientType.Meat:
                    images[i].sprite = Icons[11];
                    break;
                case Ingredient_Net.IngredientType.Dough:
                    images[i].sprite = Icons[12];
                    break;
                case Ingredient_Net.IngredientType.Cheese:
                    images[i].sprite = Icons[13];
                    break;
                case Ingredient_Net.IngredientType.SushiRice:
                    images[i].sprite = Icons[9];
                    break;
                case Ingredient_Net.IngredientType.SushiFish:
                    images[i].sprite = Icons[0];
                    break;
                case Ingredient_Net.IngredientType.SushiCucumber:
                    images[i].sprite = Icons[4];
                    break;
                case Ingredient_Net.IngredientType.PizzaTomato:
                    images[i].sprite = Icons[2];
                    break;
                default:
                    // 기본적으로 아무것도 하지 않음
                    break;
            }

        }
    }

    private void UpdateSushiCombinations()
    {
        bool hasSeaWeed = containIngredients.Contains(Ingredient_Net.IngredientType.SeaWeed);
        bool hasSushiFish = containIngredients.Contains(Ingredient_Net.IngredientType.SushiFish);
        bool hasSushiCucumber = containIngredients.Contains(Ingredient_Net.IngredientType.SushiCucumber);
        bool hasSushiRice = containIngredients.Contains(Ingredient_Net.IngredientType.SushiRice);

        // 모든 조합 비활성화
        transform.GetChild(7).gameObject.SetActive(false); // SeaWeed
        transform.GetChild(13).GetChild(0).gameObject.SetActive(false); // SeaWeed + SushiFish
        transform.GetChild(13).GetChild(1).gameObject.SetActive(false); // SeaWeed + SushiCucumber
        transform.GetChild(13).GetChild(2).gameObject.SetActive(false); // SeaWeed + SushiRice
        transform.GetChild(13).GetChild(3).gameObject.SetActive(false); // SeaWeed + SushiFish + SushiCucumber
        transform.GetChild(13).GetChild(4).gameObject.SetActive(false); // SeaWeed + SushiRice + SushiFish
        transform.GetChild(13).GetChild(5).gameObject.SetActive(false); // SeaWeed + SushiRice + SushiCucumber
        transform.GetChild(13).GetChild(6).gameObject.SetActive(false); // SeaWeed + SushiRice + SushiFish + SushiCucumber

        // 적절한 조합 활성화 2
        if (hasSeaWeed)
        {
            if (hasSushiFish && hasSushiCucumber && hasSushiRice)
            {
                transform.GetChild(13).GetChild(6).gameObject.SetActive(true); // SeaWeed + SushiRice + SushiFish + SushiCucumber
            }
            else if (hasSushiFish && hasSushiCucumber)
            {
                transform.GetChild(13).GetChild(3).gameObject.SetActive(true); // SeaWeed + SushiFish + SushiCucumber
            }
            else if (hasSushiRice && hasSushiFish)
            {
                transform.GetChild(13).GetChild(4).gameObject.SetActive(true); // SeaWeed + SushiRice + SushiFish
            }
            else if (hasSushiRice && hasSushiCucumber)
            {
                transform.GetChild(13).GetChild(5).gameObject.SetActive(true); // SeaWeed + SushiRice + SushiCucumber
            }
            else if (hasSushiFish)
            {
                transform.GetChild(13).GetChild(0).gameObject.SetActive(true); // SeaWeed + SushiFish
            }
            else if (hasSushiCucumber)
            {
                transform.GetChild(13).GetChild(1).gameObject.SetActive(true); // SeaWeed + SushiCucumber
            }
            else if (hasSushiRice)
            {
                transform.GetChild(13).GetChild(2).gameObject.SetActive(true); // SeaWeed + SushiRice
            }
            else
            {
                transform.GetChild(7).gameObject.SetActive(true); // SeaWeed만 활성화
            }
        }
    }

    private void UpdateTortillaCombinations()
    {
        bool hasTortilla = containIngredients.Contains(Ingredient_Net.IngredientType.Tortilla);
        bool hasMeat = containIngredients.Contains(Ingredient_Net.IngredientType.Meat);
        bool hasChicken = containIngredients.Contains(Ingredient_Net.IngredientType.Chicken);
        bool hasRice = containIngredients.Contains(Ingredient_Net.IngredientType.Rice);

        // 모든 조합 비활성화
        transform.GetChild(8).gameObject.SetActive(false); // Tortilla
        transform.GetChild(14).GetChild(0).gameObject.SetActive(false); // Tortilla_Meat
        transform.GetChild(14).GetChild(1).gameObject.SetActive(false); // Tortilla_Chicken
        transform.GetChild(14).GetChild(2).gameObject.SetActive(false); // Tortilla_Rice
        transform.GetChild(14).GetChild(3).gameObject.SetActive(false); // Tortilla_Rice_Meat
        transform.GetChild(14).GetChild(4).gameObject.SetActive(false); // Tortilla_Rice_Chicken
        transform.GetChild(14).GetChild(5).gameObject.SetActive(false); // Tortilla_Chicken_Meat
        transform.GetChild(14).GetChild(6).gameObject.SetActive(false); // Tortilla_Rice_Meat_Chicken

        // 적절한 조합 활성화
        if (hasTortilla)
        {
            if (hasMeat && hasChicken && hasRice)
            {
                transform.GetChild(14).GetChild(6).gameObject.SetActive(true); // Tortilla + Rice + Meat + Chicken
            }
            else if (hasChicken && hasMeat)
            {
                transform.GetChild(14).GetChild(5).gameObject.SetActive(true); // Tortilla + Chicken + Meat
            }
            else if (hasRice && hasMeat)
            {
                transform.GetChild(14).GetChild(3).gameObject.SetActive(true); // Tortilla + Rice + Meat
            }
            else if (hasRice && hasChicken)
            {
                transform.GetChild(14).GetChild(4).gameObject.SetActive(true); // Tortilla + Rice + Chicken
            }
            else if (hasMeat)
            {
                transform.GetChild(14).GetChild(0).gameObject.SetActive(true); // Tortilla + Meat
            }
            else if (hasChicken)
            {
                transform.GetChild(14).GetChild(1).gameObject.SetActive(true); // Tortilla + Chicken
            }
            else if (hasRice)
            {
                transform.GetChild(14).GetChild(2).gameObject.SetActive(true); // Tortilla + Rice
            }
            else
            {
                transform.GetChild(8).gameObject.SetActive(true); // Tortilla만 활성화
            }
        }
    }
}

*/