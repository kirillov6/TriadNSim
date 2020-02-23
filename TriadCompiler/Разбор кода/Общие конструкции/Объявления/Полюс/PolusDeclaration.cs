using System;
using System.Collections.Generic;
using System.Text;

using TriadCompiler.Parser.Common.Declaration.Var;

namespace TriadCompiler.Parser.Common.Declaration.Polus
    {
    /// <summary>
    /// ������ ���������� �������
    /// </summary>
    internal class PolusDeclaration : CommonParser
        {
        /// <summary>
        /// ��������� ��������� �������� ���������� ������
        /// </summary>
        private static List<Key> startKeys = null;


        /// <summary>
        /// ��������� ������� ���������� ������
        /// </summary>
        public static List<Key> StartKeys
            {
            get
                {
                if ( startKeys == null )
                    {
                    Key[] keySet = { Key.Identificator, Key.InOut, Key.Input, Key.Output };

                    startKeys = new List<Key>( keySet );
                    }
                return startKeys;
                }
            }

        /// <summary>
        /// ���������� �������
        /// </summary>
        /// <syntax>Input | Output | InOut PolusNameInDeclaration {,PolusNameInDeclaration}</syntax>
        /// <param name="endKeys">��������� ���������� �������� ��������</param>
        /// <param name="isSpyObject">������� spy-�������</param>
        /// <returns>������ ����� ����������� �������</returns>
        public static List<IPolusType> Parse( EndKeyList endKeys, bool isSpyObject )
            {
            List<IPolusType> polusTypeList = new List<IPolusType>();

            if ( !StartKeys.Contains( currKey ) )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.WrongStartSymbol.PolusDeclaration, StartKeys );
                SkipTo( endKeys.UniteWith( StartKeys ) );
                }
            if ( StartKeys.Contains( currKey ) )
                {
                bool isInput = true;
                bool isOutput = true;

                //������� �����
                if ( currKey == Key.Input )
                    {
                    GetNextKey();

                    isInput = true;
                    isOutput = false;
                    }
                //�������� �����
                else if ( currKey == Key.Output )
                    {
                    GetNextKey();
                    
                    isInput = false;
                    isOutput = true;
                    }
                //������������� �����
                else if ( currKey == Key.InOut )
                    {
                    GetNextKey();

                    isInput = true;
                    isOutput = true;
                    }
                else
                    {
                    //���� ������ �� �������, �� ������������ ��� � Polus
                    }

                //��� ������
                IPolusType polusType = PolusName( endKeys.UniteWith( Key.Comma ), isInput, isOutput );
                polusType.IsSpyObject = isSpyObject;
                polusTypeList.Add( polusType );

                while ( currKey == Key.Comma )
                    {
                    GetNextKey();

                    //��� ������
                    polusType = PolusName( endKeys.UniteWith( Key.Comma ), isInput, isOutput );
                    polusType.IsSpyObject = isSpyObject;
                    polusTypeList.Add( polusType );
                    }
                }
            return polusTypeList;
            }


        /// <summary>
        /// ��� ������
        /// </summary>
        /// <syntax>Identificator # RangeDeclaration #</syntax>
        /// <param name="endKeys">��������� ���������� �������� ��������</param>
        /// <param name="isInput">��� ������� �����</param>
        /// <param name="isOutput">��� �������� �����</param>
        /// <returns>��� ������</returns>
        private static IPolusType PolusName( EndKeyList endKeys, bool isInput, bool isOutput )
            {
            IPolusType polusType = new PolusType( isInput, isOutput );

            if ( currKey != Key.Identificator )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.WrongStartSymbol.PolusDeclarationName, Key.Identificator );
                SkipTo( endKeys.UniteWith( Key.Identificator ) );
                }
            if ( currKey == Key.Identificator )
                {
                //��� ������
                string polusName = ( currSymbol as IdentSymbol ).Name;
                polusType.Name = polusName;

                Accept( Key.Identificator );

                //���� ��� ��������������� �����
                if ( currKey == Key.LeftBracket )
                    {
                    PolusArrayType polusArrayType = new PolusArrayType( polusType.IsInput, polusType.IsOutput );
                    polusArrayType.Name = polusName;

                    TypeDeclaration.RangeDeclaration( endKeys.UniteWith( Key.RightBracket ), polusArrayType );
                    polusType = polusArrayType;
                    }

                if ( polusName != string.Empty )
                    {
                    CommonArea.Instance.Register( polusType );
                    }

                if ( !endKeys.Contains( currKey ) )
                    {
                    Fabric.Instance.ErrReg.Register( Err.Parser.WrongEndSymbol.PolusDeclarationName, endKeys.GetLastKeys() );
                    SkipTo( endKeys );
                    }
                }

            return polusType;
            }


        }
    }
