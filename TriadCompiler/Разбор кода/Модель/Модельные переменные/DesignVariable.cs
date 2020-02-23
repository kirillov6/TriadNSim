using System;
using System.Collections.Generic;
using System.Text;

using TriadCompiler.Parser.Common.ObjectRef;
using TriadCompiler.Parser.Common.Expr;

namespace TriadCompiler.Parser.Model.DesignVar
    {
    /// <summary>
    /// ������ ������-����������
    /// </summary>
    internal class DesignVariable : CommonParser
        {
        /// <summary>
        /// ��������� � design ����������
        /// </summary>
        /// <param name="endKeys">��������� �������� ��������</param>
        /// <param name="expectedTypeCode">��������� ��� design ����������</param>
        /// <syntax>Identificator #[ Expression {, Expression } ]#</syntax>
        /// <returns>�������� ����������</returns>
        public static ObjectRefInfo Parse( EndKeyList endKeys, DesignTypeCode expectedTypeCode )
            {
            IDesignVarType designVarType = null;

            ObjectRefInfo objRef = ObjectReference.Parse( endKeys, true,
                delegate( string varName, ExprInfo indexExpr, int indexNumber )
                    {
                    //���� ��� �� ���� ���������
                    if ( varName == string.Empty )
                        return;

                    if ( designVarType == null )
                        {
                        //�������� ��� ����������
                        designVarType = CommonArea.Instance.GetType<IDesignVarType>( varName );
                        
                        //���� ��� ����� ����������� ���
                        if ( designVarType != null )
                            if ( designVarType.TypeCode != expectedTypeCode )
                                {
                                Fabric.Instance.ErrReg.Register( Err.Parser.Usage.DesignVar.NotExpectedTypeCode );
                                }
                        }

                    //���� ���������� ����� ��������������� ���
                    if ( designVarType is DesignVarArrayType )
                        {
                        DesignVarArrayType varArrayType = designVarType as DesignVarArrayType;
                        //���� ������ ����� ����������� ���������� � ������ �������� ����������
                        if ( indexExpr.Value.IsConstant && indexNumber < varArrayType.IndexCount )
                            {
                            //�������� ��������� � �������
                            if ( !varArrayType.IsValidIndex( ( indexExpr.Value as IntegerValue ).Value, indexNumber ) )
                                {
                                Fabric.Instance.ErrReg.Register( Err.Parser.Usage.Array.OutOfArrayBound );
                                }
                            }
                        }
                    } );

            //�������� ��� ������
            if ( designVarType == null )
                if ( objRef.Name != string.Empty )
                    designVarType = CommonArea.Instance.GetType<IDesignVarType>( objRef.Name );
                else
                    designVarType = null;

            //��������� ����� �������� � ����������
            ObjectReference.CheckIndexCount( designVarType, objRef, false );

            return objRef;
            }
        }
    }
