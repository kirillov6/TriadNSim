using System;
using System.Collections.Generic;
using System.Text;

namespace TriadCompiler.Parser.Common.Declaration.Event
    {
    internal class EventDeclaration : CommonParser
        {
        /// <summary>
        /// ���������� �������
        /// </summary>
        /// <syntax>Identificator {,Identificator}</syntax>
        /// <param name="endKeys">��������� ���������� �������� ��������</param>
        /// <param name="isSpyObject">������� spy-��������</param>
        /// <returns>������ ����� ����������� �������</returns>
        public static List<EventType> Parse( EndKeyList endKeys, bool isSpyObject )
            {
            List<EventType> eventList = new List<EventType>();

            //��� �������
            eventList.Add( EventName( endKeys.UniteWith( Key.Comma ), isSpyObject ) );

            while ( currKey == Key.Comma )
                {
                GetNextKey();

                //��� �������
                eventList.Add( EventName( endKeys.UniteWith( Key.Comma ), isSpyObject ) );
                }

            return eventList;
            }


        /// <summary>
        /// ������ ����� ������� � ����������
        /// </summary>
        /// <param name="endKeys">���������� �������� �������</param>
        /// <param name="isSpyObject">������� spy-�������</param>
        /// <returns>��� �������</returns>
        private static EventType EventName( EndKeyList endKeys, bool isSpyObject )
            {
            //��� �������
            EventType resultType = null;

            if ( currKey != Key.Identificator )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.WrongStartSymbol.EventDeclarationName, Key.Identificator );
                SkipTo( endKeys.UniteWith( Key.Identificator ) );
                }
            if ( currKey == Key.Identificator )
                {
                string eventName = ( currSymbol as IdentSymbol ).Name;

                //��� �������
                resultType = new EventType( eventName );
                resultType.IsSpyObject = isSpyObject;

                //������������ �������
                CommonArea.Instance.Register( resultType );
                
                GetNextKey();

                if ( !endKeys.Contains( currKey ) )
                    {
                    Fabric.Instance.ErrReg.Register( Err.Parser.WrongEndSymbol.EventDeclarationName, endKeys.GetLastKeys() );
                    SkipTo( endKeys );
                    }
                }
            return resultType;
            }

        }
    }
