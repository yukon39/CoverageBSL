using OneScript.Language.Extensions;
using OneScript.Language.SyntaxAnalysis;
using OneScript.Language.SyntaxAnalysis.AstNodes;
using System;
using System.Collections.Generic;

namespace com.github.yukon39.CoverageBSL.AddIn.Internal.Ast
{
    internal static class GlobalCallsFilter
    {
        private static readonly HashSet<string> FilteredMethodNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "eval", "вычислить",
            "boolean", "булево",
            "number", "число",
            "string", "строка",
            "date", "дата",
            "type", "тип",
            "typeof", "типзнч",
            "strlen", "стрдлина",
            "triml", "сокрл",
            "trimr", "сокрп",
            "trimall", "сокрлп",
            "left", "лев",
            "right", "прав",
            "mid", "сред",
            "strpos",
            "upper", "врег",
            "lower", "нрег",
            "title", "трег",
            "char", "символ",
            "charcode", "кодсимвола",
            "isblankstring", "пустаястрока",
            "strreplace", "стрзаменить",
            "strgetline", "стрполучитьстроку",
            "strlinecount", "стрчислострок",
            "stroccurrencecount", "стрчисловхождений",
            "year", "год",
            "month", "месяц",
            "day", "день",
            "hour", "час",
            "minute", "минута",
            "second", "секунда",
            "begofyear", "началогода",
            "begofmonth", "началомесяца",
            "begofweek", "началонедели",
            "begofday", "началодня",
            "begofhour", "началочаса",
            "begofminute", "началоминуты",
            "begofquarter", "началоквартала",
            "endofyear", "конецгода",
            "endofmonth", "конецмесяца",
            "endofweek", "конецнедели",
            "endofday", "конецдня",
            "endofhour", "конецчаса",
            "endofminute", "конецминуты",
            "endofquarter", "конецквартала",
            "weekofyear", "неделягода",
            "dayofyear", "деньгода",
            "weekday", "деньнедели",
            "addmonth", "добавитьмесяц",
            "currentdate", "текущаядата",
            "int", "цел",
            "round", "окр",
            "log", "log10", "sin", "cos", "tan", "asin", "acos", "atan", "exp", "pow", "sqrt",
            "min", "мин", "max", "макс",
            "format", "формат",
            "errorinfo", "информацияобошибке",
            "errordescription", "описаниеошибки",
            "найти", "find"
        };

        public static bool ShouldInclude(string methodName) =>
            !FilteredMethodNames.Contains(methodName);

        /// <summary>
        /// Аналог <c>GlobalCallsFilter.filterByName</c> в Coverage41C (этап после <c>mustCovered</c>).
        /// </summary>
        public static bool ShouldIncludeNode(BslSyntaxNode node)
        {
            if (node.Kind != NodeKind.GlobalCall)
            {
                return true;
            }

            if (node is CallNode call && call.Identifier != null)
            {
                return ShouldInclude(call.Identifier.GetIdentifier());
            }

            return true;
        }
    }
}
