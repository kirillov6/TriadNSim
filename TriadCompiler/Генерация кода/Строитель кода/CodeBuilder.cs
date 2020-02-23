using System;
using System.Collections.Generic;
using System.CodeDom;

using TriadCompiler.Code.Generator;

namespace TriadCompiler
    {
    /// <summary>
    /// ���������� ����������� ����
    /// !!! ��� ����������-����� ����� ������ ������ ��������������� � ������ Reload
    /// </summary>
    internal class CodeBuilder
        {
        /// <summary>
        /// ������ ��� ������
        /// </summary>
        /// <param name="className">��� ������������� ������</param>
        public void SetClassName( string className )
            {
            resultClass.Name = className;
            }


        /// <summary>
        /// �������� �������� � ����������� ������
        /// </summary>
        /// <param name="varType">��� ���������</param>
        /// <param name="varName">��� ���������</param>
        public void AddParameterInConstructor( IExprType varType, string varName )
            {
            //���� � ������ ��� �� ���� ������������, �� ��������� ���
            if ( this.codeConstructor == null )
                {
                this.codeConstructor = new CodeConstructor();
                this.codeConstructor.Attributes = MemberAttributes.Public;
                this.resultClass.Members.Add( this.codeConstructor );
                }

            CodeParameterDeclarationExpression constructorParam =
                new CodeParameterDeclarationExpression( GetTypeString( GetBaseTypeString( varType ), varType ), varName );

            this.codeConstructor.Parameters.Add( constructorParam );
           
            //��������� ��� ������ ����������� ���������
            //� ����������� ���������� ����������
            CodeAssignStatement assignStat = new CodeAssignStatement();
            assignStat.Left = new CodeFieldReferenceExpression( new CodeThisReferenceExpression(), varName );
            assignStat.Right = new CodeArgumentReferenceExpression( varName );

            this.codeConstructor.Statements.Add( assignStat );
            }


        /// <summary>
        /// �������� ���������� ����������
        /// </summary>
        /// <param name="varType">��� ����������</param>
        public virtual void AddVarDefinition( IExprType varType )
            {
            //����������� �-� �����, �.�. � ������ ����� ������������� ��������
            //����� ����������� ������ �-� Initialize
            //����� ��� ��������� ����� ������ �� ��������� ������
            //� ��� ����� ����� �������
            }

        /// <summary>
        /// �������� ���������� ���������� c ��������������
        /// </summary>
        /// <param name="varType">��� ����������</param>
        /// /// <param name="initExpression">��� �������������</param>
        public virtual void AddVarDefinition(IExprType varType, CodeExpression initExpression)
        {
            if (initExpression == null)
                AddVarDefinition(varType);
            else
            {
                CodeMemberField field = new CodeMemberField();
                string baseSimpleType = GetBaseTypeString(varType);
                field = new CodeMemberField(GetTypeString(GetBaseTypeString(varType), varType), varType.Name);
                field.InitExpression = initExpression;
                field.Attributes = MemberAttributes.Private;
                resultClass.Members.Add(field);
            }
        }

        /// <summary>
        /// �������� design ����������
        /// </summary>
        /// <param name="designVarType">��� design ����������</param>
        public static CodeStatementCollection GetDesignVarDefinitionStatements(IDesignVarType designVarType)
        {
            CodeStatementCollection resultStatList = new CodeStatementCollection();
            //��� ������, ��������������� ����������
            string varClass = "";

            switch (designVarType.TypeCode)
            {
                //�������� ����������
                case DesignTypeCode.Structure:
                    varClass = Builder.Structure.GraphClass;
                    break;
                //����������-������
                case DesignTypeCode.Routine:
                    varClass = Builder.Routine.BaseClass;
                    break;
                //����������-������
                case DesignTypeCode.Model:
                    varClass = Builder.Model.ModelClass;
                    break;
            }

            //��������� �������� ����������
            CodeVariableDeclarationStatement varDeclaration = new CodeVariableDeclarationStatement(
                GetTypeString(varClass, designVarType), designVarType.Name);

            resultStatList.Add(varDeclaration);

            //�������������� design ����������
            CodeAssignStatement assignStat = new CodeAssignStatement();
            //������������� ������� ( ���� ����� )
            CodeMethodInvokeExpression initializeArrayMethod = null;

            //���� ��� ������� ����������
            if (designVarType is DesignVarType)
            {
                assignStat = new CodeAssignStatement(new CodeVariableReferenceExpression(designVarType.Name),
                    new CodeObjectCreateExpression(varClass));
            }
            //���� ��� ������
            if (designVarType is DesignVarArrayType)
            {
                assignStat = new CodeAssignStatement(new CodeVariableReferenceExpression(designVarType.Name),
                    new CodeSnippetExpression(GetIndexFieldInitialization(varClass,
                    designVarType as DesignVarArrayType)));

                initializeArrayMethod = new CodeMethodInvokeExpression();
                initializeArrayMethod.Method = new CodeMethodReferenceExpression(
                    new CodeVariableReferenceExpression(Builder.Common.ArrayInitializing.InitializingClass),
                    Builder.Common.ArrayInitializing.InitializingMethod);

                initializeArrayMethod.Parameters.Add(new CodeArgumentReferenceExpression(designVarType.Name));

                //������� ������, ������� ����� ������������������ ������
                CodeObjectCreateExpression createStat = new CodeObjectCreateExpression();
                createStat.CreateType = new CodeTypeReference(varClass);
                initializeArrayMethod.Parameters.Add(createStat);
            }
            
            resultStatList.Add(assignStat);
            
            if (initializeArrayMethod != null)
                resultStatList.Add(initializeArrayMethod);

            return resultStatList;
        }

        /// <summary>
        /// �������� ���������� ����������
        /// </summary>
        /// <param name="varTypeList">������ ����� ����������</param>
        public void AddVarDefinition( List<IExprType> varTypeList )
            {
            foreach ( IExprType varType in varTypeList )
                AddVarDefinition( varType );
            }

        /// <summary>
        /// �������� ���������� ���������� c ��������������
        /// </summary>
        /// <param name="dict">������� ���->��� �������������</param>
        public void AddVarDefinition(Dictionary<IExprType, CodeExpression> dict)
        {
            foreach (KeyValuePair<IExprType, CodeExpression> pair in dict)
            {
                AddVarDefinition(pair.Key, pair.Value);
            }
        }


        /// <summary>
        /// ������������� ������������� ��� �������
        /// </summary>
        /// <param name="baseTypeString">������� ��� �������</param>
        /// <param name="indexType">���������� � �������</param>
        /// <returns></returns>
        protected static string GetIndexFieldInitialization( string baseTypeString, IndexedType indexType )
            {          
            //������������� �������
            string initStringCode = "new " + baseTypeString + " [";

            int index = 0;
            foreach ( int maxIndexValue in indexType )
                {
                if ( index != 0 )
                    initStringCode += ",";
                initStringCode += maxIndexValue.ToString();
                index++;
                }

            initStringCode += "]";

            return initStringCode;
            }


        /// <summary>
        /// ������������� ������������� ����������-���������
        /// </summary>
        /// <returns></returns>
        protected string GetSetFieldInitialization()
            {
            return "new " + Builder.Common.SetClassName + "()";
            }
        

        /// <summary>
        /// �������� ��������� ������ �������� ���� ���������� (��� �������� � ��������)
        /// </summary>
        /// <param name="varType">��� ����������</param>
        /// <returns></returns>
        public static string GetBaseTypeString( IExprType varType )
            {
            string typeStringCode;

            //���� ��� ���������
            if ( varType is SetType )
                typeStringCode = Builder.Common.SetClassName;
            //���� ��� ������� ����������
            else
                {
                switch ( varType.Code )
                    {
                    case TypeCode.Bit:
                        typeStringCode = "Int64";
                        break;
                    case TypeCode.Boolean:
                        typeStringCode = "Boolean";
                        break;
                    case TypeCode.Char:
                        typeStringCode = "Char";
                        break;
                    case TypeCode.Integer:
                        typeStringCode = "Int32";
                        break;
                    case TypeCode.Real:
                        typeStringCode = "Double";
                        break;
                    case TypeCode.String:
                        typeStringCode = "String";
                        break;
                    case TypeCode.UndefinedType:
                        typeStringCode = "Object";
                        break;
                    //by jum
                    case TypeCode.Node:
                        typeStringCode = Builder.Common.NodeClassName;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException( "����������� ��� ����������" );
                    }
                }

            return typeStringCode;
            }


        /// <summary>
        /// �������� ��������� ������������� ���� ���������� (�������� � ���������)
        /// </summary>
        /// <param name="baseTypeString">������� ��� ���������� (��� ��������)</param>
        /// <param name="varType">��� ����������</param>
        /// <returns></returns>
        public static string GetTypeString( string baseTypeString, ICommonType varType )
            {
            string resultStringCode = baseTypeString;

            if ( varType is IndexedType )
                {
                resultStringCode += "[";
                for ( int i = 1; i < ( varType as IndexedType ).IndexCount; i++ )
                    {
                    resultStringCode += ",";
                    }
                resultStringCode += "]";
                }

            return resultStringCode;
            }


        /// <summary>
        /// �������� ������� �������
        /// </summary>
        /// <param name="methodName">��� ��� ������</param>
        /// <param name="statementList">������ ����������</param>
        public void AddPrivateMethod( string methodName, CodeStatementCollection statementList )
            {
            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = methodName;
            method.Attributes = MemberAttributes.Private | MemberAttributes.Final;
            method.Statements.AddRange( statementList ); 

            this.AddMethod( method );
            }


        /// <summary>
        /// ������ ������������ �����
        /// </summary>
        /// <param name="baseClassName">��� ������������� ������</param>
        public void SetBaseClass( string baseClassName )
            {
            //��������� �� ������������� ������
            resultClass.BaseTypes.Add( new CodeTypeReference( baseClassName ) );
            }


        /// <summary>
        /// �������� �����
        /// </summary>
        /// <param name="method">�������� ������</param>
        public void AddMethod( CodeMemberMethod method )
            {
            resultClass.Members.Add( method );
            }

        
        /// <summary>
        /// ������������� ���
        /// </summary>
        public virtual void Build()
            {
            //���� ���� ���������������� ������, �� ������������ ��� �� ����
            if ( Fabric.Instance.ErrReg.ErrorCount > 0 )
                return;

            CodeFabric.Instance.AddTypeInUnit( resultClass );            
            }


        /// <summary>
        /// ����������� ����������� ���� ��� ����� ����������
        /// </summary>
        public virtual void Reload()
            {
            this.resultClass = new CodeTypeDeclaration();
            this.codeConstructor = null;
            }


        /// <summary>
        /// �������� �����
        /// </summary>
        protected CodeTypeDeclaration resultClass = new CodeTypeDeclaration();
        /// <summary>
        /// ����������� ��������� ������ (=null, ���� �� �� �����)
        /// </summary>
        protected CodeConstructor codeConstructor = null;
        }
    }

    
