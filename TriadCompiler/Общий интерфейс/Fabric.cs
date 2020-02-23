using System;
using System.Collections.Generic;
using System.Text;

using TriadCompiler.Code.Generator;

namespace TriadCompiler
    {
    /// <summary>
    /// ������ ������ �����������
    /// </summary>
    internal enum CodeBuilderMode
        {
        /// <summary>
        /// ���������� ������
        /// </summary>
        BuildModel,
        /// <summary>
        /// ���������� �����
        /// </summary>
        BuildRoutine,
        /// <summary>
        /// ���������� ��������
        /// </summary>
        BuildStructure,
          /// <summary>
        /// ���������� �������������� ��������
        /// </summary>
        BuildIProcedure,
        /// <summary>
        /// ���������� ������� �������������
        /// </summary>
        BuildICondition,
        /// <summary>
        /// ���������� �������
        /// </summary>
        BuildDesign,
        /// <summary>
        /// ������������ ������
        /// </summary>
        TestModel,
        /// <summary>
        /// ������������ �����
        /// </summary>
        TestRoutine,
        /// <summary>
        /// ������������ ��������
        /// </summary>
        TestStructure,
        /// <summary>
        /// ������������ �������������� ��������
        /// </summary>
        TestIProcedure,
        /// <summary>
        /// ������������ ������� �������������
        /// </summary>
        TestICondition,
        /// <summary>
        /// ������������ �������
        /// </summary>
        TestDesign
        }


    /// <summary>
    /// ������� ��� ������� ��� �������� ������ ����������
    /// </summary>
    internal abstract class Fabric
        {
        /// <summary>
        /// ���������� �����������
        /// </summary>
        protected Fabric()
            {
            }

        /// <summary>
        /// ��������� ������� (��������)
        /// </summary>
        public static Fabric Instance
            {
            get
                {
                if ( instance == null )
                    switch ( compileMode )
                        {
                        //���������� �������
                        case CodeBuilderMode.BuildModel:
                            instance = new ModelCompileFabric();
                            break;
                        //���������� ��������
                        case CodeBuilderMode.BuildStructure:
                            instance = new StructureCompileFabric();
                            break;
                        //���������� �����
                        case CodeBuilderMode.BuildRoutine:
                            instance = new RoutineCompileFabric();
                            break;
                        //���������� ��
                        case CodeBuilderMode.BuildIProcedure:
                            instance = new IProcedureCompileFabric();
                            break;
                        //���������� ������� �������������
                        case CodeBuilderMode.BuildICondition:
                            instance = new IConditionCompileFabric();
                            break;
                        //���������� �������
                        case CodeBuilderMode.BuildDesign:
                            instance = new DesignCompileFabric();
                            break;
                        //������������ �����
                        case CodeBuilderMode.TestRoutine:
                            instance = new RoutineTestFabric();
                            break;
                        //������������ ��������
                        case CodeBuilderMode.TestStructure:
                            instance = new StructureTestFabric();
                            break;
                        //������������ �������
                        case CodeBuilderMode.TestModel:
                            instance = new ModelTestFabric();
                            break;
                        //������������ �������������� ��������
                        case CodeBuilderMode.TestIProcedure:
                            instance = new IProcedureTestFabric();
                            break;
                        //������������ ������� �������������
                        case CodeBuilderMode.TestICondition:
                            instance = new IConditionTestFabric();
                            break;
                        //������������ �������
                        case CodeBuilderMode.TestDesign:
                            instance = new DesignTestFabric();
                            break;
                        default:
                            throw new InvalidOperationException( "������������ ����� ������ ������� �������" );
                        }
                return instance;
                }
            set
                {
                instance = value;
                }
            }


        /// <summary>
        /// ������� ����-�����
        /// </summary>
        public static IO IO
            {
            get
                {
                return currentIO;
                }
            set
                {
                currentIO = value;              
                }
            }


        /// <summary>
        /// ����������� ������� ������� ��� ����� ����������
        /// </summary>
        public static void ReloadFabric( CodeBuilderMode mode )
            {
            //���� ������� ��� �������
            if ( instance != null )
                {
                //������������� ������� � ����� ������
                scanner.Reload();

                //���� ����� ������ �� ���������, �� ������������� ��� ������
                if ( compileMode == mode )
                    {
                    //������� ����������� ������
                    Instance.ErrReg.Reload(); 
                 
                    //������������� ����������� ����
                    Instance.Builder.Reload();
                    }
                //���� ����� ������ ���������, �� ������� ����� �������
                else
                    {
                    compileMode = mode;
                    instance = null;
                    }
                }
            //����� ������ ������� ����� �������
            else
                {                
                compileMode = mode;
                }
            }


        /// <summary>
        /// �������� ����������� ������
        /// </summary>
        public virtual ErrorReg ErrReg
            {
            get
                {
                if ( this.err == null )
                    this.err = new ErrorReg();
                return this.err;
                }
            }


        /// <summary>
        /// �������� ������ ��������
        /// </summary>
        public Scanner Scanner
            {
            get
                {
                return scanner;
                }
            }


        /// <summary>
        /// �������� ����������� ����
        /// </summary>
        public virtual CodeBuilder Builder
            {
            get
                {
                if ( this.codeBuilder == null )
                    this.codeBuilder = new CodeBuilder();
                return this.codeBuilder;
                }
            set
                {
                this.codeBuilder = value;
                }
            }


        /// <summary>
        /// �������� ������
        /// </summary>
        public virtual CommonParser Parser
            {
            get
                {
                throw new InvalidOperationException( "����������� ������" );
                }
            set
                {
                this.parser = value;
                }
            }


        /// <summary>
        /// ��������� ����� ������
        /// </summary>
        private static Fabric instance = null;
        /// <summary>
        /// ������� �����
        /// </summary>
        private static CodeBuilderMode compileMode = CodeBuilderMode.BuildStructure;
        /// <summary>
        /// ������� ����-�����
        /// </summary>
        private static IO currentIO = new IO( new Input(), new Output() );

        /// <summary>
        /// ����������� ������
        /// </summary>
        protected ErrorReg err = null;
        /// <summary>
        /// ������ ��������
        /// </summary>
        protected static Scanner scanner = new Scanner();
        /// <summary>
        /// ������
        /// </summary>
        protected CommonParser parser = null;
        /// <summary>
        /// ����������� ����
        /// </summary>
        protected CodeBuilder codeBuilder = null;
        /// <summary>
        /// ��������� ����
        /// </summary>
        protected CommonGenerator codeGenerator = null;
        }


    /// <summary>
    /// ������� ������� ��� ���������� ������
    /// </summary>
    internal class ModelCompileFabric : Fabric
        {
        /// <summary>
        /// �������� ������
        /// </summary>
        public override CommonParser Parser
            {
            get
                {
                if ( this.parser == null )
                    this.parser = new ModelParser();
                return this.parser;
                }
            }


        /// <summary>
        /// �������� ����������� ����
        /// </summary>
        public override CodeBuilder Builder
            {
            get
                {
                if ( this.codeBuilder == null )
                    this.codeBuilder = new GraphCodeBuilder();
                return this.codeBuilder;
                }
            }
        }

    
    /// <summary>
    /// ������� ������� ��� ���������� ��������
    /// </summary>
    internal class StructureCompileFabric : Fabric
        {
        /// <summary>
        /// �������� ������
        /// </summary>
        public override CommonParser Parser
            {
            get
                {
                if ( this.parser == null )
                    this.parser = new StructureParser();
                return this.parser;
                }
            }

        /// <summary>
        /// �������� ����������� ����
        /// </summary>
        public override CodeBuilder Builder
            {
            get
                {
                if ( this.codeBuilder == null )
                    this.codeBuilder = new GraphCodeBuilder();
                return this.codeBuilder;
                }
            }
        }


    /// <summary>
    /// ������� ������� ��� ���������� �����
    /// </summary>
    internal class RoutineCompileFabric : Fabric
        {
        /// <summary>
        /// �������� ������
        /// </summary>
        public override CommonParser Parser
            {
            get
                {
                if ( this.parser == null )
                    this.parser = new RoutineParser();
                return this.parser;
                }
            }

        /// <summary>
        /// �������� ����������� ����
        /// </summary>
        public override CodeBuilder Builder
            {
            get
                {
                if ( this.codeBuilder == null )
                    this.codeBuilder = new RoutineCodeBuilder();
                return this.codeBuilder;
                }
            }
        }


    /// <summary>
    /// ������� ������� ��� ���������� �������������� ��������
    /// </summary>
    internal class IProcedureCompileFabric : Fabric
        {
        /// <summary>
        /// �������� ������
        /// </summary>
        public override CommonParser Parser
            {
            get
                {
                if ( this.parser == null )
                    this.parser = new InfProcedureParser();
                return this.parser;
                }
            }

        /// <summary>
        /// �������� ����������� ����
        /// </summary>
        public override CodeBuilder Builder
            {
            get
                {
                if ( this.codeBuilder == null )
                    this.codeBuilder = new IProcedureCodeBuilder();
                return this.codeBuilder;
                }
            }
        }


    /// <summary>
    /// ������� ������� ��� ���������� ������� �������������
    /// </summary>
    internal class IConditionCompileFabric : Fabric
        {
        /// <summary>
        /// �������� ������
        /// </summary>
        public override CommonParser Parser
            {
            get
                {
                if ( this.parser == null )
                    this.parser = new SimConditionParser();
                return this.parser;
                }
            }


        /// <summary>
        /// �������� ����������� ����
        /// </summary>
        public override CodeBuilder Builder
            {
            get
                {
                if ( this.codeBuilder == null )
                    this.codeBuilder = new IConditionCodeBuilder();
                return this.codeBuilder;
                }
            }
        }


    /// <summary>
    /// ������� ������� ��� ���������� �������
    /// </summary>
    internal class DesignCompileFabric : Fabric
        {
        /// <summary>
        /// �������� ������
        /// </summary>
        public override CommonParser Parser
            {
            get
                {
                if ( this.parser == null )
                    this.parser = new DesignParser();
                return this.parser;
                }
            }


        /// <summary>
        /// �������� ����������� ����
        /// </summary>
        public override CodeBuilder Builder
            {
            get
                {
                if ( this.codeBuilder == null )
                    this.codeBuilder = new GraphCodeBuilder();
                return this.codeBuilder;
                }
            }
        }



    /// <summary>
    /// ������� ������� ��� ������������ ������
    /// </summary>
    internal class ModelTestFabric : ModelCompileFabric
        {
        /// <summary>
        /// �������� ����������� ������
        /// </summary>
        public override ErrorReg ErrReg
            {
            get
                {
                if ( this.err == null )
                    this.err = new TestErrorReg();
                return this.err;
                }
            }
        }


    /// <summary>
    /// ������� ������� ��� ������������ �����
    /// </summary>
    internal class RoutineTestFabric : RoutineCompileFabric
        {
        /// <summary>
        /// �������� ����������� ������
        /// </summary>
        public override ErrorReg ErrReg
            {
            get
                {
                if ( this.err == null )
                    this.err = new TestErrorReg();
                return this.err;
                }
            }
        }


    /// <summary>
    /// ������� ������� ��� ������������ ��������
    /// </summary>
    internal class StructureTestFabric : StructureCompileFabric
        {
        /// <summary>
        /// �������� ����������� ������
        /// </summary>
        public override ErrorReg ErrReg
            {
            get
                {
                if ( this.err == null )
                    this.err = new TestErrorReg();
                return this.err;
                }
            }
        }


    /// <summary>
    /// ������� ������� ��� ������������ �������������� ��������
    /// </summary>
    internal class IProcedureTestFabric : IProcedureCompileFabric
        {
        /// <summary>
        /// �������� ����������� ������
        /// </summary>
        public override ErrorReg ErrReg
            {
            get
                {
                if ( this.err == null )
                    this.err = new TestErrorReg();
                return this.err;
                }
            }
        }


    /// <summary>
    /// ������� ������� ��� ������������ ������� �������������
    /// </summary>
    internal class IConditionTestFabric : IConditionCompileFabric
        {
        /// <summary>
        /// �������� ����������� ������
        /// </summary>
        public override ErrorReg ErrReg
            {
            get
                {
                if ( this.err == null )
                    this.err = new TestErrorReg();
                return this.err;
                }
            }
        }


    /// <summary>
    /// ������� ������� ��� ������������ �������
    /// </summary>
    internal class DesignTestFabric : DesignCompileFabric
        {
        /// <summary>
        /// �������� ����������� ������
        /// </summary>
        public override ErrorReg ErrReg
            {
            get
                {
                if ( this.err == null )
                    this.err = new TestErrorReg();
                return this.err;
                }
            }
        }
        
    }