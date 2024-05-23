using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class InfiniteScroll : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform viewPortTransform;
    public RectTransform contentPanelTransform;
    public HorizontalLayoutGroup HLG;

    public RectTransform[] ItemList;

    private Queue<RectTransform> itemQueue = new Queue<RectTransform>();

    Vector2 OldVelocity;
    bool isUpdated;


    public int firstslotIdx = 0;
    public RectTransform lastItem;

    void Start()
    {
  
    }


    private void FixedUpdate()
    {

    }





    void Update()
    {

       // Debug.Log(contentPanelTransform.localPosition.x);
        if (contentPanelTransform.localPosition.x > 0)
        {
            Debug.Log("?");
            Canvas.ForceUpdateCanvases();

            // OldVelocity = scrollRect.velocity;
            //contentPanelTransform.localPosition -= new Vector3(ItemList.Length * (ItemList[0].rect.width + HLG.spacing), 0, 0);
            //isUpdated = true;
        }


        if (contentPanelTransform.localPosition.x < 0 - (ItemList.Length * ItemList[0].rect.width + ItemList.Length-1 *HLG.spacing))
        {
            Debug.Log("!");
            //Debug.Log(firstslotIdx);
            //firstslotIdx = firstslotIdx % 5;
            //Canvas.ForceUpdateCanvases();
            //ItemList[firstslotIdx].localPosition += new Vector3(ItemList.Length * (ItemList[0].rect.width + HLG.spacing), 0, 0);
            //firstslotIdx++;
        }



    }


}
