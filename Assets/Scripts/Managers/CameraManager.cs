using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VoodooPackages.Tech;

public class CameraManager : SingletonMB<CameraManager>
{
    private const float TRANSITION_DURATION = 1;

    [Header("References")]
    [SerializeField] private Transform m_Origin;
    [SerializeField] private Camera m_MainCamera;
    [SerializeField] private List<Camera> m_OtherCameras;

    [Header("Settings")]
    [SerializeField] private CameraData m_MenuCameraData;
    [SerializeField] private CameraData m_GameCameraData;
    [SerializeField] private Transform m_Target;

    private float m_FOV = 60;
    private float m_TargetFOV = 60;
    private Vector3 m_OriginOffset = new Vector2(0, 0);
    private Vector3 m_TargetOriginOffset = new Vector3(0, 0, 0);
    private Vector3 m_Angle = new Vector3(60, 0, 0);
    private float m_Distance = 50;
    private float m_TargetDistance = 50;
    private float m_RotateAround = 0;
    private float m_TargetRotateAround = 0;
    private IEnumerator recalculatePositionRoutine;
    private CameraData m_CurrentCameraData;

    public Camera MainCamera => m_MainCamera;
    public float Distance => m_Distance;

    private void OnEnable()
    {
        /*FSMManager.onGamePhaseStarted += OnGamePhaseStarted;*/
    }

    private void OnDisable()
    {
        /*FSMManager.onGamePhaseStarted -= OnGamePhaseStarted;*/
    }

    private void Start()
    {
        SetCameraData(m_MenuCameraData, true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SetCameraData(m_MenuCameraData);

        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SetCameraData(m_GameCameraData);

        }

    }

    private void LateUpdate()
    {
        #if UNITY_EDITOR
        //SetCameraData(m_GameCameraData[m_CurrentCameraData]);
        #endif
        
     

        RecalculatePosition();
    }

    /*private void OnGamePhaseStarted(GamePhase _Phase)
    {
        switch(_Phase)
        {
            case GamePhase.MAIN_MENU:
                SetCameraData(m_GameCameraData[m_CurrentCameraData]);
                break;
        }
    }*/

    private void SetCameraData(CameraData _Data, bool _SkipTransition = false)
    {
        if (m_CurrentCameraData == _Data)
            return;

        m_CurrentCameraData = _Data;
        m_TargetFOV = _Data.m_FOV;
        m_TargetOriginOffset = _Data.m_OriginOffset;
        m_TargetDistance = _Data.m_Distance;
       
        m_Angle = _Data.m_Angle;
        m_TargetRotateAround = _Data.m_RotateAround;

        UpdateCamera(_SkipTransition);
    }

    private void UpdateCamera(bool _SkipTransition)
    {
        if (_SkipTransition)
        {
            m_FOV = m_TargetFOV;
            m_Distance = m_TargetDistance;
            m_RotateAround = m_TargetRotateAround;
            m_OriginOffset = m_TargetOriginOffset;
            RecalculatePosition();
        }
        else
        {
            if (recalculatePositionRoutine != null) StopCoroutine(recalculatePositionRoutine);
            recalculatePositionRoutine = RecalculatePositionRoutine();
            StartCoroutine(recalculatePositionRoutine);
        }  
    }

    private void RecalculatePosition()
    {
        MainCamera.fieldOfView = m_FOV;

        for (int i = 0; i < m_OtherCameras.Count; i++)
        {
            m_OtherCameras[i].fieldOfView = m_FOV;
        }
        
        m_Origin.transform.rotation = Quaternion.Euler(0, m_RotateAround, 0);
        m_Origin.transform.position = m_Target.position;

        MainCamera.transform.localRotation = Quaternion.Euler(m_Angle);
        MainCamera.transform.localPosition =
            new Vector3(
                0,
                m_Distance * Mathf.Sin(m_Angle.x * Mathf.Deg2Rad),
                -m_Distance * Mathf.Cos(m_Angle.x * Mathf.Deg2Rad)
            )
            + m_OriginOffset;
    }
    
    private IEnumerator RecalculatePositionRoutine()
    {
        float timer = 0;
        float lerp = 0;
        Vector3 startOriginPosition = m_Origin.transform.position;
        Vector3 endOriginPosition = m_Target.position;
        Quaternion startLocalRotation = MainCamera.transform.localRotation;
        Quaternion endLocalRotation = Quaternion.Euler(m_Angle);
        float startFov = m_FOV;
        float endFov = m_TargetFOV;
        float startRotateAround = m_RotateAround;
        float endRotateAround = m_TargetRotateAround;
        float startDistance = m_Distance;
        float endDistance = m_TargetDistance;
        Vector3 startOffset = m_OriginOffset;
        Vector3 endOffset = m_TargetOriginOffset;

        while (timer < 1)
        {
            lerp = CalculationEasing.EaseInOutSine(0, 1, timer);

            // FOV Lerp
            m_FOV = Mathf.Lerp(startFov, endFov, lerp);

            MainCamera.fieldOfView = m_FOV;

            for (int i = 0; i < m_OtherCameras.Count; i++)
            {
                m_OtherCameras[i].fieldOfView = m_FOV;
            }

            // Distance Lerp
            m_Distance = Mathf.Lerp(startDistance, endDistance, lerp);

            //Position Lerp
            m_RotateAround = Mathf.Lerp(startRotateAround, endRotateAround, lerp);
            m_Origin.transform.rotation = Quaternion.Euler(0, m_RotateAround, 0);
            m_Origin.transform.position = Vector3.Lerp(startOriginPosition, endOriginPosition, lerp);

            m_OriginOffset = Vector3.Lerp(startOffset, endOffset, lerp);

            MainCamera.transform.localRotation = Quaternion.Lerp(startLocalRotation, endLocalRotation, lerp);
            MainCamera.transform.localPosition =
               new Vector3(
                  0,
                  m_Distance * Mathf.Sin(m_Angle.x * Mathf.Deg2Rad),
                  -m_Distance * Mathf.Cos(m_Angle.x * Mathf.Deg2Rad)
                  )
               + m_OriginOffset;


            timer += Time.deltaTime / TRANSITION_DURATION;
            yield return new WaitForEndOfFrame();
        }

        
        m_FOV = m_TargetFOV;
        m_Distance = m_TargetDistance;
        m_RotateAround = m_TargetRotateAround;
        m_OriginOffset = m_TargetOriginOffset;
        
        MainCamera.fieldOfView = endFov;

        for (int i = 0; i < m_OtherCameras.Count; i++)
        {
            m_OtherCameras[i].fieldOfView = endFov;
        }

        m_Origin.transform.rotation = Quaternion.Euler(0, endRotateAround, 0);
        m_Origin.transform.position = endOriginPosition;
        
        MainCamera.transform.localRotation = endLocalRotation;
        MainCamera.transform.localPosition =
           new Vector3(
              0,
              m_Distance * Mathf.Sin(m_Angle.x * Mathf.Deg2Rad),
              -m_Distance * Mathf.Cos(m_Angle.x * Mathf.Deg2Rad)
              )
           + endOffset;
    }
}
