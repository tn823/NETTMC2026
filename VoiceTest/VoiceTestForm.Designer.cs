using System.Drawing;
using System.Windows.Forms;

namespace VoiceTest
{
    partial class VoiceTestForm
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
            tableLayoutPanel1 = new TableLayoutPanel();
            textBoxVoiceTest = new TextBox();
            buttonVoiceTest = new Button();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 85F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            tableLayoutPanel1.Controls.Add(textBoxVoiceTest, 0, 0);
            tableLayoutPanel1.Controls.Add(buttonVoiceTest, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(800, 450);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // textBoxVoiceTest
            // 
            textBoxVoiceTest.Dock = DockStyle.Fill;
            textBoxVoiceTest.Location = new Point(3, 4);
            textBoxVoiceTest.Margin = new Padding(3, 4, 3, 4);
            textBoxVoiceTest.Multiline = true;
            textBoxVoiceTest.Name = "textBoxVoiceTest";
            textBoxVoiceTest.ScrollBars = ScrollBars.Vertical;
            textBoxVoiceTest.Size = new Size(674, 442);
            textBoxVoiceTest.TabIndex = 0;
            // 
            // buttonVoiceTest
            // 
            buttonVoiceTest.BackColor = Color.White;
            buttonVoiceTest.Dock = DockStyle.Right;
            buttonVoiceTest.FlatAppearance.BorderColor = Color.FromArgb(0, 0, 192);
            buttonVoiceTest.FlatStyle = FlatStyle.Flat;
            buttonVoiceTest.Font = new Font("Arial", 26.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            buttonVoiceTest.Image = Properties.Resources.voice_whisper;
            buttonVoiceTest.Location = new Point(683, 4);
            buttonVoiceTest.Margin = new Padding(3, 4, 3, 4);
            buttonVoiceTest.Name = "buttonVoiceTest";
            buttonVoiceTest.Size = new Size(114, 442);
            buttonVoiceTest.TabIndex = 7;
            buttonVoiceTest.TextImageRelation = TextImageRelation.ImageAboveText;
            buttonVoiceTest.UseVisualStyleBackColor = false;
            // 
            // VoiceTestForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tableLayoutPanel1);
            Name = "VoiceTestForm";
            Text = "VoiceTestForm";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox textBoxVoiceTest;
        private System.Windows.Forms.Button buttonVoiceTest;
    }
}