using System;
using System.Collections.Generic;
using System.Text;

namespace TriadCompiler
    {
    /// <summary>
    /// ����������������� ���
    /// </summary>
    /// <typeparam name="ItemType">��� ��������</typeparam>
    public class ParameterList<ItemType>
        {
        /// <summary>
        /// �������� ��������
        /// </summary>
        /// <param name="paramType">��� ���������</param>
        public void AddParameter( ItemType paramType )
            {
            this.paramList.Add( paramType );
            }


        /// <summary>
        /// �������� ������ ����������
        /// </summary>
        /// <param name="paramTypeList">������ ����� ����������</param>
        public void AddParameterList( List<ItemType> paramTypeList )
            {
            this.paramList.AddRange( paramTypeList );
            }


        /// <summary>
        /// �������� ������� ����������
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ItemType> GetEnumerator()
            {
            return this.paramList.GetEnumerator();
            }


        /// <summary>
        /// ����� ����������
        /// </summary>
        public int ParameterCount
            {
            get
                {
                return this.paramList.Count;
                }
            }

        //by jum
        public List<ItemType> ToList()
        {
            return paramList;
        }

        /// <summary>
        /// ������ ����������
        /// </summary>
        private List<ItemType> paramList = new List<ItemType>();
        }
    }
