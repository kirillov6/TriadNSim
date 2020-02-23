using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

using TriadCompiler.Parser.Common.Function;
using TriadCompiler.Parser.Common.Var;
using TriadCompiler.Parser.Common.Polus;
using TriadCompiler.Parser.Common.Ev;
using TriadCompiler.Parser.Common.Statement;
using TriadCompiler.Parser.Common.Expr;
using TriadCompiler.Parser.Design.Statement;

namespace TriadCompiler.Parser.SimCondition.Statement
    {
    /// <summary>
    /// ������ ������ ��
    /// </summary>
    internal partial class IPCall : CommonParser
        {
        /// <summary>
        /// ���������� ����� ������ ��
        /// </summary>
        private static int ipCallNumber = 0;


        /// <summary>
        ///  ��������� ����
        /// </summary>
        private static IConditionCodeBuilder codeBuilder
            {
            get
                {
                return Fabric.Instance.Builder as IConditionCodeBuilder;
                }
            }


        /// <summary>
        /// ����� ��
        /// </summary>
        /// <syntax>Identificator # [ ParameterList ] # SpyParameterList # { ParameterList } # # IPAssignment #</syntax>
        /// <param name="endKeys">��������� �������� ��������</param> 
        /// <returns>��� ��</returns>
        public static IPCallInfo Parse( EndKeyList endKeys )
            {
            IPCallInfo ipCallInfo = new IPCallInfo();

            //��� ��
            string ipName = ( currSymbol as IdentSymbol ).Name;

            //������ ������ ������ Identificator
            ipCallInfo.Type = CommonArea.Instance.GetType<IProcedureType>( ipName );
            //���������� ����� ��
            ipCallInfo.ipCallNumber = ipCallNumber;

            Accept( Key.Identificator );

            //������ ���������� 
            List<ExprInfo> paramList = new List<ExprInfo>();
            //�������� ������ ����������
            if ( currKey == Key.LeftBracket )
                {
                paramList.AddRange( FunctionInvoke.ParameterList( endKeys.UniteWith( Key.LeftPar, Key.LeftFigurePar, Key.Colon ),
                    ipCallInfo.Type.ParamVarList, Key.LeftBracket, Key.RightBracket ) );
                }
            //���� ��������� �� �������, � ��� ������ ����
            else if ( ipCallInfo.Type.ParamVarList.ParameterCount > 0 )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.Usage.ParameterList.NotEnoughParameters );
                }

            //������� �����, ����������� ��
            codeBuilder.SetInitialSection( GenerateIProcedureCreation( ipName, ipCallNumber, paramList ) );

            //������ spy-��������
            codeBuilder.SetInitialSection( SpyParameterList( endKeys.UniteWith( Key.LeftFigurePar, Key.Colon ), ipCallInfo, SingleSpyObject ) );

            //������ Out-����������
            if ( currKey == Key.LeftFigurePar )
                {
                ipCallInfo.Code.Add( OutVarList( endKeys.UniteWith( Key.Colon ), ipCallInfo ) );
                }

            //���� ������� ����������, ���� ����� �������� ���������
            if ( currKey == Key.Colon )
                {
                //���� �� �� ��������� �������� ��������
                if ( ipCallInfo.Type.ReturnCode == TypeCode.UndefinedType )
                    {
                    Fabric.Instance.ErrReg.Register( Err.Parser.Usage.IProcedure.NoReturnedValue );
                    }

                ipCallInfo.Code.Add( IPAssignment( endKeys, ipName ) );
                }

            //���������� �����, ���������������� ��
            codeBuilder.SetInitialSection( GenerateIProcedureInitialization( ipCallNumber ) );

            ipCallNumber++;
            return ipCallInfo;
            } 


        
        /// <summary>
        /// ������ spy-��������
        /// </summary>
        /// <syntax>( SingleSpyObject {, SingleSpyObject } )</syntax>
        /// <param name="endKeys">���������� �������� �������</param>
        /// <param name="ipCallInfo">���������� � ������ �������������� ���������</param>
        /// <param name="singeSpyObject">�����, ����������� ��������� spy-������</param>
        /// <returns>��������������� ���</returns>
        public static CodeStatementCollection SpyParameterList( EndKeyList endKeys, IPCallInfo ipCallInfo, SingleSpyObjectDelegate singeSpyObject )
            {
            //���������� �����, �������������� spy-������� � ������
            CodeMethodInvokeExpression registerSpyObjectsStat = new CodeMethodInvokeExpression();
            registerSpyObjectsStat.Method = new CodeMethodReferenceExpression();
            registerSpyObjectsStat.Method.MethodName = Builder.IProcedure.RegisterAllSpyObjects;
            registerSpyObjectsStat.Method.TargetObject = GetIProcedureCode( ipCallInfo.Type.Name, ipCallInfo.ipCallNumber );

            if ( currKey != Key.LeftPar )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.WrongStartSymbol.FunctionParameterList, Key.LeftPar );
                SkipTo( endKeys.UniteWith( Key.LeftPar ) );
                }
            if ( currKey == Key.LeftPar )
                {
                GetNextKey();

                //������� ����������
                IEnumerator<ISpyType> paramEnumerator = ipCallInfo.Type.GetEnumerator();
                paramEnumerator.MoveNext();

                //���� ��� �� ������ ������ ����������
                if ( currKey != Key.RightPar )
                    {
                    registerSpyObjectsStat.Parameters.Add(
                        singeSpyObject( endKeys.UniteWith( Key.RightPar, Key.Comma ), paramEnumerator ) );

                    while ( currKey == Key.Comma )
                        {
                        GetNextKey();

                        registerSpyObjectsStat.Parameters.Add(
                            singeSpyObject( endKeys.UniteWith( Key.RightPar, Key.Comma ), paramEnumerator ) );
                        }
                    }

                Accept( Key.RightPar );

                //���� ���� ������� �� ��� ���������
                if ( paramEnumerator.Current != null )
                    {
                    Fabric.Instance.ErrReg.Register( Err.Parser.Usage.ParameterList.NotEnoughParameters );
                    }

                if ( !endKeys.Contains( currKey ) )
                    {
                    Fabric.Instance.ErrReg.Register( Err.Parser.WrongEndSymbol.FunctionParameterList, endKeys.GetLastKeys() );
                    SkipTo( endKeys );
                    }
                }

            //��������� ���� ����� � ������ �������������
            CodeStatementCollection statList = new CodeStatementCollection();
            statList.Add( registerSpyObjectsStat );
            return statList;
            }


        /// <summary>
        /// �����, ����������� ��������� spy-������ � ������ spy-��������
        /// </summary>
        /// <param name="endKeys">���������� �������� �������</param>
        /// <param name="enumerator">��� ����������� ���������</param>
        /// <returns>>��� ������, ������������ ���� spy-������</returns>
        public delegate CodeMethodInvokeExpression SingleSpyObjectDelegate( EndKeyList endKeys, IEnumerator<ISpyType> enumerator );


        /// <summary>
        /// ���� spy-������
        /// </summary>
        /// <syntax>Variable | PolusVar | EventVar</syntax>
        /// <param name="endKeys">���������� �������� �������</param>
        /// <param name="enumerator">��� ����������� ���������</param>
        /// <returns>��� ������, ������������ ���� spy-������</returns>
        private static CodeMethodInvokeExpression SingleSpyObject( EndKeyList endKeys, IEnumerator<ISpyType> enumerator )
            {
            CodeMethodInvokeExpression getSpyObjectStat = new CodeMethodInvokeExpression();
            getSpyObjectStat.Method = new CodeMethodReferenceExpression();


            if ( currKey != Key.Identificator )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.WrongStartSymbol.SpyObject, Key.Identificator );
                SkipTo( endKeys.UniteWith( Key.Identificator ) );
                }
            if ( currKey == Key.Identificator )
                {
                ISpyType spyFormalType = enumerator.Current;

                if ( spyFormalType != null )
                    {
                    ISpyType spyOjectType = null;

                    getSpyObjectStat.Method.MethodName = Builder.IProcedure.GetSpyObject;

                    // ????????
                    string sIdentName = (currSymbol as IdentSymbol).Name;
                    if (CommonArea.Instance.IsGraphRegistered(sIdentName) /*&& sIdentName == Builder.ICondition.CurrentModel*/)
                    {
                        Accept(Key.Identificator);
                        Accept(Key.Point);
                        Simulate.modelName = sIdentName;
                        return Simulate.SingleSpyObject(endKeys, enumerator);
                    }
                    else if (CommonArea.Instance.IsNodeRegistered(sIdentName))
                    {
                        return Simulate.SingleSpyObject(endKeys, enumerator);
                    }

                    //���� ��� ������ ���� ����������
                    if ( spyFormalType is IExprType )
                        {
                        VarInfo varInfo = Variable.Parse( endKeys, /*Allow range*/ true,
                            /*Allow not indexed array*/ false );
                        Assignement.CheckVarTypes( spyFormalType as IExprType, varInfo.Type );
                        spyOjectType = varInfo.Type;

                        //��������
                        getSpyObjectStat.Parameters.Add( varInfo.CoreNameCode );
                        }
                    //���� ��� ������ ���� �����
                    else if ( spyFormalType is IPolusType )
                        {
                        PolusInfo polusInfo = PolusVar.Parse( endKeys );
                        Assignement.CheckPolusTypes( spyFormalType as IPolusType, polusInfo.Type );
                        spyOjectType = polusInfo.Type;

                        //��������
                        getSpyObjectStat.Parameters.Add( polusInfo.CoreNameCode );
                        }
                    //���� ��� ������ ���� �������
                    else if ( spyFormalType is EventType )
                        {
                        EventInfo eventInfo = EventVar.Parse( endKeys, /*Check registration*/ true );
                        spyOjectType = eventInfo.Type;

                        //������� ��������
                        getSpyObjectStat.Parameters.Add( eventInfo.CoreNameCode );
                        }

                    //���������, ��� spy-������?
                    if ( spyOjectType != null )
                        if ( spyOjectType != null && !spyOjectType.IsSpyObject )
                            {
                            Fabric.Instance.ErrReg.Register( Err.Parser.Usage.NeedSpyObject );
                            }
                    enumerator.MoveNext();
                    }
                //������� �������� ��������� �� ����
                else
                    {
                    Fabric.Instance.ErrReg.Register( Err.Parser.Usage.ParameterList.TooManyParameters );
                    }
                }

            return getSpyObjectStat;
            }


        /// <summary>
        /// ������ out-����������
        /// </summary>
        /// <param name="endKeys">���������� �������� �������</param>
        /// <param name="ipCallInfo">���������� � ��</param>
        /// <returns>��� ������, ������������ �������� out-����������</returns>
        public static CodeMethodInvokeExpression OutVarList( EndKeyList endKeys, IPCallInfo ipCallInfo )
            {
            //���������� ����� �������, ����������� �������� out-����������
            CodeMethodInvokeExpression getOutVarStat = new CodeMethodInvokeExpression();
            getOutVarStat.Method = new CodeMethodReferenceExpression();
            getOutVarStat.Method.TargetObject = GetIProcedureCode( ipCallInfo.Type.Name, ipCallInfo.ipCallNumber );
            getOutVarStat.Method.MethodName = Builder.IProcedure.GetOutVariables;

            Accept( Key.LeftFigurePar );

            //������� ����������
            IEnumerator<IExprType> paramEnumerator = ipCallInfo.Type.OutVarList.GetEnumerator();
            paramEnumerator.MoveNext();

            //���� ��� �� ������ ������ ����������
            if ( currKey != Key.RightFigurePar )
                {
                VarInfo varInfo = Variable.Parse( endKeys.UniteWith( Key.RightFigurePar, Key.Comma ), false, true );

                //�������� ���������
                FunctionInvoke.CheckParameterType( paramEnumerator, varInfo.Type );

                //������� ��������
                getOutVarStat.Parameters.Add( new CodeSnippetExpression( "out " + varInfo.StrCode ) );

                while ( currKey == Key.Comma )
                    {
                    GetNextKey();

                    varInfo = Variable.Parse( endKeys.UniteWith( Key.RightFigurePar, Key.Comma ), false, true );

                    //�������� ���������
                    FunctionInvoke.CheckParameterType( paramEnumerator, varInfo.Type );

                    //������� ��������
                    getOutVarStat.Parameters.Add( new CodeSnippetExpression( "out " + varInfo.StrCode ) );
                    }
                }

            //���� ���� ������� �� ��� ���������
            if ( paramEnumerator.Current != null )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.Usage.ParameterList.NotEnoughParameters );
                }

            Accept( Key.RightFigurePar );

            if ( !endKeys.Contains( currKey ) )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.WrongEndSymbol.FunctionParameterList, endKeys.GetLastKeys() );
                SkipTo( endKeys );
                }
            return getOutVarStat;
            }


        /// <summary>
        /// ������ ��������������� ������������
        /// </summary>
        /// <param name="endKeys">���������� �������� �������</param>
        /// <param name="ipName">��� ��</param>
        /// <returns>��� ������, ������������ ������������</returns>
        private static CodeAssignStatement IPAssignment( EndKeyList endKeys, string ipName )
            {
            Accept( Key.Colon );

            VarInfo varInfo = Variable.Parse( endKeys, false, false );

            //������� �����, ����������� ������������
            CodeAssignStatement asignStat = new CodeAssignStatement();
            asignStat.Left = varInfo.Code;

            //�������� ��������� �� ������
            CodeMethodInvokeExpression doProcessStat = new CodeMethodInvokeExpression();
            doProcessStat.Method = new CodeMethodReferenceExpression();
            doProcessStat.Method.MethodName = Builder.IProcedure.DoProcessing;
            doProcessStat.Method.TargetObject = GetIProcedureCode( ipName, ipCallNumber );

            asignStat.Right = doProcessStat;

            return asignStat;
            }

        }
    }
