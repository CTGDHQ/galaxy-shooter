using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public void ShakeCamera()
    {
        StartCoroutine(ShakeCameraRoutine());
    }
    
    private IEnumerator ShakeCameraRoutine()
    {
        var cameraPos = transform.position;

        var elapsed = 0f;
        var duration = 0.5f;

        while (elapsed < duration)
        {
            var x = Random.Range(-0.5f, 0.5f);
            var y = Random.Range(-0.5f, 0.5f);

            transform.position = new Vector3(x, y, cameraPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = cameraPos;
    }
}
