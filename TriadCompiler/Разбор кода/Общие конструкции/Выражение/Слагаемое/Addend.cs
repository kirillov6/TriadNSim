using System;
using System.Collections.Generic;
using System.Text;

using TriadCompiler.Parser.Common.Expr;
using TriadCompiler.Parser.Common.Expr.Fact;

namespace TriadCompiler.Parser.Common.Expr.Add
    {
    /// <summary>
    /// ������ ���������� � �������������� ���������
    /// </summary>
    internal partial class Addend : CommonParser
        {
        /// <summary>
        /// ��������� �������� ���������
        /// </summary>
        private static List<Key> multSet = null;


        /// <summary>
        /// ��������� �������� ���������
        /// </summary>
        private static List<Key> MultKeys
            {
            get
                {
                if ( multSet == null )
                    {
                    Key[] keySet = { Key.Star, Key.Slash, Key.And, Key.ResidueOfDivision };

                    multSet = new List<Key>( keySet );
                    }
                return multSet;
                }
            }


        /// <summary>
        /// ���������
        /// </summary>
        /// <syntax>Factor {MULT_OP Factor}</syntax>
        /// <param name="endKeys">��������� �������� ��������</param>
        /// <returns>���������� � ���������</returns>
        public static ExprInfo Parse( EndKeyList endKeys )
            {
            ExprInfo info = Factor.Parse( endKeys.UniteWith( MultKeys ) );

            while ( MultKeys.Contains( currKey ) )
                {
                ExprInfo nextFactorInfo = new ExprInfo();
                Key multiplierOperation = currKey;
                GetNextKey();

                nextFactorInfo = Factor.Parse( endKeys.UniteWith( MultKeys ) );

                //��������� ����
                BuildStringCodeForAddend( info, nextFactorInfo, multiplierOperation );

                //�������� �����
                info.Type = CheckTypeInFactor( info.Type, nextFactorInfo.Type, multiplierOperation );

                if ( info.HasNoError )
                    info.Value = info.Value.CalculateWith( multiplierOperation, nextFactorInfo.Value );
                }
            return info;
            }
        }
    }
