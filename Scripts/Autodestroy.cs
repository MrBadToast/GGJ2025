using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autodestroy : MonoBehaviour
{
    [SerializeField] float destroyTime = 2f;

    private void Start()
    {
        StartCoroutine(Cor_Destroy());
    }

    IEnumerator Cor_Destroy()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }
}
