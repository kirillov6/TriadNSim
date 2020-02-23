using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

using TriadCompiler.Parser.Common.Header;
using TriadCompiler.Parser.Common.Statement;

namespace TriadCompiler
    {
    /// <summary>
    /// ����� ��� ������� ��������
    /// </summary>
    internal partial class StructureParser : CommonParser
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
            this.codeBuilder.SetBaseClass( Builder.Structure.BaseClass );

            GetNextKey();
            Structure( endKey );
            }


        /// <summary>
        /// ���������� ���������
        /// </summary>
        /// <param name="endKeys">��������� �������� ��������</param>
        /// <syntax>Structure Identificator Define StatementList EndStructure</syntax>
        private void Structure( EndKeyList endKeys )
            {
            if ( currKey != Key.Structure )
                {
                err.Register( Err.Parser.WrongStartSymbol.Structure, Key.Structure );
                SkipTo( endKeys.UniteWith( Key.Structure ) );
                }
            if ( currKey == Key.Structure )
                {
                Accept( Key.Structure );

                //��� ���������
                DesignTypeType designTypeType = null;

                //��� ���������
                HeaderName.Parse( endKeys.UniteWith( Key.LeftPar, Key.LeftBracket, Key.Include, Key.Define,
                    Key.EndStructure ), delegate( string headerName )
                        {
                            designTypeType = new DesignTypeType( headerName, DesignTypeCode.Structure );
                            CommonArea.Instance.Register( designTypeType );
                        } );
                
                this.designTypeName = designTypeType.Name;

                //������� �����, �������������� ������
                Fabric.Instance.Builder.SetClassName( designTypeName );

                //��������� ������� ���������
                varArea.AddArea();
                //������������ ����������� �������
                RegisterStandardFuntions();

                //��������� ����������� � ������� ������-����������
                DesignVarType designVarType = new DesignVarType( this.designTypeName, DesignTypeCode.Structure );
                CommonArea.Instance.Register( designVarType );
                codeBuilder.AddBuildStatementList(TriadCompiler.CodeBuilder.GetDesignVarDefinitionStatements(designVarType));

                //���������
                designTypeType.AddParameterList( Header.Parse( endKeys.UniteWith( Key.Include, Key.Define, Key.EndStructure ) ) );
                
                //������ ���������� ����������� �����
                List<Key> allowedTypeList = new List<Key>();
                allowedTypeList.Add( Key.Structure );

                //������ �����������
                IncludeSection.Parse( endKeys.UniteWith( Key.Define, Key.EndStructure ), allowedTypeList );

                Accept( Key.Define );

                codeBuilder.AddBuildStatementList( StatementList.Parse( endKeys.UniteWith( Key.EndStructure ), StatementContext.Common ) );

                //������� ������� ���������
                varArea.RemoveArea();

                Accept( Key.EndStructure );

                if ( !endKeys.Contains( currKey ) )
                    {
                    err.Register( Err.Parser.WrongEndSymbol.Structure, endKeys.GetLastKeys() );
                    SkipTo( endKeys );
                    }
                }
            }


        
        }
    }