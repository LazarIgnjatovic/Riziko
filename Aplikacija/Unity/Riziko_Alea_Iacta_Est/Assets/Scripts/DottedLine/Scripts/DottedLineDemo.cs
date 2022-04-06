using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DottedLineDemo : MonoBehaviour
{
    [Serializable]
    public class Point
    {
        public Vector2 pointA;
        public Vector2 pointB;
    }
    public Point[] points;

    // Update is called once per frame
    void Update()
    {
        for(int i=0;i<points.Length;i++)
        {
            DottedLine.DottedLine.Instance.DrawDottedLine(points[i].pointA, points[i].pointB);
        }    
    }
}
