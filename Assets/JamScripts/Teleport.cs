using Gamekit2D;
using UnityEngine;


public class Teleport : MonoBehaviour
{
    [SerializeField] private KeyCode _activationKey = KeyCode.F;
    [SerializeField] private KeyCode _moveLeftMirror = KeyCode.Comma;
    [SerializeField] private KeyCode _moveRightMirror = KeyCode.Period;
    [SerializeField] private float _movingSpeedMirror = 10.0f;
    [SerializeField] private GameObject _mirrorObject;
    [SerializeField] private GameObject _stickObject;
    [SerializeField] private float _maxCameraOffset = 10.0f;
    [SerializeField] private float _minCameraOffset = -10.0f;

    private Transform _player;
    private Transform _mirror;
    private Transform _stick;
    private Vector3 _mirrorPositionOffset;
    private bool _playerFacesRight;

    private Transform _mainCameraTransform;
    private const string _mainCamera = "MainCamera";
    private PlayerCharacter _playerCharacter;
    private SpriteRenderer _spriteRendererPlayer;
    private SpriteRenderer _spriteRendererMirror;

    private void Start()
    {
        _player = transform;
        
        _playerCharacter = GetComponent<PlayerCharacter>();
        
        _mirror = Instantiate(_mirrorObject).transform;
        _mirror.position = _player.position;

        _stick = Instantiate(_stickObject).transform;

        _mainCameraTransform = GameObject.Find(_mainCamera).transform;
        if (_mainCameraTransform == null) Debug.Log("камера не нашлась");
        UpdateStickPosition();

        _spriteRendererPlayer = GetComponent<SpriteRenderer>();
        _spriteRendererMirror = _mirror.GetComponent<MirrorView>().SpriteRenderer;
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

        if (Input.GetKeyDown(_activationKey))
        {
            _player.position = _mirror.position;
        }

        if (Input.GetKey(_moveLeftMirror))
        {
            if (_playerFacesRight)
            {
                MoveMirrirLeft();
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
                MoveMirrirLeft();
            }
        }

        void MoveMirrirLeft()
        {
            _playerCharacter.cameraHorizontalFacingOffset -= _movingSpeedMirror * Time.deltaTime;
            if (_playerCharacter.cameraHorizontalFacingOffset < _minCameraOffset)
                _playerCharacter.cameraHorizontalFacingOffset = _minCameraOffset;
            _mirrorPositionOffset.x = _playerCharacter.cameraHorizontalFacingOffset;
        }

        void MoveMirrorRight()
        {
            _playerCharacter.cameraHorizontalFacingOffset += _movingSpeedMirror * Time.deltaTime;
            if (_playerCharacter.cameraHorizontalFacingOffset > _maxCameraOffset)
                _playerCharacter.cameraHorizontalFacingOffset = _maxCameraOffset;
            _mirrorPositionOffset.x = _playerCharacter.cameraHorizontalFacingOffset;
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

        UpdateStickPosition();
        _spriteRendererMirror.sprite = _spriteRendererPlayer.sprite;
    }

    private void TurnAroundSystem()
    {
        _playerCharacter.cameraHorizontalFacingOffset *= -1;
        _mirrorPositionOffset.x = _playerCharacter.cameraHorizontalFacingOffset;
    }

    private void UpdateStickPosition()
    {
        Vector3 stickPosition = new Vector3(_mainCameraTransform.position.x, _mainCameraTransform.position.y, _player.position.z);
        _stick.position = stickPosition;
    }
}
