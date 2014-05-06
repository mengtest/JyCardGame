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
    public class CommonSettings
    {
        public const string IMAGE_ROOT_URL = "Resources/Images/";
        public const string DEFAULT_CARD_IMAGE = "Resources/Images/UI-kuang2.png";
        public const int HOST_CARD_IN_ONE_LINE = 4;

        static public string[] SUGGEST_WORDS = new string[3]{
            "请挑选你要出的卡牌，或者点击回合结束",
            "请挑选你卡牌施放目标",
            "请等待对方行动"
        };

        #region DELEGATE
        public delegate void VoidCallBack();
        public delegate void IntCallBack(int rst);
        public delegate void ItemCallBack(Dictionary<string, int> items, int point);
        public delegate void StringCallBack(string rst);
        public delegate void ObjectCallBack(object obj);
        #endregion

    }
}
