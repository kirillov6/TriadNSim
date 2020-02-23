using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace TriadCore
    {
    /// <summary>
    /// ������� �����, �������������� ������ � ����������� ��������� ����� ������
    /// </summary>
    public class ReflectionObject
        {
        /// <summary>
        /// ����������� ��������� ��������
        /// </summary>
        protected SortedList<CoreName, List<SpyHandler>> spyHandlerList =
            new SortedList<CoreName, List<SpyHandler>>();


        /// <summary>
        /// �������� �����
        /// </summary>
        /// <returns></returns>
        public ReflectionObject Clone()
            {
            ReflectionObject newObject = this.MemberwiseClone() as ReflectionObject;
            //�������� ������ ������������
            newObject.spyHandlerList = new SortedList<CoreName, List<SpyHandler>>();
            foreach ( KeyValuePair<CoreName, List<SpyHandler>> pair in this.spyHandlerList )
                newObject.spyHandlerList.Add( pair.Key, pair.Value );
            
            return newObject;
            }


        /// <summary>
        /// ���������������� ���������� ��������� �������
        /// </summary>
        /// <param name="objectInfo">������ ��������</param>
        /// <param name="handler">����������</param>
        public void RegisterSpyHandler( SpyObject objectInfo, SpyHandler handler )
            {
            ReflectionObject objectContainer = objectInfo.Container;
            if ( objectContainer.spyHandlerList.ContainsKey( objectInfo.RealName ) )
                objectContainer.spyHandlerList[ objectInfo.RealName ].Add( handler );
            else
                {
                List<SpyHandler> handlerList = new List<SpyHandler>();
                handlerList.Add( handler );
                objectContainer.spyHandlerList.Add( objectInfo.RealName, handlerList );
                }
            }


        /// <summary>
        /// ���������������� ���������� ��������� ��������� ��������
        /// </summary>
        /// <param name="objectInfoArray">��������</param>
        /// <param name="handler">����������</param>
        public void RegisterSpyHandler( SpyObject[] objectInfoArray, SpyHandler handler )
            {
            foreach ( SpyObject objectInfo in objectInfoArray )
                RegisterSpyHandler( objectInfo, handler );
            }


        /// <summary>
        /// ������� ��� ����������� ��������� ����������
        /// </summary>
        public void RemoveAllVarChangeHandlers()
            {
            this.spyHandlerList.Clear();
            }


        /// <summary>
        /// ��������� �����
        /// </summary>
        protected virtual double SystemTime
            {
            get
                {
                return 0;
                }
            } 


        /// <summary>
        /// �������� �������� ���������� ����������
        /// </summary>
        /// <param name="varName">��� ����������</param>
        /// <returns>�������� ����������</returns>
        public object GetValueForVar( CoreName varName )
            {
            Type selfType = this.GetType();
            FieldInfo fieldInfo = selfType.GetField( varName.BaseName, BindingFlags.NonPublic | 
                BindingFlags.Instance );
            
            //���� ��� ������� ����������
            if ( fieldInfo != null )
                {
                //���� ��� ����������������� ����������
                if ( !varName.IsIndexed )
                    {
                    return fieldInfo.GetValue( this );
                    }
                //���� ��� ������
                else
                    {
                    return ( (Array)fieldInfo.GetValue( this ) ).GetValue( varName.IndexArray );
                    }
                }
            //���� ��� ��������
            else
                {
                PropertyInfo propertyInfo = selfType.GetProperty( varName.BaseName, BindingFlags.NonPublic | 
                    BindingFlags.Instance );

                if ( propertyInfo != null )
                    {
                    //���� ��� ����������������� ����������
                    if ( !varName.IsIndexed )
                        {
                        return propertyInfo.GetValue( this, null );
                        }
                    //���� ��� ������
                    else
                        {
                        return ( (Array)propertyInfo.GetValue( this, null ) ).GetValue( varName.IndexArray );
                        }
                    }
                else
                    {
                    throw new ArgumentException( "���������� " + varName + " �� �������� ���������� ���������� " + this.ToString() );
                    }
                }
            }


        /// <summary>
        /// ���������� �������� ���������� ����������  
        /// </summary>
        /// <param name="varName">��� ����������</param>
        /// <param name="value">�������� ����������</param>
        public void SetValueForVar( CoreName varName, object value )
            {
            Type selfType = this.GetType();
            FieldInfo fieldInfo = selfType.GetField( varName.BaseName, BindingFlags.NonPublic | BindingFlags.Instance );
            if ( fieldInfo != null )
                {
                //���� ��� ����������������� ����������
                if ( !varName.IsIndexed )
                    {
                    fieldInfo.SetValue( this, value );
                    }
                //���� ��� ������
                else
                    {
                    ( (Array)fieldInfo.GetValue( this ) ).SetValue( value, varName.IndexArray );
                    }
                }
            //���� ��� ��������
            else
                {
                PropertyInfo propertyInfo = selfType.GetProperty( varName.BaseName, BindingFlags.NonPublic |
                    BindingFlags.Instance );

                if ( propertyInfo != null )
                    {
                    //���� ��� ����������������� ����������
                    if ( !varName.IsIndexed )
                        {
                        propertyInfo.SetValue( this, value, null );
                        object val = propertyInfo.GetValue( this, null );
                        }
                    //���� ��� ������
                    else
                        {
                        ( (Array)propertyInfo.GetValue( this, null ) ).SetValue( value, varName.IndexArray );
                        }
                    }
                else
                    {
                    throw new ArgumentException( "���������� " + varName + " �� �������� ���������� ���������� " + this.ToString() );
                    }                
                }
            }



        /// <summary>
        /// ����� ������������ ��������� ����������
        /// </summary>
        /// <param name="varName">��� ����������</param>
        protected void DoVarChanging( CoreName varName )
            {
            //����� ������������ ��������� ����������
            if ( this.spyHandlerList.ContainsKey( varName ) )
                foreach ( SpyHandler handler in this.spyHandlerList[ varName ] )
                    {
                    handler.Invoke( new SpyVar( varName, this ), this.SystemTime );
                    }
            }

        
        }
    }
