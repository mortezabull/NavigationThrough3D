using UnityEngine;
using System.Collections;

public class WalkingCamera : MonoBehaviour
{

    #region Variables
    [Range(1,3)]
    public float Height = 1.75f;

    public GameObject GUIButtons;
    [Range(0, 1)]
    public float VerticalRotationUpperLimit;
    [Range(0, 1)]
    public float VerticalRotationLowerLimit;

    public float MaximumMovementSpeedLimit;
    public float MaximumRotationSpeedLimit;
    public float MovementSpeed;
    public float RotationSpeed;

    //Camera's Initial Position
    private Vector3 initialPosition;
    //Camera's Initial Rotation
    private Quaternion initialRotation;

    private Quaternion initialCameraRotation;
    //Variable used to start movement of walking camera 
    public static bool startMoving_WalkingCamera;

    //Variable used to start rotation of camera and body
    public static bool startRotation_WalkingCamera;

    //Temporary variable used to store modified joystick movement (position)
    private Vector2 desiredPosition;
    //Temporary variable used to store modified joystick movement (rotation)
    private Vector2 desiredRotation;
    //Reset Animation Duration
    private float time = 1;

    private float arcSinVerticalAngle = 0;

    private Vector3 temp_Vector3;

    #region Shake Camera Variables
    
    #endregion

    #endregion

    void Start()
    {
        transform.position = new Vector3(transform.position.x, Height, transform.position.z);
        //Getting Initial Position and Rotation
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;

        MaximumMovementSpeedLimit *= ((float)Screen.width / 1920); ;
        MovementSpeed *= ((float)Screen.width / 1920);
        RotationSpeed *= ((float)Screen.width / 1920);
    }

    void Update()
    {
        //Checking if we're in Repositioning Mode to disable or enable Joysticks GUIs on screen
        if (ChangePositionHandler.isInRepositioningMode)
        {
            if (GUIButtons.activeInHierarchy)
            {
                GUIButtons.SetActive(false);
            }
        }
        else
        {
            if (!GUIButtons.activeInHierarchy)
            {
                GUIButtons.SetActive(true);
            }
        }
    }
    //Getting the Movement of Left Joystick and translating it to Camera Movement
    public void GetMovement(Vector3 JoyStickMovement, bool isStationary)
    {
        temp_Vector3 = MovementSpeed * ((JoyStickMovement.y * transform.forward) + (JoyStickMovement.x * transform.right));
        
        GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Clamp(temp_Vector3.x, -MaximumMovementSpeedLimit, MaximumMovementSpeedLimit), 0, Mathf.Clamp(temp_Vector3.z, -MaximumMovementSpeedLimit, MaximumMovementSpeedLimit));

    }

    //Getting the Movement of Right Joystick and translating it to Camera and Body Rotation
    public void GetRotation(Vector2 JoyStickRotation, bool isStationary)
    {
        #region Old Approach
        if (!ModeSwitcher.isInTransition)
        {
            JoyStickRotation.x = 20 * JoyStickRotation.x.Centimeters();
            JoyStickRotation.y = 20 * JoyStickRotation.y.Centimeters();

            JoyStickRotation.x = Mathf.Clamp(JoyStickRotation.x, -10, 10);
            JoyStickRotation.y = -Mathf.Clamp(JoyStickRotation.y, -10, 10);

            if (!isStationary)
            {
                desiredRotation = new Vector3((10f * JoyStickRotation.x), (10f * JoyStickRotation.y), 0);
            }
            else
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    desiredRotation = new Vector3((100f * JoyStickRotation.x), (100f * JoyStickRotation.y), 0);
                }
                else
                {
                    desiredRotation = new Vector3((10f * JoyStickRotation.x), (10f * JoyStickRotation.y), 0);
                }
            }

            startRotation_WalkingCamera = true;
        }
        #endregion

        arcSinVerticalAngle = transform.forward.y / Vector3.Magnitude(transform.forward);

        if ((arcSinVerticalAngle < -VerticalRotationLowerLimit && JoyStickRotation.y < 0) || (arcSinVerticalAngle > VerticalRotationUpperLimit && JoyStickRotation.y > 0) || (arcSinVerticalAngle < VerticalRotationUpperLimit && arcSinVerticalAngle > -VerticalRotationLowerLimit))
        {
            //Rotating camera parallel to world's up vector
            transform.RotateAround(transform.position, transform.right, JoyStickRotation.y * RotationSpeed * Time.deltaTime);
        }

        //Rotating Camera around it's transform.right vector (parallel to horizon)
        transform.RotateAround(transform.position, Vector3.up, JoyStickRotation.x * RotationSpeed * Time.deltaTime);
    }

    //Resets the Transform to it's Initial State
    public void ResetTransform()
    {
        //Adding x, y, z position and x, y, z, w Rotation of MainBody
        //and x, y, z, w Rotation of Camera to an Array of floats to Pass it to a Coroutine Function as a single object
        float[] tempTransform = new float[11];
        //Main Body x Position
        tempTransform[0] = initialPosition.x;
        //Main Body y Position
        tempTransform[1] = initialPosition.y;
        //Main Body z Position
        tempTransform[2] = initialPosition.z;
        //Main Body x Rotation
        tempTransform[3] = initialRotation.x;
        //Main Body y Rotation
        tempTransform[4] = initialRotation.y;
        //Main Body z Rotation
        tempTransform[5] = initialRotation.z;
        //Main Body w Rotation
        tempTransform[6] = initialRotation.w;
        //Camera x Rotation
        tempTransform[7] = initialCameraRotation.x;
        //Camera y Rotation
        tempTransform[8] = initialCameraRotation.y;
        //Camera z Rotation
        tempTransform[9] = initialCameraRotation.z;
        //Camera w Rotation
        tempTransform[10] = initialCameraRotation.w;

        StartCoroutine("MoveAndRotateToPosition", tempTransform);
    }

    //Coroutine used to move Main body and Camera and align it to initial state
    public IEnumerator MoveAndRotateToPosition(float[] tempTransform)
    {
        ModeSwitcher.isInTransition = true;
        float elapsedTime = 0.0f;
        Vector3 startingPosition = transform.localPosition;

        Vector3 tempPosition = new Vector3(tempTransform[0], tempTransform[1], tempTransform[2]);
        Quaternion tempRotation = new Quaternion(tempTransform[3], tempTransform[4], tempTransform[5], tempTransform[6]);
        
        while (elapsedTime < time)
        {
            transform.localPosition = Vector3.Lerp(startingPosition, tempPosition, (elapsedTime / time));

            // MainBody Rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, tempRotation, (elapsedTime / 5 * time));
            
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        ModeSwitcher.isInTransition = false;
        yield return 0;
    }

    //Resets the Transform to a Transform
    public void ResetToTransform(Transform target)
    {
        //Adding x, y, z position and x, y, z, w Rotation of MainBody
        //and x, y, z, w Rotation of Camera to an Array of floats to Pass it to a Coroutine Function as a single object
        float[] tempTransform = new float[11];
        //Main Body x Position
        tempTransform[0] = target.transform.position.x;
        //Main Body y Position Stays the same
        tempTransform[1] = Height;
        //Main Body z Position
        tempTransform[2] = target.transform.position.z;
        //Main Body x Rotation
        tempTransform[3] = target.transform.rotation.x;
        //Main Body y Rotation
        tempTransform[4] = target.transform.rotation.y;
        //Main Body z Rotation
        tempTransform[5] = target.transform.rotation.z;
        //Main Body w Rotation
        tempTransform[6] = target.transform.rotation.w;

        StartCoroutine("MoveAndRotateToPosition", tempTransform);
    }

    //Double Tap with two fingers used to initiate ResetTransform Function
    void OnDoubleTapReset(TapGesture gesture)
    {
        ResetTransform();
    }

    
}
