using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextUIEffects : MonoBehaviour
{
    private TMP_Text _text;
    [SerializeField] private float _pulseDuration = 3.0f;
    [SerializeField] private float _scaleSpeed = 2.0f;
    [SerializeField] private Vector3 _startScale;
    [SerializeField] private Vector3 _endScale;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        _text.enabled = false;
    }

    public void ShowAndPulseWaveText()
    {
        StartCoroutine(PulseWaveText());
    }

    private IEnumerator PulseWaveText()
    {
        _text.enabled = true;
        float time = 0;

        while (time < _pulseDuration)
        {
            float t = Mathf.PingPong(time * _scaleSpeed, 1);
            _text.transform.localScale = Vector3.Lerp(_startScale, _endScale, t);
            time += Time.deltaTime;
            yield return null;
        }

        _text.transform.localScale = _startScale;
        _text.enabled = false;
    }
}
