using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    //private PlayerInput input;

    private CinemachineFreeLook FreeLook { get; set; }

    void OnAwake()
    {
        //input = new PlayerInput();
        //input.CameraControls.MoveView.performed += ctx => Debug.Log(ctx.ReadValueAsObject());
    }
    void Start()
    {
        FreeLook = GetComponent<CinemachineFreeLook>();
        // FreeLook.m_YAxis.Value = mouse y value
    }
}
