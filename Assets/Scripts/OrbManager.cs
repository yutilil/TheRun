using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OrbManager : MonoBehaviour
{
    // 定数定義
    private const int ORB_POINT = 100;  // オーブの得点

    // private変数
    private GameObject gameManager;     // GameManagerオブジェクト

    // Start is called before the first frame update
    void Start()
    {
        // GameManagerインスタンス取得
        gameManager = GameObject.Find("GameManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// オーブ入手処理
    /// </summary>
    public void GetOrb()
    {
        gameManager.GetComponent<GameManager>().AddScore(ORB_POINT);
        // コライダー削除
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        Destroy(circleCollider);
        // 消失アニメーション
        transform.DOScale(3.5f, 0.3f);
        SpriteRenderer spriteRenderer = transform.GetComponent<SpriteRenderer>();
        DOTween.ToAlpha(() => spriteRenderer.color, a => spriteRenderer.color = a, 0.0f, 0.3f);

        Destroy(this.gameObject, 0.5f);
    }
}
