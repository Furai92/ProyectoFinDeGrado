using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform _camVerticalRotationAxis;

    private const float MOVEMENT_SPEED = 10f;
    private const float ROTATION_SPEED = 360f;
    private const float MIN_CAM_VERTICAL_ROTATION_X = 340f;
    private const float MAX_CAM_VERTICAL_ROTATION_X = 50f;


    private void FixedUpdate()
    {
        float movementInputH = Input.GetAxis("Horizontal");
        float movementInputV = Input.GetAxis("Vertical");
        float rotationInputH = Input.GetAxis("Mouse X");
        float rotationInputV = Input.GetAxis("Mouse Y");

        
        transform.position += movementInputV * Time.fixedDeltaTime * MOVEMENT_SPEED * transform.forward;
        transform.position += movementInputH * Time.fixedDeltaTime * MOVEMENT_SPEED * transform.right;
        transform.Rotate(0, rotationInputH * Time.fixedDeltaTime * ROTATION_SPEED, 0);
        _camVerticalRotationAxis.Rotate(-rotationInputV * Time.fixedDeltaTime * ROTATION_SPEED, 0, 0);
        ClampCamVerticalRotation();
    }
    private void ClampCamVerticalRotation() 
    {
        float x = _camVerticalRotationAxis.localRotation.eulerAngles.x;
        x = x < 180 ? Mathf.Clamp(x, 0, MAX_CAM_VERTICAL_ROTATION_X) : Mathf.Clamp(x, MIN_CAM_VERTICAL_ROTATION_X, 360); 
        _camVerticalRotationAxis.localRotation = Quaternion.Euler(x, 0, 0);
    }
}
