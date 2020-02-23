using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

using TriadCompiler.Parser.Common.Statement;
using TriadCompiler.Parser.Common.Polus;

namespace TriadCompiler.Parser.Routine.Statement
    {
    /// <summary>
    /// ������ ��������� case � ������
    /// </summary>
    internal class Case : CommonParser
        {
        /// <summary>
        /// �������� �������������� ������
        /// </summary>
        /// <syntax>Case caseConditionList Colon StatementList 
        /// {Break caseConditionList Colon StatementList} EndCase</syntax>
        /// <param name="endKeys">��������� �������� ��������</param>
        /// <param name="context">������� ��������</param>
        /// <returns>������������� ��� ��������� ����</returns>
        public static CodeStatement Parse( EndKeyList endKeys, StatementContext context )
            {
            Accept( Key.Case );

            CodeConditionStatement ifStat = new CodeConditionStatement();

            ifStat.Condition = CaseConditionList( endKeys.UniteWith( Key.Colon, Key.EndCase ) );

            Accept( Key.Colon );
            ifStat.TrueStatements.AddRange( StatementList.Parse( endKeys.UniteWith( Key.Break, Key.EndCase ), context ) );

            CodeConditionStatement ifParentStat, ifFirstStat;
            ifFirstStat = ifStat;

            while ( currKey == Key.Break )
                {
                GetNextKey();

                ifParentStat = ifStat;
                ifStat = new CodeConditionStatement();
                ifParentStat.FalseStatements.Add( ifStat );

                ifStat.Condition = CaseConditionList( endKeys.UniteWith( Key.Colon, Key.EndCase ) );

                Accept( Key.Colon );
                ifStat.TrueStatements.AddRange( StatementList.Parse( endKeys.UniteWith( Key.Break, Key.EndCase ), context ) );
                }
            Accept( Key.EndCase );

            return ifFirstStat;
            }


        /// <summary>
        /// ����� � ��������� case
        /// </summary>
        /// <syntax>caseCondition{,caseCondition}</syntax>
        /// <param name="endKeys">��������� �������� ��������</param>
        /// <returns>������������� ��� ��������� ����</returns>
        private static CodeExpression CaseConditionList( EndKeyList endKeys )
            {
            CodeExpression ifExprStat = CaseCondition( endKeys.UniteWith( Key.Comma ) );
            while ( currKey == Key.Comma )
                {
                Accept( Key.Comma );
                CodeExpression newCompareStat = CaseCondition( endKeys.UniteWith( Key.Comma ) );
                ;
                CodeExpression oldIfExprStat = ifExprStat;
                ifExprStat = new CodeBinaryOperatorExpression();
                ( ifExprStat as CodeBinaryOperatorExpression ).Left = oldIfExprStat;
                ( ifExprStat as CodeBinaryOperatorExpression ).Right = newCompareStat;
                ( ifExprStat as CodeBinaryOperatorExpression ).Operator = CodeBinaryOperatorType.BooleanOr;
                }

            return ifExprStat;
            }


        /// <summary>
        /// ��� ������ � ��������� case
        /// </summary>
        /// <syntax>PolusVariable</syntax>
        /// <param name="endKeys">��������� �������� ��������</param>
        /// <returns>������������� ��� ��������� ����</returns>
        private static CodeExpression CaseCondition( EndKeyList endKeys )
            {
            CodeMethodInvokeExpression compareMethod = new CodeMethodInvokeExpression();

            PolusInfo polusInfo = PolusVar.Parse( endKeys.UniteWith( Key.Comma ) );

            //�������� ���� ������
            if ( polusInfo.Type != null )
                if ( !polusInfo.Type.IsInput )
                    Fabric.Instance.ErrReg.Register( Err.Parser.Type.Polus.Need.Input );

            //���������� ��������� ���� �������
            compareMethod.Method = new CodeMethodReferenceExpression(
                new CodeArgumentReferenceExpression( Builder.Routine.Receive.ReceivedPolus ),
                Builder.CoreName.Compare );
            compareMethod.Parameters.Add( polusInfo.CoreNameCode );

            return compareMethod;
            }
        }
    }
