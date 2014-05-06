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
    /// <summary>
    /// 牌堆
    /// </summary>
    public class CardQueue
    {
        private List<string> _queue = new List<string>();
        public CardQueue(GamePlayer player)
        {
            this.Reset(player);
        }

        private CardQueue() { }

        /// <summary>
        /// 生成卡牌堆
        /// </summary>
        /// <param name="currentHeros">当前存活的英雄</param>
        public void Reset(GamePlayer player)
        {
            _queue.Clear();
            //取当前runtimedata中的卡牌队列
            List<RuntimeCard> runtimeCards = GameEngine.Instance.Runtime.Cards;
            foreach (var card in runtimeCards)
            {
                for (int i = 0; i < card.Selected; ++i)
                {
                    _queue.Add(card.Name);
                }
            }
            //取当前的英雄队列
            foreach (var hero in player.GetCurrentAliveHeros())
            {
                foreach (var card in hero.Cards)
                {
                    _queue.Add(card.Name);
                }
            }
        }

        /// <summary>
        /// 从卡牌堆里，摸一张牌
        /// </summary>
        /// <returns>如果没有牌了</returns>
        public Card FetchCard()
        {
            int length = _queue.Count;
            if (length == 0) { return null; }
            int select = Tools.GetRandomInt(0, length - 1);
            string cardName = _queue[select];
            return GameEngine.Instance.CardRepository.GetCard(cardName);
        }
    }
}
