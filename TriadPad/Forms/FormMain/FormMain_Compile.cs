using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

using TriadCompiler;

/// <summary>
/// Режимы компиляции
/// </summary>
public enum CompilationMode
    {
    /// <summary>
    /// Компиляция моделей
    /// </summary>
    Model = 0,
    /// <summary>
    /// Компиляция структур
    /// </summary>
    Structure = 1,
    /// <summary>
    /// Компиляция рутин
    /// </summary>
    Routine = 2,
    /// <summary>
    /// Компиляция дизайна
    /// </summary>
    Design = 3,
    /// <summary>
    /// Компиляция информационных процедур
    /// </summary>
    IProcedure = 4,
    /// <summary>
    /// Компиляция условий моделирования
    /// </summary>
    ICondition = 5
    }


namespace TriadPad.Forms
    {
    /// <summary>
    /// Все, что относится к компиляции в главной форме
    /// </summary>
    partial class FormMain
        {
        /// <summary>
        /// Имя редактируемого файла
        /// </summary>
        private string currFileName = NewFileName;

       
        /// <summary>
        /// Имя результирующего файла
        /// </summary>
        private string compiledFileFullName
            {
            get
                {
                return Options.Instance.CompiledDllPath + "\\" + 
                    Path.GetFileNameWithoutExtension( currFileName ) + ".dll"; 
                }
            }


        /// <summary>
        /// Папка с файлом ядра
        /// </summary>
        private string CoreFilePath
            {
            get
                {
                return Path.GetDirectoryName( Application.ExecutablePath ); 
                }
            }
        

        /// <summary>
        /// Транслировать
        /// </summary>
        private void Translate()
            {
            //Убираем подчеркивание ошибок
            this.rtbText.SetCharBackColor( CharFormatArea.AllText, this.rtbText.BackColor );
            
            InputRichEdit input = new InputRichEdit( this.rtbText );
            OutputRichEdit output = new OutputRichEdit( this.rtbListing );
            IOErrorListener io = new IOErrorListener( input, output );

            string tempFileName = Path.GetTempFileName();
            //Выбор режима компиляции
            switch ( Options.Instance.CompilationMode )
                {
                case CompilationMode.Model:
                    CompilerFacade.CompileModelToTxt( io, tempFileName );
                    break;
                case CompilationMode.Routine:
                    CompilerFacade.CompileRoutineToTxt( io, tempFileName );
                    break;
                case CompilationMode.Structure:
                    CompilerFacade.CompileStructureToTxt( io, tempFileName );
                    break;
                case CompilationMode.Design:
                    CompilerFacade.CompileDesignToTxt( io, tempFileName );
                    break;
                case CompilationMode.IProcedure:
                    CompilerFacade.CompileIProcedureToTxt( io, tempFileName );
                    break;
                case CompilationMode.ICondition:
                    CompilerFacade.CompileIConditionToTxt( io, tempFileName );
                    break;
                }            

            this.rtbCode.LoadFile( tempFileName, RichTextBoxStreamType.UnicodePlainText );
            File.Delete( tempFileName );

            //Выдаем список ошибок
            this.lvErrors.Items.Clear();
            foreach ( ErrorDescription errorFound in io.getRegisteredErrors() )
                {
                ListViewItem newItem = new ListViewItem( ( this.lvErrors.Items.Count + 1 ).ToString() );
                newItem.Tag = errorFound;
                newItem.SubItems.Add( errorFound.ToString() );
                newItem.SubItems.Add( errorFound.lineNumber.ToString() );
                newItem.SubItems.Add( errorFound.chNumber.ToString() );
                this.lvErrors.Items.Add( newItem );
                }
            }


        /// <summary>
        /// Компилировать
        /// </summary>
        /// <returns>Успех компиляции</returns>
        private bool Compile()
            {
            //Убираем подчеркивание ошибок
            this.rtbText.SetCharBackColor( CharFormatArea.AllText, this.rtbText.BackColor );

            bool success = true;

            InputRichEdit input = new InputRichEdit( this.rtbText );
            OutputRichEdit output = new OutputRichEdit( this.rtbListing );
            IOErrorListener io = new IOErrorListener( input, output );

            //Выбор режима компиляции
            switch ( Options.Instance.CompilationMode )
                {
                case CompilationMode.Model:
                    CompilerFacade.CompileModelToDll( io, this.compiledFileFullName );
                    break;
                case CompilationMode.Routine:
                    CompilerFacade.CompileRoutineToDll( io, this.compiledFileFullName );
                    break;
                case CompilationMode.Structure:
                    CompilerFacade.CompileStructureToDll( io, this.compiledFileFullName );
                    break;
                case CompilationMode.Design:
                    CompilerFacade.CompileDesignToDll( io, this.compiledFileFullName );
                    break;
                case CompilationMode.IProcedure:
                    CompilerFacade.CompileIProcedureToDll( io, this.compiledFileFullName );
                    break;
                case CompilationMode.ICondition:
                    CompilerFacade.CompileIConditionToDll( io, this.compiledFileFullName );
                    break;
                }

            if ( !File.Exists( Options.Instance.CompiledDllPath + "\\TriadCore.dll" ) )
                {
                File.Copy( this.CoreFilePath + "\\TriadCore.dll", Options.Instance.CompiledDllPath + "\\TriadCore.dll" );
                }

            //Выдаем список ошибок
            this.lvErrors.Items.Clear();
            ErrorDescription[] registeresErrors = io.getRegisteredErrors();
            foreach ( ErrorDescription errorFound in registeresErrors )
                {
                ListViewItem newItem = new ListViewItem( ( this.lvErrors.Items.Count + 1 ).ToString() );
                newItem.Tag = errorFound;
                newItem.SubItems.Add( errorFound.ToString() );
                newItem.SubItems.Add( errorFound.lineNumber.ToString() );
                newItem.SubItems.Add( errorFound.chNumber.ToString() );
                this.lvErrors.Items.Add( newItem );
                success = false;
                }

            return success;
            }


        /// <summary>
        /// Компилировать и выполнить
        /// </summary>
        private void CompileAndRun()
            {
            if ( Compile() ) 
                {
                RunProgram();
                }
            }


        /// <summary>
        /// Выполнить
        /// </summary>
        private void RunProgram()
            {
            //Если файл уже скомпилирован
            if ( File.Exists( compiledFileFullName ) && CompilerFacade.DesignTypeName != string.Empty )
                {
                //В режиме запуска моделей нужно указывать конечное время моделирования
                if ( Options.Instance.CompilationMode == CompilationMode.Model )
                    {
                    Process.Start( "TriadRunner.exe", " \"" + compiledFileFullName + "\" " +
                        CompilerFacade.DesignTypeName + " " + Int32.Parse( this.tstEndTime.Text ));
                    }
                //В режиме запуска дизайна указывать время не нужно (указываем -1)
                else if ( Options.Instance.CompilationMode == CompilationMode.Design )
                    {
                    Process.Start( "TriadRunner.exe", " \"" + compiledFileFullName + "\" " +
                        CompilerFacade.DesignTypeName + " -1" );
                    }
                }
            }
        }
    }
