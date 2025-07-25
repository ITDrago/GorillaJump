using UnityEngine;

namespace Environment
{
    public class Block : MonoBehaviour
    {
        [SerializeField] private int _durability = 3;
        [SerializeField] private Transform _stickAnchor;

        public Transform StickAnchor => _stickAnchor;

        public void TakeDamage(int damage)
        {
            _durability -= damage;
            if (_durability <= 0) Destroy(gameObject);
        }
    }
}