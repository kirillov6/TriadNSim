using System;
using System.Collections.Generic;
using System.Text;

using TriadCompiler.Parser.Common.Expr;

namespace TriadCompiler.Parser.Common.Expr.SimpleExpr
    {
    /// <summary>
    /// �������� ����� ��� �������� ��������� � �������������� ���������
    /// </summary>
    internal partial class SimpleExpression
        {
        /// <summary>
        /// ������������� ����� �� ������ Addend
        /// </summary>
        /// <param name="prevType">����������� ���</param>
        /// <param name="nextType">����������� ���</param>
        /// <param name="operation">��� ��������</param>
        /// <returns>�������������� ��� ����� ���������� ��������</returns>
        private static IExprType CheckTypeInAddend( IExprType prevType, IExprType nextType, Key operation )
            {
            //���� ���� �� ���� �� ����� ������ ������
            if ( prevType == null || nextType == null )
                return null;

            //���� ���� �� ���� �� ����� - ��� ������
            if ( prevType is VarArrayType || nextType is VarArrayType )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.Usage.NeedNotIndexed );
                return null;
                }

            //���� ��� ��������� - ��� ���������
            if ( prevType is SetType && nextType is SetType )
                {
                //���� �������� �����������
                if ( operation != Key.Plus && operation != Key.Minus )
                    {
                    Fabric.Instance.ErrReg.Register( Err.Parser.Type.Var.WrongType.InPlus );
                    return null;
                    }

                //���� ��� ������ �� ��-� �����������
                if ( prevType.Code == TypeCode.UndefinedType ||
                    nextType.Code == TypeCode.UndefinedType )
                    {
                    return new SetType( TypeCode.UndefinedType );
                    }
                
                //���� ���� �������� ���������
                if ( prevType.Code == nextType.Code ||
                    //��� ���� �� �������� ������
                         prevType is EmptySetType || nextType is EmptySetType )
                    {
                    //���� ������ ��������� ��������
                    if ( !( prevType is EmptySetType ) )
                        return prevType;
                    else
                        return nextType;
                    }

                Fabric.Instance.ErrReg.Register( Err.Parser.Type.Var.WrongType.InPlus );
                return null;
                }
            //by jum
            //�������� ��� ���������
            else if (prevType.Code == TypeCode.Node && nextType.Code == TypeCode.Node)
            {
                //���� �������� �����������
                if ( operation != Key.Plus && operation != Key.Minus )
                {
                   Fabric.Instance.ErrReg.Register( Err.Parser.Type.Var.WrongType.InPlus );
                   return null;
                }
                return new VarType( TypeCode.Node );
            }
            //���� ��� ��������� - �������
            else if ( !( prevType is SetType ) && !( nextType is SetType ) )
                {
                switch ( operation )
                    {
                    //��������
                    case Key.Plus:
                    //���������
                    case Key.Minus:
                        //����� ��� ������������ �����
                        if ( ( prevType.Code == TypeCode.Integer || prevType.Code == TypeCode.Real ) &&
                            ( nextType.Code == TypeCode.Integer || nextType.Code == TypeCode.Real ) )
                            {
                            //������ ����� �����
                            if ( prevType.Code == TypeCode.Integer && nextType.Code == TypeCode.Integer )
                                return new VarType( TypeCode.Integer );
                            else
                                return new VarType( TypeCode.Real );
                            }
                        //������������ ����� (�������� ������ + )
                        else if ( prevType.Code == TypeCode.String && nextType.Code == TypeCode.String && operation == Key.Plus )
                            {
                            return new VarType( TypeCode.String );
                            }
                        else
                            {
                            Fabric.Instance.ErrReg.Register( Err.Parser.Type.Var.WrongType.InPlus );
                            return null;
                            }
                        

                    //���������� ���
                    case Key.Or:
                        //Boolean | Boolean
                        if ( prevType.Code == TypeCode.Boolean && nextType.Code == TypeCode.Boolean )
                            {
                            return new VarType( TypeCode.Boolean );
                            }
                        //������� ������
                        else if ( prevType.Code == TypeCode.Bit && nextType.Code == TypeCode.Bit )
                            {
                            return new VarType( TypeCode.Bit );
                            }
                        else
                            {
                            Fabric.Instance.ErrReg.Register( Err.Parser.Type.Var.WrongType.InOrAnd );
                            return null;
                            }
                        
                    }
                }

            Fabric.Instance.ErrReg.Register( Err.Parser.Type.Var.WrongType.InPlus );
            return null;
            }


        /// <summary>
        /// ������������� ����� � �������� minus
        /// </summary>
        /// <param name="type">����������� ���</param>
        /// <returns>�������������� ��� ����� ���������� ��������</returns>
        private static IExprType CheckTypeInMinusAddend( IExprType type )
            {
            //���� ��� ������
            if ( type == null )
                return null;

            //���� ��� ���������
            if ( type is SetType )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.Usage.NeedNotSet );
                return null;
                }

            //���� ��� ������
            if ( type is VarArrayType )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.Usage.NeedNotIndexed );
                return null;
                }

            //���� ��� ������ � ������������ ����
            if ( type.Code != TypeCode.Integer && type.Code != TypeCode.Real )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.Usage.WrongMinusUsage );
                return null;
                }
            
            return type;
            }
        }
    }
