using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using static Ingredient;

public class CuttingBoard : MonoBehaviour
{
    private ObjectHighlight parentObject;
    public Slider cookingBar;
    [SerializeField] Vector3 pos;
    public Coroutine _CoTimer;
    public bool Pause = false;
    public float CuttingTime;

    [SerializeField] private GameObject Canvas;
    [SerializeField] private GameObject IngredientUI;
    [SerializeField] private Sprite[] Icons;

    private void Start()
    {
        parentObject = transform.parent.GetComponent<ObjectHighlight>();
    }

    private void Update()
    {
        UpdateCookingBarPosition();
        UpdateCookingBarValue();
        ToggleKnife();
    }

    private void UpdateCookingBarPosition()
    {
        cookingBar.transform.position = Camera.main.WorldToScreenPoint(parentObject.transform.position + pos);
    }

    private void UpdateCookingBarValue()
    {
        cookingBar.value = CuttingTime;
    }

    private void ToggleKnife()
    {
        transform.GetChild(0).GetChild(0).gameObject.SetActive(!parentObject.onSomething);
    }

    public void StartCutting1(UnityAction EndCallBack = null)
    {
        StartCutting(FindObjectOfType<PlayerInteractController>(), EndCallBack);
    }

    public void StartCutting2(UnityAction EndCallBack = null)
    {
        StartCutting(FindObjectOfType<Player2InteractController>(), EndCallBack);
    }

    private void StartCutting(PlayerInteractController playerController, UnityAction EndCallBack = null)
    {
        if (parentObject.onSomething)
        {
            SoundManager.Instance.PlayEffect("cut");
            cookingBar.gameObject.SetActive(true);
            ClearTime();
            _CoTimer = StartCoroutine(CoStartCutting(EndCallBack));
            playerController.anim.SetTrigger("startCut");
            playerController.anim.SetBool("canCut", true);
        }
    }
    private void StartCutting(Player2InteractController playerController, UnityAction EndCallBack = null)
    {
        if (parentObject.onSomething)
        {
            SoundManager.Instance.PlayEffect("cut");
            cookingBar.gameObject.SetActive(true);
            ClearTime();
            _CoTimer = StartCoroutine(CoStartCutting(EndCallBack));
            playerController.anim.SetTrigger("startCut");
            playerController.anim.SetBool("canCut", true);
        }
    }

    public void PauseSlider(bool pause)
    {
        Pause = pause;
    }

    private IEnumerator CoStartCutting(UnityAction EndCallBack = null)
    {
        while (CuttingTime <= 1)
        {
            while (Pause)
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.45f);
            CuttingTime += 0.25f;
            //SoundManager.instance.PlayEffect("cut");
        }
        EndCallBack?.Invoke();
        OffSlider();
        _CoTimer = null;
        Pause = false;
        CuttingTime = 0;
    }

    private void ClearTime()
    {
        if (_CoTimer != null)
        {
            StopCoroutine(_CoTimer);
            _CoTimer = null;
        }
        Pause = false;
    }

    public void OffSlider()
    {
        cookingBar.value = 0f;
        cookingBar.gameObject.SetActive(false);
        UpdateCanCutState(FindObjectOfType<PlayerInteractController>());
        UpdateCanCutState(FindObjectOfType<Player2InteractController>());
        UpdateIngredientState();
        InstantiateUI();
    }

    private void UpdateCanCutState(PlayerInteractController playerController)
    {
        if (playerController.interactObject == transform.parent.gameObject)
        {
            playerController.anim.SetBool("canCut", false);
        }
    }
    private void UpdateCanCutState(Player2InteractController playerController)
    {
        if (playerController.interactObject == transform.parent.gameObject)
        {
            playerController.anim.SetBool("canCut", false);
        }
    }

    private void UpdateIngredientState()
    {
        Ingredient Ingredient = transform.parent.parent.GetChild(2).GetChild(0).GetChild(0).GetComponent<Ingredient>();
        Ingredient.isCooked = true;

        if (Ingredient.type == IngredientType.Meat || Ingredient.type == IngredientType.Chicken)
            Ingredient.isCooked = false;
        
        Ingredient.ChangeMesh(Ingredient.type);
    }

    public void OffAnim1()
    {

    }

    public void InstantiateUI()
    {
        Ingredient Ingredient = transform.parent.parent.GetChild(2).GetChild(0).GetChild(0).GetComponent<Ingredient>();
        GameObject madeUI = Instantiate(IngredientUI, Vector3.zero, Quaternion.identity, Canvas.transform);
        madeUI.transform.GetChild(0).gameObject.SetActive(true);
        Image image = madeUI.transform.GetChild(0).GetComponent<Image>();
        image.sprite = GetIcon(Ingredient.type);
        madeUI.GetComponent<IngredientUI>().Target = Ingredient.transform;
    }

    private Sprite GetIcon(Ingredient.IngredientType ingredientType)
    {
        //switch (ingredientType)
        //{
        //    case Ingredient.IngredientType.Fish:
        //        return Icons[0];
        //    case Ingredient.IngredientType.Shrimp:
        //        return Icons[1];
        //    case Ingredient.IngredientType.Tomato:
        //        return Icons[2];
        //    case Ingredient.IngredientType.Lettuce:
        //        return Icons[3];
        //    case Ingredient.IngredientType.Cucumber:
        //        return Icons[4];
        //    case Ingredient.IngredientType.Potato:
        //        return Icons[5];
        //    case Ingredient.IngredientType.Chicken:
        //        return Icons[6];
        //    default:
        //        throw new ArgumentOutOfRangeException();
        //}
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
