using UnityEngine;

public class CombatWarningLine : MonoBehaviour
{
    [SerializeField] MeshRenderer mr;

    private int animPhase;
    private float animT;
    private float duration;
    private float fillPercent;

    private const float SPAWN_ANIM_DURATION = 0.25f;
    private const float FADE_ANIM_DURATION = 0.25f;
    private const float SPAWN_HEIGHT = 0.15f;

    private void OnDisable()
    {
        EventManager.StageStateEndedEvent -= OnStageStateEnded;
    }
    private void OnEnable()
    {
        EventManager.StageStateEndedEvent += OnStageStateEnded;
    }
    public void SetUp(Vector3 pos, float width, float lenght, float angle, float _duration)
    {
        duration = _duration;
        UpdateSize(width, lenght);
        UpdateRotation(angle);
        UpdatePosition(pos);
        animT = 0;
        animPhase = 0;
        fillPercent = 0;
        gameObject.SetActive(true);
        UpdateShader(0, 0);
    }
    private void Update()
    {
        switch (animPhase)
        {
            case 0: // Spawning
                {
                    fillPercent = Mathf.MoveTowards(fillPercent, 1, Time.deltaTime / duration);
                    animT += Time.deltaTime / SPAWN_ANIM_DURATION;
                    UpdateShader(animT, fillPercent);
                    if (animT > 1) { animT = 0; animPhase = 1; }
                    break;
                }
            case 1: // Growing
                {
                    fillPercent = Mathf.MoveTowards(fillPercent, 1, Time.deltaTime / duration);
                    UpdateShader(1, fillPercent);
                    if (fillPercent >= 1) { fillPercent = 1; animT = 0; animPhase = 2; }
                    break;
                }
            case 2: // Fading
                {
                    animT += Time.deltaTime / FADE_ANIM_DURATION;
                    UpdateShader(1 - animT, 1);
                    if (animT > 1) { gameObject.SetActive(false); }
                    break;
                }
        }
    }
    private void UpdateShader(float a, float percent)
    {
        mr.material.SetFloat("_FillPercent", percent);
        mr.material.SetFloat("_CombinedAlpha", a);
    }
    public void UpdateRotation(float a) 
    {
        transform.rotation = Quaternion.Euler(0, a, 0);
    }
    public void UpdateSize(float w, float h) 
    {
        transform.localScale = new Vector3(w, 1, h);
        mr.material.SetFloat("_BorderThicknesScaleMult", h/w);
    }
    public void UpdatePosition(Vector3 wpos)
    {
        transform.position = new Vector3(wpos.x, SPAWN_HEIGHT, wpos.z);
    }
    private void OnStageStateEnded(StageStateBase.GameState s)
    {
        gameObject.SetActive(false);
    }
}
