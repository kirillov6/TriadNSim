using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace TriadCompiler
    {
    class IProcedureCodeBuilder : RoutineCodeBuilder
        {
        /// <summary>
        /// �����������
        /// </summary>
        public IProcedureCodeBuilder()
            {
            CreateRegisterSpyObjectMethod();
            CreateGetOutVarMethod();
            CreateDoHandlingMethod();
            CreateDoProcessingMethod();            
            }


        /// <summary>
        /// ������� �����, �������������� ��� spy-�������
        /// </summary>
        private void CreateRegisterSpyObjectMethod()
            {
            this.registerSpyObjectsMethod = new CodeMemberMethod();
            this.registerSpyObjectsMethod.Name = Builder.IProcedure.RegisterAllSpyObjects;
            this.registerSpyObjectsMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            this.resultClass.Members.Add( this.registerSpyObjectsMethod );
            }


        /// <summary>
        /// ������� �����, ������������ ��� out-����������
        /// </summary>
        private void CreateGetOutVarMethod()
            {
            this.getOutVarMethod = new CodeMemberMethod();
            this.getOutVarMethod.Name = Builder.IProcedure.GetOutVariables;
            this.getOutVarMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            this.resultClass.Members.Add( this.getOutVarMethod );
            }


        /// <summary>
        /// ������� �����, ������������ �������� ��
        /// </summary>
        private void CreateDoProcessingMethod()
            {
            this.doProcessingMethod = new CodeMemberMethod();
            this.doProcessingMethod.Name = Builder.IProcedure.DoProcessing;
            this.doProcessingMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            this.resultClass.Members.Add( this.doProcessingMethod );
            }


        /// <summary>
        /// ������� �����, �������������� ��������� spy-��������
        /// </summary>
        private void CreateDoHandlingMethod()
            {
            this.doHandlingMethod = new CodeMemberMethod();
            this.doHandlingMethod.Name = Builder.IProcedure.Handling.DoHandling;
            this.doHandlingMethod.Attributes = MemberAttributes.Family | MemberAttributes.Override;

            //������ �������� - ������������ ������
            CodeParameterDeclarationExpression paramSpyObject = new CodeParameterDeclarationExpression();
            paramSpyObject.Name = Builder.IProcedure.Handling.SpyObjectNameInDoHandling;
            paramSpyObject.Type = new CodeTypeReference( Builder.IProcedure.SpyObject );

            this.doHandlingMethod.Parameters.Add( paramSpyObject );

            //������ �������� - ������� ��������� �����
            CodeParameterDeclarationExpression paramSystemTime = new CodeParameterDeclarationExpression();
            paramSystemTime.Name = Builder.Routine.SystemTime;
            paramSystemTime.Type = new CodeTypeReference( "Double" );

            this.doHandlingMethod.Parameters.Add( paramSystemTime );

            //������ ������� ��������� ����������, ���������� ��������� � ������, ���� ��� ������ �� �����
            CodeVariableDeclarationStatement messageDeclaration = new CodeVariableDeclarationStatement(
                "String", Builder.Routine.Receive.ReceivedMessage );
            //���������������� ��������
            CodeFieldReferenceExpression initExpr = new CodeFieldReferenceExpression();
            initExpr.TargetObject = new CodeArgumentReferenceExpression( Builder.IProcedure.Handling.SpyObjectNameInDoHandling );
            initExpr.FieldName = Builder.IProcedure.Handling.MessageField;

            messageDeclaration.InitExpression = initExpr;

            this.doHandlingMethod.Statements.Insert( 0, messageDeclaration );

            this.resultClass.Members.Add( this.doHandlingMethod );
            }


        /// <summary>
        /// �������� spy-������
        /// </summary>
        /// <param name="spyObjectType">��� spy-�������</param>
        public void AddSpyObject( ISpyType spyObjectType )
            {
            //��������� ��� ��� �������� � ������� ����������� spy-��������
            CodeParameterDeclarationExpression param = new CodeParameterDeclarationExpression();
            param.Name = spyObjectType.Name;
            
            //���� ��� ��������� spy-������
            if ( !( spyObjectType is IndexedType ) )
                param.Type = new CodeTypeReference( Builder.IProcedure.SpyObject );
            //���� ��� ������ spy-�������� (����� ���� �����������!!! - �� ���������� ��� ����������)
            else
                param.Type = new CodeTypeReference( Builder.IProcedure.SpyObject + "[]" );

            this.registerSpyObjectsMethod.Parameters.Add( param );

            //������� �����, �������������� spy-������
            AddRegisterSpyObjectMethod( spyObjectType );

            //���� ������ ������ �� ����������
            if ( spyObjectType is IExprType )
                {
                //�� ��������� ����������� ��������
                AddProperty( spyObjectType as IExprType );
                }
            }



        /// <summary>
        /// �������� �������� ��� �������, ��������� �� ����������
        /// </summary>
        /// <param name="spyVarType">������</param>
        private void AddProperty( IExprType spyVarType )
            {
            string baseTypeString = GetBaseTypeString( spyVarType );
            string typeStr = GetTypeString( baseTypeString, spyVarType );

            CodeMemberProperty property = new CodeMemberProperty();
            property.Name = spyVarType.Name;
            property.Type = new CodeTypeReference( typeStr );
            property.Attributes = MemberAttributes.Private;
            
            //�������� Get
            CodeMethodReturnStatement getValueStat = new CodeMethodReturnStatement( 
                new CodeSnippetExpression(
                "((" + typeStr + ")" + Builder.IProcedure.GetValueForVar + 
                "(new " + Builder.CoreName.Name + "(\"" + spyVarType.Name + "\")))" ) );
            
            property.GetStatements.Add( getValueStat );

            //�������� set
            CodeMethodInvokeExpression setValueStat = new CodeMethodInvokeExpression();
            setValueStat.Method = new CodeMethodReferenceExpression();
            setValueStat.Method.MethodName = Builder.IProcedure.SetValueForVar;
            setValueStat.Parameters.Add( new CodeSnippetExpression(
                "new " + Builder.CoreName.Name + "(\"" + spyVarType.Name + "\")" ) );
            setValueStat.Parameters.Add( new CodeArgumentReferenceExpression( "value" ) );

            property.SetStatements.Add( setValueStat );

            resultClass.Members.Add( property );
            }


        /// <summary>
        /// �������� �����, �������������� ���� spy-������
        /// </summary>
        /// <param name="spyObjectType">��� spy-�������</param>
        private void AddRegisterSpyObjectMethod( ISpyType spyObjectType )
            {
            CodeObjectCreateExpression coreNameCode = new CodeObjectCreateExpression();

            //���� ��� ��������� ������
            if ( !( spyObjectType is IndexedType ) )
                coreNameCode.CreateType = new CodeTypeReference( Builder.CoreName.Name );
            //���� ��� ������ ��������
            else
                coreNameCode.CreateType = new CodeTypeReference( Builder.CoreName.Range );

            coreNameCode.Parameters.Add( new CodePrimitiveExpression( spyObjectType.Name ) );

            //���� ��� ������ ��������
            if ( spyObjectType is IndexedType )
                {
                //���������������� ����� ���� ������ ���������� � ������
                IndexedType indexedType = spyObjectType as IndexedType;

                if ( indexedType != null )
                    {
                    foreach ( int maxIndex in indexedType )
                        {
                        //������ ������� ��������� ������ 0
                        coreNameCode.Parameters.Add( new CodePrimitiveExpression( 0 ) );
                        //������� ������� ��������� ����� �� ���� �������
                        coreNameCode.Parameters.Add( new CodePrimitiveExpression( maxIndex - 1 ) );
                        }
                    }
                }

            //����� ������� �����������
            CodeMethodInvokeExpression registerStat = new CodeMethodInvokeExpression();
            registerStat.Method = new CodeMethodReferenceExpression();
            registerStat.Method.MethodName = Builder.IProcedure.RegisterSpyObject;
            registerStat.Parameters.Add( new CodeArgumentReferenceExpression( spyObjectType.Name ) );
            registerStat.Parameters.Add( coreNameCode );

            this.registerSpyObjectsMethod.Statements.Add( registerStat ); 
            }


        /// <summary>
        /// �������� �����, �������������� ���������� ��������� ������� ��������
        /// </summary>
        /// <param name="spyObjectType">��� ������� ��������</param>
        public void AddSpyHandler( ISpyType spyObjectType )
            {
            //����� ������� �����������
            CodeMethodInvokeExpression registerStat = new CodeMethodInvokeExpression();
            registerStat.Method = new CodeMethodReferenceExpression();
            registerStat.Method.MethodName = Builder.IProcedure.RegisterSpyHandler;
            registerStat.Parameters.Add( new CodeArgumentReferenceExpression( spyObjectType.Name ) );
            registerStat.Parameters.Add( new CodeArgumentReferenceExpression( Builder.IProcedure.Handling.DoHandling ) );

            this.registerSpyObjectsMethod.Statements.Add( registerStat ); 
            }


        /// <summary>
        /// �������� out-����������
        /// </summary>
        /// <param name="varType">��� ����������</param>
        public void AddOutVariable( IExprType varType )
            {
            //��������� ��� ���������� ������
            this.AddVarDefinition( varType );

            //��������� ���������� ��� �������� � �������, ������������ out-����������
            CodeParameterDeclarationExpression param = new CodeParameterDeclarationExpression();
            param.Name = varType.Name;
            param.Type = new CodeTypeReference( GetTypeString( GetBaseTypeString( varType ), varType ) );
            param.Direction = FieldDirection.Out;

            this.getOutVarMethod.Parameters.Add( param );

            //���������� �������� out-����������
            CodeAssignStatement assignStat = new CodeAssignStatement();
            assignStat.Left = new CodeArgumentReferenceExpression( varType.Name );
            assignStat.Right = new CodeFieldReferenceExpression(
                new CodeThisReferenceExpression(), varType.Name );

            this.getOutVarMethod.Statements.Add( assignStat ); 
            }


        /// <summary>
        /// ������ ��� ��������, ������������ ��
        /// </summary>
        /// <param name="varType">��� ��������</param>
        public void SetIPResultType( IExprType varType )
            {
            //������ ��� ������������ ��������
            this.doProcessingMethod.ReturnType = new CodeTypeReference( GetBaseTypeString( varType ) );

            string baseType = GetBaseTypeString( varType );
            //������� ����������, �������������� ��������� (�� ��� ��������� � ������ ��)
            CodeVariableDeclarationStatement varDeclaration = new CodeVariableDeclarationStatement(
                baseType, this.resultClass.Name );
            //���������������� ��������
            CodeObjectCreateExpression initExpr = new CodeObjectCreateExpression();
            initExpr.CreateType = new CodeTypeReference( baseType );
            varDeclaration.InitExpression = initExpr;

            this.doProcessingMethod.Statements.Insert( 0, varDeclaration );

            //� ����� ���������� ��� ����������
            CodeMethodReturnStatement returnStat = new CodeMethodReturnStatement();
            returnStat.Expression = new CodeArgumentReferenceExpression( this.resultClass.Name );

            this.doProcessingMethod.Statements.Add( returnStat );
            }


        /// <summary>
        /// ������ ��������� � ������ processing
        /// </summary>
        /// <param name="statList">������ ����������</param>
        public void SetDoProcessing( CodeStatementCollection statList )
            {
            //��������������, ��� doProcessingMethod ��� �������� ��� ���������
            if ( this.doProcessingMethod.Statements.Count < 2 )
                return;

            //������� ���� � ���� ������ �.�. doProcessingMethod ��� �������� ��������� ��� (�� SetIPResultType)
            foreach ( CodeStatement stat in statList )
                this.doProcessingMethod.Statements.Insert( doProcessingMethod.Statements.Count - 1, stat );
            }


        /// <summary>
        /// ������ ��������� � ������ handling
        /// </summary>
        /// <param name="statList">������ ����������</param>
        public void SetDoHandling( CodeStatementCollection statList )
            {
            this.doHandlingMethod.Statements.AddRange( statList );
            }


        /// <summary>
        /// ������������� ��������� ����
        /// </summary>
        public override void Reload()
            {
            base.Reload();
            CreateRegisterSpyObjectMethod();
            CreateGetOutVarMethod();
            CreateDoHandlingMethod();
            CreateDoProcessingMethod();            
            }


        /// <summary>
        /// ����� �������������� spy-�������
        /// </summary>
        private CodeMemberMethod registerSpyObjectsMethod = new CodeMemberMethod();
        /// <summary>
        /// �����, ������������ ��� out-����������
        /// </summary>
        private CodeMemberMethod getOutVarMethod = new CodeMemberMethod();
        /// <summary>
        /// �����, ������������ �������� ��
        /// </summary>
        private CodeMemberMethod doProcessingMethod = new CodeMemberMethod();
        /// <summary>
        /// �����, �������������� ��������� spy-��������
        /// </summary>
        private CodeMemberMethod doHandlingMethod = new CodeMemberMethod();
        }
    }
