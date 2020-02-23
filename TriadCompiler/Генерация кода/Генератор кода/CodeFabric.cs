using System;
using System.Collections.Generic;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.IO;

using TriadCompiler.Code.Generator;

namespace TriadCompiler.Code.Generator
    {
    /// <summary>
    /// ������ ��������������� ����
    /// </summary>
    internal enum CodeFormat
        {
        /// <summary>
        /// ��� ��������� ����
        /// </summary>
        None,
        /// <summary>
        /// ��������� Dll
        /// </summary>
        Dll,
        /// <summary>
        /// ��������� Txt
        /// </summary>
        Txt,
        /// <summary>
        /// ��������� � ������
        /// </summary>
        Memory
        }


    /// <summary>
    /// ������� ��������� ���� (��������)
    /// </summary>
    internal class CodeFabric
        {
        /// <summary>
        /// ���������� �����������
        /// </summary>
        protected CodeFabric()
            {
            }


        /// <summary>
        /// �������� ����� ������ ���������� ����
        /// </summary>
        /// <param name="codeFormat"></param>
        public static void ReloadFabric( CodeFormat codeFormat )
            {
            //���� ������� ��� �������
            if ( instance != null )
                {
                //���� ����� ������ �� ���������, �� ������������� ��� ������
                if ( generatingMode == codeFormat )
                    {
                    //������������� ��������� ����
                    Instance.Reload();
                    }
                //���� ����� ������ ���������, �� ������� ����� �������
                else
                    {
                    generatingMode = codeFormat;
                    instance = null;
                    }
                }
            //����� ������ ������� ����� �������
            else
                {
                generatingMode = codeFormat;
                }
            }


        /// <summary>
        /// ��������� ���� (��������)
        /// </summary>
        public static CommonGenerator Instance
            {
            get
                {
                if ( instance == null )
                    switch ( generatingMode )
                        {
                        case CodeFormat.None:
                            instance = new CommonGenerator();
                            break;
                        case CodeFormat.Dll:
                            instance = new DllGenerator();
                            break;
                        case CodeFormat.Txt:
                            instance = new TxtGenerator();
                            break;
                        case CodeFormat.Memory:
                            instance = new MemoryGenerator();
                            break;
                        default:
                            throw new InvalidOperationException( "������������ ����� ������" );
                        }
                return instance;
                }
            }


        /// <summary>
        /// ������� ����� ��������� ����� ������
        /// </summary>
        /// <param name="codeFormat">������ ��������������� ����</param>
        public static void CreateNewInstance( CodeFormat codeFormat )
            {
            generatingMode = codeFormat;
            instance = null;
            }


        /// <summary>
        /// ��������� ����� ������
        /// </summary>
        private static CommonGenerator instance = null;
        /// <summary>
        /// ������� �����
        /// </summary>
        private static CodeFormat generatingMode = CodeFormat.None;
        }   
    
    }





    
    