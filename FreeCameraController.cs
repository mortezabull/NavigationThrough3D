using UnityEngine;
using System.Collections;

public class FreeCameraController : MonoBehaviour
{
    public float MovementSpeed = 5;
    public float RotationSpeed = 100;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * MovementSpeed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.forward * -Time.deltaTime * MovementSpeed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * Time.deltaTime * MovementSpeed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.right * -Time.deltaTime * MovementSpeed);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Vector3.up * Time.deltaTime * RotationSpeed);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.up * Time.deltaTime * -RotationSpeed);
        }
        if (Input.GetKey(KeyCode.C))
        {
            transform.Rotate(Vector3.right * Time.deltaTime * -RotationSpeed);
        }
        if (Input.GetKey(KeyCode.Z))
        {
            transform.Rotate(Vector3.right * Time.deltaTime * RotationSpeed);
        }
    }
}
