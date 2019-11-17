 using UnityEngine;
using System.Collections;

public class WallHandler : MonoBehaviour
{
    private ModeSwitcher Sub_cameraSwitcher;
    private Transform Sub_CenterOfRotation;
    private Color OpaqueColor;
    private Color SemiTransparentColor;
    private Color CompletelyTransparentColor;

    private MeshFilter Comp_MeshFilter;
    private float time = 1;
    void Start()
    {
        Sub_cameraSwitcher = GameObject.FindObjectOfType<ModeSwitcher>();
        Sub_CenterOfRotation = Sub_cameraSwitcher.BirdEyeCamera.GetComponent<BirdEyeViewer>().CenterOfRotation.transform;
        if (GetComponent<Renderer>().material.HasProperty("_Color"))
        {
            OpaqueColor = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, 1);
            SemiTransparentColor = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, 0.5f);
            CompletelyTransparentColor = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, 0); 
        }
        Comp_MeshFilter = GetComponent<MeshFilter>();

    }

    public void YourOn()
    {
        switch (ModeSwitcher.CurrentCameraState)
        {
            case ModeSwitcher.cameraTypes.MainCamera:
                {
                    //StartCoroutine("ChangeTransparency", SemiTransparentColor);
                    if (GetComponent<Renderer>().material.HasProperty("_Color"))
                    {
                        InvokeRepeating("TransparencyHandler", 1f, 0.5f); 
                    }
                    break;
                }
            case ModeSwitcher.cameraTypes.FreeCamera:
                {
                    if (GetComponent<Renderer>().material.HasProperty("_Color"))
                    {
                        StartCoroutine("ChangeTransparency", OpaqueColor); 
                    }
                    CancelInvoke("TransparencyHandler");
                    break;
                }
            case ModeSwitcher.cameraTypes.WalkingCamera:
                {
                    if (GetComponent<Renderer>().material.HasProperty("_Color"))
                    {
                        StartCoroutine("ChangeTransparency", OpaqueColor); 
                    }
                    CancelInvoke("TransparencyHandler");
                    break;
                }
            case ModeSwitcher.cameraTypes.PresetedCameras:
                {
                    if (GetComponent<Renderer>().material.HasProperty("_Color"))
                    {
                        StartCoroutine("ChangeTransparency", OpaqueColor); 
                    }
                    CancelInvoke("TransparencyHandler");
                    break;
                }
            case ModeSwitcher.cameraTypes.OverallCamera:
                {
                    if (GetComponent<Renderer>().material.HasProperty("_Color"))
                    {
                        StartCoroutine("ChangeTransparency", SemiTransparentColor); 
                    }
                    CancelInvoke("TransparencyHandler");
                    break;
                }
        }
    }

    IEnumerator ChangeTransparency(Color toWhichColor)
    {
        float elapsedTime = 0.0f;
        Color fromWhichColor = GetComponent<Renderer>().material.color;
        time = Mathf.Abs(fromWhichColor.a - toWhichColor.a);
        while (elapsedTime < time)
        {
            GetComponent<Renderer>().material.color = Color.Lerp(fromWhichColor, toWhichColor, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return 0;
    }

    void TransparencyHandler()
    {
        if (ModeSwitcher.CurrentCameraState == ModeSwitcher.cameraTypes.MainCamera)
        {
            if (Vector3.Distance(Sub_CenterOfRotation.position, Sub_cameraSwitcher.BirdEyeCamera.transform.position) < Vector3.Distance(transform.position, Sub_cameraSwitcher.BirdEyeCamera.transform.position))
            //if (Mathf.Abs(Vector3.Angle(Sub_CenterOfRotation.position, Sub_cameraSwitcher.BirdEyeCamera.transform.position) - Vector3.Angle(Sub_CenterOfRotation.position, transform.position)) > 10.0)
            {
                if (GetComponent<Renderer>().material.HasProperty("_Color"))
                {
                    StartCoroutine("ChangeTransparency", OpaqueColor); 
                }
            }
            else
            {
                if (GetComponent<Renderer>().material.HasProperty("_Color"))
                {
                    StartCoroutine("ChangeTransparency", CompletelyTransparentColor); 
                }
            }
        }
    }

    public void SetToTransparent()
    {
        if (GetComponent<Renderer>().material.HasProperty("_Color"))
        {
            StartCoroutine("ChangeTransparency", CompletelyTransparentColor); 
        }
        CancelInvoke("TransparencyHandler");
    }

    void OnDestroy()
    {
        if (GameObject.FindObjectOfType<ModeSwitcher>() != null)
        {
            GameObject.FindObjectOfType<ModeSwitcher>().GetComponent<ModeSwitcher>().delegate_WallHandler_YourOn -= this.YourOn;
        }
        
    }
}
