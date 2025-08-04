using Interfaces;
using UnityEngine;

namespace Menu.Bird
{
    public class BirdSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject[] _birdPrefabs;
        [SerializeField] private float _spawnYMin = -2;
        [SerializeField] private float _spawnYMax = 2;
        [SerializeField] private float _spawnDelay = 2;
        [SerializeField] private Vector2Int _birdSpeed = new(3, 5);
        [SerializeField] private float _verticalAmplitude = 0.5f;
        [SerializeField] private float _verticalFrequency = 2;

        private UnityEngine.Camera _camera;
        private IBirdFactory _birdFactory;

        private void Start()
        {
            _camera = UnityEngine.Camera.main;
            _birdFactory = new BirdFactory(_birdPrefabs, _camera);
            SpawnLoop();
        }

        private async void SpawnLoop()
        {
            while (true)
            {
                var bird = _birdFactory.CreateRandomBird(_spawnYMin, _spawnYMax);
                var mover = bird.GetComponent<IBirdMover>();
                mover.Initialize(_birdFactory.LastDirection, Random.Range(_birdSpeed.x, _birdSpeed.y), _verticalAmplitude, _verticalFrequency,
                    _camera);

                try
                {
                    await Awaitable.WaitForSecondsAsync(_spawnDelay);
                }
                catch
                {
                    return;
                }
            }
        }
    }
}