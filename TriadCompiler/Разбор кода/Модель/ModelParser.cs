using System;
using System.Collections.Generic;
using System.Text;

using TriadCompiler.Parser.Common.Header;
using TriadCompiler.Parser.Common.Statement;

namespace TriadCompiler
    {
    /// <summary>
    /// ������ ������
    /// </summary>
    internal partial class ModelParser : CommonParser
        {
        /// <summary>
        ///  ��������� ����
        /// </summary>
        protected GraphCodeBuilder codeBuilder
            {
            get
                {
                return Fabric.Instance.Builder as GraphCodeBuilder;
                }
            }


        /// <summary>
        /// ������ ������
        /// </summary>
        /// <param name="endKey">��������� ���������� �������� ��������</param>
        public override void Compile( EndKeyList endKey )
            {
            this.codeBuilder.SetBaseClass( Builder.Model.BaseClass );

            GetNextKey();
            Model( endKey );
            }


        /// <summary>
        /// �������� ������
        /// </summary>
        /// <syntax>Model EndModel</syntax>
        /// <param name="endKeys">��������� ���������� �������� ��������</param>
        private void Model( EndKeyList endKeys )
            {
            if ( currKey != Key.Model )
                {
                err.Register( Err.Parser.WrongStartSymbol.Model, Key.Model );
                SkipTo( endKeys.UniteWith( Key.Model ) );
                }
            if ( currKey == Key.Model )
                {
                Accept( Key.Model );
               
                //������������ ����������� ������� (�� ������� �����)
                RegisterStandardFuntions();

                //��� ������
                HeaderName.Parse( endKeys.UniteWith( Key.LeftBracket, Key.Structure, Key.Routine,
                    Key.Include, Key.Define, Key.EndModel ), delegate( string headerName )
                        {
                            this.designTypeName = headerName;
                            CommonArea.Instance.Register( new DesignTypeType( headerName, DesignTypeCode.Model ) ); 
                        } );

                //������� �����, �������������� ������
                Fabric.Instance.Builder.SetClassName( designTypeName );

                //��������� ������
                List<IExprType> varTypeList = ParameterSection.Parse( endKeys.UniteWith( Key.Structure, Key.Routine, Key.Include,
                    Key.Define, Key.EndModel ), VarDeclarationContext.Common );

                foreach ( IExprType varType in varTypeList )
                    {
                    //��������� ��� ���������� � ��������� ������������
                    Fabric.Instance.Builder.AddParameterInConstructor( varType, varType.Name );
                    //��������� ���������� ������ ������
                    Fabric.Instance.Builder.AddVarDefinition( varType );
                    }

                //������ ���������� ������������ �����
                List<Key> allowedTypeList = new List<Key>();
                allowedTypeList.Add( Key.Structure );
                allowedTypeList.Add( Key.Routine );

                //���������� �������� � �����
                while ( currKey == Key.Structure || currKey == Key.Routine || currKey == Key.Include )
                    {
                    //��������� ������� ������ � �������������
                    CommonParser currParser = Fabric.Instance.Parser;
                    CodeBuilder currBuilder = Fabric.Instance.Builder;

                    //���������� ���������
                    if ( currKey == Key.Structure )
                        {  
                        Fabric.Instance.Parser = new StructureParser();
                        Fabric.Instance.Builder = new GraphCodeBuilder();
                        }
                    //���������� ������
                    else if ( currKey == Key.Routine )
                        {
                        Fabric.Instance.Parser = new RoutineParser();
                        Fabric.Instance.Builder = new RoutineCodeBuilder();
                        }
                    //������ �����������
                    else if ( currKey == Key.Include )
                        {
                        IncludeSection.Parse( endKeys.UniteWith( Key.Include, Key.Structure, Key.Routine, Key.Define, Key.EndModel ), allowedTypeList );
                        continue;
                        }

                    //��������� ��������
                    Fabric.Instance.Scanner.SaveSymbol( currSymbol );
                    Fabric.Instance.Parser.Compile( endKeys.UniteWith( Key.Include, Key.Structure, Key.Routine, Key.Define, Key.EndModel ) );
                    Fabric.Instance.Builder.Build();

                    //��������������� ��������
                    Fabric.Instance.Scanner.SaveSymbol( Fabric.Instance.Parser.CurrentSymbol );
                    Fabric.Instance.Parser = currParser;
                    Fabric.Instance.Builder = currBuilder;
                    GetNextKey();
                    }

                Accept( Key.Define );

                //��������� ������� ���������
                varArea.AddArea();

                //��������� ����������� � ������� ������-����������
                DesignVarType designVarType = new DesignVarType( this.designTypeName, DesignTypeCode.Structure );
                CommonArea.Instance.Register( designVarType );
                codeBuilder.AddBuildStatementList(TriadCompiler.CodeBuilder.GetDesignVarDefinitionStatements(designVarType));

                codeBuilder.AddBuildStatementList( StatementList.Parse( endKeys.UniteWith( Key.EndModel ), StatementContext.Common ) );

                Accept( Key.EndModel );

                //������� ������� ���������
                varArea.RemoveArea();

                if ( !endKeys.Contains( currKey ) )
                    {
                    err.Register( Err.Parser.WrongEndSymbol.Model, endKeys.GetLastKeys() );
                    SkipTo( endKeys );
                    }
                }
            }


        }
    }
