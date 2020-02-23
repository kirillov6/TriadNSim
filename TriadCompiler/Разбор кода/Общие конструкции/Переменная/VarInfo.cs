using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

using TriadCompiler.Parser.Common.ObjectRef;

namespace TriadCompiler.Parser.Common.Var
    {
    /// <summary>
    /// ���������� � ����������
    /// </summary>
    internal class VarInfo : ObjectRefInfo
        {
        /// <summary>
        /// ��� ����������
        /// </summary>
        public IExprType Type = null;


        /// <summary>
        /// ������� ����, ��� ���������� ���� ��������� ���������
        /// </summary>
        public bool HasNoError
            {
            get
                {
                return this.Type != null;
                }
            }


        /// <summary>
        /// ������������� � ���� ����
        /// </summary>
        public CodeExpression Code
            {
            get
                {
                return new CodeSnippetExpression( StrCode.ToString() );
                }
            }

        }
    }
