namespace DEV.RFID
{
    partial class RFID_Alarm
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            splitContainerControl1 = new SplitContainer();
            groupControl1 = new GroupBox();
            gridControl1 = new DataGridView();
            gridColumn1 = new DataGridViewButtonColumn();
            gridColumn2 = new DataGridViewButtonColumn();
            gridColumn3 = new DataGridViewButtonColumn();
            gridColumn4 = new DataGridViewButtonColumn();
            groupControl2 = new GroupBox();
            gridControl2 = new DataGridView();
            panel1 = new Panel();
            simpleButton1 = new Button();
            simpleButton2 = new Button();
            btnExcel = new Button();
            timer1 = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).BeginInit();
            splitContainerControl1.Panel1.SuspendLayout();
            splitContainerControl1.Panel2.SuspendLayout();
            splitContainerControl1.SuspendLayout();
            groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridControl1).BeginInit();
            groupControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridControl2).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainerControl1
            // 
            splitContainerControl1.Dock = DockStyle.Bottom;
            splitContainerControl1.FixedPanel = FixedPanel.Panel1;
            splitContainerControl1.Location = new Point(0, 46);
            splitContainerControl1.Margin = new Padding(4, 3, 4, 3);
            splitContainerControl1.Name = "splitContainerControl1";
            // 
            // splitContainerControl1.Panel1
            // 
            splitContainerControl1.Panel1.Controls.Add(groupControl1);
            // 
            // splitContainerControl1.Panel2
            // 
            splitContainerControl1.Panel2.Controls.Add(groupControl2);
            splitContainerControl1.Size = new Size(1335, 576);
            splitContainerControl1.SplitterDistance = 444;
            splitContainerControl1.SplitterWidth = 5;
            splitContainerControl1.TabIndex = 2;
            // 
            // groupControl1
            // 
            groupControl1.Controls.Add(gridControl1);
            groupControl1.Dock = DockStyle.Fill;
            groupControl1.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            groupControl1.Location = new Point(0, 0);
            groupControl1.Margin = new Padding(4, 3, 4, 3);
            groupControl1.Name = "groupControl1";
            groupControl1.Padding = new Padding(4, 3, 4, 3);
            groupControl1.Size = new Size(444, 576);
            groupControl1.TabIndex = 0;
            groupControl1.TabStop = false;
            groupControl1.Text = "Real Time Data ( Refresh ~ 1 minute )";
            // 
            // gridControl1
            // 
            gridControl1.BackgroundColor = SystemColors.ButtonHighlight;
            gridControl1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridControl1.Columns.AddRange(new DataGridViewColumn[] { gridColumn1, gridColumn2, gridColumn3, gridColumn4 });
            gridControl1.Dock = DockStyle.Fill;
            gridControl1.Location = new Point(4, 22);
            gridControl1.Margin = new Padding(4, 3, 4, 3);
            gridControl1.Name = "gridControl1";
            gridControl1.Size = new Size(436, 551);
            gridControl1.TabIndex = 1;
            gridControl1.CellClick += gridControl1_CellClick;
            gridControl1.RowPrePaint += gridControl1_RowPrePaint;
            // 
            // gridColumn1
            // 
            gridColumn1.DataPropertyName = "RFID_CODE";
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            gridColumn1.DefaultCellStyle = dataGridViewCellStyle1;
            gridColumn1.HeaderText = "RFID Code";
            gridColumn1.Name = "gridColumn1";
            gridColumn1.Text = "RFID Code";
            // 
            // gridColumn2
            // 
            gridColumn2.DataPropertyName = "GATE_NAME";
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            gridColumn2.DefaultCellStyle = dataGridViewCellStyle2;
            gridColumn2.HeaderText = "Gate name";
            gridColumn2.Name = "gridColumn2";
            // 
            // gridColumn3
            // 
            gridColumn3.DataPropertyName = "MODEL_NAME";
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            gridColumn3.DefaultCellStyle = dataGridViewCellStyle3;
            gridColumn3.HeaderText = "Model Name";
            gridColumn3.Name = "gridColumn3";
            // 
            // gridColumn4
            // 
            gridColumn4.DataPropertyName = "C_TIME";
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            gridColumn4.DefaultCellStyle = dataGridViewCellStyle4;
            gridColumn4.HeaderText = "Time";
            gridColumn4.Name = "gridColumn4";
            // 
            // groupControl2
            // 
            groupControl2.Controls.Add(gridControl2);
            groupControl2.Dock = DockStyle.Fill;
            groupControl2.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            groupControl2.Location = new Point(0, 0);
            groupControl2.Margin = new Padding(4, 3, 4, 3);
            groupControl2.Name = "groupControl2";
            groupControl2.Padding = new Padding(4, 3, 4, 3);
            groupControl2.Size = new Size(886, 576);
            groupControl2.TabIndex = 0;
            groupControl2.TabStop = false;
            groupControl2.Text = "Daily Alarm By Gate";
            // 
            // gridControl2
            // 
            gridControl2.BackgroundColor = SystemColors.ButtonHighlight;
            gridControl2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridControl2.Dock = DockStyle.Fill;
            gridControl2.Location = new Point(4, 22);
            gridControl2.Margin = new Padding(4, 3, 4, 3);
            gridControl2.Name = "gridControl2";
            gridControl2.Size = new Size(878, 551);
            gridControl2.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Controls.Add(simpleButton1);
            panel1.Controls.Add(simpleButton2);
            panel1.Controls.Add(btnExcel);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(1335, 46);
            panel1.TabIndex = 3;
            // 
            // simpleButton1
            // 
            simpleButton1.Location = new Point(1120, 3);
            simpleButton1.Margin = new Padding(4, 3, 4, 3);
            simpleButton1.Name = "simpleButton1";
            simpleButton1.Size = new Size(97, 36);
            simpleButton1.TabIndex = 3;
            simpleButton1.Text = "Alarm On";
            simpleButton1.UseVisualStyleBackColor = true;
            simpleButton1.Click += simpleButton1_Click;
            // 
            // simpleButton2
            // 
            simpleButton2.Location = new Point(1224, 3);
            simpleButton2.Margin = new Padding(4, 3, 4, 3);
            simpleButton2.Name = "simpleButton2";
            simpleButton2.Size = new Size(97, 36);
            simpleButton2.TabIndex = 2;
            simpleButton2.Text = "Alarm Off";
            simpleButton2.UseVisualStyleBackColor = true;
            simpleButton2.Click += simpleButton2_Click;
            // 
            // btnExcel
            // 
            btnExcel.Location = new Point(21, 7);
            btnExcel.Margin = new Padding(4, 3, 4, 3);
            btnExcel.Name = "btnExcel";
            btnExcel.Size = new Size(97, 36);
            btnExcel.TabIndex = 1;
            btnExcel.Text = "Excel";
            btnExcel.UseVisualStyleBackColor = true;
            btnExcel.Click += btnExcel_Click;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            // 
            // RFID_Alarm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panel1);
            Controls.Add(splitContainerControl1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "RFID_Alarm";
            Size = new Size(1335, 622);
            Load += RFID_Alarm_Load;
            splitContainerControl1.Panel1.ResumeLayout(false);
            splitContainerControl1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).EndInit();
            splitContainerControl1.ResumeLayout(false);
            groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridControl1).EndInit();
            groupControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridControl2).EndInit();
            panel1.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerControl1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button simpleButton1;
        private System.Windows.Forms.Button simpleButton2;
        private System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.GroupBox groupControl1;
        private System.Windows.Forms.DataGridView gridControl1;
        private System.Windows.Forms.DataGridViewButtonColumn gridColumn1;
        private System.Windows.Forms.DataGridViewButtonColumn gridColumn2;
        private System.Windows.Forms.DataGridViewButtonColumn gridColumn3;
        private System.Windows.Forms.DataGridViewButtonColumn gridColumn4;
        private System.Windows.Forms.GroupBox groupControl2;
        private System.Windows.Forms.DataGridView gridControl2;
    }
}
