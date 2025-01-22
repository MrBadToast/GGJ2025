using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformPattern : MonoBehaviour
{
    [SerializeField] private Transform endPoint;
    public Vector3 EndPointPositon { get { return endPoint.position; } }

    public PatternDirection GetNextDirection()
    {
        if(endPoint.position.x <= 0)
        {
            return PatternDirection.Right;
        }
        else
        {
            return PatternDirection.Left;
        }
    }

}
