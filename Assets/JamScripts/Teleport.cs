using Gamekit2D;
using UnityEngine;


public class Teleport : MonoBehaviour
{
    [Header("Base options")]
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
    [Header("Upgrade Options")]
    [SerializeField] private KeyCode _moveMirrorUp = KeyCode.W;
    [SerializeField] private KeyCode _moveMirrorDown = KeyCode.S;
    [SerializeField] private float _maxMirrorVerticalMoving = 3.0f;
    [SerializeField] private float _minMirrorVerticalMoving = -3.0f;

    private Transform _player;
    private Transform _mirror;
    private Vector3 _mirrorPositionOffset;
    private bool _isPlayerFacesRight = true;

    private PlayerCharacter _playerCharacter;
    private SpriteRenderer _spriteRendererPlayer;
    private SpriteRenderer _spriteRendererMirror;
    private MirrorView _mirrorView;

    private bool _isMirrorInBlockage;
    private float _attackCooldown;
    private bool _isUpgradeActivated = false;

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
        UpdateMirror();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            if (!_isPlayerFacesRight) TurnAroundSystem();
            _isPlayerFacesRight = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (_isPlayerFacesRight) TurnAroundSystem();
            _isPlayerFacesRight = false;
        }

        if (Input.GetKeyDown(_activationKey) && !_isMirrorInBlockage)
        {
            _player.position = _mirror.position;
            TurnAroundSystem();
            _mirrorPositionOffset.y *= -1;
        }

        if (Input.GetKey(_moveLeftMirror))
        {
            if (_isPlayerFacesRight)
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
            if (_isPlayerFacesRight)
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


        if(_isUpgradeActivated)
        {
            if(Input.GetKey(_moveMirrorUp))
            {
                _mirrorPositionOffset.y += _movingSpeedMirror * Time.deltaTime;
                if(_mirrorPositionOffset.y > _maxMirrorVerticalMoving)
                {
                    _mirrorPositionOffset.y = _maxMirrorVerticalMoving;
                }
            }

            if (Input.GetKey(_moveMirrorDown))
            {
                _mirrorPositionOffset.y -= _movingSpeedMirror * Time.deltaTime;
                if (_mirrorPositionOffset.y < _minMirrorVerticalMoving)
                {
                    _mirrorPositionOffset.y = _minMirrorVerticalMoving;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        UpdateMirror();
        MirrorAttack();
    }

    private void TurnAroundSystem()
    {
        _playerCharacter.cameraHorizontalFacingOffset *= -1;
        _mirrorPositionOffset.x = _playerCharacter.cameraHorizontalFacingOffset;
    }

    private void UpdateMirror()
    {
        if (_isPlayerFacesRight)
        {
            _mirror.position = _player.position + _mirrorPositionOffset;
        }
        else
        {
            Vector3 mirrorPosition = _mirror.position;
            mirrorPosition.x = _player.position.x - _mirrorPositionOffset.x;
            mirrorPosition.y = _player.position.y + _mirrorPositionOffset.y;
            _mirror.position = mirrorPosition;
        }

        _spriteRendererMirror.sprite = _spriteRendererPlayer.sprite;

        if (_isMirrorInBlockage)
        {
            _spriteRendererMirror.color = _redColor;
        }
        else
        {
            _spriteRendererMirror.color = _normalColor;
        }

        _mirrorPositionOffset.x = _playerCharacter.cameraHorizontalFacingOffset;
        Vector3 currentScale = _mirror.localScale;
        if (_isPlayerFacesRight)
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
            _isMirrorInBlockage = true;
        }
        else
        {
            _isMirrorInBlockage = false;
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

    public void ActivateUpgrade()
    {
        _isUpgradeActivated = true;
    }

    private void OnDestroy()
    {
        _mirrorView.OnMirrorInBlockage -= OnMirronInBlockage;
    }
}
