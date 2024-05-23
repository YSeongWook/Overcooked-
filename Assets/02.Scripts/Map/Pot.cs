using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : MonoBehaviour
{
    //public GameObject interactObject;//상호작용 가능한 오브젝트



   

    //void Rosting()
    //{
    //    Debug.Log("OnCookOrThrow");
    //    if (checkInteractObject())
    //    {
    //        if (ShouldStartCutting())
    //            StartRoastingProcess();
    //    }
    //}
    //bool checkInteractObject()
    //{
    //    if (interactObject != null)
    //    {
    //        if (interactObject.GetComponent<ObjectHighlight>().objectType == ObjectHighlight.ObjectType.Ingredient)
    //            return false;
    //        else
    //            return true;
    //    }
    //    return false;
    //}
    //bool ShouldStartCutting()
    //{
    //    return objectHighlight.objectType == ObjectHighlight_Net.ObjectType.Board &&
    //           interactObject.transform.parent.childCount > 2 &&
    //           !interactObject.transform.parent.GetChild(2).GetChild(0).GetChild(0).GetComponent<Ingredient_Net>().isCooked &&
    //           !isHolding;
    //}
    //void StartRoastingProcess()
    //{
    //    var cuttingBoard = interactObject.transform.GetChild(0).GetComponent<CuttingBoard>();

    //    if (cuttingBoard._CoTimer == null) // 한번도 실행 안된거면 시작 가능
    //    {
    //        anim.SetTrigger("startCut");
    //        cuttingBoard.Pause = false;
    //        cuttingBoard.CuttingTime = 0;
    //        cuttingBoard.StartCutting1();
    //    }
    //    else if (cuttingBoard.Pause) // 실행되다 만거라면
    //    {
    //        anim.SetTrigger("startCut");
    //        cuttingBoard.PauseSlider(false);
    //    }
    //}
}
