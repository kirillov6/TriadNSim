using System;
using System.Collections.Generic;
using System.Text;

namespace TriadCompiler
    {
    /// <summary>
    /// ��� ��������.
    /// </summary>
    internal class VarArrayType : IndexedType, IExprType
        {
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="typeCode">��� �������� ����</param>
        public VarArrayType( TypeCode typeCode )
            {
            this.Code = typeCode;
            }


        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="typeCode">��� �������� ����</param>
        /// <param name="varName">��� ����������</param>
        public VarArrayType( TypeCode typeCode, string varName )
            {
            this.code = typeCode;
            this.varName = varName;
            }


        /// <summary>
        /// ��� ���� ����������
        /// </summary>
        public TypeCode Code
            {
            get
                {
                return code;
                }
            set
                {
                code = value;
                }
            }


        /// <summary>
        /// ��� ���������� ����������
        /// </summary>
        public string Name
            {
            get { return varName; }
            set { varName = value; }
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
        /// ������� �����
        /// </summary>
        /// <returns></returns>
        public IExprType Clone()
            {
            VarArrayType cloneType = new VarArrayType( this.code, this.varName );
            foreach ( int indexLimit in this )
                cloneType.arrIndexMaxSizeList.Add( indexLimit );
            return cloneType;
            }


        /// <summary>
        /// ��� �������
        /// </summary>
        private string varName;
        /// <summary>
        /// ������� spy-�������
        /// </summary>
        private bool isSpyObject = false;
        /// <summary>
        /// ��� �������� ���� �������
        /// </summary>
        private TypeCode code;
        }
    }
