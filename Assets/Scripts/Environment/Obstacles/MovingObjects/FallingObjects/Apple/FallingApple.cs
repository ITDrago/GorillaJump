using Core.Time;
using Player;
using Skins;
using UnityEngine;

namespace Environment.Obstacles.MovingObjects.FallingObjects.Apple
{
    public class FallingApple : MovingObject
    {
        [SerializeField] private float _slowdownFactor = 0.5f;
        [SerializeField] private float _effectDuration = 5f;

        protected override void OnPlayerEnter(PlayerCore playerCore)
        {
            var durationMultiplier = ActiveSkinManager.Instance.CurrentSkin?.EffectDurationMultiplier ?? 1;
            var finalDuration = _effectDuration * durationMultiplier;
            
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.ApplyEffect(_slowdownFactor, finalDuration);
            }
            
            Destroy(gameObject);
        }
    }
}