using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Utilities.TickSystem
{
    public class TickSystem : MonoBehaviour
    {
        [SerializeField] float tickLength = 0.5f;
        public float TickLength { get => tickLength; }
        [SerializeField] string key;
        [SerializeField] bool unscaledTime;

        public static Dictionary<string, TickSystem> Instances { get; private set; }

        [Space]
        public UnityEvent OnTickEarly;
        public UnityEvent OnTick;
        public UnityEvent OnTickLate;

        private void Awake()
        {
            if (Instances == null)
                Instances = new Dictionary<string, TickSystem>();

            if (key == null) 
                key = gameObject.name;

            Instances.Add(key, this);
        }

        float timer;
        private void Update()
        {
            timer += unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            if (timer >= tickLength)
            {
                timer = 0;

                OnTickEarly?.Invoke();
                OnTick?.Invoke();
                OnTickLate?.Invoke();
            }
        }
    }
}