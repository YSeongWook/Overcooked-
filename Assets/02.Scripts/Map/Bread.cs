using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bread : MonoBehaviour
{

    Animator ani;
    public bool die;
    void Start()
    {
        die = false;
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CompareTag("Bus");
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Bus")&&!die)
        {
            // 충돌한 오브젝트의 위치를 얻기
            Vector3 targetPosition = other.transform.position;
            // 현재 위치에서 충돌한 오브젝트를 바라보도록 회전 구하기
            Vector3 direction = targetPosition - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            // 회전 적용
            transform.rotation = targetRotation;
            ani.SetTrigger("Die");
            die= true;
            SoundManager.Instance.PlayEffect("boing");
            StartCoroutine(ReSpawn());
        }
    }

    IEnumerator ReSpawn()
    {
        yield return new WaitForSecondsRealtime(2f);
        ani.SetTrigger("Spawn");
        die = false;
    }
}
