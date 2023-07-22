using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [SerializeField] private PostProcessVolume ppVolume;

    public List<Transform> objsReactingToBass, objsReactingToNB, objsReactingToMiddle, objsReactingToHigh;
    private AudioSource _audioSource;
    private float _lerpSpeed = 0.3f;
    private float _basicbloomIntensity;
    float _bloomIntensityOffset;
    private float _glintTimer;
    private bool _glintFlag;
    private float[] _spectrumWidth;

    private Bloom _bloomEffect;

    private void Awake()
    {
        _spectrumWidth = new float[64];
        _audioSource = GetComponent<AudioSource>();
        ppVolume.profile.TryGetSettings(out _bloomEffect);
    }

    private void FixedUpdate()
    {
        _audioSource.GetSpectrumData(_spectrumWidth, 0, FFTWindow.Blackman);
        GloomEffectReactsToMusic();
        ObjsReactToMusic();
    }

    private void GloomEffectReactsToMusic()
    {
        float totalFrequency = GetBassAvergeFrequency() + GetNBAvergeFrequency();
        float minBloomIntensity = 5f;
        float lerpSpeed = 0.05f;
        float _bloomIntensityAmplifier = 5f;
        _basicbloomIntensity = Mathf.Lerp(_basicbloomIntensity, minBloomIntensity + _bloomIntensityAmplifier * totalFrequency, lerpSpeed);

        _glintTimer += Time.deltaTime;
        _glintFlag = !_glintFlag;

        float _glintInterval = 0.25f;
        float _glintIntensityAmplifier = 1f;
        float _glintIntensity = totalFrequency * _glintIntensityAmplifier;
        if (!GetIsBassLouder())
        {
            if (_glintTimer > _glintInterval)
            {
                if (_glintFlag)
                {
                    _bloomIntensityOffset = 0;
                }
                else
                {
                    _bloomIntensityOffset = -_glintIntensity;
                }
            }
        }

        _bloomEffect.intensity.value = _basicbloomIntensity + _bloomIntensityOffset;
    }

    private void ObjsReactToMusic()
    {
        foreach (Transform obj in objsReactingToBass)
        {
            obj.localScale = Vector3.Lerp(obj.localScale, new Vector3(1, GetBassAvergeFrequency(), 1), _lerpSpeed);
        }
        foreach (Transform obj in objsReactingToNB)
        {
            obj.localScale = Vector3.Lerp(obj.localScale, new Vector3(1, GetNBAvergeFrequency(), 1), _lerpSpeed);
        }
        foreach (Transform obj in objsReactingToMiddle)
        {
            obj.localScale = Vector3.Lerp(obj.localScale, new Vector3(1, GetMiddleAvergeFrequency(), 1), _lerpSpeed);
        }
        foreach (Transform obj in objsReactingToHigh)
        {
            obj.localScale = Vector3.Lerp(obj.localScale, new Vector3(1, GetHighAvergeFrequency(), 1), _lerpSpeed);
        }
    }

    private float GetFrequenciesDiapason(int start, int end, int mult)
    {
        return _spectrumWidth.ToList().GetRange(start, end).Average() * mult;
    }

    private float GetBassAvergeFrequency()
    {
        return GetFrequenciesDiapason(0, 7, 10);
    }

    private float GetNBAvergeFrequency()
    {
        return GetFrequenciesDiapason(7, 15, 100);
    }

    private float GetMiddleAvergeFrequency()
    {
        return GetFrequenciesDiapason(15, 30, 200);
    }

    private float GetHighAvergeFrequency()
    {
        return GetFrequenciesDiapason(30, 32, 1000);
    }

    private bool GetIsBassLouder()
    {
        return (2 * GetBassAvergeFrequency()) > GetNBAvergeFrequency();
    }

}
