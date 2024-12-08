using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Transform _camVerticalRotationAxis;
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private GameObject _cube;
    [SerializeField] private GameObject _capsule;

    private const float MOVEMENT_SPEED = 0.5f;
    private const float ROTATION_SPEED = 200f;
    private const float MIN_CAM_VERTICAL_ROTATION_X = 350f;
    private const float MAX_CAM_VERTICAL_ROTATION_X = 50f;
    private bool _isPlayerControlsEnabled = false;

    private void Start () 
    {
        if(IsOwner)
        {
            _cube.GetComponent<Renderer>().material.color = Color.green;
            _capsule.GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            _cube.GetComponent<Renderer>().material.color = Color.red;
            _capsule.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            _isPlayerControlsEnabled = true;
            _playerCamera.gameObject.SetActive(true);
        }
    }
    private void Update()
    {
        if (!_isPlayerControlsEnabled) return;

        float movementInputH = Input.GetAxis("Horizontal");
        float movementInputV = Input.GetAxis("Vertical");
        float rotationInputH = Input.GetAxis("Mouse X");
        float rotationInputV = Input.GetAxis("Mouse Y");
        
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
