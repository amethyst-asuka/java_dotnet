Imports System.Collections.Generic

'
' * Copyright (c) 2005, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.tools


	''' <summary>
	''' Forwards calls to a given file manager.  Subclasses of this class
	''' might override some of these methods and might also provide
	''' additional fields and methods.
	''' </summary>
	''' @param <M> the kind of file manager forwarded to by this object
	''' @author Peter von der Ah&eacute;
	''' @since 1.6 </param>
	Public Class ForwardingJavaFileManager(Of M As JavaFileManager)
		Implements JavaFileManager

		''' <summary>
		''' The file manager which all methods are delegated to.
		''' </summary>
		Protected Friend ReadOnly fileManager As M

		''' <summary>
		''' Creates a new instance of ForwardingJavaFileManager. </summary>
		''' <param name="fileManager"> delegate to this file manager </param>
		Protected Friend Sub New(ByVal fileManager As M)
			fileManager.GetType() ' null check
			Me.fileManager = fileManager
		End Sub

		''' <exception cref="SecurityException"> {@inheritDoc} </exception>
		''' <exception cref="IllegalStateException"> {@inheritDoc} </exception>
		Public Overridable Function getClassLoader(ByVal location As Location) As ClassLoader Implements JavaFileManager.getClassLoader
			Return fileManager.getClassLoader(location)
		End Function

		''' <exception cref="IOException"> {@inheritDoc} </exception>
		''' <exception cref="IllegalStateException"> {@inheritDoc} </exception>
		Public Overridable Function list(ByVal location As Location, ByVal packageName As String, ByVal kinds As java.util.Set(Of javax.tools.JavaFileObject.Kind), ByVal recurse As Boolean) As IEnumerable(Of JavaFileObject) Implements JavaFileManager.list
			Return fileManager.list(location, packageName, kinds, recurse)
		End Function

		''' <exception cref="IllegalStateException"> {@inheritDoc} </exception>
		Public Overridable Function inferBinaryName(ByVal location As Location, ByVal file As JavaFileObject) As String Implements JavaFileManager.inferBinaryName
			Return fileManager.inferBinaryName(location, file)
		End Function

		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Public Overridable Function isSameFile(ByVal a As FileObject, ByVal b As FileObject) As Boolean Implements JavaFileManager.isSameFile
			Return fileManager.isSameFile(a, b)
		End Function

		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		''' <exception cref="IllegalStateException"> {@inheritDoc} </exception>
		Public Overridable Function handleOption(ByVal current As String, ByVal remaining As IEnumerator(Of String)) As Boolean Implements JavaFileManager.handleOption
			Return fileManager.handleOption(current, remaining)
		End Function

		Public Overridable Function hasLocation(ByVal location As Location) As Boolean Implements JavaFileManager.hasLocation
			Return fileManager.hasLocation(location)
		End Function

		Public Overridable Function isSupportedOption(ByVal [option] As String) As Integer Implements OptionChecker.isSupportedOption
			Return fileManager.isSupportedOption([option])
		End Function

		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		''' <exception cref="IllegalStateException"> {@inheritDoc} </exception>
		Public Overridable Function getJavaFileForInput(ByVal location As Location, ByVal className As String, ByVal kind As javax.tools.JavaFileObject.Kind) As JavaFileObject
			Return fileManager.getJavaFileForInput(location, className, kind)
		End Function

		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		''' <exception cref="IllegalStateException"> {@inheritDoc} </exception>
		Public Overridable Function getJavaFileForOutput(ByVal location As Location, ByVal className As String, ByVal kind As javax.tools.JavaFileObject.Kind, ByVal sibling As FileObject) As JavaFileObject
			Return fileManager.getJavaFileForOutput(location, className, kind, sibling)
		End Function

		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		''' <exception cref="IllegalStateException"> {@inheritDoc} </exception>
		Public Overridable Function getFileForInput(ByVal location As Location, ByVal packageName As String, ByVal relativeName As String) As FileObject Implements JavaFileManager.getFileForInput
			Return fileManager.getFileForInput(location, packageName, relativeName)
		End Function

		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		''' <exception cref="IllegalStateException"> {@inheritDoc} </exception>
		Public Overridable Function getFileForOutput(ByVal location As Location, ByVal packageName As String, ByVal relativeName As String, ByVal sibling As FileObject) As FileObject Implements JavaFileManager.getFileForOutput
			Return fileManager.getFileForOutput(location, packageName, relativeName, sibling)
		End Function

		Public Overridable Sub flush() Implements JavaFileManager.flush
			fileManager.flush()
		End Sub

		Public Overridable Sub close() Implements JavaFileManager.close
			fileManager.close()
		End Sub
	End Class

End Namespace