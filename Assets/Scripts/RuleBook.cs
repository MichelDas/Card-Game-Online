using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleBook : MonoBehaviour
{
    // if there is minister then if player's number is bigger and he is minister then he win twice

    // ・道化(どうけ）（このターン引き分け）if clown comes, this turn is draw
    // ・暗殺者（あんさつしゃ）がいると、王子（おおじ）がいないと数字効果反転（すうじこうかはんてん）if there is assasin, and the prince is not there the result will be reversal
    // ・王子と姫（ひめ）がいると勝利判定（しょうりはんてい）if player is princess && enemy is prince, game win
    public Result GetResult(Battler player, Battler enemy)
    {
        if(player.SubmitCard.Base.Type == CardType.Magician || enemy.SubmitCard.Base.Type == CardType.Magician )
        {
            return NumberBattle(player, enemy, ministerEffect: false, reverseEffect:false);
        }

        if (player.SubmitCard.Base.Type == CardType.Spy && enemy.SubmitCard.Base.Type != CardType.Spy)
        {
            enemy.IsFirstSubmit = true;
        } 
        if (player.SubmitCard.Base.Type != CardType.Spy && enemy.SubmitCard.Base.Type == CardType.Spy)
        {
            player.IsFirstSubmit = true;
        }

        if (player.SubmitCard.Base.Type == CardType.Shogun )
        {
            player.IsAddNumberMode = true;
        }
        if (player.SubmitCard.Base.Type != CardType.Shogun)
        {
            player.IsAddNumberMode = true;
        }




        if ((player.SubmitCard.Base.Type == CardType.Clown || enemy.SubmitCard.Base.Type == CardType.Clown)
             || (enemy.SubmitCard.Base.Type == CardType.Clown || player.SubmitCard.Base.Type == CardType.Clown))
        {
            return Result.TurnDraw;
        }

        if (player.SubmitCard.Base.Type == CardType.Assassin && enemy.SubmitCard.Base.Type != CardType.Prince)
        {
            return NumberBattle(player, enemy, ministerEffect: true, reverseEffect: true);
        }
        if (player.SubmitCard.Base.Type == CardType.Princess && enemy.SubmitCard.Base.Type == CardType.Prince)
        {
            return Result.GameWin;
        }
        if (player.SubmitCard.Base.Type == CardType.Prince && enemy.SubmitCard.Base.Type == CardType.Princess)
        {
            return Result.GameLose;
        }

        return NumberBattle(player  , enemy, ministerEffect: true, reverseEffect: false);
    }

    Result NumberBattle(Battler player, Battler enemy, bool ministerEffect, bool reverseEffect)
    {
        if(ministerEffect == false)
        {
            if (player.SubmitCard.Base.Number + player.AddNumber > enemy.SubmitCard.Base.Number + enemy.AddNumber)
            {
                if (reverseEffect)
                {
                    return Result.TurnLose;
                }
                return Result.TurnWin;
            }
            else if (player.SubmitCard.Base.Number + player.AddNumber < enemy.SubmitCard.Base.Number + enemy.AddNumber)
            {
                if (reverseEffect)
                {
                    return Result.TurnWin ;
                }
                return Result.TurnLose;
            }
        }
        else
        {
            if (player.SubmitCard.Base.Number + player.AddNumber > enemy.SubmitCard.Base.Number + enemy.AddNumber)
            {
                if (reverseEffect)
                {
                    if (enemy.SubmitCard.Base.Type == CardType.Minister)
                    {
                        return Result.TurnLose2;
                    }
                    return Result.TurnLose;
                }
                if(player.SubmitCard.Base.Type == CardType.Minister)
                {
                    return Result.TurnWin2; 
                }
                return Result.TurnWin;
            }
            else if (player.SubmitCard.Base.Number + player.AddNumber < enemy.SubmitCard.Base.Number + enemy.AddNumber)
            {
                if (reverseEffect)
                {
                    if (player.SubmitCard.Base.Type == CardType.Minister)
                    {
                        return Result.TurnWin2;
                    }
                    return Result.TurnWin;
                }
                if (enemy.SubmitCard.Base.Type == CardType.Minister)
                {
                    return Result.TurnLose2;
                }
            }
        }
        

        return Result.TurnDraw;
    }
}

public enum Result
{
    TurnWin,
    TurnLose,
    TurnDraw,
    TurnWin2,
    TurnLose2,
    GameWin,
    GameLose,
    GameDraw
}
