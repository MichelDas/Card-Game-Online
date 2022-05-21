using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    [SerializeField] Battler player;
    [SerializeField] Battler enemy;
    [SerializeField] CardGenerator cardGenerator;
    [SerializeField] GameUI gameUI;
    RuleBook ruleBook;

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
        gameUI.Init();
        player.Life = 4;
        enemy.Life = 4;
        gameUI.ShowLifes(player.Life, enemy.Life);
        gameUI.UpdateAddNumber(player.AddNumber, enemy.AddNumber);
        player.OnSubmitAction = SubmittedAction;
        enemy.OnSubmitAction = SubmittedAction;
        SetupCards(battler: player, isEnemy: false);
        SetupCards(battler: enemy, isEnemy: true);
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
            // if only player submit the card, the button will be closed
            // Enemyからカードを出す
            enemy.RandomSubmit();
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
            enemy.RandomSubmit();
            enemy.SubmitCard.Open();
        }

        if(player.IsFirstSubmit)
        {
            // PLAYER WILL put card first
            // this will be usefull in online
        }

    }

    public void OnRetryButton()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    public void OnTitleButton()
    {
        SceneManager.LoadScene("Menu");
    }

}
