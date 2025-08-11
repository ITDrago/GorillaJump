using Interfaces;
using UnityEngine;

namespace Menu.Bird
{
    public class BirdMover : MonoBehaviour, IBirdMover
    {
        private bool _leftToRight;
        private float _speed;
        private float _amplitude;
        private float _frequency;
        private UnityEngine.Camera _camera;
        private float _time;

        public void Initialize(bool leftToRight, float speed, float amplitude, float frequency, UnityEngine.Camera camera)
        {
            _leftToRight = leftToRight;
            _speed = speed;
            _amplitude = amplitude;
            _frequency = frequency;
            _camera = camera;

            if (!_leftToRight)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        private void Update()
        {
            _time += Time.deltaTime;
            var horizontal = _speed * Time.deltaTime * (_leftToRight ? 1 : -1);
            var vertical = Mathf.Sin(_time * _frequency) * _amplitude * Time.deltaTime;

            transform.position += new Vector3(horizontal, vertical, 0);

            var viewportPos = _camera.WorldToViewportPoint(transform.position);
            if (viewportPos.x < -0.2f || viewportPos.x > 1.2f)
                Destroy(gameObject);
        }
    }
}