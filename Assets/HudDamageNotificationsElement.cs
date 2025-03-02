using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudDamageNotificationsElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI notificationText;

    private Vector3 eventWorldPosition;
    private Camera cam;
    private float removeTime;
    private float animT;

    private const float ANIMATION_MAX_SCALE = 10f;
    private const float ANIMATION_MIN_SCALE = 1f;
    private const float ANIMATION_DURATION = 0.33f;
    private const float LIFETIME = 1.5f;


    public void SetUp(Vector3 wpos, float magnitude, int critlevel, GameEnums.DamageElement element) 
    {
        gameObject.SetActive(true);
        animT = 0;
        eventWorldPosition = wpos;
        notificationText.text = magnitude.ToString("F0");
        cam = Camera.main;
        removeTime = Time.time + LIFETIME;
    }
    private void Update()
    {
        animT += Time.deltaTime / ANIMATION_DURATION;
        float colorAlpha = animT < 1 ? Mathf.Lerp(0, 1, animT) : 1;
        notificationText.color = new Color(notificationText.color.r, notificationText.color.g, notificationText.color.b, colorAlpha);
        transform.localScale = animT < 1 ? Mathf.Lerp(ANIMATION_MAX_SCALE, ANIMATION_MIN_SCALE, animT) * Vector3.one : Vector3.one; 
        transform.position = cam.WorldToScreenPoint(eventWorldPosition);
        if (transform.position.z < 0) { gameObject.SetActive(false); }
        if (Time.time > removeTime) { gameObject.SetActive(false); }
    }
}
