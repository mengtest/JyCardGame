using JyCard.Interface;
using System;
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
    public class GameEngine
    {
        #region singleton
        static public GameEngine Instance
        {
            get
            {
                if (_gameEngine == null) _gameEngine = new GameEngine();
                return _gameEngine;
            }
        }

        private GameEngine() { }
        static private GameEngine _gameEngine = null;
        #endregion

        public void Init(IUIHost uiHost)
        {
            _uiHost = uiHost;
            heroRepository = HeroRepository.Create("Resources/Scripts/heros.xml");
            cardRepository = CardRepository.Create("Resources/Scripts/cards.xml");

            //for test
            runtimeData = RuntimeData.Create("Resources/Scripts/test_save.xml");
        }

        private IUIHost _uiHost = null;
        private HeroRepository heroRepository = null;
        private CardRepository cardRepository = null;
        private RuntimeData runtimeData = null;
        public RuntimeData Runtime { get { return runtimeData; } }
        public HeroRepository HeroRepository { get { return heroRepository; } }
        public CardRepository CardRepository { get { return cardRepository; } }

        public void Login(string account, string password)
        {
            //TODO..

            _uiHost.HideLoginPage();
            _uiHost.ShowHostPage();
        }

        public void GameStart()
        {
            //TODO..

            _uiHost.HideHostPage();
            _uiHost.ShowGamePage();
        }
    }
}
