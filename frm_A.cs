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

namespace NBA
{
    public partial class frm_A : Form
    {
        // DeepSeek API Key 和 URL
        private const string API_KEY = "yourAPIkey";
        private const string API_URL = "https://api.deepseek.com/v1/chat/completions";

        // 在 frm_A 类中添加字段，保存历史对话
        private List<Dictionary<string, string>> chatHistory = new();

        // 在 frm_A 类中添加字段，保存顶部按钮集合
        private List<Button> topMenuButtons;

        // 在类中添加一个变量，记录高亮状态
        private enum RankingMode { Player, Team }
        private RankingMode currentRankingMode = RankingMode.Player;

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

        private void players_Click(object sender, EventArgs e)
        {
            tabControl_main.SelectedIndex = 0; // 切换到players页

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
        }

        private void teams_Click(object sender, EventArgs e)
        {
            tabControl_main.SelectedIndex = 1;
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
        }


        private void data_analysis_Click(object sender, EventArgs e)
        {
            tabControl_main.SelectedIndex = 2; // tabPage3
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tabControl_main.SelectedIndex = 3; // tabPage4
        }

        private void button3_Click(object sender, EventArgs e)
        {
            tabControl_main.SelectedIndex = 4; // tabPage5
            string connStr = ConfigurationManager.ConnectionStrings["NBAScheduleConnection"].ConnectionString;
            string sql = "SELECT * FROM [dbo].[Schedule]";

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView5.DataSource = dt;
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
            tabControl_main.SelectedIndex = 5; // 切换到第6个tab页（索引从0开始）
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
            DrawLineChart("得分"); // 画“得分”折线图
            DrawLineChart("得分");
            DrawLineChart("助攻");
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

        private void DrawLineChart(string yField)
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();

            ChartArea area = new ChartArea("MainArea");
            chart1.ChartAreas.Add(area);

            Series series = new Series(yField)
            {
                ChartType = SeriesChartType.FastLine, // 或 Spline
                BorderWidth = 2,
                XValueType = ChartValueType.DateTime
            };

            var dt = dataGridView_player_inf.DataSource as DataTable;
            if (dt == null) return;

            // 新建一个排序后的DataView
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

                    if (double.TryParse(row[yField]?.ToString(), out double yValue) && yValue > 0)
                    {
                        series.Points.AddXY(fullDate, yValue);
                    }
                }
            }

            chart1.Series.Add(series);
            chart1.Legends.Clear();
            chart1.Legends.Add(new Legend("Legend"));

            // 设置x轴格式
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

            chart1.Legends.Clear();
            chart1.Legends.Add(new Legend("Legend"));

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

                    if (double.TryParse(row[yField]?.ToString(), out double yValue) && yValue > 0)
                    {
                        series.Points.AddXY(fullDate, yValue);
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
            if (clickedBtn == players) tabControl_main.SelectedIndex = 0;
            else if (clickedBtn == teams) tabControl_main.SelectedIndex = 1;
            else if (clickedBtn == data_analysis) tabControl_main.SelectedIndex = 2;
            else if (clickedBtn == button4) tabControl_main.SelectedIndex = 3;
            else if (clickedBtn == button3) tabControl_main.SelectedIndex = 4;
            else if (clickedBtn == DS) tabControl_main.SelectedIndex = 5;
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
    }
}
