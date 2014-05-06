using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace JyCard.Logic
{
    [XmlType("hero")]
    public class Hero
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("hp")]
        public int Hp;

        [XmlAttribute("attack")]
        public int Attack;

        [XmlAttribute("pic")]
        public string Pic;

        [XmlArray("cards")]
        [XmlArrayItem("card")]
        public List<HeroCard> Cards = new List<HeroCard>();

        public ImageSource Image
        {
            get
            {
                return Tools.GetImage(CommonSettings.IMAGE_ROOT_URL + Pic);
            }
        }
    }

    [XmlType("card")]
    public class HeroCard
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("number")]
        public int Number;

        public Card Card
        {
            get
            {
                if (_card == null)
                    _card = GameEngine.Instance.CardRepository.GetCard(Name);
                return _card;
            }
        }

        private Card _card = null;
    }

    [XmlRootAttribute("root")]
    public class HeroRepository
    {
        [XmlArray("heros")]
        [XmlArrayItem("hero")]
        public List<Hero> Heros = new List<Hero>();

        static public HeroRepository Create(string xmlPath)
        {
            return XmlSearializeHelper<HeroRepository>.DeSearialize(xmlPath);
        }

        public Hero GetHero(string key)
        {
            foreach (var h in Heros)
            {
                if (h.Name == key)
                    return h;
            }
            MessageBox.Show("error, null hero key:" + key);
            return null;
        }
    }
}
