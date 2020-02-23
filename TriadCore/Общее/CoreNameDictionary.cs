using System;
using System.Collections.Generic;
using System.Text;

namespace TriadCore
    {
    /// <summary>
    /// ����������� �������
    /// </summary>
    /// <typeparam name="TKey">����</typeparam>
    /// <typeparam name="TValue">��������</typeparam>
    [Serializable]
    public class CoreNameDictionary<TKey, TValue> : Dictionary<TKey, TValue>
        where TValue : class
        {
        /// <summary>
        /// �����������
        /// </summary>
        public CoreNameDictionary()
            {
            }



        /// <summary>
        /// �������� �������� �� �������
        /// </summary>
        /// <param name="index">������</param>
        /// <returns>������� ��������</returns>
        public TValue this[ int index ]
            {
            get
                {
                if ( index < 0 || index >= this.Count )
                    throw new ArgumentOutOfRangeException( "index" );

                int indexCurr = 0;
                TValue resultItem = null;

                foreach ( TValue currItem in this.Values )
                    {
                    if ( indexCurr == index )
                        {
                        resultItem = currItem;
                        break;
                        }
                    indexCurr++;
                    }

                return resultItem;
                }
            }
        }
    }
