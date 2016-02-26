Imports System
Imports System.Threading

'
' * Copyright (c) 2001, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.print


	''' <summary>
	''' This class is an implementation of interface <code>Doc</code> that can
	''' be used in many common printing requests.
	''' It can handle all of the presently defined "pre-defined" doc flavors
	''' defined as static variables in the DocFlavor class.
	''' <p>
	''' In particular this class implements certain required semantics of the
	''' Doc specification as follows:
	''' <ul>
	''' <li>constructs a stream for the service if requested and appropriate.
	''' <li>ensures the same object is returned for each call on a method.
	''' <li>ensures multiple threads can access the Doc
	''' <li>performs some validation of that the data matches the doc flavor.
	''' </ul>
	''' Clients who want to re-use the doc object in other jobs,
	''' or need a MultiDoc will not want to use this class.
	''' <p>
	''' If the print data is a stream, or a print job requests data as a
	''' stream, then <code>SimpleDoc</code> does not monitor if the service
	''' properly closes the stream after data transfer completion or job
	''' termination.
	''' Clients may prefer to use provide their own implementation of doc that
	''' adds a listener to monitor job completion and to validate that
	''' resources such as streams are freed (ie closed).
	''' </summary>

	Public NotInheritable Class SimpleDoc
		Implements Doc

		Private flavor As DocFlavor
		Private attributes As javax.print.attribute.DocAttributeSet
		Private printData As Object
		Private reader As java.io.Reader
		Private inStream As java.io.InputStream

		''' <summary>
		''' Constructs a <code>SimpleDoc</code> with the specified
		''' print data, doc flavor and doc attribute set. </summary>
		''' <param name="printData"> the print data object </param>
		''' <param name="flavor"> the <code>DocFlavor</code> object </param>
		''' <param name="attributes"> a <code>DocAttributeSet</code>, which can
		'''                   be <code>null</code> </param>
		''' <exception cref="IllegalArgumentException"> if <code>flavor</code> or
		'''         <code>printData</code> is <code>null</code>, or the
		'''         <code>printData</code> does not correspond
		'''         to the specified doc flavor--for example, the data is
		'''         not of the type specified as the representation in the
		'''         <code>DocFlavor</code>. </exception>
		Public Sub New(ByVal printData As Object, ByVal flavor As DocFlavor, ByVal attributes As javax.print.attribute.DocAttributeSet)

		   If flavor Is Nothing OrElse printData Is Nothing Then Throw New System.ArgumentException("null argument(s)")

		   Dim repClass As Type = Nothing
		   Try
				Dim className As String = flavor.representationClassName
				sun.reflect.misc.ReflectUtil.checkPackageAccess(className)
				repClass = Type.GetType(className, False, Thread.CurrentThread.contextClassLoader)
		   Catch e As Exception
			   Throw New System.ArgumentException("unknown representation class")
		   End Try

		   If Not repClass.IsInstanceOfType(printData) Then Throw New System.ArgumentException("data is not of declared type")

		   Me.flavor = flavor
		   If attributes IsNot Nothing Then Me.attributes = javax.print.attribute.AttributeSetUtilities.unmodifiableView(attributes)
		   Me.printData = printData
		End Sub

	   ''' <summary>
	   ''' Determines the doc flavor in which this doc object will supply its
	   ''' piece of print data.
	   ''' </summary>
	   ''' <returns>  Doc flavor. </returns>
		Public Property docFlavor As DocFlavor Implements Doc.getDocFlavor
			Get
				Return flavor
			End Get
		End Property

		''' <summary>
		''' Obtains the set of printing attributes for this doc object. If the
		''' returned attribute set includes an instance of a particular attribute
		''' <I>X,</I> the printer must use that attribute value for this doc,
		''' overriding any value of attribute <I>X</I> in the job's attribute set.
		''' If the returned attribute set does not include an instance
		''' of a particular attribute <I>X</I> or if null is returned, the printer
		''' must consult the job's attribute set to obtain the value for
		''' attribute <I>X,</I> and if not found there, the printer must use an
		''' implementation-dependent default value. The returned attribute set is
		''' unmodifiable.
		''' </summary>
		''' <returns>  Unmodifiable set of printing attributes for this doc, or null
		'''          to obtain all attribute values from the job's attribute
		'''          set. </returns>
		Public Property attributes As javax.print.attribute.DocAttributeSet Implements Doc.getAttributes
			Get
				Return attributes
			End Get
		End Property

	'    
	'     * Obtains the print data representation object that contains this doc
	'     * object's piece of print data in the format corresponding to the
	'     * supported doc flavor.
	'     * The <CODE>getPrintData()</CODE> method returns an instance of
	'     * the representation class whose name is given by
	'     * {@link DocFlavor#getRepresentationClassName() getRepresentationClassName},
	'     * and the return value can be cast
	'     * from class Object to that representation class.
	'     *
	'     * @return  Print data representation object.
	'     *
	'     * @exception  IOException if the representation class is a stream and
	'     *     there was an I/O error while constructing the stream.
	'     
		Public Property printData As Object Implements Doc.getPrintData
			Get
				Return printData
			End Get
		End Property

		''' <summary>
		''' Obtains a reader for extracting character print data from this doc.
		''' The <code>Doc</code> implementation is required to support this
		''' method if the <code>DocFlavor</code> has one of the following print
		''' data representation classes, and return <code>null</code>
		''' otherwise:
		''' <UL>
		''' <LI> <code>char[]</code>
		''' <LI> <code>java.lang.String</code>
		''' <LI> <code>java.io.Reader</code>
		''' </UL>
		''' The doc's print data representation object is used to construct and
		''' return a <code>Reader</code> for reading the print data as a stream
		''' of characters from the print data representation object.
		''' However, if the print data representation object is itself a
		''' <code>Reader</code> then the print data representation object is
		''' simply returned.
		''' <P> </summary>
		''' <returns>  a <code>Reader</code> for reading the print data
		'''          characters from this doc.
		'''          If a reader cannot be provided because this doc does not meet
		'''          the criteria stated above, <code>null</code> is returned.
		''' </returns>
		''' <exception cref="IOException"> if there was an I/O error while creating
		'''             the reader. </exception>
		Public Property readerForText As java.io.Reader Implements Doc.getReaderForText
			Get
    
				If TypeOf printData Is java.io.Reader Then Return CType(printData, java.io.Reader)
    
				SyncLock Me
					If reader IsNot Nothing Then Return reader
    
					If TypeOf printData Is Char() Then
					   reader = New java.io.CharArrayReader(CType(printData, Char()))
					ElseIf TypeOf printData Is String Then
						reader = New java.io.StringReader(CStr(printData))
					End If
				End SyncLock
				Return reader
			End Get
		End Property

		''' <summary>
		''' Obtains an input stream for extracting byte print data from
		''' this doc.
		''' The <code>Doc</code> implementation is required to support this
		''' method if the <code>DocFlavor</code> has one of the following print
		''' data representation classes; otherwise this method
		''' returns <code>null</code>:
		''' <UL>
		''' <LI> <code>byte[]</code>
		''' <LI> <code>java.io.InputStream</code>
		''' </UL>
		''' The doc's print data representation object is obtained.  Then, an
		''' input stream for reading the print data
		''' from the print data representation object as a stream of bytes is
		''' created and returned.
		''' However, if the print data representation object is itself an
		''' input stream then the print data representation object is simply
		''' returned.
		''' <P> </summary>
		''' <returns>  an <code>InputStream</code> for reading the print data
		'''          bytes from this doc.  If an input stream cannot be
		'''          provided because this doc does not meet
		'''          the criteria stated above, <code>null</code> is returned.
		''' </returns>
		''' <exception cref="IOException">
		'''     if there was an I/O error while creating the input stream. </exception>
		Public Property streamForBytes As java.io.InputStream Implements Doc.getStreamForBytes
			Get
    
				If TypeOf printData Is java.io.InputStream Then Return CType(printData, java.io.InputStream)
    
				SyncLock Me
					If inStream IsNot Nothing Then Return inStream
    
					If TypeOf printData Is SByte() Then inStream = New java.io.ByteArrayInputStream(CType(printData, SByte()))
				End SyncLock
				Return inStream
			End Get
		End Property

	End Class

End Namespace