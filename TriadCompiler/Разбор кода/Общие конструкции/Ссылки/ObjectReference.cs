using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

using TriadCompiler.Parser.Common.Expr;

namespace TriadCompiler.Parser.Common.ObjectRef
    {
    /// <summary>
    /// ������ �� ����� ������ ��� �� �������� ��������
    /// </summary>
    /// <remarks>� ���� ������ ������ ��������� static ����������,
    /// �.�. ������ ������� ������� ����� ���� ����������!!!</remarks>
    class ObjectReference : CommonParser
        {
        /// <summary>
        /// �������������� �������� �������
        /// </summary>
        /// <param name="objectName">��� �������</param>
        /// <param name="exprInfo">�������� �������</param>
        /// <param name="indexNumber">����� �������</param>
        public delegate void AdditionalIndexCheck( string objectName, ExprInfo exprInfo, int indexNumber );


        /// <summary>
        /// ������ ������
        /// </summary>
        /// <param name="endKeys">���������� �������� �������</param>
        /// <param name="allowRange">��������� �� ���������</param>
        /// <returns>�������� ������</returns>
        /// <remarks>� ����� ���������� ������������ ObjectRefInfo ����
        /// ��������� ������� ��������� ��� ��������� �� ������ �������, ����
        /// ��� ������� ���������</remarks>
        /// <syntax>Identificator # [ IndexBounds {,IndexBounds} ] #</syntax>
        public static ObjectRefInfo Parse( EndKeyList endKeys, bool allowRange )
            {
            return Parse( endKeys, allowRange, delegate( string objectName, ExprInfo exprInfo, int indexNumber ) { } );
            }


        /// <summary>
        /// ������ ������
        /// </summary>
        /// <param name="endKeys">���������� �������� �������</param>
        /// <param name="allowRange">��������� �� ���������</param>
        /// <param name="additionalIndexCheck">�������������� �������� �������</param>
        /// <returns>�������� ������</returns>
        /// <remarks>� ����� ���������� ������������ ObjectRefInfo ����
        /// ��������� ������� ��������� ��� ��������� �� ������ �������, ����
        /// ��� ������� ���������</remarks>
        /// <syntax>Ident # [ IndexBounds {,IndexBounds} ] #</syntax>
        public static ObjectRefInfo Parse( EndKeyList endKeys, bool allowRange, AdditionalIndexCheck additionalIndexCheck )
            {
            ObjectRefInfo refInfo = new ObjectRefInfo();

            if ( currKey != Key.Identificator )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.WrongStartSymbol.ObjectReference, Key.Identificator );
                SkipTo( endKeys.UniteWith( Key.Identificator ) );
                }
            if ( currKey == Key.Identificator )
                {
                refInfo.Name = ( currSymbol as IdentSymbol ).Name;
                refInfo.AppendStrCode( refInfo.Name );

                Accept( Key.Identificator );

                //���� ��� ��������������� ������
                if ( currKey == Key.LeftBracket )
                    {
                    refInfo.AppendStrCode( "[" );

                    //��� �� ������ �������� ��������
                    bool indexRangeFound = false;

                    // ����� �������� ������������ �������
                    int currIndexNumber = 0;
                    do
                        {
                        GetNextKey();
                        refInfo.AddIndexBounds( IndexBounds( endKeys.UniteWith( Key.Comma, Key.RightBracket ), allowRange,
                            ref indexRangeFound, refInfo, currIndexNumber, additionalIndexCheck ) );
                        currIndexNumber++;

                        if ( currKey == Key.Comma )
                            refInfo.AppendStrCode( "," );
                        }
                    while ( currKey == Key.Comma );

                    Accept( Key.RightBracket );
                    refInfo.AppendStrCode( "]" );

                    refInfo.IsRange = indexRangeFound;

                    //��������� ����
                    CodeObjectCreateExpression createStat = new CodeObjectCreateExpression();
                    createStat.Parameters.Add( new CodePrimitiveExpression( refInfo.Name ) );

                    //���� ��� ������ �� ��������� ��������������� ������
                    if ( !indexRangeFound )
                        {
                        createStat.CreateType = new CodeTypeReference( Builder.CoreName.Name );
                        
                        foreach ( ObjectRefInfo.IndexBounds indexBounds in refInfo.IndexBoundArray )
                            {
                            //���������� ������ ������ ������� (������ �������)
                            createStat.Parameters.Add( indexBounds.lowIndexExpr.Code );
                            }
                        }
                    //���� ������ �������� ��������
                    else
                        {
                        createStat.CreateType = new CodeTypeReference( Builder.CoreName.Range );

                        foreach ( ObjectRefInfo.IndexBounds indexBounds in refInfo.IndexBoundArray )
                            {
                            createStat.Parameters.Add( indexBounds.lowIndexExpr.Code );
                            createStat.Parameters.Add( indexBounds.topIndexExpr.Code );
                            }
                        }
                    refInfo.CoreNameCode = createStat;
                    }
                //���� ��� ����������������� ������
                else
                    {
                    //��������� ����
                    CodeObjectCreateExpression createStat = new CodeObjectCreateExpression();
                    createStat.CreateType = new CodeTypeReference( Builder.CoreName.Name );
                    createStat.Parameters.Add( new CodePrimitiveExpression( refInfo.Name ) );

                    refInfo.CoreNameCode = createStat;
                    }

                if ( !endKeys.Contains( currKey ) )
                    {
                    Fabric.Instance.ErrReg.Register( Err.Parser.WrongEndSymbol.ObjectReference, endKeys.GetLastKeys() );
                    SkipTo( endKeys );
                    }
                }

            return refInfo;
            }



        /// <summary>
        /// ������ ������� ��� ��������� ��������
        /// </summary>
        /// <param name="endKeys">���������� �������� �������</param>
        /// <param name="allowRange">��������� �� ���������</param>
        /// <param name="rangeFound">True, ���� ������ ��������</param>
        /// <param name="refInfo">���������� � ����������� �������</param>
        /// <param name="currIndexNumber">����� �������� �������</param>
        /// <param name="additionalIndexCheck">�������������� �������� �������</param>
        /// <returns>�������� ���������. ���� ��� ��������� ������,
        /// �� ������� ������� ��������� ��������� � ������</returns>
        /// <syntax>Expression # : Expression #</syntax>
        public static ObjectRefInfo.IndexBounds IndexBounds( EndKeyList endKeys, bool allowRange, ref bool rangeFound,
            ObjectRefInfo refInfo, int currIndexNumber, AdditionalIndexCheck additionalIndexCheck )
            {
            ExprInfo lowExpr;
            if ( allowRange )
                lowExpr = Expression.Parse( endKeys.UniteWith( Key.Colon ) );
            else
                lowExpr = Expression.Parse( endKeys );
            refInfo.AppendStrCode( lowExpr.StrCode );

            ExprInfo topExpr = lowExpr;

            //�������� �������
            bool errorFound = !lowExpr.IsNotIndexed();
            errorFound |= !lowExpr.IsNotSet();
            errorFound |= !lowExpr.IsInteger();
            errorFound |= !lowExpr.NotNegativeIntegerOrReal();

            //�������������� ��������
            if ( !errorFound )
                additionalIndexCheck( refInfo.Name, lowExpr, currIndexNumber );

            //���� ������ �������� �������
            if ( currKey == Key.Colon && allowRange )
                {
                Accept( Key.Colon );

                rangeFound = true;

                topExpr = Expression.Parse( endKeys );

                //�������� �������
                errorFound |= !topExpr.IsNotIndexed();
                errorFound |= !topExpr.IsNotSet();
                errorFound |= !topExpr.IsInteger();
                errorFound |= !topExpr.NotNegativeIntegerOrReal();

                //�������������� ��������
                if ( !errorFound )
                    additionalIndexCheck( refInfo.Name, topExpr, currIndexNumber );

                if ( !errorFound && topExpr.HasNoError && lowExpr.HasNoError )
                    //���� ������� ������ �����������
                    if ( topExpr.Value.IsConstant && lowExpr.Value.IsConstant )
                        //�������� ���������� �������� ��������
                        if ( ( lowExpr.Value as IntegerValue ).Value > ( topExpr.Value as IntegerValue ).Value )
                            {
                            Fabric.Instance.ErrReg.Register( Err.Parser.Usage.Polus.LowIndexIsGreaterThanTopInRange );
                            errorFound = true;
                            }

                }
 
            return new ObjectRefInfo.IndexBounds( lowExpr, topExpr );
            }


        /// <summary>
        /// ��������� ����� �������� � �������
        /// </summary>
        /// <param name="varType">���������� ��� �������</param>
        /// <param name="objRef">�������� ������������ ������</param>
        /// <param name="arrayAllowed">��������� �� ������� ��� ��������</param>
        public static void CheckIndexCount( ICommonType varType, ObjectRefInfo objRef, bool arrayAllowed )
            {
            //���� ���������� ����� ��������������� ���
            if ( varType is IndexedType )
                {
                IndexedType indexedType = varType as IndexedType;
                //���� ��������������� ���������� ������������ ��� ��������
                if ( !objRef.HasIndexes && !arrayAllowed )
                    {
                    Fabric.Instance.ErrReg.Register( Err.Parser.Usage.Array.ArrayIsNotVar );
                    }
                //���� ������� �������
                else if ( objRef.HasIndexes )
                    {
                    //�������� ����� ��������
                    if ( indexedType.IndexCount > objRef.IndexCount )
                        {
                        Fabric.Instance.ErrReg.Register( Err.Parser.Usage.Array.LostIndex );
                        }
                    else if ( indexedType.IndexCount < objRef.IndexCount )
                        {
                        Fabric.Instance.ErrReg.Register( Err.Parser.Usage.Array.TooManyIndexes );
                        }
                    }
                }
            //���� ������ ������������ � ����������������� ����������            
            else if ( !( varType is IndexedType ) && objRef.HasIndexes )
                {
                Fabric.Instance.ErrReg.Register( Err.Parser.Usage.Array.VarIsNotArray );
                }
            }
        }
    }
