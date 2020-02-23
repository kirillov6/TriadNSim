using System;
using System.Collections.Generic;
using System.Text;

namespace TriadCore
    {
    /// <summary>
    /// ���������� ��� �������� �� ����������
    /// </summary>
    public class SpyVar : SpyObject
        {
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="realName">��� ���������� � �������</param>
        /// <param name="varContainer">������, ���������� ����������</param>
        public SpyVar( CoreName realName, ReflectionObject varContainer )
            : base( realName, varContainer )
            {
            this.varContainer = varContainer;
            }


        /// <summary>
        /// �������� ���������
        /// </summary>
        /// <param name="other"></param>
        /// <returns>True, ���� ������� ���������</returns>
        public override bool Equals( SpyObject other )
            {
            SpyVar spyVarInfo = other as SpyVar;

            //���� ������ ����� ������������ ���
            if ( spyVarInfo == null )
                return false;

            return base.Equals( other );
            }



        /// <summary>
        /// �������� ����������
        /// </summary>
        public object Value
            {
            set
                {
                if ( this.varContainer != null )
                    {
                    this.varContainer.SetValueForVar( this.RealName, value );
                    }
                }
            get
                {
                if ( this.varContainer != null )
                    {
                    return this.varContainer.GetValueForVar( this.RealName );
                    }
                return null;
                }
            }


        /// <summary>
        /// ������� �����
        /// </summary>
        /// <returns></returns>
        public override SpyObject Clone()
            {
            return new SpyVar( this.RealName, this.Container );
            }


        /// <summary>
        /// ������, ���������� ����������
        /// </summary>
        private ReflectionObject varContainer;
        }
    }
