namespace SendClashToAnalytics
{
    partial class SendForm
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
            this.cmb_project = new System.Windows.Forms.ComboBox();
            this.lbl_Project = new System.Windows.Forms.Label();
            this.lbl_xmlLocation = new System.Windows.Forms.Label();
            this.bttn_browse = new System.Windows.Forms.Button();
            this.txt_filePath = new System.Windows.Forms.TextBox();
            this.bttn_send = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmb_project
            // 
            this.cmb_project.FormattingEnabled = true;
            this.cmb_project.Location = new System.Drawing.Point(139, 22);
            this.cmb_project.Name = "cmb_project";
            this.cmb_project.Size = new System.Drawing.Size(369, 21);
            this.cmb_project.TabIndex = 0;
            // 
            // lbl_Project
            // 
            this.lbl_Project.AutoSize = true;
            this.lbl_Project.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Project.Location = new System.Drawing.Point(12, 23);
            this.lbl_Project.Name = "lbl_Project";
            this.lbl_Project.Size = new System.Drawing.Size(121, 20);
            this.lbl_Project.TabIndex = 1;
            this.lbl_Project.Text = "Project Name:";
            this.lbl_Project.Click += new System.EventHandler(this.label1_Click);
            // 
            // lbl_xmlLocation
            // 
            this.lbl_xmlLocation.AutoSize = true;
            this.lbl_xmlLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_xmlLocation.Location = new System.Drawing.Point(49, 74);
            this.lbl_xmlLocation.Name = "lbl_xmlLocation";
            this.lbl_xmlLocation.Size = new System.Drawing.Size(84, 20);
            this.lbl_xmlLocation.TabIndex = 2;
            this.lbl_xmlLocation.Text = "XML File:";
            // 
            // bttn_browse
            // 
            this.bttn_browse.Location = new System.Drawing.Point(483, 73);
            this.bttn_browse.Name = "bttn_browse";
            this.bttn_browse.Size = new System.Drawing.Size(25, 24);
            this.bttn_browse.TabIndex = 3;
            this.bttn_browse.Text = "...";
            this.bttn_browse.UseVisualStyleBackColor = true;
            this.bttn_browse.Click += new System.EventHandler(this.bttn_browse_Click);
            // 
            // txt_filePath
            // 
            this.txt_filePath.Location = new System.Drawing.Point(139, 76);
            this.txt_filePath.Name = "txt_filePath";
            this.txt_filePath.Size = new System.Drawing.Size(338, 20);
            this.txt_filePath.TabIndex = 4;
            // 
            // bttn_send
            // 
            this.bttn_send.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bttn_send.Location = new System.Drawing.Point(139, 127);
            this.bttn_send.Name = "bttn_send";
            this.bttn_send.Size = new System.Drawing.Size(238, 35);
            this.bttn_send.TabIndex = 5;
            this.bttn_send.Text = "Send";
            this.bttn_send.UseVisualStyleBackColor = true;
            this.bttn_send.Click += new System.EventHandler(this.bttn_send_Click);
            // 
            // SendForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 174);
            this.Controls.Add(this.bttn_send);
            this.Controls.Add(this.txt_filePath);
            this.Controls.Add(this.bttn_browse);
            this.Controls.Add(this.lbl_xmlLocation);
            this.Controls.Add(this.lbl_Project);
            this.Controls.Add(this.cmb_project);
            this.Name = "SendForm";
            this.Text = "Send .XML to Analytics";
            this.Load += new System.EventHandler(this.SendForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmb_project;
        private System.Windows.Forms.Label lbl_Project;
        private System.Windows.Forms.Label lbl_xmlLocation;
        private System.Windows.Forms.Button bttn_browse;
        private System.Windows.Forms.TextBox txt_filePath;
        private System.Windows.Forms.Button bttn_send;
    }
}