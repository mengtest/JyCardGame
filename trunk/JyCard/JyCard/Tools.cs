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
using System.Windows.Media.Imaging;

using System.Xml.Linq;
using System.Collections.Generic;
using System.Windows.Browser;
using System.IO;
using System.Threading;
using System.Text;
using System.Security.Cryptography;

using System.Collections;
using System.Xml.Serialization;
using System.Windows.Resources;

namespace JyCard
{
    public class Tools
    {
        private static Dictionary<string, BitmapImage> imageCache = new Dictionary<string, BitmapImage>();
        
        public static BitmapSource GetImage(string path)
        {
            if (imageCache.ContainsKey(path)) 
                return imageCache[path];
            //BitmapImage rst =  new BitmapImage(new Uri(string.Format(@"{0}", path), UriKind.Relative));
            StreamResourceInfo sr = Application.GetResourceStream(new Uri(string.Format(@"{0}", path), UriKind.Relative));
            BitmapImage bmp = new BitmapImage();
            bmp.SetSource(sr.Stream);
            imageCache.Add(path, bmp);
            return bmp;
        }

        public static void PutImageCache(string path, BitmapImage img)
        {
            imageCache[path] = img;
        }

        public static string GetImageUrl(BitmapImage img)
        {
            foreach (var d in imageCache)
            {
                if (d.Value == img)
                {
                    return d.Key;
                }
            }
            return string.Empty;
        }

        #region XML操作

        public static XElement LoadXml(string path)
        {
            try
            {
                return XElement.Load("/JyCard;component/" + path);
            }
            catch(Exception e)
            {
                MessageBox.Show("xml载入错误:" + path);
                return null;
            }
        }

        public static XElement GetXmlElement(XElement xml, string key)
        {
            return xml.Element(key);
        }

        public static IEnumerable<XElement> GetXmlElements(XElement xml, string key)
        {
            return xml.Elements(key);
        }

        public static string GetXmlAttribute(XElement xml, string attribute)
        {
            return xml.Attribute(attribute).Value;
        }

        public static float GetXmlAttributeFloat(XElement xml, string attribute)
        {
            return float.Parse(xml.Attribute(attribute).Value);
        }

        public static int GetXmlAttributeInt(XElement xml, string attribute)
        {
            return int.Parse(xml.Attribute(attribute).Value);
        }
        #endregion

        #region 数学方法

        private static Random rnd = new Random();

        /// <summary>
        /// 生成a到b之间的随机数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double GetRandom(double a, double b)
        {
            double k = rnd.NextDouble();
            double tmp = 0;
            if (b > a)
            {
                tmp = a;
                a = b;
                b = tmp;
            }
            return b + (a - b) * k;
        }

        public static int GetRandomInt(int a, int b)
        {
            return (int)Tools.GetRandom(a, b+1);
        }

        /// <summary>
        /// 测试概率
        /// </summary>
        /// <param name="p">小于1的</param>
        /// <returns></returns>
        public static bool ProbabilityTest(double p)
        {
            if (p < 0) return false;
            if (p >= 1) return true;
            return rnd.NextDouble() < p;
        }

        #endregion
    }

    #region 数据加密
    public class DEncryptHelper
    {
        #region 整型加密解密

        public static int EncryptInt(int k)
        {
            return (k + 1024) << 2;
        }

        public static int DecryptInt(int k)
        {
            return (k >> 2) - 1024;
        }
        #endregion


    }
    #endregion

    #region 加密的dict
    public class SecureDictionary
    {
        private Dictionary<string, int> _data = new Dictionary<string, int>();
        public int this[string key]
        {
            get
            {
                return DEncryptHelper.DecryptInt(_data[key]);
            }
            set
            {
                _data[key] = DEncryptHelper.EncryptInt(value);
            }
        }

        public bool ContainsKey(string key)
        {

            return _data.ContainsKey(key);
        }

        public Dictionary<string, int>.KeyCollection Keys
        {
            get
            {
                return _data.Keys;
            }
        }

    }

    #endregion

#region xml序列化

    static public class XmlSearializeHelper<T> where T: new()
    {
        static public T DeSearialize(string xmlPath)
        {
            T rst = new T();

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            XElement xml = Tools.LoadXml(xmlPath);
            using (StringReader sr = new StringReader(xml.ToString()))
            {
                rst = (T)(serializer.Deserialize(sr));
            }
            return rst;
        }
    }
#endregion
}
