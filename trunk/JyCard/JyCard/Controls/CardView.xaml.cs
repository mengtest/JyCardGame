using JyCard.Logic;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace JyCard
{
	public partial class CardView : UserControl
	{
		public CardView()
		{
			// 为初始化变量所必需
			InitializeComponent();
		}

        public Card _card = null;
        public Hero _hero = null;
        public GameRole _gameRole = null;

        static public CardView CreateBack()
        {
            CardView view = new CardView();
            view.LeftNumber.Text = "";
            view.RightNumber.Text = "";
            view.CardName.Text = "";
            view.CardImage.Source = null;
            return view;
        }

        static public CardView CreateFromGameRole(GameRole role)
        {
            CardView view = new CardView();
            if (role.IsHero)
            {
                Hero hero = role.Hero;
                view.CardName.Text = hero.Name;
                view.CardImage.Source = hero.Image;
                view.LeftNumber.Text = role.Hp.ToString();
                view.RightNumber.Text = role.Attack.ToString();
                view._gameRole = role;
            }
            return view;
        }

        static public CardView CreateFromHero(Hero hero)
        {
            CardView view = new CardView();
            view.CardName.Text = hero.Name;
            view.CardImage.Source = hero.Image;
            view.LeftNumber.Text = hero.Hp.ToString();
            view.RightNumber.Text = hero.Attack.ToString() ;
            view._hero = hero;
            return view;
        }

        static public CardView CreateFromCard(Card card, int number = -1)
        {
            CardView view = new CardView();
            view._card = card;
            if (card.Name.Length >= 5)
            {
                view.CardName.Text = card.Name.Substring(0, 4) + "...";
            }
            else 
            {
                view.CardName.Text = card.Name;
            }
            if (number > 0)
            {
                view.BottomNumber.Text = number.ToString();
            }
            else
            {
                view.BottomNumber.Text = "";
            }
            view.CardImage.Source = card.Image;
            view.LeftNumber.Text = "";
            view.RightNumber.Text = card.Cost.ToString();
            string tooltipInfo = string.Format("{0}\n最多配备{1}个（英雄自带除外）\n{2}", card.Name, card.Max, card.Desc);
            ToolTipService.SetToolTip(view, tooltipInfo);
            return view;
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        public CommonSettings.ObjectCallBack Callback = null;
        private void UserControl_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (IsSelectActive && this.Callback != null)
            {
                Callback(this);
            }
        }

        public bool IsSelectActive
        {
            get { return _isSelectAction; }
            set
            {
                _isSelectAction = value;
                if (value)
                {
                    this.SelectiveAnimation.Begin();
                    this.PushDown();
                }
                else
                {
                    this.SelectiveAnimation.Stop();
                    this.PushDown();
                }
            }
        }
        private bool _isSelectAction = false;

        private void UserControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.IsSelectActive) //卡牌弹起
            {
                this.PopUp();
            }
        }

        private void UserControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.IsSelectActive) //卡牌放下
            {
                this.PushDown();
            }
        }

        public void PopUp()
        {
            this.Margin = new Thickness(0, -12, 0, 0);
        }

        public void PushDown()
        {
            this.Margin = new Thickness(0, 0, 0, 0);
        }
	}
}