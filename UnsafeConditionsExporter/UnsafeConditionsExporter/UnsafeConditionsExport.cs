using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace UnsafeConditionsExporter
{
    public partial class frm_Export : Form
    {
        private DataSet ds = new DataSet();
        private DataTable dt = new DataTable();
        public frm_Export()
        {
            InitializeComponent();
        }

        private void frm_Export_Load(object sender, EventArgs e)
        {
            try
            {
                // PostgeSQL-style connection string
                string connstring = String.Format("Host=bim360field.ccfdrqcswrzr.us-west-2.rds.amazonaws.com;Database=myfield;Username=readonly;Password=password;Timeout=30");
                // Making connection with Npgsql provider
                NpgsqlConnection conn = new NpgsqlConnection(connstring);
                conn.Open();
                // quite complex sql statement
                string sql = "SELECT * FROM projects";
                // data adapter making request from our connection
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);
                // i always reset DataSet before i do
                // something with it.... i don't know why :-)
                ds.Reset();
                // filling DataSet with result from NpgsqlDataAdapter
                da.Fill(ds);
                // since it C# DataSet can handle multiple tables, we will select first
                dt = ds.Tables[0];
                // connect grid to DataTable
                dataGridView1.DataSource = dt;
                // since we only showing the result we don't need connection anymore
                conn.Close();
            }
            catch (Exception msg)
            {
                MessageBox.Show(msg.ToString());
                throw;
            }
        }
    }
}
