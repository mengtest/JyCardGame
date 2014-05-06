using JyCard.Logic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace JyCard.Controls
{
    public enum GameStatus
    {
        SelectCard = 0, //选牌
        SelectTarget, //选目标
        Waiting, //等待
    }

    public partial class GameView : UserControl
    {
        public GameView()
        {
            InitializeComponent();
            statusMap.Add(GameStatus.SelectCard, OnSelectCard);
            statusMap.Add(GameStatus.SelectTarget, OnSelectTarget);
            statusMap.Add(GameStatus.Waiting, OnWaiting);
            CardStackPanels.Add(this.Player1Cards);
            CardStackPanels.Add(this.Player2Cards);
            CardStackPanels.Add(this.Player1Heros);
            CardStackPanels.Add(this.Player2Heros);
            CardStackPanels.Add(this.Player1Warriors);
            CardStackPanels.Add(this.Player2Warriors);
        }

        private GameStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                Debug.Assert((int)_status < CommonSettings.SUGGEST_WORDS.Length);
                SuggestText.Text = CommonSettings.SUGGEST_WORDS[(int)(_status)];
                this.Dispatcher.BeginInvoke(() => { statusMap[value](); });
            }
        }

        private GameStatus _status = GameStatus.SelectCard;
        private Dictionary<GameStatus, CommonSettings.VoidCallBack> statusMap =
            new Dictionary<GameStatus, CommonSettings.VoidCallBack>();

        #region 游戏状态机
        private List<StackPanel> CardStackPanels = new List<StackPanel>();
        private void SetAllCardsUnSelective()
        {
            foreach (var panel in CardStackPanels)
            {
                foreach (var view in panel.Children)
                {
                    if (view is CardView)
                    {
                        (view as CardView).IsSelectActive = false;
                    }
                }
            }
        }

        private void SetChildrenSelective(StackPanel panel, CommonSettings.ObjectCallBack callback)
        {
            foreach (var view in panel.Children)
            {
                if (view is CardView)
                {
                    (view as CardView).IsSelectActive = true;
                }
            }
        }

        private void OnSelectCard()
        {
            this.Refresh();
            //this.SetAllCardsUnSelective();
            this.CancelButton.Visibility = System.Windows.Visibility.Collapsed;
            foreach (var c in this.Player1Cards.Children)
            {
                Debug.Assert(c is CardView);
                CardView card = c as CardView;
                card.IsSelectActive = true;
                card.Callback = (viewObj) =>
                {
                    Card selectCard = card._card;
                    int cost = selectCard.Cost;
                    if (cost > _gameData.PlayerMe.Mp)
                    {
                        MessageBox.Show("抱歉，您的内力不足够出这张牌，需要" + cost + "点内力。");
                        return;
                    }
                    else
                    {
                        if (selectCard.IsPoint == false) //不需要选择目标的卡牌类型
                        {
                            this.CastCard(selectCard, null);//直接施展卡牌效果
                        }
                        else
                        {
                            _currentSelectCard = selectCard;
                            _currentSelectCardView = card;
                            Status = GameStatus.SelectTarget;
                        }
                    }
                };
            }
        }

        private void OnSelectTarget()
        {
            this.SetAllCardsUnSelective();
            _currentSelectCardView.PopUp();
            this.CancelButton.Visibility = System.Windows.Visibility.Visible;
            Card selectCard = _currentSelectCard;
            if (selectCard.IsSelf)
            {
                foreach (var role in _gameData.PlayerMe.GameRoles)
                {
                    if (role.Hp == 0) continue; 
                    role.CardView.IsSelectActive = true;
                    role.CardView.Callback = (viewObj) =>
                    {
                        this.CastCard(selectCard, role.CardView);
                    };
                }
            }
            if (selectCard.IsTarget)
            {
                foreach (var role in _gameData.PlayerOpp.GameRoles)
                {
                    if (role.Hp == 0) continue;
                    role.CardView.IsSelectActive = true;
                    role.CardView.Callback = (viewObj) =>
                    {
                        this.CastCard(selectCard, role.CardView);
                    };
                }
            }
        }
        private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Status != GameStatus.SelectTarget) return;
            Status = GameStatus.SelectCard;
        }

        private void OnWaiting()
        {

        }
        #endregion

        /// <summary>
        /// 开始游戏
        /// </summary>
        /// <param name="playerIndex">分配到的序号</param>
        /// <param name="opp">对手的数据</param>
        public void Show(int playerIndex, GamePlayer opp, GamePlayer me, CardQueue cardQueue)
        {
            _playerIndex = playerIndex;
            _cardQueue = cardQueue;

            GameData gameData = new GameData();
            gameData.PlayerMe = me;
            gameData.PlayerOpp = opp;
            _gameData = gameData;

            this.Refresh();
            this.Visibility = System.Windows.Visibility.Visible;
            if (playerIndex == 0)
            {
                Status = GameStatus.SelectCard;
            }
            else
            {
                Status = GameStatus.Waiting;
            }
        }

        private void Refresh()
        {
            this.RefreshRoundInfo();
            this.RefreshRoles(_gameData.PlayerMe, Player1Heros, Player1Warriors);
            this.RefreshHandleCards(_gameData.PlayerMe, Player1Cards, false);
            this.RefreshRoles(_gameData.PlayerOpp, Player2Heros, Player2Warriors);
            this.RefreshHandleCards(_gameData.PlayerOpp, Player2Cards, true);
            this.RefreshPlayerInfo(_gameData.PlayerMe, Player1AccountText, Player1MpText);
            this.RefreshPlayerInfo(_gameData.PlayerOpp, Player2AccountText, Player2MpText);
        }

        private void RefreshPlayerInfo(GamePlayer player, TextBlock accountText, TextBlock mpText)
        {
            accountText.Text = player.Account;
            mpText.Text = string.Format("{0}/{1}", player.Mp, player.MaxMp);
        }

        private void RefreshRoundInfo()
        {
            this.RoundText.Text = string.Format("第{0}轮\t", _gameData.Round / 2 + 1);
            if ((_gameData.Round % 2 == 0 && _playerIndex == 0) || (_gameData.Round % 2 == 1 && _playerIndex == 1))
                this.RoundText.Text += string.Format("我方回合");
            else
                this.RoundText.Text += string.Format("对手回合");
        }

        /// <summary>
        /// 刷新手牌显示
        /// </summary>
        /// <param name="player"></param>
        /// <param name="cardPanel"></param>
        /// <param name="isBack">是否显示背面</param>
        private void RefreshHandleCards(GamePlayer player, StackPanel cardPanel, bool isBack)
        {
            cardPanel.Children.Clear();
            foreach (var card in player.HandleCard)
            {
                CardView view = null;
                if (!isBack)
                    view = CardView.CreateFromCard(card);
                else
                    view = CardView.CreateBack();
                cardPanel.Children.Add(view);
            }
        }

        private void RefreshRoles(GamePlayer player, StackPanel heroPanel, StackPanel warriorPanel)
        {
            warriorPanel.Children.Clear();
            heroPanel.Children.Clear();
            foreach (var role in player.GameRoles)
            {
                CardView view = CardView.CreateFromGameRole(role);
                if (role.IsHero)
                {
                    heroPanel.Children.Add(view);
                }
                else
                {
                    warriorPanel.Children.Add(view);
                }
                role.CardView = view;
            }
        }

        /// <summary>
        /// 显示游戏动画
        /// </summary>
        /// <param name="viewAction">动画描述</param>
        /// <param name="CallBack">播放完毕的回调</param>
        private void ShowViewAction(GameViewAction viewAction, CommonSettings.VoidCallBack CallBack)
        {
            this.SetAllCardsUnSelective();


            CallBack();
        }

        /// <summary>
        /// 使用卡牌
        /// </summary>
        /// <param name="card">卡牌实例</param>
        /// <param name="targetCard">目标卡牌</param>
        private void CastCard(Card card, CardView targetCard)
        {
            Debug.Assert(card.Cost <= _gameData.PlayerMe.Mp);
            //_playerMe.Mp -= card.Cost;
            CardCasting casting = new CardCasting(_gameData, card, targetCard, _cardQueue);
            CastResult result = casting.Cast();
            this.ShowViewAction(result.ViewAction, () =>
            {
                _gameData = result.Data;
                Status = GameStatus.SelectCard;
            });
        }
        
        private GameData _gameData = null;
        private CardQueue _cardQueue = null;
        //private GamePlayer _playerMe = null;
        //private GamePlayer _playerOpp = null;
        private Card _currentSelectCard = null;
        private CardView _currentSelectCardView = null;
        private int _playerIndex = -1;
    }
}
