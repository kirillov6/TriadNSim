using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

using TriadCompiler.Parser.Structure.StructExpr.Add;

namespace TriadCompiler.Parser.Structure.StructExpr
    {
    /// <summary>
    /// ������ ���������� ���������
    /// </summary>
    internal partial class StructExpression : CommonParser
        {
        /// <summary>
        /// ����������� ���������
        /// </summary>
        /// <param name="endKeys">��������� �������� ��������</param>
        /// <syntax>StructAddend {StructAdd StructAddend }</syntax>
        /// <returns></returns>
        public static CodeStatementCollection Parse( EndKeyList endKeys )
            {
            CodeStatementCollection resultStatList = new CodeStatementCollection();

            resultStatList.AddRange( Addend.Parse( endKeys.UniteWith( StructExprKeySet.Operation.Add ) ) );

            while ( StructExprKeySet.Operation.Add.Contains( currKey ) )
                {
                //��� ������� ��������
                Key currOperation = currKey;

                GetNextKey();
                resultStatList.AddRange( Addend.Parse( endKeys.UniteWith( StructExprKeySet.Operation.Add ) ) );

                //��������� ����
                resultStatList.AddRange( ExpressionCode( currOperation ) );
                }

            return resultStatList;
            }


        /// <summary>
        /// ������������� ��� ��� ������������ ���������
        /// </summary>
        /// <param name="operation">����������� ��������</param>
        /// <returns>��������������� ���</returns>
        public static CodeStatementCollection ExpressionCode( Key operation )
            {
            CodeStatementCollection resultStatList = new CodeStatementCollection();

            //���������� ���������� ��������
            CodeExpression resultExpr = new CodeExpression();

            switch ( operation )
                {
                //�������
                case Key.Plus:
                    CodeFieldReferenceExpression resultGraph = new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), Builder.Structure.BuildExpr.Stack.Second );
                    CodeFieldReferenceExpression addGraph = new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), Builder.Structure.BuildExpr.Stack.First );

                    resultExpr = new CodeMethodInvokeExpression( resultGraph, Builder.Structure.BuildExpr.DinamicOperation.Unite,
                        addGraph );
                    break;

                //���������
                case Key.Minus:
                    resultGraph = new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), Builder.Structure.BuildExpr.Stack.Second );
                    addGraph = new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), Builder.Structure.BuildExpr.Stack.First );

                    resultExpr = new CodeMethodInvokeExpression( resultGraph, Builder.Structure.BuildExpr.DinamicOperation.Substract,
                        addGraph );
                    break;

                default:
                    throw new ArgumentOutOfRangeException( "������������ ��� ��������" );
                }

            resultStatList.Add( new CodeExpressionStatement( resultExpr ) );

            //���������� ������� �����
            CodeExpression clearStackStat = new CodeMethodInvokeExpression( new CodeThisReferenceExpression(),
                Builder.Structure.BuildExpr.Stack.Pop );
            resultStatList.Add( new CodeExpressionStatement( clearStackStat ) );

            return resultStatList;
            }
        
        }
    }
