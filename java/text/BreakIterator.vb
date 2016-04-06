Imports System.Runtime.CompilerServices

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

'
' * (C) Copyright Taligent, Inc. 1996, 1997 - All Rights Reserved
' * (C) Copyright IBM Corp. 1996 - 1998 - All Rights Reserved
' *
' * The original version of this source code and documentation
' * is copyrighted and owned by Taligent, Inc., a wholly-owned
' * subsidiary of IBM. These materials are provided under terms
' * of a License Agreement between Taligent and Sun. This technology
' * is protected by multiple US and International patents.
' *
' * This notice and attribution to Taligent may not be removed.
' * Taligent is a registered trademark of Taligent, Inc.
' *
' 

Namespace java.text



	''' <summary>
	''' The <code>BreakIterator</code> class implements methods for finding
	''' the location of boundaries in text. Instances of <code>BreakIterator</code>
	''' maintain a current position and scan over text
	''' returning the index of characters where boundaries occur.
	''' Internally, <code>BreakIterator</code> scans text using a
	''' <code>CharacterIterator</code>, and is thus able to scan text held
	''' by any object implementing that protocol. A <code>StringCharacterIterator</code>
	''' is used to scan <code>String</code> objects passed to <code>setText</code>.
	''' 
	''' <p>
	''' You use the factory methods provided by this class to create
	''' instances of various types of break iterators. In particular,
	''' use <code>getWordInstance</code>, <code>getLineInstance</code>,
	''' <code>getSentenceInstance</code>, and <code>getCharacterInstance</code>
	''' to create <code>BreakIterator</code>s that perform
	''' word, line, sentence, and character boundary analysis respectively.
	''' A single <code>BreakIterator</code> can work only on one unit
	''' (word, line, sentence, and so on). You must use a different iterator
	''' for each unit boundary analysis you wish to perform.
	''' 
	''' <p><a name="line"></a>
	''' Line boundary analysis determines where a text string can be
	''' broken when line-wrapping. The mechanism correctly handles
	''' punctuation and hyphenated words. Actual line breaking needs
	''' to also consider the available line width and is handled by
	''' higher-level software.
	''' 
	''' <p><a name="sentence"></a>
	''' Sentence boundary analysis allows selection with correct interpretation
	''' of periods within numbers and abbreviations, and trailing punctuation
	''' marks such as quotation marks and parentheses.
	''' 
	''' <p><a name="word"></a>
	''' Word boundary analysis is used by search and replace functions, as
	''' well as within text editing applications that allow the user to
	''' select words with a double click. Word selection provides correct
	''' interpretation of punctuation marks within and following
	''' words. Characters that are not part of a word, such as symbols
	''' or punctuation marks, have word-breaks on both sides.
	''' 
	''' <p><a name="character"></a>
	''' Character boundary analysis allows users to interact with characters
	''' as they expect to, for example, when moving the cursor through a text
	''' string. Character boundary analysis provides correct navigation
	''' through character strings, regardless of how the character is stored.
	''' The boundaries returned may be those of supplementary characters,
	''' combining character sequences, or ligature clusters.
	''' For example, an accented character might be stored as a base character
	''' and a diacritical mark. What users consider to be a character can
	''' differ between languages.
	''' 
	''' <p>
	''' The <code>BreakIterator</code> instances returned by the factory methods
	''' of this class are intended for use with natural languages only, not for
	''' programming language text. It is however possible to define subclasses
	''' that tokenize a programming language.
	''' 
	''' <P>
	''' <strong>Examples</strong>:<P>
	''' Creating and using text boundaries:
	''' <blockquote>
	''' <pre>
	''' Public Shared  Sub  main(String args[]) {
	'''      if (args.length == 1) {
	'''          String stringToExamine = args[0];
	'''          //print each word in order
	'''          BreakIterator boundary = BreakIterator.getWordInstance();
	'''          boundary.setText(stringToExamine);
	'''          printEachForward(boundary, stringToExamine);
	'''          //print each sentence in reverse order
	'''          boundary = BreakIterator.getSentenceInstance(Locale.US);
	'''          boundary.setText(stringToExamine);
	'''          printEachBackward(boundary, stringToExamine);
	'''          printFirst(boundary, stringToExamine);
	'''          printLast(boundary, stringToExamine);
	'''      }
	''' }
	''' </pre>
	''' </blockquote>
	''' 
	''' Print each element in order:
	''' <blockquote>
	''' <pre>
	''' Public Shared  Sub  printEachForward(BreakIterator boundary, String source) {
	'''     int start = boundary.first();
	'''     for (int end = boundary.next();
	'''          end != BreakIterator.DONE;
	'''          start = end, end = boundary.next()) {
	'''          System.out.println(source.substring(start,end));
	'''     }
	''' }
	''' </pre>
	''' </blockquote>
	''' 
	''' Print each element in reverse order:
	''' <blockquote>
	''' <pre>
	''' Public Shared  Sub  printEachBackward(BreakIterator boundary, String source) {
	'''     int end = boundary.last();
	'''     for (int start = boundary.previous();
	'''          start != BreakIterator.DONE;
	'''          end = start, start = boundary.previous()) {
	'''         System.out.println(source.substring(start,end));
	'''     }
	''' }
	''' </pre>
	''' </blockquote>
	''' 
	''' Print first element:
	''' <blockquote>
	''' <pre>
	''' Public Shared  Sub  printFirst(BreakIterator boundary, String source) {
	'''     int start = boundary.first();
	'''     int end = boundary.next();
	'''     System.out.println(source.substring(start,end));
	''' }
	''' </pre>
	''' </blockquote>
	''' 
	''' Print last element:
	''' <blockquote>
	''' <pre>
	''' Public Shared  Sub  printLast(BreakIterator boundary, String source) {
	'''     int end = boundary.last();
	'''     int start = boundary.previous();
	'''     System.out.println(source.substring(start,end));
	''' }
	''' </pre>
	''' </blockquote>
	''' 
	''' Print the element at a specified position:
	''' <blockquote>
	''' <pre>
	''' Public Shared  Sub  printAt(BreakIterator boundary, int pos, String source) {
	'''     int end = boundary.following(pos);
	'''     int start = boundary.previous();
	'''     System.out.println(source.substring(start,end));
	''' }
	''' </pre>
	''' </blockquote>
	''' 
	''' Find the next word:
	''' <blockquote>
	''' <pre>{@code
	''' Public Shared int nextWordStartAfter(int pos, String text) {
	'''     BreakIterator wb = BreakIterator.getWordInstance();
	'''     wb.setText(text);
	'''     int last = wb.following(pos);
	'''     int current = wb.next();
	'''     while (current != BreakIterator.DONE) {
	'''         for (int p = last; p < current; p++) {
	'''             if (Character.isLetter(text.codePointAt(p)))
	'''                 return last;
	'''         }
	'''         last = current;
	'''         current = wb.next();
	'''     }
	'''     return BreakIterator.DONE;
	''' }
	''' }</pre>
	''' (The iterator returned by BreakIterator.getWordInstance() is unique in that
	''' the break positions it returns don't represent both the start and end of the
	''' thing being iterated over.  That is, a sentence-break iterator returns breaks
	''' that each represent the end of one sentence and the beginning of the next.
	''' With the word-break iterator, the characters between two boundaries might be a
	''' word, or they might be the punctuation or whitespace between two words.  The
	''' above code uses a simple heuristic to determine which boundary is the beginning
	''' of a word: If the characters between this boundary and the next boundary
	''' include at least one letter (this can be an alphabetical letter, a CJK ideograph,
	''' a Hangul syllable, a Kana character, etc.), then the text between this boundary
	''' and the next is a word; otherwise, it's the material between words.)
	''' </blockquote>
	''' </summary>
	''' <seealso cref= CharacterIterator
	'''  </seealso>

	Public MustInherit Class BreakIterator
		Implements Cloneable

		''' <summary>
		''' Constructor. BreakIterator is stateless and has no default behavior.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Create a copy of this iterator </summary>
		''' <returns> A copy of this </returns>
		Public Overrides Function clone() As Object
			Try
				Return MyBase.clone()
			Catch e As CloneNotSupportedException
				Throw New InternalError(e)
			End Try
		End Function

		''' <summary>
		''' DONE is returned by previous(), next(), next(int), preceding(int)
		''' and following(int) when either the first or last text boundary has been
		''' reached.
		''' </summary>
		Public Const DONE As Integer = -1

		''' <summary>
		''' Returns the first boundary. The iterator's current position is set
		''' to the first text boundary. </summary>
		''' <returns> The character index of the first text boundary. </returns>
		Public MustOverride Function first() As Integer

		''' <summary>
		''' Returns the last boundary. The iterator's current position is set
		''' to the last text boundary. </summary>
		''' <returns> The character index of the last text boundary. </returns>
		Public MustOverride Function last() As Integer

		''' <summary>
		''' Returns the nth boundary from the current boundary. If either
		''' the first or last text boundary has been reached, it returns
		''' <code>BreakIterator.DONE</code> and the current position is set to either
		''' the first or last text boundary depending on which one is reached. Otherwise,
		''' the iterator's current position is set to the new boundary.
		''' For example, if the iterator's current position is the mth text boundary
		''' and three more boundaries exist from the current boundary to the last text
		''' boundary, the next(2) call will return m + 2. The new text position is set
		''' to the (m + 2)th text boundary. A next(4) call would return
		''' <code>BreakIterator.DONE</code> and the last text boundary would become the
		''' new text position. </summary>
		''' <param name="n"> which boundary to return.  A value of 0
		''' does nothing.  Negative values move to previous boundaries
		''' and positive values move to later boundaries. </param>
		''' <returns> The character index of the nth boundary from the current position
		''' or <code>BreakIterator.DONE</code> if either first or last text boundary
		''' has been reached. </returns>
		Public MustOverride Function [next](  n As Integer) As Integer

		''' <summary>
		''' Returns the boundary following the current boundary. If the current boundary
		''' is the last text boundary, it returns <code>BreakIterator.DONE</code> and
		''' the iterator's current position is unchanged. Otherwise, the iterator's
		''' current position is set to the boundary following the current boundary. </summary>
		''' <returns> The character index of the next text boundary or
		''' <code>BreakIterator.DONE</code> if the current boundary is the last text
		''' boundary.
		''' Equivalent to next(1). </returns>
		''' <seealso cref= #next(int) </seealso>
		Public MustOverride Function [next]() As Integer

		''' <summary>
		''' Returns the boundary preceding the current boundary. If the current boundary
		''' is the first text boundary, it returns <code>BreakIterator.DONE</code> and
		''' the iterator's current position is unchanged. Otherwise, the iterator's
		''' current position is set to the boundary preceding the current boundary. </summary>
		''' <returns> The character index of the previous text boundary or
		''' <code>BreakIterator.DONE</code> if the current boundary is the first text
		''' boundary. </returns>
		Public MustOverride Function previous() As Integer

		''' <summary>
		''' Returns the first boundary following the specified character offset. If the
		''' specified offset equals to the last text boundary, it returns
		''' <code>BreakIterator.DONE</code> and the iterator's current position is unchanged.
		''' Otherwise, the iterator's current position is set to the returned boundary.
		''' The value returned is always greater than the offset or the value
		''' <code>BreakIterator.DONE</code>. </summary>
		''' <param name="offset"> the character offset to begin scanning. </param>
		''' <returns> The first boundary after the specified offset or
		''' <code>BreakIterator.DONE</code> if the last text boundary is passed in
		''' as the offset. </returns>
		''' <exception cref="IllegalArgumentException"> if the specified offset is less than
		''' the first text boundary or greater than the last text boundary. </exception>
		Public MustOverride Function following(  offset As Integer) As Integer

		''' <summary>
		''' Returns the last boundary preceding the specified character offset. If the
		''' specified offset equals to the first text boundary, it returns
		''' <code>BreakIterator.DONE</code> and the iterator's current position is unchanged.
		''' Otherwise, the iterator's current position is set to the returned boundary.
		''' The value returned is always less than the offset or the value
		''' <code>BreakIterator.DONE</code>. </summary>
		''' <param name="offset"> the character offset to begin scanning. </param>
		''' <returns> The last boundary before the specified offset or
		''' <code>BreakIterator.DONE</code> if the first text boundary is passed in
		''' as the offset. </returns>
		''' <exception cref="IllegalArgumentException"> if the specified offset is less than
		''' the first text boundary or greater than the last text boundary.
		''' @since 1.2 </exception>
		Public Overridable Function preceding(  offset As Integer) As Integer
			' NOTE:  This implementation is here solely because we can't add new
			' abstract methods to an existing class.  There is almost ALWAYS a
			' better, faster way to do this.
			Dim pos As Integer = following(offset)
			Do While pos >= offset AndAlso pos <> DONE
				pos = previous()
			Loop
			Return pos
		End Function

		''' <summary>
		''' Returns true if the specified character offset is a text boundary. </summary>
		''' <param name="offset"> the character offset to check. </param>
		''' <returns> <code>true</code> if "offset" is a boundary position,
		''' <code>false</code> otherwise. </returns>
		''' <exception cref="IllegalArgumentException"> if the specified offset is less than
		''' the first text boundary or greater than the last text boundary.
		''' @since 1.2 </exception>
		Public Overridable Function isBoundary(  offset As Integer) As Boolean
			' NOTE: This implementation probably is wrong for most situations
			' because it fails to take into account the possibility that a
			' CharacterIterator passed to setText() may not have a begin offset
			' of 0.  But since the abstract BreakIterator doesn't have that
			' knowledge, it assumes the begin offset is 0.  If you subclass
			' BreakIterator, copy the SimpleTextBoundary implementation of this
			' function into your subclass.  [This should have been abstract at
			' this level, but it's too late to fix that now.]
			If offset = 0 Then Return True
			Dim boundary_Renamed As Integer = following(offset - 1)
			If boundary_Renamed = DONE Then Throw New IllegalArgumentException
			Return boundary_Renamed = offset
		End Function

		''' <summary>
		''' Returns character index of the text boundary that was most
		''' recently returned by next(), next(int), previous(), first(), last(),
		''' following(int) or preceding(int). If any of these methods returns
		''' <code>BreakIterator.DONE</code> because either first or last text boundary
		''' has been reached, it returns the first or last text boundary depending on
		''' which one is reached. </summary>
		''' <returns> The text boundary returned from the above methods, first or last
		''' text boundary. </returns>
		''' <seealso cref= #next() </seealso>
		''' <seealso cref= #next(int) </seealso>
		''' <seealso cref= #previous() </seealso>
		''' <seealso cref= #first() </seealso>
		''' <seealso cref= #last() </seealso>
		''' <seealso cref= #following(int) </seealso>
		''' <seealso cref= #preceding(int) </seealso>
		Public MustOverride Function current() As Integer

		''' <summary>
		''' Get the text being scanned </summary>
		''' <returns> the text being scanned </returns>
		Public MustOverride Property text As CharacterIterator

		''' <summary>
		''' Set a new text string to be scanned.  The current scan
		''' position is reset to first(). </summary>
		''' <param name="newText"> new text to scan. </param>
		Public Overridable Property text As String
			Set(  newText As String)
				text = New StringCharacterIterator(newText)
			End Set
		End Property


		Private Const CHARACTER_INDEX As Integer = 0
		Private Const WORD_INDEX As Integer = 1
		Private Const LINE_INDEX As Integer = 2
		Private Const SENTENCE_INDEX As Integer = 3

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Shared ReadOnly iterCache As SoftReference(Of BreakIteratorCache)() = CType(New SoftReference(Of ?)(3){}, SoftReference(Of BreakIteratorCache)())

		''' <summary>
		''' Returns a new <code>BreakIterator</code> instance
		''' for <a href="BreakIterator.html#word">word breaks</a>
		''' for the <seealso cref="Locale#getDefault() default locale"/>. </summary>
		''' <returns> A break iterator for word breaks </returns>
		PublicShared ReadOnly PropertywordInstance As BreakIterator
			Get
				Return getWordInstance(java.util.Locale.default)
			End Get
		End Property

		''' <summary>
		''' Returns a new <code>BreakIterator</code> instance
		''' for <a href="BreakIterator.html#word">word breaks</a>
		''' for the given locale. </summary>
		''' <param name="locale"> the desired locale </param>
		''' <returns> A break iterator for word breaks </returns>
		''' <exception cref="NullPointerException"> if <code>locale</code> is null </exception>
		Public Shared Function getWordInstance(  locale As java.util.Locale) As BreakIterator
			Return getBreakInstance(locale, WORD_INDEX)
		End Function

		''' <summary>
		''' Returns a new <code>BreakIterator</code> instance
		''' for <a href="BreakIterator.html#line">line breaks</a>
		''' for the <seealso cref="Locale#getDefault() default locale"/>. </summary>
		''' <returns> A break iterator for line breaks </returns>
		PublicShared ReadOnly PropertylineInstance As BreakIterator
			Get
				Return getLineInstance(java.util.Locale.default)
			End Get
		End Property

		''' <summary>
		''' Returns a new <code>BreakIterator</code> instance
		''' for <a href="BreakIterator.html#line">line breaks</a>
		''' for the given locale. </summary>
		''' <param name="locale"> the desired locale </param>
		''' <returns> A break iterator for line breaks </returns>
		''' <exception cref="NullPointerException"> if <code>locale</code> is null </exception>
		Public Shared Function getLineInstance(  locale As java.util.Locale) As BreakIterator
			Return getBreakInstance(locale, LINE_INDEX)
		End Function

		''' <summary>
		''' Returns a new <code>BreakIterator</code> instance
		''' for <a href="BreakIterator.html#character">character breaks</a>
		''' for the <seealso cref="Locale#getDefault() default locale"/>. </summary>
		''' <returns> A break iterator for character breaks </returns>
		PublicShared ReadOnly PropertycharacterInstance As BreakIterator
			Get
				Return getCharacterInstance(java.util.Locale.default)
			End Get
		End Property

		''' <summary>
		''' Returns a new <code>BreakIterator</code> instance
		''' for <a href="BreakIterator.html#character">character breaks</a>
		''' for the given locale. </summary>
		''' <param name="locale"> the desired locale </param>
		''' <returns> A break iterator for character breaks </returns>
		''' <exception cref="NullPointerException"> if <code>locale</code> is null </exception>
		Public Shared Function getCharacterInstance(  locale As java.util.Locale) As BreakIterator
			Return getBreakInstance(locale, CHARACTER_INDEX)
		End Function

		''' <summary>
		''' Returns a new <code>BreakIterator</code> instance
		''' for <a href="BreakIterator.html#sentence">sentence breaks</a>
		''' for the <seealso cref="Locale#getDefault() default locale"/>. </summary>
		''' <returns> A break iterator for sentence breaks </returns>
		PublicShared ReadOnly PropertysentenceInstance As BreakIterator
			Get
				Return getSentenceInstance(java.util.Locale.default)
			End Get
		End Property

		''' <summary>
		''' Returns a new <code>BreakIterator</code> instance
		''' for <a href="BreakIterator.html#sentence">sentence breaks</a>
		''' for the given locale. </summary>
		''' <param name="locale"> the desired locale </param>
		''' <returns> A break iterator for sentence breaks </returns>
		''' <exception cref="NullPointerException"> if <code>locale</code> is null </exception>
		Public Shared Function getSentenceInstance(  locale As java.util.Locale) As BreakIterator
			Return getBreakInstance(locale, SENTENCE_INDEX)
		End Function

		Private Shared Function getBreakInstance(  locale As java.util.Locale,   type As Integer) As BreakIterator
			If iterCache(type) IsNot Nothing Then
				Dim cache As BreakIteratorCache = iterCache(type).get()
				If cache IsNot Nothing Then
					If cache.locale.Equals(locale) Then Return cache.createBreakInstance()
				End If
			End If

			Dim result As BreakIterator = createBreakInstance(locale, type)
			Dim cache As New BreakIteratorCache(locale, result)
			iterCache(type) = New SoftReference(Of )(cache)
			Return result
		End Function

		Private Shared Function createBreakInstance(  locale As java.util.Locale,   type As Integer) As BreakIterator
			Dim adapter As sun.util.locale.provider.LocaleProviderAdapter = sun.util.locale.provider.LocaleProviderAdapter.getAdapter(GetType(java.text.spi.BreakIteratorProvider), locale)
			Dim [iterator] As BreakIterator = createBreakInstance(adapter, locale, type)
			If [iterator] Is Nothing Then [iterator] = createBreakInstance(sun.util.locale.provider.LocaleProviderAdapter.forJRE(), locale, type)
			Return [iterator]
		End Function

		Private Shared Function createBreakInstance(  adapter As sun.util.locale.provider.LocaleProviderAdapter,   locale As java.util.Locale,   type As Integer) As BreakIterator
			Dim breakIteratorProvider As java.text.spi.BreakIteratorProvider = adapter.breakIteratorProvider
			Dim [iterator] As BreakIterator = Nothing
			Select Case type
			Case CHARACTER_INDEX
				[iterator] = breakIteratorProvider.getCharacterInstance(locale)
			Case WORD_INDEX
				[iterator] = breakIteratorProvider.getWordInstance(locale)
			Case LINE_INDEX
				[iterator] = breakIteratorProvider.getLineInstance(locale)
			Case SENTENCE_INDEX
				[iterator] = breakIteratorProvider.getSentenceInstance(locale)
			End Select
			Return [iterator]
		End Function

		''' <summary>
		''' Returns an array of all locales for which the
		''' <code>get*Instance</code> methods of this class can return
		''' localized instances.
		''' The returned array represents the union of locales supported by the Java
		''' runtime and by installed
		''' <seealso cref="java.text.spi.BreakIteratorProvider BreakIteratorProvider"/> implementations.
		''' It must contain at least a <code>Locale</code>
		''' instance equal to <seealso cref="java.util.Locale#US Locale.US"/>.
		''' </summary>
		''' <returns> An array of locales for which localized
		'''         <code>BreakIterator</code> instances are available. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		PublicShared ReadOnly PropertyavailableLocales As java.util.Locale()
			Get
				Dim pool As sun.util.locale.provider.LocaleServiceProviderPool = sun.util.locale.provider.LocaleServiceProviderPool.getPool(GetType(java.text.spi.BreakIteratorProvider))
				Return pool.availableLocales
			End Get
		End Property

		Private NotInheritable Class BreakIteratorCache

			Private iter As BreakIterator
			Private locale As java.util.Locale

			Friend Sub New(  locale As java.util.Locale,   iter As BreakIterator)
				Me.locale = locale
				Me.iter = CType(iter.clone(), BreakIterator)
			End Sub

			Friend Property locale As java.util.Locale
				Get
					Return locale
				End Get
			End Property

			Friend Function createBreakInstance() As BreakIterator
				Return CType(iter.clone(), BreakIterator)
			End Function
		End Class
	End Class

End Namespace