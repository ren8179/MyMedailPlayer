using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using Microsoft.Win32;

namespace MyMedailPlayer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        string xmlMediaListPath = System.Environment.CurrentDirectory + "\\MediaList.xml";//MediaList.xml主播放列表的存取位置
        string currentMediaListPath = string.Empty;//当前播放列表位置

        #region 页面加载
        public MainWindow()
        {
            InitializeComponent();
            loadMediaList(xmlMediaListPath);
        }

        /// <summary>
        /// loadMediaList：导入指定路径播放列表
        /// </summary>
        private void loadMediaList(string MediaListPath)
        {
            currentMediaListPath = MediaListPath;//刷新当前播放列表物理路径
            if (listView.Items.Count > 0)
                listView.Items.Clear();
            XmlDocument xml_mediaList = new XmlDocument();
            if (System.IO.File.Exists(MediaListPath))
            {
                xml_mediaList.Load(MediaListPath);
                XmlNodeList nodelist = xml_mediaList.SelectSingleNode("MediaList").ChildNodes;
                foreach (XmlNode node in nodelist)
                {
                    AddToTreeView(node.InnerText);
                }
            }
        }

        /// <summary>
        /// 添加到界面的列表中
        /// </summary>
        private void AddToTreeView(string text)
        {
            ListBoxItem item = new ListBoxItem();            //创建Item
            item.Content = text.Substring(text.LastIndexOf("\\") + 1); //从完整路径中取出文件名
            item.Tag = text;
            item.FontWeight = FontWeights.Bold;                //设定字体等显示属性
            item.FontSize = 12;
            item.Foreground = Brushes.Silver;
            listView.Items.Add(item);                         //将Item添加到TreeView中
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetpicBoxConver();
            if (listView.Items.Count > 0)
            {
                listView.SelectedIndex = 0;
                McMediaElement.Source = new Uri(((ListBoxItem)listView.SelectedItem).Tag.ToString());
            }
        }

        /// <summary>
        /// 随机设置专辑封面图片
        /// </summary>
        protected void SetpicBoxConver()
        {
            Random ram = new Random();
            int num = ram.Next(1, 11);
            picBoxConver.Source = new BitmapImage((new Uri(@"Images\Ambupicture" + num + ".png", UriKind.Relative)));
        }

        #endregion

        #region 主界面事件

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            Image ig = sender as Image;
            if (ig.Tag.ToString() == "close")
            {
                Uri ur = new Uri("images/close1.png", UriKind.Relative);
                BitmapImage bp = new BitmapImage(ur);
                ig.Source = bp;
            }
            else
            {
                Uri ur = new Uri("images/mini1.png", UriKind.Relative);
                BitmapImage bp = new BitmapImage(ur);
                ig.Source = bp;
            }
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            Image ig = sender as Image;
            if (ig.Tag.ToString() == "close")
            {
                Uri ur = new Uri("images/close0.png", UriKind.Relative);
                BitmapImage bp = new BitmapImage(ur);
                ig.Source = bp;
            }
            else
            {
                Uri ur = new Uri("images/mini0.png", UriKind.Relative);
                BitmapImage bp = new BitmapImage(ur);
                ig.Source = bp;
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image ig = sender as Image;
            if (ig.Tag.ToString() == "close")
            {
                this.Close();
                return;
            }
            this.WindowState = WindowState.Minimized;
        }

        #endregion

        private void Btn_Play_Click(object sender, RoutedEventArgs e)
        {
            if (Btn_Play.Tag.ToString() == "play")
            {
                PlayingMedia();
            }
            else if (Btn_Play.Tag.ToString() == "pause")
            {
                PauseMedia();
            }
        }
        /// <summary>
        /// 播放
        /// </summary>
        private void PlayingMedia()
        {
            McMediaElement.Play();
            Uri ur = new Uri("images/pause_down.png", UriKind.RelativeOrAbsolute);
            Btn_Play.Background = new ImageBrush(new BitmapImage(ur));
            Btn_Play.Tag = "pause";
        }
        /// <summary>
        /// 暂停
        /// </summary>
        private void PauseMedia()
        {
            McMediaElement.Pause();
            Uri ur = new Uri("images/play_on.png", UriKind.RelativeOrAbsolute);
            Btn_Play.Background = new ImageBrush(new BitmapImage(ur));
            Btn_Play.Tag = "play";
        }

        private void Btn_Preview_Click(object sender, RoutedEventArgs e)
        {
            //如果上一曲存在
            if (listView.SelectedIndex >= 1)
                listView.SelectedIndex = listView.SelectedIndex - 1;//选中上一个音乐
            else
                listView.SelectedIndex = listView.Items.Count - 1;//选中最后一个音乐
            PlaySelectedMedia();
        }

        private void Btn_Next_Click(object sender, RoutedEventArgs e)
        {
            //如果下一曲存在
            if (listView.SelectedIndex < listView.Items.Count - 1)
                listView.SelectedIndex = listView.SelectedIndex + 1;//选中下一曲音乐
            else
                listView.SelectedIndex = 0;
            PlaySelectedMedia();
        }
        private void Btn_AddMusic_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "mp3,wma,wav|*.mp3;*.wma;*.wav|*.*|*.*";
            openFileDialog.Multiselect = true;
            openFileDialog.FileName = "可以选择多首歌曲(Ctrl+A)";
            if (openFileDialog.ShowDialog().Value)
            {
                string[] mediaNames = openFileDialog.FileNames;
                listView.Items.Clear();
                for (int i = 1; i < mediaNames.Length + 1; i++)
                {
                    AddToTreeView(mediaNames[i - 1]);
                }
                if (listView.Items.Count > 0)
                {
                    listView.SelectedIndex = 0;
                    McMediaElement.Source = new Uri(((ListBoxItem)listView.SelectedItem).Tag.ToString());
                }
                saveMediaList(mediaNames, currentMediaListPath);//将当前播放列表信息保存为xml文件
            }
        }

        //saveMediaList:将指定音乐文件保存为XML到指定路径
        private void saveMediaList(string[] mediaNames, string MediaListPath)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";
            XmlWriter xml = XmlWriter.Create(MediaListPath, settings);
            xml.WriteStartElement("MediaList");
            foreach (string mediaPath in mediaNames)
            {
                xml.WriteElementString("Media", mediaPath);
            }
            xml.WriteEndElement();
            xml.Flush();
            xml.Close();
        }

        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PlaySelectedMedia();
        }

        private void McMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (!listView.Items.MoveCurrentToNext())
                listView.Items.MoveCurrentToFirst();
            listView.SelectedIndex = listView.Items.CurrentPosition;
            PlaySelectedMedia();
        }

        /// <summary>
        /// 播放当前选中的歌曲
        /// </summary>
        private void PlaySelectedMedia()
        {
            ListBoxItem temp = (ListBoxItem)listView.SelectedItem;//获取被选中条目
            if (temp == null)
                return;
            string mediaUrl = temp.Tag.ToString();
            if (string.IsNullOrEmpty(mediaUrl))
            {
                MessageBox.Show("当前播放列表无歌曲，请点击添加按钮添加歌曲。", "提示");
                return;
            }
            McMediaElement.Source = new Uri(mediaUrl);
            PlayingMedia();
        }

    }
}
