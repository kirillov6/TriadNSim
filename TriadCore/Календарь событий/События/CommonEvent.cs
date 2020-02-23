using System;
using System.Collections.Generic;
using System.Text;

namespace TriadCore
    {
    /// <summary>
    /// ������� ������� ��� ������������ ������������
    /// </summary>
    public class CommonEvent : IComparable
        {
        /// <summary>
        /// ���������� ����� ������������ �������
        /// </summary>
        public const double MaxEventTime = double.MaxValue;


        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="executionTime">����� ������������ �������</param>
        /// <param name="routine">������, �� ������� ������ ��������� �������</param>
        public CommonEvent( double executionTime, Routine routine )
            {
            this.executionTime = executionTime;
            this.routine = routine;
            }


        /// <summary>
        /// ������� ���������� ������� �� �������
        /// </summary>
        public int CompareTo( Object obj )
            {
            CommonEvent calendarEvent = obj as CommonEvent;
            if ( calendarEvent != null )
                {
                return this.ExecutionTime.CompareTo( calendarEvent.ExecutionTime );
                }
            else
                throw new ArgumentException( "��� ������� ��������� ������������� ������ ��� �������" );
            }



        /// <summary>
        /// ����� ������������ �������
        /// </summary>
        /// <remarks>����� ������ ���� ���������������, 
        /// ����� ������������ ���������� ArgumentOutOfRangeException</remarks>
        /// <value>executionTime</value>
        public double ExecutionTime
            {
            set
                {
                if ( value >= 0 )
                    {
                    executionTime = value;
                    }
                else
                    throw new ArgumentOutOfRangeException( "value" );
                }
            get
                {
                return executionTime;
                }
            }


        /// <summary>
        /// ������� ���������� �������
        /// </summary>
        public virtual void ExecuteAllEventHandlers()
            {
            //�����
            }


        /// <summary>
        /// ������������ �������
        /// </summary>
        /// <returns>����� ������</returns>
        public CommonEvent Clone()
            {
            return this.MemberwiseClone() as CommonEvent;
            }


        /// <summary>
        /// �������� ���������
        /// </summary>
        /// <param name="obj">������ ������</param>
        /// <returns>��������� ���������</returns>
        public override bool Equals( object obj )
            {
            CommonEvent otherEvent = obj as CommonEvent;
            if ( otherEvent == null )
                return false;
            else
                return this.executionTime == otherEvent.executionTime;
            }


        /// <summary>
        /// ���
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            {
            return this.executionTime.GetHashCode();
            }


        /// <summary>
        /// �������� ������
        /// </summary>
        /// <param name="first">������ �������</param>
        /// <param name="second">������ �������</param>
        /// <returns>��������� ���������</returns>
        public static bool operator >( CommonEvent first, CommonEvent second )
            {
            if ( first == null )
                throw new ArgumentNullException( "first" );
            if ( second == null )
                throw new ArgumentNullException( "second" );

            return first.CompareTo( second ) > 0;
            }


        /// <summary>
        /// �������� ������
        /// </summary>
        /// <param name="first">������ �������</param>
        /// <param name="second">������ �������</param>
        /// <returns>��������� ���������</returns>
        public static bool operator <( CommonEvent first, CommonEvent second )
            {
            if ( first == null )
                throw new ArgumentNullException( "first" );
            if ( second == null )
                throw new ArgumentNullException( "second" );

            return first.CompareTo( second ) < 0;
            }


        /// <summary>
        /// ������, �� ������� ���������� �������
        /// </summary>
        protected Routine routine = null;
        /// <summary>
        /// ����� ������������ �������
        /// </summary>
        private double executionTime = 0;
        /// <summary>
        /// ������������������ ����������� ������� (����������� ����� ������������ ���������� �� ��)
        /// </summary>
        public SpyHandler EventSpyHandler = null;
        }
    }
