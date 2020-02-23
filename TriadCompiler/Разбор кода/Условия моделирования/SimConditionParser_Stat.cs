using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

using TriadCore;
using TriadCompiler.Parser.Common.Declaration.Var;
using TriadCompiler.Parser.Common.Statement;
using TriadCompiler.Parser.SimCondition.Statement;

//by jum
using TriadCompiler.Parser.Model.Declaration.DesignVariable;
using TriadCompiler.Parser.Structure.Statement;
using TriadCompiler.Parser.Common.Function;
using TriadCompiler.Parser.Model.Statement;

namespace TriadCompiler
    {
    public partial class SimConditionParser
        {
        /// <summary>
        /// ��������� ��������� �������� ��������� � ��
        /// </summary>
        private static List<Key> startKeys = null;
        /// <summary>
        /// ����������� ��
        /// </summary>
        private static List<IProcedureType> standartIP;


        /// <summary>
        /// ��������� ������� ��������� � ��
        /// </summary>
        public static List<Key> StartKeys
            {
            get
                {
                if ( startKeys == null )
                    {
                    Key[] keySet = { Key.Identificator, Key.For, Key.Case, Key.While, 
                            Key.If, Key.Interlock, Key.Available, Key.Print, Key.Eor, Key.Foreach,
                            Key.Let, Key.Put, Key.Structure, Key.Routine, Key.Model}; //by jum

                    startKeys = new List<Key>( keySet );
                    //���������� ����������
                    startKeys.AddRange( TypeDeclaration.TypeStartKeys );
                    }
                return startKeys;
                }
            }

        public static List<IProcedureType> StandartIP
        {
            get
            {
                if (standartIP == null)
                {
                    CreateStandartIP();
                }
                return standartIP;
            }
        }


        /// <summary> 
        /// �������� � �������� �������������
        /// </summary>
        /// <param name="endKeys"> ��������� �������� �������� </param>
        /// <param name="context"> ������� �������� </param>
        /// <returns> ������������� ��� ��������� ���� </returns>
        public override CodeStatementCollection Statement( EndKeyList endKeys, StatementContext context )
        {
            CodeStatementCollection statementList = new CodeStatementCollection();

            if ( !StartKeys.Contains( currKey ) && !endKeys.Contains( currKey ) )
            {
                err.Register( Err.Parser.WrongStartSymbol.Statement );
                SkipTo( endKeys.UniteWith( StartKeys ) );
            }
            if ( StartKeys.Contains( currKey ) )
            {
                //���������� ����������
                //if ( TypeDeclaration.TypeStartKeys.Contains( currKey ) )
                if (TypeDeclaration.TypeStartKeys.Contains(currKey) && currKey != Key.Graph) // by jum
                {
                    Fabric.Instance.Builder.AddVarDefinition(VarDeclaration.Parse(endKeys));
                }
                else
                {
                    switch ( currKey )
                    {
                        case Key.Identificator:
                            //�������������� ���������
                            if ( CommonArea.Instance.IsIProcedureRegistered( ( currSymbol as IdentSymbol ).Name ) )
                            {
                                //����� �������������� ���������             
                                statementList.AddRange( IPCall.Parse( endKeys ).Code );
                            }
                            //����� ������� // by jum
                            else if (CommonArea.Instance.IsFunctionRegistered((currSymbol as IdentSymbol).Name))
                            {
                                CodeStatement methodInvoke = new CodeExpressionStatement(FunctionInvoke.Parse(endKeys).Code);
                                statementList.Add(methodInvoke);
                            }
                            //����������� ������������ // by jum
                            else if (CommonArea.Instance.IsGraphRegistered((currSymbol as IdentSymbol).Name))
                            {
                                statementList.AddRange(StructAssignement.Parse(endKeys));
                            }
                            //������������
                            else
                            {
                                statementList.AddRange( Assignement.Parse( endKeys, AssignContext.Common ) );
                            }
                            break;
                        
                        //by jum
                        //========
                        case Key.Graph:
                        case Key.Structure:
                        //���������� ����������-������
                        case Key.Routine:
                        //���������� ��������� ����������
                        case Key.Model:
                            statementList.AddRange(DesignVarDeclaration.Parse(endKeys));
                            break;
                        //����������� design ����������
                        case Key.Let:
                            statementList.AddRange(Let.Parse(endKeys));
                            break;
                        //�������� ��������� ������
                        case Key.Put:
                            statementList.AddRange(PutRoutine.Parse(endKeys));
                            break;
                        //========

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

        //by jum
        /// <summary>
        ///
        /// </summary>
        private static void CreateStandartIP()
        {
            standartIP = new List<IProcedureType>();
            //Max
            IProcedureType proc = new IProcedureType("IPMax", TypeCode.Real);
            IExprType param = new VarType(TypeCode.Real);
            param.IsSpyObject = true;
            proc.AddParameter(param);
            proc.Description = IPMax.Description;
            standartIP.Add(proc);
            //Min
            proc = new IProcedureType("IPMin", TypeCode.Real);
            param = new VarType(TypeCode.Real);
            param.IsSpyObject = true;
            proc.AddParameter(param);
            proc.Description = IPMin.Description;
            standartIP.Add(proc);
            //Mean
            proc = new IProcedureType("IPMean", TypeCode.Real);
            param = new VarType(TypeCode.Real);
            param.IsSpyObject = true;
            proc.AddParameter(param);
            proc.Description = IPMean.Description;
            standartIP.Add(proc);
            //Dispersion
            proc = new IProcedureType("IPDispersion", TypeCode.Real);
            param = new VarType(TypeCode.Real);
            param.IsSpyObject = true;
            proc.AddParameter(param);
            proc.Description = IPDispersion.Description;
            standartIP.Add(proc);
            //TimeEvent
            proc = new IProcedureType("IPTimeEvent", TypeCode.Real);
            EventType ev = new EventType(string.Empty);
            ev.IsSpyObject = true;
            proc.AddParameter(ev);
            proc.Description = IPTimeEvent.Description;
            standartIP.Add(proc);
            //TimeChange
            proc = new IProcedureType("IPTimeChange", TypeCode.Real);
            param = new VarType(TypeCode.Real);
            param.IsSpyObject = true;
            proc.AddParameter(param);
            proc.Description = IPTimeChange.Description;
            standartIP.Add(proc);
            //TimePolus
            proc = new IProcedureType("IPTimePolus", TypeCode.Real);
            PolusType polus = new PolusType(true, true, true);
            proc.AddParameter(polus);
            proc.Description = IPTimePolus.Description;
            standartIP.Add(proc);
            //IntervalEvent
            proc = new IProcedureType("IPIntervalEvent", TypeCode.Real);
            ev = new EventType(string.Empty);
            ev.IsSpyObject = true;
            proc.AddParameter(ev);
            proc.Description = IPMax.Description;
            ev = new EventType(string.Empty);
            ev.IsSpyObject = true;
            proc.AddParameter(ev);
            proc.Description = IPIntervalEvent.Description;
            standartIP.Add(proc);
            //IntervalChange
            proc = new IProcedureType("IPIntervalChange", TypeCode.Real);
            param = new VarType(TypeCode.Real);
            param.IsSpyObject = true;
            proc.AddParameter(param);
            param = new VarType(TypeCode.Real);
            param.IsSpyObject = true;
            proc.AddParameter(param);
            proc.Description = IPIntervalChange.Description;
            standartIP.Add(proc);
            //IntervalPolus
            proc = new IProcedureType("IPIntervalPolus", TypeCode.Real);
            polus = new PolusType(true, true, true);
            polus.IsSpyObject = true;
            proc.AddParameter(polus);
            polus = new PolusType(true, true, true);
            polus.IsSpyObject = true;
            proc.AddParameter(polus);
            proc.Description = IPIntervalPolus.Description;
            standartIP.Add(proc);
        }

        /// <summary>
        /// ���������������� ����������� �������������� ���������
        /// </summary>
        protected void RegisterIProcedures()
        {
            foreach (var IP in StandartIP)
            {
                CommonArea.Instance.Register(IP);
            }
        }

        }
    }
