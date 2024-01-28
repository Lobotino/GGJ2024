using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vector2 = UnityEngine.Vector2;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] public RuntimeAnimatorController redPlayerAnimatorController;
    [SerializeField] public RuntimeAnimatorController bluePlayerAnimatorController;

    public float speed = 10f;

    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private PlayerNetworkParams _playerNetworkParams;
    private Animator _animator;

    private bool isMoveHorizontal, isFlip, isMoveForward, isMoveBackward, isKnifes, isBalance, isBalls, isIdle = true;
    private static readonly int IsMoveForward = Animator.StringToHash("isMoveForward");
    private static readonly int IsMoveHorizontal = Animator.StringToHash("isMoveHorizontal");
    private static readonly int IsMoveBackward = Animator.StringToHash("isMoveBackward");
    private static readonly int IsIdle = Animator.StringToHash("isIdle");
    private static readonly int IsBalls = Animator.StringToHash("isBalls");
    private static readonly int IsBalance = Animator.StringToHash("isBalance");
    private static readonly int IsKnifes = Animator.StringToHash("isKnifes");

    private bool inputLeft, inputRight, inputUp, inputDown, inputAction;

    private TimeClickMiniGame _currentPlayingGame;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _playerNetworkParams = GetComponent<PlayerNetworkParams>();

        UpdatePlayerColor(_playerNetworkParams.PlayerNumber);
        UpdatePlayerSpawn(_playerNetworkParams.PlayerNumber);
    }

    void FixedUpdate()
    {
        CheckSound();
        CheckAnimation();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // gameController.Disconnect();
            Runner.Shutdown();
            SceneManager.LoadScene("MENU_BEATY");
        }

        inputLeft = Input.GetKey(KeyCode.A);
        inputRight = Input.GetKey(KeyCode.D);
        inputUp = Input.GetKey(KeyCode.W);
        inputDown = Input.GetKey(KeyCode.S);

        inputAction = Input.GetKeyDown(KeyCode.E);
        if (inputAction)
        {
            if (_currentPlayingGame == null)
            {
                DoAction();
                SyncNetworkAnimation();
            }
            else
            {
                _currentPlayingGame.StopPlaying();
                _currentPlayingGame = null;
                isBalance = false;
                isBalls = false;
                SyncNetworkAnimation();
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        // Only move own player and not every other player. Each player controls its own player object.
        if (HasStateAuthority && PlayerPrefs.GetInt("GameStarted") == 1)
        {
            if (_currentPlayingGame == null)
            {
                //Update своего персонажа
                MovePlayer(gameObject.transform.position);
            }
            else
            {
                SyncNetworkAnimation();
            }
        }
    }

    public void DoAction()
    {
        if (!HasInputAuthority) return;

        var colliders = Physics2D.OverlapCircleAll(gameObject.transform.position, 1f);
        foreach (var col in colliders)
        {
            switch (col.tag)
            {
                case "KingFunGame":
                    _currentPlayingGame = col.gameObject.GetComponent<TimeClickMiniGame>();
                    var startPlaying = _currentPlayingGame.StartPlaying(gameObject.transform);

                    if (startPlaying)
                    {
                        switch (_currentPlayingGame.miniGameType)
                        {
                            case "Balance":
                            {
                                AnimBalance();
                                break;
                            }
                            case "Balls":
                            {
                                AnimBalls();
                                break;
                            }
                            case "Knifes":
                            {
                                AnimKnifes();
                                break;
                            }
                        }
                    }

                    break;

                case "Pickable":
                    col.gameObject.GetComponent<ItemPickable>().PickUp();
                    break;

                case "FinalPaper":
                    col.gameObject.GetComponent<EndGameScript>().FinishGame();
                    break;
            }
        }
    }

    private void SyncAnimationOtherPlayer()
    {
        if (!(inputLeft && inputRight))
        {
            if (inputLeft)
            {
                isMoveHorizontal = true;
                isFlip = false;
            }
            else
            {
                if (inputRight)
                {
                    isMoveHorizontal = true;
                    isFlip = true;
                }
                else
                {
                    isMoveHorizontal = false;
                }
            }
        }

        if (!(inputUp && inputDown))
        {
            if (inputDown)
            {
                isMoveBackward = true;
                isMoveForward = false;
            }
            else
            {
                if (inputUp)
                {
                    isMoveForward = true;
                    isMoveBackward = false;
                }
                else
                {
                    isMoveBackward = false;
                    isMoveForward = false;
                }
            }
        }
    }

    private void MovePlayer(Vector2 position)
    {
        float resultXMove = position.x, resultYMove = position.y;

        if (!(inputLeft && inputRight))
        {
            if (inputLeft)
            {
                resultXMove = position.x - speed;
                isMoveHorizontal = true;
                isFlip = false;
            }
            else
            {
                if (inputRight)
                {
                    resultXMove = position.x + speed;
                    isMoveHorizontal = true;
                    isFlip = true;
                }
                else
                {
                    isMoveHorizontal = false;
                }
            }
        }

        if (!(inputUp && inputDown))
        {
            if (inputDown)
            {
                isMoveBackward = true;
                isMoveForward = false;
                resultYMove = position.y - speed;
            }
            else
            {
                if (inputUp)
                {
                    resultYMove = position.y + speed;
                    isMoveForward = true;
                    isMoveBackward = false;
                }
                else
                {
                    isMoveBackward = false;
                    isMoveForward = false;
                }
            }
        }

        SyncNetworkAnimation();

        if (_rigidbody != null)
        {
            _rigidbody.MovePosition(Vector2.Lerp(position, new Vector2(resultXMove, resultYMove), Runner.DeltaTime));
        }
    }

    private void SyncNetworkAnimation()
    {
        if (_playerNetworkParams == null)
        {
            return;
        }

        if (isBalls)
        {
            _playerNetworkParams.AnimationState = 5;
            return;
        }

        if (isBalance)
        {
            _playerNetworkParams.AnimationState = 6;
            return;
        }

        if (isKnifes)
        {
            _playerNetworkParams.AnimationState = 7;
            return;
        }

        if (isIdle)
        {
            _playerNetworkParams.AnimationState = 0;
            return;
        }
        //
        // if (isIdle)
        // {
        //     if (_currentPlayingGame != null)
        //     {
        //         switch (_currentPlayingGame.miniGameType)
        //         {
        //             case "Balls":
        //             {
        //                 _playerNetworkParams.AnimationState = 5;
        //                 break;
        //             }
        //             case "Balance":
        //             {
        //                 _playerNetworkParams.AnimationState = 6;
        //                 break;
        //             }
        //             case "Knifes":
        //             {
        //                 _playerNetworkParams.AnimationState = 7;
        //                 break;
        //             }
        //         }
        //     }
        //     else
        //     {
        //         _playerNetworkParams.AnimationState = 0;
        //     }
        //
        //     return;
        // }

        if (isMoveForward)
        {
            _playerNetworkParams.AnimationState = 1;
            return;
        }

        if (isMoveBackward)
        {
            _playerNetworkParams.AnimationState = 2;
            return;
        }

        if (isMoveHorizontal)
        {
            _playerNetworkParams.AnimationState = isFlip ? 4 : 3;
        }
    }

    private void CheckSound()
    {
        //        if (Math.Abs(_rigidbody.velocity.x) > 0.01) //сука звук
//        {
//            walkAudio.Play();
//        }
//        else
//        {
//            walkAudio.Pause();
//        }
    }

    private void CheckAnimation()
    {
        _spriteRenderer.flipX = isFlip;

        if (isBalance)
        {
            AnimBalance();
            return;
        }

        if (isBalls)
        {
            AnimBalls();
            return;
        }
        
        if (isMoveForward)
        {
            AnimMoveForward();
        }
        else
        {
            if (isMoveHorizontal)
            {
                AnimMoveHorizontal();
            }
            else
            {
                if (isMoveBackward)
                {
                    AnimMoveBackward();
                }
                else
                {
                    if (_currentPlayingGame != null)
                    {
                        switch (_currentPlayingGame.miniGameType)
                        {
                            case "Balls":
                            {
                                AnimBalls();
                                break;
                            }
                            case "Balance":
                            {
                                AnimBalance();
                                break;
                            }
                            case "Knifes":
                            {
                                AnimKnifes();
                                break;
                            }
                        }
                    }
                    else
                    {
                        AnimIdle();
                    }
                }
            }
        }
    }

    private void AnimMoveForward()
    {
        isKnifes = false;
        isBalance = false;
        isBalls = false;
        isMoveForward = true;
        isMoveBackward = false;
        isMoveHorizontal = false;
        isIdle = false;
        SyncWithAnimator();
    }

    private void AnimMoveBackward()
    {
        isKnifes = false;
        isBalance = false;
        isBalls = false;
        isMoveForward = false;
        isMoveBackward = true;
        isMoveHorizontal = false;
        isIdle = false;
        SyncWithAnimator();
    }

    private void AnimMoveHorizontal()
    {
        isKnifes = false;
        isBalance = false;
        isBalls = false;
        isMoveForward = false;
        isMoveBackward = false;
        isMoveHorizontal = true;
        isIdle = false;
        SyncWithAnimator();
    }

    private void AnimIdle()
    {
        isKnifes = false;
        isBalance = false;
        isBalls = false;
        isMoveForward = false;
        isMoveBackward = false;
        isMoveHorizontal = false;
        isIdle = true;
        SyncWithAnimator();
    }

    private void AnimBalls()
    {
        isKnifes = false;
        isBalance = false;
        isBalls = true;
        isMoveForward = false;
        isMoveBackward = false;
        isMoveHorizontal = false;
        isIdle = false;
        SyncWithAnimator();
    }

    private void AnimBalance()
    {
        isKnifes = false;
        isBalance = true;
        isBalls = false;
        isMoveForward = false;
        isMoveBackward = false;
        isMoveHorizontal = false;
        isIdle = false;
        SyncWithAnimator();
    }

    private void AnimKnifes()
    {
        isKnifes = true;
        isBalance = false;
        isBalls = false;
        isMoveForward = false;
        isMoveBackward = false;
        isMoveHorizontal = false;
        isIdle = false;
        SyncWithAnimator();
    }

    private void SyncWithAnimator()
    {
        _animator.SetBool(IsMoveForward, isMoveForward);
        _animator.SetBool(IsMoveBackward, isMoveBackward);
        _animator.SetBool(IsMoveHorizontal, isMoveHorizontal);
        _animator.SetBool(IsIdle, isIdle);
        _animator.SetBool(IsBalls, isBalls);
        _animator.SetBool(IsBalance, isBalance);
        // _animator.SetBool(IsKnifes, isKnifes);
    }

    public void UpdatePlayerColor(int playerNumber)
    {
        _animator.runtimeAnimatorController =
            playerNumber % 2 == 0 ? bluePlayerAnimatorController : redPlayerAnimatorController;
    }

    private void UpdatePlayerSpawn(int playerNumber)
    {
        transform.position = playerNumber % 2 == 0 ? new Vector3(31.04f, -9.62f, -2) : new Vector3(27.88f, -9.62f, -2);
    }

    public void UpdateAnimationState(int animationState)
    {
        switch (animationState)
        {
            case 0:
            {
                isBalance = false;
                isKnifes = false;
                isBalls = false;
                isIdle = true;
                isMoveForward = false;
                isMoveBackward = false;
                isMoveHorizontal = false;
                break;
            }
            case 1:
            {
                isBalance = false;
                isKnifes = false;
                isBalls = false;
                isIdle = false;
                isMoveForward = true;
                isMoveBackward = false;
                isMoveHorizontal = false;
                break;
            }
            case 2:
            {
                isBalance = false;
                isKnifes = false;
                isBalls = false;
                isIdle = false;
                isMoveForward = false;
                isMoveBackward = true;
                isMoveHorizontal = false;
                break;
            }
            case 3:
            {
                isBalance = false;
                isKnifes = false;
                isBalls = false;
                isIdle = false;
                isMoveForward = false;
                isMoveBackward = false;
                isMoveHorizontal = true;
                isFlip = false;
                break;
            }
            case 4:
            {
                isBalance = false;
                isKnifes = false;
                isBalls = false;
                isIdle = false;
                isMoveForward = false;
                isMoveBackward = false;
                isMoveHorizontal = true;
                isFlip = true;
                break;
            }
            case 5: //balls
            {
                isBalance = false;
                isKnifes = false;
                isBalls = true;
                isIdle = false;
                isMoveForward = false;
                isMoveBackward = false;
                isMoveHorizontal = false;
                break;
            }
            case 6: //balance   
            {
                isBalance = true;
                isKnifes = false;
                isBalls = false;
                isIdle = false;
                isMoveForward = false;
                isMoveBackward = false;
                isMoveHorizontal = false;
                break;
            }
            case 7: //knifes
            {
                isBalance = true;
                isKnifes = false;
                isBalls = false;
                isIdle = false;
                isMoveForward = false;
                isMoveBackward = false;
                isMoveHorizontal = false;
                break;
            }
        }
    }
}