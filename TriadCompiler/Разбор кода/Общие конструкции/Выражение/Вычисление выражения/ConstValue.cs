using System;

namespace TriadCompiler
    {
    /// <summary>
    /// ������������ �����, ��������������� ��� �������� � ���������� ����������� ���������.
    /// </summary>
    internal class ConstValue
        {
        /// <summary>
        /// 	<para> ����������� ������ <see cref="ConstValue"/> .</para>
        /// </summary>
        public ConstValue()
        { }


        /// <summary>
        /// �������� ����, �������� �� �������� ����������
        /// </summary>
        /// <returns>true - ���� ��� ���������</returns>
        public bool IsConstant
            {
            get
                {
                return isConstant;
                }
            }


        /// <summary>
        /// ���������� ���������� �� ������� �������� relationOperation � ������� ����������
        /// </summary>
        /// <param name="operation">��� ��������</param>
        /// <returns>��������� �������� ����������</returns>
        public virtual ConstValue CalculateWith( Key operation )
            {
            return this;
            }


        /// <summary>
        /// ���������� ���������� �� �������� relationOperation � ������� � ���������� ����������
        /// ��������� ����� �������� � ������� ��������
        /// </summary>
        /// <param name="operation">��� ��������</param>
        /// <param name="operand">������ ������� (������ - this)</param>
        /// <returns>��������� �������� ����������</returns>
        public virtual ConstValue CalculateWith( Key operation, ConstValue operand )
            {
            return this;
            }


        /// <summary>
        /// ��������� ����, ��� �������� - ���������
        /// </summary>
        protected bool isConstant = false;
        }
    }


