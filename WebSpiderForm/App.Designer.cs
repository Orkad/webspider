namespace WebSpiderForm
{
    partial class App
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.textBoxStartUri = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxFilter1 = new System.Windows.Forms.TextBox();
            this.textBoxFilter2 = new System.Windows.Forms.TextBox();
            this.checkBoxGet = new System.Windows.Forms.CheckBox();
            this.textBoxGet1 = new System.Windows.Forms.TextBox();
            this.textBoxGet2 = new System.Windows.Forms.TextBox();
            this.textBoxGet3 = new System.Windows.Forms.TextBox();
            this.textBoxGet4 = new System.Windows.Forms.TextBox();
            this.textBoxGet5 = new System.Windows.Forms.TextBox();
            this.labelLog = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonStop
            // 
            this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStop.Enabled = false;
            this.buttonStop.Location = new System.Drawing.Point(343, 154);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(75, 23);
            this.buttonStop.TabIndex = 0;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            // 
            // buttonStart
            // 
            this.buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStart.Location = new System.Drawing.Point(424, 154);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 1;
            this.buttonStart.Text = "Lancer";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // textBoxStartUri
            // 
            this.textBoxStartUri.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxStartUri.Location = new System.Drawing.Point(12, 25);
            this.textBoxStartUri.Name = "textBoxStartUri";
            this.textBoxStartUri.Size = new System.Drawing.Size(487, 20);
            this.textBoxStartUri.TabIndex = 2;
            this.textBoxStartUri.TextChanged += new System.EventHandler(this.textBoxStartUri_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Adresse de départ";
            // 
            // textBoxFilter1
            // 
            this.textBoxFilter1.Location = new System.Drawing.Point(91, 51);
            this.textBoxFilter1.Name = "textBoxFilter1";
            this.textBoxFilter1.Size = new System.Drawing.Size(408, 20);
            this.textBoxFilter1.TabIndex = 4;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(12, 54);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(73, 13);
            label2.TabIndex = 5;
            label2.Text = "Premier Filtre :";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(12, 80);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(75, 13);
            label3.TabIndex = 7;
            label3.Text = "Second Filtre :";
            // 
            // textBoxFilter2
            // 
            this.textBoxFilter2.Location = new System.Drawing.Point(91, 77);
            this.textBoxFilter2.Name = "textBoxFilter2";
            this.textBoxFilter2.Size = new System.Drawing.Size(408, 20);
            this.textBoxFilter2.TabIndex = 6;
            // 
            // checkBoxGet
            // 
            this.checkBoxGet.AutoSize = true;
            this.checkBoxGet.Location = new System.Drawing.Point(15, 103);
            this.checkBoxGet.Name = "checkBoxGet";
            this.checkBoxGet.Size = new System.Drawing.Size(43, 17);
            this.checkBoxGet.TabIndex = 8;
            this.checkBoxGet.Text = "Get";
            this.checkBoxGet.UseVisualStyleBackColor = true;
            this.checkBoxGet.CheckedChanged += new System.EventHandler(this.checkBoxGet_CheckedChanged);
            // 
            // textBoxGet1
            // 
            this.textBoxGet1.Enabled = false;
            this.textBoxGet1.Location = new System.Drawing.Point(91, 103);
            this.textBoxGet1.Name = "textBoxGet1";
            this.textBoxGet1.Size = new System.Drawing.Size(76, 20);
            this.textBoxGet1.TabIndex = 9;
            // 
            // textBoxGet2
            // 
            this.textBoxGet2.Enabled = false;
            this.textBoxGet2.Location = new System.Drawing.Point(173, 103);
            this.textBoxGet2.Name = "textBoxGet2";
            this.textBoxGet2.Size = new System.Drawing.Size(76, 20);
            this.textBoxGet2.TabIndex = 10;
            // 
            // textBoxGet3
            // 
            this.textBoxGet3.Enabled = false;
            this.textBoxGet3.Location = new System.Drawing.Point(255, 103);
            this.textBoxGet3.Name = "textBoxGet3";
            this.textBoxGet3.Size = new System.Drawing.Size(76, 20);
            this.textBoxGet3.TabIndex = 11;
            // 
            // textBoxGet4
            // 
            this.textBoxGet4.Enabled = false;
            this.textBoxGet4.Location = new System.Drawing.Point(337, 103);
            this.textBoxGet4.Name = "textBoxGet4";
            this.textBoxGet4.Size = new System.Drawing.Size(76, 20);
            this.textBoxGet4.TabIndex = 12;
            // 
            // textBoxGet5
            // 
            this.textBoxGet5.Enabled = false;
            this.textBoxGet5.Location = new System.Drawing.Point(419, 103);
            this.textBoxGet5.Name = "textBoxGet5";
            this.textBoxGet5.Size = new System.Drawing.Size(80, 20);
            this.textBoxGet5.TabIndex = 13;
            // 
            // labelLog
            // 
            this.labelLog.AutoSize = true;
            this.labelLog.ForeColor = System.Drawing.Color.Red;
            this.labelLog.Location = new System.Drawing.Point(12, 131);
            this.labelLog.Name = "labelLog";
            this.labelLog.Size = new System.Drawing.Size(47, 13);
            this.labelLog.TabIndex = 14;
            this.labelLog.Text = "Stopped";
            // 
            // App
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(511, 189);
            this.Controls.Add(this.labelLog);
            this.Controls.Add(this.textBoxGet5);
            this.Controls.Add(this.textBoxGet4);
            this.Controls.Add(this.textBoxGet3);
            this.Controls.Add(this.textBoxGet2);
            this.Controls.Add(this.textBoxGet1);
            this.Controls.Add(this.checkBoxGet);
            this.Controls.Add(label3);
            this.Controls.Add(this.textBoxFilter2);
            this.Controls.Add(label2);
            this.Controls.Add(this.textBoxFilter1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxStartUri);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.buttonStop);
            this.Name = "App";
            this.Text = "Web Spider";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.TextBox textBoxStartUri;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxFilter1;
        private System.Windows.Forms.TextBox textBoxFilter2;
        private System.Windows.Forms.CheckBox checkBoxGet;
        private System.Windows.Forms.TextBox textBoxGet1;
        private System.Windows.Forms.TextBox textBoxGet2;
        private System.Windows.Forms.TextBox textBoxGet3;
        private System.Windows.Forms.TextBox textBoxGet4;
        private System.Windows.Forms.TextBox textBoxGet5;
        private System.Windows.Forms.Label labelLog;
    }
}

