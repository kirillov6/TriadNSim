using System;
using System.Collections.Generic;
using System.Text;

namespace TriadCompiler
    {
    /// <summary>
    /// ���������� ��������� �����������
    /// </summary>
    public class CommonArea
        {
        /// <summary>
        /// �����������
        /// </summary>
        private CommonArea()
            {
            AddArea();
            }


        /// <summary>
        /// �������� ��������� ������
        /// </summary>
        public static CommonArea Instance
            {
            get
                {
                if ( instance == null )
                    instance = new CommonArea();
                return instance;
                }
            }


        /// <summary>
        /// ������� ����� �������
        /// </summary>
        public static void CreateNewArea()
            {
            if ( instance != null )
                {
                instance.RemoveAllAreas();

                instance = null;
                }
            }


        /// <summary>
        /// �������� ���������� ���������� ���� � ������� ������� ���������
        /// </summary>
        /// <param name="varType">��� ����������</param>
        public void Register( ICommonType varType )
            {
            //���� ��� �� ����� ������� ���������
            if ( areaList.Count == 0 )
                return;
            Area area = areaList[ areaList.Count - 1 ];
            //���� ������ ���������� �����
            if ( area == null )
                return;

            //���� ������ � ����� ������ ��� ���� � ���� ������� ���������
            if ( area.typeList.ContainsKey( varType.Name ) )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.Usage.DeclaredAgain );
                area.typeList[ varType.Name ].Add( varType );
                }
            else
                {
                //����� ������ �����
                List<ICommonType> typeList = new List<ICommonType>();
                typeList.Add( varType );
                //��������� ���������� � ������
                area.typeList.Add( varType.Name, typeList );
                }
            }



        /// <summary>
        /// ���������, ��������������� �� ��������� ��� � ��������� ������
        /// </summary>
        /// <param name="name">���</param>
        /// <param name="isRequiredType">���</param>
        /// <returns>True, ���� ���������������</returns>
        private bool IsSomeTypeRegistered( string name, IsRequiredType isRequiredType )
            {
            for ( int areaIndex = areaList.Count - 1 ; areaIndex >= 0 ; areaIndex-- )
                {
                Area area = areaList[ areaIndex ];
                if ( area == null )
                    return false;

                //���� ����� ��� ������������
                if ( area.typeList.ContainsKey( name ) )
                    foreach ( ICommonType type in area.typeList[ name ] )
                        if ( isRequiredType( type ) )
                            return true;
                }
            return false;
            }



        /// <summary>
        /// ���������, ��������������� �� ���� � ����� ������
        /// </summary>
        /// <param name="varName">��� ����������</param>
        /// <returns>True, ���� ���������������</returns>
        public bool IsGraphRegistered( string varName )
            {
            return IsSomeTypeRegistered( varName, delegate( ICommonType type )
                {
                    if ( type is IDesignVarType )
                        if ((type as IDesignVarType).TypeCode == DesignTypeCode.Structure)
                            return true;
                    return false;
                });
            }


        //======by jum======
        /// <summary>
        /// ���������, ���������������� �� ������� � ����� ������
        /// </summary>
        /// <param name="varName">��� ����������</param>
        /// <returns>True, ���� ���������������</returns>
        public bool IsNodeRegistered( string varName )
        {
            return IsSomeTypeRegistered(varName, delegate(ICommonType type)
            {
                if (type is VarType)
                    if ((type as VarType).Code == TypeCode.Node)
                        return true;
                return false;
            });
        }


        /// <summary>
        /// ���������, ���������������� �� ������� � ����� ������
        /// </summary>
        /// <param name="functionName">��� �������</param>
        /// <returns>True, ���� ����������������</returns>
        public bool IsFunctionRegistered( string functionName )
            {
            return IsSomeTypeRegistered( functionName, delegate( ICommonType type )
                {
                    return type is FunctionType;
                } );
            }


        /// <summary>
        /// ���������, ���������������� �� �� � ����� ������
        /// </summary>
        /// <param name="functionName">��� ��</param>
        /// <returns>True, ���� ����������������</returns>
        public bool IsIProcedureRegistered( string functionName )
            {
            return IsSomeTypeRegistered( functionName, delegate( ICommonType type )
                {
                    return type is IProcedureType;
                } );
            }


        /// <summary>
        /// ���������, ���������������� �� ������� � ����� ������
        /// </summary>
        /// <param name="eventName">��� �������</param>
        /// <returns>True, ���� ����������������</returns>
        public bool IsEventRegistered( string eventName )
            {
            return IsSomeTypeRegistered( eventName, delegate( ICommonType type )
                {
                    return type is EventType;
                } );
            }


        /// <summary>
        /// ������� �������� ������������ ����
        /// </summary>
        /// <param name="type">����������� ���</param>
        /// <returns>True, ���� ��� ��������</returns>
        private delegate bool IsRequiredType( ICommonType type );


        /// <summary>
        /// �������� ��������� ���
        /// </summary>
        /// <param name="usedName">�������������� ���</param>
        /// <returns>��������� ���</returns>
        public T GetType<T>( string usedName )
            where T : class
            {
            Area area;

            //������� ����, ��� �� ��������� ��� ��������������� ������ ���
            bool emptyTypeExists = false;

            for ( int areaIndex = areaList.Count - 1 ; areaIndex >= 0 ; areaIndex-- )
                {
                area = areaList[ areaIndex ];

                if ( area == null )
                    return null;

                //���� ����� ��� ����������������
                if ( area.typeList.ContainsKey( usedName ) )
                    foreach ( ICommonType type in area.typeList[ usedName ] )
                        //���� ��� ������ ���
                        if ( type == null )
                            {
                            emptyTypeExists = true;
                            continue;
                            }
                        //���� ���� ��� ��������
                        else if ( type is T )
                            {
                            return (T)type;
                            }
                }

            if ( emptyTypeExists )
                return null;

            //���������� ��������� ������� ���������
            area = areaList[ areaList.Count - 1 ];

            //���� ��� ��� ����������� � ������ ���
            if ( !area.typeList.ContainsKey( usedName ) )
                {
                area.typeList.Add( usedName, new List<ICommonType>() );
                
                //������������ ������ ���
                area.typeList[ usedName ].Add( null );
                }

            Fabric.Instance.ErrReg.Register( Err.Parser.Usage.NotDeclared );

            return null;
            }


        /// <summary>
        /// �������� ����� ������� ���������
        /// </summary>
        public void AddArea()
            {
            areaList.Add( new Area() );
            }


        /// <summary>
        /// ������� ������� ������� ���������
        /// </summary>
        public void RemoveArea()
            {
            if ( areaList.Count == 0 )
                return;

            areaList.RemoveAt( areaList.Count - 1 );
            }


        /// <summary>
        /// �������� ��� ������� ��������� ����������
        /// </summary>
        public void RemoveAllAreas()
            {
            while ( this.areaList.Count != 0 )
                RemoveArea();
            }



        /// <summary>
        /// ��������� ����� ������
        /// </summary>
        private static CommonArea instance = null;

        /// <summary>
        /// ������� ���������
        /// </summary>
        public class Area
            {
            /// <summary>
            /// ������ ������������������ �����
            /// ������� ����� �������������� ������ �����, ������� ��� ����������
            /// </summary>
            public SortedList<string, List<ICommonType>> typeList = new SortedList<string, List<ICommonType>>();
            }

        /// <summary>
        /// ������ �������� ���������
        /// </summary>
        private List<Area> areaList = new List<Area>();
        }
    }
