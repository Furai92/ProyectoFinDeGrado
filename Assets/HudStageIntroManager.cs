using UnityEngine;
using TMPro;
using System.Collections;

public class HudStageIntroManager : MonoBehaviour
{
    [SerializeField] private Transform introParent;
    [SerializeField] private TextMeshProUGUI stageNameText;
    [SerializeField] private TextMeshProUGUI difficultyNameText;
    [SerializeField] private TextMeshProUGUI pressToBeginText;
    [SerializeField] private CanvasGroup fadeCG;

    private const float FADE_IN_DURATION = 2f;

    private void OnEnable()
    {
        stageNameText.text = "Test Stage";
        difficultyNameText.text = "Normal";
        pressToBeginText.text = "- Press R to begin -";

        StartCoroutine(FadeInCR());
        EventManager.StageStateStartedEvent += OnStageStateStart;
        OnStageStateStart(StageManagerBase.GetCurrentStateType());
    }
    private void OnDisable()
    {
        EventManager.StageStateStartedEvent -= OnStageStateStart;
    }
    private void OnStageStateStart(StageStateBase.GameState s) 
    {
        if (s != StageStateBase.GameState.Intro) 
        {
            StopAllCoroutines();
            introParent.gameObject.SetActive(false);
        }
    }
    IEnumerator FadeInCR() 
    {
        introParent.gameObject.SetActive(true);
        float animT = 0;
        while (animT < 1) 
        {
            animT += Time.deltaTime / FADE_IN_DURATION;
            fadeCG.alpha = 1-animT;
            yield return null;
        }
    }
}
