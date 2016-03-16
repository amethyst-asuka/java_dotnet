Module Program

    Public Function Main() As Integer
        Dim ss = FunctionParser.Replace("protected void normalizeWhitespace(XMLString value, int fromIndex)")


        Return GetType(CLI).RunCLI(App.CommandLine)
    End Function
End Module
