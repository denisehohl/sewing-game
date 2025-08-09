using System;
using Moreno.SewingGame.Path;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace Moreno.SewingGame
{
    public class VolumeAccuracyTrendListener : MonoBehaviour
    {
        [SerializeField]
        private Volume _volume;
        [SerializeField]
        private float _smoothDamp = 0;
        [SerializeField]
        private float _smoothDampMaxspeed = 0;

        private float _targetValue = 0;
        private float _velocity;

        private void OnEnable()
        {
            PathEvaluater.OnAccuracyChanged += OnAccuracy;
            DamageManager.OnDamageTaken += OnDamage;
            OnAccuracy(PathEvaluater.StaticAccuracyTrend);
        }

        private void OnDisable()
        {
            PathEvaluater.OnAccuracyChanged -= OnAccuracy;
            DamageManager.OnDamageTaken -= OnDamage;
        }

        private void Update()
        {
            _volume.weight = Mathf.SmoothDamp(_volume.weight, _targetValue, ref _velocity, _smoothDamp,
                _smoothDampMaxspeed);
        }

        private void OnAccuracy(float accuracy)
        {
            _targetValue = accuracy;
        }

        private void OnDamage(float damage, float intensity)
        {
            float weight = _volume.weight;
            weight -= intensity;
            _volume.weight = Mathf.Clamp01(weight);
        }
    }
}