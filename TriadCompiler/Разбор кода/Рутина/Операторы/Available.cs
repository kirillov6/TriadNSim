using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

using TriadCompiler.Parser.Common.Polus;

namespace TriadCompiler.Parser.Routine.Statement
    {
    /// <summary>
    /// ������ ��������� ��������������� �������
    /// </summary>
    internal class Available : CommonParser
        {
        /// <summary>
        /// �������� ��������������� ������ 
        /// </summary>
        /// <syntax>Available PolusVariable {,PolusVariable}</syntax>
        /// <param name="endKeys">��������� �������� ��������</param> 
        /// <returns>������������� ��� ��������� ����</returns>
        public static CodeStatementCollection Parse( EndKeyList endKeys )
            {
            CodeStatementCollection resultStatList = new CodeStatementCollection();

            Accept( Key.Available );

            PolusInfo polusInfo = PolusVar.Parse( endKeys.UniteWith( Key.Comma ) );

            //���� ��� �� ������� �����
            if ( polusInfo.Type != null )
                if ( !polusInfo.Type.IsInput )
                    Fabric.Instance.ErrReg.Register( Err.Parser.Type.Polus.Need.Input );

            //������� ����� ������� available
            CodeMethodInvokeExpression availableStat = new CodeMethodInvokeExpression();
            availableStat.Method = new CodeMethodReferenceExpression( null, Builder.Routine.Block.Available );
            //��������� �������������� �����
            availableStat.Parameters.Add( polusInfo.CoreNameCode );
            resultStatList.Add( availableStat );

            while ( currKey == Key.Comma )
                {
                GetNextKey();

                polusInfo = PolusVar.Parse( endKeys.UniteWith( Key.Comma ) );

                //���� ��� �� ������� �����
                if ( polusInfo.Type != null )
                    if ( !polusInfo.Type.IsInput )
                        Fabric.Instance.ErrReg.Register( Err.Parser.Type.Polus.Need.Input );

                //������� ����� ������� available
                availableStat = new CodeMethodInvokeExpression();
                availableStat.Method = new CodeMethodReferenceExpression( null, Builder.Routine.Block.Available );
                //��������� �������������� �����
                availableStat.Parameters.Add( polusInfo.CoreNameCode );
                resultStatList.Add( availableStat );
                }

            return resultStatList;
            }
        }
    }
