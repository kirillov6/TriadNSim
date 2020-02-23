using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

using TriadCompiler.Parser.Common.Declaration.Var;
using TriadCompiler.Parser.Model.Declaration.DesignVariable;

using TriadCompiler.Parser.Common.Statement;
using TriadCompiler.Parser.Design.Statement;
using TriadCompiler.Parser.Model.Statement;
using TriadCompiler.Parser.SimCondition.Statement;
using TriadCompiler.Parser.Routine.Statement;
using TriadCompiler.Parser.Structure.Statement;
using TriadCompiler.Parser.Common.Function;

namespace TriadCompiler
    {
    internal partial class DesignParser
        {
        /// <summary>
        /// ��������� ��������� �������� ��������� � Design
        /// </summary>
        private static List<Key> startKeys = null;


        /// <summary>
        /// ��������� ������� ��������� � Design
        /// </summary>
        public static List<Key> StartKeys
            {
            get
                {
                if ( startKeys == null )
                    {
                    Key[] keySet = { Key.Identificator, 
                                     Key.For, Key.While, Key.If, Key.Case, Key.Print,
                                     Key.Structure, Key.Routine, Key.Model, Key.Let, Key.Put,                                                             
                                     Key.Eor, Key.Simulate, Key.Foreach };

                    startKeys = new List<Key>( keySet );
                    //���������� ����������
                    startKeys.AddRange( TypeDeclaration.TypeStartKeys );
                    }
                return startKeys;
                }
            }


        /// <summary> 
        /// �������� � Design
        /// </summary>
        /// <param name="endKeys"> ��������� �������� �������� </param>
        /// <param name="context"> ������� �������� </param>
        /// <returns> ������������� ��� ��������� ���� </returns>
        public override CodeStatementCollection Statement( EndKeyList endKeys, StatementContext context )
            {
            CodeStatementCollection statementList = new CodeStatementCollection();

            if ( !StartKeys.Contains( currKey ) &&
                 !endKeys.Contains( currKey ) )
                {
                err.Register( Err.Parser.WrongStartSymbol.Statement );
                SkipTo( endKeys.UniteWith( StartKeys ) );
                }
            if ( StartKeys.Contains( currKey ) )
                {
                //by jum
                //���������� ����������
                if ( TypeDeclaration.TypeStartKeys.Contains( currKey ) && currKey != Key.Graph )
                    {
                    Fabric.Instance.Builder.AddVarDefinition( VarDeclaration.Parse( endKeys) );
                    }
                else
                    {
                    switch ( currKey )
                        {
                        //�������������
                        case Key.Simulate:
                            statementList.AddRange( Simulate.Parse( endKeys ) );
                            break;
                        //���������� �������� ����������
                        //by jum
                        case Key.Graph:
                        case Key.Structure:
                        //���������� ����������-������
                        case Key.Routine:
                        //���������� ��������� ����������
                        case Key.Model:
                            statementList.AddRange(DesignVarDeclaration.Parse( endKeys ));
                            break;
                        case Key.Identificator:
                            //����� �������
                            if ( CommonArea.Instance.IsFunctionRegistered( ( currSymbol as IdentSymbol ).Name ) )
                                {
                                CodeStatement methodInvoke = new CodeExpressionStatement( FunctionInvoke.Parse( endKeys ).Code );
                                statementList.Add( methodInvoke );
                                }
                            //����������� ������������
                            else if ( CommonArea.Instance.IsGraphRegistered( ( currSymbol as IdentSymbol ).Name ) )
                                {
                                statementList.AddRange( StructAssignement.Parse( endKeys ) );
                                }
                            //������� ������������
                            else
                                {
                                statementList.AddRange( Assignement.Parse( endKeys, AssignContext.Common ) );
                                }
                            break;
                        //����������� design ����������
                        case Key.Let:
                            statementList.AddRange( Let.Parse( endKeys ) );
                            break;
                        //�������� ��������� ������
                        case Key.Put:
                            statementList.AddRange( PutRoutine.Parse( endKeys ) );
                            break;
                        //�������� ��������
                        case Key.If:
                            statementList.Add( Condition.Parse( endKeys, context ) );
                            break;
                        //���� While
                        case Key.While:
                            statementList.Add( WhileCicle.Parse( endKeys, context ) );
                            break;
                        //���� For
                        case Key.For:
                            statementList.Add( ForCicle.Parse( endKeys, context ) );
                            break;
                        //���������� ������
                        case Key.Print:
                            statementList.Add( Print.Parse( endKeys ) );
                            break;
                        //�������� ��������� �������������
                        case Key.Eor:
                            statementList.Add( Eor.Parse( endKeys ) );
                            break;
                        //�������� �������� ��������� ��-��
                        case Key.Foreach:
                            statementList.AddRange( Foreach.Parse( endKeys, context ) );
                            break;
                        }
                    }
                }
            if ( !endKeys.Contains( currKey ) )
                {
                err.Register( Err.Parser.WrongEndSymbol.Statement );
                SkipTo( endKeys );
                }
            return statementList;
            }

        }
    }
