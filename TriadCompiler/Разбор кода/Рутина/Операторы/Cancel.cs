using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

using TriadCompiler.Parser.Common.Ev;

namespace TriadCompiler.Parser.Routine.Statement
    {
    /// <summary>
    /// ������ ��������� ������ �������
    /// </summary>
    internal class Cancel : CommonParser
        {
        /// <summary>
        /// �������� ������ ������� 
        /// </summary>
        /// <syntax>Cancel Identificator {,Identificator}</syntax>
        /// <param name="endKeys">��������� �������� ��������</param>
        /// <returns>������������� ��� ��������� ����</returns>
        public static CodeStatement Parse( EndKeyList endKeys )
            {
            //������� ����� ������� cancel
            CodeMethodInvokeExpression cancelStat = new CodeMethodInvokeExpression();
            cancelStat.Method = new CodeMethodReferenceExpression( null, Builder.Routine.Shedule.CancelEvent );

            Accept( Key.Cancel );

            //��� �������
            EventInfo eventInfo = EventVar.Parse( endKeys.UniteWith( Key.Comma, Key.In ), false );

            while ( currKey == Key.Comma )
                {
                GetNextKey();

                //��� �������
                eventInfo = EventVar.Parse( endKeys.UniteWith( Key.Comma, Key.In ), false );
                }

            return new CodeExpressionStatement( cancelStat );
            }
        }
    }
