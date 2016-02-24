Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>XMLEncoder</code> class is a complementary alternative to
	''' the <code>ObjectOutputStream</code> and can used to generate
	''' a textual representation of a <em>JavaBean</em> in the same
	''' way that the <code>ObjectOutputStream</code> can
	''' be used to create binary representation of <code>Serializable</code>
	''' objects. For example, the following fragment can be used to create
	''' a textual representation the supplied <em>JavaBean</em>
	''' and all its properties:
	''' <pre>
	'''       XMLEncoder e = new XMLEncoder(
	'''                          new BufferedOutputStream(
	'''                              new FileOutputStream("Test.xml")));
	'''       e.writeObject(new JButton("Hello, world"));
	'''       e.close();
	''' </pre>
	''' Despite the similarity of their APIs, the <code>XMLEncoder</code>
	''' class is exclusively designed for the purpose of archiving graphs
	''' of <em>JavaBean</em>s as textual representations of their public
	''' properties. Like Java source files, documents written this way
	''' have a natural immunity to changes in the implementations of the classes
	''' involved. The <code>ObjectOutputStream</code> continues to be recommended
	''' for interprocess communication and general purpose serialization.
	''' <p>
	''' The <code>XMLEncoder</code> class provides a default denotation for
	''' <em>JavaBean</em>s in which they are represented as XML documents
	''' complying with version 1.0 of the XML specification and the
	''' UTF-8 character encoding of the Unicode/ISO 10646 character set.
	''' The XML documents produced by the <code>XMLEncoder</code> class are:
	''' <ul>
	''' <li>
	''' <em>Portable and version resilient</em>: they have no dependencies
	''' on the private implementation of any class and so, like Java source
	''' files, they may be exchanged between environments which may have
	''' different versions of some of the classes and between VMs from
	''' different vendors.
	''' <li>
	''' <em>Structurally compact</em>: The <code>XMLEncoder</code> class
	''' uses a <em>redundancy elimination</em> algorithm internally so that the
	''' default values of a Bean's properties are not written to the stream.
	''' <li>
	''' <em>Fault tolerant</em>: Non-structural errors in the file,
	''' caused either by damage to the file or by API changes
	''' made to classes in an archive remain localized
	''' so that a reader can report the error and continue to load the parts
	''' of the document which were not affected by the error.
	''' </ul>
	''' <p>
	''' Below is an example of an XML archive containing
	''' some user interface components from the <em>swing</em> toolkit:
	''' <pre>
	''' &lt;?xml version="1.0" encoding="UTF-8"?&gt;
	''' &lt;java version="1.0" class="java.beans.XMLDecoder"&gt;
	''' &lt;object class="javax.swing.JFrame"&gt;
	'''   &lt;void property="name"&gt;
	'''     &lt;string&gt;frame1&lt;/string&gt;
	'''   &lt;/void&gt;
	'''   &lt;void property="bounds"&gt;
	'''     &lt;object class="java.awt.Rectangle"&gt;
	'''       &lt;int&gt;0&lt;/int&gt;
	'''       &lt;int&gt;0&lt;/int&gt;
	'''       &lt;int&gt;200&lt;/int&gt;
	'''       &lt;int&gt;200&lt;/int&gt;
	'''     &lt;/object&gt;
	'''   &lt;/void&gt;
	'''   &lt;void property="contentPane"&gt;
	'''     &lt;void method="add"&gt;
	'''       &lt;object class="javax.swing.JButton"&gt;
	'''         &lt;void property="label"&gt;
	'''           &lt;string&gt;Hello&lt;/string&gt;
	'''         &lt;/void&gt;
	'''       &lt;/object&gt;
	'''     &lt;/void&gt;
	'''   &lt;/void&gt;
	'''   &lt;void property="visible"&gt;
	'''     &lt;boolean&gt;true&lt;/boolean&gt;
	'''   &lt;/void&gt;
	''' &lt;/object&gt;
	''' &lt;/java&gt;
	''' </pre>
	''' The XML syntax uses the following conventions:
	''' <ul>
	''' <li>
	''' Each element represents a method call.
	''' <li>
	''' The "object" tag denotes an <em>expression</em> whose value is
	''' to be used as the argument to the enclosing element.
	''' <li>
	''' The "void" tag denotes a <em>statement</em> which will
	''' be executed, but whose result will not be used as an
	''' argument to the enclosing method.
	''' <li>
	''' Elements which contain elements use those elements as arguments,
	''' unless they have the tag: "void".
	''' <li>
	''' The name of the method is denoted by the "method" attribute.
	''' <li>
	''' XML's standard "id" and "idref" attributes are used to make
	''' references to previous expressions - so as to deal with
	''' circularities in the object graph.
	''' <li>
	''' The "class" attribute is used to specify the target of a static
	''' method or constructor explicitly; its value being the fully
	''' qualified name of the class.
	''' <li>
	''' Elements with the "void" tag are executed using
	''' the outer context as the target if no target is defined
	''' by a "class" attribute.
	''' <li>
	''' Java's String class is treated specially and is
	''' written &lt;string&gt;Hello, world&lt;/string&gt; where
	''' the characters of the string are converted to bytes
	''' using the UTF-8 character encoding.
	''' </ul>
	''' <p>
	''' Although all object graphs may be written using just these three
	''' tags, the following definitions are included so that common
	''' data structures can be expressed more concisely:
	''' <p>
	''' <ul>
	''' <li>
	''' The default method name is "new".
	''' <li>
	''' A reference to a java class is written in the form
	'''  &lt;class&gt;javax.swing.JButton&lt;/class&gt;.
	''' <li>
	''' Instances of the wrapper classes for Java's primitive types are written
	''' using the name of the primitive type as the tag. For example, an
	''' instance of the <code>Integer</code> class could be written:
	''' &lt;int&gt;123&lt;/int&gt;. Note that the <code>XMLEncoder</code> class
	''' uses Java's reflection package in which the conversion between
	''' Java's primitive types and their associated "wrapper classes"
	''' is handled internally. The API for the <code>XMLEncoder</code> class
	''' itself deals only with <code>Object</code>s.
	''' <li>
	''' In an element representing a nullary method whose name
	''' starts with "get", the "method" attribute is replaced
	''' with a "property" attribute whose value is given by removing
	''' the "get" prefix and decapitalizing the result.
	''' <li>
	''' In an element representing a monadic method whose name
	''' starts with "set", the "method" attribute is replaced
	''' with a "property" attribute whose value is given by removing
	''' the "set" prefix and decapitalizing the result.
	''' <li>
	''' In an element representing a method named "get" taking one
	''' integer argument, the "method" attribute is replaced
	''' with an "index" attribute whose value the value of the
	''' first argument.
	''' <li>
	''' In an element representing a method named "set" taking two arguments,
	''' the first of which is an integer, the "method" attribute is replaced
	''' with an "index" attribute whose value the value of the
	''' first argument.
	''' <li>
	''' A reference to an array is written using the "array"
	''' tag. The "class" and "length" attributes specify the
	''' sub-type of the array and its length respectively.
	''' </ul>
	''' 
	''' <p>
	''' For more information you might also want to check out
	''' <a
	''' href="http://java.sun.com/products/jfc/tsc/articles/persistence4">Using XMLEncoder</a>,
	''' an article in <em>The Swing Connection.</em> </summary>
	''' <seealso cref= XMLDecoder </seealso>
	''' <seealso cref= java.io.ObjectOutputStream
	''' 
	''' @since 1.4
	''' 
	''' @author Philip Milne </seealso>
	Public Class XMLEncoder
		Inherits Encoder
		Implements AutoCloseable

		Private ReadOnly encoder As java.nio.charset.CharsetEncoder
		Private ReadOnly charset As String
		Private ReadOnly declaration As Boolean

		Private out As OutputStreamWriter
		Private owner As Object
		Private indentation As Integer = 0
		Private internal As Boolean = False
		Private valueToExpression As Map(Of Object, ValueData)
		Private targetToStatementList As Map(Of Object, List(Of Statement))
		Private preambleWritten As Boolean = False
		Private nameGenerator_Renamed As NameGenerator

		Private Class ValueData
			Private ReadOnly outerInstance As XMLEncoder

			Public Sub New(ByVal outerInstance As XMLEncoder)
				Me.outerInstance = outerInstance
			End Sub

			Public refs As Integer = 0
			Public marked As Boolean = False ' Marked -> refs > 0 unless ref was a target.
			Public name As String = Nothing
			Public exp As Expression = Nothing
		End Class

		''' <summary>
		''' Creates a new XML encoder to write out <em>JavaBeans</em>
		''' to the stream <code>out</code> using an XML encoding.
		''' </summary>
		''' <param name="out">  the stream to which the XML representation of
		'''             the objects will be written
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''          if <code>out</code> is <code>null</code>
		''' </exception>
		''' <seealso cref= XMLDecoder#XMLDecoder(InputStream) </seealso>
		Public Sub New(ByVal out As OutputStream)
			Me.New(out, "UTF-8", True, 0)
		End Sub

		''' <summary>
		''' Creates a new XML encoder to write out <em>JavaBeans</em>
		''' to the stream <code>out</code> using the given <code>charset</code>
		''' starting from the given <code>indentation</code>.
		''' </summary>
		''' <param name="out">          the stream to which the XML representation of
		'''                     the objects will be written </param>
		''' <param name="charset">      the name of the requested charset;
		'''                     may be either a canonical name or an alias </param>
		''' <param name="declaration">  whether the XML declaration should be generated;
		'''                     set this to <code>false</code>
		'''                     when embedding the contents in another XML document </param>
		''' <param name="indentation">  the number of space characters to indent the entire XML document by
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''          if <code>out</code> or <code>charset</code> is <code>null</code>,
		'''          or if <code>indentation</code> is less than 0
		''' </exception>
		''' <exception cref="IllegalCharsetNameException">
		'''          if <code>charset</code> name is illegal
		''' </exception>
		''' <exception cref="UnsupportedCharsetException">
		'''          if no support for the named charset is available
		'''          in this instance of the Java virtual machine
		''' </exception>
		''' <exception cref="UnsupportedOperationException">
		'''          if loaded charset does not support encoding
		''' </exception>
		''' <seealso cref= Charset#forName(String)
		''' 
		''' @since 1.7 </seealso>
		Public Sub New(ByVal out As OutputStream, ByVal charset As String, ByVal declaration As Boolean, ByVal indentation As Integer)
			If out Is Nothing Then Throw New IllegalArgumentException("the output stream cannot be null")
			If indentation < 0 Then Throw New IllegalArgumentException("the indentation must be >= 0")
			Dim cs As java.nio.charset.Charset = java.nio.charset.Charset.forName(charset)
			Me.encoder = cs.newEncoder()
			Me.charset = charset
			Me.declaration = declaration
			Me.indentation = indentation
			Me.out = New OutputStreamWriter(out, cs.newEncoder())
			valueToExpression = New IdentityHashMap(Of )
			targetToStatementList = New IdentityHashMap(Of )
			nameGenerator_Renamed = New NameGenerator
		End Sub

		''' <summary>
		''' Sets the owner of this encoder to <code>owner</code>.
		''' </summary>
		''' <param name="owner"> The owner of this encoder.
		''' </param>
		''' <seealso cref= #getOwner </seealso>
		Public Overridable Property owner As Object
			Set(ByVal owner As Object)
				Me.owner = owner
				writeExpression(New Expression(Me, "getOwner", New Object(){}))
			End Set
			Get
				Return owner
			End Get
		End Property


		''' <summary>
		''' Write an XML representation of the specified object to the output.
		''' </summary>
		''' <param name="o"> The object to be written to the stream.
		''' </param>
		''' <seealso cref= XMLDecoder#readObject </seealso>
		Public Overrides Sub writeObject(ByVal o As Object)
			If internal Then
				MyBase.writeObject(o)
			Else
				writeStatement(New Statement(Me, "writeObject", New Object(){o}))
			End If
		End Sub

		Private Function statementList(ByVal target As Object) As List(Of Statement)
			Dim list As List(Of Statement) = targetToStatementList.get(target)
			If list Is Nothing Then
				list = New List(Of )
				targetToStatementList.put(target, list)
			End If
			Return list
		End Function


		Private Sub mark(ByVal o As Object, ByVal isArgument As Boolean)
			If o Is Nothing OrElse o Is Me Then Return
			Dim d As ValueData = getValueData(o)
			Dim exp As Expression = d.exp
			' Do not mark liternal strings. Other strings, which might,
			' for example, come from resource bundles should still be marked.
			If o.GetType() Is GetType(String) AndAlso exp Is Nothing Then Return

			' Bump the reference counts of all arguments
			If isArgument Then d.refs += 1
			If d.marked Then Return
			d.marked = True
			Dim target As Object = exp.target
			mark(exp)
			If Not(TypeOf target Is Class) Then
				statementList(target).add(exp)
				' Pending: Why does the reference count need to
				' be incremented here?
				d.refs += 1
			End If
		End Sub

		Private Sub mark(ByVal stm As Statement)
			Dim args As Object() = stm.arguments
			For i As Integer = 0 To args.Length - 1
				Dim arg As Object = args(i)
				mark(arg, True)
			Next i
			mark(stm.target, TypeOf stm Is Expression)
		End Sub


		''' <summary>
		''' Records the Statement so that the Encoder will
		''' produce the actual output when the stream is flushed.
		''' <P>
		''' This method should only be invoked within the context
		''' of initializing a persistence delegate.
		''' </summary>
		''' <param name="oldStm"> The statement that will be written
		'''               to the stream. </param>
		''' <seealso cref= java.beans.PersistenceDelegate#initialize </seealso>
		Public Overrides Sub writeStatement(ByVal oldStm As Statement)
			' System.out.println("XMLEncoder::writeStatement: " + oldStm);
			Dim internal As Boolean = Me.internal
			Me.internal = True
			Try
				MyBase.writeStatement(oldStm)
	'            
	'               Note we must do the mark first as we may
	'               require the results of previous values in
	'               this context for this statement.
	'               Test case is:
	'                   os.setOwner(this);
	'                   os.writeObject(this);
	'            
				mark(oldStm)
				Dim target As Object = oldStm.target
				If TypeOf target Is Field Then
					Dim method As String = oldStm.methodName
					Dim args As Object() = oldStm.arguments
					If (method Is Nothing) OrElse (args Is Nothing) Then
					ElseIf method.Equals("get") AndAlso (args.Length = 1) Then
						target = args(0)
					ElseIf method.Equals("set") AndAlso (args.Length = 2) Then
						target = args(0)
					End If
				End If
				statementList(target).add(oldStm)
			Catch e As Exception
				exceptionListener.exceptionThrown(New Exception("XMLEncoder: discarding statement " & oldStm, e))
			End Try
			Me.internal = internal
		End Sub


		''' <summary>
		''' Records the Expression so that the Encoder will
		''' produce the actual output when the stream is flushed.
		''' <P>
		''' This method should only be invoked within the context of
		''' initializing a persistence delegate or setting up an encoder to
		''' read from a resource bundle.
		''' <P>
		''' For more information about using resource bundles with the
		''' XMLEncoder, see
		''' http://java.sun.com/products/jfc/tsc/articles/persistence4/#i18n
		''' </summary>
		''' <param name="oldExp"> The expression that will be written
		'''               to the stream. </param>
		''' <seealso cref= java.beans.PersistenceDelegate#initialize </seealso>
		Public Overrides Sub writeExpression(ByVal oldExp As Expression)
			Dim internal As Boolean = Me.internal
			Me.internal = True
			Dim oldValue As Object = getValue(oldExp)
			If [get](oldValue) Is Nothing OrElse (TypeOf oldValue Is String AndAlso (Not internal)) Then
				getValueData(oldValue).exp = oldExp
				MyBase.writeExpression(oldExp)
			End If
			Me.internal = internal
		End Sub

		''' <summary>
		''' This method writes out the preamble associated with the
		''' XML encoding if it has not been written already and
		''' then writes out all of the values that been
		''' written to the stream since the last time <code>flush</code>
		''' was called. After flushing, all internal references to the
		''' values that were written to this stream are cleared.
		''' </summary>
		Public Overridable Sub flush()
			If Not preambleWritten Then ' Don't do this in constructor - it throws ... pending.
				If Me.declaration Then writeln("<?xml version=" & quote("1.0") & " encoding=" & quote(Me.charset) & "?>")
				writeln("<java version=" & quote(System.getProperty("java.version")) & " class=" & quote(GetType(XMLDecoder).name) & ">")
				preambleWritten = True
			End If
			indentation += 1
			Dim statements As List(Of Statement) = statementList(Me)
			Do While Not statements.empty
				Dim s As Statement = statements.remove(0)
				If "writeObject".Equals(s.methodName) Then
					outputValue(s.arguments(0), Me, True)
				Else
					outputStatement(s, Me, False)
				End If
			Loop
			indentation -= 1

			Dim statement_Renamed As Statement = missedStatement
			Do While statement_Renamed IsNot Nothing
				outputStatement(statement_Renamed, Me, False)
				statement_Renamed = missedStatement
			Loop

			Try
				out.flush()
			Catch e As IOException
				exceptionListener.exceptionThrown(e)
			End Try
			clear()
		End Sub

		Friend Overrides Sub clear()
			MyBase.clear()
			nameGenerator_Renamed.clear()
			valueToExpression.clear()
			targetToStatementList.clear()
		End Sub

		Friend Overridable Property missedStatement As Statement
			Get
				For Each statements As List(Of Statement) In Me.targetToStatementList.values()
					For i As Integer = 0 To statements.size() - 1
						If GetType(Statement) Is statements.get(i).GetType() Then Return statements.remove(i)
					Next i
				Next statements
				Return Nothing
			End Get
		End Property


		''' <summary>
		''' This method calls <code>flush</code>, writes the closing
		''' postamble and then closes the output stream associated
		''' with this stream.
		''' </summary>
		Public Overridable Sub close() Implements AutoCloseable.close
			flush()
			writeln("</java>")
			Try
				out.close()
			Catch e As IOException
				exceptionListener.exceptionThrown(e)
			End Try
		End Sub

		Private Function quote(ByVal s As String) As String
			Return """" & s & """"
		End Function

		Private Function getValueData(ByVal o As Object) As ValueData
			Dim d As ValueData = valueToExpression.get(o)
			If d Is Nothing Then
				d = New ValueData(Me)
				valueToExpression.put(o, d)
			End If
			Return d
		End Function

		''' <summary>
		''' Returns <code>true</code> if the argument,
		''' a Unicode code point, is valid in XML documents.
		''' Unicode characters fit into the low sixteen bits of a Unicode code point,
		''' and pairs of Unicode <em>surrogate characters</em> can be combined
		''' to encode Unicode code point in documents containing only Unicode.
		''' (The <code>char</code> datatype in the Java Programming Language
		''' represents Unicode characters, including unpaired surrogates.)
		''' <par>
		''' [2] Char ::= #x0009 | #x000A | #x000D
		'''            | [#x0020-#xD7FF]
		'''            | [#xE000-#xFFFD]
		'''            | [#x10000-#x10ffff]
		''' </par>
		''' </summary>
		''' <param name="code">  the 32-bit Unicode code point being tested </param>
		''' <returns>  <code>true</code> if the Unicode code point is valid,
		'''          <code>false</code> otherwise </returns>
		Private Shared Function isValidCharCode(ByVal code As Integer) As Boolean
			Return (&H20 <= code AndAlso code <= &HD7FF) OrElse (&HA = code) OrElse (&H9 = code) OrElse (&HD = code) OrElse (&HE000 <= code AndAlso code <= &HFFFD) OrElse (&H10000 <= code AndAlso code <= &H10ffff)
		End Function

		Private Sub writeln(ByVal exp As String)
			Try
				Dim sb As New StringBuilder
				For i As Integer = 0 To indentation - 1
					sb.append(" "c)
				Next i
				sb.append(exp)
				sb.append(ControlChars.Lf)
				Me.out.write(sb.ToString())
			Catch e As IOException
				exceptionListener.exceptionThrown(e)
			End Try
		End Sub

		Private Sub outputValue(ByVal value As Object, ByVal outer As Object, ByVal isArgument As Boolean)
			If value Is Nothing Then
				writeln("<null/>")
				Return
			End If

			If TypeOf value Is Class Then
				writeln("<class>" & CType(value, [Class]).name & "</class>")
				Return
			End If

			Dim d As ValueData = getValueData(value)
			If d.exp IsNot Nothing Then
				Dim target As Object = d.exp.target
				Dim methodName As String = d.exp.methodName

				If target Is Nothing OrElse methodName Is Nothing Then Throw New NullPointerException((If(target Is Nothing, "target", "methodName")) & " should not be null")

				If isArgument AndAlso TypeOf target Is Field AndAlso methodName.Equals("get") Then
					Dim f As Field = CType(target, Field)
					writeln("<object class=" & quote(f.declaringClass.name) & " field=" & quote(f.name) & "/>")
					Return
				End If

				Dim primitiveType As Class = primitiveTypeFor(value.GetType())
				If primitiveType IsNot Nothing AndAlso target Is value.GetType() AndAlso methodName.Equals("new") Then
					Dim primitiveTypeName As String = primitiveType.name
					' Make sure that character types are quoted correctly.
					If primitiveType Is Character.TYPE Then
						Dim code As Char = CChar(value)
						If Not isValidCharCode(code) Then
							writeln(createString(code))
							Return
						End If
						value = quoteCharCode(code)
						If value Is Nothing Then value = Convert.ToChar(code)
					End If
					writeln("<" & primitiveTypeName & ">" & value & "</" & primitiveTypeName & ">")
					Return
				End If

			ElseIf TypeOf value Is String Then
				writeln(createString(CStr(value)))
				Return
			End If

			If d.name IsNot Nothing Then
				If isArgument Then
					writeln("<object idref=" & quote(d.name) & "/>")
				Else
					outputXML("void", " idref=" & quote(d.name), value)
				End If
			ElseIf d.exp IsNot Nothing Then
				outputStatement(d.exp, outer, isArgument)
			End If
		End Sub

		Private Shared Function quoteCharCode(ByVal code As Integer) As String
			Select Case code
			  Case "&"c
				  Return "&amp;"
			  Case "<"c
				  Return "&lt;"
			  Case ">"c
				  Return "&gt;"
			  Case """"c
				  Return "&quot;"
			  Case "'"c
				  Return "&apos;"
			  Case ControlChars.Cr
				  Return "&#13;"
			  Case Else
				  Return Nothing
			End Select
		End Function

		Private Shared Function createString(ByVal code As Integer) As String
			Return "<char code=""#" & Convert.ToString(code, 16) & """/>"
		End Function

		Private Function createString(ByVal [string] As String) As String
			Dim sb As New StringBuilder
			sb.append("<string>")
			Dim index As Integer = 0
			Do While index < string_Renamed.length()
				Dim point As Integer = string_Renamed.codePointAt(index)
				Dim count As Integer = Character.charCount(point)

				If isValidCharCode(point) AndAlso Me.encoder.canEncode(string_Renamed.Substring(index, count)) Then
					Dim value_Renamed As String = quoteCharCode(point)
					If value_Renamed IsNot Nothing Then
						sb.append(value_Renamed)
					Else
						sb.appendCodePoint(point)
					End If
					index += count
				Else
					sb.append(createString(string_Renamed.Chars(index)))
					index += 1
				End If
			Loop
			sb.append("</string>")
			Return sb.ToString()
		End Function

		Private Sub outputStatement(ByVal exp As Statement, ByVal outer As Object, ByVal isArgument As Boolean)
			Dim target As Object = exp.target
			Dim methodName As String = exp.methodName

			If target Is Nothing OrElse methodName Is Nothing Then Throw New NullPointerException((If(target Is Nothing, "target", "methodName")) & " should not be null")

			Dim args As Object() = exp.arguments
			Dim expression As Boolean = exp.GetType() Is GetType(Expression)
			Dim value_Renamed As Object = If(expression, getValue(CType(exp, Expression)), Nothing)

			Dim tag As String = If(expression AndAlso isArgument, "object", "void")
			Dim attributes As String = ""
			Dim d As ValueData = getValueData(value_Renamed)

			' Special cases for targets.
			If target Is outer Then
			ElseIf target Is GetType(Array) AndAlso methodName.Equals("newInstance") Then
				tag = "array"
				attributes = attributes & " class=" & quote(CType(args(0), [Class]).name)
				attributes = attributes & " length=" & quote(args(1).ToString())
				args = New Object(){}
			ElseIf target.GetType() Is GetType(Class) Then
				attributes = attributes & " class=" & quote(CType(target, [Class]).name)
			Else
				d.refs = 2
				If d.name Is Nothing Then
					getValueData(target).refs += 1
					Dim statements As List(Of Statement) = statementList(target)
					If Not statements.contains(exp) Then statements.add(exp)
					outputValue(target, outer, False)
				End If
				If expression Then outputValue(value_Renamed, outer, isArgument)
				Return
			End If
			If expression AndAlso (d.refs > 1) Then
				Dim instanceName As String = nameGenerator_Renamed.instanceName(value_Renamed)
				d.name = instanceName
				attributes = attributes & " id=" & quote(instanceName)
			End If

			' Special cases for methods.
			If ((Not expression) AndAlso methodName.Equals("set") AndAlso args.Length = 2 AndAlso TypeOf args(0) Is Integer?) OrElse (expression AndAlso methodName.Equals("get") AndAlso args.Length = 1 AndAlso TypeOf args(0) Is Integer?) Then
				attributes = attributes & " index=" & quote(args(0).ToString())
				args = If(args.Length = 1, New Object(){}, New Object){args(1)}
			ElseIf ((Not expression) AndAlso methodName.StartsWith("set") AndAlso args.Length = 1) OrElse (expression AndAlso methodName.StartsWith("get") AndAlso args.Length = 0) Then
				If 3 < methodName.length() Then attributes = attributes & " property=" & quote(Introspector.decapitalize(methodName.Substring(3)))
			ElseIf (Not methodName.Equals("new")) AndAlso (Not methodName.Equals("newInstance")) Then
				attributes = attributes & " method=" & quote(methodName)
			End If
			outputXML(tag, attributes, value_Renamed, args)
		End Sub

		Private Sub outputXML(ByVal tag As String, ByVal attributes As String, ByVal value As Object, ParamArray ByVal args As Object())
			Dim statements As List(Of Statement) = statementList(value)
			' Use XML's short form when there is no body.
			If args.Length = 0 AndAlso statements.size() = 0 Then
				writeln("<" & tag + attributes & "/>")
				Return
			End If

			writeln("<" & tag + attributes & ">")
			indentation += 1

			For i As Integer = 0 To args.Length - 1
				outputValue(args(i), Nothing, True)
			Next i

			Do While Not statements.empty
				Dim s As Statement = statements.remove(0)
				outputStatement(s, value, False)
			Loop

			indentation -= 1
			writeln("</" & tag & ">")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Shared Function primitiveTypeFor(ByVal wrapper As Class) As Class
			If wrapper Is GetType(Boolean) Then Return Boolean.TYPE
			If wrapper Is GetType(Byte) Then Return Byte.TYPE
			If wrapper Is GetType(Character) Then Return Character.TYPE
			If wrapper Is GetType(Short) Then Return Short.TYPE
			If wrapper Is GetType(Integer) Then Return Integer.TYPE
			If wrapper Is GetType(Long) Then Return Long.TYPE
			If wrapper Is GetType(Float) Then Return Float.TYPE
			If wrapper Is GetType(Double) Then Return Double.TYPE
			If wrapper Is GetType(Void) Then Return Void.TYPE
			Return Nothing
		End Function
	End Class

End Namespace