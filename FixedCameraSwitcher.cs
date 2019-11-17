using UnityEngine;
using System.Collections;

public class FixedCameraSwitcher : TouchButtonBase
{
    private FixedCameraHandler Sub_FixedCameraHandler;
    private MessageHandler Sub_MessageHandler;
    void Start()
    {
        Sub_FixedCameraHandler = GameObject.FindObjectOfType<FixedCameraHandler>();
        Sub_MessageHandler = GameObject.FindObjectOfType<MessageHandler>();
        GUISprites Sub_GUISprites = GameObject.FindObjectOfType<GUISprites>();
        transform.position = Sub_GUISprites.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(0.75f * Screen.width, 0.945f * Screen.height, 1));
    }

    void OnTapButton(TapGesture gesture)
    {
        if (gesture.Selection == this.gameObject)
        {
            ShowPositiveButtonFeedback();
            if (!ModeSwitcher.isInTransition)
            {
                Sub_FixedCameraHandler.ActivateCamera(true);
            }
        }
        

    }
}
