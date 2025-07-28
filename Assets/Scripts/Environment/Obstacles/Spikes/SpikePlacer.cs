using System.Collections.Generic;
using UnityEngine;

namespace Environment.Obstacles.Spikes
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class SpikePlacer : MonoBehaviour
    {
        [SerializeField] [Range(0f, 0.4f)] private float _safeZoneOffset = 0.2f;

        [Header("Overlap Prevention")]
        [SerializeField] private float _spikeClearanceRadius = 0.5f;
        [SerializeField] private int _maxPlacementAttempts = 20;

        private BoxCollider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
        }

        public void PlaceSpikes(GameObject spikePrefab, int count)
        {
            if (!spikePrefab)
                return;

            var spawnedPositions = new List<Vector2>();

            for (var i = 0; i < count; i++)
            {
                for (var attempt = 0; attempt < _maxPlacementAttempts; attempt++)
                {
                    var (spawnPoint, spawnNormal) = GetRandomPointOnAnyEdge();

                    var isOverlapping = false;
                    foreach (var placedPos in spawnedPositions)
                    {
                        if (Vector2.Distance(placedPos, spawnPoint) < _spikeClearanceRadius)
                        {
                            isOverlapping = true;
                            break;
                        }
                    }

                    if (!isOverlapping)
                    {
                        var rotation = Quaternion.FromToRotation(Vector3.up, spawnNormal);
                        Instantiate(spikePrefab, spawnPoint, rotation, transform);
                        spawnedPositions.Add(spawnPoint);
                        break;
                    }
                }
            }
        }

        private (Vector2 position, Vector2 normal) GetRandomPointOnAnyEdge()
        {
            var halfSize = _collider.size / 2f;

            var corners = new Vector2[]
            {
                new Vector2(-halfSize.x, halfSize.y) + _collider.offset,
                new Vector2(halfSize.x, halfSize.y) + _collider.offset,
                new Vector2(halfSize.x, -halfSize.y) + _collider.offset,
                new Vector2(-halfSize.x, -halfSize.y) + _collider.offset
            };

            var normals = new Vector2[]
            {
                transform.up,
                transform.right,
                -transform.up,
                -transform.right
            };

            var edgeIndex = Random.Range(0, 4);

            var startPoint = corners[edgeIndex];
            var endPoint = corners[(edgeIndex + 1) % 4];

            var t = Random.Range(_safeZoneOffset, 1f - _safeZoneOffset);
            var localSpawnPoint = Vector2.Lerp(startPoint, endPoint, t);

            return (transform.TransformPoint(localSpawnPoint), normals[edgeIndex]);
        }
    }
}