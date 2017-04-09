using UnityEngine;
using System.Collections;

namespace JCCode
{
    namespace ObjectPool
    {

        public class RandomCube : MonoBehaviour
        {
            float _t = 1.0f;
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
    }
}