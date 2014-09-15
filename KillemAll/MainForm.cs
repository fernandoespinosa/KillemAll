using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace KillemAll
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            this.Text = this.ProductName;
            this.StartPosition = FormStartPosition.Manual;
            this.Top = 0;
            this.Width = 800;
            this.Left = (Screen.GetWorkingArea(this).Width - this.Width) / 2;
            this.Height = Screen.GetWorkingArea(this).Height;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            RefreshData();
            this.dataGridView.Sort(this.dataGridView.Columns["ProcessName"], ListSortDirection.Ascending);
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void RefreshData()
        {
            this.dataGridView.DataSource = SortableBindingList(Process.GetProcesses().Select(p => new {
                p.Id,
                p.ProcessName,
                FileName = Eval.TryInvoke(() => p.MainModule.FileName)
            }).OrderBy(p => p.Id));
            this.dataGridView.AutoResizeColumns();
        }

        private void buttonKillemAll_Click(object sender, EventArgs e)
        {
            var row = this.dataGridView.CurrentRow;
            if (row != null)
            {
                dynamic item = row.DataBoundItem;
                string processName = item.ProcessName;
                var res = MessageBox.Show(string.Format("Kill all instances of '{0}'", processName), this.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                if (res != DialogResult.OK)
                    return;

                foreach (var process in Process.GetProcessesByName(processName))
                {
                    process.Kill();
                }
            }

            RefreshData();
        }

        public static SortableBindingList<T> SortableBindingList<T>(IEnumerable<T> sequence)
        {
            return new SortableBindingList<T>(sequence.ToList());
        }
    }
}
