using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class SwitchParticle : MonoBehaviour
{
    //public Transform start;
    //public Transform end;
    public float speed = 50f;
    public float totalTime = 0.4f;
    public ParticleSystem spark;
    public ParticleSystem line;
    public ParticleSystem switchBurst;

    public void SwitchPlayer(Transform start,Transform end)
    {
        StartCoroutine(MoveTowardsTarget(start, end));
    }

    IEnumerator MoveTowardsTarget(Transform start,Transform end)
    {
        float elapsedTime = 0f;
        var sparking = spark.main;
        var lineing = line.main;
        lineing.loop = true;
        sparking.loop = true;
        spark.Play();
        line.Play();
        //while (Vector3.Distance(transform.position, end.position) > 0.3f) // 타겟에 가까워질 때까지 이동
        //{
        //    Vector3 direction = (end.position - transform.position).normalized;
        //    transform.Translate(direction * speed * Time.deltaTime, Space.World);
        //    yield return null; // 다음 프레임까지 대기
        //}
        while (elapsedTime < totalTime) // totalTime 동안 이동
        {
            // 현재 시간에 대한 보간값 계산
            float t = elapsedTime / totalTime;
            // 시작 지점에서 목표 지점까지 보간된 위치 계산
            Vector3 newPosition = Vector3.Lerp(start.transform.position, end.transform.position, t);
            // 이동
            transform.position = newPosition;

            // 경과 시간 업데이트
            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        SwitchEnd();
    }

    public void SwitchEnd()
    {
        var sparking = spark.main;
        var lineing = line.main;
        sparking.loop = false;
        lineing.loop = false;
        spark.Stop();
        line.Stop();
        switchBurst.Play();
        Invoke("die", 2f);
    }
    private void die()
    {
       Destroy(gameObject);
    }

}
