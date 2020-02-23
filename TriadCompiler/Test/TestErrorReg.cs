using System;
using System.Collections.Generic;
using System.Text;

namespace TriadCompiler
    {
    /// <summary>
    /// ����� ��� ��������� ������������������ ������ � ����������
    /// </summary>
    internal class TestErrorReg : ErrorReg
        {
        /// <summary>
        /// ����������� ������
        /// </summary>
        /// <param name="errCode">��� ������</param>
        public override void Register( uint errCode )
            {
            errorCount++;
            io.TestError( errCode );
            }


        /// <summary>
        /// ����������� ������
        /// </summary>
        /// <param name="errCode">��� ������</param>
        /// <param name="additionalText">�������������� �����</param>
        public override void Register( uint errCode, string additionalText )
            {
            errorCount++;
            
            io.TestError( errCode );
            io.Output.PrintLine( additionalText );
            }


        /// <summary>
        /// ����������� ������, ��������� � ����������� ��������
        /// </summary>
        /// <param name="errCode">��� ������</param>
        /// <param name="allowedEndKeys">���������� �������� �������</param>
        public override void Register( uint errCode, List<Key> allowedEndKeys )
            {
            errorCount++;

            io.TestError( errCode );

            //������ ���������� �������� �������� �� ����
            }


        // �������� ����-�����
        private IOTest io
            {
            get
                {
                return Fabric.IO as IOTest;
                }
            }
        }
    }
