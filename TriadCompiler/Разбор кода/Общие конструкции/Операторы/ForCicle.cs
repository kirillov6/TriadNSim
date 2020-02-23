using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

using TriadCompiler.Parser.Common.Var;
using TriadCompiler.Parser.Common.Expr;

namespace TriadCompiler.Parser.Common.Statement
    {
    /// <summary>
    /// ������ ����� �� ���������
    /// </summary>
    internal class ForCicle : CommonParser
        {
        /// <summary>
        /// ���� �� ���������
        /// </summary>
        /// <syntax>For Variable := Expression [ By Expression ] To Expression Do StatementList EndFor</syntax>
        /// <param name="endKeys">��������� �������� ��������</param>
        /// <param name="context">������� ��������</param>
        /// <returns>������������� ��� ��������� ����</returns>
        public static CodeStatement Parse( EndKeyList endKeys, StatementContext context )
            {
            CodeIterationStatement forStatement = new CodeIterationStatement();
            Accept( Key.For );

            VarInfo indexVarInfo = Variable.Parse( endKeys.UniteWith( Key.Assign, Key.EndFor ), false, false );

            //��� ���������� ������ ���� �����
            if ( indexVarInfo.HasNoError )
                if ( indexVarInfo.Type.Code != TypeCode.Integer )
                    {
                    Fabric.Instance.ErrReg.Register( Err.Parser.Type.Var.Need.Integer );
                    }

            Accept( Key.Assign );

            ExprInfo initialExprInfo = Expression.Parse( endKeys.UniteWith( Key.By, Key.To, Key.DownTo, Key.EndFor ) );

            //��������� ����
            forStatement.InitStatement = new CodeAssignStatement( indexVarInfo.Code,
                initialExprInfo.Code );


            //��������� ������ � ��������� ���������
            bool errorInInitialExpr = !initialExprInfo.IsNotIndexed();
            errorInInitialExpr |= !initialExprInfo.IsNotSet();
            //��� ��������� ������ ���� �����
            errorInInitialExpr |= !initialExprInfo.IsInteger();

            ExprInfo stepExprInfo = new ExprInfo();

            //���� ������ ��� ��������� �������
            if ( currKey == Key.By )
                {
                Accept( Key.By );

                stepExprInfo = Expression.Parse( endKeys.UniteWith( Key.To, Key.DownTo, Key.EndFor ) );

                stepExprInfo.IsNotIndexed();
                stepExprInfo.IsNotSet();
                //��� ��������� ������ ���� �����
                stepExprInfo.IsInteger();
                //��� ��������� ������ ���� ����������
                stepExprInfo.IsConstant();
                //��������� ������ ���� �������������
                stepExprInfo.PositiveIntegerOrReal();
                }
            //�� ��������� ����������� �� 1
            else
                {
                stepExprInfo.Append( "1" );
                }


            //����������� ��������� �������
            bool growingDirect = true;

            if ( currKey == Key.To )
                {
                Accept( Key.To );
                }
            else if ( currKey == Key.DownTo )
                {
                Accept( Key.DownTo );
                growingDirect = false;
                }
            else
                Accept( Key.To );

            ExprInfo terminalExprInfo = Expression.Parse( endKeys.UniteWith( Key.Do, Key.EndFor ) );

            bool errorInTerminalExpr = !terminalExprInfo.IsNotIndexed();
            errorInTerminalExpr |= !terminalExprInfo.IsNotSet();
            //��� ��������� ������ ���� �����
            errorInTerminalExpr |= !terminalExprInfo.IsInteger();

            //���� ��� ��������� ����������
            if ( !errorInInitialExpr && !errorInTerminalExpr )
                    //���� ��������� ��������� � �������� ���� ����������
                    if ( initialExprInfo.Value.IsConstant && terminalExprInfo.Value.IsConstant )
                        if ( growingDirect && ( initialExprInfo.Value as IntegerValue ).Value >
                                          ( terminalExprInfo.Value as IntegerValue ).Value )
                            {
                            Fabric.Instance.ErrReg.Register( Err.Parser.Usage.For.InitExprIsGreaterThanTerminal );
                            }
                        else if ( !growingDirect && ( initialExprInfo.Value as IntegerValue ).Value <
                                          ( terminalExprInfo.Value as IntegerValue ).Value )
                            {
                            Fabric.Instance.ErrReg.Register( Err.Parser.Usage.For.InitExprIsLowerThanTerminal );
                            }


            CodeBinaryOperatorExpression compareStatementCode = new CodeBinaryOperatorExpression();
            compareStatementCode.Left = indexVarInfo.Code;
            compareStatementCode.Right = terminalExprInfo.Code;

            CodeAssignStatement stepStatementCode = new CodeAssignStatement();
            stepStatementCode.Left = indexVarInfo.Code;
            CodeBinaryOperatorExpression stepExprCode = new CodeBinaryOperatorExpression();
            stepStatementCode.Right = stepExprCode;

            stepExprCode.Left = indexVarInfo.Code;
            stepExprCode.Right = stepExprInfo.Code;

            if ( growingDirect )
                {
                compareStatementCode.Operator = CodeBinaryOperatorType.LessThanOrEqual;
                stepExprCode.Operator = CodeBinaryOperatorType.Add;
                }
            else
                {
                compareStatementCode.Operator = CodeBinaryOperatorType.GreaterThanOrEqual;
                stepExprCode.Operator = CodeBinaryOperatorType.Subtract;
                }

            forStatement.TestExpression = compareStatementCode;
            forStatement.IncrementStatement = stepStatementCode;

            Accept( Key.Do );

            forStatement.Statements.AddRange( StatementList.Parse( endKeys.UniteWith( Key.EndFor ), context ) );

            Accept( Key.EndFor );

            return forStatement;
            }
        }
    }
