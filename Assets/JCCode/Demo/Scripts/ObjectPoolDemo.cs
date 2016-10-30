using UnityEngine;
using System.Collections;
using JCCode;

public class ObjectPoolDemo : MonoBehaviour 
{
    public RandomCube randomCube;
    public RandomSphere randomSphere;

    private float _delayTime = 2.0f;
    private float _timer = 0.0f;

    void Awake()
    {
        randomSphere.CreatePool();
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > _delayTime)
        {
            randomCube.Spawn(Vector3.zero);

            randomSphere.Spawn(Vector3.one * Random.Range(-2.0f, 2.0f));
        }
    }
}