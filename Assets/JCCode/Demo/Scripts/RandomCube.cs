using UnityEngine;
using System.Collections;
using JCCode;

public class RandomCube : MonoBehaviour 
{
    float _t = 3.0f;
    float _timer = 0.0f;

    void OnEnable()
    {
        _timer = 0.0f;
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > _t)
        {
            gameObject.Recycle();
        }
    }
}
