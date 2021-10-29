using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SearchRank
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {           

            var url = "https://www.google.com.au/search";
            var keyword = tbKeywords.Text.Replace(" ","+");
            var limit = tbLimit.Text;

            var searchUrl = url + "?num=" + limit + "&q=" + keyword;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(searchUrl);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                string data = readStream.ReadToEnd();
                var pos = FindPositions(data, tbUrl.Text);
                tbResult.Text = pos;
                response.Close();
                readStream.Close();

            }
        }

        private static string FindPositions(string html, string url)
        {
            string output = "";
            Regex regex = new Regex("href=\"\\/url\\?q=\\s*(?:\"(?<x>[^\"]*)\"|(?<y>\\S+))", RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(html, 0);

            for (int i = 0; i < matches.Count && i < 99; i++)
            {
                string match = matches[i].Groups[2].Value;
                if (match.Contains(url)) output += (string.IsNullOrEmpty(output) ? "" : ",") + (i + 1).ToString();
                //string lookup = "(<h2 class=r><a href=\")(\\w+[a-zA-Z0-9.-?=/]*)";
            }
            if (string.IsNullOrEmpty(output)) output = "0";
            return output;
            //MatchCollection matches = Regex.Matches(html, lookup);
            //for (int i = 0; i < matches.Count; i++)
            //{
            //    string match = matches[i].Groups[2].Value;
            //    if (match.Contains(url)) return i + 1;
            //}
            //return 0;
        }
    }
}
