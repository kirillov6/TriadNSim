using System;
using System.Collections.Generic;
using System.Text;

using System.CodeDom;
using TriadCompiler.Parser.Common.Statement;
using TriadCompiler.Parser.Common.Expr;

namespace TriadCompiler.Parser.Routine.Statement
{
    /// <summary>
    /// Разбор оператора rule в рутине
    /// </summary>
    internal class Rule : CommonParser
    {
        /// <summary>
        /// Оператор описания правила
        /// </summary>
        /// <syntax>Rule Identificator If Expression Then StatementList EndIf {Reason Expression} EndRule</syntax>
        /// <param name="endKeys">Множество конечных символов</param>
        /// <param name="context">Текущий контекст</param>
        /// <returns>Представление для генерации кода</returns>
        public static CodeStatementCollection Parse(EndKeyList endKeys, StatementContext context)
        {
            Accept(Key.Rule);

            CodeStatementCollection statementList = new CodeStatementCollection();
            string ruleName;

            if (currKey != Key.Identificator)
            {
                Fabric.Instance.ErrReg.Register(Err.Parser.WrongStartSymbol.RuleDeclarationName, Key.Identificator);
                SkipTo(endKeys.UniteWith(Key.Identificator, Key.Semicolon));
            }
            else
            {
                ruleName = (currSymbol as IdentSymbol).Name;
                GetNextKey();
            }

            CodeStatement ifStatement = Condition.Parse(endKeys, context, true);
            Accept(Key.Semicolon);

            if (currKey == Key.Reason)
            {
                GetNextKey();

                ExprInfo exprInfo = Expression.Parse(endKeys.UniteWith(Key.Semicolon, Key.EndRule));
                exprInfo.IsString();

                CodeMethodInvokeExpression writeStat = new CodeMethodInvokeExpression();
                writeStat.Method = new CodeMethodReferenceExpression(null, Builder.Routine.Print);
                writeStat.Parameters.Add(exprInfo.Code);

                (ifStatement as CodeConditionStatement).TrueStatements.Add(writeStat);

                Accept(Key.Semicolon);
            }

            statementList.Add(ifStatement);

            Accept(Key.EndRule);

            return statementList;
        }
    }
}
