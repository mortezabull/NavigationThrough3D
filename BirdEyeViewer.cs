using UnityEngine;
using System.Collections;

public class BirdEyeViewer : MonoBehaviour
{
    
    public Transform CenterOfRotation;
    public float HorizontalTolerance = 1;
    public float TopLimit;
    public float DownLimit;
    public float MinimumZoomDistance = 4;
    public float MaximumZoomDistance = 8;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float rotationX;
    private float gestureSigned;
    private float distanceToCenter;
    private float time = 1;
    private float gestureXMovement;
    private float gestureStationaryPoint = 0;

    //!!!
    Vector3 CenterToCameraVector;
    Vector3 XZCenterToCameraVector;
    float ArcSinCameraRotation;
    //!!!
    void Start()
    {
        if (CenterOfRotation == null)
        {

            CenterOfRotation = GameObject.Find("Center").transform;
            CenterOfRotation.transform.position += GameObject.FindObjectOfType<Lollypop>().floorPosition;

        }

        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    //!!!
    void Update()
    {
        transform.LookAt(CenterOfRotation);
    }
    //!!!



    void OnDrag(DragGesture gesture)
    {
            if (!GUISprites.isOnAGUIElement)
            {
                if (!ChangePositionHandler.isAnchorSet)
                {
                    gestureXMovement = gesture.DeltaMove.x.Centimeters();

                    if (gesture.DeltaMove.x == 0)
                    {
                        gestureStationaryPoint = gesture.Position.x;
                    }

                    if (Mathf.Abs(gesture.DeltaMove.x.Centimeters()) > HorizontalTolerance)
                    {
                        if (!ModeSwitcher.isInTransition)
                        {
                            if (gesture.DeltaMove.x != 0 && gesture.Phase == ContinuousGesturePhase.Updated)
                            {
                                StartCoroutine("RotateObject", Mathf.Sign(gestureXMovement) * 3);

                            }
                        }
                    }

                    CenterToCameraVector = transform.position - CenterOfRotation.position;
                    XZCenterToCameraVector = new Vector3(0, CenterToCameraVector.y, 0);
                    ArcSinCameraRotation = Mathf.Sqrt(XZCenterToCameraVector.sqrMagnitude / CenterToCameraVector.sqrMagnitude);

                    if (Mathf.Abs(gesture.DeltaMove.y.Centimeters()) > HorizontalTolerance)
                    {
                        if (!ModeSwitcher.isInTransition)
                        {
                            if (gesture.Phase == ContinuousGesturePhase.Updated && Mathf.Sign(gesture.DeltaMove.y) > 0 && ArcSinCameraRotation > DownLimit)
                            {
                                StartCoroutine("RotateObjectVertical", -1);
                            }
                            else if (gesture.Phase == ContinuousGesturePhase.Updated && Mathf.Sign(gesture.DeltaMove.y) < 0 && ArcSinCameraRotation < TopLimit)
                            {
                                StartCoroutine("RotateObjectVertical", 1);
                            }
                        }
                    }

                }
            }
    }

    //Pinch Gesture Used to Zoom Camera 
    void OnPinch(PinchGesture gesture)
    {
        if (!ChangePositionHandler.isAnchorSet)
        {
            //Get direction of pinch
            gestureSigned = Mathf.Sign(gesture.Delta);
            distanceToCenter = Vector3.Distance(transform.position, CenterOfRotation.position);

            if ((gestureSigned > 0 && distanceToCenter > MinimumZoomDistance) || (gestureSigned < 0 && distanceToCenter < MaximumZoomDistance))
            {
                transform.position = Vector3.MoveTowards(transform.position, CenterOfRotation.position, gestureSigned * Time.deltaTime * 5);
                //StartCoroutine("MoveInOut", gestureSigned * 5);
            }
        }
    }

    //Double Tap (with Two Fingers) used to Reset Camera to It's Initial Position
    void OnDoubleTapReset(TapGesture gesture)
    {
        if (!ChangePositionHandler.isInRepositioningMode)
        {
            ResetTransform();
        }
    }

    //Reset Transform Function
    public void ResetTransform()
    {
        float[] tempTransform = new float[7];

        //Position Elemnts of initail transform
        tempTransform[0] = initialPosition.x;
        tempTransform[1] = initialPosition.y;
        tempTransform[2] = initialPosition.z;

        //Rotation Elements of initial transform
        tempTransform[3] = initialRotation.x;
        tempTransform[4] = initialRotation.y;
        tempTransform[5] = initialRotation.z;
        tempTransform[6] = initialRotation.w;

        StartCoroutine("MoveAndRotateToPosition", tempTransform);
    }

    //Coroutine Used to take camera to it's initial state
    public IEnumerator MoveAndRotateToPosition(float[] tempTransform)
    {
        ModeSwitcher.isInTransition = true;
        float elapsedTime = 0.0f;

        Vector3 startingPosition = transform.localPosition;
        //Converting first 3 elements of Input to Position of initial transform
        Vector3 tempPosition = new Vector3(tempTransform[0], tempTransform[1], tempTransform[2]);

        //Converting 4th to 7th elements of Input to Quaternion of initial rotation
        Quaternion tempRotation = new Quaternion(tempTransform[3], tempTransform[4], tempTransform[5], tempTransform[6]);

        while (elapsedTime < time)
        {
            //Positions
            transform.localPosition = Vector3.Lerp(startingPosition, tempPosition, (elapsedTime / time));

            // Rotations
            transform.rotation = Quaternion.Slerp(transform.rotation, tempRotation, (elapsedTime / 5 * time));

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        ModeSwitcher.isInTransition = false;
        yield return 0;
    }
    IEnumerator RotateObject(float rotateAmount)
    {
        Vector3 point = CenterOfRotation.transform.position;
        Vector3 axis = CenterOfRotation.up;
        float rotateTime = 0.5f;
        float step = 0.0f; //non-smoothed
        float rate = 1.0f / rotateTime; //amount to increase non-smooth step by
        float smoothStep = 0.0f; //smooth step this time
        float lastStep = 0.0f; //smooth step last time
        while (step < 1.0)
        { // until we're done
            step += Time.deltaTime * rate; //increase the step
            smoothStep = Mathf.SmoothStep(0.0f, 1.0f, step); //get the smooth step
            transform.RotateAround(point, axis, rotateAmount * (smoothStep - lastStep));
            lastStep = smoothStep; //store the smooth step
            yield return new WaitForEndOfFrame();
        }
        //finish any left-over
        if (step > 1.0) transform.RotateAround(point, axis, rotateAmount * (1.0f - lastStep));
        yield return null;

    }
    IEnumerator RotateObjectVertical(float rotateAmount)
    {
        CenterToCameraVector = transform.position - CenterOfRotation.position;
        XZCenterToCameraVector = new Vector3(0, CenterToCameraVector.y, 0);
        ArcSinCameraRotation = Mathf.Sqrt(XZCenterToCameraVector.sqrMagnitude / CenterToCameraVector.sqrMagnitude);

        Vector3 point = CenterOfRotation.transform.position;
        Vector3 axis = transform.right;
        float rotateTime = 0.5f;
        float step = 0.0f; //non-smoothed
        float rate = 1.0f / rotateTime; //amount to increase non-smooth step by
        float smoothStep = 0.0f; //smooth step this time
        float lastStep = 0.0f; //smooth step last time
        while (step < 1.0)
        { // until we're done
            step += Time.deltaTime * rate; //increase the step
            smoothStep = Mathf.SmoothStep(0.0f, 1.0f, step); //get the smooth step
            if (rotateAmount < 0 && ArcSinCameraRotation > DownLimit)
            {
                transform.RotateAround(point, axis, rotateAmount * (smoothStep - lastStep));
            }
            if (rotateAmount > 0 && ArcSinCameraRotation < TopLimit)
            {
                transform.RotateAround(point, axis, rotateAmount * (smoothStep - lastStep));
            }
            lastStep = smoothStep; //store the smooth step
            yield return new WaitForEndOfFrame();
        }
        //finish any left-over
        if (step > 1.0) transform.RotateAround(point, axis, rotateAmount * (1.0f - lastStep));
        yield return null;

    }
    IEnumerator MoveInOut(float movementAmount)
    {
        float duration = 0.5f;
        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        { // until we're done
            if ((gestureSigned > 0 && distanceToCenter > 4) || (gestureSigned < 0 && distanceToCenter < 8))
            {
                transform.position = Vector3.MoveTowards(transform.position, CenterOfRotation.position, movementAmount * (elapsedTime / duration));
            }
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return null;

    }

}
