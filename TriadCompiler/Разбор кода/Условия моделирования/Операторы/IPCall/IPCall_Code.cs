using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

using TriadCompiler.Parser.Common.Expr;

namespace TriadCompiler.Parser.SimCondition.Statement
    {
    /// <summary>
    /// ������ ������ �� (��������� ����)
    /// </summary>
    internal partial class IPCall
        {
        /// <summary>
        /// ������������� ���, ��������� ��������� ��
        /// </summary>
        /// <param name="ipName">��� ��</param>
        /// <param name="ipNumber">���������� ����� ��</param>
        /// <param name="paramExprInfoList">������ ����������</param>
        /// <returns>���</returns>
        public static CodeStatementCollection GenerateIProcedureCreation( string ipName, int ipNumber, List<ExprInfo> paramExprInfoList )
            {
            CodeStatementCollection statList = new CodeStatementCollection();

            CodeObjectCreateExpression createStat = new CodeObjectCreateExpression();
            createStat.CreateType = new CodeTypeReference( ipName );

            //��������� ��������� ��
            foreach ( ExprInfo exprInfo in paramExprInfoList )
                {
                createStat.Parameters.Add( exprInfo.Code );
                }

            //�������� �����, ��������� ��
            CodeMethodInvokeExpression addIpStat = new CodeMethodInvokeExpression();
            addIpStat.Method = new CodeMethodReferenceExpression();
            addIpStat.Method.MethodName = Builder.ICondition.AddIProcedure;

            addIpStat.Parameters.Add( createStat );
            addIpStat.Parameters.Add( new CodePrimitiveExpression( ipNumber ) );

            //�������� ���� ����� � ������� �������������
            statList.Add( addIpStat );

            return statList;
            }


        /// <summary>
        /// ������������� ���, ���������������� ��
        /// </summary>
        /// <param name="ipNumber">���������� ����� ��</param>
        /// <returns>���</returns>
        public static CodeStatementCollection GenerateIProcedureInitialization( int ipNumber )
            {
            CodeStatementCollection statList = new CodeStatementCollection();

            CodeMethodInvokeExpression initStat = new CodeMethodInvokeExpression();
            initStat.Method = new CodeMethodReferenceExpression();
            initStat.Method.MethodName = Builder.ICondition.InitializeIProcedure;
            initStat.Parameters.Add( new CodePrimitiveExpression( ipNumber ) );

            statList.Add( initStat );

            return statList;
            }


        /// <summary>
        /// �������� ���, ������������ ����������� ��
        /// </summary>
        /// <param name="ipName">��� ��</param>
        /// <param name="ipCallNumber">���������� ����� ��</param>
        /// <returns>���</returns>
        private static CodeExpression GetIProcedureCode( string ipName, int ipCallNumber )
            {
            return new CodeSnippetExpression( "((" + ipName + ")" + Builder.ICondition.GetIProcedure +
                "(" + ipCallNumber.ToString() + "))" );
            }
        }
    }
