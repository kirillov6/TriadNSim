using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

using TriadCompiler.Parser.Model.DesignVar;
using TriadCompiler.Parser.Structure.StructExpr;
using TriadCompiler.Parser.Common.ObjectRef;

namespace TriadCompiler.Parser.Model.Statement
    {
    /// <summary>
    /// ������ ��������� ��������� ������
    /// </summary>
    internal class PutRoutine : CommonParser
        {
        /// <summary>
        /// �������� ��������� �����
        /// </summary>
        /// <param name="endKeys">��������� ���������� �������� ��������</param>
        /// <syntax>Put DesignVariable On DesignVariable.ObjectReference #PolusPairList#</syntax>
        /// <returns>��������������� ���</returns>
        public static CodeStatementCollection Parse( EndKeyList endKeys )
            {
            CodeStatementCollection resultStatList = new CodeStatementCollection();

            CodeMethodInvokeExpression registerStat = new CodeMethodInvokeExpression();
            Accept( Key.Put );
            string routineVarName = DesignVariable.Parse( endKeys.UniteWith( Key.On ), DesignTypeCode.Routine ).StrCode;

            Accept( Key.On );

            string graphVarName = DesignVariable.Parse( endKeys.UniteWith( Key.Point, Key.Later ), DesignTypeCode.Structure ).StrCode;

            //���� ������� ������� �����, �� ������� ����� �������� ������
            if ( currKey == Key.Point )
                {
                Accept( Key.Point );
                CodeObjectCreateExpression nodeNameCode = ObjectReference.Parse( endKeys.UniteWith( Key.Later ), true ).CoreNameCode;

                registerStat.Method.TargetObject = new CodeSnippetExpression( graphVarName );
                registerStat.Method.MethodName = Builder.Model.PutRoutine.PutOnOneNodeMethod;
                registerStat.Parameters.Add( nodeNameCode );
                registerStat.Parameters.Add( new CodeSnippetExpression( routineVarName ) );
                }
            //����� - ����������� ������ �� ��� ������� �����
            else
                {
                registerStat.Method.TargetObject = new CodeSnippetExpression( graphVarName );
                registerStat.Method.MethodName = Builder.Model.PutRoutine.PutOnAllNodesMethod;
                registerStat.Parameters.Add( new CodeSnippetExpression( routineVarName ) );
                }            

            //������ ������������ ������� ������ � �������
            if ( currKey == Key.Later )
                {
                resultStatList.AddRange( PolusPairList( endKeys, routineVarName ) );
                }

            resultStatList.Add( registerStat );

            return resultStatList;
            }


        /// <summary>
        /// ������ ������ ������������ ������� ������ � �������
        /// </summary>
        /// <param name="endKeys">��������� ���������� �������� ��������</param>
        /// <param name="routineVarName">��� ������-����������</param>
        /// <synatx> {SinglePolusPair} </synatx>
        /// <returns>������ ����������</returns>
        private static CodeStatementCollection PolusPairList( EndKeyList endKeys, string routineVarName )
            {
            CodeStatementCollection resultStatList = new CodeStatementCollection();

            //��������� ������ ������� ������� ������� ������ ������������
            CodeMethodInvokeExpression clearStat = new CodeMethodInvokeExpression(
                new CodeArgumentReferenceExpression( routineVarName ),
                Builder.Model.PutRoutine.ClearPolusPairList );
            resultStatList.Add( new CodeExpressionStatement( clearStat ) );

            GetNextKey();

            CodeStatement addPairStat = SinglePolusPair( endKeys.UniteWith( Key.Greater, Key.Comma ), routineVarName );
            resultStatList.Add( addPairStat );

            while ( currKey == Key.Comma )
                {
                GetNextKey();

                addPairStat = SinglePolusPair( endKeys.UniteWith( Key.Greater, Key.Comma ), routineVarName );
                resultStatList.Add( addPairStat );
                }

            Accept( Key.Greater );

            return resultStatList;
            }


        /// <summary>
        /// ������ ������ ������������ ����� ������� ������ � �������
        /// </summary>
        /// <param name="endKeys">��������� ���������� �������� ��������</param>
        /// <param name="routineVarName">��� ������-����������</param>
        /// <syntax>PolusVar = PolusVar</syntax>
        /// <returns>��������</returns>
        private static CodeStatement SinglePolusPair( EndKeyList endKeys, string routineVarName )
            {
            ObjectRefInfo routinePolusInfo = ObjectReference.Parse( endKeys.UniteWith( Key.Equal ), true );

            Accept( Key.Equal );

            ObjectRefInfo nodePolusInfo = ObjectReference.Parse( endKeys, true );

            //��������� ���� ������ ������ ���������� �����������
            CodeMethodInvokeExpression addPairStat = new CodeMethodInvokeExpression( new CodeArgumentReferenceExpression( routineVarName ),
                Builder.Model.PutRoutine.AddPolusPairMethod,
                routinePolusInfo.CoreNameCode, nodePolusInfo.CoreNameCode );

            return new CodeExpressionStatement( addPairStat );
            }

        }
    }
