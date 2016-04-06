'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.io


	''' <summary>
	''' A filter for abstract pathnames.
	''' 
	''' <p> Instances of this interface may be passed to the <code>{@link
	''' File#listFiles(java.io.FileFilter) listFiles(FileFilter)}</code> method
	''' of the <code><seealso cref="java.io.File"/></code> class.
	''' 
	''' @since 1.2
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Interface FileFilter

		''' <summary>
		''' Tests whether or not the specified abstract pathname should be
		''' included in a pathname list.
		''' </summary>
		''' <param name="pathname">  The abstract pathname to be tested </param>
		''' <returns>  <code>true</code> if and only if <code>pathname</code>
		'''          should be included </returns>
		Function accept(  pathname As File) As Boolean
	End Interface

End Namespace