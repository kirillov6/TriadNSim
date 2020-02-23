using System;
using System.Collections.Generic;
using System.Text;

namespace TriadCompiler
    {
    /// <summary>
    /// ���, ����������� �������������� ���������
    /// </summary>
    public class IProcedureType : ParameterList<ISpyType>, ICommonType
        {
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="typeName">��� ����</param>
        public IProcedureType( string typeName )
            {
            this.Name = typeName;
            }


        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="typeName">��� ����</param>
        /// <param name="returnedType">����������� ��������</param>
        public IProcedureType( string typeName, TypeCode returnedType )
            {
            this.Name = typeName;
            this.returnedType = returnedType;
            }


        /// <summary>
        /// ��� ���� ����������
        /// ��� �������� ���������������� � IConditionType
        /// </summary>
        public virtual TypeCode ReturnCode
            {
            get
                {
                return returnedType;
                }
            set
                {
                this.returnedType = value;
                }
            }

        
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

        //By jum
        /// <summary>
        /// ��������
        /// </summary>
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }
        /// <summary>
        /// ��� ���� ����������
        /// </summary>
        private TypeCode returnedType = TypeCode.Void;
        /// <summary>
        /// ��� ��
        /// </summary>
        private string typeName = string.Empty;
        /// <summary>
        /// ��� �� � ����
        /// </summary>
        private string description = string.Empty;
        /// <summary>
        /// ������ out-����������
        /// </summary>
        public ParameterList<IExprType> OutVarList = new ParameterList<IExprType>();
        /// <summary>
        /// ������ ���������� ��
        /// </summary>
        public ParameterList<IExprType> ParamVarList = new ParameterList<IExprType>(); 
        }
    }
