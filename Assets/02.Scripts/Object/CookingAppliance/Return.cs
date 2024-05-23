using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Return : MonoBehaviour
{
    public List<GameObject> returnPlates = new List<GameObject>();

    private void Update()
    {
        if (returnPlates.Count != 0)
        {
            GetComponent<ObjectHighlight>().onSomething = true;
        }
        else
        {
            GetComponent<ObjectHighlight>().onSomething = false;
        }
    }

    public Vector3 SetPosition()
    {
        Vector3 spawnPos = Vector3.zero;
        if (returnPlates.Count == 1)
        {
            spawnPos = new Vector3(0.1278f, -0.0711f, -0.0261f);
        }
        else if (returnPlates.Count == 2)
        {
            spawnPos = new Vector3(0.1278f, -0.0704f, -0.0261f);
        }
        else if (returnPlates.Count == 3)
        {
            spawnPos = new Vector3(0.1278f, -0.06976f, -0.0261f);
        }
        else if (returnPlates.Count == 4)
        {
            spawnPos = new Vector3(0.1278f, -0.06915f, -0.0261f);
        }
        return spawnPos;
    }

    public GameObject TakePlate()
    {
        GameObject returnPlate = returnPlates[returnPlates.Count - 1];
        returnPlates.RemoveAt(returnPlates.Count - 1); //list에서 마지막 오브젝트 삭제
        return returnPlate;
    }
}
