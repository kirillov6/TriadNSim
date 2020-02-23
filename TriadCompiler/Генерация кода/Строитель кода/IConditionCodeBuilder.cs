using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

using TriadCompiler.Parser.Common.Expr;

namespace TriadCompiler
    {
    /// <summary>
    /// ��������� ���� ������� �������������
    /// </summary>
    class IConditionCodeBuilder : IProcedureCodeBuilder
        {
        /// <summary>
        /// �����, ����������� ������� ��������� �������������
        /// </summary>
        private CodeMemberMethod doCheckMethod = new CodeMemberMethod();


        /// <summary>
        /// �����������
        /// </summary>
        public IConditionCodeBuilder()
            {
            CreateDoCheckMethod();
            }


        /// <summary>
        /// ������� �����, ����������� ������� ��������� �������������
        /// </summary>
        private void CreateDoCheckMethod()
            {
            this.doCheckMethod = new CodeMemberMethod();
            this.doCheckMethod.Name = Builder.ICondition.DoCheck;
            this.doCheckMethod.Attributes = MemberAttributes.Override | MemberAttributes.Public;
            
            //����� ����������� ���������� ��������
            this.doCheckMethod.ReturnType = new CodeTypeReference( "Boolean" );

            //�������� - ������� ��������� �����
            CodeParameterDeclarationExpression param = new CodeParameterDeclarationExpression();
            param.Name = Builder.Routine.SystemTime;
            param.Type = new CodeTypeReference( "Double" );
            this.doCheckMethod.Parameters.Add(param);

            //
            //CodeParameterDeclarationExpression param2 = new CodeParameterDeclarationExpression(Builder.Structure.GraphClass, Builder.ICondition.CurrentModel);
            //this.doCheckMethod.Parameters.Add( param2 );

            //� ����� ������ ����� ���������� ������� �� ��������� ������������� �� ���������
            CodeMethodReturnStatement returnStat = new CodeMethodReturnStatement();
            returnStat.Expression = new CodePrimitiveExpression( true );

            this.doCheckMethod.Statements.Add( returnStat );

            this.resultClass.Members.Add( this.doCheckMethod );
            }


        /// <summary>
        /// ������ ��������� � ������, ����������� ������� ��������� �������������
        /// </summary>
        /// <param name="statList">������ ����������</param>
        public void SetDoCheckMethod( CodeStatementCollection statList )
            {
            //������ ���� ��������� ��������, ����������� �������� �� ���������
            if ( this.doCheckMethod.Statements.Count < 1 )
                return;

            CodeStatement returnStat = this.doCheckMethod.Statements[ this.doCheckMethod.Statements.Count - 1 ];
            this.doCheckMethod.Statements.Remove( returnStat );

            this.doCheckMethod.Statements.AddRange( statList );
            this.doCheckMethod.Statements.Add( returnStat );
            }


        //????????
        /// <summary>
        /// �������� ���������� ���������� c ��������������
        /// </summary>
        /// <param name="varType">��� ����������</param>
        /// <param name="initExpression">��� �������������</param>
        public override void AddVarDefinition(IExprType varType, CodeExpression initExpression)
        {
            if (initExpression == null)
                AddVarDefinition(varType);
            else
            {
                CodeMemberField field = new CodeMemberField();
                string baseSimpleType = GetBaseTypeString(varType);
                field = new CodeMemberField(GetTypeString(GetBaseTypeString(varType), varType), varType.Name);
                //field.InitExpression = initExpression;

                //���������������� � �-� Initialize
                CodeAssignStatement initArrayStat = new CodeAssignStatement();
                initArrayStat.Left = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
                    varType.Name);
                initArrayStat.Right = initExpression;
                initialMethod.Statements.Add(initArrayStat);

                field.Attributes = MemberAttributes.Private;
                resultClass.Members.Add(field);
            }
        }


        /// <summary>
        /// ������������� ��������� ����
        /// </summary>
        public override void Reload()
            {
            base.Reload();
            CreateDoCheckMethod();
            }
        }
    }
