using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollMove : MonoBehaviour
{
    [SerializeField] private Vector2 scrollMinMax;
    [SerializeField,Range(0f,1f)] public float scrollValue;

    private void Update()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(scrollMinMax.x, scrollMinMax.y, scrollValue), transform.localPosition.z);
    }

}
