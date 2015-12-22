using System;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace CSharp_Task3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //给刷票地址url发送times次请求
        private int vote(String url,int times)
        {
            if (url == null||times == -1)  //地址无效
                return 0;
            try
            {
                Random r = new Random();
                for (int i = 1; i <= times; i++)
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                    req.Method = "GET";
                    req.Headers["X-Forwarded-For"] = "202." + r.Next(256) + "." + r.Next(256) + "." + r.Next(256);
                    req.Timeout = 2000;
                    req.GetResponse().Close();
                    label2.Text = "第" + i + "次刷票";
                    this.Refresh();
                    //Thread.Sleep(1000);  //两次刷票间延时
                }
            }catch(Exception e)  //发生异常
            {
                return -1;
            }
            return 1;  //正常退出
        }

        //获取页面文本
        private String webCatch(String url)
        {
            if (url == null || url.Equals(""))
                return null;
            String result;
            Encoding myEncoding = Encoding.GetEncoding("gb2312");
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            try
            {
                HttpWebResponse myResponse = (HttpWebResponse)req.GetResponse();
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), myEncoding);
                result = reader.ReadToEnd();
                reader.Close();
            }
            catch (System.Net.WebException e)
            {
                return null;
            }
            return result;
        }

        //获取刷票使用的地址
        private String getCommendUrl(String result)
        {
            if (result != null && result.IndexOf("/commend/") >= 0)
                return "http://today.hit.edu.cn" + result.Substring(result.IndexOf("/commend/"), 16) + "1.htm";
            else
                return null;
        }

        //获取已刷的次数
        private int getCommendNum(String result)
        {
            try
            {
                String[] lines = result.Split('\n');
                return int.Parse(lines[35]);
            }catch(Exception e)
            {
                return -1;
            }

        }

        //获取想要刷票的次数
        private int getRepeatNum(String num)
        {
            try
            {
                return int.Parse(num);
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        //刷票线程
        private void run()
        {
            String url = getCommendUrl(webCatch(textBox1.Text));
            int t = vote(url,getRepeatNum(textBox2.Text));
            if (t == 1)
                label2.Text = "刷票成功，现在推荐量为"+ getCommendNum(webCatch(url));
            else if (t == 0)
                label2.Text = "不正确的地址或次数";
            else if (t == -1)
                label2.Text = "刷票过程中出现网络异常，现在推荐量为" + getCommendNum(webCatch(url));
        }

        //点击则刷票
        private void button2_Click(object sender, EventArgs e)
        {
            run();
        }

    }
}
