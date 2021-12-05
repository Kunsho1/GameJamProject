using System;
using UnityEngine;


public class MirrorView : MonoBehaviour
{
    [SerializeField] private LayerMask _blockageLayer;
    [SerializeField] private Transform _attackPoint;

    public SpriteRenderer SpriteRenderer { get; private set; }
    public Transform AttackPoint => _attackPoint;

    private Collider2D _collider;

    public event Action<bool> OnMirrorInBlockage;

    private void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(_collider.IsTouchingLayers(_blockageLayer))
        {
            OnMirrorInBlockage?.Invoke(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_collider.IsTouchingLayers(_blockageLayer))
        {
            OnMirrorInBlockage?.Invoke(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!_collider.IsTouchingLayers(_blockageLayer))
        {
            OnMirrorInBlockage?.Invoke(false);
        }
    }
}
