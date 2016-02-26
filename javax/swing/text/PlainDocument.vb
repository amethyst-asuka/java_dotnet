Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text


	''' <summary>
	''' A plain document that maintains no character attributes.  The
	''' default element structure for this document is a map of the lines in
	''' the text.  The Element returned by getDefaultRootElement is
	''' a map of the lines, and each child element represents a line.
	''' This model does not maintain any character level attributes,
	''' but each line can be tagged with an arbitrary set of attributes.
	''' Line to offset, and offset to line translations can be quickly
	''' performed using the default root element.  The structure information
	''' of the DocumentEvent's fired by edits will indicate the line
	''' structure changes.
	''' <p>
	''' The default content storage management is performed by a
	''' gapped buffer implementation (GapContent).  It supports
	''' editing reasonably large documents with good efficiency when
	''' the edits are contiguous or clustered, as is typical.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author  Timothy Prinzing </summary>
	''' <seealso cref=     Document </seealso>
	''' <seealso cref=     AbstractDocument </seealso>
	Public Class PlainDocument
		Inherits AbstractDocument

		''' <summary>
		''' Name of the attribute that specifies the tab
		''' size for tabs contained in the content.  The
		''' type for the value is Integer.
		''' </summary>
		Public Const tabSizeAttribute As String = "tabSize"

		''' <summary>
		''' Name of the attribute that specifies the maximum
		''' length of a line, if there is a maximum length.
		''' The type for the value is Integer.
		''' </summary>
		Public Const lineLimitAttribute As String = "lineLimit"

		''' <summary>
		''' Constructs a plain text document.  A default model using
		''' <code>GapContent</code> is constructed and set.
		''' </summary>
		Public Sub New()
			Me.New(New GapContent)
		End Sub

		''' <summary>
		''' Constructs a plain text document.  A default root element is created,
		''' and the tab size set to 8.
		''' </summary>
		''' <param name="c">  the container for the content </param>
		Public Sub New(ByVal c As Content)
			MyBase.New(c)
			putProperty(tabSizeAttribute, Convert.ToInt32(8))
			defaultRoot = createDefaultRoot()
		End Sub

		''' <summary>
		''' Inserts some content into the document.
		''' Inserting content causes a write lock to be held while the
		''' actual changes are taking place, followed by notification
		''' to the observers on the thread that grabbed the write lock.
		''' <p>
		''' This method is thread safe, although most Swing methods
		''' are not. Please see
		''' <A HREF="https://docs.oracle.com/javase/tutorial/uiswing/concurrency/index.html">Concurrency
		''' in Swing</A> for more information.
		''' </summary>
		''' <param name="offs"> the starting offset &gt;= 0 </param>
		''' <param name="str"> the string to insert; does nothing with null/empty strings </param>
		''' <param name="a"> the attributes for the inserted content </param>
		''' <exception cref="BadLocationException">  the given insert position is not a valid
		'''   position within the document </exception>
		''' <seealso cref= Document#insertString </seealso>
		Public Overrides Sub insertString(ByVal offs As Integer, ByVal str As String, ByVal a As AttributeSet)
			' fields don't want to have multiple lines.  We may provide a field-specific
			' model in the future in which case the filtering logic here will no longer
			' be needed.
			Dim filterNewlines As Object = getProperty("filterNewlines")
			If (TypeOf filterNewlines Is Boolean?) AndAlso filterNewlines.Equals(Boolean.TRUE) Then
				If (str IsNot Nothing) AndAlso (str.IndexOf(ControlChars.Lf) >= 0) Then
					Dim filtered As New StringBuilder(str)
					Dim n As Integer = filtered.Length
					For i As Integer = 0 To n - 1
						If filtered.Chars(i) = ControlChars.Lf Then filtered(i) = " "c
					Next i
					str = filtered.ToString()
				End If
			End If
			MyBase.insertString(offs, str, a)
		End Sub

		''' <summary>
		''' Gets the default root element for the document model.
		''' </summary>
		''' <returns> the root </returns>
		''' <seealso cref= Document#getDefaultRootElement </seealso>
		Public Property Overrides defaultRootElement As Element
			Get
				Return defaultRoot
			End Get
		End Property

		''' <summary>
		''' Creates the root element to be used to represent the
		''' default document structure.
		''' </summary>
		''' <returns> the element base </returns>
		Protected Friend Overridable Function createDefaultRoot() As AbstractElement
			Dim map As BranchElement = CType(createBranchElement(Nothing, Nothing), BranchElement)
			Dim line As Element = createLeafElement(map, Nothing, 0, 1)
			Dim lines As Element() = New Element(0){}
			lines(0) = line
			map.replace(0, 0, lines)
			Return map
		End Function

		''' <summary>
		''' Get the paragraph element containing the given position.  Since this
		''' document only models lines, it returns the line instead.
		''' </summary>
		Public Overrides Function getParagraphElement(ByVal pos As Integer) As Element
			Dim lineMap As Element = defaultRootElement
			Return lineMap.getElement(lineMap.getElementIndex(pos))
		End Function

		''' <summary>
		''' Updates document structure as a result of text insertion.  This
		''' will happen within a write lock.  Since this document simply
		''' maps out lines, we refresh the line map.
		''' </summary>
		''' <param name="chng"> the change event describing the dit </param>
		''' <param name="attr"> the set of attributes for the inserted text </param>
		Protected Friend Overrides Sub insertUpdate(ByVal chng As DefaultDocumentEvent, ByVal attr As AttributeSet)
			removed.Clear()
			added.Clear()
			Dim lineMap As BranchElement = CType(defaultRootElement, BranchElement)
			Dim offset As Integer = chng.offset
			Dim ___length As Integer = chng.length
			If offset > 0 Then
			  offset -= 1
			  ___length += 1
			End If
			Dim index As Integer = lineMap.getElementIndex(offset)
			Dim rmCandidate As Element = lineMap.getElement(index)
			Dim rmOffs0 As Integer = rmCandidate.startOffset
			Dim rmOffs1 As Integer = rmCandidate.endOffset
			Dim lastOffset As Integer = rmOffs0
			Try
				If s Is Nothing Then s = New Segment
				content.getChars(offset, ___length, s)
				Dim hasBreaks As Boolean = False
				For i As Integer = 0 To ___length - 1
					Dim c As Char = s.array(s.offset + i)
					If c = ControlChars.Lf Then
						Dim breakOffset As Integer = offset + i + 1
						added.Add(createLeafElement(lineMap, Nothing, lastOffset, breakOffset))
						lastOffset = breakOffset
						hasBreaks = True
					End If
				Next i
				If hasBreaks Then
					removed.Add(rmCandidate)
					If (offset + ___length = rmOffs1) AndAlso (lastOffset <> rmOffs1) AndAlso ((index+1) < lineMap.elementCount) Then
						Dim e As Element = lineMap.getElement(index+1)
						removed.Add(e)
						rmOffs1 = e.endOffset
					End If
					If lastOffset < rmOffs1 Then added.Add(createLeafElement(lineMap, Nothing, lastOffset, rmOffs1))

					Dim aelems As Element() = New Element(added.Count - 1){}
					added.CopyTo(aelems)
					Dim relems As Element() = New Element(removed.Count - 1){}
					removed.CopyTo(relems)
					Dim ee As New ElementEdit(lineMap, index, relems, aelems)
					chng.addEdit(ee)
					lineMap.replace(index, relems.Length, aelems)
				End If
				If Utilities.isComposedTextAttributeDefined(attr) Then insertComposedTextUpdate(chng, attr)
			Catch e As BadLocationException
				Throw New Exception("Internal error: " & e.ToString())
			End Try
			MyBase.insertUpdate(chng, attr)
		End Sub

		''' <summary>
		''' Updates any document structure as a result of text removal.
		''' This will happen within a write lock. Since the structure
		''' represents a line map, this just checks to see if the
		''' removal spans lines.  If it does, the two lines outside
		''' of the removal area are joined together.
		''' </summary>
		''' <param name="chng"> the change event describing the edit </param>
		Protected Friend Overrides Sub removeUpdate(ByVal chng As DefaultDocumentEvent)
			removed.Clear()
			Dim map As BranchElement = CType(defaultRootElement, BranchElement)
			Dim offset As Integer = chng.offset
			Dim ___length As Integer = chng.length
			Dim line0 As Integer = map.getElementIndex(offset)
			Dim line1 As Integer = map.getElementIndex(offset + ___length)
			If line0 <> line1 Then
				' a line was removed
				For i As Integer = line0 To line1
					removed.Add(map.getElement(i))
				Next i
				Dim p0 As Integer = map.getElement(line0).startOffset
				Dim p1 As Integer = map.getElement(line1).endOffset
				Dim aelems As Element() = New Element(0){}
				aelems(0) = createLeafElement(map, Nothing, p0, p1)
				Dim relems As Element() = New Element(removed.Count - 1){}
				removed.CopyTo(relems)
				Dim ee As New ElementEdit(map, line0, relems, aelems)
				chng.addEdit(ee)
				map.replace(line0, relems.Length, aelems)
			Else
				'Check for the composed text element
				Dim line As Element = map.getElement(line0)
				If Not line.leaf Then
					Dim leaf As Element = line.getElement(line.getElementIndex(offset))
					If Utilities.isComposedTextElement(leaf) Then
						Dim aelem As Element() = New Element(0){}
						aelem(0) = createLeafElement(map, Nothing, line.startOffset, line.endOffset)
						Dim relem As Element() = New Element(0){}
						relem(0) = line
						Dim ee As New ElementEdit(map, line0, relem, aelem)
						chng.addEdit(ee)
						map.replace(line0, 1, aelem)
					End If
				End If
			End If
			MyBase.removeUpdate(chng)
		End Sub

		'
		' Inserts the composed text of an input method. The line element
		' where the composed text is inserted into becomes an branch element
		' which contains leaf elements of the composed text and the text
		' backing store.
		'
		Private Sub insertComposedTextUpdate(ByVal chng As DefaultDocumentEvent, ByVal attr As AttributeSet)
			added.Clear()
			Dim lineMap As BranchElement = CType(defaultRootElement, BranchElement)
			Dim offset As Integer = chng.offset
			Dim ___length As Integer = chng.length
			Dim index As Integer = lineMap.getElementIndex(offset)
			Dim elem As Element = lineMap.getElement(index)
			Dim elemStart As Integer = elem.startOffset
			Dim elemEnd As Integer = elem.endOffset
			Dim abelem As BranchElement() = New BranchElement(0){}
			abelem(0) = CType(createBranchElement(lineMap, Nothing), BranchElement)
			Dim relem As Element() = New Element(0){}
			relem(0) = elem
			If elemStart <> offset Then added.Add(createLeafElement(abelem(0), Nothing, elemStart, offset))
			added.Add(createLeafElement(abelem(0), attr, offset, offset+___length))
			If elemEnd <> offset+___length Then added.Add(createLeafElement(abelem(0), Nothing, offset+___length, elemEnd))
			Dim alelem As Element() = New Element(added.Count - 1){}
			added.CopyTo(alelem)
			Dim ee As New ElementEdit(lineMap, index, relem, abelem)
			chng.addEdit(ee)

			abelem(0).replace(0, 0, alelem)
			lineMap.replace(index, 1, abelem)
		End Sub

		Private defaultRoot As AbstractElement
		Private added As New List(Of Element)
		Private removed As New List(Of Element)
		<NonSerialized> _
		Private s As Segment
	End Class

End Namespace