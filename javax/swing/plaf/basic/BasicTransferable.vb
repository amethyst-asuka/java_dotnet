Imports System

'
' * Copyright (c) 2000, 2002, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.plaf.basic



	''' <summary>
	''' A transferable implementation for the default data transfer of some Swing
	''' components.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Friend Class BasicTransferable
		Implements Transferable, javax.swing.plaf.UIResource

		Protected Friend plainData As String
		Protected Friend htmlData As String

		Private Shared htmlFlavors As DataFlavor()
		Private Shared stringFlavors As DataFlavor()
		Private Shared plainFlavors As DataFlavor()

		Shared Sub New()
			Try
				htmlFlavors = New DataFlavor(2){}
				htmlFlavors(0) = New DataFlavor("text/html;class=java.lang.String")
				htmlFlavors(1) = New DataFlavor("text/html;class=java.io.Reader")
				htmlFlavors(2) = New DataFlavor("text/html;charset=unicode;class=java.io.InputStream")

				plainFlavors = New DataFlavor(2){}
				plainFlavors(0) = New DataFlavor("text/plain;class=java.lang.String")
				plainFlavors(1) = New DataFlavor("text/plain;class=java.io.Reader")
				plainFlavors(2) = New DataFlavor("text/plain;charset=unicode;class=java.io.InputStream")

				stringFlavors = New DataFlavor(1){}
				stringFlavors(0) = New DataFlavor(DataFlavor.javaJVMLocalObjectMimeType & ";class=java.lang.String")
				stringFlavors(1) = DataFlavor.stringFlavor

			Catch cle As ClassNotFoundException
				Console.Error.WriteLine("error initializing javax.swing.plaf.basic.BasicTranserable")
			End Try
		End Sub

		Public Sub New(ByVal plainData As String, ByVal htmlData As String)
			Me.plainData = plainData
			Me.htmlData = htmlData
		End Sub


		''' <summary>
		''' Returns an array of DataFlavor objects indicating the flavors the data
		''' can be provided in.  The array should be ordered according to preference
		''' for providing the data (from most richly descriptive to least descriptive). </summary>
		''' <returns> an array of data flavors in which this data can be transferred </returns>
		Public Overridable Property transferDataFlavors As DataFlavor()
			Get
				Dim ___richerFlavors As DataFlavor() = richerFlavors
				Dim nRicher As Integer = If(___richerFlavors IsNot Nothing, ___richerFlavors.Length, 0)
				Dim nHTML As Integer = If(hTMLSupported, htmlFlavors.Length, 0)
				Dim nPlain As Integer = If(plainSupported, plainFlavors.Length, 0)
				Dim nString As Integer = If(plainSupported, stringFlavors.Length, 0)
				Dim nFlavors As Integer = nRicher + nHTML + nPlain + nString
				Dim flavors As DataFlavor() = New DataFlavor(nFlavors - 1){}
    
				' fill in the array
				Dim nDone As Integer = 0
				If nRicher > 0 Then
					Array.Copy(___richerFlavors, 0, flavors, nDone, nRicher)
					nDone += nRicher
				End If
				If nHTML > 0 Then
					Array.Copy(htmlFlavors, 0, flavors, nDone, nHTML)
					nDone += nHTML
				End If
				If nPlain > 0 Then
					Array.Copy(plainFlavors, 0, flavors, nDone, nPlain)
					nDone += nPlain
				End If
				If nString > 0 Then
					Array.Copy(stringFlavors, 0, flavors, nDone, nString)
					nDone += nString
				End If
				Return flavors
			End Get
		End Property

		''' <summary>
		''' Returns whether or not the specified data flavor is supported for
		''' this object. </summary>
		''' <param name="flavor"> the requested flavor for the data </param>
		''' <returns> boolean indicating whether or not the data flavor is supported </returns>
		Public Overridable Function isDataFlavorSupported(ByVal flavor As DataFlavor) As Boolean
			Dim flavors As DataFlavor() = transferDataFlavors
			For i As Integer = 0 To flavors.Length - 1
				If flavors(i).Equals(flavor) Then Return True
			Next i
			Return False
		End Function

		''' <summary>
		''' Returns an object which represents the data to be transferred.  The class
		''' of the object returned is defined by the representation class of the flavor.
		''' </summary>
		''' <param name="flavor"> the requested flavor for the data </param>
		''' <seealso cref= DataFlavor#getRepresentationClass </seealso>
		''' <exception cref="IOException">                if the data is no longer available
		'''              in the requested flavor. </exception>
		''' <exception cref="UnsupportedFlavorException"> if the requested data flavor is
		'''              not supported. </exception>
		Public Overridable Function getTransferData(ByVal flavor As DataFlavor) As Object
			Dim ___richerFlavors As DataFlavor() = richerFlavors
			If isRicherFlavor(flavor) Then
				Return getRicherData(flavor)
			ElseIf isHTMLFlavor(flavor) Then
				Dim data As String = hTMLData
				data = If(data Is Nothing, "", data)
				If GetType(String).Equals(flavor.representationClass) Then
					Return data
				ElseIf GetType(Reader).Equals(flavor.representationClass) Then
					Return New StringReader(data)
				ElseIf GetType(InputStream).Equals(flavor.representationClass) Then
					Return createInputStream(flavor, data)
				End If
				' fall through to unsupported
			ElseIf isPlainFlavor(flavor) Then
				Dim data As String = plainData
				data = If(data Is Nothing, "", data)
				If GetType(String).Equals(flavor.representationClass) Then
					Return data
				ElseIf GetType(Reader).Equals(flavor.representationClass) Then
					Return New StringReader(data)
				ElseIf GetType(InputStream).Equals(flavor.representationClass) Then
					Return createInputStream(flavor, data)
				End If
				' fall through to unsupported

			ElseIf isStringFlavor(flavor) Then
				Dim data As String = plainData
				data = If(data Is Nothing, "", data)
				Return data
			End If
			Throw New UnsupportedFlavorException(flavor)
		End Function

		Private Function createInputStream(ByVal flavor As DataFlavor, ByVal data As String) As InputStream
			Dim cs As String = sun.awt.datatransfer.DataTransferer.getTextCharset(flavor)
			If cs Is Nothing Then Throw New UnsupportedFlavorException(flavor)
			Return New ByteArrayInputStream(data.getBytes(cs))
		End Function

		' --- richer subclass flavors ----------------------------------------------

		Protected Friend Overridable Function isRicherFlavor(ByVal flavor As DataFlavor) As Boolean
			Dim ___richerFlavors As DataFlavor() = richerFlavors
			Dim nFlavors As Integer = If(___richerFlavors IsNot Nothing, ___richerFlavors.Length, 0)
			For i As Integer = 0 To nFlavors - 1
				If ___richerFlavors(i).Equals(flavor) Then Return True
			Next i
			Return False
		End Function

		''' <summary>
		''' Some subclasses will have flavors that are more descriptive than HTML
		''' or plain text.  If this method returns a non-null value, it will be
		''' placed at the start of the array of supported flavors.
		''' </summary>
		Protected Friend Overridable Property richerFlavors As DataFlavor()
			Get
				Return Nothing
			End Get
		End Property

		Protected Friend Overridable Function getRicherData(ByVal flavor As DataFlavor) As Object
			Return Nothing
		End Function

		' --- html flavors ----------------------------------------------------------

		''' <summary>
		''' Returns whether or not the specified data flavor is an HTML flavor that
		''' is supported. </summary>
		''' <param name="flavor"> the requested flavor for the data </param>
		''' <returns> boolean indicating whether or not the data flavor is supported </returns>
		Protected Friend Overridable Function isHTMLFlavor(ByVal flavor As DataFlavor) As Boolean
			Dim flavors As DataFlavor() = htmlFlavors
			For i As Integer = 0 To flavors.Length - 1
				If flavors(i).Equals(flavor) Then Return True
			Next i
			Return False
		End Function

		''' <summary>
		''' Should the HTML flavors be offered?  If so, the method
		''' getHTMLData should be implemented to provide something reasonable.
		''' </summary>
		Protected Friend Overridable Property hTMLSupported As Boolean
			Get
				Return htmlData IsNot Nothing
			End Get
		End Property

		''' <summary>
		''' Fetch the data in a text/html format
		''' </summary>
		Protected Friend Overridable Property hTMLData As String
			Get
				Return htmlData
			End Get
		End Property

		' --- plain text flavors ----------------------------------------------------

		''' <summary>
		''' Returns whether or not the specified data flavor is an plain flavor that
		''' is supported. </summary>
		''' <param name="flavor"> the requested flavor for the data </param>
		''' <returns> boolean indicating whether or not the data flavor is supported </returns>
		Protected Friend Overridable Function isPlainFlavor(ByVal flavor As DataFlavor) As Boolean
			Dim flavors As DataFlavor() = plainFlavors
			For i As Integer = 0 To flavors.Length - 1
				If flavors(i).Equals(flavor) Then Return True
			Next i
			Return False
		End Function

		''' <summary>
		''' Should the plain text flavors be offered?  If so, the method
		''' getPlainData should be implemented to provide something reasonable.
		''' </summary>
		Protected Friend Overridable Property plainSupported As Boolean
			Get
				Return plainData IsNot Nothing
			End Get
		End Property

		''' <summary>
		''' Fetch the data in a text/plain format.
		''' </summary>
		Protected Friend Overridable Property plainData As String
			Get
				Return plainData
			End Get
		End Property

		' --- string flavorss --------------------------------------------------------

		''' <summary>
		''' Returns whether or not the specified data flavor is a String flavor that
		''' is supported. </summary>
		''' <param name="flavor"> the requested flavor for the data </param>
		''' <returns> boolean indicating whether or not the data flavor is supported </returns>
		Protected Friend Overridable Function isStringFlavor(ByVal flavor As DataFlavor) As Boolean
			Dim flavors As DataFlavor() = stringFlavors
			For i As Integer = 0 To flavors.Length - 1
				If flavors(i).Equals(flavor) Then Return True
			Next i
			Return False
		End Function


	End Class

End Namespace