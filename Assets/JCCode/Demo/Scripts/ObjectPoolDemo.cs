using UnityEngine;
using System.Collections;
using JCCode;

public class ObjectPoolDemo : MonoBehaviour 
{
    public RandomCube randomCube;
    public RandomSphere randomSphere;

    void Update()
    {
        randomCube.Spawn(Vector3.zero);

        randomSphere.Spawn(Vector3.one * Random.Range(-2.0f, 2.0f));
    }
}