'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.datatransfer



	''' <summary>
	''' A <code>Transferable</code> which implements the capability required
	''' to transfer a <code>String</code>.
	''' 
	''' This <code>Transferable</code> properly supports
	''' <code>DataFlavor.stringFlavor</code>
	''' and all equivalent flavors. Support for
	''' <code>DataFlavor.plainTextFlavor</code>
	''' and all equivalent flavors is <b>deprecated</b>. No other
	''' <code>DataFlavor</code>s are supported.
	''' </summary>
	''' <seealso cref= java.awt.datatransfer.DataFlavor#stringFlavor </seealso>
	''' <seealso cref= java.awt.datatransfer.DataFlavor#plainTextFlavor </seealso>
	Public Class StringSelection
		Implements Transferable, ClipboardOwner

		Private Const [STRING] As Integer = 0
		Private Const PLAIN_TEXT As Integer = 1

		Private Shared ReadOnly flavors As DataFlavor() = { DataFlavor.stringFlavor, DataFlavor.plainTextFlavor }

		Private data As String

		''' <summary>
		''' Creates a <code>Transferable</code> capable of transferring
		''' the specified <code>String</code>.
		''' </summary>
		Public Sub New(ByVal data As String)
			Me.data = data
		End Sub

		''' <summary>
		''' Returns an array of flavors in which this <code>Transferable</code>
		''' can provide the data. <code>DataFlavor.stringFlavor</code>
		''' is properly supported.
		''' Support for <code>DataFlavor.plainTextFlavor</code> is
		''' <b>deprecated</b>.
		''' </summary>
		''' <returns> an array of length two, whose elements are <code>DataFlavor.
		'''         stringFlavor</code> and <code>DataFlavor.plainTextFlavor</code> </returns>
		Public Overridable Property transferDataFlavors As DataFlavor() Implements Transferable.getTransferDataFlavors
			Get
				' returning flavors itself would allow client code to modify
				' our internal behavior
				Return CType(flavors.clone(), DataFlavor())
			End Get
		End Property

		''' <summary>
		''' Returns whether the requested flavor is supported by this
		''' <code>Transferable</code>.
		''' </summary>
		''' <param name="flavor"> the requested flavor for the data </param>
		''' <returns> true if <code>flavor</code> is equal to
		'''   <code>DataFlavor.stringFlavor</code> or
		'''   <code>DataFlavor.plainTextFlavor</code>; false if <code>flavor</code>
		'''   is not one of the above flavors </returns>
		''' <exception cref="NullPointerException"> if flavor is <code>null</code> </exception>
		Public Overridable Function isDataFlavorSupported(ByVal flavor As DataFlavor) As Boolean Implements Transferable.isDataFlavorSupported
			' JCK Test StringSelection0003: if 'flavor' is null, throw NPE
			For i As Integer = 0 To flavors.Length - 1
				If flavor.Equals(flavors(i)) Then Return True
			Next i
			Return False
		End Function

		''' <summary>
		''' Returns the <code>Transferable</code>'s data in the requested
		''' <code>DataFlavor</code> if possible. If the desired flavor is
		''' <code>DataFlavor.stringFlavor</code>, or an equivalent flavor,
		''' the <code>String</code> representing the selection is
		''' returned. If the desired flavor is
		''' <code>DataFlavor.plainTextFlavor</code>,
		''' or an equivalent flavor, a <code>Reader</code> is returned.
		''' <b>Note:</b> The behavior of this method for
		''' <code>DataFlavor.plainTextFlavor</code>
		''' and equivalent <code>DataFlavor</code>s is inconsistent with the
		''' definition of <code>DataFlavor.plainTextFlavor</code>.
		''' </summary>
		''' <param name="flavor"> the requested flavor for the data </param>
		''' <returns> the data in the requested flavor, as outlined above </returns>
		''' <exception cref="UnsupportedFlavorException"> if the requested data flavor is
		'''         not equivalent to either <code>DataFlavor.stringFlavor</code>
		'''         or <code>DataFlavor.plainTextFlavor</code> </exception>
		''' <exception cref="IOException"> if an IOException occurs while retrieving the data.
		'''         By default, StringSelection never throws this exception, but a
		'''         subclass may. </exception>
		''' <exception cref="NullPointerException"> if flavor is <code>null</code> </exception>
		''' <seealso cref= java.io.Reader </seealso>
		Public Overridable Function getTransferData(ByVal flavor As DataFlavor) As Object Implements Transferable.getTransferData
			' JCK Test StringSelection0007: if 'flavor' is null, throw NPE
			If flavor.Equals(flavors([STRING])) Then
				Return CObj(data)
			ElseIf flavor.Equals(flavors(PLAIN_TEXT)) Then
				Return New StringReader(If(data Is Nothing, "", data))
			Else
				Throw New UnsupportedFlavorException(flavor)
			End If
		End Function

		Public Overridable Sub lostOwnership(ByVal clipboard As Clipboard, ByVal contents As Transferable) Implements ClipboardOwner.lostOwnership
		End Sub
	End Class

End Namespace