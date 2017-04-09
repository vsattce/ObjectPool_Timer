using UnityEngine;
using System.Collections;


namespace JCCode
{
    namespace ObjectPool
    {
        public class ObjectPoolDemo1 : MonoBehaviour
        {
            public RandomCube randomCube;
            public RandomSphere randomSphere;

            private float _delayTime = 2.0f;
            private float _timer = 0.0f;

            void Update()
            {
                _timer += Time.deltaTime;
                if (_timer > _delayTime)
                {
                    randomCube.Spawn(Vector3.one);

                    randomSphere.Spawn(Vector3.one * 2.0f);
                }
            }
        }
    }
}