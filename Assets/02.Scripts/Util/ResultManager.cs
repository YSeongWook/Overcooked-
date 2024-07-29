using System.Collections;
using EnumTypes;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private Text Stagename;
    [SerializeField] private Text name1;
    [SerializeField] private Text name2;
    [SerializeField] private StarLimit[] limits;
    [SerializeField] private GameObject[] newStars;
    [SerializeField] private Text[] txtStars;
    [SerializeField] private GameObject success;
    [SerializeField] private GameObject tip;
    [SerializeField] private GameObject fail;
    [SerializeField] private GameObject total;
    [SerializeField] private bool canSkip = false;

    private int successMoney;

    private void Awake()
    {
        UIManager.Instance.sceneType = SceneType.Result;
        UIManager.Instance.mapType = MapType.None;
        canSkip = false;
        Time.timeScale = 1;

        if (StageManager.Instance.playStage == StageManager.State.Stage1)
        {
            Stagename.text = "Mine";
        }
        //else if (StageManager.Instance.playStage == StageManager.State.stage2)
        //{
        //    Stagename.text = "Stage 2";
        //}
        //else if (StageManager.Instance.playStage == StageManager.State.stage3)
        //{
        //    Stagename.text = "Stage 3";
        //}
        SetScoreLimit();
        Invoke("ShowSuccess", 1f);
        Invoke("SetStar", 2f);
    }

    private void Start()
    {
        SoundManager.Instance.ResultBgm();
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (canSkip && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                UIManager.Instance.EnterLoadingKeyUI();
                canSkip = false;
            }
        }
        else
        {
            if (canSkip && Input.GetKeyDown(KeyCode.Space))
            {
                UIManager.Instance.EnterLoadingKeyUI();
                canSkip = false;
            }
        }
       
    }
    private void SetStar()
    {
        if (StageManager.Instance.playStage == StageManager.State.Stage1)
        {
            if (StageManager.Instance.totalMoney > limits[0].oneStarLimit)
            {
                if (StageManager.Instance.map1Star < 1) StageManager.Instance.map1Star = 1;
                StartCoroutine(BiggerStar(newStars[0], 0));
            }
            else
            {
                //canSkip = true;
            }
        }
        //else if (StageManager.Instance.playStage == StageManager.State.stage2)
        //{
        //    if (StageManager.Instance.totalMoney > limits[1].oneStarLimit)
        //    {
        //        if (StageManager.Instance.map2Star < 1) StageManager.Instance.map2Star = 1;
        //        StartCoroutine(BiggerStar(newStars[0], 1));
        //    }
        //    else
        //    {
        //        canSkip = true;
        //    }
        //}
        //else if (StageManager.Instance.playStage == StageManager.State.stage3)
        //{
        //    if (StageManager.Instance.totalMoney > limits[2].oneStarLimit)
        //    {
        //        if (StageManager.Instance.map3Star < 1) StageManager.Instance.map3Star = 1;
        //        StartCoroutine(BiggerStar(newStars[0], 2));
        //    }
        //    else
        //    {
        //        canSkip = true;
        //    }
        //}

    }
    IEnumerator BiggerStar(GameObject star, int i)
    {
        star.SetActive(true);
        float time = 0;
        while (time < 1f)
        {
            star.transform.localScale = Vector3.one * (1 + time);
            time += 0.01f;
            yield return new WaitForSeconds(0.005f);
        }
        SoundManager.Instance.PlayEffect("RoundResult_Star1");
        if (StageManager.Instance.totalMoney > limits[i].twoStarLimit)
        {
            if (i == 0)
            {
                if (StageManager.Instance.map1Star < 2) StageManager.Instance.map1Star = 2;
            }
            else if (i == 1)
            {
                if (StageManager.Instance.map2Star < 2) StageManager.Instance.map2Star = 2;
            }
            else if (i == 2)
            {
                if (StageManager.Instance.map3Star < 2) StageManager.Instance.map3Star = 2;
            }

            StartCoroutine(BiggerStar1(newStars[1], i));
        }
        else
        {
            //canSkip = true;
        }
    }
    IEnumerator BiggerStar1(GameObject star, int i)
    {
        star.SetActive(true);
        float time = 0;
        while (time < 1f)
        {
            star.transform.localScale = Vector3.one * (1 + time);
            time += 0.01f;
            yield return new WaitForSeconds(0.005f);
        }
        SoundManager.Instance.PlayEffect("RoundResult_Star2");
        if (StageManager.Instance.totalMoney > limits[i].threeStarLimit)
        {
            if (i == 0)
            {
                if (StageManager.Instance.map1Star < 3) StageManager.Instance.map1Star = 3;
            }
            else if (i == 1)
            {
                if (StageManager.Instance.map2Star < 3) StageManager.Instance.map2Star = 3;
            }
            else if (i == 2)
            {
                if (StageManager.Instance.map3Star < 3) StageManager.Instance.map3Star = 3;
            }
            StartCoroutine(BiggerStar2(newStars[2]));
        }
        else
        {
            //canSkip = true;
        }
    }
    IEnumerator BiggerStar2(GameObject star)
    {
        star.SetActive(true);
        float time = 0;
        while (time < 1f)
        {
            star.transform.localScale = Vector3.one * (1 + time);
            time += 0.01f;
            yield return new WaitForSeconds(0.005f);
        }
        SoundManager.Instance.PlayEffect("RoundResult_Star3");
        yield return null;
        //canSkip = true;
    }
    private void SetScoreLimit()
    {
        if (StageManager.Instance.playStage == StageManager.State.Stage1)
        {
            txtStars[0].text = limits[0].oneStarLimit.ToString();
            txtStars[1].text = limits[0].twoStarLimit.ToString();
            txtStars[2].text = limits[0].threeStarLimit.ToString();
            successMoney = limits[0].successMoney;
        }

        //else if (StageManager.Instance.playStage == StageManager.State.stage2)
        //{
        //    txtStars[0].text = limits[1].oneStarLimit.ToString();
        //    txtStars[1].text = limits[1].twoStarLimit.ToString();
        //    txtStars[2].text = limits[1].threeStarLimit.ToString();
        //    successMoney = limits[1].successMoney;
        //}
        //else if (StageManager.Instance.playStage == StageManager.State.stage3)
        //{
        //    txtStars[0].text = limits[2].oneStarLimit.ToString();
        //    txtStars[1].text = limits[2].twoStarLimit.ToString();
        //    txtStars[2].text = limits[2].threeStarLimit.ToString();
        //    successMoney = limits[2].successMoney;
        // }
    }

    private void ShowSuccess()
    {
        success.transform.GetChild(0).GetComponent<Text>().text = "배달된 주문 x " + StageManager.Instance.success;
        success.transform.GetChild(0).gameObject.SetActive(true);
        Invoke("ShowSuccessMoney", .5f);
        SoundManager.Instance.PlayEffect("RoundResults_Description");
    }
    private void ShowSuccessMoney()
    {
        success.transform.GetChild(1).GetComponent<Text>().text = (StageManager.Instance.successMoney).ToString();
        success.transform.GetChild(1).gameObject.SetActive(true);
        Invoke("ShowTip", .5f);
    }
    private void ShowTip()
    {
        tip.transform.GetChild(0).gameObject.SetActive(true);
        Invoke("ShowTipMoney", .5f);
        SoundManager.Instance.PlayEffect("RoundResults_Description");
    }
    private void ShowTipMoney()
    {
        tip.transform.GetChild(1).GetComponent<Text>().text = (StageManager.Instance.tipMoney).ToString();
        tip.transform.GetChild(1).gameObject.SetActive(true);
        Invoke("ShowFail", .5f);
    }
    private void ShowFail()
    {
        fail.transform.GetChild(0).GetComponent<Text>().text = "실패한 주문 x " + StageManager.Instance.fail.ToString();
        fail.transform.GetChild(0).gameObject.SetActive(true);
        Invoke("ShowFailMoney", .5f);
        SoundManager.Instance.PlayEffect("RoundResults_Description");

    }
    private void ShowFailMoney()
    {
        fail.transform.GetChild(1).GetComponent<Text>().text = "-" + StageManager.Instance.failMoney.ToString();
        fail.transform.GetChild(1).gameObject.SetActive(true);
        Invoke("ShowTotal", .5f);
    }
    private void ShowTotal()
    {
        total.transform.GetChild(0).gameObject.SetActive(true);
        Invoke("ShowTotalMoney", .5f);
    }
    private void ShowTotalMoney()
    {
        total.transform.GetChild(1).GetComponent<Text>().text = StageManager.Instance.totalMoney.ToString();
        total.transform.GetChild(1).gameObject.SetActive(true);
        SoundManager.Instance.PlayEffect("RoundResults_FinalScore");
        canSkip = true;

    }
}
