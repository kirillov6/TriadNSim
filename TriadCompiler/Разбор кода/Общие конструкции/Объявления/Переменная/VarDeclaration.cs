using System;
using System.Collections.Generic;
using System.Text;
//by jum
using System.CodeDom;

using TriadCompiler.Parser.Common.Declaration.Var;
using TriadCompiler.Parser.Common.Expr;
using TriadCompiler.Parser.Common.Statement;

namespace TriadCompiler.Parser.Common.Declaration.Var
    {
    /// <summary>
    /// ������ ���������� ����������
    /// </summary>
    internal class VarDeclaration : CommonParser
        {
        /// <summary>
        /// ���������� ����������
        /// </summary>
        /// <param name="endKeys">��������� ���������� �������� ��������</param>
        /// <param name="context">���������</param>
        /// <returns>���� ����������� ����������</returns>
        public static List<IExprType> Parse( EndKeyList endKeys, VarDeclarationContext context )
            {
            List<IExprType> varTypeList = new List<IExprType>();

            //���� ����� �������������� ����������
            if ( context != VarDeclarationContext.IncludeSection )
                varTypeList.AddRange( DeclarationWithRegistration( endKeys, context ) );
            else
                varTypeList.AddRange( DeclarationWithOutRegistration( endKeys, context ) );
            
            return varTypeList;
            }


        /// <summary>
        /// ���������� ���������� � ��������������
        /// </summary>
        /// <param name="endKeys">��������� ���������� �������� ��������</param>
        /// <returns>������� ���->��� �������������</returns>
        public static Dictionary<IExprType, CodeExpression> Parse(EndKeyList endKeys)
        {
            Dictionary<IExprType, CodeExpression> result = new Dictionary<IExprType, CodeExpression>();
            IExprType varType = TypeDeclaration.Parse(endKeys.UniteWith(Key.Identificator));
            do
            {
                if (result.Count > 0)
                    Accept(Key.Comma);
                IExprType var = VarName(endKeys.UniteWith(Key.Comma, Key.Assign), varType, true, VarDeclarationContext.Common);
                CodeExpression initExpression = null;
                if (currKey == Key.Assign)
                {
                    Accept(Key.Assign);

                    ExprInfo exprInfo = Expression.Parse(endKeys.UniteWith(Key.Comma));
                    
                    if (!Assignement.CheckVarTypes(varType, exprInfo.Type))
                        SkipTo(endKeys.UniteWith(Key.Comma));
                    else
                        initExpression = exprInfo.Code;
                }
                result[var] = initExpression;
            }
            while (currKey == Key.Comma);

            return result;
        }

        /// <summary>
        /// ������ ����� ���������� � �� ����������
        /// </summary>
        /// <syntax>IDentificator</syntax>
        /// <param name="endKeys">���������� �������� �������</param>
        /// <param name="varType">��� ���� ���������� � ����������</param>
        /// <param name="registerType">������������� ����������� ����</param>
        /// <param name="context">�������� ����������</param>
        /// <returns>��� ����������</returns>
        private static IExprType VarName( EndKeyList endKeys, IExprType varType, bool registerType, VarDeclarationContext context )
            {
            //��� ����������
            IExprType resultType = null;
            if ( varType != null )
                resultType = varType.Clone();

            if ( currKey != Key.Identificator )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.WrongStartSymbol.VarDeclarationName, Key.Identificator );
                SkipTo( endKeys.UniteWith( Key.Identificator ) );
                }
            if ( currKey == Key.Identificator )
                {
                resultType.Name = ( currSymbol as IdentSymbol ).Name;                

                //���� ��� ���������� ����� ��������������
                if ( registerType )
                    CommonArea.Instance.Register( resultType );

                //���� ��� spy-������
                if ( context == VarDeclarationContext.SpyObjectList )
                    resultType.IsSpyObject = true;

                GetNextKey();

                //by jum
                //���������� ������� � �������
                if (resultType.Code == TypeCode.Node && currKey == Key.Later)
                {
                    (Fabric.Instance.Builder as GraphCodeBuilder).AddBuildStatementList(PolusDeclarationParse(endKeys, resultType.Name));
                }

                if ( !endKeys.Contains( currKey ) )
                    {
                    Fabric.Instance.ErrReg.Register( Err.Parser.WrongEndSymbol.VarDeclarationName, endKeys.GetLastKeys() );
                    SkipTo( endKeys );
                    }
                }

            return resultType;
            }

        //by jum
        /// <summary>
        ///  ������ ���������� ������� � ���������� �������
        /// </summary>
        /// <param name="endKeys">���������� �������� �������</param>
        /// <param name="nodeVarName">��� �������, ���� �������� ������</param>
        /// <returns>������������� ��� ��������� ����</returns>
        private static CodeStatementCollection PolusDeclarationParse(EndKeyList endKeys, string nodeVarName)
        {
            CodeStatementCollection statements = new CodeStatementCollection();

            Accept(Key.Later);

            if (currKey != Key.Identificator)
            {
                Fabric.Instance.ErrReg.Register(Err.Parser.WrongStartSymbol.ObjectReference, Key.Identificator);
                SkipTo(endKeys.UniteWith(Key.Identificator, Key.Greater));
            }

            if (currKey == Key.Identificator)
            {
                string polusName = (currSymbol as IdentSymbol).Name;

                Accept(Key.Identificator);

                //��������� ����
                CodeMethodInvokeExpression declarePolusStat = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(nodeVarName), Builder.Structure.BuildExpr.DeclareOperation.DecalarePolusInNode, new CodeObjectCreateExpression(Builder.CoreName.Name, new CodePrimitiveExpression(polusName)));

                statements.Add(declarePolusStat);
            }

            while (currKey == Key.Comma)
            {
                Accept(Key.Comma);

                if (currKey != Key.Identificator)
                {
                    Fabric.Instance.ErrReg.Register(Err.Parser.WrongStartSymbol.ObjectReference, Key.Identificator);
                    SkipTo(endKeys.UniteWith(Key.Identificator, Key.Greater));
                }

                if (currKey == Key.Identificator)
                {
                    string polusName = (currSymbol as IdentSymbol).Name;

                    Accept(Key.Identificator);

                    //��������� ����
                    CodeMethodInvokeExpression declarePolusStat = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(nodeVarName), Builder.Structure.BuildExpr.DeclareOperation.DecalarePolusInNode, new CodeObjectCreateExpression(Builder.CoreName.Name, new CodePrimitiveExpression(polusName)));

                    statements.Add(declarePolusStat);
                }
            }

            Accept(Key.Greater);

            return statements;
        }

        /// <summary>
        /// ������� ���������� ����������
        /// </summary>
        /// <syntax>Type Identificator {,Identificator}</syntax>
        /// <param name="endKeys">��������� ���������� �������� ��������</param>
        /// <param name="context">��������</param>
        /// <returns>������ ����� ����������� ����������</returns>
        private static List<IExprType> DeclarationWithRegistration( EndKeyList endKeys, VarDeclarationContext context )
            {
            List<IExprType> varTypeList = new List<IExprType>();

            IExprType varType = TypeDeclaration.Parse( endKeys.UniteWith( Key.Identificator ) );

            //��� ����������
            varTypeList.Add( VarName( endKeys.UniteWith( Key.Comma ), varType, true, context ) );
 
            while ( currKey == Key.Comma )
                {
                GetNextKey();

                //��� ����������
                varTypeList.Add( VarName( endKeys.UniteWith( Key.Comma ), varType, true, context ) );
                }

            return varTypeList;
            }


        /// <summary>
        /// ���������� ���������� ��� ����������� �� ����
        /// </summary>
        /// <syntax>Type #Identificator {,Identificator}#</syntax>
        /// <param name="endKeys">��������� ���������� �������� ��������</param>
        /// <param name="context">��������� ����������</param>
        /// <returns>������ ����� ����������� ����������</returns>
        private static List<IExprType> DeclarationWithOutRegistration( EndKeyList endKeys, VarDeclarationContext context )
            {
            List<IExprType> varTypeList = new List<IExprType>();

            IExprType varType = TypeDeclaration.Parse( endKeys.UniteWith( Key.Identificator ) );

            //�������������� �����
            if ( currKey == Key.Identificator )
                {
                //��� ����������
                varTypeList.Add( VarName( endKeys.UniteWith( Key.Comma ), varType, false, context ) );

                while ( currKey == Key.Comma )
                    {
                    GetNextKey();

                    //��� ����������
                    varTypeList.Add( VarName( endKeys.UniteWith( Key.Comma ), varType, false, context ) );
                    }
                }
            //���� ����� ���������� �� �������
            else
                {
                varTypeList.Add( varType );
                }

            return varTypeList;
            }
        
        }
    }
