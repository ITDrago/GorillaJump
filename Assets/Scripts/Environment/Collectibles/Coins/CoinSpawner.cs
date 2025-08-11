using Environment.Blocks;
using Environment.Blocks.BlockTypes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Environment.Collectibles.Coins
{
    public class CoinSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject _coinPrefab;
        [SerializeField] private BlockSpawner _blockSpawner;

        [Header("Spawn Settings")]
        [SerializeField] private int _minBlocksBetweenCoins = 2;
        [SerializeField] private int _maxBlocksBetweenCoins = 3;

        [Header("Safety Checks")]
        [SerializeField] private LayerMask _spawnBlockingLayers;
        [SerializeField] private Vector2 _clearanceBoxSize = new(2, 2);

        private int _blocksUntilCoin;
        private int _blockCounter;
        private Block _lastBlock;
        private Vector2 _lastCheckedPosition;

        private void Start()
        {
            if (_blockSpawner) _blockSpawner.OnBlockSpawned += HandleBlockSpawned;
            
            SetNewCoinInterval();
        }

        private void OnDestroy()
        {
            if (_blockSpawner) _blockSpawner.OnBlockSpawned -= HandleBlockSpawned;
        }

        private void HandleBlockSpawned(Block newBlock)
        {
            if (!_lastBlock)
            {
                _lastBlock = newBlock;
                return;
            }

            _blockCounter++;

            if (_blockCounter >= _blocksUntilCoin)
            {
                SpawnCoinAtMidpoint(_lastBlock, newBlock);
                _blockCounter = 0;
                SetNewCoinInterval();
            }
            
            _lastBlock = newBlock;
        }

        private void SpawnCoinAtMidpoint(Block firstBlock, Block secondBlock)
        {
            if (!firstBlock || !secondBlock || !_coinPrefab) return;
            
            var spawnPosition = (firstBlock.transform.position + secondBlock.transform.position) / 2;

            if (IsPositionClear(spawnPosition)) Instantiate(_coinPrefab, spawnPosition, Quaternion.identity, transform);
        }

        private bool IsPositionClear(Vector2 position)
        {
            _lastCheckedPosition = position;
            return !Physics2D.BoxCast(position, _clearanceBoxSize, 0, Vector2.down, 0.1f, _spawnBlockingLayers);
        }

        private void SetNewCoinInterval() => _blocksUntilCoin = Random.Range(_minBlocksBetweenCoins, _maxBlocksBetweenCoins + 1);

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(_lastCheckedPosition, new Vector3(_clearanceBoxSize.x, _clearanceBoxSize.y));
        }
    }
}