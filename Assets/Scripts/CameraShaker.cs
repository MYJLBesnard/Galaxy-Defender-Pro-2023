using System.Collections;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public void StartDamageCameraShake(float dur, float mag)
    {
        StartCoroutine(Shake(dur, mag));
    }

    IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 OriginalPos = transform.position;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-0.5f, 0.5f) * magnitude;
            float y = Random.Range(-0.5f, 0.5f) * magnitude;
            transform.localPosition = new Vector3(x, y, OriginalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = OriginalPos;
    }
}
