using System;
using System.Collections.Generic;
using System.Text;

namespace TriadCompiler
    {     
    /// <summary>
    /// ��� ����������� �������.
    /// </summary>
    public class PolusType : IPolusType
        {
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="isInput">������� ����, ��� ��� ������ Input</param>
        /// <param name="isOutput">������� ����, ��� ��� ������ Output</param>
        public PolusType( bool isInput, bool isOutput )
            {
            this.isInput = isInput;
            this.isOutput = isOutput;
            }


        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="isInput">������� ����, ��� ��� ������ Input</param>
        /// <param name="isOutput">������� ����, ��� ��� ������ Output</param>
        /// <param name="isSpyObject">������� spy-�������</param>
        public PolusType( bool isInput, bool isOutput, bool isSpyObject )
            : this( isInput, isOutput )
            {
            this.isSpyObject = isSpyObject;
            }


        /// <summary>
        /// �������� ����, ��� ��� ������ - Input
        /// </summary>
        public bool IsInput
            {
            get
                {
                return isInput;
                }
            }


        /// <summary>
        /// �������� ����, ��� ��� ������ - Output
        /// </summary>
        public bool IsOutput
            {
            get
                {
                return isOutput;
                }
            }


        /// <summary>
        /// ��� ������
        /// </summary>
        public string Name
            {
            get
                {
                return this.polusName;
                }
            set
                {
                this.polusName = value;
                }
            }


        /// <summary>
        /// ������� spy-�������
        /// </summary>
        public bool IsSpyObject
            {
            get { return isSpyObject; }
            set { isSpyObject = value; }
            }


        /// <summary>
        /// ��� ������
        /// </summary>
        private string polusName = string.Empty;
        /// <summary>
        /// ������� �������� ������
        /// </summary>
        private bool isInput = true;
        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        private bool isOutput = true;
        /// <summary>
        /// ������� spy-�������
        /// </summary>
        private bool isSpyObject = false;
        };    
    }
