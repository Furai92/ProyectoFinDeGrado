using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudDamageNotificationsElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI notificationText;

    private Vector3 eventWorldPosition;
    private Camera cam;
    private float removeTime;

    private const float LIFETIME = 2f;


    public void SetUp(Vector3 wpos, float magnitude, int critlevel, GameEnums.DamageElement element) 
    {
        gameObject.SetActive(true);
        eventWorldPosition = wpos;
        notificationText.text = magnitude.ToString("F0");
        cam = Camera.main;
        removeTime = Time.time + LIFETIME;
    }
    private void Update()
    {
        transform.position = cam.WorldToScreenPoint(eventWorldPosition);
        if (transform.position.z < 0) { gameObject.SetActive(false); }
        if (Time.time > removeTime) { gameObject.SetActive(false); }
    }
}
