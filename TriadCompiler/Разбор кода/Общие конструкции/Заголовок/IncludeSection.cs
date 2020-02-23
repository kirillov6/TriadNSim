using System;
using System.Collections.Generic;
using System.Text;

using TriadCompiler.Parser.Common.Expr;
using TriadCompiler.Code.Generator;

namespace TriadCompiler.Parser.Common.Header
    {
    /// <summary>
    /// ������ ������ ����������� ������� �������
    /// </summary>
    internal class IncludeSection : CommonParser
        {
        /// <summary>
        /// ������ �����������
        /// </summary>
        /// <param name="endKeys">��������� ���������� �������� ��������</param>
        /// <param name="allowedTypeList">��������� �����������, ������� ����� ����������</param>
        /// <syntax>{ IncludeStatement }</syntax>
        public static void Parse( EndKeyList endKeys, List<Key> allowedTypeList )
            {
            while ( currKey == Key.Include )
                {
                Accept( Key.Include );
                IncludeStatement( endKeys.UniteWith( Key.Include ), allowedTypeList );
                }
            }


        /// <summary>
        /// �������� ����������
        /// </summary>
        /// <param name="endKeys">��������� ���������� �������� ��������</param>
        /// <param name="allowedTypeList">��������� �����������, ������� ����� ����������</param>
        /// <syntax>Structure | Routine | InfProcedure | Model | ModelCondition
        /// Identificator #ParameterList# from String </syntax>
        private static void IncludeStatement( EndKeyList endKeys, List<Key> allowedTypeList )
            {
            if ( !allowedTypeList.Contains( currKey ) )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.WrongStartSymbol.IncludeSection, allowedTypeList );
                SkipTo( endKeys.UniteWith( allowedTypeList ) );
                }
            if ( allowedTypeList.Contains( currKey ) )
                {
                //������������ ���
                DesignTypeType designType = new DesignTypeType( string.Empty, DesignTypeCode.Structure );

                switch ( currKey )
                    {
                    //������������ ���������
                    case Key.Structure:
                        designType.Code = DesignTypeCode.Structure;
                        break;
                    //������������ ������
                    case Key.Routine:
                        designType.Code = DesignTypeCode.Routine;
                        break;
                    //������������ ������
                    case Key.Model:
                        designType.Code = DesignTypeCode.Structure;
                        break;
                    }


                GetNextKey();

                //��� ������������� �������
                DesignTypeType designTypeType = new DesignTypeType( string.Empty, DesignTypeCode.Structure );

                //��� ����������� ��������� ����������
                HeaderName.Parse( endKeys.UniteWith( Key.LeftBracket, Key.From, Key.StringValue ),
                    delegate( string headerName )
                        {
                        designTypeType = new DesignTypeType( headerName, designType.Code );
                        CommonArea.Instance.Register( designTypeType ); 
                        } );

                //������������ ���������
                designTypeType.AddParameterList( ParameterSection.Parse( endKeys.UniteWith( Key.From, Key.StringValue ), 
                    VarDeclarationContext.IncludeSection ) );

                Accept( Key.From );

                //���� � �����
                ExprInfo exprInfo = Expression.Parse( endKeys );

                //���� ��� ��������� ����������� ���������
                if ( exprInfo.HasNoError && exprInfo.IsNotIndexed() && exprInfo.IsNotSet() 
                    && exprInfo.IsConstant() && exprInfo.IsString() )
                    {
                    CodeFabric.Instance.AddReference( ( exprInfo.Value as StringValue ).Value );
                    }
                
                if ( !endKeys.Contains( currKey ) )
                    {
                    Fabric.Instance.ErrReg.Register( Err.Parser.WrongEndSymbol.IncludeSection, endKeys.GetLastKeys() );
                    SkipTo( endKeys );
                    }
                }
            }        


        }
    }
