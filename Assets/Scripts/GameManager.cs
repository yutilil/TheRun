using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 定数定義
    private const int MAX_SCORE = 999999;   // スコア最大値
    // public変数
    public GameObject textGameOver;     // ゲームオーバーテキストオブジェクト
    public GameObject textClear;        // クリアテキストオブジェクト
    public GameObject buttons;          // ボタンオブジェクト
    public GameObject textScoreNumber;  // スコアテキスト
    public enum GAME_MODE               // ゲーム状態定義列挙体
    {
        PLAY,                           // プレイ中
        CLEAR,                          // クリア
        GAMEOVER,                       // ゲームオーバー
    };
    public GAME_MODE gameMode = GAME_MODE.PLAY; // ゲーム状態変数定義
    public AudioClip clearSE;           // 効果音：クリア
    public AudioClip gameoverSE;        // 効果音：ゲームオーバー
    public int stageNo;                 // ステージナンバー
    
    // private変数
    private int score = 0;              // スコア
    private int displayScore = 0;       // 表示用スコア
    private AudioSource audioSource;    // オーディオソースオブジェクト

    // Start is called before the first frame update
    void Start()
    {
        // オーディオソースインスタンス取得
        audioSource = this.gameObject.GetComponent<AudioSource>();

        // スコア表示初期化
        RefreshScore();
    }

    // Update is called once per frame
    void Update()
    {
        // フレーム更新ごとに呼ばれるので、繰り返し処理になる
        if (score > displayScore)
        {
            // 10ずつ更新
            displayScore += 10;

            if (displayScore > score)
            {
                // 表示スコアがスコアより大きくなったらスコアを代入
                displayScore = score;
            }

            // スコア表示更新
            RefreshScore();
        }
    }

    /// <summary>
    /// GameOver処理
    /// </summary>
    public void GameOver()
    {
        // GameOverSE再生
        audioSource.PlayOneShot(gameoverSE);
        // GameOverテキスト表示
        textGameOver.SetActive(true);
        // buttons(left,right,jump)非表示
        buttons.SetActive(false);

        // ステージセレクトに戻る
        Invoke("GoBackStageSelect", 2.0f);
    }
    /// <summary>
    /// Clear処理
    /// </summary>
    public void GameClear()
    {
        audioSource.PlayOneShot(clearSE);
        gameMode = GAME_MODE.CLEAR;
        textClear.SetActive(true);
        buttons.SetActive(false);

        // セーブデータ更新
        if (PlayerPrefs.GetInt("CLEAR", 0) < stageNo)
        {
            PlayerPrefs.SetInt("CLEAR", stageNo);
        }

        // ステージセレクトに戻る
        Invoke("GoBackStageSelect", 2.0f);
    }
    /// <summary>
    /// スコア加算
    /// </summary>
    /// <param name="val">取得スコア値</param>
    public void AddScore(int val)
    {
        score += val;
        // 最大スコアより大きければ、最大スコアを代入
        if (score > MAX_SCORE)
        {
            score = MAX_SCORE;
        }
    }

    // スコア表示更新
    void RefreshScore()
    {
        textScoreNumber.GetComponent<Text>().text = displayScore.ToString();
    }

    // ステージセレクトシーンに戻る
    void GoBackStageSelect()
    {
        SceneManager.LoadScene("StageSelectScene");
    }
}
