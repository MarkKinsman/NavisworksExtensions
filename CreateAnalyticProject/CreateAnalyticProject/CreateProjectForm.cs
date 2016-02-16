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

using Newtonsoft.Json;

namespace CreateAnalyticProject
{
    public partial class CreateProjectForm : Form
    {
        public CreateProjectForm()
        {
            InitializeComponent();
        }

        private void bttn_addLevel_Click(object sender, EventArgs e)
        {
            if (txt_levelName.Text != "" && txt_levelHeight.Text != "")
            {
                ListViewItem item = new ListViewItem(new[] { txt_levelName.Text, txt_levelHeight.Text});
                lst_levels.Items.Add(item);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (txt_fileName.Text != "" && txt_fileResp.Text != "" && txt_fileOwner.Text != "")
            {
                ListViewItem item = new ListViewItem(new[] { txt_fileName.Text, txt_fileResp.Text, txt_fileOwner.Text });
                lst_files.Items.Add(item);
            }
        }

        private void bttn_create_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> levelsDict = new Dictionary<string, string>();
            List<Dictionary<string, string>> filesList = new List<Dictionary<string, string>>();

            foreach (ListViewItem levelItem in lst_levels.Items)
            {
                levelsDict.Add(levelItem.SubItems[0].Text, levelItem.SubItems[1].Text);
            }

            foreach (ListViewItem fileItem in lst_files.Items)
            {
                Dictionary<string, string> temp = new Dictionary<string, string>();
                temp.Add("name", txt_fileName.Text);
                temp.Add("responsibility", txt_fileResp.Text);
                temp.Add("owner", txt_fileOwner.Text);
                filesList.Add(temp);
            }

            UtilFile utilFile = new UtilFile();
            utilFile.levels = levelsDict;
            utilFile.file_specs = filesList;

 //           string output = JsonConvert.SerializeObject(utilFile);

//            MessageBox.Show(output);

            //string uriString = "http://1.hackday.xyz/xprojects/" + txt_project.Text;
            //WebClient myWebClient = new WebClient();
            //byte[] responseArray = myWebClient.UploadFile(uriString, txt_filePath.Text);
            //MessageBox.Show("Response: " + Encoding.ASCII.GetString(responseArray));
        }
    }
}
