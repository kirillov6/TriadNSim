using System;
using System.IO;
using System.Collections.Generic;

namespace TriadCompiler
    {
    /// <summary>
    /// ����� �������� �� ����������� ������ � �� ������.
    /// </summary>
    public class ErrorReg
        {
        /// <summary>
        /// ��� ����� � ������ ������
        /// </summary>
        private const string ErrorFileName = "ErrMessages.txt";
        /// <summary>
        /// ����������� ��������� ��� ������
        /// </summary>
        private const int MaxErrorCodeNumber = 1000;
        /// <summary>
        /// ������ ���������� ��������� �� ������
        /// </summary>
        private const string NoTextForErrorCodeMessage = "�� ���� ����� ��������� �� ������ � ����� �����.";
        /// <summary>
        /// �������� ������ ������ � ����� 
        /// </summary>
        private const string WrongErrorTextFormatMessage = "�������� ������ ������ � ����� � ����������� �� �������.";


        /// <summary>
        /// ����� ����� ������������������ ������
        /// </summary>
        public int ErrorCount
            {
            get
                {                
                return errorCount;
                }
            }


        /// <summary>
        /// �����������
        /// </summary>
        public ErrorReg()
            {
            //��������� ������ � ����������� �� �������
            FillErrMessagesList();
            }


        /// <summary>
        /// ��������
        /// </summary>
        public virtual void Reload()
            {
            errorCount = 0;
            }


        /// <summary>
        /// ���������������� ������
        /// </summary>
        /// <param name="errCode">��� ��������� �� ������</param>
        public virtual void Register( uint errCode )
            {
            errorCount++;

            //���� ��������� � ����� ����� ����
            if ( errList[ errCode ] != null )
                {
                Fabric.IO.ShowError( errCode.ToString() + ": " + errList[ errCode ] );
                }
            else
                {
                Fabric.IO.ShowError( errCode.ToString() + ": " + NoTextForErrorCodeMessage );
                }
            }


        /// <summary>
        /// ���������������� ������ � �������� � ��������� ������������ �����
        /// </summary>
        /// <param name="errCode">��� ��������� �� ������</param>
        /// <param name="additionalText">�������������� ����� ������</param>
        public virtual void Register( uint errCode, string additionalText )
            {
            if ( additionalText == null )
                throw new ArgumentNullException( "additionalText" );

            errorCount++;

            //���� ��������� � ����� ����� ����
            if ( errList[ errCode ] != null )
                {
                Fabric.IO.ShowError( errCode.ToString() + ": " + errList[ errCode ] + " ("
                    + additionalText + ")" );
                }
            else
                {
                Fabric.IO.ShowError( errCode.ToString() + ": " + NoTextForErrorCodeMessage );
                }
            }


        /// <summary>
        /// ���������������� ������ � ������� ��������� ���������� ��������
        /// </summary>
        /// <param name="errCode">��� ������</param>
        /// <param name="allowedEndKeys">��������� ���������� ��������</param>
        public virtual void Register( uint errCode, List<Key> allowedEndKeys )
            {
            errorCount++;

            //���� ��������� � ����� ����� ����
            if ( errList[ errCode ] != null )
                {
                string errorMessage = errCode.ToString() + ": " + errList[ errCode ];
                if ( this.printAllowedKeys )
                    {
                    errorMessage += ". ���������� �������: ";
                    foreach ( Key key in allowedEndKeys )
                        errorMessage += key.ToString() + " ";
                    }
                Fabric.IO.ShowError( errorMessage );
                }
            else
                {
                Fabric.IO.ShowError( errCode.ToString() + ": " + NoTextForErrorCodeMessage );
                }
            }


        /// <summary>
        /// ���������������� ������ � ������� ��������� ���������� ��������
        /// </summary>
        /// <param name="errCode">��� ������</param>
        /// <param name="keys">��������� ���������� ��������</param>
        public void Register( uint errCode, params Key[] keys )
            {
            List<Key> allowedKeys = new List<Key>();
            foreach ( Key key in keys )
                allowedKeys.Add( key );
            
            Register( errCode, allowedKeys );
            }

        
        /// <summary>
        /// ��������� ������ � ����������� �� �������
        /// </summary>
        private void FillErrMessagesList()
            {
            try
                {
                string[] messageList = Properties.Resources.ErrMessages.Split( '\n' );

                //��� ������
                int errorCode = 0;
                //��� ������ + ��������� �� ������
                string[] spliterString = new string[ 2 ];
                //�����������
                char[] delimiter = { ' ' };

                foreach ( string message in messageList )
                    {
                    spliterString = message.Split( delimiter, 2 );

                    errorCode = int.Parse( spliterString[ 0 ] );
                    //������� ������ �������� ������� � �����
                    errList[ errorCode ] = spliterString[ 1 ].Replace( '\r', ' ' );
                    }
                }
            catch ( FormatException e )
                {
                throw new FormatException( WrongErrorTextFormatMessage, e );
                }
            }


        /// <summary>
        /// ����������� �������� ��� ��� ��������� ���������� �������� ��� ������ ������
        /// </summary>
        public bool PrintAllowedKeys
            {
            get
                {
                return printAllowedKeys;
                }
            set
                {
                printAllowedKeys = value;
                }
            }


        //������ ��������� �� ������� (��� ������ - ��� �� ����� � ������)
        private string[] errList = new string[ MaxErrorCodeNumber ];
        /// <summary>
        /// ����� ����� ������������������ ������
        /// </summary>
        protected int errorCount = 0;
        /// <summary>
        /// ����������� �������� ��� ��� ��������� ���������� �������� ��� ������ ������
        /// </summary>
        private bool printAllowedKeys = CompilerFacade.ShowExtendedErrorInfo;
        }

    }
