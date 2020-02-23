using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TriadPad.Forms
    {
    /// <summary>
    /// ������ �������� �� ����� ������
    /// </summary>
    internal partial class FormGoTo : Form
        {
        /// <summary>
        /// �����������
        /// </summary>
        protected FormGoTo()
            {
            InitializeComponent();
            }


        /// <summary>
        /// ��������� ������
        /// </summary>
        public static FormGoTo Instance
            {
            get
                {
                if ( instance == null )
                    instance = new FormGoTo();
                return instance;
                }
            }


        /// <summary>
        /// ������� �� ����� ����� � ���� ��������������
        /// </summary>
        /// <param name="rtb">���� ��������������</param>
        public void Go( RichTextBoxEx rtb )
            {
            this.editRtb = rtb;
            int lineNumberMax = rtb.Lines.Length - 1;
            this.lTextLineRange.Text = String.Format( "����� ������ ( 0 - {0} )", lineNumberMax );
            this.nudLineNumber.Maximum = lineNumberMax;
            this.nudLineNumber.Value = rtb.SelectedFirstLineNumber;

            this.nudLineNumber.Focus();
            this.ShowDialog();
            rtb.Focus();
            }


        //������ - �������
        private void btGo_Click( object sender, EventArgs e )
            {
            this.editRtb.Scroll( (int)this.nudLineNumber.Value );
            this.Close();
            }


        //������ - ������
        private void buttonCancel_Click( object sender, EventArgs e )
            {
            this.Close();
            }


        /// <summary>
        /// ��������� ������
        /// </summary>
        private static FormGoTo instance = null;
        /// <summary>
        /// ���� ��������������
        /// </summary>
        private RichTextBoxEx editRtb = null;

        }
    }