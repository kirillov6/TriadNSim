using System;
using System.Collections.Generic;
using System.Text;


namespace TriadCompiler.Parser.Common.Expr
    {
    /// <summary>
    /// ��������� ���� ��� �������������� ���������
    /// </summary>
    internal partial class Expression : CommonParser
        {
        /// <summary>
        /// ��������� ���������� ������������� ���������
        /// </summary>
        /// <param name="info">������ � ����� ������� ���������</param>
        /// <param name="relationOperation">�������� ���������</param>
        /// <param name="rightSimpleExprInfo">������ � ������ ������� ���������</param>
        private static void BuildStringCodeForExpression( ExprInfo info, Key relationOperation, ExprInfo rightSimpleExprInfo )
            {
            if ( info.HasNoError && rightSimpleExprInfo.HasNoError )
                {
                switch ( relationOperation )
                    {
                    case Key.Equal:
                        info.Append( "==" );
                        info.Append( rightSimpleExprInfo.StrCode );
                        break;

                    case Key.NotEqual:
                        info.Append( "!=" );
                        info.Append( rightSimpleExprInfo.StrCode );
                        break;

                    case Key.Later:
                        if ( info.Type.Code == TypeCode.String )
                            {
                            info.InsertFirst( "String.Compare(" );
                            info.Append( "," );
                            info.Append( rightSimpleExprInfo.StrCode );
                            info.Append( ")<0" );
                            }
                        else
                            {
                            info.Append( "<" );
                            info.Append( rightSimpleExprInfo.StrCode );
                            }
                        break;

                    case Key.LaterEqual:
                        if ( info.Type.Code == TypeCode.String )
                            {
                            info.InsertFirst( "String.Compare(" );
                            info.Append( "," );
                            info.Append( rightSimpleExprInfo.StrCode );
                            info.Append( ")<=0" );
                            }
                        else
                            {
                            info.Append( "<=" );
                            info.Append( rightSimpleExprInfo.StrCode );
                            }
                        break;

                    case Key.Greater:
                        if ( info.Type.Code == TypeCode.String )
                            {
                            info.InsertFirst( "String.Compare(" );
                            info.Append( "," );
                            info.Append( rightSimpleExprInfo.StrCode );
                            info.Append( ")>0" );
                            }
                        else
                            {
                            info.Append( ">" );
                            info.Append( rightSimpleExprInfo.StrCode );
                            }
                        break;

                    case Key.GreaterEqual:
                        if ( info.Type.Code == TypeCode.String )
                            {
                            info.InsertFirst( "String.Compare(" );
                            info.Append( "," );
                            info.Append( rightSimpleExprInfo.StrCode );
                            info.Append( ")>=0" );
                            }
                        else
                            {
                            info.Append( ">=" );
                            info.Append( rightSimpleExprInfo.StrCode );
                            }
                        break;

                    case Key.In:
                        info.InsertFirst( "(" + rightSimpleExprInfo.StrCode + ").In(" );
                        info.Append( ")" );
                        break;
                    }
                }
            }

        }
    }
