using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float shakeTime = 0.15f;
    [SerializeField] private float shakeAmount = 0.12f;

    private Vector3 startingPosition;

    private void Awake()
    {
        startingPosition = transform.localPosition;
    }

    public void Shake()
    {
        StopAllCoroutines();
        StartCoroutine(ShakeCamera());
    }

    private IEnumerator ShakeCamera()
    {
        float timeLeft = shakeTime;

        while (timeLeft > 0f)
        {
            transform.localPosition = startingPosition + Random.insideUnitSphere * shakeAmount;
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        transform.localPosition = startingPosition;
    }
}
