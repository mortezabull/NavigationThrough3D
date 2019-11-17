using UnityEngine;
using System.Collections;

public class FreeCameraPositionResetter : TouchButtonBase
{

    private FreeCameraHandler Sub_FreeCamera;
    private ModeSwitcher Sub_cameraSwitcher;

    void Start()
    {
        Sub_cameraSwitcher = GameObject.FindObjectOfType<ModeSwitcher>();

        Sub_FreeCamera = GameObject.FindObjectOfType<FreeCameraHandler>();

    }

    void OnTapButton(TapGesture gesture)
    {
        if (gesture.Selection == this.gameObject)
        {
            ShowPositiveButtonFeedback();
            Sub_FreeCamera.ResetTransform();
        }
    }
}
