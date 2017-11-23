using UnityEngine;
using System.Collections;

using UnityEngine.Events;
using UnityEngine.UI;
using System;
using MyTcpCommandLibrary.Model;

public class GameScoreHandler : MonoBehaviour
{

    public static UpdateScoreEvent updateScoreEvent = new UpdateScoreEvent();
    public static UpdateLivesEvent updateLivesEvent = new UpdateLivesEvent();
    public static UpdateMissedEvent updateMissedEvent = new UpdateMissedEvent();

    public static UnityEvent SendUpdateScoreEvent = new UnityEvent();
   // public static UnityEvent<GameScoreTempModel> SetGameScoreUI;



    public Text userNameText;
    public Text now_scoreText;
    public Text now_livesText;
    public Text now_missedText;
    public Text serverText;
    
    public Text last_scoreText;
    public Text last_missedText;

    // Use this for initialization
    void Start()
    {
        updateScoreEvent.AddListener( OnUpdateScore);
        updateLivesEvent.AddListener(OnUpdateLives);
        updateMissedEvent.AddListener(OnUpdateMissed);
        SendUpdateScoreEvent.AddListener(OnSendUpdateScore);
        MainClient.Instance.setGameScoreTempModelEvent.AddListener(SetLastData);

    }

    private void OnSendUpdateScore()
    {
        //throw new NotImplementedException();
        serverText.text = "积分已发往服务器...";
    }

    private void OnUpdateScore(int _score)
    {
       
        now_scoreText.text = "积分：" + _score.ToString();
       
    }
    void OnDestory()
    {

        updateScoreEvent.RemoveListener(OnUpdateScore);
        updateLivesEvent.RemoveListener(OnUpdateLives);
        updateMissedEvent.RemoveListener(OnUpdateMissed);
        SendUpdateScoreEvent.RemoveListener(OnSendUpdateScore);
        MainClient.Instance.setGameScoreTempModelEvent.RemoveListener(SetLastData);


    }
    private void OnUpdateLives(int _lives)
    {
        now_livesText.text = "生命：" + _lives.ToString();
       
    }

    private void OnUpdateMissed(int _missed)
    {
       
        now_missedText.text = "放走敌人：" + _missed.ToString();
    }

    private void OnDestroy()
    {

    }

    public void SetUserNameData(string _uname)
    {
        userNameText.text = _uname;
    }



    public void SetLastData(GameScoreTempModel gsModel)
    {
        //userNameText.text = "生命：" + gsModel.userName;
        //last_scoreText.text = "积分：" + gsModel.score.ToString();
        //last_missedText.text = "放走敌人：" + gsModel.missenemy.ToString();
        tempScore = gsModel;
    }

    public void SetLastDataText()
    {
        userNameText.text = "生命：" + tempScore.userName;
        last_scoreText.text = "积分：" + tempScore.score.ToString();
        last_missedText.text = "放走敌人：" + tempScore.missenemy.ToString();
    }

    public void SetNowDate(int _lives, int _lastScore, int _lastMissed)
    {
        now_livesText.text ="生命："+ _lives.ToString();
        now_scoreText.text = "积分：" + _lastScore.ToString();
        now_missedText.text = "放走敌人：" + _lastMissed.ToString();
    }

    GameScoreTempModel tempScore;
    // Update is called once per frame
    void Update()
    {
        if(tempScore != null)
        {
            SetLastDataText();
            tempScore = null;
        }
    }

    public void ReloadGameScene()
    {
        //跳转场景
        MainClient.Instance.SetLoadSceneFlag();


        //发送读取上次分数数据的逻辑
        MainClient.Instance.SendCheckUserScore();
    }


}
public class UpdateScoreEvent: UnityEvent<int> { }
public class UpdateLivesEvent : UnityEvent<int> { }
public class UpdateMissedEvent : UnityEvent<int> { }