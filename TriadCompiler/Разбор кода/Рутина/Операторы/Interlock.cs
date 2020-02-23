using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

using TriadCompiler.Parser.Common.Polus;

namespace TriadCompiler.Parser.Routine.Statement
    {
    /// <summary>
    /// ������ ��������� ������������ ������
    /// </summary>
    internal class Interlock : CommonParser
        {
        /// <summary>
        /// �������� ������������ ������
        /// </summary>
        /// <syntax>Iterlock PolusVariable {,PolusVariable}</syntax>
        /// <param name="endKeys">��������� �������� ��������</param>
        /// <returns>������������� ��� ��������� ����</returns>
        public static CodeStatementCollection Parse( EndKeyList endKeys )
            {
            CodeStatementCollection resultStatList = new CodeStatementCollection();

            Accept( Key.Interlock );

            PolusInfo polusInfo = PolusVar.Parse( endKeys.UniteWith( Key.Comma ) );

            //���� ��� �� ������� �����
            if ( polusInfo.Type != null )
                if ( !polusInfo.Type.IsInput )
                    Fabric.Instance.ErrReg.Register( Err.Parser.Type.Polus.Need.Input );

            //������� ����� ������� interlock
            CodeMethodInvokeExpression interlockStat = new CodeMethodInvokeExpression();
            interlockStat.Method = new CodeMethodReferenceExpression( null, Builder.Routine.Block.Interlock );
            //��������� ����������� �����
            interlockStat.Parameters.Add( polusInfo.CoreNameCode );
            resultStatList.Add( interlockStat );

            while ( currKey == Key.Comma )
                {
                GetNextKey();
                polusInfo = PolusVar.Parse( endKeys.UniteWith( Key.Comma ) );

                //���� ��� �� ������� �����
                if ( polusInfo.Type != null )
                    if ( !polusInfo.Type.IsInput )
                        Fabric.Instance.ErrReg.Register( Err.Parser.Type.Polus.Need.Input );

                //������� ����� ������� interlock
                interlockStat = new CodeMethodInvokeExpression();
                interlockStat.Method = new CodeMethodReferenceExpression( null, Builder.Routine.Block.Interlock );
                //��������� ����������� �����
                interlockStat.Parameters.Add( polusInfo.CoreNameCode );
                resultStatList.Add( interlockStat );
                }

            return resultStatList;
            }
        }
    }
