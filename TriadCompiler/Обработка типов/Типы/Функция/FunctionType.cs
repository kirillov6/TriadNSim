using System;
using System.Collections.Generic;
using System.Text;

namespace TriadCompiler
    {
    /// <summary>
    /// ��� �������
    /// </summary>
    internal class FunctionType : ParameterList<IExprType>, ICommonType
        {
        /// <summary>
        /// ���
        /// </summary>
        public string Name
            {
            get
                {
                return typeName;
                }
            set
                {
                typeName = value;
                }
            }


        /// <summary>
        /// ��� ������� � ��������������� ����
        /// </summary>
        public string MethodCodeName
            {
            get
                {
                return this.strCode;
                }
            set
                {
                this.strCode = value;
                }
            }


        /// <summary>
        /// ���, ����������� ��������
        /// </summary>
        public IExprType ReturnedType
            {
            get
                {
                return this.returnedType;
                }
            set
                {
                this.returnedType = value;
                }
            }


        /// <summary>
        /// ��� ����
        /// </summary>
        private string typeName = string.Empty;
        /// <summary>
        /// ��� ������� � ��������������� ����
        /// </summary>
        private string strCode = string.Empty;
        /// <summary>
        /// ���, ����������� ��������
        /// </summary>
        private IExprType returnedType = null;
        }    
    }
