using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudCombatWarningElement : MonoBehaviour
{
    public enum WarningType { Normal, Explosion }

    [SerializeField] private Image warningImg;

    private Vector3 eventWorldPosition;
    private Camera cam;
    private float removeTime;
    private float animT;

    private const float MAXIMUM_WORLD_TO_SCREEN_Z = 0f;
    private const float RADAR_RADIUS_SIZE = 400f;
    private const float ANIMATION_MAX_SCALE = 20f;
    private const float ANIMATION_MIN_SCALE = 3f;
    private const float ANIMATION_DURATION = 0.33f;
    private const float LIFETIME = 1f;


    public void SetUp(Camera c, Vector3 pos, WarningType wt)
    {
        gameObject.SetActive(true);
        animT = 0;
        eventWorldPosition = pos;
        cam = c;
        removeTime = Time.time + LIFETIME;
    }
    private void Update()
    {
        animT += Time.deltaTime / ANIMATION_DURATION;
        float colorAlpha = animT < 1 ? Mathf.Lerp(0, 1, animT) : 1;
        warningImg.color = new Color(warningImg.color.r, warningImg.color.g, warningImg.color.b, colorAlpha);
        transform.localScale = animT < 1 ? Mathf.Lerp(ANIMATION_MAX_SCALE, ANIMATION_MIN_SCALE, animT) * Vector3.one : Vector3.one;
        transform.position = cam.WorldToScreenPoint(eventWorldPosition);


        if (transform.position.z > MAXIMUM_WORLD_TO_SCREEN_Z) { gameObject.SetActive(false); }

        transform.localPosition = -transform.localPosition;
        transform.localPosition = Vector3.Normalize(transform.localPosition);
        transform.localPosition *= RADAR_RADIUS_SIZE;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(transform.localPosition.y, transform.localPosition.x) * Mathf.Rad2Deg);

        if (Time.time > removeTime) { gameObject.SetActive(false); }
    }
}
