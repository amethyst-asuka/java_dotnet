Imports System
Imports System.Runtime.CompilerServices

'
' * Copyright (c) 1998, 2011, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text.html.parser



	''' <summary>
	''' Responsible for starting up a new DocumentParser
	''' each time its parse method is invoked. Stores a
	''' reference to the dtd.
	''' 
	''' @author  Sunita Mani
	''' </summary>

	<Serializable> _
	Public Class ParserDelegator
		Inherits javax.swing.text.html.HTMLEditorKit.Parser

		Private Shared ReadOnly DTD_KEY As New Object

		Protected Friend Shared Sub setDefaultDTD()
			defaultDTD
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Property Shared defaultDTD As DTD
			Get
				Dim appContext As sun.awt.AppContext = sun.awt.AppContext.appContext
    
				Dim dtd As DTD = CType(appContext.get(DTD_KEY), DTD)
    
				If dtd Is Nothing Then
					Dim _dtd As DTD = Nothing
					' (PENDING) Hate having to hard code!
					Dim nm As String = "html32"
					Try
						_dtd = DTD.getDTD(nm)
					Catch e As java.io.IOException
						' (PENDING) UGLY!
						Console.WriteLine("Throw an exception: could not get default dtd: " & nm)
					End Try
					dtd = createDTD(_dtd, nm)
    
					appContext.put(DTD_KEY, dtd)
				End If
    
				Return dtd
			End Get
		End Property

		Protected Friend Shared Function createDTD(ByVal dtd As DTD, ByVal name As String) As DTD

			Dim [in] As java.io.InputStream = Nothing
			Dim debug As Boolean = True
			Try
				Dim path As String = name & ".bdtd"
				[in] = getResourceAsStream(path)
				If [in] IsNot Nothing Then
					dtd.read(New java.io.DataInputStream(New java.io.BufferedInputStream([in])))
					dtd.putDTDHash(name, dtd)
				End If
			Catch e As Exception
				Console.WriteLine(e)
			End Try
			Return dtd
		End Function


		Public Sub New()
			defaultDTDDTD()
		End Sub

		Public Overridable Sub parse(ByVal r As java.io.Reader, ByVal cb As javax.swing.text.html.HTMLEditorKit.ParserCallback, ByVal ignoreCharSet As Boolean)
			CType(New DocumentParser(defaultDTD), DocumentParser).parse(r, cb, ignoreCharSet)
		End Sub

		''' <summary>
		''' Fetch a resource relative to the ParserDelegator classfile.
		''' If this is called on 1.2 the loading will occur under the
		''' protection of a doPrivileged call to allow the ParserDelegator
		''' to function when used in an applet.
		''' </summary>
		''' <param name="name"> the name of the resource, relative to the
		'''  ParserDelegator class.
		''' @returns a stream representing the resource </param>
		Friend Shared Function getResourceAsStream(ByVal name As String) As java.io.InputStream
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			Return java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<java.io.InputStream>()
	'		{
	'					public InputStream run()
	'					{
	'						Return ParserDelegator.class.getResourceAsStream(name);
	'					}
	'				});
		End Function

		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()
			defaultDTDDTD()
		End Sub
	End Class

End Namespace