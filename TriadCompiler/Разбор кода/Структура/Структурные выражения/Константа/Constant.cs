using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

using TriadCompiler.Parser.Structure.StructExpr.Node;

namespace TriadCompiler.Parser.Structure.StructExpr.Const
    {
    /// <summary>
    /// ������ �������� ��������� � ����������� ���������
    /// </summary>
    internal class Constant : CommonParser
        {
        /// <summary>
        /// ����������� ���������
        /// </summary>
        /// <param name="endKeys">��������� ���������� �������� ��������</param>
        /// <param name="createConstGraphMethosName">�������� ������, ���������� ����������� ���������</param>
        /// <syntax>DirectCycle | UndirectCycle | DirectPath | UndirectPath |
        /// DirectStar | UndirectStar ( NodeDeclarationInExpr , { NodeDeclarationInExpr } ) </syntax>
        /// <returns>��������������� ���</returns>
        public static CodeStatementCollection Parse( EndKeyList endKeys, string createConstGraphMethosName )
            {
            CodeStatementCollection resultStatList = new CodeStatementCollection();

            if ( currKey != Key.LeftPar )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.WrongStartSymbol.StructConstant, Key.LeftPar );
                SkipTo( endKeys.UniteWith( Key.LeftPar ) );
                }
            if ( currKey == Key.LeftPar )
                {
                Accept( Key.LeftPar );

                //������� ������ ����
                CodeMethodInvokeExpression createGraphStat = new CodeMethodInvokeExpression();
                createGraphStat.Method.TargetObject = new CodeThisReferenceExpression();

                resultStatList.Add( new CodeExpressionStatement( createGraphStat ) );
                createGraphStat.Method.MethodName = createConstGraphMethosName;

                //���������� �������
                resultStatList.AddRange( NodeDeclaration.Parse( endKeys.UniteWith( Key.Comma, Key.RightPar ) ) );

                //��� ���������� ���� ������� � ����-���������
                resultStatList.AddRange( StructExpression.ExpressionCode( Key.Plus ) );

                while ( currKey == Key.Comma )
                    {
                    Accept( Key.Comma );

                    //���������� �������
                    resultStatList.AddRange( NodeDeclaration.Parse( endKeys.UniteWith( Key.Comma, Key.RightPar ) ) );

                    //��� ���������� ���� ������� � ����-���������
                    resultStatList.AddRange( StructExpression.ExpressionCode( Key.Plus ) );
                    }

                //����� ������ completeGraph
                CodeMethodInvokeExpression completeGraphStat = new CodeMethodInvokeExpression(
                    new CodeFieldReferenceExpression( new CodeThisReferenceExpression(),
                                                    Builder.Structure.BuildExpr.Stack.First ),
                    Builder.Structure.BuildExpr.DeclareOperation.Complete );

                resultStatList.Add( new CodeExpressionStatement( completeGraphStat ) );

                Accept( Key.RightPar );

                if ( !endKeys.Contains( currKey ) )
                    {
                    Fabric.Instance.ErrReg.Register( Err.Parser.WrongEndSymbol.StructConstant, StructExprKeySet.Operation.All );
                    SkipTo( endKeys );
                    }
                }

            return resultStatList;
            }


        
        }
    }
