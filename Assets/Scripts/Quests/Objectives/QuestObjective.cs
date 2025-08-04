using System;
using UnityEngine;

namespace Quests.Objectives
{
    public abstract class QuestObjective : MonoBehaviour
    {
        private int _currentProgress;
        private int _targetValue;
        private Action _onProgressUpdated;
        private Action _onObjectiveCompleted;

        public virtual void Initialize(int target, int currentProgress, Action onUpdate, Action onComplete)
        {
            _targetValue = target;
            _currentProgress = currentProgress;
            _onProgressUpdated = onUpdate;
            _onObjectiveCompleted = onComplete;

            if (_currentProgress >= _targetValue)
                _onObjectiveCompleted?.Invoke();
            else
                SubscribeToEvents();
        }

        protected abstract void SubscribeToEvents();
        protected abstract void UnsubscribeFromEvents();

        protected virtual void OnDestroy() => UnsubscribeFromEvents();

        protected void UpdateProgress()
        {
            if (_currentProgress >= _targetValue) return;

            _currentProgress++;
            _onProgressUpdated?.Invoke();

            if (_currentProgress >= _targetValue)
            {
                _onObjectiveCompleted?.Invoke();
                UnsubscribeFromEvents();
            }
        }
    }
}