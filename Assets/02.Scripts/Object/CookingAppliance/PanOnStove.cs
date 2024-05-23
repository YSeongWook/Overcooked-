using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class PanOnStove : MonoBehaviour
{
    public Slider cookingBar;
    public bool isOnStove = false;
    public bool inSomething = false;
    public float cookingTime;
    private Coroutine _coTimer;
    private bool pause = false;
    private bool stateIsCooked = false;
    //public Animator potAnim;

    [SerializeField] private GameObject Canvas;
    [SerializeField] private GameObject IngredientUI;
    [SerializeField] private Sprite[] Icons;

    private void Start()
    {
        //potAnim.GetComponent<Animator>();
    }

    private void Update()
    {
        if (isOnStove && inSomething && !stateIsCooked)
        {
            UpdateCookingBarPosition();
            UpdateCookingBarValue();
            UpdateisIngredientState();
        }

        if (stateIsCooked)
            cookingBar.gameObject.SetActive(false);
    }

    private void UpdateisIngredientState()
    {
        if (transform.GetChild(2).gameObject != null)
        {
            stateIsCooked = transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Ingredient>().isCooked;
        }
    }

    private void UpdateCookingBarPosition()
    {
        cookingBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1, 0)); // 적절한 위치 조정
    }

    private void UpdateCookingBarValue()
    {
        cookingBar.value = cookingTime;
    }

    public void StartCooking(UnityAction EndCallBack = null)
    {
        if (_coTimer == null)
        {
            cookingBar.gameObject.SetActive(true);
            ClearTime();
            _coTimer = StartCoroutine(CoStartCooking(EndCallBack));
        }
        else if (pause)
        {
            pause = false;
        }
    }

    private IEnumerator CoStartCooking(UnityAction EndCallBack = null)
    {
        while (cookingTime <= 1)
        {
            while (pause)
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.45f);
            cookingTime += 0.25f;
        }
        EndCallBack?.Invoke();
        OffSlider();
        _coTimer = null;
        pause = false;
        cookingTime = 0;
    }

    private void ClearTime()
    {
        if (_coTimer != null)
        {
            StopCoroutine(_coTimer);
            _coTimer = null;
        }
        pause = false;
    }

    public void OffSlider()
    {
        cookingBar.value = 0f;
        cookingBar.gameObject.SetActive(false);
        UpdateIngredientState();
        InstantiateUI();
    }

    private void UpdateIngredientState()
    {
        if (transform.childCount < 3)
            return;
        Ingredient Ingredient = transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Ingredient>();
        Ingredient.isCooked = true;

        //여기선 이제 쿡되야함
        //if (Ingredient.type == Ingredient.IngredientType.Meat || Ingredient.type == Ingredient.IngredientType.Chicken)
        //    Ingredient.isCooked = false;

        Ingredient.ChangeMesh(Ingredient.type);
    }

    public void InstantiateUI()
    {
        if (transform.childCount < 3)
            return;

        Ingredient Ingredient = transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Ingredient>();
        GameObject madeUI = Instantiate(IngredientUI, Vector3.zero, Quaternion.identity, Canvas.transform);
        madeUI.transform.GetChild(0).gameObject.SetActive(true);
        Image image = madeUI.transform.GetChild(0).GetComponent<Image>();
        image.sprite = GetIcon(Ingredient.type);
        madeUI.GetComponent<IngredientUI>().Target = Ingredient.transform;
    }

    private Sprite GetIcon(Ingredient.IngredientType ingredientType)
    {
        switch (ingredientType)
        {
            case Ingredient.IngredientType.Fish:
                return Icons[0];

            case Ingredient.IngredientType.Shrimp:
                return Icons[1];

            case Ingredient.IngredientType.Tomato:
                return Icons[2];

            case Ingredient.IngredientType.Lettuce:
                return Icons[3];

            case Ingredient.IngredientType.Cucumber:
                return Icons[4];

            case Ingredient.IngredientType.Potato:
                return Icons[5];

            case Ingredient.IngredientType.Chicken:
                return Icons[6];

            case Ingredient.IngredientType.SeaWeed:
                return Icons[7];

            case Ingredient.IngredientType.Tortilla:
                return Icons[8];

            case Ingredient.IngredientType.Rice:
                return Icons[9];

            case Ingredient.IngredientType.Pepperoni:
                return Icons[10];

            case Ingredient.IngredientType.Meat:
                return Icons[11];

            case Ingredient.IngredientType.Dough:
                return Icons[12];

            case Ingredient.IngredientType.Cheese:
                return Icons[13];

            case Ingredient.IngredientType.SushiRice:
                return Icons[9];

            case Ingredient.IngredientType.SushiFish:
                return Icons[0];

            case Ingredient.IngredientType.SushiCucumber:
                return Icons[4];

            case Ingredient.IngredientType.PizzaTomato:
                return Icons[2];

            default:
                // 기본적으로 아무것도 하지 않음
                throw new ArgumentOutOfRangeException();
        }
    }
}
