using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;

namespace TriadCompiler
    {
    /// <summary>
    /// ��������� Test
    /// </summary>
    internal struct TestConst
        {
        /// <summary>
        /// ����, ���������� ������ ������ � �������� ������ � ������ � ������ ������ ��� �����
        /// </summary>
        public const string RoutineTestFileList = "..\\..\\Test\\File\\Routine\\TestList.txt";
        /// <summary>
        /// ����, ���������� ������ ������ � �������� ������ � ������ � ������ ������ ��� ��������
        /// </summary>
        public const string StructureTestFileList = "..\\..\\Test\\File\\Structure\\TestList.txt";
        /// <summary>
        /// ����, ���������� ������ ������ � �������� ������ � ������ � ������ ������ ��� �������
        /// </summary>
        public const string ModelTestFileList = "..\\..\\Test\\File\\Model\\TestList.txt";
        /// <summary>
        /// ����, ���������� ������ ������ � �������� ������ � ������ � ������ ������ ��� ���. ��������
        /// </summary>
        public const string IProcedureFileList = "..\\..\\Test\\File\\InfProcedure\\TestList.txt";
        /// <summary>
        /// ����, ���������� ������ ������ � �������� ������ � ������ � ������ ������ ��� ������� �������������
        /// </summary>
        public const string IConditionFileList = "..\\..\\Test\\File\\SimCondition\\TestList.txt";

        /// <summary>
        /// ��������� �� ������ �������� �����
        /// </summary>
        public const string CanNotReadTestFileListMessage = "������ ��������� ���� �� ������� ������";
        /// <summary>
        /// ��������� � �������� ������� ���� ��������� ������
        /// </summary>
        public const string WrongFormatErrorMessage = "�������� ������ ���� ��������� �� ������";
        /// <summary>
        /// ��������� � ����������� ������
        /// </summary>
        public const string ErrorWasNotExpectedMessage = "����������� ������";
        /// <summary>
        /// ��������� �� ���������� ��������� ������
        /// </summary>
        public const string ExpectedErrorIsMissing = "���������� ��������� ������";
        };
    


    /// <summary>
    /// ��������� ������� ������������
    /// </summary>
    [Flags]
    internal enum ObjectForTesting
        {
        /// <summary>
        /// ���������
        /// </summary>
        Structure = 1,
        /// <summary>
        /// ������
        /// </summary>
        Routine = 2,
        /// <summary>
        /// ������
        /// </summary>
        Model = 4,
        /// <summary>
        /// �������������� ���������
        /// </summary>
        InfProcedure = 8,
        /// <summary>
        /// ������� �������������
        /// </summary>
        SimCondition = 16,
        /// <summary>
        /// ������
        /// </summary>
        Design = 32
        }    


    /// <summary>
    /// ������������ ��� ������������
    /// </summary>
    internal class Test
            {
            /// <summary>
            /// ������ ������������
            /// </summary>
            /// <param name="objectForTesting">������ ��� ������������</param>
            public static void Start( ObjectForTesting objectForTesting )
                {                
                try
                    {
                    //������������ �����
                    if ( ( (short)objectForTesting & (short)ObjectForTesting.Routine ) == (short)ObjectForTesting.Routine )
                        {
                        Console.WriteLine( "\n\t������������ �����" );
                        DoTestList( TestConst.RoutineTestFileList, CompilerFacade.TestRoutine );
                        }

                    //������������ ��������
                    if ( ( (short)objectForTesting & (short)ObjectForTesting.Structure ) == (short)ObjectForTesting.Structure )
                        {
                        Console.WriteLine( "\n\t������������ ��������" );
                        DoTestList( TestConst.StructureTestFileList, CompilerFacade.TestStructure );
                        }


                    //������������ �������                    
                    if ( ( ( short )objectForTesting & ( short )ObjectForTesting.Model ) == ( short )ObjectForTesting.Model )
                        {
                        Console.WriteLine( "\n\t������������ �������" );
                        DoTestList( TestConst.ModelTestFileList, CompilerFacade.TestModel );
                        }
                    
                    //������������ �������������� ��������
                    if ( ( (short)objectForTesting & (short)ObjectForTesting.InfProcedure ) == (short)ObjectForTesting.InfProcedure )
                        {
                        Console.WriteLine( "\n\t������������ �������������� ��������" );
                        DoTestList( TestConst.IProcedureFileList, CompilerFacade.TestIProcedure );
                        }

                    //������������ ������� �������������
                    if ( ( (short)objectForTesting & (short)ObjectForTesting.SimCondition ) == (short)ObjectForTesting.SimCondition )
                        {
                        Console.WriteLine( "\n\t������������ ������� �������������" );
                        DoTestList( TestConst.IConditionFileList, CompilerFacade.TestICondition );
                        }
                    }
                catch ( IOException e )
                    {
                    throw new IOException( TestConst.CanNotReadTestFileListMessage, e );
                    }
                }


        /// <summary>
        /// ������� ������������
        /// </summary>
        /// <param name="io">����-�����</param>
        /// <param name="fileName">��� �����</param>
        private delegate void TestMethod( IO io, string fileName );

        
        /// <summary>
        /// ��������� ������ ������
        /// </summary>
        /// <param name="testListFileName">��� ����� � �������</param>
        /// <param name="testMethod">����� ������������</param>
        private static void DoTestList( string testListFileName, TestMethod testMethod )
            {
            using ( StreamReader testListFile = new StreamReader( testListFileName ) )
                {
                string codeFileName = "";
                Output output = new ConsoleOutput();

                string testDirPath = Path.GetDirectoryName( testListFileName );
                //������ ����� �� ������� ������ ��� ����� 
                while ( testListFile.Peek() >= 0 )
                    {
                    codeFileName = testListFile.ReadLine();
                    InputFile input = new InputFile( testDirPath + "\\" + codeFileName );
                    IOTest io = new IOTest( input, output );
                    Console.WriteLine( "�������� ����� <" + codeFileName + ">" );
                    testMethod( io, codeFileName );
                    }
                }
            }
            }
        }
