using UnityEngine;
using System.Collections;
using JCCode;

public class ObjectPoolDemo2 : MonoBehaviour 
{
    public RandomCube randomCube;
    public RandomSphere randomSphere;

    private float _delayTime = 2.0f;
    private float _timer = 0.0f;

    void Awake()
    {
        randomCube.CreatePool(5);
        randomSphere.CreatePool(5);
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > _delayTime)
        {
            if(randomCube.HasObject())
                randomCube.Spawn(Vector3.one);

            randomSphere.Spawn(Vector3.one * 2.0f);
        }
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 120, 80), "Destroy RandomCube Pool"))
        {
            randomCube.DestroyAllPool();
        }
    }
}