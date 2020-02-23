using System;
using System.Collections.Generic;
using System.Text;

namespace TriadCompiler
    {
    /// <summary>
    /// ��� ���� �����������
    /// </summary>
    public class DesignTypeType : ParameterList<IExprType>, ICommonType
    {
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="typeName">��� ����</param>
        /// <param name="typeCode">��� ����</param>
        public DesignTypeType( string typeName, DesignTypeCode typeCode )
            {
            this.Code = typeCode;
            this.Name = typeName;
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


        /// <summary>
        /// ��� ����
        /// </summary>
        public DesignTypeCode Code
            {
            get
                {
                return typeCode;
                }
            set
                {
                this.typeCode = value;
                }
            }


        /// <summary>
        /// ��� ������-����
        /// </summary>
        private string typeName = "";
        /// <summary>
        /// ��� ����
        /// </summary>
        private DesignTypeCode typeCode = DesignTypeCode.Structure;
        }

    }
