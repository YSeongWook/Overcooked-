using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut_Net : MonoBehaviour
{
    private Vector3 target;  // 이동할 목표 위치
    private Text GetText;  // Text 컴포넌트 참조
    private bool isCo = false;  // 코루틴 실행 여부 확인

    private void Awake()
    {
        TryGetComponent(out GetText);  // Text 컴포넌트를 가져옴
    }

    private void Start()
    {
        // Station 오브젝트의 위치를 화면 좌표로 변환하여 목표 위치 설정
        target = Camera.main.WorldToScreenPoint(FindObjectOfType<Station_Net>().transform.position) + new Vector3(0, 100, 0);
    }

    private void Update()
    {
        // 현재 위치에서 목표 위치로 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, target, 1.5f * Time.deltaTime);

        // 목표 위치에 도달하면 색상 변경 코루틴 시작
        if (Vector3.Distance(transform.position, target) < 10 && !isCo)
        {
            StartCoroutine(ColorChange_co());
        }
    }

    IEnumerator ColorChange_co()
    {
        isCo = true;  // 코루틴 실행 중 표시

        Color c = GetText.color;  // 텍스트 색상을 가져옴
        c.a -= 0.05f;  // 알파 값을 줄여서 투명하게 만듦
        GetText.color = c;  // 변경된 색상을 적용

        if (c.a < 0.01f)  // 투명도가 거의 0이 되면
        {
            Destroy(gameObject);  // 게임 오브젝트를 삭제
        }

        yield return new WaitForSeconds(0.005f);  // 잠시 대기

        isCo = false;  // 코루틴 실행 종료 표시
    }
}
