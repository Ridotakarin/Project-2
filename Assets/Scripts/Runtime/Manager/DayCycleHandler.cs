using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayCycleHandler : Singleton<DayCycleHandler>
{ 
    public Transform lightRoot;

    [Header("Day Light")]
    public Light2D dayLight;
    public Gradient dayLightGradient;

    [Header("Night Light")]
    public Light2D nightLight;
    public Gradient nightLightGradient;

    [Header("Global Light")]
    public Light2D globalLight;
    public Gradient golobalLightGradient;

    [Header("RimLights")]
    public Light2D sunRimLight;
    public Gradient sunRimLightGradient;
    public Light2D moonRimLight;
    public Gradient MoonRimLightGradient;

    [Tooltip("The angle 0 = upward, going clockwise to 1 along the day")]
    public AnimationCurve ShadowAngle;
    [Tooltip("The scale of the normal shadow length (0 to 1) along the day")]
    public AnimationCurve ShadowLength;

    public float orbitRadius = 10f;
    //public int minutesToIncrease;

    private void Update()
    {
        MoveSunAndMoon();
        UpdateLight();
    }

    public void MoveSunAndMoon()
    {
        float timeOfDay = (float)(EnviromentalStatusManager.Instance.eStatus.DateTime.Hour * 60f + EnviromentalStatusManager.Instance.eStatus.DateTime.Minute) / (24f * 60f);

        float angle = timeOfDay * 360f * Mathf.Deg2Rad;

        nightLight.transform.position = new Vector3(lightRoot.position.x + Mathf.Cos(angle) * orbitRadius, lightRoot.position.y + Mathf.Sin(angle) * orbitRadius);
        //moonRimLight.transform.position = new Vector3(lightRoot.position.x + Mathf.Cos(angle) * orbitRadius, lightRoot.position.y + Mathf.Sin(angle) * orbitRadius);
        dayLight.transform.position = new Vector3(lightRoot.position.x + -Mathf.Cos(angle) * orbitRadius, lightRoot.position.y + -Mathf.Sin(angle) * orbitRadius);
        //sunRimLight.transform.position = new Vector3(lightRoot.position.x + -Mathf.Cos(angle) * orbitRadius, lightRoot.position.y + -Mathf.Sin(angle) * orbitRadius);
    }

    public void UpdateLight()
    {
        float timeOfDay = (float)(EnviromentalStatusManager.Instance.eStatus.DateTime.Hour * 60f + EnviromentalStatusManager.Instance.eStatus.DateTime.Minute) / (24f * 60f);

        dayLight.color = dayLightGradient.Evaluate(timeOfDay);
        nightLight.color = nightLightGradient.Evaluate(timeOfDay);
        globalLight.color = golobalLightGradient.Evaluate(timeOfDay);
        //sunRimLight.color = sunRimLightGradient.Evaluate(timeOfDay);
        nightLight.color = nightLightGradient.Evaluate(timeOfDay);
    }
}
