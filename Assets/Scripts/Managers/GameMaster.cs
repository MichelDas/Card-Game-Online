using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameMaster : MonoBehaviourPunCallbacks
{
    [SerializeField] Battler player;
    [SerializeField] Battler enemy;
    [SerializeField] CardGenerator cardGenerator;
    [SerializeField] GameUI gameUI;
    RuleBook ruleBook;
    bool playerRetryReady;
    bool enemyRetryReady;

    private void Awake()
    {
        ruleBook = GetComponent<RuleBook>();
    }

    private void Start()
    {
        Setup();   
    }

    void Setup()
    {
        playerRetryReady = false;
        enemyRetryReady = false;
        gameUI.Init();
        player.Life = 4;
        enemy.Life = 4;
        gameUI.ShowLifes(player.Life, enemy.Life);
        gameUI.UpdateAddNumber(player.AddNumber, enemy.AddNumber);
        player.OnSubmitAction = SubmittedAction;
        enemy.OnSubmitAction = SubmittedAction;
        SetupCards(battler: player, isEnemy: false);
        SetupCards(battler: enemy, isEnemy: true);

        if (GameDataManager.Instance.IsOnlineBattle)
        {
            player.OnSubmitAction += SendPlayerCard;
        }
    }

    void SubmittedAction()
    {
        if(player.IsSubmitted && enemy.IsSubmitted)
        {
            //Cardの　勝利判定(しょうりはんてい）（check if win or not)
            StartCoroutine(CardBattle());
        }
        else　if (player.IsSubmitted)
        {            
            if (GameDataManager.Instance.IsOnlineBattle == false)
            {
                // Enemyからカードを出す
                enemy.RandomSubmit();
            }

        }
        else if (enemy.IsSubmitted)
        {
            //Playerの提出を待つ
        }
    }

    void SetupCards(Battler battler, bool isEnemy)
    {
        for (int i = 0; i < 8; i++)
        {
            Card card = cardGenerator.Spawn(i, isEnemy);
            battler.SetCardToHand(card);
        }
        battler.Hand.ResetPosition();
    }

    // Card勝利判定（しょうりはんてい）
    // ちょっと遅らせてから結果を表示：コルーチン使う
    IEnumerator CardBattle()
    {
        yield return new WaitForSeconds(.6f);
        enemy.SubmitCard.Open();
        yield return new WaitForSeconds(1);
        Result result = ruleBook.GetResult(player, enemy);
        //Debug.Log(result);
        switch (result)
        {
            case Result.TurnWin:
            case Result.GameWin:
                gameUI.ShowTurnResult("WIN");
                enemy.Life--;
                break;
            case Result.TurnWin2:
                gameUI.ShowTurnResult("WIN");
                enemy.Life -= 2;
                break;
            case Result.TurnLose:
            case Result.GameLose:
                gameUI.ShowTurnResult("LOSE");
                player.Life--;
                break;
            case Result.TurnLose2:
                gameUI.ShowTurnResult("LOSE");
                player.Life -= 2;
                break;
            case Result.TurnDraw:
                gameUI.ShowTurnResult("DRAW");
                break;
        }

        gameUI.ShowLifes(player.Life, enemy.Life);
        yield return new WaitForSeconds(1f);
        if ((player.Hand.IsEmpty) || (result == Result.GameWin) || (result == Result.GameLose)
            || (player.Life <= 0 || enemy.Life <= 0))
        {
            ShowResult(result);
        }
        else
        {
            SetupNextTurn();
        }
    }

    void ShowResult(Result result)
    {

        if(result == Result.GameWin)
        {
            gameUI.ShowGameResult("WIN");
        }
        else if (result == Result.GameLose)
        {
            gameUI.ShowGameResult("LOSE");
        }
        else if (player.Life <= 0 && enemy.Life <= 0)
        {
            gameUI.ShowGameResult("DRAW");
        }
        else if (player.Life <= 0 )
        {
            gameUI.ShowGameResult("LOSE");
        }
        else if (enemy.Life <= 0)
        {
            gameUI.ShowGameResult("WIN");
        }
        else if(player.Life > enemy.Life)
        {
            gameUI.ShowGameResult("WIN");
        }
        else if (player.Life < enemy.Life)
        {
            gameUI.ShowGameResult("LOSE");
        }
        else
        {
            gameUI.ShowGameResult("DRAW");
        }
    }

    // 表示を終わったら、次のターンにうつる（場のカードを捨てる） 
    void SetupNextTurn()
    {
        player.SetupNextTurn();
        enemy.SetupNextTurn();
        gameUI.SetupNextTurn();
        gameUI.UpdateAddNumber(player.AddNumber, enemy.AddNumber);

        if (enemy.IsFirstSubmit)
        {
            enemy.IsFirstSubmit = false;
            if (GameDataManager.Instance.IsOnlineBattle)
            {
                enemy.RandomSubmit();
                enemy.SubmitCard.Open();
            }
        }

        if(player.IsFirstSubmit)
        {
            // PLAYER WILL put card first
            // this will be usefull in online
        }

    }

    public void OnRetryButton()
    {
        playerRetryReady = true;
        if (GameDataManager.Instance.IsOnlineBattle)
        {
            gameUI.HideRetryButton();
            SendRetryMessage();
            if (playerRetryReady && enemyRetryReady)
            {
                string currentScene = SceneManager.GetActiveScene().name;
                FadeManager.Instance.LoadScene(currentScene, 1f);
            }
            
        }
        else
        {
            string currentScene = SceneManager.GetActiveScene().name;
            FadeManager.Instance.LoadScene(currentScene, 1f);
        }

    }

    public void OnTitleButton()
    {
        if (GameDataManager.Instance.IsOnlineBattle)
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.Disconnect();
            }
        }
        FadeManager.Instance.LoadScene("Menu", 1f);
    }

    // When Player submits a card
    void SendPlayerCard()
    {
        photonView.RPC(nameof(RPCOnRecievedCard), RpcTarget.Others, player.SubmitCard.Base.Number);
    }

    // Here the player on the other side sends the number of card.
    // previously in the current player, The enemy (CPU) removed (submitted) a card with random number.
    // Now the player on the other side send me the card number he submitted.
    // So In my System the Enemy will submit card of that number instead of a random number

    [PunRPC]
    void RPCOnRecievedCard(int number)
    {
        enemy.SetSubmitCard(number);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (GameDataManager.Instance.IsOnlineBattle)
        {
            gameUI.ShowLeftPanel();
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.Disconnect();
            }
        }
    }

    private void SendRetryMessage()
    {
        photonView.RPC(nameof(OnRecieveRetryMessage), RpcTarget.Others);
    }

    [PunRPC]
    private void OnRecieveRetryMessage()
    {
        enemyRetryReady = true;
        if (playerRetryReady)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            FadeManager.Instance.LoadScene(currentScene, 1f);
        }
        else
        {
            gameUI.ShowRetryMessage();

        }
    }

}
