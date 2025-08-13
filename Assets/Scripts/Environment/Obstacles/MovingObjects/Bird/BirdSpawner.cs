using System.Collections.Generic;
using System.Linq;
using Environment.Blocks.BlockTypes;
using UnityEngine;

namespace Environment.Obstacles.MovingObjects.Bird
{
    public class BirdSpawner : BaseMovingObjectsSpawner<Bird>
    {
        [Header("Interception Settings")]
        [SerializeField] [Range(0, 1)] private float _spawnChance = 0.5f;
        [SerializeField] private float _minGapSizeForSpawn = 5;
        [SerializeField] [Range(0.1f, 0.9f)] private float _interceptionPointRatio = 0.5f;
        [SerializeField] private float _spawnOffsetBelowCamera = 2;
        [SerializeField] private Bird _birdPrefab;
        
        private Block _currentTargetBlock;
        private float _calculatedBirdSpeed;

        protected override void Awake()
        {
            base.Awake();
            if (!_birdPrefab) enabled = false;
            else PlayerController.OnJumped += HandlePlayerJumped;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            PlayerController.OnJumped -= HandlePlayerJumped;
        }

        private void HandlePlayerJumped(Block jumpedFromBlock, float jumpForce)
        {
            var allBlocks = BlockSpawner.SpawnedBlocks.ToList();
            var nextBlock = GetNextBlock(jumpedFromBlock, allBlocks);
            if (!nextBlock) return;

            if (Vector3.Distance(jumpedFromBlock.transform.position, nextBlock.transform.position) < _minGapSizeForSpawn) return;
            if (Random.value > _spawnChance) return;

            _currentTargetBlock = GetTargetBlock(nextBlock, allBlocks);
            if (!_currentTargetBlock) return;

            CalculateAndSpawnInterceptor(jumpedFromBlock, nextBlock, jumpForce);
        }

        private void CalculateAndSpawnInterceptor(Block fromBlock, Block toBlock, float jumpForce)
        {
            if (!PlayerController.TryGetComponent(out Rigidbody2D playerRigidbody)) return;

            var startVelocity = (Vector2)(PlayerController.GorillaBody.right * jumpForce);
            if (startVelocity.x <= 0) return;

            var startPosition = (Vector2)PlayerController.transform.position;
            var gravity = Physics2D.gravity.y * playerRigidbody.gravityScale;
            var interceptionX = Mathf.Lerp(fromBlock.transform.position.x, toBlock.transform.position.x, _interceptionPointRatio);
            
            var timeToIntercept = (interceptionX - startPosition.x) / startVelocity.x;
            if (timeToIntercept <= 0) return;
            
            var interceptionY = startPosition.y + startVelocity.y * timeToIntercept + 
                                0.5f * gravity * timeToIntercept * timeToIntercept;

            var cameraBottomY = MainCamera.ViewportToWorldPoint(new Vector3(0, 0, MainCamera.nearClipPlane)).y;
            var spawnY = cameraBottomY - _spawnOffsetBelowCamera;
            var requiredSpeed = (interceptionY - spawnY) / timeToIntercept;

            if (requiredSpeed <= 0) return;
            _calculatedBirdSpeed = requiredSpeed;
            
            if (IsPositionClear(new Vector2(interceptionX, spawnY)))
                SpawnObject(_birdPrefab, new Vector2(interceptionX, spawnY));
        }
        
        private Block GetTargetBlock(Block startBlock, List<Block> allBlocks)
        {
            var startIndex = allBlocks.IndexOf(startBlock);
            return startIndex != -1 && startIndex + 1 < allBlocks.Count ? allBlocks[startIndex + 1] : null;
        }
        
        protected override Bird CreateMovingObjectInstance(Bird prefab, Vector2 spawnPosition)
        {
            var instance = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
            instance.SetMovementSpeed(_calculatedBirdSpeed);
            if (_currentTargetBlock) instance.Initialize(_currentTargetBlock.transform);
            return instance;
        }

        private Block GetNextBlock(Block currentBlock, List<Block> blocks)
        {
            var currentIndex = blocks.IndexOf(currentBlock);
            return currentIndex != -1 && currentIndex + 1 < blocks.Count ? blocks[currentIndex + 1] : null;
        }
    }
}