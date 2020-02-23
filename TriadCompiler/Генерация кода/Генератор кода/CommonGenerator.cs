using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;

namespace TriadCompiler.Code.Generator
    {
    /// <summary>
    /// ��������� ����
    /// </summary>
    internal class CommonGenerator
        {
        /// <summary>
        /// ���� � ����� � ����� ����
        /// </summary>
        protected const string CoreFilePath = "TriadCore.dll";


        /// <summary>
        /// �����������
        /// </summary>
        public CommonGenerator()
            {
            Reload();
            }


        /// <summary>
        /// ���������� ���� ������ � ������
        /// </summary>
        /// <param name="typeCode"></param>
        public void AddTypeInUnit( CodeTypeDeclaration typeCode )
            {
            namespaceCode.Types.Add( typeCode );
            }


        /// <summary>
        /// �������� ������ �� ������ ������
        /// </summary>
        /// <param name="fileName">��� ������</param>
        public virtual void AddReference( string fileName )
            {
            }


        /// <summary>
        /// ������� ���
        /// </summary>
        /// <param name="fileName">��� ������</param>
        public virtual void GenerateCode( string fileName )
            {
            }


        /// <summary>
        /// ����������� ��������� ���� � ����� ����������
        /// </summary>
        public virtual void Reload()
            {
            unitCode = new CodeCompileUnit();

            namespaceCode = new CodeNamespace( Builder.Common.Namespace );
            namespaceCode.Imports.Add( new CodeNamespaceImport( "System" ) );
            namespaceCode.Imports.Add( new CodeNamespaceImport( "System.Collections.Generic" ) );
            namespaceCode.Imports.Add( new CodeNamespaceImport( "System.Collections" ) );

            unitCode.Namespaces.Add( namespaceCode );
            }


        /// <summary>
        /// ����������� ������
        /// </summary>
        protected CodeCompileUnit unitCode = new CodeCompileUnit();
        /// <summary>
        /// ������������ ����
        /// </summary>
        protected CodeNamespace namespaceCode = new CodeNamespace( Builder.Common.Namespace );
        }
    }
