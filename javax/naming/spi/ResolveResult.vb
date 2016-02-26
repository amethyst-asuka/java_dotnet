Imports System

'
' * Copyright (c) 1999, 2004, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.naming.spi


	''' <summary>
	''' This class represents the result of resolution of a name.
	''' It contains the object to which name was resolved, and the portion
	''' of the name that has not been resolved.
	''' <p>
	''' A ResolveResult instance is not synchronized against concurrent
	''' multithreaded access. Multiple threads trying to access and modify
	''' a single ResolveResult instance should lock the object.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @since 1.3
	''' </summary>
	<Serializable> _
	Public Class ResolveResult
		''' <summary>
		''' Field containing the Object that was resolved to successfully.
		''' It can be null only when constructed using a subclass.
		''' Constructors should always initialize this.
		''' @serial
		''' </summary>
		Protected Friend resolvedObj As Object
		''' <summary>
		''' Field containing the remaining name yet to be resolved.
		''' It can be null only when constructed using a subclass.
		''' Constructors should always initialize this.
		''' @serial
		''' </summary>
		Protected Friend remainingName As javax.naming.Name

		''' <summary>
		''' Constructs an instance of ResolveResult with the
		''' resolved object and remaining name both initialized to null.
		''' </summary>
		Protected Friend Sub New()
			resolvedObj = Nothing
			remainingName = Nothing
		End Sub

		''' <summary>
		''' Constructs a new instance of ResolveResult consisting of
		''' the resolved object and the remaining unresolved component.
		''' </summary>
		''' <param name="robj"> The non-null object resolved to. </param>
		''' <param name="rcomp"> The single remaining name component that has yet to be
		'''                 resolved. Cannot be null (but can be empty). </param>
		Public Sub New(ByVal robj As Object, ByVal rcomp As String)
			resolvedObj = robj
			Try
			remainingName = New javax.naming.CompositeName(rcomp)
	'          remainingName.appendComponent(rcomp);
			Catch e As javax.naming.InvalidNameException
				' ignore; shouldn't happen
			End Try
		End Sub

		''' <summary>
		''' Constructs a new instance of ResolveResult consisting of
		''' the resolved Object and the remaining name.
		''' </summary>
		''' <param name="robj"> The non-null Object resolved to. </param>
		''' <param name="rname"> The non-null remaining name that has yet to be resolved. </param>
		Public Sub New(ByVal robj As Object, ByVal rname As javax.naming.Name)
			resolvedObj = robj
			remainingName = rname
		End Sub

		''' <summary>
		''' Retrieves the remaining unresolved portion of the name.
		''' </summary>
		''' <returns> The remaining unresolved portion of the name.
		'''          Cannot be null but empty OK. </returns>
		''' <seealso cref= #appendRemainingName </seealso>
		''' <seealso cref= #appendRemainingComponent </seealso>
		''' <seealso cref= #setRemainingName </seealso>
		Public Overridable Property remainingName As javax.naming.Name
			Get
				Return Me.remainingName
			End Get
			Set(ByVal name As javax.naming.Name)
				If name IsNot Nothing Then
					Me.remainingName = CType(name.clone(), javax.naming.Name)
				Else
					' ??? should throw illegal argument exception
					Me.remainingName = Nothing
				End If
			End Set
		End Property

		''' <summary>
		''' Retrieves the Object to which resolution was successful.
		''' </summary>
		''' <returns> The Object to which resolution was successful. Cannot be null. </returns>
		''' <seealso cref= #setResolvedObj </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Function getResolvedObj() As Object 'JavaToDotNetTempPropertyGetresolvedObj
		Public Overridable Property resolvedObj As Object
			Get
				Return Me.resolvedObj
			End Get
			Set(ByVal obj As Object)
		End Property


		''' <summary>
		''' Adds components to the end of remaining name.
		''' </summary>
		''' <param name="name"> The components to add. Can be null. </param>
		''' <seealso cref= #getRemainingName </seealso>
		''' <seealso cref= #setRemainingName </seealso>
		''' <seealso cref= #appendRemainingComponent </seealso>
		Public Overridable Sub appendRemainingName(ByVal name As javax.naming.Name)
	'      System.out.println("appendingRemainingName: " + name.toString());
	'      Exception e = new Exception();
	'      e.printStackTrace();
			If name IsNot Nothing Then
				If Me.remainingName IsNot Nothing Then
					Try
						Me.remainingName.addAll(name)
					Catch e As javax.naming.InvalidNameException
						' ignore; shouldn't happen for composite name
					End Try
				Else
					Me.remainingName = CType(name.clone(), javax.naming.Name)
				End If
			End If
		End Sub

		''' <summary>
		''' Adds a single component to the end of remaining name.
		''' </summary>
		''' <param name="name"> The component to add. Can be null. </param>
		''' <seealso cref= #getRemainingName </seealso>
		''' <seealso cref= #appendRemainingName </seealso>
		Public Overridable Sub appendRemainingComponent(ByVal name As String)
			If name IsNot Nothing Then
				Dim rname As New javax.naming.CompositeName
				Try
					rname.add(name)
				Catch e As javax.naming.InvalidNameException
					' ignore; shouldn't happen for empty composite name
				End Try
				appendRemainingName(rname)
			End If
		End Sub

			Me.resolvedObj = obj
			' ??? should check for null?
		End Sub

		Private Const serialVersionUID As Long = -4552108072002407559L
	End Class

End Namespace