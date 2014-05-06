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
using System.Xml.Serialization;
using System.Linq;

namespace JyCard.Logic
{
    /// <summary>
    /// 玩家运行时数据
    /// </summary>
    /// 
    [XmlRootAttribute("root")]
    public class RuntimeData
    {
        [XmlAttribute("account")]
        public string Account;

        [XmlAttribute("money")]
        public int Money;

        [XmlArray("heros")]
        [XmlArrayItem("hero")]
        public List<RuntimeHero> Heros = new List<RuntimeHero>();

        [XmlArray("cards")]
        [XmlArrayItem("card")]
        public List<RuntimeCard> Cards = new List<RuntimeCard>();

        static public RuntimeData Create(string xmlPath)
        {
            return XmlSearializeHelper<RuntimeData>.DeSearialize(xmlPath);
        }

        public IEnumerable<RuntimeHero> SelectedHeros
        {
            get
            {
                return Heros.Where(p => p.IsSelected);
            }
        }

        public int SelectHeroCount
        {
            get
            {
                int count = 0;
                foreach (var h in Heros) { if (h.IsSelected) count++; }
                return count;
            }
        }

        public int SelectCardCount
        {
            get
            {
                int count = 0;
                foreach (var c in Cards) { count += c.Selected; }
                return count;
            }
        }
    }

    [XmlType("hero")]
    public class RuntimeHero
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("selected")]
        public bool IsSelected;

        public Hero Hero
        {
            get
            {
                if(_hero == null)
                    _hero = GameEngine.Instance.HeroRepository.GetHero(Name);
                return _hero;
            }
        }

        private Hero _hero = null;
    }

    [XmlType("card")]
    public class RuntimeCard
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("number")]
        public int Number;

        [XmlAttribute("selected")]
        public int Selected;

        public Card Card
        {
            get
            {
                if(_card == null)
                 _card =  GameEngine.Instance.CardRepository.GetCard(Name);
                return _card;
            }
        }

        private Card _card = null;
    }
}
