using System;
using System.Collections.Generic;
using System.Text;

namespace TriadCore
    {
    /// <summary>
    /// ������� ����� ��� ���� ������-�������
    /// </summary>
    public class Design : ICondition
        {
        /// <summary>
        /// �����, ������������ �������������
        /// </summary>
        /// <returns>������ �� ������-����</returns>
        public virtual Graph Build()
            {
            return new Graph();
            }
        }
    }
