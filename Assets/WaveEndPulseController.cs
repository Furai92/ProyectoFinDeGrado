using UnityEngine;
using System.Collections;

public class WaveEndPulseController : MonoBehaviour
{
    [SerializeField] private MeshRenderer mr1;
    [SerializeField] private MeshRenderer mr2;

    private const float INNER_SIZE_MULT = 0.25f;
    private const float OUTTER_SIZE_MULT = 1f;
    private const float MIN_SIZE = 3f;
    private const float MAX_SIZE = 500f;
    private const float EFFECT_DURATION = 3f;

    private void OnEnable()
    {
        EventManager.StageStateEndedEvent += OnStageStateEnded;
        mr1.transform.gameObject.SetActive(false);
        mr2.transform.gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        EventManager.StageStateEndedEvent -= OnStageStateEnded;
        StopAllCoroutines();
    }
    private void OnStageStateEnded(StageStateBase.GameState s) 
    {
        if (s != StageStateBase.GameState.EnemyWave) { return; }

        StopAllCoroutines();
        StartCoroutine(PulseCR());
    }


    private void UpdateMatAlpha(float a) 
    {
        a = Mathf.Clamp01(a);
        mr1.material.SetFloat("_Alpha", a);
        mr2.material.SetFloat("_Alpha", a);
    }
    private IEnumerator PulseCR() 
    {
        UpdateMatAlpha(0);
        mr1.transform.gameObject.SetActive(true);
        mr2.transform.gameObject.SetActive(true);
        float t = 0;
        while (t < 1) 
        {
            t += Time.deltaTime / EFFECT_DURATION;
            mr1.transform.localScale = INNER_SIZE_MULT * Mathf.Lerp(MIN_SIZE, MAX_SIZE, t) * Vector3.one;
            mr2.transform.localScale = OUTTER_SIZE_MULT * Mathf.Lerp(MIN_SIZE, MAX_SIZE, t) * Vector3.one;
            UpdateMatAlpha(Mathf.Lerp(1, 0, t));
            yield return null;
        }
        mr1.transform.gameObject.SetActive(false);
        mr2.transform.gameObject.SetActive(false);
    }
}
