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
using System.Linq;
using System.Text;

namespace JyCard.Logic
{
    [XmlType("card")]
    public class Card
    {
        [XmlAttribute("name")]
        public string Name;
        [XmlAttribute("cost")]
        public int Cost;
        [XmlAttribute("pic")]
        public string Pic;
        [XmlAttribute("point")]
        public bool IsPoint = true;
        [XmlAttribute("self")]
        public bool IsSelf = false;
        [XmlAttribute("target")]
        public bool IsTarget = true;
        [XmlAttribute("equippment")]
        public bool IsEquippment = false;
        [XmlAttribute("desc")]
        public string Desc;
        [XmlAttribute("max")]
        public int Max = 1;

        public ImageSource Image
        {
            get
            {
                if (string.IsNullOrEmpty(Pic))
                {
                    return Tools.GetImage(CommonSettings.DEFAULT_CARD_IMAGE);
                }
                return Tools.GetImage(CommonSettings.IMAGE_ROOT_URL + Pic);
            }
        }

        public Card Clone()
        {
            return this.MemberwiseClone() as Card;
        }
    }

    [XmlRootAttribute("root")]
    public class CardRepository
    {
        [XmlArray("cards")]
        [XmlArrayItem("card")]
        public List<Card> Cards = new List<Card>();

        static public CardRepository Create(string xmlPath)
        {
            return XmlSearializeHelper<CardRepository>.DeSearialize(xmlPath);
        }

        public Card GetCard(string key)
        {
            foreach (var c in Cards)
            {
                if (c.Name == key)
                    return c;
            }
            return null;
        }
    }
}
