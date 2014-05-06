using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace JyCard.Logic
{
    public class CastResult
    {
        public GameData Data;

        public GameViewAction ViewAction;
    }

    /// <summary>
    /// 展示封装
    /// </summary>
    public class GameViewAction
    {
        /// <summary>
        /// 日志信息
        /// </summary>
        public string Log;

        /// <summary>
        /// 敌方MISS,index
        /// </summary>
        public List<int> MissedOpp = new List<int>();

        /// <summary>
        /// 敌方被攻击,index
        /// </summary>
        public List<int> BeAttackedOpp = new List<int>();

        /// <summary>
        /// 抽取获得的卡牌
        /// </summary>
        public List<Card> GetCard = new List<Card>();

        /// <summary>
        /// 使用的卡牌
        /// </summary>
        public Card card;

        /// <summary>
        /// 装备到的角色的index
        /// </summary>
        public int EquipRoleIndex = -1;
    }

    /// <summary>
    /// 卡牌施放计算类
    /// </summary>
    public class CardCasting
    {
        public CardCasting(GameData gameData, Card card, CardView target, CardQueue cardQueue)
        {
            this.gameData = gameData;
            this.card = card;
            this.target = target;
            this.cardQueue = cardQueue;
        }
        private GameData gameData;
        private Card card;
        private CardView target;
        private CardQueue cardQueue;
        private CastResult rst;
        private int IndexOf(GameRole targetRole, bool me)
        {
            if(me)
                return gameData.PlayerMe.GameRoles.IndexOf(targetRole);
            else
                return gameData.PlayerOpp.GameRoles.IndexOf(targetRole);
        }

        private void Attack(GameRole role, int attack)
        {
            int index = IndexOf(role, false);
            bool missed = role.BeAttack(2);
            if (missed)
                rst.ViewAction.MissedOpp.Add(index);
            else
                rst.ViewAction.BeAttackedOpp.Add(index);
        }

        public CastResult Cast()
        {
            rst = new CastResult();
            rst.ViewAction = new GameViewAction();
            rst.Data = gameData;
            rst.Data.PlayerMe.HandleCard.Remove(card); //移除使用的手牌
            rst.Data.PlayerMe.Mp -= card.Cost; //消耗内力
            GameRole targetRole = target._gameRole;
            switch (card.Name)
            {
                case "雪山飞狐":
                    { 
                        foreach(var role in gameData.PlayerMe.GameRoles)
                        {
                            role.AddBuff(new Buff() { LeftRound = 2, Level = 50, Name = "闪避" });
                        }
                        break;
                    }
                case "胡家刀法":
                    { 
                        foreach (var role in gameData.PlayerOpp.GameRoles)
                        {
                            Attack(role,2);
                        }
                        foreach (var role in gameData.PlayerMe.GameRoles)
                        {
                            if (role.Equippment == "冷月宝刀") //当角色有冷月宝刀时，抽一张牌
                            {
                                Card newCard = gameData.PlayerMe.FetchCard(cardQueue);
                                rst.ViewAction.GetCard.Add(newCard);
                            }
                        }
                        break;
                    }
                case "冷月宝刀":
                    {
                        targetRole.Equip("冷月宝刀", true);
                        int index = IndexOf(targetRole, true);
                        rst.ViewAction.EquipRoleIndex = index;
                        break;
                    }
                case "杀":
                    {
                        int attack = 0;
                        for (int i = 0; i < 3; ++i) 
                        {
                            attack += gameData.PlayerMe.GameRoles[i].Attack;
                        }
                        Attack(targetRole,attack);
                        break;
                    }
                case "毒手药王":
                    {
                        foreach(var role in gameData.PlayerOpp.GameRoles)
                        {
                            Buff buff = role.GetBuff("中毒");
                            if (buff == null || buff.LeftRound <= 0) continue;
                            int attack = buff.Level * buff.LeftRound;
                            Attack(role, attack);
                        }
                        foreach (var role in gameData.PlayerOpp.GameRoles)
                        {
                            role.AddBuff(new Buff() { Name = "中毒", Level = 2, LeftRound = 2 });
                        }
                        break;
                    }
                case "七星海棠": //指定单位立刻受到3点伤害，并中毒，每回合-2，持续3个回合，立刻生效。
                    {
                        Attack(targetRole, 3);
                        targetRole.AddBuff(new Buff() { Name = "中毒", Level = 2, LeftRound = 3 });
                        break;
                    }
                case "毒":
                    {
                        targetRole.AddBuff(new Buff() { Name = "中毒", Level = 2, LeftRound = 2 });
                        break;
                    }
                case "解毒":
                    {
                        targetRole.RemoveBuff("中毒");
                        break;
                    }
                case "打遍天下无敌手":
                    {
                        foreach (var role in gameData.PlayerMe.GameRoles)
                        {
                            if (role.Hero.Name == "苗人凤")
                            {
                                targetRole.AddBuff(new Buff() { Name = "攻击提高", Level = 4, LeftRound = 2 });
                            }
                        }
                        break;
                    }
                case "苗家剑法":
                    {
                        //苗人凤立刻攻击某个敌方单位（若苗人凤死亡，无效果）。
                        foreach (var role in gameData.PlayerMe.GameRoles)
                        {
                            if (role.Hero.Name == "苗人凤")
                            {
                                Attack(targetRole, role.Attack);
                                break;
                            }
                        }
                        break;
                    }
                case "嘲讽":
                    {
                        targetRole.AddBuff(new Buff() { Name = "嘲讽", Level = 2, LeftRound = 2 });
                        break;
                    }
                default:
                    MessageBox.Show("invalid card name:" + card.Name);
                    break;
            }
            return rst;
        }
    }
}
