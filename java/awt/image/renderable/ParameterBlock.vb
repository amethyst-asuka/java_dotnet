Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic

'
' * Copyright (c) 1998, 2004, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.image.renderable

    ''' <summary>
    ''' A <code>ParameterBlock</code> encapsulates all the information about sources and
    ''' parameters (Objects) required by a RenderableImageOp, or other
    ''' classes that process images.
    ''' 
    ''' <p> Although it is possible to place arbitrary objects in the
    ''' source Vector, users of this class may impose semantic constraints
    ''' such as requiring all sources to be RenderedImages or
    ''' RenderableImage.  <code>ParameterBlock</code> itself is merely a container and
    ''' performs no checking on source or parameter types.
    ''' 
    ''' <p> All parameters in a <code>ParameterBlock</code> are objects; convenience
    ''' add and set methods are available that take arguments of base type and
    ''' construct the appropriate subclass of Number (such as
    ''' Integer or Float).  Corresponding get methods perform a
    ''' downward cast and have return values of base type; an exception
    ''' will be thrown if the stored values do not have the correct type.
    ''' There is no way to distinguish between the results of
    ''' "short s; add(s)" and "add(new Short(s))".
    ''' 
    ''' <p> Note that the get and set methods operate on references.
    ''' Therefore, one must be careful not to share references between
    ''' <code>ParameterBlock</code>s when this is inappropriate.  For example, to create
    ''' a new <code>ParameterBlock</code> that is equal to an old one except for an
    ''' added source, one might be tempted to write:
    ''' 
    ''' <pre>
    ''' ParameterBlock addSource(ParameterBlock pb, RenderableImage im) {
    '''     ParameterBlock pb1 = new ParameterBlock(pb.getSources());
    '''     pb1.addSource(im);
    '''     return pb1;
    ''' }
    ''' </pre>
    ''' 
    ''' <p> This code will have the side effect of altering the original
    ''' <code>ParameterBlock</code>, since the getSources operation returned a reference
    ''' to its source Vector.  Both pb and pb1 share their source Vector,
    ''' and a change in either is visible to both.
    ''' 
    ''' <p> A correct way to write the addSource function is to clone
    ''' the source Vector:
    ''' 
    ''' <pre>
    ''' ParameterBlock addSource (ParameterBlock pb, RenderableImage im) {
    '''     ParameterBlock pb1 = new ParameterBlock(pb.getSources().clone());
    '''     pb1.addSource(im);
    '''     return pb1;
    ''' }
    ''' </pre>
    ''' 
    ''' <p> The clone method of <code>ParameterBlock</code> has been defined to
    ''' perform a clone of both the source and parameter Vectors for
    ''' this reason.  A standard, shallow clone is available as
    ''' shallowClone.
    ''' 
    ''' <p> The addSource, setSource, add, and set methods are
    ''' defined to return 'this' after adding their argument.  This allows
    ''' use of syntax like:
    ''' 
    ''' <pre>
    ''' ParameterBlock pb = new ParameterBlock();
    ''' op = new RenderableImageOp("operation", pb.add(arg1).add(arg2));
    ''' </pre>
    ''' 
    ''' </summary>
    <Serializable>
    Public Class ParameterBlock : Inherits java.lang.Object
        Implements Cloneable

        ''' <summary>
        ''' A dummy constructor. </summary>
        Public Sub New()
		End Sub

        ''' <summary>
        ''' Constructs a <code>ParameterBlock</code> with a given Vector
        ''' of sources. </summary>
        ''' <param name="sources"> a <code>Vector</code> of source images </param>
        Public Sub New(  sources As ArrayList)
            sources = sources
        End Sub

        ''' <summary>
        ''' Constructs a <code>ParameterBlock</code> with a given Vector of sources and
        ''' Vector of parameters. </summary>
        ''' <param name="sources"> a <code>Vector</code> of source images </param>
        ''' <param name="parameters"> a <code>Vector</code> of parameters to be used in the
        '''        rendering operation </param>
        Public Sub New(  sources As ArrayList,   parameters As ArrayList)
            sources = sources
            parameters = parameters
        End Sub

        ''' <summary>
        ''' Creates a shallow copy of a <code>ParameterBlock</code>.  The source and
        ''' parameter Vectors are copied by reference -- additions or
        ''' changes will be visible to both versions.
        ''' </summary>
        ''' <returns> an Object clone of the <code>ParameterBlock</code>. </returns>
        Public Overridable Function shallowClone() As Object
            Try
                Return MyBase.clone()
            Catch e As Exception
                ' We can't be here since we implement Cloneable.
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Creates a copy of a <code>ParameterBlock</code>.  The source and parameter
        ''' Vectors are cloned, but the actual sources and parameters are
        ''' copied by reference.  This allows modifications to the order
        ''' and number of sources and parameters in the clone to be invisible
        ''' to the original <code>ParameterBlock</code>.  Changes to the shared sources or
        ''' parameters themselves will still be visible.
        ''' </summary>
        ''' <returns> an Object clone of the <code>ParameterBlock</code>. </returns>
        Public Overrides Function clone() As Object
            Dim theClone As ParameterBlock

            Try
                theClone = CType(MyBase.clone(), ParameterBlock)
            Catch e As Exception
                ' We can't be here since we implement Cloneable.
                Return Nothing
            End Try

            If sources IsNot Nothing Then theClone.sources = CType(sources.Clone(), ArrayList)
            If parameters IsNot Nothing Then theClone.parameters = CType(parameters.Clone(), ArrayList)
            Return CObj(theClone)
        End Function

        ''' <summary>
        ''' Adds an image to end of the list of sources.  The image is
        ''' stored as an object in order to allow new node types in the
        ''' future.
        ''' </summary>
        ''' <param name="source"> an image object to be stored in the source list. </param>
        ''' <returns> a new <code>ParameterBlock</code> containing the specified
        '''         <code>source</code>. </returns>
        Public Overridable Function addSource(  source As Object) As ParameterBlock
            sources.Add(source)
            Return Me
        End Function

        ''' <summary>
        ''' Returns a source as a general Object.  The caller must cast it into
        ''' an appropriate type.
        ''' </summary>
        ''' <param name="index"> the index of the source to be returned. </param>
        ''' <returns> an <code>Object</code> that represents the source located
        '''         at the specified index in the <code>sources</code>
        '''         <code>Vector</code>. </returns>
        ''' <seealso cref= #setSource(Object, int) </seealso>
        Public Overridable Function getSource(  index As Integer) As Object
            Return sources(index)
        End Function

        ''' <summary>
        ''' Replaces an entry in the list of source with a new source.
        ''' If the index lies beyond the current source list,
        ''' the list is extended with nulls as needed. </summary>
        ''' <param name="source"> the specified source image </param>
        ''' <param name="index"> the index into the <code>sources</code>
        '''              <code>Vector</code> at which to
        '''              insert the specified <code>source</code> </param>
        ''' <returns> a new <code>ParameterBlock</code> that contains the
        '''         specified <code>source</code> at the specified
        '''         <code>index</code>. </returns>
        ''' <seealso cref= #getSource(int) </seealso>
        Public Overridable Function setSource(  source As Object,   index As Integer) As ParameterBlock
            Dim oldSize As Integer = sources.Count
            Dim newSize As Integer = index + 1
            If oldSize < newSize Then sources.Capacity = newSize
            sources(index) = source
            Return Me
        End Function

        ''' <summary>
        ''' Returns a source as a <code>RenderedImage</code>.  This method is
        ''' a convenience method.
        ''' An exception will be thrown if the source is not a RenderedImage.
        ''' </summary>
        ''' <param name="index"> the index of the source to be returned </param>
        ''' <returns> a <code>RenderedImage</code> that represents the source
        '''         image that is at the specified index in the
        '''         <code>sources</code> <code>Vector</code>. </returns>
        Public Overridable Function getRenderedSource(  index As Integer) As java.awt.image.RenderedImage
            Return CType(sources(index), java.awt.image.RenderedImage)
        End Function

        ''' <summary>
        ''' Returns a source as a RenderableImage.  This method is a
        ''' convenience method.
        ''' An exception will be thrown if the sources is not a RenderableImage.
        ''' </summary>
        ''' <param name="index"> the index of the source to be returned </param>
        ''' <returns> a <code>RenderableImage</code> that represents the source
        '''         image that is at the specified index in the
        '''         <code>sources</code> <code>Vector</code>. </returns>
        Public Overridable Function getRenderableSource(  index As Integer) As RenderableImage
            Return CType(sources(index), RenderableImage)
        End Function

        ''' <summary>
        ''' Returns the number of source images. </summary>
        ''' <returns> the number of source images in the <code>sources</code>
        '''         <code>Vector</code>. </returns>
        Public Overridable Property numSources As Integer
            Get
                Return sources.Count
            End Get
        End Property

        ''' <summary>
        ''' Returns the entire Vector of sources. </summary>
        ''' <returns> the <code>sources</code> <code>Vector</code>. </returns>
        ''' <seealso cref= #setSources(Vector) </seealso>
        Public Overridable Property sources As ArrayList

        ''' <summary>
        ''' Clears the list of source images. </summary>
        Public Overridable Sub removeSources()
            sources = New ArrayList
        End Sub

        ''' <summary>
        ''' Returns the number of parameters (not including source images). </summary>
        ''' <returns> the number of parameters in the <code>parameters</code>
        '''         <code>Vector</code>. </returns>
        Public Overridable Property numParameters As Integer
            Get
                Return parameters.Count
            End Get
        End Property

        ''' <summary>
        ''' Returns the entire Vector of parameters. </summary>
        ''' <returns> the <code>parameters</code> <code>Vector</code>. </returns>
        ''' <seealso cref= #setParameters(Vector) </seealso>
        Public Overridable Property parameters As ArrayList

        ''' <summary>
        ''' Clears the list of parameters. </summary>
        Public Overridable Sub removeParameters()
			parameters = New ArrayList
		End Sub

		''' <summary>
		''' Adds an object to the list of parameters. </summary>
		''' <param name="obj"> the <code>Object</code> to add to the
		'''            <code>parameters</code> <code>Vector</code> </param>
		''' <returns> a new <code>ParameterBlock</code> containing
		'''         the specified parameter. </returns>
		Public Overridable Function add(  obj As Object) As ParameterBlock
			parameters.Add(obj)
			Return Me
		End Function

		''' <summary>
		''' Adds a Byte to the list of parameters. </summary>
		''' <param name="b"> the byte to add to the
		'''            <code>parameters</code> <code>Vector</code> </param>
		''' <returns> a new <code>ParameterBlock</code> containing
		'''         the specified parameter. </returns>
		Public Overridable Function add(  b As SByte) As ParameterBlock
			Return add(New Byte(b))
		End Function

		''' <summary>
		''' Adds a Character to the list of parameters. </summary>
		''' <param name="c"> the char to add to the
		'''            <code>parameters</code> <code>Vector</code> </param>
		''' <returns> a new <code>ParameterBlock</code> containing
		'''         the specified parameter. </returns>
		Public Overridable Function add(  c As Char) As ParameterBlock
			Return add(New Character(c))
		End Function

		''' <summary>
		''' Adds a Short to the list of parameters. </summary>
		''' <param name="s"> the short to add to the
		'''            <code>parameters</code> <code>Vector</code> </param>
		''' <returns> a new <code>ParameterBlock</code> containing
		'''         the specified parameter. </returns>
		Public Overridable Function add(  s As Short) As ParameterBlock
			Return add(New Short?(s))
		End Function

		''' <summary>
		''' Adds a Integer to the list of parameters. </summary>
		''' <param name="i"> the int to add to the
		'''            <code>parameters</code> <code>Vector</code> </param>
		''' <returns> a new <code>ParameterBlock</code> containing
		'''         the specified parameter. </returns>
		Public Overridable Function add(  i As Integer) As ParameterBlock
			Return add(New Integer?(i))
		End Function

		''' <summary>
		''' Adds a Long to the list of parameters. </summary>
		''' <param name="l"> the long to add to the
		'''            <code>parameters</code> <code>Vector</code> </param>
		''' <returns> a new <code>ParameterBlock</code> containing
		'''         the specified parameter. </returns>
		Public Overridable Function add(  l As Long) As ParameterBlock
			Return add(New Long?(l))
		End Function

		''' <summary>
		''' Adds a Float to the list of parameters. </summary>
		''' <param name="f"> the float to add to the
		'''            <code>parameters</code> <code>Vector</code> </param>
		''' <returns> a new <code>ParameterBlock</code> containing
		'''         the specified parameter. </returns>
		Public Overridable Function add(  f As Single) As ParameterBlock
			Return add(New Float(f))
		End Function

		''' <summary>
		''' Adds a Double to the list of parameters. </summary>
		''' <param name="d"> the double to add to the
		'''            <code>parameters</code> <code>Vector</code> </param>
		''' <returns> a new <code>ParameterBlock</code> containing
		'''         the specified parameter. </returns>
		Public Overridable Function add(  d As Double) As ParameterBlock
			Return add(New Double?(d))
		End Function

		''' <summary>
		''' Replaces an Object in the list of parameters.
		''' If the index lies beyond the current source list,
		''' the list is extended with nulls as needed. </summary>
		''' <param name="obj"> the parameter that replaces the
		'''        parameter at the specified index in the
		'''        <code>parameters</code> <code>Vector</code> </param>
		''' <param name="index"> the index of the parameter to be
		'''        replaced with the specified parameter </param>
		''' <returns> a new <code>ParameterBlock</code> containing
		'''        the specified parameter. </returns>
		Public Overridable Function [set](  obj As Object,   index As Integer) As ParameterBlock
			Dim oldSize As Integer = parameters.Count
			Dim newSize As Integer = index + 1
			If oldSize < newSize Then parameters.Capacity = newSize
			parameters(index) = obj
			Return Me
		End Function

		''' <summary>
		''' Replaces an Object in the list of parameters with a java.lang.[Byte].
		''' If the index lies beyond the current source list,
		''' the list is extended with nulls as needed. </summary>
		''' <param name="b"> the parameter that replaces the
		'''        parameter at the specified index in the
		'''        <code>parameters</code> <code>Vector</code> </param>
		''' <param name="index"> the index of the parameter to be
		'''        replaced with the specified parameter </param>
		''' <returns> a new <code>ParameterBlock</code> containing
		'''        the specified parameter. </returns>
		Public Overridable Function [set](  b As SByte,   index As Integer) As ParameterBlock
			Return [set](New Byte(b), index)
		End Function

		''' <summary>
		''' Replaces an Object in the list of parameters with a Character.
		''' If the index lies beyond the current source list,
		''' the list is extended with nulls as needed. </summary>
		''' <param name="c"> the parameter that replaces the
		'''        parameter at the specified index in the
		'''        <code>parameters</code> <code>Vector</code> </param>
		''' <param name="index"> the index of the parameter to be
		'''        replaced with the specified parameter </param>
		''' <returns> a new <code>ParameterBlock</code> containing
		'''        the specified parameter. </returns>
		Public Overridable Function [set](  c As Char,   index As Integer) As ParameterBlock
			Return [set](New Character(c), index)
		End Function

		''' <summary>
		''' Replaces an Object in the list of parameters with a  java.lang.[Short].
		''' If the index lies beyond the current source list,
		''' the list is extended with nulls as needed. </summary>
		''' <param name="s"> the parameter that replaces the
		'''        parameter at the specified index in the
		'''        <code>parameters</code> <code>Vector</code> </param>
		''' <param name="index"> the index of the parameter to be
		'''        replaced with the specified parameter </param>
		''' <returns> a new <code>ParameterBlock</code> containing
		'''        the specified parameter. </returns>
		Public Overridable Function [set](  s As Short,   index As Integer) As ParameterBlock
			Return [set](New Short?(s), index)
		End Function

		''' <summary>
		''' Replaces an Object in the list of parameters with an  java.lang.[Integer].
		''' If the index lies beyond the current source list,
		''' the list is extended with nulls as needed. </summary>
		''' <param name="i"> the parameter that replaces the
		'''        parameter at the specified index in the
		'''        <code>parameters</code> <code>Vector</code> </param>
		''' <param name="index"> the index of the parameter to be
		'''        replaced with the specified parameter </param>
		''' <returns> a new <code>ParameterBlock</code> containing
		'''        the specified parameter. </returns>
		Public Overridable Function [set](  i As Integer,   index As Integer) As ParameterBlock
			Return [set](New Integer?(i), index)
		End Function

		''' <summary>
		''' Replaces an Object in the list of parameters with a java.lang.[Long].
		''' If the index lies beyond the current source list,
		''' the list is extended with nulls as needed. </summary>
		''' <param name="l"> the parameter that replaces the
		'''        parameter at the specified index in the
		'''        <code>parameters</code> <code>Vector</code> </param>
		''' <param name="index"> the index of the parameter to be
		'''        replaced with the specified parameter </param>
		''' <returns> a new <code>ParameterBlock</code> containing
		'''        the specified parameter. </returns>
		Public Overridable Function [set](  l As Long,   index As Integer) As ParameterBlock
			Return [set](New Long?(l), index)
		End Function

		''' <summary>
		''' Replaces an Object in the list of parameters with a Float.
		''' If the index lies beyond the current source list,
		''' the list is extended with nulls as needed. </summary>
		''' <param name="f"> the parameter that replaces the
		'''        parameter at the specified index in the
		'''        <code>parameters</code> <code>Vector</code> </param>
		''' <param name="index"> the index of the parameter to be
		'''        replaced with the specified parameter </param>
		''' <returns> a new <code>ParameterBlock</code> containing
		'''        the specified parameter. </returns>
		Public Overridable Function [set](  f As Single,   index As Integer) As ParameterBlock
			Return [set](New Float(f), index)
		End Function

		''' <summary>
		''' Replaces an Object in the list of parameters with a java.lang.[Double].
		''' If the index lies beyond the current source list,
		''' the list is extended with nulls as needed. </summary>
		''' <param name="d"> the parameter that replaces the
		'''        parameter at the specified index in the
		'''        <code>parameters</code> <code>Vector</code> </param>
		''' <param name="index"> the index of the parameter to be
		'''        replaced with the specified parameter </param>
		''' <returns> a new <code>ParameterBlock</code> containing
		'''        the specified parameter. </returns>
		Public Overridable Function [set](  d As Double,   index As Integer) As ParameterBlock
			Return [set](New Double?(d), index)
		End Function

		''' <summary>
		''' Gets a parameter as an object. </summary>
		''' <param name="index"> the index of the parameter to get </param>
		''' <returns> an <code>Object</code> representing the
		'''         the parameter at the specified index
		'''         into the <code>parameters</code>
		'''         <code>Vector</code>. </returns>
		Public Overridable Function getObjectParameter(  index As Integer) As Object
			Return parameters(index)
		End Function

		''' <summary>
		''' A convenience method to return a parameter as a java.lang.[Byte].  An
		''' exception is thrown if the parameter is
		''' <code>null</code> or not a <code>Byte</code>.
		''' </summary>
		''' <param name="index"> the index of the parameter to be returned. </param>
		''' <returns> the parameter at the specified index
		'''         as a <code>byte</code> value. </returns>
		''' <exception cref="ClassCastException"> if the parameter at the
		'''         specified index is not a <code>Byte</code> </exception>
		''' <exception cref="NullPointerException"> if the parameter at the specified
		'''         index is <code>null</code> </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <code>index</code>
		'''         is negative or not less than the current size of this
		'''         <code>ParameterBlock</code> object </exception>
		Public Overridable Function getByteParameter(  index As Integer) As SByte
			Return CByte(parameters(index))
		End Function

		''' <summary>
		''' A convenience method to return a parameter as a char.  An
		''' exception is thrown if the parameter is
		''' <code>null</code> or not a <code>Character</code>.
		''' </summary>
		''' <param name="index"> the index of the parameter to be returned. </param>
		''' <returns> the parameter at the specified index
		'''         as a <code>char</code> value. </returns>
		''' <exception cref="ClassCastException"> if the parameter at the
		'''         specified index is not a <code>Character</code> </exception>
		''' <exception cref="NullPointerException"> if the parameter at the specified
		'''         index is <code>null</code> </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <code>index</code>
		'''         is negative or not less than the current size of this
		'''         <code>ParameterBlock</code> object </exception>
		Public Overridable Function getCharParameter(  index As Integer) As Char
			Return CChar(parameters(index))
		End Function

		''' <summary>
		''' A convenience method to return a parameter as a  java.lang.[Short].  An
		''' exception is thrown if the parameter is
		''' <code>null</code> or not a <code>Short</code>.
		''' </summary>
		''' <param name="index"> the index of the parameter to be returned. </param>
		''' <returns> the parameter at the specified index
		'''         as a <code>short</code> value. </returns>
		''' <exception cref="ClassCastException"> if the parameter at the
		'''         specified index is not a <code>Short</code> </exception>
		''' <exception cref="NullPointerException"> if the parameter at the specified
		'''         index is <code>null</code> </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <code>index</code>
		'''         is negative or not less than the current size of this
		'''         <code>ParameterBlock</code> object </exception>
		Public Overridable Function getShortParameter(  index As Integer) As Short
			Return CShort(Fix(parameters(index)))
		End Function

		''' <summary>
		''' A convenience method to return a parameter as an int.  An
		''' exception is thrown if the parameter is
		''' <code>null</code> or not an <code>Integer</code>.
		''' </summary>
		''' <param name="index"> the index of the parameter to be returned. </param>
		''' <returns> the parameter at the specified index
		'''         as a <code>int</code> value. </returns>
		''' <exception cref="ClassCastException"> if the parameter at the
		'''         specified index is not a <code>Integer</code> </exception>
		''' <exception cref="NullPointerException"> if the parameter at the specified
		'''         index is <code>null</code> </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <code>index</code>
		'''         is negative or not less than the current size of this
		'''         <code>ParameterBlock</code> object </exception>
		Public Overridable Function getIntParameter(  index As Integer) As Integer
			Return CInt(Fix(parameters(index)))
		End Function

		''' <summary>
		''' A convenience method to return a parameter as a java.lang.[Long].  An
		''' exception is thrown if the parameter is
		''' <code>null</code> or not a <code>Long</code>.
		''' </summary>
		''' <param name="index"> the index of the parameter to be returned. </param>
		''' <returns> the parameter at the specified index
		'''         as a <code>long</code> value. </returns>
		''' <exception cref="ClassCastException"> if the parameter at the
		'''         specified index is not a <code>Long</code> </exception>
		''' <exception cref="NullPointerException"> if the parameter at the specified
		'''         index is <code>null</code> </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <code>index</code>
		'''         is negative or not less than the current size of this
		'''         <code>ParameterBlock</code> object </exception>
		Public Overridable Function getLongParameter(  index As Integer) As Long
			Return CLng(Fix(parameters(index)))
		End Function

		''' <summary>
		''' A convenience method to return a parameter as a float.  An
		''' exception is thrown if the parameter is
		''' <code>null</code> or not a <code>Float</code>.
		''' </summary>
		''' <param name="index"> the index of the parameter to be returned. </param>
		''' <returns> the parameter at the specified index
		'''         as a <code>float</code> value. </returns>
		''' <exception cref="ClassCastException"> if the parameter at the
		'''         specified index is not a <code>Float</code> </exception>
		''' <exception cref="NullPointerException"> if the parameter at the specified
		'''         index is <code>null</code> </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <code>index</code>
		'''         is negative or not less than the current size of this
		'''         <code>ParameterBlock</code> object </exception>
		Public Overridable Function getFloatParameter(  index As Integer) As Single
			Return CSng(parameters(index))
		End Function

		''' <summary>
		''' A convenience method to return a parameter as a java.lang.[Double].  An
		''' exception is thrown if the parameter is
		''' <code>null</code> or not a <code>Double</code>.
		''' </summary>
		''' <param name="index"> the index of the parameter to be returned. </param>
		''' <returns> the parameter at the specified index
		'''         as a <code>double</code> value. </returns>
		''' <exception cref="ClassCastException"> if the parameter at the
		'''         specified index is not a <code>Double</code> </exception>
		''' <exception cref="NullPointerException"> if the parameter at the specified
		'''         index is <code>null</code> </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <code>index</code>
		'''         is negative or not less than the current size of this
		'''         <code>ParameterBlock</code> object </exception>
		Public Overridable Function getDoubleParameter(  index As Integer) As Double
			Return CDbl(parameters(index))
		End Function

        ''' <summary>
        ''' Returns an array of Class objects describing the types
        ''' of the parameters. </summary>
        ''' <returns> an array of <code>Class</code> objects. </returns>
        Public Overridable ReadOnly Property paramClasses As [Class]()
            Get
                Dim numParams As Integer = numParameters
                Dim classes As [Class]() = New [Class](numParams - 1) {}
                Dim i As Integer

                For i = 0 To numParams - 1
                    Dim obj As Object = getObjectParameter(i)
                    If TypeOf obj Is Byte Then
                        classes(i) = GetType(SByte)
                    ElseIf TypeOf obj Is Character Then
                        classes(i) = GetType(Char)
                    ElseIf TypeOf obj Is Short? Then
                        classes(i) = GetType(Short)
                    ElseIf TypeOf obj Is Integer? Then
                        classes(i) = GetType(Integer)
                    ElseIf TypeOf obj Is Long? Then
                        classes(i) = GetType(Long)
                    ElseIf TypeOf obj Is Float Then
                        classes(i) = GetType(Single)
                    ElseIf TypeOf obj Is Double? Then
                        classes(i) = GetType(Double)
                    Else
                        classes(i) = obj.GetType()
                    End If
                Next i

                Return classes
            End Get
        End Property
    End Class

End Namespace