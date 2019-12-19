using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerManager : MonoBehaviour
{
    // 定数定義
    private const float MOVE_SPEED = 3; // 移動速度固定値

    // public変数
    public LayerMask blockLayer;    // Blockレイヤー設定変数
    public GameObject gameManager;  // GameManagerオブジェクト
    public AudioClip jumpSE;        // 効果音：ジャンプ
    public AudioClip getSE;         // 効果音：オーブゲット
    public AudioClip stampSE;       // 効果音：Enemy踏みつけ

    // メンバ変数
    private Rigidbody2D rbody;      // プレイヤー制御用Rigidbody2D
    private Animator animator;      // アニメーターオブジェクト
    private AudioSource audioSource;// オーディオソースオブジェクト
    private float moveSpeed;        // 移動速度
    public enum MOVE_DIR            // 移動状態列挙体
    {
        STOP,
        LEFT,
        RIGHT,
    };
    private MOVE_DIR moveDirection = MOVE_DIR.STOP; // enum型移動方向
    private float jumpPower = 400;  // ジャンプ力
    private bool goJump = false;    // ジャンプしたか否か
    private bool canJump = false;   // ブロックに接しているか
    private bool usingButtons = false;  // ボタンを押しているか否か
    
    // Start is called before the first frame update
    void Start()
    {
        // PlayerのRigidbody2Dコンポーネントセット
        rbody = GetComponent<Rigidbody2D>();
        // PlayerのAnimatorオブジェクトセット
        animator = GetComponent<Animator>();
        // GameManagerのAudioSourceオブジェクト取得
        audioSource = gameManager.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        canJump = 
        Physics2D.Linecast(
            transform.position - (transform.right * 0.3f), transform.position - (transform.up * 0.1f),
            blockLayer
        )
        ||
        Physics2D.Linecast(
            transform.position + (transform.right * 0.3f), transform.position - (transform.up * 0.1f),
            blockLayer
        );

        // Jump状態かどうかanimatorにセット
        animator.SetBool("onGround", canJump);

        // キーボードボタン使用処理
        if (!usingButtons)
        {
            float x = Input.GetAxisRaw("Horizontal");
            if (x == 0)
            {
                moveDirection = MOVE_DIR.STOP;
            }
            else
            {
                if (x < 0)
                {
                    moveDirection = MOVE_DIR.LEFT;
                }
                else
                {
                    moveDirection = MOVE_DIR.RIGHT;
                }
            }
            if (Input.GetKeyDown("space"))
            {
                PushJumpButton();
            }
        }
    }

    // 常に固定間隔で呼び出される
    void FixedUpdate()
    {
        // 移動方向で処理分岐
        switch (moveDirection)
        {
            case MOVE_DIR.STOP:     // 停止
                moveSpeed = 0;
                break;
            case MOVE_DIR.LEFT:     // 左移動（座標はマイナス方向になる）
                moveSpeed = MOVE_SPEED * -1;
                transform.localScale = new Vector2(-1, 1);  // Player画像-1（LEFT）向き
                break;
            case MOVE_DIR.RIGHT:
                moveSpeed = MOVE_SPEED;
                transform.localScale = new Vector2(1, 1);   // Playre画像+1（RIGHT）向き
                break;
        }
        // Player速度　x軸　moveSpeed, y軸 rbody.velocity.y（デフォルト値）
        rbody.velocity = new Vector2(moveSpeed, rbody.velocity.y);

        // ジャンプ処理
        if (goJump)
        {
            // ジャンプSE
            audioSource.PlayOneShot(jumpSE);
            // Vector2.upまっすぐ上
            rbody.AddForce(Vector2.up * jumpPower);
            // ジャンプ中に呼ばれてもfalse
            goJump = false;
        }
    }

    /// <summary>
    /// LeftButton押下処理
    /// </summary>
    public void PushLeftButton()
    {
        moveDirection = MOVE_DIR.LEFT;
        // キーボード処理
        usingButtons = true;
    }
    /// <summary>
    /// RightButton押下処理
    /// </summary>
    public void PushRightButton()
    {
        moveDirection = MOVE_DIR.RIGHT;
        // キーボード処理
        usingButtons = true;
    }
    /// <summary>
    /// ButtonRelease処理
    /// </summary>
    public void ReleaseMoveButton()
    {
        moveDirection = MOVE_DIR.STOP;
        // キーボード処理
        usingButtons = false;
    }
    /// <summary>
    /// ジャンプボタン押下処理
    /// </summary>
    public void PushJumpButton()
    {
        if (canJump)
        {
            // ジャンプ可能に設定
            goJump = true;
        }
    }

    // Trapタグ衝突処理
    void OnTriggerEnter2D(Collider2D col) 
    {
        // プレイ中のみ判定（CLEAR時はreturn）
        if (gameManager.GetComponent<GameManager>().gameMode != GameManager.GAME_MODE.PLAY)
        {
            return;
        }
        // 衝突相手(ここでは穴の下のオブジェクト)を参照
        if (col.gameObject.tag == "Trap")
        {
            gameManager.GetComponent<GameManager>().GameOver();
            DestroyPlayer();
        }
        // Goalに衝突した場合
        if (col.gameObject.tag == "Goal")
        {
            // GameManagerのゲームクリア処理
            gameManager.GetComponent<GameManager>().GameClear();
        }
        // Enemyに衝突した場合
        if (col.gameObject.tag == "Enemy")
        {
            // 踏んだ場合（Playerのy軸がEnemyのy軸より0.4上ならば）
            if (transform.position.y > col.gameObject.transform.position.y + 0.4f)
            {
                // 踏みつけSE
                audioSource.PlayOneShot(stampSE);
                // Playerのx軸方向の現在速度を設定、y軸は0
                rbody.velocity = new Vector2(rbody.velocity.x, 0);
                // Playerをその位置からジャンプさせる
                rbody.AddForce(Vector2.up * jumpPower * 1.5f);
                // Enemy削除処理
                col.gameObject.GetComponent<EnemyManager>().DestroyEnemy();
            }
            else
            {
                // 上からの接触ではない場合Gameover
                gameManager.GetComponent<GameManager>().GameOver();
                DestroyPlayer();
            }
        }
        // Orbに衝突した場合
        if (col.gameObject.tag == "Orb")
        {
            audioSource.PlayOneShot(getSE);
            col.gameObject.GetComponent<OrbManager>().GetOrb();
        }
    }

    // Playerオブジェクト削除処理
    void DestroyPlayer()
    {
        // GAME_MODEをGAMAOVERに変更
        gameManager.GetComponent<GameManager>().gameMode = GameManager.GAME_MODE.GAMEOVER;

        // コライダー削除
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        Destroy(circleCollider);
        Destroy(boxCollider);

        // 死亡アニメーション
        Sequence animSet = DOTween.Sequence();
        animSet.Append(transform.DOLocalMoveY(2.0f, 0.5f).SetRelative());
        animSet.Append(transform.DOLocalMoveY(-10.0f, 1.5f).SetRelative());

        // 1.2秒後に削除
        Destroy(this.gameObject, 1.2f);
    }
}
