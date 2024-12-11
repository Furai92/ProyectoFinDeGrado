using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Transform _camVerticalRotationAxis;
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private GameObject _cube;
    [SerializeField] private GameObject _capsule;

    private const float MOVEMENT_SPEED = 0.5f;
    private const float ROTATION_SPEED = 15f;
    private const float MIN_CAM_VERTICAL_ROTATION_X = 350f;
    private const float MAX_CAM_VERTICAL_ROTATION_X = 50f;
    private bool _isPlayerControlsEnabled = false;

    private void Start () 
    {
        if (IsOwner || NetworkManager.Singleton == null)
        {
            _isPlayerControlsEnabled = true;
            _playerCamera.gameObject.SetActive(true);
        }

        if(IsHost && IsOwner)
        {
            _cube.GetComponent<Renderer>().material.color = Color.green;
            _capsule.GetComponent<Renderer>().material.color = Color.green;
        }
        else if (IsHost && !IsOwner)
        {
            _cube.GetComponent<Renderer>().material.color = Color.red;
            _capsule.GetComponent<Renderer>().material.color = Color.red;
        }
        else if (!IsHost && IsOwner)
        {
            _cube.GetComponent<Renderer>().material.color = Color.red;
            _capsule.GetComponent<Renderer>().material.color = Color.red;
        }
        else if (!IsHost && !IsOwner)
        {
            _cube.GetComponent<Renderer>().material.color = Color.green;
            _capsule.GetComponent<Renderer>().material.color = Color.green;
        }
    }
    private void Update()
    {
        if (!_isPlayerControlsEnabled) return;

        float movementInputH = InputManager.Instance.GetMovementInput().x;
        float movementInputV = InputManager.Instance.GetMovementInput().y;
        float rotationInputH = InputManager.Instance.GetLookInput().x;
        float rotationInputV = InputManager.Instance.GetLookInput().y;
        
        transform.position += movementInputV * Time.fixedDeltaTime * MOVEMENT_SPEED * transform.forward;
        transform.position += movementInputH * Time.fixedDeltaTime * MOVEMENT_SPEED * transform.right;
        transform.Rotate(0, rotationInputH * Time.fixedDeltaTime * ROTATION_SPEED, 0);
        _camVerticalRotationAxis.Rotate(-rotationInputV * Time.fixedDeltaTime * ROTATION_SPEED, 0, 0);
        ClampCamVerticalRotation();
        StageManager.UpdatePlayerPosition(transform.position);
    }
    private void ClampCamVerticalRotation() 
    {
        float x = _camVerticalRotationAxis.localRotation.eulerAngles.x;
        x = x < 180 ? Mathf.Clamp(x, 0, MAX_CAM_VERTICAL_ROTATION_X) : Mathf.Clamp(x, MIN_CAM_VERTICAL_ROTATION_X, 360); 
        _camVerticalRotationAxis.localRotation = Quaternion.Euler(x, 0, 0);
    }
}
