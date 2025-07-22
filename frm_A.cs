using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.IO;
using NBA.Helpers;
using System.Windows.Forms.DataVisualization.Charting;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace NBA
{
    public partial class frm_A : Form
    {
        // DeepSeek API Key 和 URL
        private static readonly string API_KEY = ConfigurationManager.AppSettings["DeepSeekApiKey"];
        private static readonly string API_URL = ConfigurationManager.AppSettings["DeepSeekApiUrl"];
        // 在 frm_A 类中添加字段，保存历史对话
        private List<Dictionary<string, string>> chatHistory = new();

        // 在 frm_A 类中添加字段，保存顶部按钮集合
        private List<Button> topMenuButtons;

        // 在类中添加一个变量，记录高亮状态
        private enum RankingMode { Player, Team }
        private RankingMode currentRankingMode = RankingMode.Player;

        // 在 frm_A 类中添加字段
        private List<Image> pictureList;
        private System.Windows.Forms.Timer pictureTimer;
        private Random rand = new Random();

        public frm_A()
        {
            InitializeComponent();
            this.Resize += frm_main_Resize;
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void frm_main_Resize(object sender, EventArgs e)
        {
            int totalRatio = 2 + 9;
            int availableWidth = this.ClientSize.Width;
            int availableHeight = this.ClientSize.Height;

            int panel1Height = availableHeight * 2 / totalRatio;
            int playerPanelHeight = availableHeight - panel1Height;

            // panel1
            panel1.Width = availableWidth;
            panel1.Height = panel1Height;
            panel1.Left = 0;
            panel1.Top = 0;

            // player_panel

        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            tabControl_main.SelectedIndex = 0;
        }
        private void players_Click(object sender, EventArgs e)
        {
            tabControl_main.SelectedIndex = 1; // 切换到players页

            // 使用 DbHelper 获取数据
            DataTable dt = DbHelper.GetPlayerMapping();
            dataGridViewPlayers.DataSource = dt;

            // 设置列宽和表头
            if (dataGridViewPlayers.Columns.Contains("player_id"))
            {
                dataGridViewPlayers.Columns["player_id"].Width = 40;
                dataGridViewPlayers.Columns["player_id"].HeaderText = "ID";
            }
            if (dataGridViewPlayers.Columns.Contains("photo"))
            {
                dataGridViewPlayers.Columns["photo"].Width = 100;
                dataGridViewPlayers.Columns["photo"].HeaderText = "headshot";
            }
            dataGridViewPlayers.RowHeadersVisible = false;

            // 填充球员名称到 checkedListBox2
            FillPlayerNamesToCheckedListBox2();
        }

        private void teams_Click(object sender, EventArgs e)
        {
            tabControl_main.SelectedIndex = 2;
            // 使用配置文件中的连接字符串
            string connectionString = ConfigurationManager.ConnectionStrings["SecondConnection"].ConnectionString;
            string query = "SELECT * FROM [dbo].[TeamStats] ORDER BY [得分] DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                team_dataGridView.DataSource = dt;
                team_dataGridView.RowHeadersVisible = false; // 关键代码，隐藏行头
            }
            FillComboBox2WithTeams();
        }


        private void data_analysis_Click(object sender, EventArgs e)
        {
            tabControl_main.SelectedIndex = 3; // tabPage3
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tabControl_main.SelectedIndex = 4; // tabPage4
        }

        private void button3_Click(object sender, EventArgs e)
        {
            tabControl_main.SelectedIndex = 5; // tabPage5
            string connStr = ConfigurationManager.ConnectionStrings["NBAScheduleConnection"].ConnectionString;
            string sql = "SELECT * FROM [dbo].[Schedule]";

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    scheduleTable = dt;
                    dataGridView5.DataSource = scheduleTable;
                    dataGridView5.RowHeadersVisible = false;


                    if (dataGridView5.Columns.Contains("Year"))
                    {
                        dataGridView5.Columns["Year"].DisplayIndex = 0;
                        dataGridView5.Columns["Year"].Width = 50;
                    }
                    // 其他列宽度设置为较宽
                    foreach (DataGridViewColumn col in dataGridView5.Columns)
                    {
                        if (col.Name != "Year")
                        {
                            col.Width = 139;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据库操作失败：" + ex.Message);
            }
        }


        private void frm_main_Load(object sender, EventArgs e)
        {
            // 设置 question_box 占位符
            this.question_box.ForeColor = System.Drawing.Color.Gray;
            this.question_box.Text = "遇事不决？就问DeepSeek！！！";
            this.question_box.GotFocus += (s, ev) =>
            {
                if (question_box.Text == "遇事不决？就问DeepSeek！！！")
                {
                    question_box.Text = "";
                    question_box.ForeColor = System.Drawing.Color.Black;
                }
            };
            this.question_box.LostFocus += (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(question_box.Text))
                {
                    question_box.Text = "遇事不决？就问DeepSeek！！！";
                    question_box.ForeColor = System.Drawing.Color.Gray;
                }
            };

            // 初始化顶部按钮集合
            topMenuButtons = new List<Button> { players, teams, data_analysis, button4, button3, DS };
            foreach (var btn in topMenuButtons)
            {
                btn.Click += TopMenuButton_Click;
            }

            // 填充球员名称到 checkedListBox2
            FillPlayerNamesToCheckedListBox2();

            // 应用运动风主题
            ApplySportyTheme();

            // 初始化图片列表（资源名与资源管理器一致）
            pictureList = new List<Image>
            {
                Properties.Resources.a1,
                Properties.Resources.a2,
                Properties.Resources.a3,
                Properties.Resources.a4,
                Properties.Resources.a5,
                Properties.Resources.a6
                // ...可继续添加
            };

            // 初始化Timer
            pictureTimer = new System.Windows.Forms.Timer();
            pictureTimer.Interval = 2000; // 切换间隔，单位毫秒
            pictureTimer.Tick += PictureTimer_Tick;
            pictureTimer.Start();

            // 设置无边框和无控制栏
            axWindowsMediaPlayer1.uiMode = "none";
            //axWindowsMediaPlayer1.BorderStyle = 0;
            // 设置循环播放
            axWindowsMediaPlayer1.settings.setMode("loop", true);

            // 播放本地视频
            //axWindowsMediaPlayer1.URL = @"../../../nba图片/NBA.mp4";
            // 推荐：基于程序启动目录的绝对路径
            string videoPath = Path.Combine(Application.StartupPath, @"..\..\..\nba图片\NBA.mp4");
            axWindowsMediaPlayer1.URL = Path.GetFullPath(videoPath);
            axWindowsMediaPlayer1.Ctlcontrols.play();

            // 或播放项目内资源（确保文件已复制到输出目录）
            // axWindowsMediaPlayer1.URL = Application.StartupPath + @"\Resources\yourvideo.mp4";
            // axWindowsMediaPlayer1.Ctlcontrols.play();

        }

        private void player_panel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel_teams_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridViewPlayers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // 获取选中行的球员id，假设id在第一列
                var playerId = dataGridViewPlayers.Rows[e.RowIndex].Cells[0].Value.ToString();
                string tableName = $"player_{playerId}";

                // 获取连接字符串
                string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                string sql = $"SELECT * FROM [{tableName}]";

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // 显示到 dataGridView_player_inf
                    dataGridView_player_inf.DataSource = dt;
                }


            }
        }



        private void dataGridViewPlayers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // 在这里添加处理单元格点击事件的代码
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void DS_Click(object sender, EventArgs e)
        {
            tabControl_main.SelectedIndex = 6; // 切换到第6个tab页（索引从0开始）
        }

        private void result_box_TextChanged(object sender, EventArgs e)
        {

        }

        private void question_box_TextChanged(object sender, EventArgs e)
        {

        }

        // 修改 send_Click 方法，发送时将历史对话和本次输入都传给 API
        private async void send_Click(object sender, EventArgs e)
        {
            string userInput = question_box.Text.Trim();
            if (string.IsNullOrEmpty(userInput) || userInput == "遇事不决？就问DeepSeek！！！") return;

            // 添加用户消息到历史
            chatHistory.Add(new Dictionary<string, string> { { "role", "user" }, { "content", userInput } });

            result_box.AppendText($"你: {userInput}\r\n");
            result_box.AppendText("AI正在思考，请稍候...\r\n");
            send.Enabled = false;

            try
            {
                await AIHelper.GetAIResponseStreaming(chatHistory, API_KEY, API_URL, result_box);
                result_box.AppendText("\r\n\r\n");
            }
            catch (Exception ex)
            {
                if (!result_box.IsDisposed)
                    result_box.AppendText($"错误: {ex.Message}\r\n");
            }
            finally
            {
                send.Enabled = true;
                question_box.Clear();
            }
        }

        private void clearChat_Click(object sender, EventArgs e)
        {
            chatHistory.Clear();
            result_box.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tabControl_main.SelectedIndex = 2;

            if (dataGridViewPlayers.CurrentRow != null)
            {
                var playerId = dataGridViewPlayers.CurrentRow.Cells[0].Value.ToString();
                // 获取球员名并显示到label5
                var playerName = dataGridViewPlayers.CurrentRow.Cells["player_name"].Value?.ToString();
                label5.Text = playerName ?? "";

                // 1. 显示 player_{playerId} 表到 dataGridView_player_inf
                dataGridView_player_inf.DataSource = DbHelper.GetPlayerTable(playerId);

                // 2. 查询 player_mapping 表，显示详细信息到 richTextBox1 和 pictureBoxPlayer
                DataTable dt = DbHelper.GetPlayerMappingById(playerId);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"姓名：{row["player_name"]}");
                    sb.AppendLine($"位置：{row["position"]}");
                    sb.AppendLine($"身高：{row["height"]}");
                    sb.AppendLine($"体重：{row["weight"]}");
                    sb.AppendLine($"生日：{row["birth"]}");
                    sb.AppendLine($"球队：{row["team"]}");
                    sb.AppendLine($"学校：{row["school"]}");
                    sb.AppendLine($"选秀：{row["draft"]}");

                    richTextBox1.Text = sb.ToString();

                    // 显示头像
                    if (row["photo"] != DBNull.Value)
                    {
                        byte[] imgBytes = (byte[])row["photo"];
                        using (var ms = new System.IO.MemoryStream(imgBytes))
                        {
                            pictureBoxPlayer.Image = Image.FromStream(ms);
                        }
                    }
                    else
                    {
                        pictureBoxPlayer.Image = null;
                    }
                }
                else
                {
                    richTextBox1.Text = "未找到球员详细信息。";
                    pictureBoxPlayer.Image = null;
                }
            }
            else
            {
                MessageBox.Show("请先在球员列表中选择一行！");
            }

            DrawLineChart("得分", "助攻", "命中率");
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (dataGridViewPlayers.CurrentRow != null)
            {
                var playerId = dataGridViewPlayers.CurrentRow.Cells[0].Value.ToString();
                string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                // 1. 显示 player_{playerId} 表到 dataGridView_player_inf
                string tableName = $"player_{playerId}";
                string sqlPlayerData = $"SELECT * FROM [{tableName}]";
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(sqlPlayerData, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView_player_inf.DataSource = dt;
                }

                // 2. 查询 player_mapping 表，显示详细信息到 richTextBox1 和 pictureBoxPlayer
                string sqlMapping = "SELECT [player_name], [photo], [position], [height], [weight], [birth], [team], [school], [draft] FROM player_mapping WHERE player_id = @playerId";
                string playerName = null;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(sqlMapping, conn);
                    adapter.SelectCommand.Parameters.AddWithValue("@playerId", playerId);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine($"姓名：{row["player_name"]}");
                        sb.AppendLine($"位置：{row["position"]}");
                        sb.AppendLine($"身高：{row["height"]}");
                        sb.AppendLine($"体重：{row["weight"]}");
                        sb.AppendLine($"生日：{row["birth"]}");
                        sb.AppendLine($"球队：{row["team"]}");
                        sb.AppendLine($"学校：{row["school"]}");
                        sb.AppendLine($"选秀：{row["draft"]}");

                        richTextBox1.Text = sb.ToString();

                        // 显示头像
                        if (row["photo"] != DBNull.Value)
                        {
                            byte[] imgBytes = (byte[])row["photo"];
                            using (var ms = new System.IO.MemoryStream(imgBytes))
                            {
                                pictureBoxPlayer.Image = Image.FromStream(ms);
                            }
                        }
                        else
                        {
                            pictureBoxPlayer.Image = null;
                        }

                        // 获取球员姓名
                        playerName = row["player_name"].ToString();
                    }
                    else
                    {
                        richTextBox1.Text = "未找到球员详细信息。";
                        pictureBoxPlayer.Image = null;
                    }
                }

                // 3. 向 DeepSeek 发送请求，显示到 richTextBox2
                if (!string.IsNullOrEmpty(playerName))
                {
                    string prompt1 = $"请快速简要介绍篮球运动员{playerName}的技术特点。";
                    string prompt2 = $"请快速简要介绍篮球运动员{playerName}的职业生涯。快速";
                    string prompt3 = $"请快速简要介绍篮球运动员{playerName}的主要成就、成长背景和社会影响。";
                    if (richTextBox2 != null)
                    {
                        richTextBox2.Text = "正在查找，请稍候...\n";
                        try
                        {
                            // 第一次提问
                            var messages1 = new List<Dictionary<string, string>>
                            {
                                new Dictionary<string, string> { { "role", "user" }, { "content", prompt1 } }
                            };
                            await AIHelper.GetAIResponseStreaming(messages1, API_KEY, API_URL, richTextBox2);

                            // 第二次提问
                            var messages2 = new List<Dictionary<string, string>>
                            {
                                new Dictionary<string, string> { { "role", "user" }, { "content", prompt2 } }
                            };
                            await AIHelper.GetAIResponseStreaming(messages2, API_KEY, API_URL, richTextBox2);

                            // 第三次提问
                            var messages3 = new List<Dictionary<string, string>>
                            {
                                new Dictionary<string, string> { { "role", "user" }, { "content", prompt3 } }
                            };
                            await AIHelper.GetAIResponseStreaming(messages3, API_KEY, API_URL, richTextBox2);
                        }
                        catch (Exception ex)
                        {
                            richTextBox2.Text = $"错误: {ex.Message}";
                        }
                    }
                }
                else
                {
                    if (richTextBox2 != null)
                        richTextBox2.Text = "未找到球员姓名。";
                }
            }
            else
            {
                MessageBox.Show("请先在球员列表中选择一行！");
            }
        }

        private void DrawLineChart(params string[] yFields)
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();

            ChartArea area = new ChartArea("MainArea");
            chart1.ChartAreas.Add(area);

            var dt = dataGridView_player_inf.DataSource as DataTable;
            if (dt == null) return;
            Color[] colors = new Color[]
        {
    Color.Blue,
    Color.Red,
    Color.Green,
    Color.Orange,
    Color.Purple,
    Color.Brown,
    Color.Teal,
    Color.Magenta,
    Color.Gold,
    Color.DarkCyan
        };
            // 1. 先排序
            DataView dv = dt.DefaultView;
            dv.Sort = "日期 ASC";
            DataTable sortedDt = dv.ToTable();

            // 2. 用字典去重，保证每个日期只有一个点
            for (int i = 0; i < yFields.Length; i++)
            {
                var yField = yFields[i];
                Series series = new Series(yField)
                {
                    ChartType = SeriesChartType.FastLine,
                    BorderWidth = 2,
                    XValueType = ChartValueType.DateTime,
                    Color = colors[i % colors.Length] // 每条线不同颜色
                };

                var dateValueDict = new Dictionary<DateTime, double>();

                foreach (DataRow row in sortedDt.Rows)
                {
                    string dateStr = row["日期"]?.ToString();
                    if (string.IsNullOrWhiteSpace(dateStr)) continue;

                    if (DateTime.TryParseExact(dateStr, "MM/dd", null, System.Globalization.DateTimeStyles.None, out DateTime md))
                    {
                        int year = (md.Month > 6) ? 2024 : 2025;
                        DateTime fullDate = new DateTime(year, md.Month, md.Day);

                        string valueStr = row[yField]?.ToString();
                        if (string.IsNullOrWhiteSpace(valueStr)) continue;

                        valueStr = valueStr.Replace("%", "");
                        if (double.TryParse(valueStr, out double yValue))
                        {
                            // 只保留第一个或最后一个
                            if (!dateValueDict.ContainsKey(fullDate))
                                dateValueDict[fullDate] = yValue;
                        }
                    }
                }

                // 3. 按日期顺序添加点
                foreach (var kv in dateValueDict.OrderBy(kv => kv.Key))
                {
                    series.Points.AddXY(kv.Key, kv.Value);
                }

                chart1.Series.Add(series);
            }

            chart1.Legends.Clear();
            chart1.Legends.Add(new Legend("Legend1"));
            area.AxisX.LabelStyle.Format = "yyyy-MM-dd";
            area.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
        }

        private void DrawCompareLineChart(string playerId1, string playerId2, string yField)
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();

            ChartArea area = new ChartArea("MainArea");
            chart1.ChartAreas.Add(area);

            // 获取两位球员的数据
            DataTable dt1 = DbHelper.GetPlayerTable(playerId1);
            DataTable dt2 = DbHelper.GetPlayerTable(playerId2);

            // 画第一个球员
            Series series1 = new Series($"球员{playerId1}")
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 2,
                XValueType = ChartValueType.DateTime
            };
            AddSeriesPoints(series1, dt1, yField);

            // 画第二个球员
            Series series2 = new Series($"球员{playerId2}")
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 2,
                XValueType = ChartValueType.DateTime
            };
            AddSeriesPoints(series2, dt2, yField);

            chart1.Series.Add(series1);
            chart1.Series.Add(series2);

            //chart1.Legends.Clear();
            //chart1.Legends.Add(new Legend());
            chart1.Legends.Clear();
            Legend legend = new Legend("Legend1");
            legend.Font = new Font("微软雅黑", 7F); // 更小字体
            legend.IsDockedInsideChartArea = false;
            legend.Position = new ElementPosition(70, 5, 28, 20); // 控制位置和大小（百分比）
            legend.MaximumAutoSize = 15; // 最大宽度百分比
            legend.ItemColumnSpacing = 1; // 项间距
            legend.TableStyle = LegendTableStyle.Auto;
            legend.LegendStyle = LegendStyle.Table; // 横向紧凑排列

            chart1.Legends.Add(legend);

            area.AxisX.LabelStyle.Format = "yyyy-MM-dd";
            area.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
        }

        // 辅助方法：为Series添加点
        private void AddSeriesPoints(Series series, DataTable dt, string yField)
        {
            if (dt == null) return;
            DataView dv = dt.DefaultView;
            dv.Sort = "日期 ASC";
            DataTable sortedDt = dv.ToTable();

            foreach (DataRow row in sortedDt.Rows)
            {
                string dateStr = row["日期"]?.ToString();
                if (string.IsNullOrWhiteSpace(dateStr)) continue;

                if (DateTime.TryParseExact(dateStr, "MM/dd", null, System.Globalization.DateTimeStyles.None, out DateTime md))
                {
                    int year = (md.Month > 6) ? 2024 : 2025;
                    DateTime fullDate = new DateTime(year, md.Month, md.Day);

                    string valueStr = row[yField]?.ToString();
                    if (string.IsNullOrWhiteSpace(valueStr)) continue;

                    // 处理百分号
                    valueStr = valueStr.Replace("%", "");
                    if (double.TryParse(valueStr, out double yValue))
                    {
                        // 如果原始数据是百分比，按实际需求决定是否除以100
                        // series.Points.AddXY(fullDate, yValue / 100.0); // 如果想显示0-1区间
                        series.Points.AddXY(fullDate, yValue); // 如果想显示0-100区间
                    }
                }
            }
        }

        private void chart2_Click(object sender, EventArgs e)
        {

        }

        // 公共点击事件处理方法
        private void TopMenuButton_Click(object sender, EventArgs e)
        {
            foreach (var btn in topMenuButtons)
            {
                btn.BackColor = SystemColors.Control; // 恢复默认色
                btn.ForeColor = Color.Black;
            }
            var clickedBtn = sender as Button;
            if (clickedBtn != null)
            {
                clickedBtn.BackColor = Color.DodgerBlue; // 高亮色
                clickedBtn.ForeColor = Color.White;
            }

            // 可选：根据按钮切换页面
            if (clickedBtn == players) tabControl_main.SelectedIndex = 1;
            else if (clickedBtn == teams) tabControl_main.SelectedIndex = 2;
            else if (clickedBtn == data_analysis) tabControl_main.SelectedIndex = 3;
            else if (clickedBtn == button4) tabControl_main.SelectedIndex = 4;
            else if (clickedBtn == button3) tabControl_main.SelectedIndex = 5;
            else if (clickedBtn == DS) tabControl_main.SelectedIndex = 6;
        }

        //private void button5_Click(object sender, EventArgs e)
        private async void button5_Click(object sender, EventArgs e)
        {
            if (team_dataGridView.CurrentRow == null)
            {
                MessageBox.Show("请先选择一个队伍！");
                return;
            }

            // 获取队伍名称（假设列名为"球队"，如有不同请修改）
            string teamName = team_dataGridView.CurrentRow.Cells["球队"].Value?.ToString();
            if (string.IsNullOrWhiteSpace(teamName))
            {
                MessageBox.Show("未能获取队伍名称！");
                return;
            }

            // 获取队伍图片并显示到 pictureBox1
            string connStr = ConfigurationManager.ConnectionStrings["SecondConnection"].ConnectionString;
            string sqlPhoto = "SELECT [photo] FROM [TeamStats] WHERE [球队] = @teamName";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(sqlPhoto, conn);
                cmd.Parameters.AddWithValue("@teamName", teamName);
                conn.Open();
                var photoObj = cmd.ExecuteScalar();
                if (photoObj != DBNull.Value && photoObj != null)
                {
                    byte[] imgBytes = (byte[])photoObj;
                    using (var ms = new MemoryStream(imgBytes))
                    {
                        pictureBox1.Image = Image.FromStream(ms);
                    }
                }
                else
                {
                    pictureBox1.Image = null;
                }
            }
            // 构造问题
            string prompt1 = $"快速介绍{teamName}队伍的阵容";
            string prompt2 = $"快速介绍{teamName}队详细情况（不用介绍阵容）";

            // 显示等待提示
            richTextBox3.Text = "正在查找，请稍候...\n";
            richTextBox4.Text = "正在查找，请稍候...\n";

            try
            {
                // 准备两个请求的参数
                var messages1 = new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string> { { "role", "user" }, { "content", prompt1 } }
                };

                var messages2 = new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string> { { "role", "user" }, { "content", prompt2 } }
                };

                // 并行发送两个请求
                var task1 = AIHelper.GetAIResponseStreaming(messages1, API_KEY, API_URL, richTextBox3);
                var task2 = AIHelper.GetAIResponseStreaming(messages2, API_KEY, API_URL, richTextBox4);

                // 等待两个任务都完成
                await Task.WhenAll(task1, task2);
            }
            catch (Exception ex)
            {
                richTextBox3.Text = $"错误: {ex.Message}";
                richTextBox4.Text = $"错误: {ex.Message}";
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        // 通用排行榜查询方法
        private void ShowRanking(string connName, string tableName, string nameColumn, string valueColumn, DataGridView dgv)
        {
            string connStr = ConfigurationManager.ConnectionStrings[connName].ConnectionString;
            string sql = $"SELECT [{nameColumn}], [{valueColumn}] FROM [{tableName}] ORDER BY [{valueColumn}] DESC";
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    // 添加序号列
                    DataColumn col = new DataColumn("排名", typeof(int));
                    dt.Columns.Add(col);
                    // 将序号列移到第一列
                    dt.Columns["排名"].SetOrdinal(0);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["排名"] = i + 1;
                    }
                    dgv.AutoGenerateColumns = true;
                    dgv.DataSource = null;
                    dgv.Columns.Clear();
                    dgv.DataSource = dt;
                    dgv.RowHeadersVisible = false;
                    dgv.Columns["排名"].Width = 40;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据库操作失败：" + ex.Message);
            }
        }



        // button6：球员榜单
        private void button6_Click(object sender, EventArgs e)
        {
            button6.BackColor = Color.DodgerBlue;
            button6.ForeColor = Color.White;
            button7.BackColor = SystemColors.Control;
            button7.ForeColor = Color.Black;
            currentRankingMode = RankingMode.Player;

            ShowRanking("DefaultConnection", "player_mapping", "player_name", "得分", dataGridView1);
            ShowRanking("DefaultConnection", "player_mapping", "player_name", "投篮命中率", dataGridView3);
            ShowRanking("DefaultConnection", "player_mapping", "player_name", "三分命中率", dataGridView2);
        }

        // button7：球队榜单
        private void button7_Click(object sender, EventArgs e)
        {
            button7.BackColor = Color.DodgerBlue;
            button7.ForeColor = Color.White;
            button6.BackColor = SystemColors.Control;
            button6.ForeColor = Color.Black;
            currentRankingMode = RankingMode.Team;

            ShowRanking("SecondConnection", "TeamStats", "球队", "得分", dataGridView1);
            ShowRanking("SecondConnection", "TeamStats", "球队", "投篮命中率", dataGridView3);
            ShowRanking("SecondConnection", "TeamStats", "球队", "三分命中率", dataGridView2);
        }

        // button8：自定义字段榜单
        private void button8_Click(object sender, EventArgs e)
        {
            string selectedColumn = comboBox1.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(selectedColumn))
            {
                MessageBox.Show("请选择排序字段！");
                return;
            }

            if (currentRankingMode == RankingMode.Player)
            {
                ShowRanking("DefaultConnection", "player_mapping", "player_name", selectedColumn, dataGridView4);
            }
            else // RankingMode.Team
            {
                ShowRanking("SecondConnection", "TeamStats", "球队", selectedColumn, dataGridView4);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            // 获取 checkedListBox1 当前选中的所有字段
            var selectedFields = checkedListBox1.CheckedItems.Cast<string>().ToArray();
            if (selectedFields.Length == 0)
            {
                MessageBox.Show("请选择要显示的字段！");
                return;
            }
            // 获取当前球员历史得分
            string playerName = null;
            if (dataGridViewPlayers.CurrentRow != null)
                playerName = dataGridViewPlayers.CurrentRow.Cells["player_name"].Value?.ToString();

            label5.Text = playerName ?? "";
            DrawLineChart(selectedFields);

            // 预测得分（以“得分”字段为例）
            if (selectedFields.Contains("得分") && dataGridView_player_inf.DataSource is DataTable dt)
            {
                List<float> scores = new();
                foreach (DataRow row in dt.Rows)
                {
                    if (float.TryParse(row["得分"]?.ToString(), out float score))
                        scores.Add(score);
                }
                if (scores.Count > 3)
                {
                    float[] predicted = PredictPlayerScores(scores, 3);

                    // 在 chart1 上添加预测线
                    var series = chart1.Series["得分"];
                    int lastIndex = series.Points.Count > 0 ? series.Points.Count : 0;
                    for (int i = 0; i < predicted.Length; i++)
                    {
                        var pt = series.Points.AddY(predicted[i]);
                        series.Points[lastIndex + i].Color = Color.OrangeRed; // 预测点用不同颜色
                        series.Points[lastIndex + i].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Star5;
                    }
                }
            }
        }

        // 1. 自动填充 checkedListBox2（球员名列表）
        private void FillPlayerNamesToCheckedListBox2()
        {
            checkedListBox2.Items.Clear();
            foreach (DataGridViewRow row in dataGridViewPlayers.Rows)
            {
                if (row.IsNewRow) continue;
                var playerName = row.Cells["player_name"].Value?.ToString();
                if (!string.IsNullOrEmpty(playerName))
                    checkedListBox2.Items.Add(playerName);
            }
        }

        // 4. 事件绑定（如未自动生成，需手动加上）
        private void checkedListBox2_SelectedIndexChanged(object sender, EventArgs e) { }
        private void button10_Click_ManualBind()
        {
            button10.Click += button10_Click;
        }
        // 在类中添加
        private string RemoveEnglishName(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            int idx = name.IndexOf('（');
            if (idx >= 0)
                return name.Substring(0, idx).Trim();
            return name.Trim();
        }
        // button10：多球员对比
        private void button10_Click(object sender, EventArgs e)
        {
            var selectedFields = checkedListBox1.CheckedItems.Cast<string>().ToArray();
            var selectedPlayers = checkedListBox2.CheckedItems.Cast<string>().ToArray();

            if (selectedFields.Length == 0)
            {
                MessageBox.Show("请选择要显示的字段！");
                return;
            }
            if (selectedPlayers.Length == 0)
            {
                MessageBox.Show("请选择要对比的球员！");
                return;
            }
            // 设置label5，格式为A vs B vs C
            label5.Text = string.Join(" vs ", selectedPlayers);

            chart1.Series.Clear();
            chart1.ChartAreas.Clear();
            ChartArea area = new ChartArea("MainArea");
            chart1.ChartAreas.Add(area);

            // 不同球员不同颜色，不同字段不同线型
            Color[] playerColors = new Color[]
    {
        Color.Blue, Color.Red, Color.Green, Color.Orange, Color.Purple,
        Color.Brown, Color.Teal, Color.Magenta, Color.Gold, Color.DarkCyan
    };
            var dashStyles = new[]
            {
                System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid,
                System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash,
                System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot,
                System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDot,
                System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDotDot
            };

            int playerIdx = 0;
            foreach (var playerName in selectedPlayers)
            {
                // 通过 player_name 查找 player_id
                string playerId = null;
                foreach (DataGridViewRow row in dataGridViewPlayers.Rows)
                {
                    if (row.IsNewRow) continue;
                    if (row.Cells["player_name"].Value?.ToString() == playerName)
                    {
                        playerId = row.Cells["player_id"].Value?.ToString();
                        break;
                    }
                }
                if (string.IsNullOrEmpty(playerId)) continue;

                // 获取该球员的数据表
                DataTable dt = DbHelper.GetPlayerTable(playerId);
                if (dt == null) continue;

                for (int fieldIdx = 0; fieldIdx < selectedFields.Length; fieldIdx++)
                {
                    var yField = selectedFields[fieldIdx];
                    string displayName = RemoveEnglishName(playerName);
                    Color lineColor = playerColors[playerIdx % playerColors.Length];
                    var dashStyle = dashStyles[fieldIdx % dashStyles.Length];

                    Series series = new Series($"{displayName}-{yField}")
                    {
                        ChartType = SeriesChartType.FastLine,
                        BorderWidth = 2,
                        XValueType = ChartValueType.DateTime,
                        Color = lineColor,
                        BorderDashStyle = dashStyle
                    };

                    // 填充数据
                    DataView dv = dt.DefaultView;
                    dv.Sort = "日期 ASC";
                    DataTable sortedDt = dv.ToTable();
                    var dateValueDict = new Dictionary<DateTime, double>();
                    foreach (DataRow row in sortedDt.Rows)
                    {
                        string dateStr = row["日期"]?.ToString();
                        if (string.IsNullOrWhiteSpace(dateStr)) continue;
                        if (DateTime.TryParseExact(dateStr, "MM/dd", null, System.Globalization.DateTimeStyles.None, out DateTime md))
                        {
                            int year = (md.Month > 6) ? 2024 : 2025;
                            DateTime fullDate = new DateTime(year, md.Month, md.Day);
                            string valueStr = row[yField]?.ToString();
                            if (string.IsNullOrWhiteSpace(valueStr)) continue;
                            valueStr = valueStr.Replace("%", "");
                            if (double.TryParse(valueStr, out double yValue))
                            {
                                if (!dateValueDict.ContainsKey(fullDate))
                                    dateValueDict[fullDate] = yValue;
                            }
                        }
                    }
                    foreach (var kv in dateValueDict.OrderBy(kv => kv.Key))
                    {
                        series.Points.AddXY(kv.Key, kv.Value);
                    }
                    chart1.Series.Add(series);
                }
                playerIdx++;
            }

            chart1.Legends.Clear();
            chart1.Legends.Add(new Legend("Legend1"));
            area.AxisX.LabelStyle.Format = "yyyy-MM-dd";
            area.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
        }

        // 1. 填充comboBox2（建议在窗体加载或数据加载后调用）
        private void FillComboBox2WithTeams()
        {
            comboBox2.Items.Clear();
            HashSet<string> teamNames = new HashSet<string>();
            foreach (DataGridViewRow row in team_dataGridView.Rows)
            {
                if (row.IsNewRow) continue;
                var value = row.Cells["球队"].Value?.ToString();
                if (!string.IsNullOrEmpty(value) && !teamNames.Contains(value))
                {
                    teamNames.Add(value);
                    comboBox2.Items.Add(value);
                }
            }
        }
        private DataTable scheduleTable;
        // 2. button11点击事件：模糊筛选赛程
        private void button11_Click(object sender, EventArgs e)
        {
            string selectedTeam = comboBox2.Text.Trim();
            if (string.IsNullOrEmpty(selectedTeam) || scheduleTable == null) return;

            // 假设只有一个 "Teams" 列
            string filter = $"Teams LIKE '%{selectedTeam}%'";
            DataView dv = new DataView(scheduleTable);
            dv.RowFilter = filter;
            dataGridView5.DataSource = dv;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            button11_Click(sender, e);
        }

        public class PlayerScoreData
        {
            public float GameIndex { get; set; }
            public float Score { get; set; }
        }

        public class ScorePrediction
        {
            public float Score { get; set; }
        }
        // 预测球员未来N场得分
        private float[] PredictPlayerScores(List<float> scores, int predictCount = 3)
        {
            var mlContext = new MLContext();
            var data = scores.Select((s, i) => new PlayerScoreData { GameIndex = i, Score = s }).ToList();
            var trainData = mlContext.Data.LoadFromEnumerable(data);

            var pipeline = mlContext.Transforms.Concatenate("Features", "GameIndex")
                .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: "Score", maximumNumberOfIterations: 100));

            var model = pipeline.Fit(trainData);

            float[] predictions = new float[predictCount];
            for (int i = 0; i < predictCount; i++)
            {
                var input = new PlayerScoreData { GameIndex = data.Count + i };
                var pred = mlContext.Model.CreatePredictionEngine<PlayerScoreData, ScorePrediction>(model).Predict(input);
                predictions[i] = pred.Score;
            }
            return predictions;
        }
        public static class NativeMethods
        {
            [System.Runtime.InteropServices.DllImport("gdi32.dll", SetLastError = true)]
            public static extern IntPtr CreateRoundRectRgn(
                int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
                int nWidthEllipse, int nHeightEllipse);
        }
        // 运动风美化方法
        private void ApplySportyTheme()
        {
            // panel1 背景色更亮
            if (panel1 != null) panel1.BackColor = ColorTranslator.FromHtml("#182B4D"); // 运动蓝


            // TabControl美化
            tabControl_main.Appearance = TabAppearance.Normal;
            tabControl_main.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl_main.ItemSize = new Size(120, 36);
            tabControl_main.SizeMode = TabSizeMode.Fixed;
            tabControl_main.DrawItem += (s, e) =>
            {
                var tab = tabControl_main.TabPages[e.Index];
                var g = e.Graphics;
                var rect = e.Bounds;
                bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
                Color backColor = selected ? ColorTranslator.FromHtml("#FF6F00") : ColorTranslator.FromHtml("#0D1333");
                Color foreColor = Color.White;
                using (SolidBrush b = new SolidBrush(backColor))
                    g.FillRectangle(b, rect);
                TextRenderer.DrawText(g, tab.Text, new Font("微软雅黑", 11, FontStyle.Bold), rect, foreColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };
            // 顶部按钮
            foreach (TabPage page in tabControl_main.TabPages)
            {
                page.BackColor = Color.White;
            }
            foreach (var btn in topMenuButtons)
            {
                btn.Font = new Font("微软雅黑", 16, FontStyle.Bold);
                btn.FlatStyle = FlatStyle.Flat;
                btn.BackColor = ColorTranslator.FromHtml("#0D1333");
                btn.ForeColor = Color.White;
                btn.FlatAppearance.BorderSize = 0;
                btn.Height = 60;
                btn.Width = 160;
                btn.Cursor = Cursors.Hand;

                btn.MouseEnter += (s, e) =>
                {
                    ((Button)s).BackColor = ColorTranslator.FromHtml("#FF6F00");
                };
                btn.MouseLeave += (s, e) =>
                {
                    // 如果当前按钮是高亮（选中）状态，则不还原
                    if (((Button)s).BackColor != Color.DodgerBlue)
                        ((Button)s).BackColor = ColorTranslator.FromHtml("#0D1333");
                };
            }
            // DataGridView美化
            void BeautifyDGV(DataGridView dgv)
            {
                dgv.EnableHeadersVisualStyles = false;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FF6F00"); // 橙色表头
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgv.ColumnHeadersDefaultCellStyle.Font = new Font("微软雅黑", 10, FontStyle.Bold);
                dgv.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F5F8FF"); // 主体淡蓝色
                dgv.DefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#0D1333");
                dgv.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#2979FF");
                dgv.DefaultCellStyle.SelectionForeColor = Color.White;
                dgv.RowTemplate.Height = 32;
                dgv.GridColor = ColorTranslator.FromHtml("#E3EAF2"); // 更淡的分隔线
                dgv.BorderStyle = BorderStyle.None;
            }
            BeautifyDGV(dataGridViewPlayers);
            BeautifyDGV(team_dataGridView);
            BeautifyDGV(dataGridView_player_inf);

            // Chart美化
            if (chart1 != null)
            {
                chart1.BackColor = Color.White;
                chart1.BorderlineColor = ColorTranslator.FromHtml("#FF6F00");
                chart1.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
                chart1.BorderlineWidth = 2;
                if (chart1.ChartAreas.Count > 0)
                    chart1.ChartAreas[0].BackColor = ColorTranslator.FromHtml("#F5F5F5");
                if (chart1.Legends.Count > 0)
                {
                    chart1.Legends[0].BackColor = Color.Transparent;
                    chart1.Legends[0].Font = new Font("微软雅黑", 10, FontStyle.Bold);
                }
            }

            // 主要Label字体
            foreach (var ctrl in this.Controls)
            {
                if (ctrl is Label lbl)
                {
                    lbl.Font = new Font("微软雅黑", 12, FontStyle.Bold);
                    lbl.ForeColor = ColorTranslator.FromHtml("#0D1333");
                }
            }
        }

        private void tabPage7_Click(object sender, EventArgs e)
        {

        }

        private void PictureTimer_Tick(object sender, EventArgs e)
        {
            if (pictureList != null && pictureList.Count > 0)
            {
                int idx = rand.Next(pictureList.Count);
                pictureBox3.Image = pictureList[idx];
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedItem != null)
            {
                question_box.Text = comboBox3.SelectedItem.ToString();
                question_box.ForeColor = Color.Black; // 可选：恢复正常字体颜色
            }
        }

        private void question_box_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                send.PerformClick();
                e.Handled = true;
                e.SuppressKeyPress = true; // 防止回车换行
            }
        }
    }
}
