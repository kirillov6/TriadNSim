using System;
using System.Collections.Generic;
using System.Text;

namespace TriadCore
    {
    /// <summary>
    /// ������� ��������� ���������
    /// </summary>
    class ReceivingMessageEvent : CommonEvent
        {
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="executionTime">����� ������������ �������</param>
        /// <param name="routine">������, �� ������� ���������� �������</param>
        /// <param name="routinePolusName">��� ������ ������, ���������� ���������</param>
        /// <param name="nodePolusName">��� ������ �������, ���������� ���������</param>
        /// <param name="message">��������� ���������</param>
        public ReceivingMessageEvent( double executionTime, Routine routine, CoreName routinePolusName, CoreName nodePolusName, string message )
            : base( executionTime, routine )
            {
            this.routinePolusName = routinePolusName;
            this.nodePolusName = nodePolusName;
            this.message = message;
            }


        /// <summary>
        /// ������� ���������� �������
        /// </summary>
        public override void ExecuteAllEventHandlers()
            {
            if ( this.OnEventFunction != null )
                {
                this.OnEventFunction( this.routinePolusName, this.nodePolusName, this.message, this.EventSpyHandler );
                }
            }


        /// <summary>
        /// ������������������ ����������� �������
        /// </summary>
        public ReceivingMessageEventHandler OnEventFunction = null;
        /// <summary>
        /// ��� ������ ������, ���������� ���������
        /// </summary>
        private CoreName routinePolusName;
        /// <summary>
        /// ��� ������ �������, ���������� ���������
        /// </summary>
        private CoreName nodePolusName;
        /// <summary>
        /// ��������� ���������
        /// </summary>
        private string message;
        }
    }
