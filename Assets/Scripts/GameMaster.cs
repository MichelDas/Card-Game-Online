using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    [SerializeField] Battler player;
    [SerializeField] Battler enemy;
    [SerializeField] CardGenerator cardGenerator;
    [SerializeField] GameObject submitButton;
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
        player.OnSubmitAction = SubmittedAction;
        enemy.OnSubmitAction = SubmittedAction;
        SetupCards(player);
        SetupCards(enemy);
    }

    void SubmittedAction()
    {
        if(player.IsSubmitted && enemy.IsSubmitted)
        {
            submitButton.SetActive(false);
            //Cardの　勝利判定(しょうりはんてい）（check if win or not)
            StartCoroutine(CardBattle());
        }
        else　if (player.IsSubmitted)
        {
            // if only player submit the card, the button will be closed
            submitButton.SetActive(false);
            // Enemyからカードを出す
            enemy.RandomSubmit();
        }
        else if (enemy.IsSubmitted)
        {
            //Playerの提出を待つ
        }
    }

    void SetupCards(Battler battler)
    {
        for (int i = 0; i < 8; i++)
        {
            Card card = cardGenerator.Spawn(i);
            battler.SetCardToHand(card);
        }
        battler.Hand.ResetPosition();
    }

    // Card勝利判定（しょうりはんてい）
    // ちょっと遅らせてから結果を表示：コルーチン使う
    IEnumerator CardBattle()
    {
        yield return new WaitForSeconds(1f);
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
        if ((result == Result.GameWin) || (result == Result.GameLose) || (player.Life <= 0 || enemy.Life <= 0))
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
    }

    // 表示を終わったら、次のターンにうつる（場のカードを捨てる） 
    void SetupNextTurn()
    {
        player.SetupNextTurn();
        enemy.SetupNextTurn();
        submitButton.SetActive(true);
        gameUI.SetupNextTurn();

        if(enemy.IsFirstSubmit)
        {
            enemy.IsFirstSubmit = false;
            enemy.RandomSubmit();
        }

        if(player.IsFirstSubmit)
        {
            // PLAYER WILL put card first
            // this will be usefull in online
        }

    }

}
