using JyCard.Interface;
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

namespace JyCard
{
    public partial class MainPage : UserControl, IUIHost
    {
        public MainPage()
        {
            InitializeComponent();
            views =  new List<Control>();
            views.Add(gameView);
            views.Add(hostView);
            views.Add(loginView);
        }

        List<Control> views = null;
        private void LayoutRoot_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //init..
            GameEngine.Instance.Init(this);

            HideAllViews();
            loginView.Show();
        }

        private void HideAllViews()
        {
            foreach (var v in views)
            {
                v.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        //implements of IUIhost
        public void HideLoginPage()
        {
            loginView.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void ShowHostPage()
        {
            hostView.Show();
        }

        public void HideHostPage()
        {
            hostView.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void ShowGamePage() 
        {
            //TODO..

            RuntimeData oppData = RuntimeData.Create("Resources/Scripts/test_save2.xml");
            GamePlayer opp = GamePlayer.CreateFromRuntimeData(oppData);
            CardQueue queue = new CardQueue(opp);
            for (int i = 0; i < 4; ++i)
            {
                opp.HandleCard.Add(queue.FetchCard());
            }

            GamePlayer me = GamePlayer.CreateFromLocal();
            CardQueue cardQueue = new CardQueue(me);
            for (int i = 0; i < 4; ++i)
            {
                me.HandleCard.Add(cardQueue.FetchCard());
            }

            gameView.Show(0, opp, me, cardQueue);
        }
    }
}
