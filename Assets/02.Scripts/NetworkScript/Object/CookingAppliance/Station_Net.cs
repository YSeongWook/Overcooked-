using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station_Net : MonoBehaviour
{
    float x = 5.5f; // x축 이동 속도를 설정 (텍스처 오프셋 이동 속도)
    float xOffset; // x축 오프셋을 저장할 변수

    float y = 5.5f; // y축 이동 속도를 설정 (텍스처 오프셋 이동 속도)
    float yOffset; // y축 오프셋을 저장할 변수

    void Update()
    {
        // xOffset을 시간에 따라 감소시킴 (x축 방향으로 텍스처 이동)
        xOffset -= (Time.deltaTime * x) / 10f;
        // Renderer 컴포넌트에서 세 번째 소재(material)의 주 텍스처 오프셋을 설정
        gameObject.GetComponent<Renderer>().materials[3].mainTextureOffset = new Vector2(xOffset, 0);
        // MeshRenderer 컴포넌트에서 세 번째 소재(material)의 주 텍스처 오프셋을 설정
        gameObject.GetComponent<MeshRenderer>().materials[3].mainTextureOffset = new Vector2(xOffset, 0);
        // MeshRenderer 컴포넌트에서 세 번째 소재(material)의 "_MainTex" 속성의 텍스처 오프셋을 설정
        gameObject.GetComponent<MeshRenderer>().materials[3].SetTextureOffset("_MainTex", new Vector2(xOffset, 0));

        // yOffset을 시간에 따라 감소시킴 (y축 방향으로 텍스처 이동)
        yOffset -= (Time.deltaTime * y) / 10f;
        // Renderer 컴포넌트에서 세 번째 소재(material)의 주 텍스처 오프셋을 설정
        gameObject.GetComponent<Renderer>().materials[3].mainTextureOffset = new Vector2(0, yOffset);
        // MeshRenderer 컴포넌트에서 세 번째 소재(material)의 주 텍스처 오프셋을 설정
        gameObject.GetComponent<MeshRenderer>().materials[3].mainTextureOffset = new Vector2(0, yOffset);
        // MeshRenderer 컴포넌트에서 세 번째 소재(material)의 "_MainTex" 속성의 텍스처 오프셋을 설정
        gameObject.GetComponent<MeshRenderer>().materials[3].SetTextureOffset("_MainTex", new Vector2(0, yOffset));
    }
}
