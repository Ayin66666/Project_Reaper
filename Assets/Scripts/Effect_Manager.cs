using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
//using Cinemachine.Editor;
using Cinemachine.Utility;
using UnityEngine.UIElements;
using Easing;

public class Effect_Manager : MonoBehaviour
{
    public static Effect_Manager instance;

    [SerializeField] private CinemachineVirtualCamera cinemachine;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private float minFOV;
    [SerializeField] private float maxFOV;
    private bool isHitStop;
    // ��� 4���� �����Ǿ� ����!
    // ī�޶� ����, ī�޶� ��, ������, ���ο���

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


        cinemachine = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
    }

    #region Camera Effect

    public void VirtualCamera_Shake(float power)
    {
        impulseSource.GenerateImpulseWithForce(power);
    }

    public void Camera_Shake(float intensity, float time)
    {
        // ù��° ���� ��鸲�� ����, �θ�° ���� ��鸱 �ð�
        StopCoroutine(nameof(CameraShake));
        StartCoroutine(CameraShake(intensity, time));
    }

    private IEnumerator CameraShake(float intensity, float time) 
    {
        float power = intensity;
        float timer = time;

        // Shake Cam
        while (power > 0)
        {
            for (int i = 0; i < 3; i++)
            {
                //cinemachine.GetRig(i).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = power;
            }
            impulseSource.GenerateImpulse(power);

            power -= Time.deltaTime / timer;
            yield return null;
        }

        // Reset
        for (int i = 0; i < 3; i++)
        {
            //cinemachine.GetRig(i).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
        }
    }

    public void Camera_FOV(bool isOn)
    {
        StartCoroutine(CameraFOV(isOn));
    }

    private IEnumerator CameraFOV(bool isOn)
    {
        if (isOn) // FOV Up On
        {
            // FOV Up
            float size;
            float speed = 1;
            float timer = 0;
            while (timer < 1)
            {
                if(speed > 0.5f)
                {
                    speed = timer <= 0.7f ? speed += Time.deltaTime * 10f : speed -= Time.deltaTime * 6f;
                }

                timer += Time.deltaTime * speed;
                size = Mathf.Lerp(minFOV, maxFOV, timer);
                cinemachine.m_Lens.FieldOfView = size;
                yield return null;
            }
            cinemachine.m_Lens.OrthographicSize = maxFOV;
        }
        else
        {
            float size;
            float speed = 1;
            float timer = 0;
            while(timer < 1)
            {
                if (speed > 0.5f)
                {
                    speed = timer <= 0.7f ? speed += Time.deltaTime * 10f : speed -= Time.deltaTime * 6f;
                }

                size = Mathf.Lerp(maxFOV, minFOV, timer);
                cinemachine.m_Lens.FieldOfView = size;
                yield return null;
            }
            cinemachine.m_Lens.FieldOfView = minFOV;
        }
    }
    #endregion

    #region Time Effect
    public void Slow_Motion(float slowTime)
    {
        StartCoroutine(SlowMotion(slowTime)); 
    }

    private IEnumerator SlowMotion(float slowTime)
    {
        Debug.Log("Call Slow");
        float timer = 0;
        while(timer < 1)
        {
            if(timer <= 0.5f && Time.timeScale > 0.5f)
            {
                Time.timeScale -= Time.deltaTime;
            }

            if(timer > 0.5f && Time.timeScale < 1f)
            {
                Time.timeScale += Time.deltaTime;
            }

            timer += Time.deltaTime / slowTime;
            yield return null;

        }

        Time.timeScale = 1;
    }

    public void Hit_Stop(float stopTime)
    {
        if(isHitStop)
        {
            Time.timeScale = 1f;
            StopCoroutine(nameof(HitStop));
        }

        // Ÿ���� ���� ���� �ð��� ������ ��!
        StartCoroutine(HitStop(stopTime));
    }

    private IEnumerator HitStop(float stopTime)
    {
        isHitStop = true;
        Time.timeScale = 0.1f;
        yield return new WaitForSeconds(stopTime);
        Time.timeScale = 1f;
        isHitStop = false;
    }
    #endregion
}
