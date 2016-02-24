'
' * Copyright (c) 2012, Oracle and/or its affiliates. All rights reserved.
' * ORACLE PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' 
Namespace java.lang


	Friend Class ClassLoaderHelper

		Private Sub New()
		End Sub

		''' <summary>
		''' Returns an alternate path name for the given file
		''' such that if the original pathname did not exist, then the
		''' file may be located at the alternate location.
		''' For most platforms, this behavior is not supported and returns null.
		''' </summary>
		Friend Shared Function mapAlternativeName(ByVal [lib] As java.io.File) As java.io.File
			Return Nothing
		End Function
	End Class

End Namespace