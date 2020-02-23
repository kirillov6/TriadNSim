using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

using TriadCompiler.Parser.Common.ObjectRef;

namespace TriadCompiler.Parser.Structure.StructExpr.Conn
    {
    /// <summary>
    /// ������ ���������� � ����������� ���������
    /// </summary>
    internal class Connection : CommonParser
        {
        /// <summary>
        /// ���� ��� �����, ����������� ������
        /// </summary>
        /// <param name="endKeys">��������� ���������� �������� ��������</param>
        /// <param name="createConnectionMethodName">��� ������, ���������� ����������</param>
        /// <syntax>Arc | Edge ConnectionTerminalNode -- ConnectionTerminalNode</syntax>
        /// <returns>��������������� ���</returns>
        public static CodeStatementCollection Parse( EndKeyList endKeys, string createConnectionMethodName )
            {
            CodeStatementCollection resultStatList = new CodeStatementCollection();

            if ( currKey != Key.LeftPar )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.WrongStartSymbol.Connection, Key.LeftPar );
                SkipTo( endKeys.UniteWith( Key.LeftPar ) );
                }
            if ( currKey == Key.LeftPar )
                {
                Accept( Key.LeftPar );

                //������� ������ ����
                CodeMethodInvokeExpression createGraphStat = new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(), Builder.Structure.BuildExpr.Stack.PushNew );
                resultStatList.Add( createGraphStat );

                //����� ������ ���������� ����������
                CodeMethodInvokeExpression addConnectionStat = new CodeMethodInvokeExpression();

                //����� ������ ���������� ����
                addConnectionStat.Method = new CodeMethodReferenceExpression(
                    new CodeFieldReferenceExpression( new CodeThisReferenceExpression(), Builder.Structure.BuildExpr.Stack.First ),
                    createConnectionMethodName );


                //������ ����������
                resultStatList.AddRange( ConnectionEndPoint( endKeys.UniteWith( Key.Connection, Key.RightPar ), addConnectionStat ) );

                Accept( Key.Connection );

                //����� ����������
                resultStatList.AddRange( ConnectionEndPoint( endKeys.UniteWith( Key.RightPar ), addConnectionStat ) );

                resultStatList.Add( addConnectionStat );

                Accept( Key.RightPar );

                if ( !endKeys.Contains( currKey ) )
                    {
                    Fabric.Instance.ErrReg.Register( Err.Parser.WrongEndSymbol.Connection, StructExprKeySet.Operation.All );
                    SkipTo( endKeys );
                    }
                }

            return resultStatList;
            }


        /// <summary>
        /// ���� �� ������ ���� ��� �����
        /// </summary>
        /// <param name="endKeys">��������� ���������� �������� ��������</param>
        /// <param name="addConnectionStat">������ �� �����, ����������� ����������</param>
        /// <returns>��������������� ���</returns>
        /// <synatx>ObjectReference . ObjectReference</synatx>
        private static CodeStatementCollection ConnectionEndPoint( EndKeyList endKeys, CodeMethodInvokeExpression addConnectionStat )
            {
            CodeStatementCollection resultStatList = new CodeStatementCollection();

            //��� ������ �������
            CodeObjectCreateExpression nodeNameCode = ObjectReference.Parse( endKeys.UniteWith( Key.Point ), false ).CoreNameCode;

            //��������� � ������� ����� ��� �������
            CodeMethodInvokeExpression declareNodeStat = new CodeMethodInvokeExpression(
                new CodeFieldReferenceExpression( new CodeThisReferenceExpression(), Builder.Structure.BuildExpr.Stack.First ),
                Builder.Structure.BuildExpr.DeclareOperation.DeclareNodeInGraph,
                nodeNameCode );
            resultStatList.Add( new CodeExpressionStatement( declareNodeStat ) );

            Accept( Key.Point );

            //��� ������ �������
            CodeObjectCreateExpression polusNameCode = ObjectReference.Parse( endKeys, false ).CoreNameCode;

            //��������� ���� ����� � �������
            declareNodeStat.Parameters.Add( polusNameCode );

            //��������� ������� � ��������� ������� ���������� ����������
            addConnectionStat.Parameters.Add( nodeNameCode );
            //��������� ����� ������� � ��������� ������� ���������� ����������
            addConnectionStat.Parameters.Add( polusNameCode );

            return resultStatList;
            }
        }
    }
