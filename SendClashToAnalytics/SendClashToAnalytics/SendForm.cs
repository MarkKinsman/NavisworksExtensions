using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace SendClashToAnalytics
{
    public partial class SendForm : Form
    {
        public SendForm()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void bttn_browse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            {
               txt_filePath.Text = openFile.FileName;
            }
        }

        private void bttn_send_Click(object sender, EventArgs e)
        {
            string uriString = "http://1.hackday.xyz/xml/"+cmb_project.Text;
            WebClient myWebClient = new WebClient();
            byte[] responseArray = myWebClient.UploadFile(uriString, txt_filePath.Text);
            MessageBox.Show("Response: " + Encoding.ASCII.GetString(responseArray));
        }

        private void SendForm_Load(object sender, EventArgs e)
        {
            string uriString = "http://1.hackday.xyz/projects";
            WebRequest myWebClient = WebRequest.Create(uriString);
            Stream myStream = myWebClient.GetResponse().GetResponseStream();
            StreamReader myReader = new StreamReader(myStream);
            while(myReader.Peek() >= 0)
            {
                cmb_project.Items.Add(myReader.ReadLine());
            }
            myStream.Close();
        }
    }
}
