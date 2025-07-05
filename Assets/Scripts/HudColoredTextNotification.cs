using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudColoredTextNotification : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI notificationText;

    private Vector3 eventWorldPosition;
    private Camera cam;
    private float removeTime;
    private float animT;
    private Vector3 randomOffsetVector;
    private float sizeMult;

    private const float VERTICAL_OFFSET = 1.5f;
    private const float RND_OFFSET = 0.5f;
    private const float ANIMATION_MAX_SCALE = 10f;
    private const float ANIMATION_MIN_SCALE = 1.5f;
    private const float ANIMATION_DURATION = 0.33f;

    public void SetUp(Camera _cam, string _text, TMP_ColorGradient _gradient, float _sizeMult, float _lifetime, bool _useRandomOffset, Vector3 _wPos)
    {
        animT = 0;
        eventWorldPosition = _wPos;
        notificationText.text = _text;
        notificationText.enableVertexGradient = true;
        notificationText.color = Color.white;
        notificationText.colorGradientPreset = _gradient;
        gameObject.SetActive(true);
        cam = _cam;
        removeTime = Time.time + _lifetime;
        sizeMult = _sizeMult;
        randomOffsetVector = _useRandomOffset ? new Vector3(Random.Range(-RND_OFFSET, RND_OFFSET), Random.Range(-RND_OFFSET, RND_OFFSET) + VERTICAL_OFFSET, Random.Range(-RND_OFFSET, RND_OFFSET)) : Vector3.zero;
    }
    public void SetUp(Camera _cam, string _text, Color _color, float _sizeMult, float _lifetime, bool _useRandomOffset, Vector3 _wPos) 
    {
        animT = 0;
        eventWorldPosition = _wPos;
        notificationText.text = _text;
        notificationText.enableVertexGradient = false;
        notificationText.color = _color;
        gameObject.SetActive(true);
        cam = _cam;
        removeTime = Time.time + _lifetime;
        sizeMult = _sizeMult;
        randomOffsetVector = _useRandomOffset ? new Vector3(Random.Range(-RND_OFFSET, RND_OFFSET), Random.Range(-RND_OFFSET, RND_OFFSET) + VERTICAL_OFFSET, Random.Range(-RND_OFFSET, RND_OFFSET)) : Vector3.zero;
    }
    private void Update()
    {
        animT += Time.deltaTime / ANIMATION_DURATION;
        float colorAlpha = animT < 1 ? Mathf.Lerp(0, 1, animT) : 1;
        notificationText.color = new Color(notificationText.color.r, notificationText.color.g, notificationText.color.b, colorAlpha);
        transform.localScale = Mathf.Lerp(ANIMATION_MAX_SCALE, ANIMATION_MIN_SCALE, animT) * sizeMult * Vector3.one; 
        transform.position = cam.WorldToScreenPoint(eventWorldPosition + randomOffsetVector);
        if (transform.position.z < 0) { gameObject.SetActive(false); }
        if (Time.time > removeTime) { gameObject.SetActive(false); }
    }
}
