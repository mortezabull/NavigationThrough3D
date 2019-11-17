using UnityEngine;
using System.Collections;

//[AddComponentMenu("Camera-Control/Mouse Look")]
public class FreeCameraHandler : MonoBehaviour
{
    #region Variable Declaration
    public ModeSwitcher.cameraTypes CameraType = ModeSwitcher.cameraTypes.FreeCamera;
    public float MovementSpeed = 0.000001f;
    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    public float panSensitivity = .1f;

    private float rotationY = 0F;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private Vector3 temp_Vector3;
    private bool startMovingForward;
    private bool startMovingBackWard;

    private GameObject BoundingBox;
    private RaycastHit RayHitInfo;

    [HideInInspector]
    public float ForwardSpeed = 0.02f;
    private float BaseForwardSpeed = 0.02f;

    private float time = 1;
    private bool isUndergoingDoubleTap = false;

    private Transform NewTransform;
    private Transform InitialTransform;

    private Vector3 NewPosition;
    private Quaternion NewRotation;



    #endregion

    void Update()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        if (axes == RotationAxes.MouseXAndY)
        {
            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            //transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        }
        #region Translate Approach
        if (!ModeSwitcher.isInTransition)
        {
            if (startMovingForward)
            {
                transform.position = Vector3.MoveTowards(transform.position, transform.position + (ForwardSpeed * transform.forward), Time.deltaTime * 200);
            }
            if (startMovingBackWard)
            {
                transform.position = Vector3.MoveTowards(transform.position, transform.position - (ForwardSpeed * transform.forward), Time.deltaTime * 200);
            }
        }
        #endregion

        #region RigidBody Approach
        //if (startMovingForward)
        //{
        //    transform.rigidbody.velocity = 100000 * ForwardSpeed * transform.forward;
        //}
        //else if (!startMovingForward)
        //{
        //    transform.rigidbody.velocity = Vector3.zero;
        //}
        //if (startMovingBackWard)
        //{
        //    transform.rigidbody.velocity = -100000 * ForwardSpeed * transform.forward;
        //}
        //else if (!startMovingBackWard)
        //{
        //    transform.rigidbody.velocity = Vector3.zero;
        //}
        #endregion
    }

    void Start()
    {

        //Setting Sensitivity according to platform
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
                {
                    break;
                }
            case RuntimePlatform.Android:
                {
                    sensitivityX *= 1.5f;
                    sensitivityY *= 1.5f;
                    BaseForwardSpeed *= 1.5f;
                    break;
                }

        }
        //Setting initial Values
        startMovingBackWard = false;
        startMovingForward = false;
        ForwardSpeed = BaseForwardSpeed;

        //Getting Initial Position and Rotation
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        //Getting Initial Transform
        if (NewTransform == null)
        {
            //InitialTransform = transform;
            initialPosition = transform.position;
            initialRotation = transform.rotation;

            //NewTransform = InitialTransform;
            NewPosition = transform.position;
            NewRotation = transform.rotation;
        }

        //Getting Refrence to Bounding Box
       // BoundingBox = GameObject.Find("RoomBoundingBox");
        BoundingBox = GameObject.FindGameObjectWithTag("RoomBoundingBox");

        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
    }

    #region Gesture Reconginition

    //Gesture used to drag Camera around the room
    void OnDrag(DragGesture gesture)
    {
            if (!ModeSwitcher.isInTransition)
            {
                if (!isUndergoingDoubleTap)
                {

                    // the following line does not let the camera move 
                    //if (!MediaHandler.isUndergoingMediaTransfer)

                    //if (gesture.StartSelection == null || gesture.StartSelection.name == "RoomBoundingBox")
                    if (gesture.StartSelection == null || !GUISprites.isOnAGUIElement)
                    {
                        if (gesture.State == GestureRecognitionState.InProgress)
                        {
                            float rotationX = transform.localEulerAngles.y + gesture.DeltaMove.x * sensitivityX;

                            rotationY += gesture.DeltaMove.y * sensitivityY;
                            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
                            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
                        }
                    }

                }
            }

    }



    //Gesture used to drag Camera around the room in backward
    void OnDragTwoFinger(DragGesture gesture)
    {
        if (!isUndergoingDoubleTap)
        {
            float rotationX = transform.localEulerAngles.y + gesture.DeltaMove.x * sensitivityX;

            rotationY += gesture.DeltaMove.y * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        }
    }


    //Gesture used to Align Camera to touched point
    void OnTapDouble(TapGesture gesture)
    {
        if (!GUISprites.isOnAGUIElement)
        {
            if (!ModeSwitcher.isInTransition)
            {
                AlignCameraWithPoint(this.GetComponent<Camera>(), gesture.StartPosition);
            }
        }
    }

    //Gesture used to start moving forward
    void OnLongPress(LongPressGesture gesture)
    {

        //if ((gesture.StartSelection && !GUISprites.isOnAGUIElement) || (gesture.StartSelection.tag == "RoomBoundingBox" && !GUISprites.isOnAGUIElement))
        //if (gesture.StartSelection.tag == "Furniture" && !GUISprites.isOnAGUIElement)
        if (gesture.StartSelection && gesture.StartSelection.CompareTag("RoomBoundingBox") && !GUISprites.isOnAGUIElement)
        {
            startMovingForward = true;
        }

    }

    //Gesture used to Recognize finger being lifted off screen
    void OnFingerUp(FingerUpEvent e)
    {
        //Stop moving Forward
        startMovingForward = false;

        //Stop moving backward
        startMovingBackWard = false;
    }

    //Gesture used to reset transform
    void OnDoubleTapReset(TapGesture gesture)
    {
        ResetTransform();
    }

    //Gesture used to start moving backward
    void OnLongPressDoubleFinger(LongPressGesture gesture)
    {
        if (!GUISprites.isOnAGUIElement)
        {
            startMovingBackWard = true;
        }
    }

    #endregion

    //Changing Speed of Movement (both forwad and backward)
    public void ForwardSpeedChanger(bool increasing)
    {
        if (increasing)
        {
            ForwardSpeed += BaseForwardSpeed;
        }
        else
        {
            ForwardSpeed = BaseForwardSpeed;
        }
    }

    //Aliging Camera with a point (both Position and Rotation)
    void AlignCameraWithPoint(Camera camera, Vector2 targetScreenPoint)
    {
        if (!Physics.Linecast(transform.position, camera.ScreenToWorldPoint(new Vector3(targetScreenPoint.x, targetScreenPoint.y, 2f))))
        {
            StartCoroutine("MoveToPosition", camera.ScreenToWorldPoint(new Vector3(targetScreenPoint.x, targetScreenPoint.y, 1.5f)));
        }
    }

    //Coroutin used to align camera with a point
    public IEnumerator MoveToPosition(Vector3 newLocalTarget)
    {
        isUndergoingDoubleTap = true;
        float elapsedTime = 0.0f;
        Vector3 startingPosition = transform.localPosition;

        Quaternion targetRotation = Quaternion.LookRotation(newLocalTarget - transform.position);

        while (elapsedTime < time)
        {
            transform.localPosition = Vector3.Lerp(startingPosition, newLocalTarget, (elapsedTime / time));

            // Rotations
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, (elapsedTime / time));

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        isUndergoingDoubleTap = false;
        yield return 0;
    }

    //Coroutin uset to align camera with a point and a rotation
    public IEnumerator MoveAndRotateToPosition(float[] tempTransform)
    {
        isUndergoingDoubleTap = true;
        ModeSwitcher.isInTransition = true;
        float elapsedTime = 0.0f;
        Vector3 startingPosition = transform.localPosition;
        Vector3 tempPosition = new Vector3(tempTransform[0], tempTransform[1], tempTransform[2]);
        Quaternion tempRotation = new Quaternion(tempTransform[3], tempTransform[4], tempTransform[5], tempTransform[6]);
        while (elapsedTime < time)
        {
            transform.localPosition = Vector3.Lerp(startingPosition, tempPosition, (elapsedTime / time));

            // Rotations
            transform.rotation = Quaternion.Slerp(transform.rotation, tempRotation, (elapsedTime / time));

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        isUndergoingDoubleTap = false;
        ModeSwitcher.isInTransition = false;
        yield return 0;
    }

    //Setting new Starting Point
    public void SetNewTransform()
    {
        //NewTransform = transform;
        NewPosition = transform.position;
        NewRotation = transform.rotation;
    }

    //Reseting Camera to it's initial (or newly setted) start point
    public void ResetTransform()
    {
        float[] tempTransform = new float[7];
        tempTransform[0] = NewPosition.x;
        tempTransform[1] = NewPosition.y;
        tempTransform[2] = NewPosition.z;
        tempTransform[3] = NewRotation.x;
        tempTransform[4] = NewRotation.y;
        tempTransform[5] = NewRotation.z;
        tempTransform[6] = NewRotation.w;

        StartCoroutine("MoveAndRotateToPosition", tempTransform);
    }

    public void ResetToATargetTransform(Vector3 targetPosition, Quaternion targetRotation)
    {
        float[] tempTransform = new float[7];
        tempTransform[0] = targetPosition.x;
        tempTransform[1] = targetPosition.y;
        tempTransform[2] = targetPosition.z;
        tempTransform[3] = targetRotation.x;
        tempTransform[4] = targetRotation.y;
        tempTransform[5] = targetRotation.z;
        tempTransform[6] = targetRotation.w;

        StartCoroutine("MoveAndRotateToPosition", tempTransform);
    }
    //Function starting when Free Camera Mode is Started
    void YourOn()
    {
        //ResetTransform();
    }


}