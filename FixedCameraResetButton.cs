using UnityEngine;
using System.Collections;

public class FixedCameraResetButton : TouchButtonBase
{

    internal FixedCameraHandler Sub_FixedCameraHandler;
    void Start()
    {

        Sub_FixedCameraHandler = GameObject.FindObjectOfType<FixedCameraHandler>();

    }

    void OnDoubleTapReset(TapGesture gesture)
    {
        Sub_FixedCameraHandler.ResetTransform();
    }

    void OnTapButton(TapGesture gesture)
    {
        ShowPositiveButtonFeedback();
        Sub_FixedCameraHandler.ResetTransform();
    }
}
