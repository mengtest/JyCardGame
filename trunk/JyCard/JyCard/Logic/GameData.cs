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
    public class Buff
    {
        public string Name;
        public int Level = 1;
        public int LeftRound = -1;
    }

    public class GameRole
    {
        public int Hp;
        public int Attack
        {
            get
            {
                int add = 0;
                if (Equippment == "冷月宝刀") add += 2;
                Buff buff = GetBuff("攻击提高");
                if (buff != null) add += buff.Level;
                return BasicAttack + add; 
            }
        }

        public int MaxHp;
        public int BasicAttack;

        public bool IsHero = false; //是否是英雄

        public List<Buff> Buffs = new List<Buff>();//当前该角色的BUFF

        private Hero _hero = null;
        
        /// <summary>
        /// 当前装备
        /// </summary>
        public string Equippment = null;

        public static GameRole CreateFromHero(Hero h)
        {
            GameRole role = new GameRole();
            role.Hp = h.Hp;
            role.MaxHp = h.Hp;
            role.BasicAttack = h.Attack;
            role.IsHero = true;
            role._hero = h;
            return role;
        }

        public Hero Hero { get { return _hero; } }
        public CardView CardView = null;

        /// <summary>
        /// 添加BUFF
        /// </summary>
        /// <param name="buff"></param>
        public void AddBuff(Buff buff)
        {
            //若已经有BUFF，新BUFF等级更高，则刷新BUFF
            foreach (var b in Buffs)
            {
                if (b.Name == buff.Name)
                {
                    if (buff.Level > b.Level)
                    {
                        b.Level = buff.Level;
                        b.LeftRound = Math.Max(b.LeftRound, buff.LeftRound); //剩余时间取更大的那个
                        return;
                    }
                }
            }
            //否则添加BUFF
            Buffs.Add(buff);
        }

        public void RemoveBuff(string Name)
        {
            Buff buff = this.GetBuff(Name);
            if (buff != null) this.Buffs.Remove(buff);
        }

        public Buff GetBuff(string Name)
        {
            foreach (var b in Buffs)
            {
                if (b.Name == Name) return b;
            }
            return null;
        }

        /// <summary>
        /// 遭受到攻击
        /// </summary>
        /// <param name="attack">攻击力</param>
        /// <returns>是否命中</returns>
        public bool BeAttack(int attack)
        {
            Buff buff = this.GetBuff("闪避");
            if (buff != null)
            {
                bool missed = Tools.ProbabilityTest((double)buff.Level / 100f);
                if (missed)
                    return false;
            }
            this.Hp -= attack;
            if (this.Hp < 0) this.Hp = 0;
            return true;
        }

        /// <summary>
        /// 装备/卸下物品
        /// </summary>
        /// <param name="EquippmentName">装备名</param>
        /// <param name="isWear">装备/卸下</param>
        public void Equip(string EquippmentName,bool isWear)
        {
            if(isWear && !string.IsNullOrEmpty(this.Equippment)) //如果有装备，则先卸载原来的
                Equip(this.Equippment, false);

            switch(EquippmentName)
            {
                case "冷月宝刀":
                    break;
                default:
                    MessageBox.Show("invalid equippment name:" + EquippmentName);
                    break;
            }
            if (isWear)
                this.Equippment = EquippmentName;
            else
                this.Equippment = string.Empty;
        }
    }

    public class GamePlayer
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Account;

        public List<Card> HandleCard = new List<Card>(); //当前手牌

        /// <summary>
        /// 内力上限
        /// </summary>
        public int MaxMp = 12;

        /// <summary>
        /// 当前内力
        /// </summary>
        public int Mp = 12;

        /// <summary>
        /// 当前场上角色，前3个为英雄
        /// 英雄死了后HP为0，其他角色死亡后从本集合中移除
        /// </summary>
        public List<GameRole> GameRoles = new List<GameRole>();

        /// <summary>
        /// 全体BUFF，一般是下回合生效之类的BUFF
        /// </summary>
        public List<Buff> Buffs = new List<Buff>();

        /// <summary>
        /// 从本地数据创建GamePlayer
        /// </summary>
        /// <returns></returns>
        public static GamePlayer CreateFromLocal()
        {
            return CreateFromRuntimeData(GameEngine.Instance.Runtime);
        }

        /// <summary>
        /// 从runtimeData生成
        /// </summary>
        /// <param name="runtimeData"></param>
        /// <returns></returns>
        public static GamePlayer CreateFromRuntimeData(RuntimeData runtimeData)
        {
            GamePlayer rst = new GamePlayer();
            rst.Account = runtimeData.Account;
            foreach (var hero in runtimeData.SelectedHeros)
            {
                rst.GameRoles.Add(GameRole.CreateFromHero(hero.Hero));
            }
            return rst;
        }

        /// <summary>
        /// 获取当前存活的英雄
        /// </summary>
        /// <returns></returns>
        public List<Hero> GetCurrentAliveHeros()
        {
            List<Hero> rst = new List<Hero>();
            for (int i = 0; i < 3; ++i)
            {
                GameRole gameRole = GameRoles[i];
                if (gameRole.IsHero && gameRole.Hp > 0)
                {
                    rst.Add(gameRole.Hero);
                }
            }
            return rst;
        }

        /// <summary>
        /// 从排堆抽一张牌，如果为空，则洗牌重新抽
        /// </summary>
        /// <param name="queue"></param>
        public Card FetchCard(CardQueue queue)
        {
            Card card = queue.FetchCard();
            if (card == null)
            {
                queue.Reset(this);
                card = queue.FetchCard();
            }
            return card;
        }
    }

    /// <summary>
    /// 游戏运行时数据
    /// </summary>
    public class GameData
    {
        /// <summary>
        /// 当前游戏轮数
        /// 每方行动都算一个回合。
        /// </summary>
        public int Round = 0;

        /// <summary>
        /// 玩家数据
        /// </summary>
        //public List<GamePlayer> Players = new List<GamePlayer>();

        public GamePlayer PlayerMe = null;
        public GamePlayer PlayerOpp = null;
    }

    
}
