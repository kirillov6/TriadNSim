using System;
using System.Collections.Generic;
using System.Text;

using TriadCompiler.Parser.Common.Var;
using TriadCompiler.Parser.Common.Expr;
using TriadCompiler.Parser.Common.Expr.Const;
using TriadCompiler.Parser.Common.Function;
using TriadCompiler.Parser.Common.Declaration.Var;

namespace TriadCompiler.Parser.Common.Expr.SimpleFact
    {
    /// <summary>
    /// ������ �������� ��������� � �������������� ���������
    /// </summary>
    internal class SimpleFactor : CommonParser
        {
        /// <summary>
        /// ��������� ��������� �������� �������� ���������
        /// </summary>
        private static List<Key> simpleFactorSet = null;
        /// <summary>
        /// ��������� ��������� �������� ���������
        /// </summary>
        private static List<Key> constantSet = null;
        
        /// <summary>
        /// ��������� ������� �������� ���������
        /// </summary>
        private static List<Key> SimpleFactorStartKeys
            {
            get
                {
                if ( simpleFactorSet == null )
                    {
                    Key[] keySet = { Key.Identificator, Key.LeftPar, Key.BitStringValue, Key.StringValue,
                            Key.CharValue, Key.RealValue, Key.IntegerValue, Key.BooleanValue, Key.Nil, Key.LeftBracket };

                    simpleFactorSet = new List<Key>( keySet );
                    }

                return simpleFactorSet;
                }
            }


        /// <summary>
        /// ��������� ������� ���������
        /// </summary>
        private static List<Key> ConstantStartKeys
            {
            get
                {
                if ( constantSet == null )
                    {
                    Key[] keySet = { Key.BitStringValue, Key.StringValue, Key.CharValue, Key.RealValue, 
                            Key.IntegerValue, Key.BooleanValue, Key.Nil };

                    constantSet = new List<Key>( keySet );
                    }
                return constantSet;
                }
            }


        /// <summary>
        /// ������� ���������
        /// </summary>
        /// <syntax>Constant | Variable | ( Expression )</syntax>
        /// <param name="endKeys">���������� �������� �������</param>
        /// <returns>���������� � ���������</returns>
        public static ExprInfo Parse( EndKeyList endKeys )
            {
            ExprInfo info = new ExprInfo();
            if ( !SimpleFactorStartKeys.Contains( currKey ) )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.WrongStartSymbol.SimpleFactor, SimpleFactorStartKeys );
                SkipTo( endKeys.UniteWith( SimpleFactorStartKeys ) );
                }

            if ( SimpleFactorStartKeys.Contains( currKey ) )
                {
                //���������
                if ( ConstantStartKeys.Contains( currKey ) )
                    {
                    info = Constant.Parse( endKeys );
                    }
                else if ( currKey == Key.Identificator )
                    {
                    string identName = ( currSymbol as IdentSymbol ).Name;
                    
                    //����� �������
                    if ( CommonArea.Instance.IsFunctionRegistered( identName ) )
                        {
                        FunctionInfo functionInfo = FunctionInvoke.Parse( endKeys );
                        info.Type = functionInfo.Type.ReturnedType;
                        info.Append( functionInfo.StrCode );
                        }
                    //����������
                    else
                        {
                        //=====By jum=====
                        VarInfo varInfo;
                        if (CommonArea.Instance.IsGraphRegistered( identName ))
                        {
                            varInfo = Variable.ParseGraphOrNode(endKeys);
                        }
                        else
                            varInfo = Variable.Parse( endKeys, false, true );
                        info.Type = varInfo.Type;
                        info.Append( varInfo.StrCode );
                        }
                    }
                //���� ��� ����������� ���������
                else if ( currKey == Key.LeftBracket )
                    {
                    info = ConstantSet.Parse( endKeys );
                    }
                //��������� � ������� ��� ���������� �����
                else if ( currKey == Key.LeftPar )
                    {
                    Accept( Key.LeftPar );

                    //���� ��� ���������� �����
                    if ( TypeDeclaration.SimpleTypeStartKeys.Contains( currKey ) )
                        {
                        info = TypeCast( endKeys );
                        }
                    //��������� � �������
                    else
                        {
                        info = Expression.Parse( endKeys.UniteWith( Key.RightPar ) );

                        info.InsertFirst( "(" );
                        info.Append( ")" );
                        Accept( Key.RightPar );
                        }

                    if ( !endKeys.Contains( currKey ) )
                        {
                        Fabric.Instance.ErrReg.Register( Err.Parser.WrongEndSymbol.ExprInPar );
                        SkipTo( endKeys );
                        }
                    }
                }
            return info;
            }


        /// <summary>
        /// ���������� �����
        /// </summary>
        /// <param name="endKeys">���������� �������� �������</param>
        /// <returns>���������� � ���������</returns>
        private static ExprInfo TypeCast( EndKeyList endKeys )
            {
            ExprInfo info = new ExprInfo();
            //������ �������� ��������� �������������
            info.Value = new ConstValue();

            //���, � �������� ���������� ���������
            VarType castType = TypeDeclaration.SimpleType( endKeys.UniteWith( Key.RightPar ) );

            Accept( Key.RightPar );

            ExprInfo exprInfo = Expression.Parse( endKeys );
            info.Append( exprInfo.StrCode );

            if ( castType != null && exprInfo.HasNoError )
                {
                if ( CheckCastTypes( castType, exprInfo ) )
                    {
                    info.Type = castType;

                    info.InsertFirst( "(" + CodeBuilder.GetBaseTypeString( castType ) + ")" );
                    }
                }

            return info;
            }


        /// <summary>
        /// ���������
        /// </summary>
        /// <param name="castType">���, � �������� ����� ��������</param>
        /// <param name="exprInfo">�������� ���������</param>
        /// <returns>True, ���� ���������� ���������</returns>
        private static bool CheckCastTypes( VarType castType, ExprInfo exprInfo )
            {
            IExprType exprType = exprInfo.Type;

            //���� ��������� - ��� ������
            if ( exprType is IndexedType )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.Usage.NeedNotIndexed );
                return false;
                }

            //���� ��������� - ��� ���������
            if ( exprType is SetType )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.Usage.NeedNotSet );
                return false;
                }

            //���� ��� ��������� �� ���������
            if ( exprType.Code == TypeCode.UndefinedType )
                //���� ��� �� �������� Nil
                if ( !( exprInfo.Value is NilValue ) )
                    {
                    // ...�� ��������� ����� ����������
                    return true;
                    }

            //���� ��������� ���������� � ��� ������������ ����
            if ( exprType.Code == castType.Code )
                {
                return true;
                }

            //���� ��������� ���������� � ��������������� ����
            if ( castType.Code == TypeCode.UndefinedType )
                {
                return true;
                }

            //���� ���� ���������� ����� ������������, �����, ������� ��� ����������
            if ( ( castType.Code == TypeCode.Real ||
                castType.Code == TypeCode.Integer ||
                castType.Code == TypeCode.Char ||
                castType.Code == TypeCode.Bit ) &&

                ( exprType.Code == TypeCode.Real ||
                exprType.Code == TypeCode.Integer ||
                exprType.Code == TypeCode.Char ||
                exprType.Code == TypeCode.Bit ) )
                {
                return true;
                }

            Fabric.Instance.ErrReg.Register( Err.Parser.Usage.UnableToCastType );
            return false;
            }

        }
    }
