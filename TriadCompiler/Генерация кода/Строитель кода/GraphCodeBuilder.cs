using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace TriadCompiler
    {
    /// <summary>
    /// ���������� ����������� ����, ���������� �� ������ Build()
    /// !!! ��� ����������-����� ����� ������ ������ ��������������� � ������ Reload
    /// </summary>
    internal class GraphCodeBuilder : CodeBuilder
        {
        /// <summary>
        /// �����������
        /// </summary>
        public GraphCodeBuilder()
            {
            //������� ����� ���������� ���������
            CreateBuildMethod();
            }

        /// <summary>
        /// ������� �����, �������� ���������
        /// </summary>
        protected virtual void CreateBuildMethod()
            {
            //��������� �����
            this.buildGraphMethod = new CodeMemberMethod();
            this.buildGraphMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            this.buildGraphMethod.Name = Builder.Common.BuildMethod;

            this.resultClass.Members.Add(buildGraphMethod);
            }


        /// <summary>
        /// �������� ���������� ����������
        /// </summary>
        /// <param name="varType">��� ����������</param>
        public override void AddVarDefinition( IExprType varType )
            {
                CodeMemberField field = new CodeMemberField();

                string baseSimpleType = GetBaseTypeString(varType);
                field = new CodeMemberField(GetTypeString(GetBaseTypeString(varType), varType), varType.Name);

                //���� ��� ������
                if (varType is VarArrayType)
                {
                    //���������������� ������� ����� � �-� Initialize
                    //����� ��� ������������� ������ ����� ����� ����� �������
                    CodeAssignStatement initArrayStat = new CodeAssignStatement();
                    initArrayStat.Left = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
                        varType.Name);
                    initArrayStat.Right = new CodeSnippetExpression(
                        GetIndexFieldInitialization(baseSimpleType, varType as VarArrayType));
                    this.buildGraphMethod.Statements.Insert(0, initArrayStat);
                }
                //���� ��� ���������
                else if (varType is SetType)
                {
                    field.InitExpression = new CodeSnippetExpression(GetSetFieldInitialization());
                }
                //=========By jum==========
                else if (varType.Code == TypeCode.Node)
                {
                     CodeObjectCreateExpression createExpr = new CodeObjectCreateExpression(Builder.Common.NodeClassName, new CodeObjectCreateExpression(Builder.CoreName.Name, new CodePrimitiveExpression(varType.Name)));
                     field.InitExpression = createExpr; 
                }

                field.Attributes = MemberAttributes.Private;
                resultClass.Members.Add(field);
            }

        
        /// <summary>
        /// �������� ���, �������� ���������
        /// </summary>
        /// <param name="statementList">������������������ ����������</param>
        public void AddBuildStatementList( CodeStatementCollection statementList )
            {
            this.buildGraphMethod.Statements.AddRange( statementList );
            }


        /// <summary>
        /// ����������� ����������� ���� ��� ����� ����������
        /// </summary>
        public override void Reload()
            {
            base.Reload();
            CreateBuildMethod();
            }


        /// <summary>
        /// ������������� ���
        /// </summary>
        public override void Build()
            {
            this.buildGraphMethod.ReturnType = new CodeTypeReference( Builder.Structure.GraphClass );

            //��������� ��������, ������������ ��������� � �����, �������� ���������
            CodeMethodReturnStatement returnStatement = new CodeMethodReturnStatement(
                 new CodeVariableReferenceExpression( this.resultClass.Name ) );                    
            this.buildGraphMethod.Statements.Add(returnStatement);

            base.Build();
            }


        /// <summary>
        /// �����, � ������� ��������� ���������
        /// </summary>
        protected CodeMemberMethod buildGraphMethod = null;
        }
    }
