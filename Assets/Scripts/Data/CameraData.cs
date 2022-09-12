using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraData : ScriptableObject
{
    [Range(10f, 120f)] public float m_FOV = 60;
    public Vector3 m_OriginOffset = new Vector2(0, 0);
    public Vector3 m_Angle = new Vector3(60, 0, 0);
    [Range(0f, 360f)] public float m_RotateAround = 0;
    [Range(1f, 200f)] public float m_Distance = 50;


}
