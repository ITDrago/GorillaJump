using System.Threading;
using Core;
using Core.Audio;
using Core.Game;
using Player;
using UnityEngine;

namespace Environment.Obstacles.MovingObjects.FallingObjects.Apple
{
    public class FallingApple : MovingObject
    {
        [SerializeField] private float _slowdownFactor = 0.5f;
        [SerializeField] private float _effectDuration = 5;
        
        private static CancellationTokenSource _cancellationTokenSource;
        
        protected override async void OnPlayerEnter(PlayerCore playerCore)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            
            GameEvents.AppleCollected();
            Destroy(gameObject);
            ApplyTimeEffect();
            
            try
            {
                await Awaitable.WaitForSecondsAsync(_effectDuration, _cancellationTokenSource.Token);
            }
            catch
            {
                return;
            }
            
            if (!_cancellationTokenSource.IsCancellationRequested) RestoreTime();
        }

        private void ApplyTimeEffect()
        {
            Time.timeScale = _slowdownFactor;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }

        private static void RestoreTime()
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = 0.02f;
        }
    }
}