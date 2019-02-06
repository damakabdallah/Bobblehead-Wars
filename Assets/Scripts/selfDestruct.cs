using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selfDestruct : MonoBehaviour
{
    public float destructTime = 3f;
    public void Instantiate()
    {
        Invoke("SelfDestruct", destructTime);
    }
    private void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
