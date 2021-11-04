using System;
using System.Collections;
using UnityEngine;

namespace Utilities.Tweening
{
    public class TweenManager : MonoBehaviour
    {
        public static TweenManager Tween { get; private set; }
        private void Awake()
        {
            if (Tween == null) Tween = this;
            else Destroy(this);
        }

        /// <summary>
        /// Tweens the given Vector3.
        /// </summary>
        /// <param name="vec3"> The Vector3 to tween. </param>
        /// <param name="endPos"> The final value. </param>
        /// <param name="time"> The time to get from the starting value to the end value. </param>
        /// <param name="tolerance"> The difference at which the Vector3 can be snapped to the end value. </param>
        /// <param name="returnAct"> The Action that sets the Vector3 over the course of the tween. </param>
        /// <param name="callbacks"> Actions to be invoked once the tween in over. </param>
        public void TweenVector3(Vector3 vec3, Vector3 endPos, float time, float tolerance, Action<Vector3> returnAct, params Action[] callbacks)
        {
            StartCoroutine(_Vector3(vec3, endPos, time, tolerance, returnAct, callbacks));
        }
        private IEnumerator _Vector3(Vector3 vec3, Vector3 endPos, float time, float tolerance, Action<Vector3> returnAct, params Action[] callbacks)
        {
            Vector3 speed = (endPos - vec3) / time;
            while (Vector3.Distance(vec3, endPos) > tolerance)
            {
                vec3 += speed * Time.deltaTime;
                returnAct(vec3);

                yield return new WaitForEndOfFrame();
            }

            vec3 = endPos;
            returnAct(vec3);

            foreach (Action callback in callbacks)
            {
                callback?.Invoke();
            }
        }

        /// <summary>
        /// Tweens the given Vector3 through all of the values in a sequence.
        /// </summary>
        /// <param name="vec3"> The Vector3 to tween. </param>
        /// <param name="sequence"> The various values that the Vector3 has to be tweened to. </param>
        /// <param name="time"> The time to get from one value to another. </param>
        /// <param name="wait"> The amount of time between the end of one tween and another. </param>
        /// <param name="tolerance"> The difference at which the Vector3 can be snapped to the end value. </param>
        /// <param name="returnAct"> The Action that sets the Vector3 over the course of the tween. </param>
        /// <param name="callbacks"> Actions to be invoked once the tween in over. </param>
        public void TweenVector3Sequence(Vector3 vec3, Vector3[] sequence, float time, float wait, float tolerance, Action<Vector3> returnAct, params Action[] callbacks)
        {
            StartCoroutine(_Vector3Sequence(vec3, sequence, time, wait, tolerance, returnAct, callbacks));
        }
        private IEnumerator _Vector3Sequence(Vector3 vec3, Vector3[] sequence, float time, float wait, float tolerance, Action<Vector3> returnAct, params Action[] callbacks)
        {
            for (int i = 0; i < sequence.Length; i++)
            {
                Vector3 speed = (sequence[i] - vec3) / time;
                while (Vector3.Distance(vec3, sequence[i]) > tolerance)
                {
                    vec3 += speed * Time.deltaTime;
                    returnAct(vec3);

                    yield return new WaitForEndOfFrame();
                }

                vec3 = sequence[i];
                returnAct(vec3);

                yield return new WaitForSeconds(wait + time);
            }

            foreach (Action callback in callbacks)
            {
                callback?.Invoke();
            }
        }
    }
}