using System;
using System.Collections.Generic;
using System.Text;

namespace TriadCore
    {
    /// <summary>
    /// ������������� ��������
    /// </summary>
    public static class ArrayInitializer
        {
        /// <summary>
        /// �������������������
        /// </summary>
        /// <param name="array">���������������� ������</param>
        /// <param name="objectToClone">������, ������� �������� ������������ �������������</param>
        public static void Initialize( object array, ICreatable objectToClone )
            {
            if ( array is Array )
                {
                Array objectArray = array as Array;

                if ( objectArray.Length == 0 )
                    return;

                //������� ������
                List<int> currIndex = new List<int>();
                List<int> minIndex = new List<int>();
                //������ ������
                List<int> maxIndex = new List<int>();

                for ( int index = 0 ; index < objectArray.Rank ; index++ )
                    {
                    currIndex.Add( 0 );
                    minIndex.Add( 0 );
                    maxIndex.Add( objectArray.GetUpperBound( index ) );
                    }

                do
                    {
                    objectArray.SetValue( objectToClone.CreateNew(), currIndex.ToArray() );
                    }
                while ( CoreNameRange.IncrementIndex( currIndex, 0, minIndex, maxIndex ) );
                }
            }

        }
    }
