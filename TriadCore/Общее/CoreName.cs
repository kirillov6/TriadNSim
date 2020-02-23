using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace TriadCore
    {
    /// <summary>
    /// ��� ��� ������������� ��������� ����
    /// </summary>
    public class CoreName : IComparable
        {
        /// <summary>
        /// �����������
        /// </summary>
        /// <param polusName="BaseName">��� �������</param>
        /// <param polusName="Index">������ � �������</param>
        public CoreName( string baseName, params int[] indexList )
            {
            if ( baseName == null )
                throw new ArgumentNullException( "������ ������� ���" );

            this.baseName = baseName;
            this.indexList = new List<int>( indexList );
            }


        /// <summary>
        /// �������� �� �������������� ���
        /// </summary>
        public bool IsIndexed
            {
            get
                {
                return ( this.indexList.Count != 0 );
                }
            }


        /// <summary>
        /// ������� ���
        /// </summary>
        public string BaseName
            {
            get
                {
                return baseName;
                }
            }


        /// <summary>
        /// �������
        /// </summary>
        public ReadOnlyCollection<int> Indices
            {
            get
                {
                return this.indexList.AsReadOnly();
                }
            }


        /// <summary>
        /// ������ ��������
        /// </summary>
        public int[] IndexArray
            {
            get
                {
                return indexList.ToArray();
                }
            }


        /// <summary>
        /// ���������� ������������� �����
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            {
            StringBuilder result = new StringBuilder( BaseName );
            if ( indexList.Count != 0 )
                {
                result.Append( "[" );

                for ( int index = 0 ; index < this.indexList.Count ; index++ )
                    {
                    result.Append( indexList[ index ].ToString() );
                    if ( index != this.indexList.Count - 1 )
                        result.Append( "," );
                    }
                result.Append( "]" );
                }

            return result.ToString();
            }


        /// <summary>
        /// ��������� ����
        /// </summary>
        /// <param name="obj">������ ���</param>
        /// <returns></returns>
        public override bool Equals( object obj )
            {
            if ( obj == null )
                throw new ArgumentNullException( "������� ������ ������" );

            CoreName otherName = obj as CoreName;

            //���� �������� ���� �� ���
            if ( otherName == null )
                return false;

            //���� ������� ����� �� ���������
            if ( this.baseName != otherName.baseName )
                return false;
            else
                {
                //���� ����������� �� ���������
                if ( this.indexList.Count != otherName.indexList.Count )
                    return false;

                //��������� ���������� ���� ��������
                for ( int index = 0; index < this.indexList.Count; index++ )
                    if ( this.indexList[ index ] != otherName.indexList[ index ] )
                        return false;

                return true;
                }
            }


        /// <summary>
        /// ��������� � ���������� ����
        /// </summary>
        /// <param name="coreNameRange">�������� ����</param>
        /// <returns></returns>
        public bool Equals( CoreNameRange coreNameRange )
            {
            if ( coreNameRange == null )
                throw new ArgumentNullException( "coreNameRange" );

            IEnumerator<CoreName> enumerator = coreNameRange.GetEnumerator();
            while ( enumerator.MoveNext() )
                if ( this.Equals( enumerator.Current ) )
                    {
                    return true;
                    }
            return false;
            }



        /// <summary>
        /// �������� ���������
        /// ��� ������� ������������ ��� �������� CoreName � SortedList
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo( object obj )
            {
            CoreName otherName = obj as CoreName;

            if ( otherName != null )
                {
                if ( this.baseName != otherName.baseName )
                    return this.baseName.CompareTo( otherName.baseName );
                else
                    {
                    for ( int index = 0 ; index < this.indexList.Count && index < otherName.indexList.Count ; index++ )
                        if ( this.indexList[ index ] != otherName.indexList[ index ] )
                            return this.indexList[ index ].CompareTo( otherName.indexList[ index ] );

                    return this.indexList.Count.CompareTo( otherName.indexList.Count );
                    }
                }
            else
                return -1;
            }


        /// <summary>
        /// ���
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            {
            int hashNameCode = this.baseName.GetHashCode();

            int indexHashCode = 0;
            foreach ( int index in this.indexList )
                indexHashCode ^= index.GetHashCode();

            return hashNameCode ^ indexHashCode;
            }


        /// <summary>
        /// ������� ���
        /// </summary>
        private string baseName = string.Empty;
        /// <summary>
        /// �������
        /// </summary>
        private List<int> indexList = new List<int>();
        }
    }
