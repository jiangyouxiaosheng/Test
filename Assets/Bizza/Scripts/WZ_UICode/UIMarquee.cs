using System;
using System.Collections;
using TMPro;
using UnityEngine;

using Random = UnityEngine.Random;

public class UIMarquee : MonoBehaviour
{
    public CanvasGroup Root;

    public TMP_Text txtMarquee;

    public string msgFormat;
        
    private WaitForSeconds m_waitForHide;

    public float[] moneyArray;
    
    void Start()
    {        
        m_waitForHide = new WaitForSeconds(3f);
    }

    private Coroutine marqueeCoroutine;

    public void StartMarquee()
    {
        marqueeCoroutine = StartCoroutine(PlayMarquee());
    }

    public void StopMarquee()
    {
        if (marqueeCoroutine != null)
            StopCoroutine(marqueeCoroutine);
        Root.gameObject.SetActive(false);
    }

    IEnumerator PlayMarquee()
    {
        yield return m_waitForHide;
        Root.gameObject.SetActive(false);
        while (true)
        {
            string uid = Guid.NewGuid().GetHashCode().ToString().TrimStart('-');
            int idx = Random.Range(0, moneyArray.Length);
            txtMarquee.text = string.Format(msgFormat, uid, moneyArray[idx].ToString());
            //_fadeAnimation.Show(0.3f, immediately: true);
            yield return m_waitForHide;
            //_fadeAnimation.Hide(0.3f, immediately: true);
            float t = Random.Range(0.5f, 3f);
            yield return new WaitForSeconds(t);
        }
    }
}