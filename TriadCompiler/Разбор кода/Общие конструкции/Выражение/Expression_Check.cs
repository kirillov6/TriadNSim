using System;
using System.Collections.Generic;
using System.Text;

namespace TriadCompiler.Parser.Common.Expr
    {
    /// <summary>
    /// ������������� �������� �������������� ���������
    /// </summary>
    internal partial class Expression : CommonParser
        {
        /// <summary>
        /// ��������� ���������� ����� � ��������� ���������
        /// </summary>
        private static List<TypeCode> relationOpTypeSet = null;


        /// <summary>
        /// ���������� ���� � ��������� ���������
        /// </summary>
        private static List<TypeCode> RelOpTypes
            {
            get
                {
                if ( relationOpTypeSet == null )
                    {
                    TypeCode[] keySet = { TypeCode.Boolean, TypeCode.Char, TypeCode.Integer, 
                        TypeCode.Real, TypeCode.String, TypeCode.Bit, TypeCode.UndefinedType };

                    relationOpTypeSet = new List<TypeCode>( keySet );
                    }
                return relationOpTypeSet;
                }
            }


        /// <summary>
        /// ������������� ����� �� ������ SimpleExpression
        /// Notype ��������� � ����� �����
        /// </summary>
        /// <param name="prevType">����������� ���</param>
        /// <param name="nextType">����������� ���</param>
        /// <param name="operation">��� ��������</param>
        /// <returns>�������������� ��� ����� ���������� ��������</returns>
        private static IExprType CheckTypeInSimpleExpression( IExprType prevType, IExprType nextType, Key operation )
            {
            //���� ���� �� ���� �� ����� ������
            if ( prevType == null || nextType == null )
                return null;

            //���� ���� �� ���� �� ����� - ��� ������
            if ( prevType is VarArrayType || nextType is VarArrayType )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.Usage.NeedNotIndexed );
                return null;
                }

            bool typesAreCompatible = false;

            //���� ��� ��������� - ���������
            if ( prevType is SetType && nextType is SetType )
                {
                //���� �������� ��� ����������� �� �������� ����������
                if ( ! new List<Key>( new[] { Key.Equal, Key.NotEqual, Key.LaterEqual, Key.GreaterEqual } ).Contains( operation ) )
                    {                    
                    }
                //���� � �������� ������������ ���
                else if ( prevType.Code != nextType.Code )
                    {
                    //���� � ������ �� ��-� �������������� ���
                    if ( prevType.Code == TypeCode.UndefinedType ||
                        nextType.Code == TypeCode.UndefinedType )
                        {
                        typesAreCompatible = true;
                        }
                    //���� ���� �� �������� ��������
                    else if ( prevType is EmptySetType || nextType is EmptySetType )
                        {
                        typesAreCompatible = true;
                        }
                    }
                else
                    typesAreCompatible = true;
                }
            //���� ������ �������� - ��� ������, � ������ - ��� ���������
            else if ( !( prevType is SetType ) && nextType is SetType )
                {
                //���� �������� �� �������� ����������
                if ( operation != Key.In )
                    {
                    }
                //���� � ��������� � ������� ������������ ���
                else if ( prevType.Code != nextType.Code )
                    {
                    //���� ��������� ������
                    if ( nextType is EmptySetType )
                        {
                        typesAreCompatible = true;
                        }
                    //���� ��� ��������� �� ���������
                    if ( nextType.Code == TypeCode.UndefinedType )
                        {
                        typesAreCompatible = true;
                        }
                    //���� ����� ����� ����������� �� ��������� � ������������ ��-��
                    else if ( prevType.Code == TypeCode.Integer &&
                        nextType.Code == TypeCode.Real )
                        {
                        typesAreCompatible = true;
                        }
                    }
                else
                    typesAreCompatible = true;
                }
            //���� ��� ��������� - ��� �������
            else if ( !( prevType is SetType ) && !( nextType is SetType ) )
                {
                //���� ���� ��������� ������������
                if ( !RelOpTypes.Contains( prevType.Code ) ||
                     !RelOpTypes.Contains( nextType.Code ) )
                    {
                    }
                //���� �������� �����������
                else if ( operation == Key.In )
                    {
                    }
                //���� ���� ��������� �� ���������
                else if ( prevType.Code != nextType.Code )
                    {
                    //���� ��� ���������� ������ � �������������
                    if ( ( prevType.Code == TypeCode.Integer && nextType.Code == TypeCode.Real ) ||
                        ( nextType.Code == TypeCode.Integer && prevType.Code == TypeCode.Real ) )
                        {
                        typesAreCompatible = true;
                        }
                    }
                //���� ��� ��������� ����� ���������� ���
                else if ( prevType.Code == TypeCode.Boolean )
                    {
                    //� �������� ����������
                    if ( operation == Key.Equal || operation == Key.NotEqual )
                        typesAreCompatible = true;
                    }
                //���� ��� ��������� ����� �������������� ���
                else if ( prevType.Code == TypeCode.UndefinedType )
                    {
                    //� �������� ����������
                    if ( operation == Key.Equal || operation == Key.NotEqual )
                        typesAreCompatible = true;
                    }
                else
                    typesAreCompatible = true;
                }


            if ( typesAreCompatible )
                return new VarType( TypeCode.Boolean );
            else
                {
                //������������� ����
                Fabric.Instance.ErrReg.Register( Err.Parser.Type.Var.WrongType.InSimpleExpr );

                return null;
                }
            }

        }
    }
