using System;
using System.Collections.Generic;
using System.Text;

namespace TriadCore
    {
    /// <summary>
    /// �������� ���� ��������� ����
    /// </summary>
    public class CoreNameRange
        {
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="arrayName">��� ������� ����</param>
        /// <param name="firstLastIndexList">������ � ������� ������� ��������</param>
        public CoreNameRange( string arrayName, params int[] firstLastIndexList )
            {
            if ( ( firstLastIndexList.Length % 2 ) != 0 )
                throw new ArgumentException( "�� ������� ���������� �������� �������" );

            List<int> lowIndexList = new List<int>();
            List<int> highIndexList = new List<int>();

            for ( int paramIndex = 0 ; paramIndex < firstLastIndexList.Length ; paramIndex += 2 )
                {
                int lowIndex = firstLastIndexList[ paramIndex ];
                int highIndex = firstLastIndexList[ paramIndex + 1 ];

                if ( lowIndex < 0 )
                    throw new ArgumentOutOfRangeException( "������ ������ ��������� " +
                        arrayName + " �� ����� ���� ������ ����" );
                if ( lowIndex > highIndex )
                    throw new ArgumentOutOfRangeException( "������ ������ ��������� " +
                        arrayName + " �� ����� ���� ������ ��������" );

                lowIndexList.Add( lowIndex );
                highIndexList.Add( highIndex );
                }

            List<int> currIndex = new List<int>( lowIndexList );

            do
                {
                this.coreNameList.Add( new CoreName( arrayName, currIndex.ToArray() ) );
                }
            while ( IncrementIndex( currIndex, 0, lowIndexList, highIndexList ) );
            }


        /// <summary>
        /// ��������� ������� ������
        /// </summary>
        /// <param name="currIndex">������� ������</param>
        /// <param name="currIndexNumber">����� ����������� �������</param>
        /// <param name="lowIndex">������ ������</param>
        /// <param name="highIndex">������� ������</param>
        /// <returns>True, ���� ������ ������� �������</returns>
        public static bool IncrementIndex( List<int> currIndex, int currIndexNumber, List<int> lowIndex, List<int> highIndex )
            {
            if ( currIndexNumber >= currIndex.Count )
                return false;

            if ( currIndex[ currIndexNumber ] < highIndex[ currIndexNumber ] )
                {
                currIndex[ currIndexNumber ]++;
                return true;
                }
            else
                {
                currIndex[ currIndexNumber ] = lowIndex[ currIndexNumber ];
                return IncrementIndex( currIndex, currIndexNumber + 1, lowIndex, highIndex );
                }
            }


        /// <summary>
        /// �������� ������������� ���� �� ���������
        /// </summary>
        /// <returns></returns>
        public IEnumerator<CoreName> GetEnumerator()
            {
            return this.coreNameList.GetEnumerator();
            }


        /// <summary>
        /// ����� ���� � ���������
        /// </summary>
        public int ItemCount
            {
            get
                {
                return this.coreNameList.Count;
                }
            }


        /// <summary>
        /// ��������� ����� ��������� �� �������
        /// </summary>
        /// <param name="index">������</param>
        /// <returns>���</returns>
        public CoreName this[ int index ]
            {
            get
                {
                if ( index < 0 || index >= this.coreNameList.Count )
                    throw new ArgumentOutOfRangeException( "������ ����� �� ������� ���������" );

                return this.coreNameList[ index ];
                }
            }


        /// <summary>
        /// ����� ��������� ���������� � ��������
        /// </summary>
        private List<CoreName> coreNameList = new List<CoreName>();
        }
    }
