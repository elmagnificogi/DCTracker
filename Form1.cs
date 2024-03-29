﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.ListViewItem;

namespace DCTracker
{
    public partial class Form1 : Form
    {
        List<DC> diabloCloneProgress = new List<DC>();

        public Form1()
        {
            InitializeComponent();
        }
        private string GetWebClient(string url)
        {
            string strHTML = "";
            WebClient myWebClient = new WebClient();
            myWebClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " +
                                  "Windows NT 5.2; .NET CLR 1.0.3705;)");
            Stream myStream = myWebClient.OpenRead(url);
            StreamReader sr = new StreamReader(myStream, System.Text.Encoding.GetEncoding("utf-8"));
            strHTML = sr.ReadToEnd();
            myStream.Close();
            return strHTML;
        }
        object lock1 = new object();
        private void DCProgress_Refresh()
        {

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            string html = GetWebClient("https://diablo2.io/dclonetracker.php");

            List<DC> dcp = new List<DC>();
            doc.LoadHtml(html);

            for (int i = 0; i < 12; i++)
            {
                int index = i + 1;

                string pro;
                string server;
                string ladder;
                string sc;
                string time;
                HtmlNode node = doc.DocumentNode.SelectSingleNode("//*[@id=\"memberlist\"]/tbody/tr[" + index.ToString() + "]/td[1]/span[1]/code");
                Debug.WriteLine(node.InnerText);  //输出节点内容     
                pro = node.InnerText;

                HtmlNode node2 = doc.DocumentNode.SelectSingleNode("//*[@id=\"memberlist\"]/tbody/tr[" + index.ToString() + "]/td[2]/span[1]/span");
                Debug.WriteLine(node2.InnerText);  //输出节点内容    
                if (node2.InnerText.Contains("Americas"))
                    server = "美服";
                else if (node2.InnerText.Contains("Europe"))
                    server = "欧服";
                else if (node2.InnerText.Contains("Asia"))
                    server = "亚服";
                else
                    server = "错误";

                HtmlNode node3 = doc.DocumentNode.SelectSingleNode("//*[@id=\"memberlist\"]/tbody/tr[" + index.ToString() + "]/td[3]/span[1]/span");
                Debug.WriteLine(node3.InnerText);  //输出节点内容    
                if (node3.InnerText.Contains("Non-Ladder"))
                    ladder = "经典";
                else if (node3.InnerText.Contains("Ladder"))
                    ladder = "天梯";
                else
                    ladder = "错误";

                HtmlNode node4 = doc.DocumentNode.SelectSingleNode("//*[@id=\"memberlist\"]/tbody/tr[" + index.ToString() + "]/td[4]/span[1]/span");
                Debug.WriteLine(node4.InnerText);  //输出节点内容    
                if (node4.InnerText.Contains("Softcore"))
                    sc = "普通";
                else if (node4.InnerText.Contains("Hardcore"))
                    sc = "专家";
                else
                    sc = "错误";

                HtmlNode node5 = doc.DocumentNode.SelectSingleNode("//*[@id=\"memberlist\"]/tbody/tr[" + index.ToString() + "]/td[5]/span[1]/span");
                Debug.WriteLine(node5.InnerText);  //输出节点内容    
                time = node5.InnerText;
                time = time.Replace("days", "天");
                time = time.Replace("day", "天");
                time = time.Replace("week", "周");
                time = time.Replace("weeks", "周");
                time = time.Replace("minutes", "分钟");
                time = time.Replace("minute", "分钟");
                time = time.Replace("hours", "小时");
                time = time.Replace("hour", "小时");
                time = time.Replace("ago", "前");
                time = time.Replace(" ", "");
                time = time.Trim();

                dcp.Add(new DC(server, sc, time, ladder, pro));
            }
            diabloCloneProgress = dcp;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Task task = Task.Run(() =>
            {
                try
                {
                    if (Monitor.TryEnter(lock1))
                    {
                        DCProgress_Refresh();
                        Debug.WriteLine("解锁");
                        Monitor.Exit(lock1);
                    }
                    else
                    {
                        Debug.WriteLine("锁");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            });
            
        }

        void listViewReset()
        {
            //添加列名
            ColumnHeader c1 = new ColumnHeader();
            c1.Width = 40;
            c1.Text = "进度";
            ColumnHeader c2 = new ColumnHeader();
            c2.Width = 50;
            c2.Text = "服务器";
            ColumnHeader c3 = new ColumnHeader();
            c3.Width = 70;
            c3.Text = "天梯/经典";
            ColumnHeader c4 = new ColumnHeader();
            c4.Width = 70;
            c4.Text = "普通/专家";
            ColumnHeader c5 = new ColumnHeader();
            c5.Width = 65;
            c5.Text = "更新";

            //设置属性
            listView1.GridLines = true;  //显示网格线
            listView1.FullRowSelect = true;  //显示全行
            listView1.MultiSelect = false;  //设置只能单选
            listView1.View = View.Details;  //设置显示模式为详细
            listView1.HoverSelection = false;  //当鼠标停留数秒后自动选择
            //把列名添加到listView1中
            listView1.Columns.Add(c1);
            listView1.Columns.Add(c2);
            listView1.Columns.Add(c3);
            listView1.Columns.Add(c4);
            listView1.Columns.Add(c5);

            for (int i = 0; i < 12; i++)
            {
                ListViewItem lvi = new ListViewItem();
                ListViewSubItem pro = new ListViewSubItem();
                ListViewSubItem server = new ListViewSubItem();
                ListViewSubItem sc = new ListViewSubItem();
                ListViewSubItem mode = new ListViewSubItem();
                ListViewSubItem exp = new ListViewSubItem();
                lvi.SubItems.Add(pro);
                lvi.SubItems.Add(server);
                lvi.SubItems.Add(sc);
                lvi.SubItems.Add(mode);
                lvi.SubItems.Add(exp);
                listView1.Items.Add(lvi);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string Version = "DC追踪-elmagnifico-V"+Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.Text = Version;
            comboBox1.SelectedIndex = 4;
            listViewReset();
            var task = Task.Run(() =>
            {
                while (true)
                {
                    if (Monitor.TryEnter(lock1))
                    {
                        DCProgress_Refresh();
                        Debug.WriteLine("解锁");
                        Monitor.Exit(lock1);
                    }
                    else
                    {
                        Debug.WriteLine("锁");
                    }
                    Thread.Sleep(5000);
                }
            });
        }
        [DllImport("User32.dll", CharSet = CharSet.Unicode, EntryPoint = "FlashWindow")]
        private static extern void FlashWindow(IntPtr hwnd, bool bInvert);
        [DllImport("kernel32.dll")]
        public static extern bool Beep(int freq, int duration);

        private int flash_interval = 2;
        private int flash_phase = 0;
        private int warning_interval = 2;
        private int warning_phase = 0;

        private bool CheckWarning(string pro)
        {
            pro = pro.Trim();
            int p1 = int.Parse(""+pro[0]);
            int p2 = int.Parse("" + (comboBox1.Text.Trim())[0]);
            return p1 > p2;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                bool needWarning = false;
                var windowInApplicationIsFocused = Form.ActiveForm != null;
                Debug.WriteLine(windowInApplicationIsFocused);
                int p1 = 0;
                for (int i = 0; i < 12; i++)
                {
                    listView1.Items[i].SubItems[0].Text = diabloCloneProgress[i].progress;
                    listView1.Items[i].SubItems[1].Text = diabloCloneProgress[i].region;
                    listView1.Items[i].SubItems[2].Text = diabloCloneProgress[i].ladder;
                    listView1.Items[i].SubItems[3].Text = diabloCloneProgress[i].sc;
                    listView1.Items[i].SubItems[4].Text = diabloCloneProgress[i].time;
                    if (diabloCloneProgress[i].progress.Contains("3/6"))
                    {
                        listView1.Items[i].SubItems[0].ForeColor = System.Drawing.Color.DarkGreen;
                    }
                    else if (diabloCloneProgress[i].progress.Contains("4/6"))
                    {
                        listView1.Items[i].SubItems[0].ForeColor = System.Drawing.Color.Orange;
                    }
                    else if (diabloCloneProgress[i].progress.Contains("5/6"))
                    {
                        listView1.Items[i].SubItems[0].ForeColor = System.Drawing.Color.Red;
                    }
                    else if (diabloCloneProgress[i].progress.Contains("2/6"))
                    {
                        listView1.Items[i].SubItems[0].ForeColor = System.Drawing.Color.RosyBrown;
                    }
                    else if (diabloCloneProgress[i].progress.Contains("6/6"))
                    {
                        listView1.Items[i].SubItems[0].ForeColor = System.Drawing.Color.DarkRed;
                    }
                    else
                    {
                        listView1.Items[i].SubItems[0].ForeColor = System.Drawing.Color.Black;
                    }

                    string pro = diabloCloneProgress[i].progress.Trim();
                    p1 = Math.Max(p1,int.Parse("" + pro[0]));
                }

                int p2 = int.Parse("" + (comboBox1.Text.Trim())[0]);

                needWarning = p1>=p2;

                if (windowInApplicationIsFocused)
                {
                    warning_interval = 5;
                }
                else
                {
                    warning_interval = 1;
                }

                if(needWarning)
                {
                    warning_phase++;
                }
                else
                {
                    warning_phase = 0;
                }

                if (warning_phase >= warning_interval)
                {
                    warning_phase = 0;
                    flash_phase++;
                    if (flash_phase == flash_interval && !windowInApplicationIsFocused)
                    {
                        FlashWindow(this.Handle, true);
                        flash_phase = 0;
                    }
                    Beep(800, 300);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

        }
    }
}
