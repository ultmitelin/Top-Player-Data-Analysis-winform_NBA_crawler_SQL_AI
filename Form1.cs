using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using System.IO;

namespace NBA
{
    public partial class Form1 : Form
    {
        // 球员赛季数据结构
        public class PlayerSeasonStats
        {
            public string Season { get; set; }
            public double Minutes { get; set; }
            public double Rebounds { get; set; }
            public double Assists { get; set; }
            public double Steals { get; set; }
            public double Blocks { get; set; }
            public double Turnovers { get; set; }
            public double Points { get; set; }
        }

        // 球员URL列表
        private readonly List<string> playerUrls = new List<string>
        {
            "https://www.basketball-reference.com/players/l/leonaka01.html",
            "https://www.basketball-reference.com/players/d/duranke01.html",
            "https://www.basketball-reference.com/players/i/irvinky01.html",
            "https://www.basketball-reference.com/players/j/jamesle01.html",
            "https://www.basketball-reference.com/players/w/walkeke02.html",
            "https://www.basketball-reference.com/players/t/thompkl01.html",
            "https://www.basketball-reference.com/players/p/paulch01.html",
            "https://www.basketball-reference.com/players/h/hardeja01.html",
            "https://www.basketball-reference.com/players/c/curryst01.html",
            "https://www.basketball-reference.com/players/s/simmobe01.html",
            "https://www.basketball-reference.com/players/l/lillada01.html",
            "https://www.basketball-reference.com/players/h/harrito02.html",
            "https://www.basketball-reference.com/players/m/middlkh01.html",
            "https://www.basketball-reference.com/players/p/porzikr01.html",
            "https://www.basketball-reference.com/players/w/westbru01.html",
            "https://www.basketball-reference.com/players/a/antetgi01.html",
            "https://www.basketball-reference.com/players/e/embiijo01.html"
        };

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            foreach (var url in playerUrls)
            {
                var stats = await GetPlayerStatsAsync(url);
                DrawChart(stats, url);
                await Task.Delay(5000); // 等待5秒，类似Python的time.sleep(5)
            }
        }

        // 获取网页并解析数据
        public async Task<List<PlayerSeasonStats>> GetPlayerStatsAsync(string url)
        {
            var statsList = new List<PlayerSeasonStats>();
            using var client = new HttpClient();
            var html = await client.GetStringAsync(url);

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            var rows = doc.DocumentNode.SelectNodes("//table[@id='per_game_stats']//tbody/tr");
            
            //var rows = doc.DocumentNode.SelectNodes("//table[@id='per_game_stats']//tbody/tr[not(contains(@class,'thead'))]");
            //var rows = doc.DocumentNode.SelectNodes("//table[@id='per_game']//tbody/tr[not(contains(@class,'thead'))]");
            //var rows = doc.DocumentNode.SelectNodes("//table[@id='per_game']//tbody/tr[not(contains(@class,'thead'))]");
            if (rows == null)
            {
                MessageBox.Show("未找到赛季数据表，可能网页结构已变或被反爬。");
            }
            if (rows != null)
            {
                foreach (var row in rows)
                {
                    // 赛季
                    var seasonNode = row.SelectSingleNode("th[@data-stat='season']/a");
                    if (seasonNode == null) continue;
                    var season = seasonNode.InnerText;

                    // 其他数据
                    double TryParse(string xpath) =>
                        double.TryParse(row.SelectSingleNode(xpath)?.InnerText, out var val) ? val : 0;

                    statsList.Add(new PlayerSeasonStats
                    {
                        Season = season,
                        Minutes = TryParse("td[@data-stat='mp_per_g']"),
                        Rebounds = TryParse("td[@data-stat='trb_per_g']"),
                        Assists = TryParse("td[@data-stat='ast_per_g']"),
                        Steals = TryParse("td[@data-stat='stl_per_g']"),
                        Blocks = TryParse("td[@data-stat='blk_per_g']"),
                        Turnovers = TryParse("td[@data-stat='tov_per_g']"),
                        Points = TryParse("td[@data-stat='pts_per_g']")
                    });
                }
            }
                return statsList;
        }

        // 绘制统计图并保存图片
        public void DrawChart(List<PlayerSeasonStats> stats, string url)
        {
            var chart = new Chart();
            chart.Dock = DockStyle.Fill;
            chart.ChartAreas.Add(new ChartArea("Main"));

            string[] seasons = stats.ConvertAll(s => s.Season).ToArray();

            void AddSeries(string name, Func<PlayerSeasonStats, double> selector, Color color)
            {
                var series = new Series(name)
                {
                    ChartType = SeriesChartType.Column,
                    Color = color
                };
                for (int i = 0; i < stats.Count; i++)
                {
                    series.Points.AddXY(seasons[i], selector(stats[i]));
                }
                chart.Series.Add(series);
            }

            AddSeries("上场时间", s => s.Minutes, Color.SkyBlue);
            AddSeries("篮板数", s => s.Rebounds, Color.Red);
            AddSeries("助攻", s => s.Assists, Color.Cyan);
            AddSeries("抢断", s => s.Steals, Color.Magenta);
            AddSeries("盖帽", s => s.Blocks, Color.Purple);
            AddSeries("失误", s => s.Turnovers, Color.Green);
            AddSeries("平均得分", s => s.Points, Color.Yellow);

            chart.Legends.Add(new Legend("Legend"));

            // 设置标题
            string title = url.Split('/')[^1].Split('.')[0].Replace("01", "");
            chart.Titles.Add($"{title}赛季数据统计 @Python知识圈 制作");

            // 设置X轴标签倾斜
            chart.ChartAreas[0].AxisX.LabelStyle.Angle = 30;
            chart.ChartAreas[0].AxisX.Interval = 1;

            // 清空并显示
            this.Controls.Clear();
            this.Controls.Add(chart);

            // 保存图片到本地
            string savePath = Path.Combine(@"../../../nba图片", $"{title}.png");
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            chart.SaveImage(savePath, ChartImageFormat.Png);
        }

      
    }
}