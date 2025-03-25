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
    private Vector3 randomOffsetVector;

    private const float RANDOM_OFFSET = 20f;
    private const float ANIMATION_MAX_SCALE = 10f;
    private const float ANIMATION_MIN_SCALE = 1f;
    private const float ANIMATION_DURATION = 0.33f;
    private const float LIFETIME = 1f;


    public void SetUp(Camera c, float magnitude, int critlevel, GameEnums.DamageElement element, EnemyEntity target) 
    {
        gameObject.SetActive(true);
        animT = 0;
        eventWorldPosition = target.transform.position;
        notificationText.text = magnitude.ToString("F0");
        cam = c;
        removeTime = Time.time + LIFETIME;
        randomOffsetVector = new Vector3(Random.Range(-RANDOM_OFFSET, RANDOM_OFFSET), Random.Range(-RANDOM_OFFSET, RANDOM_OFFSET), 0);
    }
    private void Update()
    {
        animT += Time.deltaTime / ANIMATION_DURATION;
        float colorAlpha = animT < 1 ? Mathf.Lerp(0, 1, animT) : 1;
        notificationText.color = new Color(notificationText.color.r, notificationText.color.g, notificationText.color.b, colorAlpha);
        transform.localScale = animT < 1 ? Mathf.Lerp(ANIMATION_MAX_SCALE, ANIMATION_MIN_SCALE, animT) * Vector3.one : Vector3.one; 
        transform.position = cam.WorldToScreenPoint(eventWorldPosition) + randomOffsetVector;
        if (transform.position.z < 0) { gameObject.SetActive(false); }
        if (Time.time > removeTime) { gameObject.SetActive(false); }
    }
}
