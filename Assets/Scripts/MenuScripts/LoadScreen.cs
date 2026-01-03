using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadScreen : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private float finishedmultiplier = 1f;

    Coroutine loadingCoroutine;

    public void OnEnable()
    {
        UpdateSlider(0f);
    }

    private void UpdateSlider(float value)
    {
        value = Mathf.Clamp01(value);
        slider.value = value;
    }

    public void StartLoading(float timeToLoad = 2f)
    {
        finishedmultiplier = 1f;
        loadingCoroutine = StartCoroutine(LoadBarFill(timeToLoad));
    }

    public Coroutine FinishLoading(float multiplier = 3f)
    {
        finishedmultiplier = multiplier;
        return loadingCoroutine;
    }

    public IEnumerator LoadBarFill(float timeToLoad = 2f)
    {
        float timer = 0f;
        while (timer < timeToLoad)
        {
            timer += Time.deltaTime * finishedmultiplier;
            UpdateSlider(timer / timeToLoad);
            yield return null;
        }
        UpdateSlider(1f);
    }
}
