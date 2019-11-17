using UnityEngine;
using System.Collections;

public class MainCameraPositionResetter : TouchButtonBase
{

    internal GameObject Sub_MainCamera;
    internal ModeSwitcher Sub_CameraSwitcher;

    void YourOn()
    {
        if (Sub_CameraSwitcher == null)
        {
            Sub_CameraSwitcher = GameObject.FindObjectOfType<ModeSwitcher>();
        }
        Sub_MainCamera = Sub_CameraSwitcher.BirdEyeCamera;
    }

    void OnTapButton(TapGesture gesture)
    {
        if (gesture.Selection == this.gameObject)
        {
            ShowPositiveButtonFeedback();
            Sub_MainCamera.SendMessage("ResetTransform");   
        }
    }
}
