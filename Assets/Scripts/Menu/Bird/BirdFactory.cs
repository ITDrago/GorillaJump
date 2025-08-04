using Interfaces;
using UnityEngine;

namespace Menu.Bird
{
    public class BirdFactory : IBirdFactory
    {
        private readonly GameObject[] _prefabs;
        private readonly UnityEngine.Camera _camera;
        private bool _lastDirection;

        public bool LastDirection => _lastDirection;

        public BirdFactory(GameObject[] prefabs, UnityEngine.Camera camera)
        {
            _prefabs = prefabs;
            _camera = camera;
        }

        public GameObject CreateRandomBird(float minY, float maxY)
        {
            var prefab = _prefabs[Random.Range(0, _prefabs.Length)];
            _lastDirection = Random.value > 0.5f;
            var spawnY = Random.Range(minY, maxY);

            var spawnX = _lastDirection
                ? _camera.ViewportToWorldPoint(Vector3.zero).x - 2f
                : _camera.ViewportToWorldPoint(Vector3.one).x + 2f;

            var bird = Object.Instantiate(prefab, new Vector3(spawnX, spawnY, 0), Quaternion.identity);
            if (bird.GetComponent<IBirdMover>() == null)
                bird.AddComponent<BirdMover>();

            return bird;
        }
    }
}