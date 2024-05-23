using UnityEngine;
using TMPro;
using DG.Tweening;

public class BattleResultText : MonoBehaviour
{
    public GameObject redVictory;
    public GameObject blueVictory;

    public TextMeshProUGUI redTipNum;
    public TextMeshProUGUI blueTipNum;

    public TextMeshProUGUI redFailMenuNum;
    public TextMeshProUGUI blueFailMenuNum;

    public TextMeshProUGUI redFailMenuTotal;
    public TextMeshProUGUI blueFailMenuTotal;

    public TextMeshProUGUI redTotalNum;
    public TextMeshProUGUI blueTotalNum;

    public int targetRedTipNum = 1;
    public int targetBlueTipNum = 2;

    public int targetRedFailMenuNum = 3;
    public int targetBlueFailMenuNum = 2;

    public int targetRedFailMenuTotal = -90;
    public int targetBlueFailMenuTotal = -60;

    public int targetRedTotalNum = -90;
    public int targetBlueTotalNum = 43;

    public float duration = 2f;

    private void Start()
    {
        TargetTipNum();
    }

    public void TargetTipNum()
    {
        Sequence mySequence = DOTween.Sequence();

        mySequence.Append(
            DOTween.To(() => 0, x => redTipNum.text = x.ToString(), targetRedTipNum, 0.5f).SetEase(Ease.Linear)
        ).Join(
            DOTween.To(() => 0, x => blueTipNum.text = x.ToString(), targetBlueTipNum, 0.5f).SetEase(Ease.Linear)
        ).OnComplete(TargetFailMenuNum);
    }

    public void TargetFailMenuNum()
    {
        Sequence mySequence = DOTween.Sequence();

        mySequence.Append(
            DOTween.To(() => 0, x => redFailMenuNum.text = x.ToString(), targetRedFailMenuNum, 0.5f).SetEase(Ease.Linear)
        ).Join(
            DOTween.To(() => 0, x => blueFailMenuNum.text = x.ToString(), targetBlueFailMenuNum, 0.5f).SetEase(Ease.Linear)
        ).OnComplete(TargetFailMenuTotal);
    }

    public void TargetFailMenuTotal()
    {
        Sequence mySequence = DOTween.Sequence();

        mySequence.Append(
            DOTween.To(() => 0, x => redFailMenuTotal.text = x.ToString(), targetRedFailMenuTotal, duration).SetEase(Ease.Linear)
        ).Join(
            DOTween.To(() => 0, x => blueFailMenuTotal.text = x.ToString(), targetBlueFailMenuTotal, duration).SetEase(Ease.Linear)
        ).OnComplete(TargetTotalNum);
    }

    public void TargetTotalNum()
    {
        Sequence mySequence = DOTween.Sequence();

        mySequence.Append(
            DOTween.To(() => 0, x => redTotalNum.text = x.ToString(), targetRedTotalNum, duration).SetEase(Ease.Linear)
        ).Join(
            DOTween.To(() => 0, x => blueTotalNum.text = x.ToString(), targetBlueTotalNum, duration).SetEase(Ease.Linear)
        ).OnComplete(VictoryTeam);
    }

    public void VictoryTeam()
    {
        //Debug.Log(targetRedTotalNum > targetBlueTotalNum);
        //Debug.Log(targetRedTotalNum < targetBlueTotalNum);

        if (targetRedTotalNum > targetBlueTotalNum) redVictory.SetActive(true);
        else if (targetRedTotalNum < targetBlueTotalNum) blueVictory.SetActive(true);
    }
}
