using System;
using System.Collections.Generic;
using System.Text;

using TriadCompiler.Parser.Common.Expr;

namespace TriadCompiler.Parser.Common.Expr.Add
    {
    /// <summary>
    /// ��������� ���� ��� ���������� � �������������� ���������
    /// </summary>
    internal partial class Addend
        {
        /// <summary>
        /// ��������� ���� � ���������
        /// </summary>
        /// <param name="info">���������� � ���� ���������</param>
        /// <param name="nextFactorInfo">���������� � ������� ���������</param>
        /// <param name="multiplierOperation">�������� ���������</param>
        private static void BuildStringCodeForAddend( ExprInfo info, ExprInfo nextFactorInfo, Key multiplierOperation )
            {
            if ( info.HasNoError && nextFactorInfo.HasNoError )
                {
                switch ( multiplierOperation )
                    {
                    //���������
                    case Key.Star:
                        info.Append( "*" );
                        info.Append( nextFactorInfo.StrCode );
                        break;

                    //�������
                    case Key.Slash:
                        info.Append( "/" );
                        info.Append( nextFactorInfo.StrCode );
                        break;

                    //���������� �
                    case Key.And:
                        if ( nextFactorInfo.Type.Code == TypeCode.Bit )
                            {
                            info.Append( "&" );
                            info.Append( nextFactorInfo.StrCode );
                            }
                        else
                            {
                            info.Append( "&&" );
                            info.Append( nextFactorInfo.StrCode );
                            }
                        break;

                    //������� �� �������
                    case Key.ResidueOfDivision:
                        info.Append( "%" );
                        info.Append( nextFactorInfo.StrCode );
                        break;
                    }
                }
            }
        }
    }
