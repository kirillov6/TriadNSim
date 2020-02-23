using System;
using System.Collections.Generic;


namespace TriadCore
    {
    /// <summary>
    /// ���������� �������������� ���������
    /// </summary>
    public class IProcedure : ReflectionObject
        {
        /// <summary>
        /// ������ ������������������ � �� �������� ��������
        /// </summary>
        protected SortedList<CoreName, SpyObject> spyObjectList = new SortedList<CoreName, SpyObject>();


        /// <summary>
        /// ������� �������������
        /// </summary>
        public virtual void DoInitialize()
            {
            //�����
            }


        /// <summary>
        /// �������� ��������� ���������(�� ���������� � ������������ ���  
        /// ����������� �����������, �� ����� ��������� ��������� ��)
        /// </summary>
        /// <param name="objectInfo">������ ��������, �� ��������� �������� ���������</param>
        /// <param name="systemTime">��������� ����� �� �������</param>
        protected virtual void DoHandling( SpyObject objectInfo, double systemTime )
            {
            //�����
            }


        /// <summary>
        /// ���������� ������
        /// </summary>
        /// <param name="message">���������</param>
        protected void PrintMessage( object message )
            {
                if (message != null)
                {
                    Console.WriteLine("��: {0} \t {1} ", this, message);
                    Logger.Instance.AddRecord(new LoggerRecord(-1, "�� " + this.ToString(), message.ToString()));
                }
            }


        /// <summary>
        /// ���������������� ������ ��������
        /// </summary>
        /// <param name="objectInfo">������ ��������</param>
        /// <param name="formalName">���������� ��� �������</param>
        protected void RegisterSpyObject( SpyObject objectInfo, CoreName formalName )
            {
            if ( !this.spyObjectList.ContainsKey( formalName ) )
                {
                this.spyObjectList.Add( formalName, objectInfo );
                }
            }


        /// <summary>
        /// ���������������� �������� �������� ��������
        /// </summary>
        /// <param name="objectInfoArray">������ �������� ��������</param>
        /// <param name="formalNameRange">�������� ����</param>
        protected void RegisterSpyObject( SpyObject[] objectInfoArray, CoreNameRange formalNameRange )
            {
            for ( int index = 0 ; index < formalNameRange.ItemCount && index < objectInfoArray.Length ; index++ )
                RegisterSpyObject( objectInfoArray[ index ], formalNameRange[ index ] );

            //��������� spy-������, ���������� �� ����� ������
            //�� �����, ����� �������� �������� ����� ����� �������
            if ( objectInfoArray.Length > 0 && formalNameRange.ItemCount > 0 )
                {
                SpyObject arraySpyObject = objectInfoArray[ 0 ].Clone();
                arraySpyObject.RealName = new CoreName( arraySpyObject.RealName.BaseName );
                this.spyObjectList.Add( new CoreName( formalNameRange[ 0 ].BaseName ), arraySpyObject );
                }
            }


        /// <summary>
        /// �������� ������ ��������
        /// </summary>
        /// <param name="objectName">��� �������</param>
        /// <returns>������</returns>
        protected SpyObject GetSpyObject( CoreName objectName )
            {
            //���� ��� ������������������ ������
            if ( this.spyObjectList.ContainsKey( objectName ) )
                return this.spyObjectList[ objectName ];

            throw new ArgumentOutOfRangeException( "������ " + this +
                " �� �������� spy-������� " + objectName );
            }


        /// <summary>
        /// �������� �������� �������� ��������
        /// </summary>
        /// <param name="objectNameRange">��� ���������</param>
        /// <returns>������</returns>
        protected SpyObject[] GetSpyObject( CoreNameRange objectNameRange )
            {
            List<SpyObject> objectInfoList = new List<SpyObject>();

            foreach ( CoreName objectName in objectNameRange )
                {
                objectInfoList.Add( GetSpyObject( objectName ) );
                }

            return objectInfoList.ToArray();
            }


        /// <summary>
        /// �������� �������� spy-���������� (������� ��� in/passive)
        /// </summary>
        /// <param name="varName">��� ����������</param>
        protected object GetSpyVarValue( CoreName varName )
            {
            //���� ��� ������ ��������
            if ( this.spyObjectList.ContainsKey( varName ) )
                {
                SpyVar spyVarInfo = this.spyObjectList[ varName ] as SpyVar;

                //���� ������ �������� - ��� ����������
                if ( spyVarInfo != null )
                    {
                    return spyVarInfo.Value;
                    }
                }
            

            return null;
            }


        /// <summary>
        /// ������ �������� spy-����������
        /// </summary>
        /// <param name="varName">��� ����������</param>
        /// <param name="value">��������</param>
        protected void SetSpyVarValue( CoreName varName, object value )
            {
            //���� ��� ������ ��������
            if ( this.spyObjectList.ContainsKey( varName ) )
                {
                SpyVar spyVarInfo = this.spyObjectList[ varName ] as SpyVar;

                //���� ������ �������� - ��� ����������
                if ( spyVarInfo != null )
                    {
                    spyVarInfo.Value = value;
                    }
                }
            }


        /// <summary>
        /// ������������� �����
        /// </summary>
        /// <param name="polusName">��� ������</param>
        protected void BlockPolus( CoreName polusName )
            {
            //���� ���� ����� ������ ��������
            if ( this.spyObjectList.ContainsKey( polusName ) )
                {
                SpyPolus spyPolusInfo = this.spyObjectList[ polusName ] as SpyPolus;

                //���� ��� �����
                if ( spyPolusInfo != null )
                    {
                    spyPolusInfo.BlockPolus();
                    }
                }
            }


        /// <summary>
        /// ������������� �������� �������
        /// </summary>
        /// <param name="polusNameRange">��������</param>
        protected void BlockPolus( CoreNameRange polusNameRange )
            {
            foreach ( CoreName polusName in polusNameRange )
                BlockPolus( polusName );
            }



        /// <summary>
        /// �������������� �����
        /// </summary>
        /// <param name="polusName">��� ������</param>
        protected void UnblockPolus( CoreName polusName )
            {
            //���� ���� ����� ������ ��������
            if ( this.spyObjectList.ContainsKey( polusName ) )
                {
                SpyPolus spyPolusInfo = this.spyObjectList[ polusName ] as SpyPolus;

                //���� ��� �����
                if ( spyPolusInfo != null )
                    {
                    spyPolusInfo.UnblockPolus();
                    }
                }
            }



        /// <summary>
        /// �������������� �������� �������
        /// </summary>
        /// <param name="polusNameRange">��������</param>
        protected void UnblockPolus( CoreNameRange polusNameRange )
            {
            foreach ( CoreName polusName in polusNameRange )
                UnblockPolus( polusName );
            }
        }
    }
