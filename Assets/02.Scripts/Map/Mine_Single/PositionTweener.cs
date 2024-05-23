using UnityEngine;
using DG.Tweening;

public class PositionTweener : MonoBehaviour
{
    public Transform cameraTransform; // 인스펙터에서 설정할 카메라 참조
    public float shakeDuration = 0.5f; // 흔들림 지속 시간
    public float shakeStrength = 0.3f; // 흔들림 강도
    public int shakeVibrato = 10; // 흔들림 진동 횟수
    private Vector3 originalCameraPosition; // 카메라의 원래 위치를 저장

    void Start()
    {
        // 카메라의 원래 위치 저장
        if (cameraTransform != null)
        {
            originalCameraPosition = cameraTransform.localPosition;
        }

        // 시퀀스 생성
        Sequence sequence = DOTween.Sequence();

        // 15초 동안 대기 후 시작
        sequence.AppendInterval(15f);

        // 첫 번째 트윈 (-0.048에서 -0.108로 이동)과 카메라 흔들림 동시에 시작
        sequence.AppendCallback(() => ShakeCamera());
        sequence.Append(transform.DOLocalMoveX(-0.108f, 5f).SetEase(Ease.InOutQuad));

        // 15초 동안 대기 후 다시 이동과 흔들림
        sequence.AppendInterval(15f);
        sequence.AppendCallback(() => ShakeCamera());
        sequence.Append(transform.DOLocalMoveX(-0.048f, 5f).SetEase(Ease.InOutQuad));

        // 시퀀스를 무한 반복
        sequence.SetLoops(-1, LoopType.Restart);
    }

    void ShakeCamera()
    {
        if (cameraTransform != null)
        {
            cameraTransform.DOKill(); // 이전 트윈이 있으면 중지
            cameraTransform.localPosition = originalCameraPosition; // 원래 위치로 복귀
            cameraTransform.DOShakePosition(shakeDuration, new Vector3(shakeStrength, 0, 0), shakeVibrato, 0, false, true).OnComplete(() =>
            {
                cameraTransform.localPosition = originalCameraPosition; // 흔들림이 끝나면 원래 위치로 복귀
            });
        }
    }
}
