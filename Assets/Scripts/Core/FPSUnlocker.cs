using UnityEngine;

namespace Core
{
    public class FPSUnlocker : MonoBehaviour
    {
        [SerializeField] private int _targetFPS = 120;

        private void Awake()
        {
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = _targetFPS;
        }
    }
}