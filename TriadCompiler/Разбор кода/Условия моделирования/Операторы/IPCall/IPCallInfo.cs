using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace TriadCompiler.Parser.SimCondition.Statement
    {
    /// <summary>
    /// ���������� � ������ ��
    /// </summary>
    class IPCallInfo
        {
        /// <summary>
        /// ��� ��
        /// </summary>
        public IProcedureType Type = null;
        /// <summary>
        /// ������������ ���
        /// </summary>
        public CodeStatementCollection Code = new CodeStatementCollection();
        /// <summary>
        /// ���������� ����� ��
        /// </summary>
        public int ipCallNumber = 0;
        }
    }
