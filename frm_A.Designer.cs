namespace NBA
{
    partial class frm_A
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            search_btn = new Button();
            panel1 = new Panel();
            button3 = new Button();
            button4 = new Button();
            data_analysis = new Button();
            teams = new Button();
            players = new Button();
            tabControl_main = new TabControl();
            tabPage1 = new TabPage();
            checkBox1 = new CheckBox();
            button2 = new Button();
            richTextBox2 = new RichTextBox();
            label1 = new Label();
            richTextBox1 = new RichTextBox();
            pictureBoxPlayer = new PictureBox();
            button1 = new Button();
            dataGridViewPlayers = new DataGridView();
            tabPage2 = new TabPage();
            tabPage3 = new TabPage();
            chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            dataGridView_player_inf = new DataGridView();
            tabPage4 = new TabPage();
            tabPage5 = new TabPage();
            tabPage6 = new TabPage();
            send = new Button();
            result_box = new RichTextBox();
            question_box = new TextBox();
            label2 = new Label();
            DS = new Button();
            panel1.SuspendLayout();
            tabControl_main.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPlayer).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewPlayers).BeginInit();
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)chart1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView_player_inf).BeginInit();
            tabPage6.SuspendLayout();
            SuspendLayout();
            // 
            // search_btn
            // 
            search_btn.Location = new Point(1234, 12);
            search_btn.Name = "search_btn";
            search_btn.Size = new Size(131, 65);
            search_btn.TabIndex = 2;
            search_btn.Text = "搜索";
            search_btn.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Controls.Add(button3);
            panel1.Controls.Add(button4);
            panel1.Controls.Add(data_analysis);
            panel1.Controls.Add(teams);
            panel1.Controls.Add(players);
            panel1.Location = new Point(0, -2);
            panel1.Name = "panel1";
            panel1.Size = new Size(876, 68);
            panel1.TabIndex = 3;
            // 
            // button3
            // 
            button3.Location = new Point(600, 0);
            button3.Name = "button3";
            button3.Size = new Size(150, 68);
            button3.TabIndex = 5;
            button3.Text = "赛程";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Location = new Point(469, 0);
            button4.Name = "button4";
            button4.Size = new Size(139, 68);
            button4.TabIndex = 6;
            button4.Text = "成绩排行";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // data_analysis
            // 
            data_analysis.Location = new Point(326, 0);
            data_analysis.Name = "data_analysis";
            data_analysis.Size = new Size(147, 68);
            data_analysis.TabIndex = 7;
            data_analysis.Text = "数据分析";
            data_analysis.UseVisualStyleBackColor = true;
            data_analysis.Click += data_analysis_Click;
            // 
            // teams
            // 
            teams.AutoSize = true;
            teams.Location = new Point(167, 0);
            teams.Name = "teams";
            teams.Size = new Size(165, 68);
            teams.TabIndex = 8;
            teams.Text = "球队";
            teams.UseVisualStyleBackColor = true;
            teams.Click += teams_Click;
            // 
            // players
            // 
            players.Location = new Point(12, 0);
            players.Name = "players";
            players.Size = new Size(160, 68);
            players.TabIndex = 4;
            players.Text = "球员";
            players.UseVisualStyleBackColor = true;
            players.Click += players_Click;
            // 
            // tabControl_main
            // 
            tabControl_main.Alignment = TabAlignment.Bottom;
            tabControl_main.Controls.Add(tabPage1);
            tabControl_main.Controls.Add(tabPage2);
            tabControl_main.Controls.Add(tabPage3);
            tabControl_main.Controls.Add(tabPage4);
            tabControl_main.Controls.Add(tabPage5);
            tabControl_main.Controls.Add(tabPage6);
            tabControl_main.ItemSize = new Size(0, 25);
            tabControl_main.Location = new Point(0, 77);
            tabControl_main.Multiline = true;
            tabControl_main.Name = "tabControl_main";
            tabControl_main.SelectedIndex = 0;
            tabControl_main.Size = new Size(1432, 697);
            tabControl_main.TabIndex = 4;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(checkBox1);
            tabPage1.Controls.Add(button2);
            tabPage1.Controls.Add(richTextBox2);
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(richTextBox1);
            tabPage1.Controls.Add(pictureBoxPlayer);
            tabPage1.Controls.Add(button1);
            tabPage1.Controls.Add(dataGridViewPlayers);
            tabPage1.Location = new Point(4, 4);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1424, 664);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "1";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(873, 43);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(109, 24);
            checkBox1.TabIndex = 8;
            checkBox1.Text = "checkBox1";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Location = new Point(1094, 213);
            button2.Name = "button2";
            button2.Size = new Size(132, 29);
            button2.TabIndex = 7;
            button2.Text = "数据分析";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // richTextBox2
            // 
            richTextBox2.Location = new Point(424, 266);
            richTextBox2.Name = "richTextBox2";
            richTextBox2.Size = new Size(856, 287);
            richTextBox2.TabIndex = 6;
            richTextBox2.Text = "";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("楷体", 15F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label1.Location = new Point(6, 16);
            label1.Name = "label1";
            label1.Size = new Size(120, 25);
            label1.TabIndex = 5;
            label1.Text = "球员列表";
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(575, 23);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(246, 216);
            richTextBox1.TabIndex = 4;
            richTextBox1.Text = "";
            // 
            // pictureBoxPlayer
            // 
            pictureBoxPlayer.Location = new Point(424, 21);
            pictureBoxPlayer.Name = "pictureBoxPlayer";
            pictureBoxPlayer.Size = new Size(145, 176);
            pictureBoxPlayer.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxPlayer.TabIndex = 3;
            pictureBoxPlayer.TabStop = false;
            // 
            // button1
            // 
            button1.Location = new Point(961, 213);
            button1.Name = "button1";
            button1.Size = new Size(118, 29);
            button1.TabIndex = 2;
            button1.Text = "显示详细信息";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // dataGridViewPlayers
            // 
            dataGridViewPlayers.AllowUserToOrderColumns = true;
            dataGridViewPlayers.BackgroundColor = Color.FromArgb(192, 255, 255);
            dataGridViewPlayers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewPlayers.Location = new Point(3, 49);
            dataGridViewPlayers.Name = "dataGridViewPlayers";
            dataGridViewPlayers.RowHeadersWidth = 51;
            dataGridViewPlayers.Size = new Size(376, 586);
            dataGridViewPlayers.TabIndex = 0;
            dataGridViewPlayers.CellContentClick += dataGridViewPlayers_CellContentClick;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 4);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1424, 664);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "2";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(chart1);
            tabPage3.Controls.Add(dataGridView_player_inf);
            tabPage3.Location = new Point(4, 4);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(1424, 664);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "3";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // chart1
            // 
            chartArea3.Name = "ChartArea1";
            chart1.ChartAreas.Add(chartArea3);
            legend3.Name = "Legend1";
            chart1.Legends.Add(legend3);
            chart1.Location = new Point(292, 52);
            chart1.Name = "chart1";
            series3.ChartArea = "ChartArea1";
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            chart1.Series.Add(series3);
            chart1.Size = new Size(663, 308);
            chart1.TabIndex = 3;
            chart1.Text = "chart1";
            chart1.Click += chart2_Click;
            // 
            // dataGridView_player_inf
            // 
            dataGridView_player_inf.AllowUserToOrderColumns = true;
            dataGridView_player_inf.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView_player_inf.Location = new Point(6, 404);
            dataGridView_player_inf.Name = "dataGridView_player_inf";
            dataGridView_player_inf.RowHeadersWidth = 51;
            dataGridView_player_inf.Size = new Size(993, 293);
            dataGridView_player_inf.TabIndex = 2;
            // 
            // tabPage4
            // 
            tabPage4.Location = new Point(4, 4);
            tabPage4.Name = "tabPage4";
            tabPage4.Size = new Size(1424, 664);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "4";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            tabPage5.Location = new Point(4, 4);
            tabPage5.Name = "tabPage5";
            tabPage5.Size = new Size(1424, 664);
            tabPage5.TabIndex = 4;
            tabPage5.Text = "5";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // tabPage6
            // 
            tabPage6.Controls.Add(send);
            tabPage6.Controls.Add(result_box);
            tabPage6.Controls.Add(question_box);
            tabPage6.Controls.Add(label2);
            tabPage6.Location = new Point(4, 4);
            tabPage6.Name = "tabPage6";
            tabPage6.Size = new Size(1424, 664);
            tabPage6.TabIndex = 5;
            tabPage6.Text = "6";
            tabPage6.UseVisualStyleBackColor = true;
            // 
            // send
            // 
            send.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Regular, GraphicsUnit.Point, 134);
            send.Location = new Point(844, 437);
            send.Name = "send";
            send.Size = new Size(94, 42);
            send.TabIndex = 3;
            send.Text = "发送";
            send.UseVisualStyleBackColor = true;
            send.Click += send_Click;
            // 
            // result_box
            // 
            result_box.Location = new Point(173, 125);
            result_box.Name = "result_box";
            result_box.Size = new Size(621, 282);
            result_box.TabIndex = 2;
            result_box.Text = "";
            result_box.TextChanged += result_box_TextChanged;
            // 
            // question_box
            // 
            question_box.Font = new Font("微软雅黑", 15F, FontStyle.Regular, GraphicsUnit.Point, 134);
            question_box.Location = new Point(173, 437);
            question_box.Name = "question_box";
            question_box.Size = new Size(621, 40);
            question_box.TabIndex = 1;
            question_box.TextChanged += question_box_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Microsoft YaHei UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label2.Location = new Point(18, 76);
            label2.Name = "label2";
            label2.Size = new Size(156, 37);
            label2.TabIndex = 0;
            label2.Text = "DeepSeek";
            // 
            // DS
            // 
            DS.Location = new Point(745, -2);
            DS.Name = "DS";
            DS.Size = new Size(131, 68);
            DS.TabIndex = 5;
            DS.Text = "DeepSeek";
            DS.UseVisualStyleBackColor = true;
            DS.Click += DS_Click;
            // 
            // frm_A
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(1430, 810);
            Controls.Add(DS);
            Controls.Add(panel1);
            Controls.Add(search_btn);
            Controls.Add(tabControl_main);
            Name = "frm_A";
            Text = "Form1";
            Load += frm_main_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            tabControl_main.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPlayer).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewPlayers).EndInit();
            tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)chart1).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView_player_inf).EndInit();
            tabPage6.ResumeLayout(false);
            tabPage6.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Button search_btn;
        private Panel panel1;
        private Button button3;
        private Button button4;
        private Button data_analysis;
        private Button teams;
        private Button players;
        private TabControl tabControl_main;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private TabPage tabPage4;
        private TabPage tabPage5;
        private DataGridView dataGridViewPlayers;
        private Button button1;
        private PictureBox pictureBoxPlayer;
        private RichTextBox richTextBox1;
        private Label label1;
        private TabPage tabPage6;
        private Button DS;
        private Label label2;
        private Button send;
        private RichTextBox result_box;
        private TextBox question_box;
        private DataGridView dataGridView_player_inf;
        private RichTextBox richTextBox2;
        private Button button2;
        private CheckBox checkBox1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
    }
}