using System;
using System.Collections.Generic;
using System.Text;

namespace TriadCore
    {
    /// <summary>
    /// ���������� ����� ���� �������
    /// </summary>
    public abstract class PathGraph : Graph
        {
        /// <summary>
        /// �����������
        /// </summary>
        protected PathGraph()
            : base( new CoreName( "����������� ���������� ����" ) )
            {
            }


        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="coreName">��� �����</param>
        protected PathGraph( CoreName coreName )
            : base( coreName )
            {
            }


        /// <summary>��������� ���� ����������������� ������� � ������
        /// ������ ����������� ������ ����� ������� ������� �
        /// ������ ������� ��������� �������
        /// </summary>
        public override void CompleteGraph()
            {
            Node nodePrev = null;
            Node nodeCurr = null;

            for ( int i = 1; i < this.NodeCount; i++ )
                {
                nodePrev = this[ i - 1 ];
                nodeCurr = this[ i ];
                AddConnection( nodePrev.Name, nodePrev[ 1 ].Name,
                              nodeCurr.Name, nodeCurr[ 0 ].Name );
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
    /// ���������� ����� ���� �������������� �������
    /// </summary>
    public class UndirectedPathGraph : PathGraph
        {
        /// <summary>
        /// �����������
        /// </summary>
        public UndirectedPathGraph()
            : base( new CoreName( "����������� �������������� ���������� ����" ) )
            {
            }


        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="coreName">��� �����</param>
        public UndirectedPathGraph( CoreName coreName )
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
    /// ���������� ����� ���� ������������ �������
    /// </summary>
    public class DirectedPathGraph : PathGraph
        {
        /// <summary>
        /// �����������
        /// </summary>
        public DirectedPathGraph()
            : base( new CoreName( "����������� ������������ ���������� ����" ) )
            {
            }


        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="coreName">��� �����</param>
        public DirectedPathGraph( CoreName coreName )
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
