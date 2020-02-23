using System;
using System.Collections.Generic;
using System.Text;

namespace TriadCompiler.Parser.Structure.StructExpr.Fact
    {
    /// <summary>
    /// ��������� ��������, ��������� �� ����������� ����������
    /// </summary>
    internal static class FactorKeySet
        {
        /// <summary>
        /// ��������� ��������� ��������
        /// </summary>
        public static class Start
            {
            /// <summary>
            /// ��������� ��������� �������� ������������ ���������
            /// </summary>
            private static List<Key> structFactorSet = null;


            /// <summary>
            /// ��������� ������� ������������ ���������
            /// </summary>
            public static List<Key> Factor
                {
                get
                    {
                    if ( structFactorSet == null )
                        {
                        Key[] keySet = { Key.Node, Key.Identificator, Key.LeftPar, Key.Arc, Key.Edge,
                            Key.DirectCycle, Key.DirectPath, Key.DirectStar, 
                            Key.UndirectCycle, Key.UndirectPath, Key.UndirectStar };

                        structFactorSet = new List<Key>( keySet );
                        }

                    return structFactorSet;
                    }
                }
            }
        }
    }
