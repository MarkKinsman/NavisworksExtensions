namespace UnsafeConditionsExporter
{
    partial class frm_Export
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_Export));
            this.cmb_project = new System.Windows.Forms.ComboBox();
            this.lbl_project = new System.Windows.Forms.Label();
            this.lbl_from = new System.Windows.Forms.Label();
            this.dtp_from = new System.Windows.Forms.DateTimePicker();
            this.lbl_to = new System.Windows.Forms.Label();
            this.dtp_to = new System.Windows.Forms.DateTimePicker();
            this.bttn_save = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // cmb_project
            // 
            this.cmb_project.FormattingEnabled = true;
            this.cmb_project.Location = new System.Drawing.Point(61, 6);
            this.cmb_project.Name = "cmb_project";
            this.cmb_project.Size = new System.Drawing.Size(435, 21);
            this.cmb_project.TabIndex = 1;
            // 
            // lbl_project
            // 
            this.lbl_project.AutoSize = true;
            this.lbl_project.Location = new System.Drawing.Point(12, 9);
            this.lbl_project.Name = "lbl_project";
            this.lbl_project.Size = new System.Drawing.Size(43, 13);
            this.lbl_project.TabIndex = 0;
            this.lbl_project.Text = "Project:";
            // 
            // lbl_from
            // 
            this.lbl_from.AutoSize = true;
            this.lbl_from.Location = new System.Drawing.Point(12, 42);
            this.lbl_from.Name = "lbl_from";
            this.lbl_from.Size = new System.Drawing.Size(33, 13);
            this.lbl_from.TabIndex = 0;
            this.lbl_from.Text = "From:";
            // 
            // dtp_from
            // 
            this.dtp_from.Location = new System.Drawing.Point(61, 38);
            this.dtp_from.Name = "dtp_from";
            this.dtp_from.Size = new System.Drawing.Size(200, 20);
            this.dtp_from.TabIndex = 2;
            // 
            // lbl_to
            // 
            this.lbl_to.AutoSize = true;
            this.lbl_to.Location = new System.Drawing.Point(267, 42);
            this.lbl_to.Name = "lbl_to";
            this.lbl_to.Size = new System.Drawing.Size(23, 13);
            this.lbl_to.TabIndex = 0;
            this.lbl_to.Text = "To:";
            // 
            // dtp_to
            // 
            this.dtp_to.Location = new System.Drawing.Point(296, 38);
            this.dtp_to.Name = "dtp_to";
            this.dtp_to.Size = new System.Drawing.Size(200, 20);
            this.dtp_to.TabIndex = 3;
            // 
            // bttn_save
            // 
            this.bttn_save.Location = new System.Drawing.Point(100, 64);
            this.bttn_save.Name = "bttn_save";
            this.bttn_save.Size = new System.Drawing.Size(303, 40);
            this.bttn_save.TabIndex = 4;
            this.bttn_save.Text = "Save to XML";
            this.bttn_save.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(15, 110);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(477, 518);
            this.dataGridView1.TabIndex = 5;
            // 
            // frm_Export
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 640);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.bttn_save);
            this.Controls.Add(this.dtp_to);
            this.Controls.Add(this.lbl_to);
            this.Controls.Add(this.dtp_from);
            this.Controls.Add(this.lbl_from);
            this.Controls.Add(this.lbl_project);
            this.Controls.Add(this.cmb_project);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frm_Export";
            this.Text = "BIM 360 Unsafe Conditions v0.1";
            this.Load += new System.EventHandler(this.frm_Export_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmb_project;
        private System.Windows.Forms.Label lbl_project;
        private System.Windows.Forms.Label lbl_from;
        private System.Windows.Forms.DateTimePicker dtp_from;
        private System.Windows.Forms.Label lbl_to;
        private System.Windows.Forms.DateTimePicker dtp_to;
        private System.Windows.Forms.Button bttn_save;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}

