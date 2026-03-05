using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraHandler : MonoBehaviour
{
    public CinemachineFreeLook mainCamera;     // refrence for Main Camera
    public CinemachineFreeLook aimingCamera;   // refrence fot Aiming Camera
    private CinemachineFreeLook currentCamera; // holds the currently used camera

    private void Start()
    {
        // at the start main camera is set as current
        currentCamera = mainCamera;

        UpdateCameraPriorities();
    }

    // procedure to handel the switch of cameras, to aiming
    public void SwitchToAimingCamera(bool isAiming)
    {
        if (isAiming == true)
        {
            SwitchCamera(aimingCamera);  
        }
        else
        {
            SwitchCamera(mainCamera);    
        }
    }

    // procedure that actually switches the cameras by changing their priority
    private void SwitchCamera(CinemachineFreeLook newCamera)
    {
        // If the new camera is already active, do nothing
        if (currentCamera == newCamera)
        {
            return;
        }
        // Sets the current camera's priority to 10 
        currentCamera.Priority = 10;

        // Switches to the camera that was passed in, by setting its priority to 20
        currentCamera = newCamera;
        currentCamera.Priority = 20;

        // updates the new camera priority
        UpdateCameraPriorities();
    }

    // continually updates the camera to maintain the new camera
    private void UpdateCameraPriorities()
    {
        // Sets the priorities for both cameras
        if (currentCamera == mainCamera)
        {
            mainCamera.Priority = 20;
            aimingCamera.Priority = 10;
        }
        else
        {
            mainCamera.Priority = 10;
            aimingCamera.Priority = 20;
        }
    }
}
