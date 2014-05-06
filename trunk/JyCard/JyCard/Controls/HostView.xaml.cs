using JyCard.Logic;
using System;
using System.Collections.Generic;
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
    public partial class HostView : UserControl
    {
        public HostView()
        {
            InitializeComponent();
        }

        public void Show()
        {
            this.Visibility = System.Windows.Visibility.Visible;

            this.Refresh();
        }

        private void Refresh()
        {
            this.Clear();
            this.RefreshHeroPanel();
            this.RefreshCardPanel();
            this.RefreshCurrentCardPanel();
        }

        private void RefreshHeroPanel()
        {
            HeroStackPanel.Children.Clear();
            int count = GameEngine.Instance.Runtime.SelectHeroCount;
            this.SelectHeroCountText.Text = count.ToString();
            if (count < 3)
            {
                this.SelectHeroCountText.Text += "(需要3个)";
            }
            
            
            //heros
            int k = 0;
            StackPanel currentStackPanel = null;
            foreach (var h in GameEngine.Instance.Runtime.Heros)
            {
                if (k % CommonSettings.HOST_CARD_IN_ONE_LINE == 0)
                {
                    k = 0;
                    currentStackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
                    HeroStackPanel.Children.Add(currentStackPanel);
                }
                RuntimeHero hero = h;
                CardView heroCard = CardView.CreateFromHero(h.Hero);
                heroCard.Opacity = h.IsSelected ? 1 : 0.3;
                heroCard.MouseLeftButtonDown += (s, e) =>
                {
                    if (hero.IsSelected)
                    {
                        hero.IsSelected = false;
                        this.Dispatcher.BeginInvoke(() =>
                        { 
                            RefreshHeroPanel(); 
                            ShowHeroInfo(h.Hero);
                        });
                    }
                    else
                    {
                        if (GameEngine.Instance.Runtime.SelectHeroCount < 3)
                        {
                            hero.IsSelected = true;
                            this.Dispatcher.BeginInvoke(() =>
                            {
                                RefreshHeroPanel();
                                ShowHeroInfo(h.Hero);
                            });
                        }
                        else
                        {
                            MessageBox.Show("最多选择3名上场英雄");
                        }
                    }
                };
                heroCard.MouseMove += (s, e) => { ShowHeroInfo(h.Hero); };
                currentStackPanel.Children.Add(heroCard);
            }
        }

        private void RefreshCardPanel()
        {
            CardStackPanel.Children.Clear();
            int k = 0;
            StackPanel currentStackPanel = null;
            foreach (var c in GameEngine.Instance.Runtime.Cards)
            {
                if (c.Selected >= c.Number) continue; //不显示挑选了的
                if (k % CommonSettings.HOST_CARD_IN_ONE_LINE == 0)
                {
                    k = 0;
                    currentStackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
                    CardStackPanel.Children.Add(currentStackPanel);
                }
                RuntimeCard card = c;
                CardView cardView = CardView.CreateFromCard(card.Card, c.Number - c.Selected);
                currentStackPanel.Children.Add(cardView);
                cardView.MouseLeftButtonDown += (s, e) =>
                {
                    if (GameEngine.Instance.Runtime.SelectCardCount < 15)
                    {
                        if (card.Selected < card.Card.Max) 
                        { 
                            card.Selected++;
                            this.Dispatcher.BeginInvoke(() =>
                            {
                                this.RefreshCardPanel();
                                this.RefreshCurrentCardPanel();
                            });
                        }
                        else
                        {
                            MessageBox.Show("此类卡牌你最多带" + card.Card.Max + "个");
                        }
                    }
                    else
                    {
                        MessageBox.Show("你最多只能挑选15张上场手牌");
                    }
                };
                k++;
            }
        }

        public void RefreshCurrentCardPanel()
        {
            int count = GameEngine.Instance.Runtime.SelectCardCount;

            this.SelectCardCountText.Text = count.ToString();
            if (count < 15) this.SelectCardCountText.Text += "(需要15个)";

            this.CurrentCardStackPanel.Children.Clear();
            int k = 0;
            StackPanel currentStackPanel = null;
            foreach (var c in GameEngine.Instance.Runtime.Cards)
            {
                if (c.Selected == 0) continue;
                if (k % CommonSettings.HOST_CARD_IN_ONE_LINE == 0)
                {
                    k = 0;
                    currentStackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
                    CurrentCardStackPanel.Children.Add(currentStackPanel);
                }
                RuntimeCard card = c;
                CardView cardView = CardView.CreateFromCard(card.Card, c.Selected);
                currentStackPanel.Children.Add(cardView);
                cardView.MouseLeftButtonDown += (s, e) =>
                {
                    card.Selected--;
                    this.Dispatcher.BeginInvoke(() => 
                    { 
                        this.RefreshCardPanel();
                        this.RefreshCurrentCardPanel();
                    });
                };
                k++;
            }
        }

        private void ShowHeroInfo(Hero hero)
        {
            HeroHead.Source = hero.Image;
            HeroInfo.Text = string.Format("{0}\n生命值:{1}\n攻击力:{2}", hero.Name, hero.Hp, hero.Attack);
            HeroCards.Children.Clear();
            foreach (var c in hero.Cards)
            {
                for (int i = 0; i < c.Number; ++i)
                {
                    CardView cardView = CardView.CreateFromCard(c.Card);
                    HeroCards.Children.Add(cardView);
                }
            }
        }

        private void Clear()
        {
            HeroStackPanel.Children.Clear();
            CardStackPanel.Children.Clear();
            CurrentCardStackPanel.Children.Clear();
            this.ClearHeroInfo();
        }

        private void ClearHeroInfo()
        {
            HeroHead.Source = null;
            HeroInfo.Text = "";
            HeroCards.Children.Clear();
        }

        private void StartGameButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            GameEngine.Instance.GameStart();
        }
    }
}
