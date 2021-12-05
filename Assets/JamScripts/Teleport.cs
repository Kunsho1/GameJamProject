using Gamekit2D;
using UnityEngine;


public class Teleport : MonoBehaviour
{
    [SerializeField] private KeyCode _activationKey = KeyCode.F;
    [SerializeField] private KeyCode _moveLeftMirror = KeyCode.Comma;
    [SerializeField] private KeyCode _moveRightMirror = KeyCode.Period;
    [SerializeField] private KeyCode _meeleAtack = KeyCode.K;
    [SerializeField] private float _movingSpeedMirror = 10.0f;
    [SerializeField] private GameObject _mirrorObject;
    [SerializeField] private float _maxCameraOffset = 10.0f;
    [SerializeField] private float _minCameraOffset = -10.0f;
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _redColor = Color.red;
    [SerializeField] private float _attackDelay = 0.1f;

    private Transform _player;
    private Transform _mirror;
    private Vector3 _mirrorPositionOffset;
    private bool _playerFacesRight = true;

    private PlayerCharacter _playerCharacter;
    private SpriteRenderer _spriteRendererPlayer;
    private SpriteRenderer _spriteRendererMirror;
    private MirrorView _mirrorView;

    private bool _mirrorInBlockage;
    private float _attackCooldown;

    private void Start()
    {
        _playerCharacter = GetComponent<PlayerCharacter>();

        _player = transform;
        _mirror = Instantiate(_mirrorObject).transform;

        _spriteRendererPlayer = GetComponent<SpriteRenderer>();
        _mirrorView = _mirror.GetComponent<MirrorView>();
        _spriteRendererMirror = _mirrorView.SpriteRenderer;

        _mirrorView.OnMirrorInBlockage += OnMirronInBlockage;
        _mirrorView.AttackPoint.gameObject.SetActive(false);
        UpdateMirrorPosition();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            if (!_playerFacesRight) TurnAroundSystem();
            _playerFacesRight = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (_playerFacesRight) TurnAroundSystem();
            _playerFacesRight = false;
        }

        if (Input.GetKeyDown(_activationKey) && !_mirrorInBlockage)
        {
            _player.position = _mirror.position;
            TurnAroundSystem();
        }

        if (Input.GetKey(_moveLeftMirror))
        {
            if (_playerFacesRight)
            {
                MoveMirrorLeft();
            }
            else
            {
                MoveMirrorRight();
            }
        }

        if (Input.GetKey(_moveRightMirror))
        {
            if (_playerFacesRight)
            {
                MoveMirrorRight();
            }
            else
            {
                MoveMirrorLeft();
            }
        }

        if (Input.GetKeyDown(_meeleAtack) && _attackCooldown <= 0)
        {
            _attackCooldown = _attackDelay;
        }
        
        void MoveMirrorLeft()
        {
            _playerCharacter.cameraHorizontalFacingOffset -= _movingSpeedMirror * Time.deltaTime;
            if (_playerCharacter.cameraHorizontalFacingOffset < _minCameraOffset)
                _playerCharacter.cameraHorizontalFacingOffset = _minCameraOffset;
        }

        void MoveMirrorRight()
        {
            _playerCharacter.cameraHorizontalFacingOffset += _movingSpeedMirror * Time.deltaTime;
            if (_playerCharacter.cameraHorizontalFacingOffset > _maxCameraOffset)
                _playerCharacter.cameraHorizontalFacingOffset = _maxCameraOffset;
        }
    }

    private void FixedUpdate()
    {
        if (_playerFacesRight)
        {
            _mirror.position = _player.position + _mirrorPositionOffset;
        }
        else
        {
            _mirror.position = _player.position - _mirrorPositionOffset;
        }

        _spriteRendererMirror.sprite = _spriteRendererPlayer.sprite;

        if(_mirrorInBlockage)
        {
            _spriteRendererMirror.color = _redColor;
        }
        else
        {
            _spriteRendererMirror.color = _normalColor;
        }

        UpdateMirrorPosition();
        MirrorAttack();
    }

    private void TurnAroundSystem()
    {
        _playerCharacter.cameraHorizontalFacingOffset *= -1;
        _mirrorPositionOffset.x = _playerCharacter.cameraHorizontalFacingOffset;
    }

    private void UpdateMirrorPosition()
    {
        _mirrorPositionOffset.x = _playerCharacter.cameraHorizontalFacingOffset;
        Vector3 currentScale = _mirror.localScale;
        if (_playerFacesRight)
        {
            currentScale.x = 1;
        }
        else
        {
            currentScale.x = -1;
        }
        _mirror.localScale = currentScale;
    }

    private void OnMirronInBlockage(bool value)
    {
        if (value)
        {
            _mirrorInBlockage = true;
        }
        else
        {
            _mirrorInBlockage = false;
        }
    }

    private void MirrorAttack()
    {
        if(_attackCooldown > 0)
        {
            _attackCooldown -= Time.deltaTime;
            if(_attackCooldown <= 0)
            {
                _mirrorView.AttackPoint.gameObject.SetActive(true);
            }
        }
        else
        {
            _mirrorView.AttackPoint.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        _mirrorView.OnMirrorInBlockage -= OnMirronInBlockage;
    }
}
