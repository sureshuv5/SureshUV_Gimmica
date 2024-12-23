using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Scripts
{
    public class Image3DRotationScript : MonoBehaviour
    {
        [SerializeField] private RectTransform _grpToFlip;
        [SerializeField] private RectTransform _coinFrontToFlip;
        [SerializeField] private Vector2Int _roandomValues = new Vector2Int(0, 360);
        [SerializeField] private float _rotationSpeed = 5.0f;
        private int value;
        void Start()
        {
            uint randomValue = (uint)Random.Range(_roandomValues.x, _roandomValues.y);
            _rotationSpeed *= 100;
            float axis = Mathf.Sign(_rotationSpeed) * 2 - 1;
            transform.DOLocalRotate(Vector3.up * axis * _rotationSpeed + (Vector3.up * randomValue), 1.0f, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Incremental);
        }
        void LateUpdate()
        {
            // value = Mathf.FloorToInt(_grpToCheck.localEulerAngles.y / 180) % 2;
            // value *= 2 - 1;
            value = (int)Mathf.Sign(Mathf.Sin(transform.localEulerAngles.y * Mathf.Deg2Rad));
            _grpToFlip.localScale = new Vector3
            (
                value,
                _grpToFlip.localScale.y,
                _grpToFlip.localScale.z
            );
            _coinFrontToFlip.localScale = new Vector3
            (
                value,
                _grpToFlip.localScale.y,
                _grpToFlip.localScale.z
            );
        }
    }
}
