using System;
using System.Collections.Generic;
using System.Text;

namespace TriadCompiler.Parser.Common.Expr.Fact
    {
    /// <summary>
    /// �������� ����� ��� ��������� � �������������� ���������
    /// </summary>
    internal partial class Factor
        {
        /// <summary>
        /// ������������� ����� � �������� not
        /// </summary>
        /// <param name="type">����������� ���</param>
        /// <returns>�������������� ��� ����� ���������� ��������</returns>
        private static IExprType CheckTypeInNotFactor( IExprType type )
            {
            //���� ��� ������ ���
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

            //���� ��� ������ � ��� �� ���������� � �� �������
            if ( type.Code != TypeCode.Boolean && type.Code != TypeCode.Bit )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.Type.Var.Need.BooleanOrBit );
                return null;
                }
            return type;
            }


        /// <summary>
        /// ������������� ����� �� ������ SimpleFactor
        /// </summary>
        /// <param name="prevType">����������� ���</param>
        /// <param name="nextType">����������� ���</param>
        /// <returns>�������������� ��� ����� ���������� ��������</returns>
        private static IExprType CheckTypeInSimpleFactor( IExprType prevType, IExprType nextType )
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
            //���� ���� �� ���� �� ����� - ��� ���������
            if ( prevType is SetType || nextType is SetType )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.Usage.NeedNotSet );
                return null;
                }

            //���� ��� �� ����� ��� ������������
            if ( prevType.Code != TypeCode.Integer && prevType.Code != TypeCode.Real ||
                nextType.Code != TypeCode.Integer && nextType.Code != TypeCode.Real )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.Type.Var.WrongType.InArrow );
                return null;
                }

            //���� � ������� ���������� ����� �����
            if ( prevType.Code == TypeCode.Integer && nextType.Code == TypeCode.Integer )
                return new VarType( TypeCode.Integer );
            else
                return new VarType( TypeCode.Real );
            }
        }
    }
