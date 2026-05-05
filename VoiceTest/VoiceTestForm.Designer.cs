using System.Drawing;
using System.Windows.Forms;

namespace VoiceTest
{
    partial class VoiceTestForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            tableLayoutMain   = new TableLayoutPanel();
            textBoxVoiceTest  = new TextBox();
            panelButtons      = new Panel();
            buttonVoiceTest   = new Button();
            buttonAutoLoop    = new Button();
            buttonRunAutoTest = new Button();

            tableLayoutMain.SuspendLayout();
            panelButtons.SuspendLayout();
            SuspendLayout();

            // ── tableLayoutMain ──────────────────────────────────────────
            tableLayoutMain.ColumnCount = 2;
            tableLayoutMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 82F));
            tableLayoutMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18F));
            tableLayoutMain.Controls.Add(textBoxVoiceTest, 0, 0);
            tableLayoutMain.Controls.Add(panelButtons, 1, 0);
            tableLayoutMain.Dock      = DockStyle.Fill;
            tableLayoutMain.Location  = new Point(0, 0);
            tableLayoutMain.Name      = "tableLayoutMain";
            tableLayoutMain.RowCount  = 1;
            tableLayoutMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutMain.Size      = new Size(900, 550);
            tableLayoutMain.TabIndex  = 0;

            // ── textBoxVoiceTest ─────────────────────────────────────────
            textBoxVoiceTest.Dock       = DockStyle.Fill;
            textBoxVoiceTest.Location   = new Point(3, 4);
            textBoxVoiceTest.Margin     = new Padding(3, 4, 3, 4);
            textBoxVoiceTest.Multiline  = true;
            textBoxVoiceTest.Name       = "textBoxVoiceTest";
            textBoxVoiceTest.ScrollBars = ScrollBars.Vertical;
            textBoxVoiceTest.Font       = new Font("Consolas", 9F);
            textBoxVoiceTest.Size       = new Size(731, 542);
            textBoxVoiceTest.TabIndex   = 0;

            // ── panelButtons ─────────────────────────────────────────────
            panelButtons.Dock     = DockStyle.Fill;
            panelButtons.Padding  = new Padding(4);
            panelButtons.Name     = "panelButtons";
            panelButtons.Controls.Add(buttonRunAutoTest);
            panelButtons.Controls.Add(buttonAutoLoop);
            panelButtons.Controls.Add(buttonVoiceTest);

            // ── buttonVoiceTest (Push-to-Talk thủ công) ──────────────────
            buttonVoiceTest.Text          = "🎤";
            buttonVoiceTest.BackColor     = Color.White;
            buttonVoiceTest.Dock          = DockStyle.Top;
            buttonVoiceTest.FlatStyle     = FlatStyle.Flat;
            buttonVoiceTest.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
            buttonVoiceTest.Font          = new Font("Segoe UI Emoji", 26F, FontStyle.Bold);
            buttonVoiceTest.Name          = "buttonVoiceTest";
            buttonVoiceTest.Size          = new Size(155, 170);
            buttonVoiceTest.TabIndex      = 1;
            buttonVoiceTest.Enabled       = false;
            buttonVoiceTest.UseVisualStyleBackColor = false;

            // ── buttonAutoLoop (Thu âm 5s / Nghỉ 5s) ────────────────────
            buttonAutoLoop.Text          = "▶ AUTO LOOP";
            buttonAutoLoop.BackColor     = Color.LightGreen;
            buttonAutoLoop.Dock          = DockStyle.Top;
            buttonAutoLoop.FlatStyle     = FlatStyle.Flat;
            buttonAutoLoop.FlatAppearance.BorderColor = Color.Green;
            buttonAutoLoop.Font          = new Font("Segoe UI", 11F, FontStyle.Bold);
            buttonAutoLoop.Name          = "buttonAutoLoop";
            buttonAutoLoop.Size          = new Size(155, 60);
            buttonAutoLoop.Margin        = new Padding(0, 6, 0, 0);
            buttonAutoLoop.TabIndex      = 2;
            buttonAutoLoop.Enabled       = false;
            buttonAutoLoop.UseVisualStyleBackColor = false;

            // ── buttonRunAutoTest (TTS → Whisper → Report) ───────────────
            buttonRunAutoTest.Text          = "🧪 AUTO TEST";
            buttonRunAutoTest.BackColor     = Color.FromArgb(0, 120, 215);
            buttonRunAutoTest.ForeColor     = Color.White;
            buttonRunAutoTest.Dock          = DockStyle.Top;
            buttonRunAutoTest.FlatStyle     = FlatStyle.Flat;
            buttonRunAutoTest.FlatAppearance.BorderColor = Color.DarkBlue;
            buttonRunAutoTest.Font          = new Font("Segoe UI", 11F, FontStyle.Bold);
            buttonRunAutoTest.Name          = "buttonRunAutoTest";
            buttonRunAutoTest.Size          = new Size(155, 60);
            buttonRunAutoTest.Margin        = new Padding(0, 6, 0, 0);
            buttonRunAutoTest.TabIndex      = 3;
            buttonRunAutoTest.Enabled       = false;
            buttonRunAutoTest.UseVisualStyleBackColor = false;

            // ── VoiceTestForm ─────────────────────────────────────────────
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode       = AutoScaleMode.Font;
            ClientSize          = new Size(900, 550);
            Controls.Add(tableLayoutMain);
            Name = "VoiceTestForm";
            Text = "Voice Auto Test — NETTMC2026";

            panelButtons.ResumeLayout(false);
            tableLayoutMain.ResumeLayout(false);
            ResumeLayout(false);
        }
        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutMain;
        private System.Windows.Forms.TextBox          textBoxVoiceTest;
        private System.Windows.Forms.Panel            panelButtons;
        private System.Windows.Forms.Button           buttonVoiceTest;
        private System.Windows.Forms.Button           buttonAutoLoop;
        private System.Windows.Forms.Button           buttonRunAutoTest;
    }
}