using System;
using System.Collections.Generic;
using System.Text;

namespace TriadCore
    {
    /// <summary>
    /// ���������� ����� ���� ������
    /// </summary>
    public abstract class StarGraph : Graph
        {
        /// <summary>
        /// �����������
        /// </summary>
        protected StarGraph()
            : base( new CoreName( "����������� ���� ���� ������" ) )
            {
            }


        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="coreName">��� �����</param>
        protected StarGraph( CoreName coreName )
            : base( coreName )
            {
            }


        /// <summary>
        /// ��������� ����� "�� ���������".
        /// ����� ������������� �� ������� ������ ������
        /// ������� � ������� ������ ������ ��������� �������
        /// </summary>
        public override void CompleteGraph()
            {
            for ( int i = 1; i < this.NodeCount; i++ )
                {
                AddConnection( this[ 0 ].Name, this[ 0 ][ 0 ].Name,
                               this[ i ].Name, this[ i ][ 0 ].Name );
                }

            base.CompleteGraph();
            }


        /// <summary>
        /// ���������� "������" ����������
        /// </summary>
        /// <param name="nodeName1">��� ������ �������</param>
        /// <param name="polusName1">��� ������� ������</param>
        /// <param name="nodeName2">��� ������ �������</param>
        /// <param name="polusName2">��� ������� ������</param>
        protected virtual void AddConnection( CoreName nodeName1, CoreName polusName1, CoreName nodeName2, CoreName polusName2 )
            {
            return;
            }
        }


    /// <summary>
    /// ���������� ����� ���� �������������� ������
    /// </summary>
    public class UndirectedStarGraph : StarGraph
        {
        /// <summary>
        /// �����������
        /// </summary>
        public UndirectedStarGraph()
            : base( new CoreName( "����������� �������������� ���� ���� ������" ) )
            {
            }


        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="coreName">��� �����</param>
        public UndirectedStarGraph( CoreName coreName )
            : base( coreName )
            {
            }


        /// <summary>
        /// ���������� "������" ����������
        /// </summary>
        /// <param name="nodeName1">��� ������ �������</param>
        /// <param name="polusName1">��� ������� ������</param>
        /// <param name="nodeName2">��� ������ �������</param>
        /// <param name="polusName2">��� ������� ������</param>
        protected override void AddConnection( CoreName nodeName1, CoreName polusName1, CoreName nodeName2, CoreName polusName2 )
            {
            this.AddEdge( nodeName1, polusName1, nodeName2, polusName2 );
            }
        }


    /// <summary>
    /// ���������� ����� ���� ������������ ������
    /// </summary>
    public class DirectedStarGraph : StarGraph
        {
        /// <summary>
        /// �����������
        /// </summary>
        public DirectedStarGraph()
            : base( new CoreName( "����������� ������������ ���� ���� ������" ) )
            {
            }


        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="coreName">��� �����</param>
        public DirectedStarGraph( CoreName coreName )
            : base( coreName )
            {
            }


        /// <summary>
        /// ���������� "������" ����������
        /// </summary>
        /// <param name="nodeName1">��� ������ �������</param>
        /// <param name="polusName1">��� ������� ������</param>
        /// <param name="nodeName2">��� ������ �������</param>
        /// <param name="polusName2">��� ������� ������</param>
        protected override void AddConnection( CoreName nodeName1, CoreName polusName1, CoreName nodeName2, CoreName polusName2 )
            {
            this.AddArc( nodeName1, polusName1, nodeName2, polusName2 );
            }
        }
    }
