using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyManager : MonoBehaviour
{
    // 定数定義
    private const int ENEMY_POINT = 500;// 敵撃破ポイント
    // public変数
    public LayerMask blockLayer;        // ブロックレイヤーオブジェクト
    // メンバ変数
    private Rigidbody2D rbody;          // 敵制御用Rigidbody2D
    private GameObject gameManager;     // ゲームマネージャーオブジェクト
    private float moveSpeed = 1;        // 敵移動速度
    public enum MOVE_DIR                // 移動方向定義列挙体
    {
        LEFT,
        RIGHT,
    };
    private MOVE_DIR moveDirection = MOVE_DIR.LEFT; // 移動方向変数

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();            // Rigidbody2Dオブジェクト取得
        gameManager = GameObject.Find("GameManager");   // GameManagerオブジェクト取得
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 敵の移動処理
    void FixedUpdate()
    {
        bool isBlock;       // 進行方向にブロックがあるか否か

        switch (moveDirection)
        {
            case MOVE_DIR.LEFT:     // 左に移動
                // 左に移動するためRigidBodyにvelocityを設定（x軸は-1、y軸はそのまま）
                rbody.velocity = new Vector2(moveSpeed * -1, rbody.velocity.y);
                // 左移動の画像の向き（x,yともにそのまま）
                transform.localScale = new Vector2(1, 1);

                // ブロック衝突判定をLinecastにて行う、transform.positionにてキャラの位置座標を取得
                isBlock = Physics2D.Linecast(
                    // Linecastのstart点、y軸はキャラのcirclecolliderの半径が0.25なので、円の一番上を指定
                    new Vector2(transform.position.x, transform.position.y + 0.5f),
                    // Linecastのend点、サークルの円より0.3先（左向きx軸マイナス）つまり鼻先を指定
                    new Vector2(transform.position.x - 0.3f, transform.position.y + 0.5f),
                    // LayerMaskにはblockLayerを指定し、Blockのみ判定
                    blockLayer
                );
                // 進行方向にブロックがある
                if (isBlock)
                {
                    // 右を向く
                    moveDirection = MOVE_DIR.RIGHT;
                }
                break;
            case MOVE_DIR.RIGHT:    // 右に移動中
                // 右に移動するためRigidBodyにvelocityを設定（x、y軸はそのまま）
                rbody.velocity = new Vector2(moveSpeed, rbody.velocity.y);
                // 右移動の画像の向き（xは反転,yそのまま）
                transform.localScale = new Vector2(-1, 1);

                // ブロック衝突判定をLinecastにて行う、transform.positionにてキャラの位置座標を取得
                isBlock = Physics2D.Linecast(
                    // Linecastのstart点、y軸はキャラのcirclecolliderの半径が0.25なので、円の一番上を指定
                    new Vector2(transform.position.x, transform.position.y + 0.5f),
                    // Linecastのend点、サークルの円より0.3先（右向きx軸プラス）
                    new Vector2(transform.position.x + 0.3f, transform.position.y + 0.5f),
                    // LayerMaskにはblockLayerを指定し、Blockのみ判定
                    blockLayer
                );
                // 進行方向にブロックがある
                if (isBlock)
                {
                    // 左を向く
                    moveDirection = MOVE_DIR.LEFT;
                }
                break;
        }
    }

    /// <summary>
    /// 敵オブジェクト削除処理
    /// </summary>
    public void DestroyEnemy()
    {
        gameManager.GetComponent<GameManager>().AddScore(ENEMY_POINT);

        // Enemyを止める
        rbody.velocity = new Vector2(0, 0);
        // コライダー削除
        CircleCollider2D　circleCollider = GetComponent<CircleCollider2D>();
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        Destroy(circleCollider);
        Destroy(boxCollider);

        // 死亡アニメーション
        Sequence animSet = DOTween.Sequence();
        animSet.Append(transform.DOLocalMoveY(0.5f, 0.2f).SetRelative());
        animSet.Append(transform.DOLocalMoveY(-10.0f, 1.0f).SetRelative());

        Destroy(this.gameObject, 1.2f);
    }
}
