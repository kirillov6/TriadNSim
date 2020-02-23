using System;
using System.Collections.Generic;
using System.Text;

namespace TriadCompiler
    {
    /// <summary>
    /// ���� ������
    /// </summary>
    internal struct Err
        {
        /// <summary>
        /// ������ ������������ �������
        /// </summary>
        public struct Lexer
            {
            /// <summary>
            /// ������������ ����� ������ ���
            /// </summary>
            public const int BitStringIsTooLong = 64;
            /// <summary>
            /// �������� ������ ������ �������
            /// </summary>
            public const int WrongCharFormat = 100;
            /// <summary>
            /// ���������� �����������
            /// </summary>
            public const int NotClosedCommentary = 101;
            /// <summary>
            /// ���������� ������
            /// </summary>
            public const int NotClosedString = 102;
            /// <summary>
            /// ����������� ������
            /// </summary>
            public const int UnknownChar = 103;
            /// <summary>
            /// �������� ������ ������������� �����
            /// </summary>
            public const int WrongRealFormat = 104;
            /// <summary>
            /// �������� ������ ������ �����
            /// </summary>
            public const int WrongIntegerFormat = 105;
            /// <summary>
            /// ���������� ������� ������
            /// </summary>
            public const int NotClosedBitString = 106;
            /// <summary>
            /// ������������ ������ � ������� ������
            /// </summary>
            public const int WrongSymbolInBitSTring = 107;
            /// <summary>
            /// ������� ������� ������� ������
            /// </summary>
            public const int TooLongBitString = 108;
            }


        /// <summary>
        /// ������ ��������������� � �������������� �������
        /// </summary>
        public struct Parser
            {
            /// <summary>
            /// �������� ���
            /// </summary>
            public struct Type
                {
                /// <summary>
                /// �������� ��� ����������
                /// </summary>
                public struct Var
                    {
                    /// <summary>
                    /// ����������� ��� ����������
                    /// </summary>
                    public const int Unknown = 150;

                    /// <summary>
                    /// ����� ������ ���
                    /// </summary>
                    public struct Need
                        {
                        /// <summary>
                        /// ������ ���� �����
                        /// </summary>
                        public const int Integer = 155;
                        /// <summary>
                        /// ��� ��������� ������ ���� ����� ��� ������������
                        /// </summary>
                        public const int IntegerOrReal = 163;
                        /// <summary>
                        /// ��������� ���������� ���
                        /// </summary>
                        public const int Boolean = 168;
                        /// <summary>
                        /// ��� ��������� ������ ���� ���������
                        /// </summary>
                        public const int String = 172;
                        /// <summary>
                        /// ��� ��������� ������ ���� ���������� ��� �������
                        /// </summary> 
                        public const int BooleanOrBit = 173;
                        }

                    /// <summary>
                    /// ������������ � ���� ��������� ���
                    /// </summary>
                    public struct WrongType
                        {
                        /// <summary>
                        /// ������������� ���� � ������� ���������
                        /// </summary>
                        public const int InSimpleExpr = 302;
                        /// <summary>
                        /// ������������� ���� � ��������� assign
                        /// </summary>
                        public const int InAssign = 308;
                        /// <summary>
                        /// ������������ ���� � �������� +
                        /// </summary>
                        public const int InPlus = 303;
                        /// <summary>
                        /// ������������ ���� � �������� Or/And
                        /// </summary>
                        public const int InOrAnd = 304;
                        /// <summary>
                        /// ������������ ���� � �������� Star/Slash
                        /// </summary>
                        public const int InStarSlash = 306;
                        /// <summary>
                        /// ������������ ���� � �������� Residue of division (������� �� �������)
                        /// </summary>
                        public const int InResidueOfDivision = 325;
                        /// <summary>
                        /// ������������ ���� � �������� ARROW
                        /// </summary>
                        public const int InArrow = 307;
                        /// <summary>
                        /// ������ ����� �������� �����������
                        /// </summary>
                        public const int NotCompatibleDimensionArrayInAssign = 310;
                        /// <summary>
                        /// ������������ ���� ��� �������� �������� �������
                        /// </summary>
                        public const int InReturn = 311;
                        }
                    }

                /// <summary>
                /// �������� ��� ������
                /// </summary>
                public struct Polus
                    {
                    /// <summary>
                    /// ����� ������ ��� ������
                    /// </summary>
                    public struct Need
                        {
                        /// <summary>
                        /// ��� ������ ������ ���� Output
                        /// </summary>
                        public const int Output = 165;
                        /// <summary>
                        /// ��� ������ ������ ���� Input
                        /// </summary>
                        public const int Input = 166;
                        }
                    }
                }


            /// <summary>
            /// ������������ ��������
            /// </summary>
            public struct Value
                {
                /// <summary>
                /// ����� ������ ��������
                /// </summary>
                public struct Need
                    {
                    /// <summary>
                    /// ������ ���� ���������
                    /// </summary>
                    public const int Constant = 156;
                    /// <summary>
                    /// ��������� ������ ���� ���������������
                    /// </summary>
                    public const int NotNegative = 169;
                    /// <summary>
                    /// ��� � ����� for ������ ���� �������������
                    /// </summary>
                    public const int Positive = 171;
                    /// <summary>
                    /// �������� ��������� �� ������ ���� ����������
                    /// </summary>
                    public const int NotConstant = 159;
                    }
                }


            /// <summary>
            /// �������� ��������
            /// </summary>
            public struct Usage
                {
                /// <summary>
                /// �������� ������������ ����� �����
                /// </summary>
                public const int WrongMinusUsage = 151;

                /// <summary>
                /// ������ � ����� ������ ��� ������
                /// </summary>
                public const int DeclaredAgain = 300;
                /// <summary>
                /// ������ � ����� ������ �� ��� ������
                /// </summary>
                public const int NotDeclared = 301;
                /// <summary>
                /// ������������� ���������������� ���� �� �����������
                /// </summary>
                public const int NeedNotIndexed = 305;
                /// <summary>
                /// ��������� spy-������
                /// </summary>
                public const int NeedSpyObject = 311;
                /// <summary>
                /// ���������� ������� �������� ��������
                /// </summary>
                public const int NeedRange = 323;
                /// <summary>
                /// ��������� �������� �� ���������
                /// </summary>
                public const int NeedNotRange = 324;
                /// <summary>
                /// ���-��������� ����� �����������
                /// </summary>
                public const int NeedNotSet = 313;
                /// <summary>
                /// ��������� ������ ��� ��-��
                /// </summary>
                public const int NeedIndexedOrSet = 314;
                /// <summary>
                /// ��� ��������� �� ����� ���� �������� � ���������� ����
                /// </summary>
                public const int UnableToCastType = 315;


                /// <summary>
                /// �������� ������������� ������
                /// </summary>
                public struct Polus
                    {
                    /// <summary>
                    /// ������ ������� ��������� ����������� �������
                    /// </summary>
                    public const int LowIndexIsGreaterThanTopInRange = 320;
                    }


                /// <summary>
                /// �������� ������������� �������
                /// </summary>
                public struct Event
                    {
                    /// <summary>
                    /// ��������� � ������������ ����� �������
                    /// </summary> 
                    public const int NotDeclared = 311;
                    /// <summary>
                    /// ������� � ����� ������ ��� ���� �������
                    /// </summary>
                    public const int DeclaredAgain = 312;
                    }


                /// <summary>
                /// �������� ������������� design ����������
                /// </summary>
                public struct DesignVar
                    {
                    /// <summary>
                    /// ��� design ���������� �� ������������� ���� design ����
                    /// </summary>
                    public const int NotCompatibleWithDesignType = 318;
                    /// <summary>
                    /// ����������� ��� design ����������
                    /// </summary>
                    public const int NotExpectedTypeCode = 316;
                    }


                /// <summary>
                /// ��������� ���
                /// </summary>
                public struct DesignType
                    {
                    /// <summary>
                    /// ��������� ��� � ����� ������ ������ �� ���
                    /// </summary>
                    public const int NotDeclared = 315;
                    }


                /// <summary>
                /// �������������� ���������
                /// </summary>
                public struct IProcedure
                    {
                    /// <summary>
                    /// �� �� ����������� �������� ��������
                    /// </summary>
                    public const int NoReturnedValue = 312;
                    }


                /// <summary>
                /// ������ ����������
                /// </summary>
                public struct ParameterList
                    {
                    /// <summary>
                    /// ������� �� ��� ���������
                    /// </summary>
                    public const int NotEnoughParameters = 321;
                    /// <summary>
                    /// ������� ������ ���������
                    /// </summary>
                    public const int TooManyParameters = 322;
                    }


                /// <summary>
                /// �������� ������������� �������
                /// </summary>
                public struct Array
                    {
                    /// <summary>
                    /// ���������� �� �������� ��������
                    /// </summary>
                    public const int VarIsNotArray = 157;
                    /// <summary>
                    /// ������� �� ��� �������
                    /// </summary>
                    public const int LostIndex = 158;
                    /// <summary>
                    /// ������� ������ �������
                    /// </summary>
                    public const int TooManyIndexes = 317;
                    /// <summary>
                    /// ��� �� ��������������� ����������
                    /// </summary>
                    public const int ArrayIsNotVar = 164;
                    /// <summary>
                    /// ������ ������� �� ���������� �������
                    /// </summary>
                    public const int OutOfArrayBound = 160;
                    }


                /// <summary>
                /// �������� ������������� ����� for
                /// </summary>
                public struct For
                    {
                    /// <summary>
                    /// �������� ���������� ��������� � ����� for to ������ ���������
                    /// </summary>
                    public const int InitExprIsGreaterThanTerminal = 161;
                    /// <summary>
                    /// �������� ���������� ��������� � ����� for downto ������ ���������
                    /// </summary>
                    public const int InitExprIsLowerThanTerminal = 162;
                    }

                
                /// <summary>
                /// �������� ������������� ����� foreach
                /// </summary>
                public struct Foreach
                    {
                    /// <summary>
                    /// ��� ����������-�������� � ���������/������� �� ���������
                    /// </summary>
                    public const int IncompatibleTypes = 174;
                    }

                
                /// <summary>
                /// ������������ ��������
                /// </summary>
                public struct Context
                    {
                    /// <summary>
                    /// �������� �������� Case
                    /// </summary>
                    public const int Case = 167;
                    /// <summary>
                    /// ������������ �������� ���������� ����������
                    /// </summary>
                    public const int VarDeclaration = 152;
                    }
                }


            /// <summary>
            /// �������� ��������� �������
            /// </summary>
            public struct WrongStartSymbol
                {
                /// <summary>
                /// ������
                /// </summary>
                public const int Routine = 200;
                /// <summary>
                /// ���
                /// </summary>
                public const int Type = 201;
                /// <summary>
                /// ���������� ������
                /// </summary>
                public const int PolusDeclaration = 202;
                /// <summary>
                /// ��� ������ � ����������
                /// </summary>
                public const int PolusDeclarationName = 204;
                /// <summary>
                /// ��� ���������� � ����������
                /// </summary>
                public const int VarDeclarationName = 205;
                /// <summary>
                /// ������� ���������
                /// </summary>
                public const int SimpleFactor = 207;
                /// <summary>
                /// ��������
                /// </summary>
                public const int Statement = 209;
                /// <summary>
                /// ���������� �������
                /// </summary>
                public const int EventDeclarationName = 211;
                /// <summary>
                /// ���������
                /// </summary>
                public const int Structure = 212;
                /// <summary>
                /// ���������� ����������� ����������
                /// </summary>
                public const int StuctVarDeclaration = 213;
                /// <summary>
                /// ����������� ���������
                /// </summary>
                public const int StructFactor = 214;
                /// <summary>
                /// ���������� design ����������
                /// </summary>
                public const int DesignVarDeclaration = 215;
                /// <summary>
                /// ���������� �������
                /// </summary>
                public const int NodeDeclaration = 218;
                /// <summary>
                /// ����������
                /// </summary>
                public const int Connection = 220;
                /// <summary>
                /// ����������� ���������
                /// </summary>
                public const int StructConstant = 222;
                /// <summary>
                /// ������ include
                /// </summary>
                public const int IncludeSection = 223;
                /// <summary>
                /// ������
                /// </summary>
                public const int Model = 224;
                /// <summary>
                /// ������
                /// </summary>
                public const int HeaderName = 225;
                /// <summary>
                /// ������ ���������� �������
                /// </summary>
                public const int FunctionParameterList = 226;
                /// <summary>
                /// �������������� ���������
                /// </summary>
                public const int IProcedure = 227;
                /// <summary>
                /// ������� �������������
                /// </summary>
                public const int ICondition = 228;
                /// <summary>
                /// ���������� spy-�������
                /// </summary>
                public const int SpyObjectDeclaration = 231;
                /// <summary>
                /// ������ �� ������ ��� �������� ��������
                /// </summary>
                public const int ObjectReference = 232;
                /// <summary>
                /// ������� � ��������� case ��
                /// </summary>
                public const int IPCaseCondition = 221;
                /// <summary>
                /// ������
                /// </summary>
                public const int Design = 233;
                /// <summary>
                /// Spy-������
                /// </summary>
                public const int SpyObject = 234;
                }


            /// <summary>
            /// �������� �������� �������
            /// </summary>
            public struct WrongEndSymbol
                {
                /// <summary>
                /// ���
                /// </summary>
                public const int Type = 250;
                /// <summary>
                /// ��� ������ � ��� ����������
                /// </summary>
                public const int PolusDeclarationName = 252;
                /// <summary>
                /// ������ �������������
                /// </summary>
                public const int InitialPart = 253;
                /// <summary>
                /// �������
                /// </summary>
                public const int Event = 254;
                /// <summary>
                /// ���������
                /// </summary>
                public const int Constant = 255;
                /// <summary>
                /// ��� ���������� � ����������
                /// </summary>
                public const int VarDeclarationName = 256;
                /// <summary>
                /// ��������
                /// </summary>
                public const int Statement = 257;
                /// <summary>
                /// ������ ����������
                /// </summary>
                public const int PolusVarIndex = 259;
                /// <summary>
                /// ��������� � �������
                /// </summary>
                public const int ExprInPar = 260;
                /// <summary>
                /// ������
                /// </summary>
                public const int Routine = 261;
                /// <summary>
                /// ��� �������
                /// </summary>
                public const int EventDeclarationName = 262;
                /// <summary>
                /// ���������
                /// </summary>
                public const int Structure = 263;
                /// <summary>
                /// ���������� �������
                /// </summary>
                public const int NodeDeclaration = 265;
                /// <summary>
                /// ���������� design ����������
                /// </summary>
                public const int DesignVarDeclaration = 267;
                /// <summary>
                /// ��������� ���������
                /// </summary>
                public const int SingleHeader = 268;
                /// <summary>
                /// ����������� ���������
                /// </summary>
                public const int StructConstant = 269;
                /// <summary>
                /// ����������
                /// </summary>
                public const int Connection = 273;
                /// <summary>
                /// ������ include
                /// </summary>
                public const int IncludeSection = 275;
                /// <summary>
                /// ������
                /// </summary>
                public const int Model = 276;
                /// <summary>
                /// ��� ������
                /// </summary>
                public const int HeaderName = 277;
                /// <summary>
                /// ������ ���������� �������
                /// </summary>
                public const int FunctionParameterList = 278;
                /// <summary>
                /// ������ ��������� �������������� ���������
                /// </summary>
                public const int Handling = 279;
                /// <summary>
                /// ������ �������������� ��������� �������������� ���������
                /// </summary>
                public const int Processing = 280;
                /// <summary>
                /// �������������� ���������
                /// </summary>
                public const int IProcedure = 281;
                /// <summary>
                /// ������� �������������
                /// </summary>
                public const int ICondition = 282;
                /// <summary>
                /// ������ �� ������ ��� �������� ��������
                /// </summary>
                public const int ObjectReference = 283;
                /// <summary>
                /// ������
                /// </summary>
                public const int Design = 284;
                /// <summary>
                /// ����������� ���������
                /// </summary>
                public const int ConstantSet = 285;

                }
            }

        public struct Generator
            {
            /// <summary>
            /// �������� ��� �����
            /// </summary>
            public const int InvalidFileName = 350;
            /// <summary>
            /// ������ ����������
            /// </summary>
            public const int Compilation = 351;
            }
        };

    }
