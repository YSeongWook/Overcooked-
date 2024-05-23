using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AniTEST : MonoBehaviour
{
    // 자식 객체의 애니메이터 컴포넌트를 저장할 배열
    private Animator[] childAnimators;
    void Awake()
    {
        // 모든 자식 객체의 애니메이터 컴포넌트를 가져옴
        childAnimators = GetComponentsInChildren<Animator>();
    }
    void Start()
    {
        // 모든 자식 객체의 애니메이터 컴포넌트를 가져옴
        childAnimators = GetComponentsInChildren<Animator>();
    }
    void OnEnable()
    {
        // 모든 자식 객체의 애니메이터 컴포넌트를 가져옴
        childAnimators = GetComponentsInChildren<Animator>();
    }

    void Update()
    {
        // 특정 조건을 만족하면 모든 자식 객체의 애니메이션을 실행시킴
        if (Input.GetKeyDown(KeyCode.Space)) // 예시로 스페이스바를 누를 때 실행하도록 설정
        {
            PlayChildAnimations();
        }
    }

    public void PlayChildAnimations()
    {
        foreach (Animator animator in childAnimators)
        {
            animator.SetTrigger("flip");
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Bus"))
    //    {
    //        foreach (Animator animator in childAnimators)
    //        {
    //            animator.SetTrigger("flip");
    //        }
    //    }
    //}

}
