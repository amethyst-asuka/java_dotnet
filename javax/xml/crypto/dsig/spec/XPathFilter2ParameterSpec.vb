Imports System.Collections
Imports System.Collections.Generic

'
' * Copyright (c) 2005, 2011, Oracle and/or its affiliates. All rights reserved.
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
'
' * $Id: XPathFilter2ParameterSpec.java,v 1.7 2005/05/13 18:45:42 mullan Exp $
' 
Namespace javax.xml.crypto.dsig.spec


	''' <summary>
	''' Parameters for the W3C Recommendation
	''' <a href="http://www.w3.org/TR/xmldsig-filter2/">
	''' XPath Filter 2.0 Transform Algorithm</a>.
	''' The parameters include a list of one or more <seealso cref="XPathType"/> objects.
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6 </summary>
	''' <seealso cref= Transform </seealso>
	''' <seealso cref= XPathFilterParameterSpec </seealso>
	Public NotInheritable Class XPathFilter2ParameterSpec
		Implements TransformParameterSpec

		Private ReadOnly xPathList As IList(Of XPathType)

		''' <summary>
		''' Creates an <code>XPathFilter2ParameterSpec</code>.
		''' </summary>
		''' <param name="xPathList"> a list of one or more <seealso cref="XPathType"/> objects. The
		'''    list is defensively copied to protect against subsequent modification. </param>
		''' <exception cref="ClassCastException"> if <code>xPathList</code> contains any
		'''    entries that are not of type <seealso cref="XPathType"/> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>xPathList</code> is empty </exception>
		''' <exception cref="NullPointerException"> if <code>xPathList</code> is
		'''    <code>null</code> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal xPathList As IList)
			If xPathList Is Nothing Then Throw New NullPointerException("xPathList cannot be null")
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim xPathListCopy As IList(Of ?) = New List(Of ?)(CType(xPathList, IList(Of ?)))
			If xPathListCopy.Count = 0 Then Throw New System.ArgumentException("xPathList cannot be empty")
			Dim size As Integer = xPathListCopy.Count
			For i As Integer = 0 To size - 1
				If Not(TypeOf xPathListCopy(i) Is XPathType) Then Throw New ClassCastException("xPathList[" & i & "] is not a valid type")
			Next i

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim temp As IList(Of XPathType) = CType(xPathListCopy, IList(Of XPathType))

			Me.xPathList = java.util.Collections.unmodifiableList(temp)
		End Sub

		''' <summary>
		''' Returns a list of one or more <seealso cref="XPathType"/> objects.
		''' <p>
		''' This implementation returns an {@link Collections#unmodifiableList
		''' unmodifiable list}.
		''' </summary>
		''' <returns> a <code>List</code> of <code>XPathType</code> objects
		'''    (never <code>null</code> or empty) </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Property xPathList As IList
			Get
				Return xPathList
			End Get
		End Property
	End Class

End Namespace