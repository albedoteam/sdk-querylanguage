namespace AlbedoTeam.Sdk.QueryLanguage.Core.Languages
{
    internal class AlbedoTQueryLanguage : BaseQueryLanguage
    {
        // private readonly BaseQueryLanguage _language;
        //
        // public AlbedoTQueryLanguage()
        // {
        //     _language = new BaseQueryLanguage(GetAllDefinitions().ToArray());
        // }
        //
        // protected virtual IEnumerable<GrammarDefinition> GetAllDefinitions()
        // {
        //     var functions = FunctionDefinitions().ToList();
        //
        //     var definitions = new List<GrammarDefinition>();
        //
        //     definitions.AddRange(TypeDefinitions());
        //     definitions.AddRange(functions);
        //     definitions.AddRange(BracketDefinitions(functions));
        //     // definitions.AddRange(LogicalOperatorDefinitions());
        //     // definitions.AddRange(ArithmeticOperatorDefinitions());
        //     // definitions.AddRange(PropertyDefinitions());
        //     // definitions.AddRange(WhitespaceDefinitions());
        //
        //     return definitions;
        // }
        //
        // protected virtual IEnumerable<GrammarDefinition> TypeDefinitions()
        // {
        //     return new List<GrammarDefinition>
        //     {
        //         new OperandDefinition(
        //             new Grammar("STRING", @"'(?:\\.|[^'])*'"),
        //             exp => Expression.Constant(exp.Trim('\'')
        //                 .Replace("\\'", "'")
        //                 .Replace("\\r", "\r")
        //                 .Replace("\\f", "\f")
        //                 .Replace("\\n", "\n")
        //                 .Replace("\\\\", "\\")
        //                 .Replace("\\b", "\b")
        //                 .Replace("\\t", "\t"))),
        //
        //         new OperandDefinition(
        //             new Grammar("FLOAT", @"\-?\d+?\.\d*f"),
        //             exp => Expression.Constant(float.Parse(exp.TrimEnd('f')))),
        //
        //         new OperandDefinition(
        //             new Grammar("DOUBLE", @"\-?\d+\.?\d*d"),
        //             exp => Expression.Constant(double.Parse(exp.TrimEnd('d')))),
        //
        //         new OperandDefinition(
        //             new Grammar("DECIMAL_EXPLICIT", @"\-?\d+\.?\d*[m|M]"),
        //             exp => Expression.Constant(decimal.Parse(exp.TrimEnd('m', 'M')))),
        //
        //         new OperandDefinition(
        //             new Grammar("DECIMAL", @"\-?\d+\.\d+"),
        //             exp => Expression.Constant(decimal.Parse(exp))),
        //
        //         new OperandDefinition(
        //             new Grammar("LONG", @"\-?\d+L"),
        //             exp => Expression.Constant(long.Parse(exp.TrimEnd('L')))),
        //
        //         new OperandDefinition(
        //             new Grammar("INTEGER", @"\-?\d+"),
        //             exp => Expression.Constant(int.Parse(exp)))
        //     };
        // }
        //
        // protected virtual IEnumerable<FunctionDefinition> FunctionDefinitions()
        // {
        //     return new List<FunctionDefinition>();
        // }
        //
        // protected virtual IEnumerable<GrammarDefinition> BracketDefinitions(
        //     IEnumerable<FunctionDefinition> functionCalls)
        // {
        //     // ParenthesisBracketOpenDefinition openBracket = null;
        //     // ListDelimiterDefinition delimeter;
        //     //
        //     // return new GrammarDefinition[]
        //     // {
        //     //     openBracket = new ParenthesisBracketOpenDefinition(new Grammar("OPEN_BRACKET", @"\(")),
        //     //
        //     //     delimeter = new ListDelimiterDefinition(new Grammar("COMMA", ",")),
        //     //
        //     //     new ParenthesisBracketCloseDefinition(new Grammar("CLOSE_BRACKET", @"\)"),
        //     //         new[] {openBracket}.Concat(functionCalls),
        //     //         delimeter)
        //     // };
        //
        //     return new List<GrammarDefinition>();
        // }
        //
        //
        // // ===
        //
        //
        // private Formula DoTheMagic(string text)
        // {
        //     var tokenStream = Tokenizer.Tokenize(text);
        //     var expression = Parser.Parse(tokenStream);
        //
        //     return new Formula("", "", "");
        // }
        //
        // // public Formula Parse(IEnumerable<Input> inputs, Func<ResolverDefinition, ResolverResult> resolverDefinition)
        // // {
        // //     var listDisplay = new List<string>();
        // //     var listRaw = new List<string>();
        // //     foreach (var input in inputs)
        // //     {
        // //         switch (input.Type)
        // //         {
        // //             case InputType.Function:
        // //                 listDisplay.Add(ApplyResolver(input, resolverDefinition));
        // //                 listRaw.Add($"<{input.Value}>");
        // //                 break;
        // //             case InputType.Resolver:
        // //                 listDisplay.Add(ApplyResolver(input, resolverDefinition));
        // //                 listRaw.Add($"[{input.Value}]");
        // //                 break;
        // //             case InputType.Operand:
        // //                 listDisplay.Add($"{input.Value}");
        // //                 listRaw.Add($"{input.Value}");
        // //                 break;
        // //             case InputType.Operator:
        // //                 listDisplay.Add($" {input.Value} ");
        // //                 listRaw.Add($" {input.Value} ");
        // //                 break;
        // //             default:
        // //                 listDisplay.Add(input.Value);
        // //                 listRaw.Add(input.Value);
        // //                 break;
        // //         }
        // //     }
        // //
        // //     var display = string.Join("", listDisplay);
        // //     var rawDisplay = string.Join("", listRaw);
        // //
        // //     var xpto = DoTheMagic(rawDisplay);
        // //
        // //     return new Formula(display, rawDisplay, "");
        // // }
        // //
        // // private string ApplyResolver(Input input, Func<ResolverDefinition, ResolverResult> resolverDefinition)
        // // {
        // //     var definition = new ResolverDefinition {InputType = input.Type, InputId = input.Value};
        // //
        // //     var result = resolverDefinition.Invoke(definition);
        // //
        // //     return result is null ? $"<{input.Value}>" : $"<{result.Nome}>";
        // // }
        //
        // public FormulaResult Execute(Formula formula)
        // {
        //     return new FormulaResult(formula, 0);
        // }
    }
}