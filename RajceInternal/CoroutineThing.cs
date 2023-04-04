using System.Collections.Generic;

using UnityEngine;

namespace RajceInternal
{
    internal class CoroutineThing : MonoBehaviour
    {
        public Coroutine StartCoro(IEnumerator<object> en) => StartCoroutine(en);
    }
}
