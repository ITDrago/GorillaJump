using System.Collections.Generic;
using UnityEngine;

namespace Background
{
    public class BackgroundSpawner : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private GameObject[] _backgroundPrefabs;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private float _backgroundWidth = 23.74789f;
        [SerializeField] private float _overlap = 0f;

        [Header("System Behavior")]
        [SerializeField] private int _activeBackgroundsLimit = 3;
        [SerializeField] private int _totalPoolSize = 6;

        private Dictionary<string, Queue<GameObject>> _objectPool = new();
        private readonly List<GameObject> _activeBackgrounds = new();
        private GameObject _lastSpawnedBackground;

        private void Start()
        {
            if (!ValidateConfiguration())
            {
                enabled = false;
                return;
            }

            InitializePool();
            InitializeBackgrounds();
        }

        private void Update()
        {
            TrySpawnNext();
            TryDespawnOldest();
        }

        private void InitializeBackgrounds()
        {
            var centerBg = SpawnBackgroundAt(transform.position);
            if (!centerBg) return;
            
            var safetyPos = centerBg.transform.position - new Vector3(_backgroundWidth - _overlap, 0, 0);
            SpawnBackgroundAt(safetyPos, true);

            for (var i = 0; i < _activeBackgroundsLimit - 1; i++)
            {
                TrySpawnNext();
            }
        }

        private bool ValidateConfiguration()
        {
            if (_backgroundPrefabs == null || _backgroundPrefabs.Length == 0) return false;
            if (!_cameraTransform) return false;
            return true;
        }

        private void InitializePool()
        {
            foreach (var prefab in _backgroundPrefabs)
            {
                _objectPool.Add(prefab.name, new Queue<GameObject>());
            }

            var createdCount = 0;
            var prefabIndex = 0;
            while (createdCount < _totalPoolSize)
            {
                var prefab = _backgroundPrefabs[prefabIndex];
                var obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
                obj.name = prefab.name;
                obj.SetActive(false);
                _objectPool[prefab.name].Enqueue(obj);

                createdCount++;
                prefabIndex = (prefabIndex + 1) % _backgroundPrefabs.Length;
            }
        }

        private void TrySpawnNext()
        {
            if (_activeBackgrounds.Count >= _activeBackgroundsLimit + 1) return;
            if (!_lastSpawnedBackground) return;

            var triggerX = _lastSpawnedBackground.transform.position.x - (_backgroundWidth / 4f);

            if (_cameraTransform.position.x >= triggerX)
            {
                var spawnPos = GetNextSpawnPosition();
                SpawnBackgroundAt(spawnPos);
            }
        }
        
        private GameObject SpawnBackgroundAt(Vector3 position, bool insertAsFirst = false)
        {
            var prefab = GetRandomPrefab();
            if (!prefab) return null;

            var poolQueue = _objectPool[prefab.name];
            if (poolQueue.Count == 0) return null;

            var background = poolQueue.Dequeue();
            
            background.transform.position = position;
            background.SetActive(true);

            if (insertAsFirst)
            {
                _activeBackgrounds.Insert(0, background);
            }
            else
            {
                _activeBackgrounds.Add(background);
                _lastSpawnedBackground = background;
            }
            
            return background;
        }

        private Vector3 GetNextSpawnPosition()
        {
            if (!_lastSpawnedBackground)
            {
                return transform.position;
            }

            var lastBgPosition = _lastSpawnedBackground.transform.position;
            return new Vector3(lastBgPosition.x + (_backgroundWidth - _overlap), lastBgPosition.y, lastBgPosition.z);
        }

        private void TryDespawnOldest()
        {
            if (_activeBackgrounds.Count == 0) return;

            var oldestBackground = _activeBackgrounds[0];
            var oldestBgRightEdge = oldestBackground.transform.position.x + _backgroundWidth / 2f;
            
            if (oldestBgRightEdge < _cameraTransform.position.x - _backgroundWidth)
            {
                Despawn(oldestBackground);
            }
        }

        private void Despawn(GameObject backgroundToDespawn)
        {
            _activeBackgrounds.Remove(backgroundToDespawn);
            backgroundToDespawn.SetActive(false);

            if (_objectPool.ContainsKey(backgroundToDespawn.name))
            {
                _objectPool[backgroundToDespawn.name].Enqueue(backgroundToDespawn);
            }
            else
            {
                Destroy(backgroundToDespawn);
            }
        }
        
        private GameObject GetRandomPrefab()
        {
            if (_backgroundPrefabs.Length == 0) return null;
            var randomIndex = Random.Range(0, _backgroundPrefabs.Length);
            return _backgroundPrefabs[randomIndex];
        }
    }
}