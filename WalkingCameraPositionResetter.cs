using UnityEngine;
using System.Collections;

public class WalkingCameraPositionResetter : TouchButtonBase
{
    internal GameObject Sub_WalkingCamera;
    internal ModeSwitcher Sub_CameraSwitcher;
    private WalkingCamera Sub_Comp_WalkingCamera;


    void YourOn()
    {
        if (Sub_CameraSwitcher == null)
        {
            Sub_CameraSwitcher = GameObject.FindObjectOfType<ModeSwitcher>();
            Sub_WalkingCamera = Sub_CameraSwitcher.Walking_Camera;
            Sub_Comp_WalkingCamera = GameObject.FindObjectOfType<WalkingCamera>();
        }
    }

    void OnTapButton(TapGesture gesture)
    {
        if (gesture.Selection == this.gameObject)
        {
            ShowPositiveButtonFeedback();
            if (Sub_WalkingCamera == null)
            {
                Sub_Comp_WalkingCamera = GameObject.FindObjectOfType<WalkingCamera>(); 
            }
            Sub_Comp_WalkingCamera.ResetTransform();
            Debug.Log("Walking Camera Resetted");
        }
    }

}
