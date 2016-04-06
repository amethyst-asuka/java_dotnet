'
' * Copyright (c) 2000, 2012, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.beans




	''' <summary>
	''' The <code>XMLDecoder</code> class is used to read XML documents
	''' created using the <code>XMLEncoder</code> and is used just like
	''' the <code>ObjectInputStream</code>. For example, one can use
	''' the following fragment to read the first object defined
	''' in an XML document written by the <code>XMLEncoder</code>
	''' class:
	''' <pre>
	'''       XMLDecoder d = new XMLDecoder(
	'''                          new BufferedInputStream(
	'''                              new FileInputStream("Test.xml")));
	'''       Object result = d.readObject();
	'''       d.close();
	''' </pre>
	''' 
	''' <p>
	''' For more information you might also want to check out
	''' <a
	''' href="http://java.sun.com/products/jfc/tsc/articles/persistence3">Long Term Persistence of JavaBeans Components: XML Schema</a>,
	''' an article in <em>The Swing Connection.</em> </summary>
	''' <seealso cref= XMLEncoder </seealso>
	''' <seealso cref= java.io.ObjectInputStream
	''' 
	''' @since 1.4
	''' 
	''' @author Philip Milne </seealso>
	Public Class XMLDecoder
		Implements AutoCloseable

		Private ReadOnly acc As java.security.AccessControlContext = java.security.AccessController.context
		Private ReadOnly handler As New com.sun.beans.decoder.DocumentHandler
		Private ReadOnly input As org.xml.sax.InputSource
		Private owner As Object
		Private array As Object()
		Private index As Integer

		''' <summary>
		''' Creates a new input stream for reading archives
		''' created by the <code>XMLEncoder</code> class.
		''' </summary>
		''' <param name="in"> The underlying stream.
		''' </param>
		''' <seealso cref= XMLEncoder#XMLEncoder(java.io.OutputStream) </seealso>
		Public Sub New(  [in] As java.io.InputStream)
			Me.New([in], Nothing)
		End Sub

		''' <summary>
		''' Creates a new input stream for reading archives
		''' created by the <code>XMLEncoder</code> class.
		''' </summary>
		''' <param name="in"> The underlying stream. </param>
		''' <param name="owner"> The owner of this stream.
		'''  </param>
		Public Sub New(  [in] As java.io.InputStream,   owner As Object)
			Me.New([in], owner, Nothing)
		End Sub

		''' <summary>
		''' Creates a new input stream for reading archives
		''' created by the <code>XMLEncoder</code> class.
		''' </summary>
		''' <param name="in"> the underlying stream. </param>
		''' <param name="owner"> the owner of this stream. </param>
		''' <param name="exceptionListener"> the exception handler for the stream;
		'''        if <code>null</code> the default exception listener will be used. </param>
		Public Sub New(  [in] As java.io.InputStream,   owner As Object,   exceptionListener As ExceptionListener)
			Me.New([in], owner, exceptionListener, Nothing)
		End Sub

		''' <summary>
		''' Creates a new input stream for reading archives
		''' created by the <code>XMLEncoder</code> class.
		''' </summary>
		''' <param name="in"> the underlying stream.  <code>null</code> may be passed without
		'''        error, though the resulting XMLDecoder will be useless </param>
		''' <param name="owner"> the owner of this stream.  <code>null</code> is a legal
		'''        value </param>
		''' <param name="exceptionListener"> the exception handler for the stream, or
		'''        <code>null</code> to use the default </param>
		''' <param name="cl"> the class loader used for instantiating objects.
		'''        <code>null</code> indicates that the default class loader should
		'''        be used
		''' @since 1.5 </param>
		Public Sub New(  [in] As java.io.InputStream,   owner As Object,   exceptionListener As ExceptionListener,   cl As  ClassLoader)
			Me.New(New org.xml.sax.InputSource([in]), owner, exceptionListener, cl)
		End Sub


		''' <summary>
		''' Creates a new decoder to parse XML archives
		''' created by the {@code XMLEncoder} class.
		''' If the input source {@code is} is {@code null},
		''' no exception is thrown and no parsing is performed.
		''' This behavior is similar to behavior of other constructors
		''' that use {@code InputStream} as a parameter.
		''' </summary>
		''' <param name="is">  the input source to parse
		''' 
		''' @since 1.7 </param>
		Public Sub New(  [is] As org.xml.sax.InputSource)
			Me.New([is], Nothing, Nothing, Nothing)
		End Sub

		''' <summary>
		''' Creates a new decoder to parse XML archives
		''' created by the {@code XMLEncoder} class.
		''' </summary>
		''' <param name="is">     the input source to parse </param>
		''' <param name="owner">  the owner of this decoder </param>
		''' <param name="el">     the exception handler for the parser,
		'''               or {@code null} to use the default exception handler </param>
		''' <param name="cl">     the class loader used for instantiating objects,
		'''               or {@code null} to use the default class loader
		''' 
		''' @since 1.7 </param>
		Private Sub New(  [is] As org.xml.sax.InputSource,   owner As Object,   el As ExceptionListener,   cl As  ClassLoader)
			Me.input = [is]
			Me.owner = owner
			exceptionListener = el
			Me.handler.classLoader = cl
			Me.handler.owner = Me
		End Sub

		''' <summary>
		''' This method closes the input stream associated
		''' with this stream.
		''' </summary>
		Public Overridable Sub close() Implements AutoCloseable.close
			If parsingComplete() Then
				close(Me.input.characterStream)
				close(Me.input.byteStream)
			End If
		End Sub

		Private Sub close(  [in] As java.io.Closeable)
			If [in] IsNot Nothing Then
				Try
					[in].close()
				Catch e As java.io.IOException
					exceptionListener.exceptionThrown(e)
				End Try
			End If
		End Sub

		Private Function parsingComplete() As Boolean
			If Me.input Is Nothing Then Return False
			If Me.array Is Nothing Then
				If (Me.acc Is Nothing) AndAlso (Nothing IsNot System.securityManager) Then Throw New SecurityException("AccessControlContext is not set")
				java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
				Me.array = Me.handler.objects
			End If
			Return True
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As Void
				outerInstance.handler.parse(outerInstance.input)
				Return Nothing
			End Function
		End Class

		''' <summary>
		''' Sets the exception handler for this stream to <code>exceptionListener</code>.
		''' The exception handler is notified when this stream catches recoverable
		''' exceptions.
		''' </summary>
		''' <param name="exceptionListener"> The exception handler for this stream;
		''' if <code>null</code> the default exception listener will be used.
		''' </param>
		''' <seealso cref= #getExceptionListener </seealso>
		Public Overridable Property exceptionListener As ExceptionListener
			Set(  exceptionListener As ExceptionListener)
				If exceptionListener Is Nothing Then exceptionListener = Statement.defaultExceptionListener
				Me.handler.exceptionListener = exceptionListener
			End Set
			Get
				Return Me.handler.exceptionListener
			End Get
		End Property


		''' <summary>
		''' Reads the next object from the underlying input stream.
		''' </summary>
		''' <returns> the next object read
		''' </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the stream contains no objects
		'''         (or no more objects)
		''' </exception>
		''' <seealso cref= XMLEncoder#writeObject </seealso>
		Public Overridable Function readObject() As Object
				Dim tempVar As Integer = Me.index
				Me.index += 1
				Return If(parsingComplete(), Me.array(tempVar), Nothing)
		End Function

		''' <summary>
		''' Sets the owner of this decoder to <code>owner</code>.
		''' </summary>
		''' <param name="owner"> The owner of this decoder.
		''' </param>
		''' <seealso cref= #getOwner </seealso>
		Public Overridable Property owner As Object
			Set(  owner As Object)
				Me.owner = owner
			End Set
			Get
				Return owner
			End Get
		End Property


		''' <summary>
		''' Creates a new handler for SAX parser
		''' that can be used to parse embedded XML archives
		''' created by the {@code XMLEncoder} class.
		''' 
		''' The {@code owner} should be used if parsed XML document contains
		''' the method call within context of the &lt;java&gt; element.
		''' The {@code null} value may cause illegal parsing in such case.
		''' The same problem may occur, if the {@code owner} class
		''' does not contain expected method to call. See details <a
		''' href="http://java.sun.com/products/jfc/tsc/articles/persistence3/">here</a>.
		''' </summary>
		''' <param name="owner">  the owner of the default handler
		'''               that can be used as a value of &lt;java&gt; element </param>
		''' <param name="el">     the exception handler for the parser,
		'''               or {@code null} to use the default exception handler </param>
		''' <param name="cl">     the class loader used for instantiating objects,
		'''               or {@code null} to use the default class loader </param>
		''' <returns> an instance of {@code DefaultHandler} for SAX parser
		''' 
		''' @since 1.7 </returns>
		Public Shared Function createHandler(  owner As Object,   el As ExceptionListener,   cl As  ClassLoader) As org.xml.sax.helpers.DefaultHandler
			Dim handler As New com.sun.beans.decoder.DocumentHandler
			handler.owner = owner
			handler.exceptionListener = el
			handler.classLoader = cl
			Return handler
		End Function
	End Class

End Namespace