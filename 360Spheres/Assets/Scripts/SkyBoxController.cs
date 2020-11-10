using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SkyBoxController : Controller
{
    protected override IEnumerator Apply(Environment environment)
    {
        //yield return FadeIn();
        float startValue = RenderSettings.skybox.GetFloat("_Exposure");
        yield return StartCoroutine(Interpolate(.25f, startValue, 0.0f, UpdateExposureCallback));

        SetTexture(environment);
        //yield return FadeOut();
        startValue = RenderSettings.skybox.GetFloat("_Exposure");
        yield return StartCoroutine(Interpolate(.25f, startValue, 1.0f, UpdateExposureCallback));
    }

    private void SetTexture(Environment environment)
    {
        RenderSettings.skybox.SetFloat("_Rotation", environment.WorldRotation);
        RenderSettings.skybox.mainTexture = environment.Background;

    }

    private IEnumerator FadeIn()
    {
        yield return Fade(.25f, 0.0f);
    }

    private IEnumerator FadeOut()
    {
       yield return Fade(.25f, 1.0f);
    }

    private IEnumerator Fade(float targetTime, float endValue)
    {
        float startValue = RenderSettings.skybox.GetFloat("_Exposure");
        yield return StartCoroutine(Interpolate(targetTime, startValue, endValue, UpdateExposureCallback));
    }

    private void UpdateExposureCallback(float value)
    {
        RenderSettings.skybox.SetFloat("_Exposure", value);
    }


}
