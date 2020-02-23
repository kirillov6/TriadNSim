using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

using TriadCompiler.Parser.Common.Header;
using TriadCompiler.Parser.Common.Statement;

namespace TriadCompiler
    {
    internal partial class DesignParser : CommonParser
        {
        /// <summary>
        ///  ��������� ����
        /// </summary>
        private GraphCodeBuilder codeBuilder
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
            GetNextKey();
            Design( endKey );
            }


        /// <summary>
        /// �������� design
        /// </summary>
        /// <syntax>Design { Model.Parse | SimCondition.Parse } Def StatementList EndDesign</syntax>
        /// <param name="endKey">��������� ���������� �������� ��������</param>
        private void Design( EndKeyList endKey )
            {
            if ( currKey != Key.Design )
                {
                err.Register( Err.Parser.WrongStartSymbol.Design, Key.Design );
                SkipTo( endKey.UniteWith( Key.Design ) );
                }
            if ( currKey == Key.Design )
                {
                Accept( Key.Design );

                DesignTypeType dType = null;

                //��� �������
                HeaderName.Parse( endKey.UniteWith( Key.Model, Key.Routine, Key.Structure, Key.SimCondition, Key.IProcedure, Key.Define, Key.EndDesign ),
                    delegate( string headerName )
                        {
                        dType = new DesignTypeType( headerName, DesignTypeCode.Design );
                        CommonArea.Instance.Register( dType );
                        } );

                this.designTypeName = dType.Name;
                codeBuilder.SetClassName( this.designTypeName );

                //������� ����� ������� - graph
                this.codeBuilder.SetBaseClass( Builder.Design.BaseClass );

                //���������� �������, �����, ��������, ������� �������������
                while ( ( currKey == Key.Model ) ||
                        ( currKey == Key.Routine ) ||
                        ( currKey == Key.Structure ) ||
                        ( currKey == Key.SimCondition ) )
                    {
                    //��������� ������� ������ � �������������
                    CommonParser currParser = Fabric.Instance.Parser;
                    CodeBuilder currBuilder = Fabric.Instance.Builder;

                    switch ( currKey )
                        {
                        case Key.Model:
                            Fabric.Instance.Parser = new ModelParser();
                            Fabric.Instance.Builder = new GraphCodeBuilder();
                            break;
                        case Key.Routine:
                            Fabric.Instance.Parser = new RoutineParser();
                            Fabric.Instance.Builder = new RoutineCodeBuilder();
                            break;
                        case Key.Structure:
                            Fabric.Instance.Parser = new StructureParser();
                            Fabric.Instance.Builder = new GraphCodeBuilder();
                            break;
                        case Key.SimCondition:
                            Fabric.Instance.Parser = new SimConditionParser();
                            Fabric.Instance.Builder = new IConditionCodeBuilder();
                            break;
                        }

                    Fabric.Instance.Scanner.SaveSymbol( currSymbol );
                    //��������� ������
                    Fabric.Instance.Parser.Compile( endKey.UniteWith( Key.Model, Key.Routine, Key.Structure,
                        Key.SimCondition, Key.Define, Key.EndDesign ) );

                    //���������� ���
                    Fabric.Instance.Builder.Build();

                    //��������������� ��������
                    Fabric.Instance.Scanner.SaveSymbol( Fabric.Instance.Parser.CurrentSymbol );
                    Fabric.Instance.Parser = currParser;
                    Fabric.Instance.Builder = currBuilder;
                    GetNextKey();
                    }

                Accept( Key.Define );

                varArea.AddArea();
                //������������ ����������� �������
                RegisterStandardFuntions();

                //��������� ����������� � �������� ������-����������
                DesignVarType designVarType = new DesignVarType( this.designTypeName, DesignTypeCode.Structure );
                CommonArea.Instance.Register( designVarType );
                codeBuilder.AddBuildStatementList(TriadCompiler.CodeBuilder.GetDesignVarDefinitionStatements(designVarType));

                codeBuilder.AddBuildStatementList( StatementList.Parse( endKey.UniteWith( Key.EndDesign ), StatementContext.Common ) );

                varArea.RemoveArea();

                //����� �������
                Accept( Key.EndDesign );

                if ( !endKey.Contains( currKey ) )
                    {
                    err.Register( Err.Parser.WrongEndSymbol.Design );
                    SkipTo( endKey );
                    }
                }
            }
        }
    }
