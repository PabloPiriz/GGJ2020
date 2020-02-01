using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSteppedImageRotate : MonoBehaviour
{
    public float _rotateTime = 1;
    public float _waitTime = .3f;

    public void Start()
    {

        StartCoroutine(Rotate());
    }

    private IEnumerator Rotate()
    {
        while (true)    
        {
            yield return new WaitForSecondsRealtime(_waitTime);
            float elapsed = 0;
            while(elapsed < _rotateTime)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / _rotateTime;
                //Debug.Log(t);
                //transform.rotation = Quaternion.Euler(Vector3.forward * Mathfx.Hermite(0, -360, t));
                transform.rotation = Quaternion.Euler(Vector3.forward * Mathfx.Hermite(-360, 0, t));
                yield return null;
            }
        }
    }
}
