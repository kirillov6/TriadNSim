using System;
using System.Collections.Generic;
using System.Text;

using TriadCompiler.Parser.Common.Expr;

namespace TriadCompiler.Parser.Common.Declaration.Var
    {
    /// <summary>
    /// ����� ���������� ���� ����������
    /// </summary>
    internal class TypeDeclaration : CommonParser
        {
        /// <summary>
        /// ��������� ��������� �������� ����
        /// </summary>
        private static List<Key> typeSet = null;
        /// <summary>
        /// ��������� ��������� �������� �������� ����
        /// </summary>
        private static List<Key> simpleTypeSet = null;


        /// <summary>
        /// ��������� ������� �������� ����
        /// </summary>
        public static List<Key> SimpleTypeStartKeys
            {
            get
                {
                if ( simpleTypeSet == null )
                    {

                    Key[] keySet = { Key.Bit, Key.String, Key.Boolean, Key.Real, Key.Char, Key.Integer, Key.Notype };

                    simpleTypeSet = new List<Key>( keySet );
                    }
                return simpleTypeSet;
                }
            }


        /// <summary>
        /// ��������� ������� ����
        /// </summary>
        public static List<Key> TypeStartKeys
            {
            get
                {
                if ( typeSet == null )
                    {
                    typeSet = new List<Key>();

                    typeSet.Add( Key.Array );
                    typeSet.Add( Key.Set );
                    //by jum
                    typeSet.Add( Key.Node );
                    typeSet.Add( Key.Graph );
                    typeSet.AddRange( SimpleTypeStartKeys );
                    }
                return typeSet;
                }
            }


        /// <summary>
        /// ���
        /// </summary>
        /// <syntax>SimpleType | Array [ ArrayIndexRange { ,ArrayIndexRange } ] Of SimpleType |
        /// SET OF SimpleType </syntax>
        /// <param name="endKeys">��������� ���������� �������� ��������</param>
        /// <returns>�������������� ���</returns>
        public static IExprType Parse( EndKeyList endKeys )
            {
            IExprType resultType = null;

            if ( !TypeDeclaration.TypeStartKeys.Contains( currKey ) )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.WrongStartSymbol.Type, TypeDeclaration.TypeStartKeys );
                SkipTo( endKeys.UniteWith( TypeDeclaration.TypeStartKeys ) );
                }
            if ( TypeDeclaration.TypeStartKeys.Contains( currKey ) )
                {
                //������
                if ( currKey == Key.Array )
                    {
                    Accept( Key.Array );                    

                    VarArrayType arrayType = new VarArrayType( TypeCode.UndefinedType );

                    RangeDeclaration( endKeys, arrayType );
                    
                    Accept( Key.Of );

                    VarType simpleType = SimpleType( endKeys );
                    if ( simpleType != null )
                        arrayType.Code = simpleType.Code;

                    resultType = arrayType;
                    }
                //���� ��� ���������
                else if ( currKey == Key.Set )
                    {
                    Accept( Key.Set );
                    Accept( Key.Of );

                    SetType setType = new SetType( TypeCode.UndefinedType );

                    //by jum
                    if (currKey == Key.Node)
                    {
                        setType.Code = TypeCode.Node;
                        GetNextKey();
                    }
                    else
                        setType.Code = SimpleType( endKeys ).Code;
                    
                    resultType = setType;
                    }
                //by jum
                else if (currKey == Key.Node)
                    {
                        Accept( Key.Node );
                        resultType = new VarType(TypeCode.Node);
                    }
                //������� ���
                else
                {
                    resultType = SimpleType(endKeys);
                }
                }

            return resultType;
            }


        /// <summary>
        /// ���������� ��������� � �������
        /// </summary>
        /// <param name="endKeys">���������� �������� �������</param>
        /// <param name="arrayType">����������� ��� �������</param>
        public static void RangeDeclaration( EndKeyList endKeys, IndexedType arrayType )
            {
            Accept( Key.LeftBracket );

            ArrayIndex( endKeys.UniteWith( Key.Comma, Key.RightBracket ), arrayType );
            while ( currKey == Key.Comma )
                {
                GetNextKey();
                ArrayIndex( endKeys.UniteWith( Key.Comma, Key.RightBracket ), arrayType );
                }

            Accept( Key.RightBracket );
            }


        /// <summary>
        /// �������� �������� ������� � �������
        /// </summary>
        /// <syntax>Expression</syntax>
        /// <param name="endKeys">��������� ���������� �������� ��������</param>
        /// <param name="arrayType">�������������� ���</param>
        /// <syntax> Expression </syntax>
        /// <returns>������� ������</returns>
        private static void ArrayIndex( EndKeyList endKeys, IndexedType arrayType )
            {
            ExprInfo exprInfo = Expression.Parse( endKeys );

            //������ ���
            if ( !CheckIndexInArrayDeclaration( exprInfo ) )
                {
                arrayType.SetNewIndex( ( exprInfo.Value as IntegerValue ).Value );
                }

            }


        /// <summary>
        /// ��������� ������ � ���������� �������
        /// </summary>
        /// <param name="exprInfo">���������� �� �������</param>
        /// <returns>True, ���� ������� ������</returns>
        private static bool CheckIndexInArrayDeclaration( ExprInfo exprInfo )
            {
            bool errorFound = !exprInfo.HasNoError;
            errorFound |= !exprInfo.IsNotIndexed();
            errorFound |= !exprInfo.IsNotSet();
            //��� ������� ������ ���� �����
            errorFound |= !exprInfo.IsInteger();
            //��� ������� ������ ���� ����������
            errorFound |= !exprInfo.IsConstant();
            //���������, �������� ������� ������ ������� ������ ���� �������������
            errorFound |= !exprInfo.PositiveIntegerOrReal();

            return errorFound;
            }


        /// <summary>
        /// ������� ���
        /// </summary>
        /// <syntax>Bit | String | Char | Integer | Real | Boolean | Notype</syntax>
        /// <param name="endKeys">��������� ���������� �������� ��������</param>
        /// <returns>�������������� ���</returns>
        public static VarType SimpleType( EndKeyList endKeys )
            {
            VarType resultType = null;

            if ( !TypeDeclaration.SimpleTypeStartKeys.Contains( currKey ) )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.WrongStartSymbol.Type, SimpleTypeStartKeys );
                SkipTo( endKeys.UniteWith( SimpleTypeStartKeys ) );
                }
            if ( SimpleTypeStartKeys.Contains( currKey ) )
                {
                switch ( currKey )
                    {
                    case Key.String:
                        resultType = new VarType( TypeCode.String );
                        GetNextKey();
                        break;
                    case Key.Integer:
                        resultType = new VarType( TypeCode.Integer );
                        GetNextKey();
                        break;
                    case Key.Char:
                        resultType = new VarType( TypeCode.Char );
                        GetNextKey();
                        break;
                    case Key.Real:
                        resultType = new VarType( TypeCode.Real );
                        GetNextKey();
                        break;
                    case Key.Boolean:
                        resultType = new VarType( TypeCode.Boolean );
                        GetNextKey();
                        break;
                    case Key.Bit:
                        resultType = new VarType( TypeCode.Bit );
                        GetNextKey();
                        break;
                    case Key.Notype:
                        resultType = new VarType( TypeCode.UndefinedType );
                        GetNextKey();
                        break;
                    //������
                    default:
                        //����������� ���
                        Fabric.Instance.ErrReg.Register( Err.Parser.Type.Var.Unknown );
                        break;
                    }
                if ( !endKeys.Contains( currKey ) )
                    {
                    Fabric.Instance.ErrReg.Register( Err.Parser.WrongEndSymbol.Type, endKeys.GetLastKeys() );
                    SkipTo( endKeys );
                    }
                }
            return resultType;
            }
        }
    }
