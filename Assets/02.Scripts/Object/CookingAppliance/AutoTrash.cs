using System.Collections;  // IEnumerator 사용을 위해 필요함.
using System.Collections.Generic;  // 일반적인 컬렉션 사용을 위해 필요함.
using UnityEngine;  // Unity 엔진 관련 클래스와 메서드를 사용하기 위해 필요함.

public class AutoTrash : MonoBehaviour  // AutoTrash 클래스는 MonoBehaviour를 상속받아 Unity의 컴포넌트로 사용됨.
{
    private void OnTriggerEnter(Collider other)  // 다른 콜라이더가 이 오브젝트의 콜라이더와 충돌했을 때 호출됨.
    {
        // 충돌한 오브젝트가 'Ingredient' 태그를 가졌는지 확인하고,
        // 그 오브젝트의 부모가 PlayerController나 Player2Controller의 자식이 아닌지 확인함.
        if (other.CompareTag("Ingredient") && (!other.transform.parent.IsChildOf(FindObjectOfType<PlayerInteractController>().transform) && !other.transform.parent.IsChildOf(FindObjectOfType<Player2InteractController>().transform)))
        {
            // 없어지는 소리 효과 재생 (ScaleSmaller 코루틴 호출 대신).
            //StartCoroutine(ScaleSmaller(other));
            SoundManager.Instance.PlayEffect("bin");  // SoundManager를 통해 "bin" 효과음 재생.
            Destroy(other.transform.parent.parent.gameObject);  // 오브젝트의 부모의 부모 오브젝트를 삭제함.
        }
    }

    IEnumerator ScaleSmaller(Collider other)  // 오브젝트를 점점 작아지게 하는 코루틴.
    {
        if (other == null)  // other가 null인지 확인. null이면 null을 반환하고 종료.
        {
            yield return null;
        }
        else if (other.transform.parent.gameObject.GetComponent<Rigidbody>() != null)  // 부모 오브젝트에 Rigidbody가 있는지 확인.
        {
            // 부모 오브젝트의 Rigidbody를 가져와 모든 움직임을 멈춤.
            other.transform.parent.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            // 부모 오브젝트의 MeshCollider를 트리거 모드로 변경.
            other.transform.parent.GetComponent<MeshCollider>().isTrigger = true;
            Vector3 pos = transform.position;  // 현재 오브젝트의 위치를 pos 변수에 저장.

            // Ingredient 컴포넌트가 존재하고, 그 type이 Fish인지 확인.
            if (other.GetComponent<Ingredient>().type == Ingredient.IngredientType.Fish)
            {
                // 부모 오브젝트의 로컬 위치를 fishLocalPos로 설정.
                other.transform.parent.localPosition = other.GetComponent<Ingredient>().fishLocalPos;
            }
            // Ingredient 컴포넌트가 존재하고, 그 hand 타입이 Shrimp인지 확인.
            else if (other.GetComponent<Ingredient>().type == Ingredient.IngredientType.Shrimp)
            {
                // 부모 오브젝트의 로컬 위치를 shrimpLocalPos로 설정.
                other.transform.parent.localPosition = other.GetComponent<Ingredient>().shrimpLocalPos;
            }
            // 부모의 부모 오브젝트의 위치를 현재 오브젝트의 위치로 설정.
            other.transform.parent.parent.position = pos;

            // 부모의 부모 오브젝트의 x축 스케일이 0보다 클 동안 반복.
            while (other.transform.parent.parent.localScale.x > 0)
            {
                Vector3 scale = other.transform.parent.parent.localScale;  // 현재 스케일을 저장.
                scale.x -= 0.05f;  // x축 스케일을 줄임.
                scale.y -= 0.05f;  // y축 스케일을 줄임.
                scale.z -= 0.05f;  // z축 스케일을 줄임.
                other.transform.parent.parent.localScale = scale;  // 스케일을 적용.
                yield return new WaitForSeconds(0.01f);  // 0.01초 대기.
            }
            Destroy(other.transform.parent.parent.gameObject);  // 오브젝트의 부모의 부모 오브젝트를 삭제함.
        }
    }
}
