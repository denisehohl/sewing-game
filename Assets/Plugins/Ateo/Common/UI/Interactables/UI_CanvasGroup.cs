using System;
using System.Collections;
using UnityEngine;

[System.Serializable]
[RequireComponent(typeof(CanvasGroup))]
public class UI_CanvasGroup : MonoBehaviour
{
    private CanvasGroup m_CanvasGroup;
    private Coroutine m_FadeCoroutine;

    private bool m_Initialized = false;

    public float alpha
    {
        get
        {
            return m_CanvasGroup.alpha;
        }
        set
        {
            m_CanvasGroup.alpha = value;

            if (value == 1f)
                m_CanvasGroup.blocksRaycasts = true;
        }
    }

    public bool IsFading { get { return m_FadeCoroutine != null; } }

    private void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        m_CanvasGroup = GetComponent<CanvasGroup>();

        if (m_CanvasGroup == null)
            m_CanvasGroup = gameObject.AddComponent<CanvasGroup>();

        m_Initialized = true;
    }
    
    public void SetActive(bool enable, float alpha)
    {
        if (!m_Initialized)
            return;

        SetActive(enable);
        m_CanvasGroup.alpha = alpha;
    }

    public void SetActive(bool enable)
    {
        if (!m_Initialized)
            return;

        if (m_FadeCoroutine != null)
            StopCoroutine(m_FadeCoroutine);

        if (enable)
        {
            m_CanvasGroup.alpha = 1f;
            m_CanvasGroup.interactable = true;
            m_CanvasGroup.blocksRaycasts = true;
        }
        else
        {
            m_CanvasGroup.alpha = 0f;
            m_CanvasGroup.interactable = false;
            m_CanvasGroup.blocksRaycasts = false;
        }
    }
    public void Fade(float alpha, float time, float delay = 0f)
    {
        if (gameObject.activeInHierarchy)
        {
            if (m_FadeCoroutine != null)
                StopCoroutine(m_FadeCoroutine);

            m_FadeCoroutine = StartCoroutine(FadeCanvasGroup(alpha, time, delay));
        }
        else
        {
            m_CanvasGroup.alpha = alpha;
            m_CanvasGroup.interactable = alpha >= 1f;
            m_CanvasGroup.blocksRaycasts = Math.Abs(alpha) > 0.001f;
        }
    }

    private IEnumerator FadeCanvasGroup(float alpha, float time, float delay = 0f)
    {
        yield return new WaitForSecondsRealtime(delay);

        float start = m_CanvasGroup.alpha;
        float timer = 0f;

        while (timer < 1f)
        {
            timer += Time.unscaledDeltaTime / time;
            m_CanvasGroup.alpha = Mathf.Lerp(start, alpha, timer);
            yield return null;
        }

        if (alpha >= 1f)
        {
            m_CanvasGroup.interactable = true;
        }

        if (alpha != 0f)
        {
            m_CanvasGroup.blocksRaycasts = true;
        }
        else
        {
            m_CanvasGroup.blocksRaycasts = false;
        }

        m_FadeCoroutine = null;
    }
}
