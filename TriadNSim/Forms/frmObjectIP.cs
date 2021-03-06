using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using DrawingPanel;
using TriadCompiler;

namespace TriadNSim.Forms
{
    public partial class frmObjectIP : Form
    {
        NetworkObject _Obj;
        public ConnectedIP Result;

        public frmObjectIP(NetworkObject obj)
        {
            InitializeComponent();
            _Obj = obj;
        }

        private void frmObjectIP_Load(object sender, EventArgs e)
        {
            cmbIPType.SelectedIndex = 0;
        }

        public void SelectIP(ConnectedIP ip)
        {
            cmbIPType.SelectedIndex = ip.IP.IsStandart ? 0 : 1;
            cmbIP.SelectedItem = ip.IP.Name;
            textBox1.Text = ip.Description;

            int iIndex = 0;
            foreach(string param in ip.Params)
            {
                dataGridViewParams.Rows[iIndex].Cells[2].Value = param;
                iIndex++;
            }
        }

        private void cmbIPType_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbIP.Items.Clear();
            dataGridViewParams.Rows.Clear();
            if (cmbIPType.SelectedIndex == 0)
            {
                foreach (var ip in frmMain.Instance.standartIProcedures)
                    cmbIP.Items.Add(ip.Name);
            }
            else
            {
                foreach (var ip in frmMain.Instance.userIProcedures)
                    cmbIP.Items.Add(ip.Name);
            }
            if (cmbIP.Items.Count > 0)
                cmbIP.SelectedIndex = 0;
        }

        private InfProcedure GetSelectedIP()
        {
            if (cmbIP.SelectedIndex < 0)
                return null;

            return cmbIPType.SelectedIndex == 0 ? frmMain.Instance.standartIProcedures[cmbIP.SelectedIndex] :
                                                    frmMain.Instance.userIProcedures[cmbIP.SelectedIndex];
        }

        private void cmbIP_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridViewParams.Rows.Clear();

            InfProcedure ip = GetSelectedIP();
            if (ip == null)
                return;

            foreach (IPParam param in ip.Params)
            {
                int iIndex = dataGridViewParams.Rows.Add();
                dataGridViewParams.Rows[iIndex].Cells[0].Value = (param.Name == null || param.Name.Length == 0) ? 
                                                                    "Param" + iIndex.ToString() : param.Name;
                dataGridViewParams.Rows[iIndex].Cells[1].Value = param.TypeName;
                DataGridViewComboBoxCell cell = (dataGridViewParams.Rows[iIndex].Cells[2] as DataGridViewComboBoxCell);
                if (param.IsEvent)
                {
                    foreach (string ev in _Obj.Routine.Events)
                        cell.Items.Add(ev);
                }
                else if (param.IsPolus)
                {
                    foreach (Polus polus in _Obj.Routine.Poluses)
                        cell.Items.Add(polus.Name);
                }
                else
                {
                    foreach (IExprType var in _Obj.Routine.Variables)
                    {
                        if (var.Code == param.Code)
                            cell.Items.Add(var.Name);
                    }
                }
                if (cell.Items.Count > 0)
                    cell.Value = cell.Items[0] as string;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Result = new ConnectedIP(GetSelectedIP());
            Result.Params = new List<string>();
            Result.Description = textBox1.Text;
            foreach (DataGridViewRow row in dataGridViewParams.Rows)
                Result.Params.Add(row.Cells[2].Value.ToString());
        }

        private void dataGridViewParams_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            bool bEnabled = dataGridViewParams.Rows.Count != 0;
            foreach (DataGridViewRow row in dataGridViewParams.Rows)
            {
                if (row.Cells[2].Value == null || row.Cells[2].Value.ToString().Length == 0)
                {
                    bEnabled = false;
                    break;
                }
            }
            btnOK.Enabled = bEnabled;
        }
    }
}
