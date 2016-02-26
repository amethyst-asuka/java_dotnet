'----------------------------------------------------------------------------------------
'	Copyright © 2007 - 2012 Tangible Software Solutions Inc.
'	This class can be used by anyone provided that the copyright notice remains intact.
'
'	This class is used to replace most calls to the Java String.split method.
'----------------------------------------------------------------------------------------
Friend Class StringHelperClass
	Private Sub New()
		'to prevent instantiation
	End Sub
	'------------------------------------------------------------------------------------
	'	This method is used to replace most calls to the Java String.split method.
	'------------------------------------------------------------------------------------
	Friend Shared Function StringSplit(ByVal source As String, ByVal regexDelimiter As String, ByVal trimTrailingEmptyStrings As Boolean) As String()
		Dim splitArray() As String = System.Text.RegularExpressions.Regex.Split(source, regexDelimiter)

		If trimTrailingEmptyStrings Then
			If splitArray.Length > 1 Then
				For i As Integer = splitArray.Length To 1 Step -1
					If splitArray(i - 1).Length > 0 Then
						If i < splitArray.Length Then
							System.Array.Resize(splitArray, i)
						End If

						Exit For
					End If
				Next i
			End If
		End If

		Return splitArray
	End Function
End Class