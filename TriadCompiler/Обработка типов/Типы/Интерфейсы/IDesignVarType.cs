using System;
using System.Collections.Generic;
using System.Text;

namespace TriadCompiler
    {
    /// <summary>
    /// ��������� design ����������
    /// </summary>
    internal interface IDesignVarType : ICommonType
        {
        /// <summary>
        /// ��� design ����������
        /// </summary>
        DesignTypeCode TypeCode
            {
            get;
            }
        }
    }
