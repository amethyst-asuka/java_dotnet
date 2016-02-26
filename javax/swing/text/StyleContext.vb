Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections
Imports System.Collections.Generic

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
	''' A pool of styles and their associated resources.  This class determines
	''' the lifetime of a group of resources by being a container that holds
	''' caches for various resources such as font and color that get reused
	''' by the various style definitions.  This can be shared by multiple
	''' documents if desired to maximize the sharing of related resources.
	''' <p>
	''' This class also provides efficient support for small sets of attributes
	''' and compresses them by sharing across uses and taking advantage of
	''' their immutable nature.  Since many styles are replicated, the potential
	''' for sharing is significant, and copies can be extremely cheap.
	''' Larger sets reduce the possibility of sharing, and therefore revert
	''' automatically to a less space-efficient implementation.
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
	''' @author  Timothy Prinzing
	''' </summary>
	<Serializable> _
	Public Class StyleContext
		Implements AbstractDocument.AttributeContext

		''' <summary>
		''' Returns default AttributeContext shared by all documents that
		''' don't bother to define/supply their own context.
		''' </summary>
		''' <returns> the context </returns>
		Public Property Shared defaultStyleContext As StyleContext
			Get
				If defaultContext Is Nothing Then defaultContext = New StyleContext
				Return defaultContext
			End Get
		End Property

		Private Shared defaultContext As StyleContext

		''' <summary>
		''' Creates a new StyleContext object.
		''' </summary>
		Public Sub New()
			styles = New NamedStyle(Me, Nothing)
			addStyle(DEFAULT_STYLE, Nothing)
		End Sub

		''' <summary>
		''' Adds a new style into the style hierarchy.  Style attributes
		''' resolve from bottom up so an attribute specified in a child
		''' will override an attribute specified in the parent.
		''' </summary>
		''' <param name="nm">   the name of the style (must be unique within the
		'''   collection of named styles in the document).  The name may
		'''   be null if the style is unnamed, but the caller is responsible
		'''   for managing the reference returned as an unnamed style can't
		'''   be fetched by name.  An unnamed style may be useful for things
		'''   like character attribute overrides such as found in a style
		'''   run. </param>
		''' <param name="parent"> the parent style.  This may be null if unspecified
		'''   attributes need not be resolved in some other style. </param>
		''' <returns> the created style </returns>
		Public Overridable Function addStyle(ByVal nm As String, ByVal parent As Style) As Style
			Dim ___style As Style = New NamedStyle(Me, nm, parent)
			If nm IsNot Nothing Then styles.addAttribute(nm, ___style)
			Return ___style
		End Function

		''' <summary>
		''' Removes a named style previously added to the document.
		''' </summary>
		''' <param name="nm">  the name of the style to remove </param>
		Public Overridable Sub removeStyle(ByVal nm As String)
			styles.removeAttribute(nm)
		End Sub

		''' <summary>
		''' Fetches a named style previously added to the document
		''' </summary>
		''' <param name="nm">  the name of the style </param>
		''' <returns> the style </returns>
		Public Overridable Function getStyle(ByVal nm As String) As Style
			Return CType(styles.getAttribute(nm), Style)
		End Function

		''' <summary>
		''' Fetches the names of the styles defined.
		''' </summary>
		''' <returns> the list of names as an enumeration </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Property styleNames As System.Collections.IEnumerator(Of ?)
			Get
				Return styles.attributeNames
			End Get
		End Property

		''' <summary>
		''' Adds a listener to track when styles are added
		''' or removed.
		''' </summary>
		''' <param name="l"> the change listener </param>
		Public Overridable Sub addChangeListener(ByVal l As javax.swing.event.ChangeListener)
			styles.addChangeListener(l)
		End Sub

		''' <summary>
		''' Removes a listener that was tracking styles being
		''' added or removed.
		''' </summary>
		''' <param name="l"> the change listener </param>
		Public Overridable Sub removeChangeListener(ByVal l As javax.swing.event.ChangeListener)
			styles.removeChangeListener(l)
		End Sub

		''' <summary>
		''' Returns an array of all the <code>ChangeListener</code>s added
		''' to this StyleContext with addChangeListener().
		''' </summary>
		''' <returns> all of the <code>ChangeListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		Public Overridable Property changeListeners As javax.swing.event.ChangeListener()
			Get
				Return CType(styles, NamedStyle).changeListeners
			End Get
		End Property

		''' <summary>
		''' Gets the font from an attribute set.  This is
		''' implemented to try and fetch a cached font
		''' for the given AttributeSet, and if that fails
		''' the font features are resolved and the
		''' font is fetched from the low-level font cache.
		''' </summary>
		''' <param name="attr"> the attribute set </param>
		''' <returns> the font </returns>
		Public Overridable Function getFont(ByVal attr As AttributeSet) As Font
			' PENDING(prinz) add cache behavior
			Dim ___style As Integer = Font.PLAIN
			If StyleConstants.isBold(attr) Then ___style = ___style Or Font.BOLD
			If StyleConstants.isItalic(attr) Then ___style = ___style Or Font.ITALIC
			Dim family As String = StyleConstants.getFontFamily(attr)
			Dim size As Integer = StyleConstants.getFontSize(attr)

			''' <summary>
			''' if either superscript or subscript is
			''' is set, we need to reduce the font size
			''' by 2.
			''' </summary>
			If StyleConstants.isSuperscript(attr) OrElse StyleConstants.isSubscript(attr) Then size -= 2

			Return getFont(family, ___style, size)
		End Function

		''' <summary>
		''' Takes a set of attributes and turn it into a foreground color
		''' specification.  This might be used to specify things
		''' like brighter, more hue, etc.  By default it simply returns
		''' the value specified by the StyleConstants.Foreground attribute.
		''' </summary>
		''' <param name="attr"> the set of attributes </param>
		''' <returns> the color </returns>
		Public Overridable Function getForeground(ByVal attr As AttributeSet) As Color
			Return StyleConstants.getForeground(attr)
		End Function

		''' <summary>
		''' Takes a set of attributes and turn it into a background color
		''' specification.  This might be used to specify things
		''' like brighter, more hue, etc.  By default it simply returns
		''' the value specified by the StyleConstants.Background attribute.
		''' </summary>
		''' <param name="attr"> the set of attributes </param>
		''' <returns> the color </returns>
		Public Overridable Function getBackground(ByVal attr As AttributeSet) As Color
			Return StyleConstants.getBackground(attr)
		End Function

		''' <summary>
		''' Gets a new font.  This returns a Font from a cache
		''' if a cached font exists.  If not, a Font is added to
		''' the cache.  This is basically a low-level cache for
		''' 1.1 font features.
		''' </summary>
		''' <param name="family"> the font family (such as "Monospaced") </param>
		''' <param name="style"> the style of the font (such as Font.PLAIN) </param>
		''' <param name="size"> the point size &gt;= 1 </param>
		''' <returns> the new font </returns>
		Public Overridable Function getFont(ByVal family As String, ByVal style As Integer, ByVal size As Integer) As Font
			fontSearch.valuelue(family, style, size)
			Dim f As Font = fontTable(fontSearch)
			If f Is Nothing Then
				' haven't seen this one yet.
				Dim defaultStyle As Style = getStyle(StyleContext.DEFAULT_STYLE)
				If defaultStyle IsNot Nothing Then
					Const FONT_ATTRIBUTE_KEY As String = "FONT_ATTRIBUTE_KEY"
					Dim defaultFont As Font = CType(defaultStyle.getAttribute(FONT_ATTRIBUTE_KEY), Font)
					If defaultFont IsNot Nothing AndAlso defaultFont.family.equalsIgnoreCase(family) Then f = defaultFont.deriveFont(style, size)
				End If
				If f Is Nothing Then f = New Font(family, style, size)
				If Not sun.font.FontUtilities.fontSupportsDefaultEncoding(f) Then f = sun.font.FontUtilities.getCompositeFontUIResource(f)
				Dim key As New FontKey(family, style, size)
				fontTable(key) = f
			End If
			Return f
		End Function

		''' <summary>
		''' Returns font metrics for a font.
		''' </summary>
		''' <param name="f"> the font </param>
		''' <returns> the metrics </returns>
		Public Overridable Function getFontMetrics(ByVal f As Font) As FontMetrics
			' The Toolkit implementations cache, so we just forward
			' to the default toolkit.
			Return Toolkit.defaultToolkit.getFontMetrics(f)
		End Function

		' --- AttributeContext methods --------------------

		''' <summary>
		''' Adds an attribute to the given set, and returns
		''' the new representative set.
		''' <p>
		''' This method is thread safe, although most Swing methods
		''' are not. Please see
		''' <A HREF="https://docs.oracle.com/javase/tutorial/uiswing/concurrency/index.html">Concurrency
		''' in Swing</A> for more information.
		''' </summary>
		''' <param name="old"> the old attribute set </param>
		''' <param name="name"> the non-null attribute name </param>
		''' <param name="value"> the attribute value </param>
		''' <returns> the updated attribute set </returns>
		''' <seealso cref= MutableAttributeSet#addAttribute </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function addAttribute(ByVal old As AttributeSet, ByVal name As Object, ByVal value As Object) As AttributeSet Implements AbstractDocument.AttributeContext.addAttribute
			If (old.attributeCount + 1) <= compressionThreshold Then
				' build a search key and find/create an immutable and unique
				' set.
				search.removeAttributes(search)
				search.addAttributes(old)
				search.addAttribute(name, value)
				reclaim(old)
				Return immutableUniqueSet
			End If
			Dim ma As MutableAttributeSet = getMutableAttributeSet(old)
			ma.addAttribute(name, value)
			Return ma
		End Function

		''' <summary>
		''' Adds a set of attributes to the element.
		''' <p>
		''' This method is thread safe, although most Swing methods
		''' are not. Please see
		''' <A HREF="https://docs.oracle.com/javase/tutorial/uiswing/concurrency/index.html">Concurrency
		''' in Swing</A> for more information.
		''' </summary>
		''' <param name="old"> the old attribute set </param>
		''' <param name="attr"> the attributes to add </param>
		''' <returns> the updated attribute set </returns>
		''' <seealso cref= MutableAttributeSet#addAttribute </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function addAttributes(ByVal old As AttributeSet, ByVal attr As AttributeSet) As AttributeSet Implements AbstractDocument.AttributeContext.addAttributes
			If (old.attributeCount + attr.attributeCount) <= compressionThreshold Then
				' build a search key and find/create an immutable and unique
				' set.
				search.removeAttributes(search)
				search.addAttributes(old)
				search.addAttributes(attr)
				reclaim(old)
				Return immutableUniqueSet
			End If
			Dim ma As MutableAttributeSet = getMutableAttributeSet(old)
			ma.addAttributes(attr)
			Return ma
		End Function

		''' <summary>
		''' Removes an attribute from the set.
		''' <p>
		''' This method is thread safe, although most Swing methods
		''' are not. Please see
		''' <A HREF="https://docs.oracle.com/javase/tutorial/uiswing/concurrency/index.html">Concurrency
		''' in Swing</A> for more information.
		''' </summary>
		''' <param name="old"> the old set of attributes </param>
		''' <param name="name"> the non-null attribute name </param>
		''' <returns> the updated attribute set </returns>
		''' <seealso cref= MutableAttributeSet#removeAttribute </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function removeAttribute(ByVal old As AttributeSet, ByVal name As Object) As AttributeSet Implements AbstractDocument.AttributeContext.removeAttribute
			If (old.attributeCount - 1) <= compressionThreshold Then
				' build a search key and find/create an immutable and unique
				' set.
				search.removeAttributes(search)
				search.addAttributes(old)
				search.removeAttribute(name)
				reclaim(old)
				Return immutableUniqueSet
			End If
			Dim ma As MutableAttributeSet = getMutableAttributeSet(old)
			ma.removeAttribute(name)
			Return ma
		End Function

		''' <summary>
		''' Removes a set of attributes for the element.
		''' <p>
		''' This method is thread safe, although most Swing methods
		''' are not. Please see
		''' <A HREF="https://docs.oracle.com/javase/tutorial/uiswing/concurrency/index.html">Concurrency
		''' in Swing</A> for more information.
		''' </summary>
		''' <param name="old"> the old attribute set </param>
		''' <param name="names"> the attribute names </param>
		''' <returns> the updated attribute set </returns>
		''' <seealso cref= MutableAttributeSet#removeAttributes </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function removeAttributes(Of T1)(ByVal old As AttributeSet, ByVal names As System.Collections.IEnumerator(Of T1)) As AttributeSet Implements AbstractDocument.AttributeContext.removeAttributes
			If old.attributeCount <= compressionThreshold Then
				' build a search key and find/create an immutable and unique
				' set.
				search.removeAttributes(search)
				search.addAttributes(old)
				search.removeAttributes(names)
				reclaim(old)
				Return immutableUniqueSet
			End If
			Dim ma As MutableAttributeSet = getMutableAttributeSet(old)
			ma.removeAttributes(names)
			Return ma
		End Function

		''' <summary>
		''' Removes a set of attributes for the element.
		''' <p>
		''' This method is thread safe, although most Swing methods
		''' are not. Please see
		''' <A HREF="https://docs.oracle.com/javase/tutorial/uiswing/concurrency/index.html">Concurrency
		''' in Swing</A> for more information.
		''' </summary>
		''' <param name="old"> the old attribute set </param>
		''' <param name="attrs"> the attributes </param>
		''' <returns> the updated attribute set </returns>
		''' <seealso cref= MutableAttributeSet#removeAttributes </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function removeAttributes(ByVal old As AttributeSet, ByVal attrs As AttributeSet) As AttributeSet Implements AbstractDocument.AttributeContext.removeAttributes
			If old.attributeCount <= compressionThreshold Then
				' build a search key and find/create an immutable and unique
				' set.
				search.removeAttributes(search)
				search.addAttributes(old)
				search.removeAttributes(attrs)
				reclaim(old)
				Return immutableUniqueSet
			End If
			Dim ma As MutableAttributeSet = getMutableAttributeSet(old)
			ma.removeAttributes(attrs)
			Return ma
		End Function

		''' <summary>
		''' Fetches an empty AttributeSet.
		''' </summary>
		''' <returns> the set </returns>
		Public Overridable Property emptySet As AttributeSet Implements AbstractDocument.AttributeContext.getEmptySet
			Get
				Return SimpleAttributeSet.EMPTY
			End Get
		End Property

		''' <summary>
		''' Returns a set no longer needed by the MutableAttributeSet implementation.
		''' This is useful for operation under 1.1 where there are no weak
		''' references.  This would typically be called by the finalize method
		''' of the MutableAttributeSet implementation.
		''' <p>
		''' This method is thread safe, although most Swing methods
		''' are not. Please see
		''' <A HREF="https://docs.oracle.com/javase/tutorial/uiswing/concurrency/index.html">Concurrency
		''' in Swing</A> for more information.
		''' </summary>
		''' <param name="a"> the set to reclaim </param>
		Public Overridable Sub reclaim(ByVal a As AttributeSet) Implements AbstractDocument.AttributeContext.reclaim
			If javax.swing.SwingUtilities.eventDispatchThread Then attributesPool.Count ' force WeakHashMap to expunge stale entries
			' if current thread is not event dispatching thread
			' do not bother with expunging stale entries.
		End Sub

		' --- local methods -----------------------------------------------

		''' <summary>
		''' Returns the maximum number of key/value pairs to try and
		''' compress into unique/immutable sets.  Any sets above this
		''' limit will use hashtables and be a MutableAttributeSet.
		''' </summary>
		''' <returns> the threshold </returns>
		Protected Friend Overridable Property compressionThreshold As Integer
			Get
				Return THRESHOLD
			End Get
		End Property

		''' <summary>
		''' Create a compact set of attributes that might be shared.
		''' This is a hook for subclasses that want to alter the
		''' behavior of SmallAttributeSet.  This can be reimplemented
		''' to return an AttributeSet that provides some sort of
		''' attribute conversion.
		''' </summary>
		''' <param name="a"> The set of attributes to be represented in the
		'''  the compact form. </param>
		Protected Friend Overridable Function createSmallAttributeSet(ByVal a As AttributeSet) As SmallAttributeSet
			Return New SmallAttributeSet(Me, a)
		End Function

		''' <summary>
		''' Create a large set of attributes that should trade off
		''' space for time.  This set will not be shared.  This is
		''' a hook for subclasses that want to alter the behavior
		''' of the larger attribute storage format (which is
		''' SimpleAttributeSet by default).   This can be reimplemented
		''' to return a MutableAttributeSet that provides some sort of
		''' attribute conversion.
		''' </summary>
		''' <param name="a"> The set of attributes to be represented in the
		'''  the larger form. </param>
		Protected Friend Overridable Function createLargeAttributeSet(ByVal a As AttributeSet) As MutableAttributeSet
			Return New SimpleAttributeSet(a)
		End Function

		''' <summary>
		''' Clean the unused immutable sets out of the hashtable.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overridable Sub removeUnusedSets()
			attributesPool.Count ' force WeakHashMap to expunge stale entries
		End Sub

		''' <summary>
		''' Search for an existing attribute set using the current search
		''' parameters.  If a matching set is found, return it.  If a match
		''' is not found, we create a new set and add it to the pool.
		''' </summary>
		Friend Overridable Property immutableUniqueSet As AttributeSet
			Get
				' PENDING(prinz) should consider finding a alternative to
				' generating extra garbage on search key.
				Dim key As SmallAttributeSet = createSmallAttributeSet(search)
				Dim reference As WeakReference(Of SmallAttributeSet) = attributesPool(key)
				Dim a As SmallAttributeSet
				a = reference.get()
				If reference Is Nothing OrElse a Is Nothing Then
					a = key
					attributesPool(a) = New WeakReference(Of SmallAttributeSet)(a)
				End If
				Return a
			End Get
		End Property

		''' <summary>
		''' Creates a mutable attribute set to hand out because the current
		''' needs are too big to try and use a shared version.
		''' </summary>
		Friend Overridable Function getMutableAttributeSet(ByVal a As AttributeSet) As MutableAttributeSet
			If TypeOf a Is MutableAttributeSet AndAlso a IsNot SimpleAttributeSet.EMPTY Then Return CType(a, MutableAttributeSet)
			Return createLargeAttributeSet(a)
		End Function

		''' <summary>
		''' Converts a StyleContext to a String.
		''' </summary>
		''' <returns> the string </returns>
		Public Overrides Function ToString() As String
			removeUnusedSets()
			Dim s As String = ""
			For Each [set] As SmallAttributeSet In attributesPool.Keys
				s = s + [set] & vbLf
			Next [set]
			Return s
		End Function

		' --- serialization ---------------------------------------------

		''' <summary>
		''' Context-specific handling of writing out attributes
		''' </summary>
		Public Overridable Sub writeAttributes(ByVal out As ObjectOutputStream, ByVal a As AttributeSet)
			writeAttributeSet(out, a)
		End Sub

		''' <summary>
		''' Context-specific handling of reading in attributes
		''' </summary>
		Public Overridable Sub readAttributes(ByVal [in] As ObjectInputStream, ByVal a As MutableAttributeSet)
			readAttributeSet([in], a)
		End Sub

		''' <summary>
		''' Writes a set of attributes to the given object stream
		''' for the purpose of serialization.  This will take
		''' special care to deal with static attribute keys that
		''' have been registered wit the
		''' <code>registerStaticAttributeKey</code> method.
		''' Any attribute key not registered as a static key
		''' will be serialized directly.  All values are expected
		''' to be serializable.
		''' </summary>
		''' <param name="out"> the output stream </param>
		''' <param name="a"> the attribute set </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		Public Shared Sub writeAttributeSet(ByVal out As ObjectOutputStream, ByVal a As AttributeSet)
			Dim n As Integer = a.attributeCount
			out.writeInt(n)
			Dim keys As System.Collections.IEnumerator = a.attributeNames
			Do While keys.hasMoreElements()
				Dim key As Object = keys.nextElement()
				If TypeOf key Is Serializable Then
					out.writeObject(key)
				Else
					Dim ioFmt As Object = freezeKeyMap(key)
					If ioFmt Is Nothing Then Throw New NotSerializableException(key.GetType().name & " is not serializable as a key in an AttributeSet")
					out.writeObject(ioFmt)
				End If
				Dim value As Object = a.getAttribute(key)
				Dim ioFmt As Object = freezeKeyMap(value)
				If TypeOf value Is Serializable Then
					out.writeObject(If(ioFmt IsNot Nothing, ioFmt, value))
				Else
					If ioFmt Is Nothing Then Throw New NotSerializableException(value.GetType().name & " is not serializable as a value in an AttributeSet")
					out.writeObject(ioFmt)
				End If
			Loop
		End Sub

		''' <summary>
		''' Reads a set of attributes from the given object input
		''' stream that have been previously written out with
		''' <code>writeAttributeSet</code>.  This will try to restore
		''' keys that were static objects to the static objects in
		''' the current virtual machine considering only those keys
		''' that have been registered with the
		''' <code>registerStaticAttributeKey</code> method.
		''' The attributes retrieved from the stream will be placed
		''' into the given mutable set.
		''' </summary>
		''' <param name="in"> the object stream to read the attribute data from. </param>
		''' <param name="a">  the attribute set to place the attribute
		'''   definitions in. </param>
		''' <exception cref="ClassNotFoundException"> passed upward if encountered
		'''  when reading the object stream. </exception>
		''' <exception cref="IOException"> passed upward if encountered when
		'''  reading the object stream. </exception>
		Public Shared Sub readAttributeSet(ByVal [in] As ObjectInputStream, ByVal a As MutableAttributeSet)

			Dim n As Integer = [in].readInt()
			For i As Integer = 0 To n - 1
				Dim key As Object = [in].readObject()
				Dim value As Object = [in].readObject()
				If thawKeyMap IsNot Nothing Then
					Dim staticKey As Object = thawKeyMap(key)
					If staticKey IsNot Nothing Then key = staticKey
					Dim staticValue As Object = thawKeyMap(value)
					If staticValue IsNot Nothing Then value = staticValue
				End If
				a.addAttribute(key, value)
			Next i
		End Sub

		''' <summary>
		''' Registers an object as a static object that is being
		''' used as a key in attribute sets.  This allows the key
		''' to be treated specially for serialization.
		''' <p>
		''' For operation under a 1.1 virtual machine, this
		''' uses the value returned by <code>toString</code>
		''' concatenated to the classname.  The value returned
		''' by toString should not have the class reference
		''' in it (ie it should be reimplemented from the
		''' definition in Object) in order to be the same when
		''' recomputed later.
		''' </summary>
		''' <param name="key"> the non-null object key </param>
		Public Shared Sub registerStaticAttributeKey(ByVal key As Object)
			Dim ioFmt As String = key.GetType().name & "." & key.ToString()
			If freezeKeyMap Is Nothing Then
				freezeKeyMap = New Dictionary(Of Object, String)
				thawKeyMap = New Dictionary(Of String, Object)
			End If
			freezeKeyMap(key) = ioFmt
			thawKeyMap(ioFmt) = key
		End Sub

		''' <summary>
		''' Returns the object previously registered with
		''' <code>registerStaticAttributeKey</code>.
		''' </summary>
		Public Shared Function getStaticAttribute(ByVal key As Object) As Object
			If thawKeyMap Is Nothing OrElse key Is Nothing Then Return Nothing
			Return thawKeyMap(key)
		End Function

		''' <summary>
		''' Returns the String that <code>key</code> will be registered with </summary>
		''' <seealso cref= #getStaticAttribute </seealso>
		''' <seealso cref= #registerStaticAttributeKey </seealso>
		Public Shared Function getStaticAttributeKey(ByVal key As Object) As Object
			Return key.GetType().name & "." & key.ToString()
		End Function

		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			' clean out unused sets before saving
			removeUnusedSets()

			s.defaultWriteObject()
		End Sub

		Private Sub readObject(ByVal s As ObjectInputStream)
			fontSearch = New FontKey(Nothing, 0, 0)
			fontTable = New Dictionary(Of FontKey, Font)
			search = New SimpleAttributeSet
			attributesPool = Collections.synchronizedMap(New java.util.WeakHashMap(Of SmallAttributeSet, WeakReference(Of SmallAttributeSet)))
			s.defaultReadObject()
		End Sub

		' --- variables ---------------------------------------------------

		''' <summary>
		''' The name given to the default logical style attached
		''' to paragraphs.
		''' </summary>
		Public Const DEFAULT_STYLE As String = "default"

		Private Shared freezeKeyMap As Dictionary(Of Object, String)
		Private Shared thawKeyMap As Dictionary(Of String, Object)

		Private styles As Style
		<NonSerialized> _
		Private fontSearch As New FontKey(Nothing, 0, 0)
		<NonSerialized> _
		Private fontTable As New Dictionary(Of FontKey, Font)

		<NonSerialized> _
		Private attributesPool As IDictionary(Of SmallAttributeSet, WeakReference(Of SmallAttributeSet)) = Collections.synchronizedMap(New java.util.WeakHashMap(Of SmallAttributeSet, WeakReference(Of SmallAttributeSet)))
		<NonSerialized> _
		Private search As MutableAttributeSet = New SimpleAttributeSet

		''' <summary>
		''' Number of immutable sets that are not currently
		''' being used.  This helps indicate when the sets need
		''' to be cleaned out of the hashtable they are stored
		''' in.
		''' </summary>
		Private unusedSets As Integer

		''' <summary>
		''' The threshold for no longer sharing the set of attributes
		''' in an immutable table.
		''' </summary>
		Friend Const THRESHOLD As Integer = 9

		''' <summary>
		''' This class holds a small number of attributes in an array.
		''' The storage format is key, value, key, value, etc.  The size
		''' of the set is the length of the array divided by two.  By
		''' default, this is the class that will be used to store attributes
		''' when held in the compact sharable form.
		''' </summary>
		Public Class SmallAttributeSet
			Implements AttributeSet

			Private ReadOnly outerInstance As StyleContext


			Public Sub New(ByVal outerInstance As StyleContext, ByVal attributes As Object())
					Me.outerInstance = outerInstance
				Me.attributes = attributes
				updateResolveParent()
			End Sub

			Public Sub New(ByVal outerInstance As StyleContext, ByVal attrs As AttributeSet)
					Me.outerInstance = outerInstance
				Dim n As Integer = attrs.attributeCount
				Dim tbl As Object() = New Object(2 * n - 1){}
				Dim names As System.Collections.IEnumerator = attrs.attributeNames
				Dim i As Integer = 0
				Do While names.hasMoreElements()
					tbl(i) = names.nextElement()
					tbl(i+1) = attrs.getAttribute(tbl(i))
					i += 2
				Loop
				attributes = tbl
				updateResolveParent()
			End Sub

			Private Sub updateResolveParent()
				resolveParent = Nothing
				Dim tbl As Object() = attributes
				For i As Integer = 0 To tbl.Length - 1 Step 2
					If tbl(i) Is StyleConstants.ResolveAttribute Then
						resolveParent = CType(tbl(i + 1), AttributeSet)
						Exit For
					End If
				Next i
			End Sub

			Friend Overridable Function getLocalAttribute(ByVal nm As Object) As Object
				If nm Is StyleConstants.ResolveAttribute Then Return resolveParent
				Dim tbl As Object() = attributes
				For i As Integer = 0 To tbl.Length - 1 Step 2
					If nm.Equals(tbl(i)) Then Return tbl(i+1)
				Next i
				Return Nothing
			End Function

			' --- Object methods -------------------------

			''' <summary>
			''' Returns a string showing the key/value pairs
			''' </summary>
			Public Overrides Function ToString() As String
				Dim s As String = "{"
				Dim tbl As Object() = attributes
				For i As Integer = 0 To tbl.Length - 1 Step 2
					If TypeOf tbl(i+1) Is AttributeSet Then
						' don't recurse
						s = s + tbl(i) & "=" & "AttributeSet" & ","
					Else
						s = s + tbl(i) & "=" & tbl(i+1) & ","
					End If
				Next i
				s = s & "}"
				Return s
			End Function

			''' <summary>
			''' Returns a hashcode for this set of attributes. </summary>
			''' <returns>     a hashcode value for this set of attributes. </returns>
			Public Overrides Function GetHashCode() As Integer
				Dim code As Integer = 0
				Dim tbl As Object() = attributes
				For i As Integer = 1 To tbl.Length - 1 Step 2
					code = code Xor tbl(i).GetHashCode()
				Next i
				Return code
			End Function

			''' <summary>
			''' Compares this object to the specified object.
			''' The result is <code>true</code> if the object is an equivalent
			''' set of attributes. </summary>
			''' <param name="obj">   the object to compare with. </param>
			''' <returns>    <code>true</code> if the objects are equal;
			'''            <code>false</code> otherwise. </returns>
			Public Overrides Function Equals(ByVal obj As Object) As Boolean
				If TypeOf obj Is AttributeSet Then
					Dim attrs As AttributeSet = CType(obj, AttributeSet)
					Return ((attributeCount = attrs.attributeCount) AndAlso containsAttributes(attrs))
				End If
				Return False
			End Function

			''' <summary>
			''' Clones a set of attributes.  Since the set is immutable, a
			''' clone is basically the same set.
			''' </summary>
			''' <returns> the set of attributes </returns>
			Public Overridable Function clone() As Object
				Return Me
			End Function

			'  --- AttributeSet methods ----------------------------

			''' <summary>
			''' Gets the number of attributes that are defined.
			''' </summary>
			''' <returns> the number of attributes </returns>
			''' <seealso cref= AttributeSet#getAttributeCount </seealso>
			Public Overridable Property attributeCount As Integer Implements AttributeSet.getAttributeCount
				Get
					Return attributes.Length \ 2
				End Get
			End Property

			''' <summary>
			''' Checks whether a given attribute is defined.
			''' </summary>
			''' <param name="key"> the attribute key </param>
			''' <returns> true if the attribute is defined </returns>
			''' <seealso cref= AttributeSet#isDefined </seealso>
			Public Overridable Function isDefined(ByVal key As Object) As Boolean Implements AttributeSet.isDefined
				Dim a As Object() = attributes
				Dim n As Integer = a.Length
				For i As Integer = 0 To n - 1 Step 2
					If key.Equals(a(i)) Then Return True
				Next i
				Return False
			End Function

			''' <summary>
			''' Checks whether two attribute sets are equal.
			''' </summary>
			''' <param name="attr"> the attribute set to check against </param>
			''' <returns> true if the same </returns>
			''' <seealso cref= AttributeSet#isEqual </seealso>
			Public Overridable Function isEqual(ByVal attr As AttributeSet) As Boolean Implements AttributeSet.isEqual
				If TypeOf attr Is SmallAttributeSet Then Return attr Is Me
				Return ((attributeCount = attr.attributeCount) AndAlso containsAttributes(attr))
			End Function

			''' <summary>
			''' Copies a set of attributes.
			''' </summary>
			''' <returns> the copy </returns>
			''' <seealso cref= AttributeSet#copyAttributes </seealso>
			Public Overridable Function copyAttributes() As AttributeSet Implements AttributeSet.copyAttributes
				Return Me
			End Function

			''' <summary>
			''' Gets the value of an attribute.
			''' </summary>
			''' <param name="key"> the attribute name </param>
			''' <returns> the attribute value </returns>
			''' <seealso cref= AttributeSet#getAttribute </seealso>
			Public Overridable Function getAttribute(ByVal key As Object) As Object Implements AttributeSet.getAttribute
				Dim value As Object = getLocalAttribute(key)
				If value Is Nothing Then
					Dim parent As AttributeSet = resolveParent
					If parent IsNot Nothing Then value = parent.getAttribute(key)
				End If
				Return value
			End Function

			''' <summary>
			''' Gets the names of all attributes.
			''' </summary>
			''' <returns> the attribute names </returns>
			''' <seealso cref= AttributeSet#getAttributeNames </seealso>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overridable Property attributeNames As System.Collections.IEnumerator(Of ?)
				Get
					Return New KeyEnumeration(attributes)
				End Get
			End Property

			''' <summary>
			''' Checks whether a given attribute name/value is defined.
			''' </summary>
			''' <param name="name"> the attribute name </param>
			''' <param name="value"> the attribute value </param>
			''' <returns> true if the name/value is defined </returns>
			''' <seealso cref= AttributeSet#containsAttribute </seealso>
			Public Overridable Function containsAttribute(ByVal name As Object, ByVal value As Object) As Boolean Implements AttributeSet.containsAttribute
				Return value.Equals(getAttribute(name))
			End Function

			''' <summary>
			''' Checks whether the attribute set contains all of
			''' the given attributes.
			''' </summary>
			''' <param name="attrs"> the attributes to check </param>
			''' <returns> true if the element contains all the attributes </returns>
			''' <seealso cref= AttributeSet#containsAttributes </seealso>
			Public Overridable Function containsAttributes(ByVal attrs As AttributeSet) As Boolean Implements AttributeSet.containsAttributes
				Dim result As Boolean = True

				Dim names As System.Collections.IEnumerator = attrs.attributeNames
				Do While result AndAlso names.hasMoreElements()
					Dim name As Object = names.nextElement()
					result = attrs.getAttribute(name).Equals(getAttribute(name))
				Loop

				Return result
			End Function

			''' <summary>
			''' If not overriden, the resolving parent defaults to
			''' the parent element.
			''' </summary>
			''' <returns> the attributes from the parent </returns>
			''' <seealso cref= AttributeSet#getResolveParent </seealso>
			Public Overridable Property resolveParent As AttributeSet Implements AttributeSet.getResolveParent
				Get
					Return resolveParent
				End Get
			End Property

			' --- variables -----------------------------------------

			Friend attributes As Object()
			' This is also stored in attributes
			Friend resolveParent As AttributeSet
		End Class

		''' <summary>
		''' An enumeration of the keys in a SmallAttributeSet.
		''' </summary>
		Friend Class KeyEnumeration
			Implements System.Collections.IEnumerator(Of Object)

			Private ReadOnly outerInstance As StyleContext


			Friend Sub New(ByVal outerInstance As StyleContext, ByVal attr As Object())
					Me.outerInstance = outerInstance
				Me.attr = attr
				i = 0
			End Sub

			''' <summary>
			''' Tests if this enumeration contains more elements.
			''' </summary>
			''' <returns>  <code>true</code> if this enumeration contains more elements;
			'''          <code>false</code> otherwise.
			''' @since   JDK1.0 </returns>
			Public Overridable Function hasMoreElements() As Boolean
				Return i < attr.Length
			End Function

			''' <summary>
			''' Returns the next element of this enumeration.
			''' </summary>
			''' <returns>     the next element of this enumeration. </returns>
			''' <exception cref="NoSuchElementException">  if no more elements exist.
			''' @since      JDK1.0 </exception>
			Public Overridable Function nextElement() As Object
				If i < attr.Length Then
					Dim o As Object = attr(i)
					i += 2
					Return o
				End If
				Throw New NoSuchElementException
			End Function

			Friend attr As Object()
			Friend i As Integer
		End Class

		''' <summary>
		''' Sorts the key strings so that they can be very quickly compared
		''' in the attribute set searches.
		''' </summary>
		Friend Class KeyBuilder
			Private ReadOnly outerInstance As StyleContext

			Public Sub New(ByVal outerInstance As StyleContext)
				Me.outerInstance = outerInstance
			End Sub


			Public Overridable Sub initialize(ByVal a As AttributeSet)
				If TypeOf a Is SmallAttributeSet Then
					initialize(CType(a, SmallAttributeSet).attributes)
				Else
					keys.Clear()
					data.Clear()
					Dim names As System.Collections.IEnumerator = a.attributeNames
					Do While names.hasMoreElements()
						Dim name As Object = names.nextElement()
						addAttribute(name, a.getAttribute(name))
					Loop
				End If
			End Sub

			''' <summary>
			''' Initialize with a set of already sorted
			''' keys (data from an existing SmallAttributeSet).
			''' </summary>
			Private Sub initialize(ByVal sorted As Object())
				keys.Clear()
				data.Clear()
				Dim n As Integer = sorted.Length
				For i As Integer = 0 To n - 1 Step 2
					keys.Add(sorted(i))
					data.Add(sorted(i+1))
				Next i
			End Sub

			''' <summary>
			''' Creates a table of sorted key/value entries
			''' suitable for creation of an instance of
			''' SmallAttributeSet.
			''' </summary>
			Public Overridable Function createTable() As Object()
				Dim n As Integer = keys.Count
				Dim tbl As Object() = New Object(2 * n - 1){}
				For i As Integer = 0 To n - 1
					Dim offs As Integer = 2 * i
					tbl(offs) = keys(i)
					tbl(offs + 1) = data(i)
				Next i
				Return tbl
			End Function

			''' <summary>
			''' The number of key/value pairs contained
			''' in the current key being forged.
			''' </summary>
			Friend Overridable Property count As Integer
				Get
					Return keys.Count
				End Get
			End Property

			''' <summary>
			''' Adds a key/value to the set.
			''' </summary>
			Public Overridable Sub addAttribute(ByVal key As Object, ByVal value As Object)
				keys.Add(key)
				data.Add(value)
			End Sub

			''' <summary>
			''' Adds a set of key/value pairs to the set.
			''' </summary>
			Public Overridable Sub addAttributes(ByVal attr As AttributeSet)
				If TypeOf attr Is SmallAttributeSet Then
					' avoid searching the keys, they are already interned.
					Dim tbl As Object() = CType(attr, SmallAttributeSet).attributes
					Dim n As Integer = tbl.Length
					For i As Integer = 0 To n - 1 Step 2
						addAttribute(tbl(i), tbl(i+1))
					Next i
				Else
					Dim names As System.Collections.IEnumerator = attr.attributeNames
					Do While names.hasMoreElements()
						Dim name As Object = names.nextElement()
						addAttribute(name, attr.getAttribute(name))
					Loop
				End If
			End Sub

			''' <summary>
			''' Removes the given name from the set.
			''' </summary>
			Public Overridable Sub removeAttribute(ByVal key As Object)
				Dim n As Integer = keys.Count
				For i As Integer = 0 To n - 1
					If keys(i).Equals(key) Then
						keys.RemoveAt(i)
						data.RemoveAt(i)
						Return
					End If
				Next i
			End Sub

			''' <summary>
			''' Removes the set of keys from the set.
			''' </summary>
			Public Overridable Sub removeAttributes(ByVal names As System.Collections.IEnumerator)
				Do While names.hasMoreElements()
					Dim name As Object = names.nextElement()
					removeAttribute(name)
				Loop
			End Sub

			''' <summary>
			''' Removes the set of matching attributes from the set.
			''' </summary>
			Public Overridable Sub removeAttributes(ByVal attr As AttributeSet)
				Dim names As System.Collections.IEnumerator = attr.attributeNames
				Do While names.hasMoreElements()
					Dim name As Object = names.nextElement()
					Dim value As Object = attr.getAttribute(name)
					removeSearchAttribute(name, value)
				Loop
			End Sub

			Private Sub removeSearchAttribute(ByVal ikey As Object, ByVal value As Object)
				Dim n As Integer = keys.Count
				For i As Integer = 0 To n - 1
					If keys(i).Equals(ikey) Then
						If data(i).Equals(value) Then
							keys.RemoveAt(i)
							data.RemoveAt(i)
						End If
						Return
					End If
				Next i
			End Sub

			Private keys As New List(Of Object)
			Private data As New List(Of Object)
		End Class

		''' <summary>
		''' key for a font table
		''' </summary>
		Friend Class FontKey

			Private family As String
			Private style As Integer
			Private size As Integer

			''' <summary>
			''' Constructs a font key.
			''' </summary>
			Public Sub New(ByVal family As String, ByVal style As Integer, ByVal size As Integer)
				valuelue(family, style, size)
			End Sub

			Public Overridable Sub setValue(ByVal family As String, ByVal style As Integer, ByVal size As Integer)
				Me.family = If(family IsNot Nothing, family.intern(), Nothing)
				Me.style = style
				Me.size = size
			End Sub

			''' <summary>
			''' Returns a hashcode for this font. </summary>
			''' <returns>     a hashcode value for this font. </returns>
			Public Overrides Function GetHashCode() As Integer
				Dim fhash As Integer = If(family IsNot Nothing, family.GetHashCode(), 0)
				Return fhash Xor style Xor size
			End Function

			''' <summary>
			''' Compares this object to the specified object.
			''' The result is <code>true</code> if and only if the argument is not
			''' <code>null</code> and is a <code>Font</code> object with the same
			''' name, style, and point size as this font. </summary>
			''' <param name="obj">   the object to compare this font with. </param>
			''' <returns>    <code>true</code> if the objects are equal;
			'''            <code>false</code> otherwise. </returns>
			Public Overrides Function Equals(ByVal obj As Object) As Boolean
				If TypeOf obj Is FontKey Then
					Dim font As FontKey = CType(obj, FontKey)
					Return (size = font.size) AndAlso (style = font.style) AndAlso (family = font.family)
				End If
				Return False
			End Function

		End Class

		''' <summary>
		''' A collection of attributes, typically used to represent
		''' character and paragraph styles.  This is an implementation
		''' of MutableAttributeSet that can be observed if desired.
		''' These styles will take advantage of immutability while
		''' the sets are small enough, and may be substantially more
		''' efficient than something like SimpleAttributeSet.
		''' <p>
		''' <strong>Warning:</strong>
		''' Serialized objects of this class will not be compatible with
		''' future Swing releases. The current serialization support is
		''' appropriate for short term storage or RMI between applications running
		''' the same version of Swing.  As of 1.4, support for long term storage
		''' of all JavaBeans&trade;
		''' has been added to the <code>java.beans</code> package.
		''' Please see <seealso cref="java.beans.XMLEncoder"/>.
		''' </summary>
		<Serializable> _
		Public Class NamedStyle
			Implements Style

			Private ReadOnly outerInstance As StyleContext


			''' <summary>
			''' Creates a new named style.
			''' </summary>
			''' <param name="name"> the style name, null for unnamed </param>
			''' <param name="parent"> the parent style, null if none
			''' @since 1.4 </param>
			Public Sub New(ByVal outerInstance As StyleContext, ByVal name As String, ByVal parent As Style)
					Me.outerInstance = outerInstance
				attributes = outerInstance.emptySet
				If name IsNot Nothing Then name = name
				If parent IsNot Nothing Then resolveParent = parent
			End Sub

			''' <summary>
			''' Creates a new named style.
			''' </summary>
			''' <param name="parent"> the parent style, null if none
			''' @since 1.4 </param>
			Public Sub New(ByVal outerInstance As StyleContext, ByVal parent As Style)
					Me.outerInstance = outerInstance
				Me.New(Nothing, parent)
			End Sub

			''' <summary>
			''' Creates a new named style, with a null name and parent.
			''' </summary>
			Public Sub New(ByVal outerInstance As StyleContext)
					Me.outerInstance = outerInstance
				attributes = outerInstance.emptySet
			End Sub

			''' <summary>
			''' Converts the style to a string.
			''' </summary>
			''' <returns> the string </returns>
			Public Overrides Function ToString() As String
				Return "NamedStyle:" & name & " " & attributes
			End Function

			''' <summary>
			''' Fetches the name of the style.   A style is not required to be named,
			''' so null is returned if there is no name associated with the style.
			''' </summary>
			''' <returns> the name </returns>
			Public Overridable Property name As String Implements Style.getName
				Get
					If isDefined(StyleConstants.NameAttribute) Then Return getAttribute(StyleConstants.NameAttribute).ToString()
					Return Nothing
				End Get
				Set(ByVal name As String)
					If name IsNot Nothing Then Me.addAttribute(StyleConstants.NameAttribute, name)
				End Set
			End Property


			''' <summary>
			''' Adds a change listener.
			''' </summary>
			''' <param name="l"> the change listener </param>
			Public Overridable Sub addChangeListener(ByVal l As javax.swing.event.ChangeListener) Implements Style.addChangeListener
				listenerList.add(GetType(javax.swing.event.ChangeListener), l)
			End Sub

			''' <summary>
			''' Removes a change listener.
			''' </summary>
			''' <param name="l"> the change listener </param>
			Public Overridable Sub removeChangeListener(ByVal l As javax.swing.event.ChangeListener) Implements Style.removeChangeListener
				listenerList.remove(GetType(javax.swing.event.ChangeListener), l)
			End Sub


			''' <summary>
			''' Returns an array of all the <code>ChangeListener</code>s added
			''' to this NamedStyle with addChangeListener().
			''' </summary>
			''' <returns> all of the <code>ChangeListener</code>s added or an empty
			'''         array if no listeners have been added
			''' @since 1.4 </returns>
			Public Overridable Property changeListeners As javax.swing.event.ChangeListener()
				Get
					Return listenerList.getListeners(GetType(javax.swing.event.ChangeListener))
				End Get
			End Property


			''' <summary>
			''' Notifies all listeners that have registered interest for
			''' notification on this event type.  The event instance
			''' is lazily created using the parameters passed into
			''' the fire method.
			''' </summary>
			''' <seealso cref= EventListenerList </seealso>
			Protected Friend Overridable Sub fireStateChanged()
				' Guaranteed to return a non-null array
				Dim ___listeners As Object() = listenerList.listenerList
				' Process the listeners last to first, notifying
				' those that are interested in this event
				For i As Integer = ___listeners.Length-2 To 0 Step -2
					If ___listeners(i) Is GetType(javax.swing.event.ChangeListener) Then
						' Lazily create the event:
						If changeEvent Is Nothing Then changeEvent = New javax.swing.event.ChangeEvent(Me)
						CType(___listeners(i+1), javax.swing.event.ChangeListener).stateChanged(changeEvent)
					End If
				Next i
			End Sub

			''' <summary>
			''' Return an array of all the listeners of the given type that
			''' were added to this model.
			''' </summary>
			''' <returns> all of the objects receiving <em>listenerType</em> notifications
			'''          from this model
			''' 
			''' @since 1.3 </returns>
			Public Overridable Function getListeners(Of T As EventListener)(ByVal listenerType As Type) As T()
				Return listenerList.getListeners(listenerType)
			End Function

			' --- AttributeSet ----------------------------
			' delegated to the immutable field "attributes"

			''' <summary>
			''' Gets the number of attributes that are defined.
			''' </summary>
			''' <returns> the number of attributes &gt;= 0 </returns>
			''' <seealso cref= AttributeSet#getAttributeCount </seealso>
			Public Overridable Property attributeCount As Integer Implements AttributeSet.getAttributeCount
				Get
					Return attributes.attributeCount
				End Get
			End Property

			''' <summary>
			''' Checks whether a given attribute is defined.
			''' </summary>
			''' <param name="attrName"> the non-null attribute name </param>
			''' <returns> true if the attribute is defined </returns>
			''' <seealso cref= AttributeSet#isDefined </seealso>
			Public Overridable Function isDefined(ByVal attrName As Object) As Boolean Implements AttributeSet.isDefined
				Return attributes.isDefined(attrName)
			End Function

			''' <summary>
			''' Checks whether two attribute sets are equal.
			''' </summary>
			''' <param name="attr"> the attribute set to check against </param>
			''' <returns> true if the same </returns>
			''' <seealso cref= AttributeSet#isEqual </seealso>
			Public Overridable Function isEqual(ByVal attr As AttributeSet) As Boolean Implements AttributeSet.isEqual
				Return attributes.isEqual(attr)
			End Function

			''' <summary>
			''' Copies a set of attributes.
			''' </summary>
			''' <returns> the copy </returns>
			''' <seealso cref= AttributeSet#copyAttributes </seealso>
			Public Overridable Function copyAttributes() As AttributeSet Implements AttributeSet.copyAttributes
				Dim a As New NamedStyle
				a.attributes = attributes.copyAttributes()
				Return a
			End Function

			''' <summary>
			''' Gets the value of an attribute.
			''' </summary>
			''' <param name="attrName"> the non-null attribute name </param>
			''' <returns> the attribute value </returns>
			''' <seealso cref= AttributeSet#getAttribute </seealso>
			Public Overridable Function getAttribute(ByVal attrName As Object) As Object Implements AttributeSet.getAttribute
				Return attributes.getAttribute(attrName)
			End Function

			''' <summary>
			''' Gets the names of all attributes.
			''' </summary>
			''' <returns> the attribute names as an enumeration </returns>
			''' <seealso cref= AttributeSet#getAttributeNames </seealso>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overridable Property attributeNames As System.Collections.IEnumerator(Of ?)
				Get
					Return attributes.attributeNames
				End Get
			End Property

			''' <summary>
			''' Checks whether a given attribute name/value is defined.
			''' </summary>
			''' <param name="name"> the non-null attribute name </param>
			''' <param name="value"> the attribute value </param>
			''' <returns> true if the name/value is defined </returns>
			''' <seealso cref= AttributeSet#containsAttribute </seealso>
			Public Overridable Function containsAttribute(ByVal name As Object, ByVal value As Object) As Boolean Implements AttributeSet.containsAttribute
				Return attributes.containsAttribute(name, value)
			End Function


			''' <summary>
			''' Checks whether the element contains all the attributes.
			''' </summary>
			''' <param name="attrs"> the attributes to check </param>
			''' <returns> true if the element contains all the attributes </returns>
			''' <seealso cref= AttributeSet#containsAttributes </seealso>
			Public Overridable Function containsAttributes(ByVal attrs As AttributeSet) As Boolean Implements AttributeSet.containsAttributes
				Return attributes.containsAttributes(attrs)
			End Function

			''' <summary>
			''' Gets attributes from the parent.
			''' If not overriden, the resolving parent defaults to
			''' the parent element.
			''' </summary>
			''' <returns> the attributes from the parent </returns>
			''' <seealso cref= AttributeSet#getResolveParent </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
            Public Overridable Function getResolveParent() As AttributeSet Implements AttributeSet.getResolveParent 'JavaToDotNetTempPropertyGetresolveParent
			Public Overridable Property resolveParent As AttributeSet Implements AttributeSet.getResolveParent
				Get
					Return attributes.resolveParent
				End Get
				Set(ByVal parent As AttributeSet)
			End Property

			' --- MutableAttributeSet ----------------------------------
			' should fetch a new immutable record for the field
			' "attributes".

			''' <summary>
			''' Adds an attribute.
			''' </summary>
			''' <param name="name"> the non-null attribute name </param>
			''' <param name="value"> the attribute value </param>
			''' <seealso cref= MutableAttributeSet#addAttribute </seealso>
			Public Overridable Sub addAttribute(ByVal name As Object, ByVal value As Object) Implements MutableAttributeSet.addAttribute
				Dim context As StyleContext = StyleContext.this
				attributes = context.addAttribute(attributes, name, value)
				fireStateChanged()
			End Sub

			''' <summary>
			''' Adds a set of attributes to the element.
			''' </summary>
			''' <param name="attr"> the attributes to add </param>
			''' <seealso cref= MutableAttributeSet#addAttribute </seealso>
			Public Overridable Sub addAttributes(ByVal attr As AttributeSet) Implements MutableAttributeSet.addAttributes
				Dim context As StyleContext = StyleContext.this
				attributes = context.addAttributes(attributes, attr)
				fireStateChanged()
			End Sub

			''' <summary>
			''' Removes an attribute from the set.
			''' </summary>
			''' <param name="name"> the non-null attribute name </param>
			''' <seealso cref= MutableAttributeSet#removeAttribute </seealso>
			Public Overridable Sub removeAttribute(ByVal name As Object) Implements MutableAttributeSet.removeAttribute
				Dim context As StyleContext = StyleContext.this
				attributes = context.removeAttribute(attributes, name)
				fireStateChanged()
			End Sub

			''' <summary>
			''' Removes a set of attributes for the element.
			''' </summary>
			''' <param name="names"> the attribute names </param>
			''' <seealso cref= MutableAttributeSet#removeAttributes </seealso>
			Public Overridable Sub removeAttributes(Of T1)(ByVal names As System.Collections.IEnumerator(Of T1))
				Dim context As StyleContext = StyleContext.this
				attributes = context.removeAttributes(attributes, names)
				fireStateChanged()
			End Sub

			''' <summary>
			''' Removes a set of attributes for the element.
			''' </summary>
			''' <param name="attrs"> the attributes </param>
			''' <seealso cref= MutableAttributeSet#removeAttributes </seealso>
			Public Overridable Sub removeAttributes(ByVal attrs As AttributeSet) Implements MutableAttributeSet.removeAttributes
				Dim context As StyleContext = StyleContext.this
				If attrs Is Me Then
					attributes = context.emptySet
				Else
					attributes = context.removeAttributes(attributes, attrs)
				End If
				fireStateChanged()
			End Sub

				If parent IsNot Nothing Then
					addAttribute(StyleConstants.ResolveAttribute, parent)
				Else
					removeAttribute(StyleConstants.ResolveAttribute)
				End If
			End Sub

			' --- serialization ---------------------------------------------

			Private Sub writeObject(ByVal s As ObjectOutputStream)
				s.defaultWriteObject()
				writeAttributeSet(s, attributes)
			End Sub

			Private Sub readObject(ByVal s As ObjectInputStream)
				s.defaultReadObject()
				attributes = SimpleAttributeSet.EMPTY
				readAttributeSet(s, Me)
			End Sub

			' --- member variables -----------------------------------------------

			''' <summary>
			''' The change listeners for the model.
			''' </summary>
			Protected Friend listenerList As New javax.swing.event.EventListenerList

			''' <summary>
			''' Only one ChangeEvent is needed per model instance since the
			''' event's only (read-only) state is the source property.  The source
			''' of events generated here is always "this".
			''' </summary>
			<NonSerialized> _
			Protected Friend changeEvent As javax.swing.event.ChangeEvent = Nothing

			''' <summary>
			''' Inner AttributeSet implementation, which may be an
			''' immutable unique set being shared.
			''' </summary>
			<NonSerialized> _
			Private attributes As AttributeSet

		End Class

		Shared Sub New()
			' initialize the static key registry with the StyleConstants keys
			Try
				Dim n As Integer = StyleConstants.keys.Length
				For i As Integer = 0 To n - 1
					StyleContext.registerStaticAttributeKey(StyleConstants.keys(i))
				Next i
			Catch e As Exception
				e.printStackTrace()
			End Try
		End Sub


	End Class

End Namespace