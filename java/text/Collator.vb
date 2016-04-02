Imports System.Runtime.CompilerServices
Imports System.Collections.Concurrent

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

'
' * (C) Copyright Taligent, Inc. 1996-1998 -  All Rights Reserved
' * (C) Copyright IBM Corp. 1996-1998 - All Rights Reserved
' *
' *   The original version of this source code and documentation is copyrighted
' * and owned by Taligent, Inc., a wholly-owned subsidiary of IBM. These
' * materials are provided under terms of a License Agreement between Taligent
' * and Sun. This technology is protected by multiple US and International
' * patents. This notice and attribution to Taligent may not be removed.
' *   Taligent is a registered trademark of Taligent, Inc.
' *
' 

Namespace java.text



	''' <summary>
	''' The <code>Collator</code> class performs locale-sensitive
	''' <code>String</code> comparison. You use this class to build
	''' searching and sorting routines for natural language text.
	''' 
	''' <p>
	''' <code>Collator</code> is an abstract base class. Subclasses
	''' implement specific collation strategies. One subclass,
	''' <code>RuleBasedCollator</code>, is currently provided with
	''' the Java Platform and is applicable to a wide set of languages. Other
	''' subclasses may be created to handle more specialized needs.
	''' 
	''' <p>
	''' Like other locale-sensitive classes, you can use the static
	''' factory method, <code>getInstance</code>, to obtain the appropriate
	''' <code>Collator</code> object for a given locale. You will only need
	''' to look at the subclasses of <code>Collator</code> if you need
	''' to understand the details of a particular collation strategy or
	''' if you need to modify that strategy.
	''' 
	''' <p>
	''' The following example shows how to compare two strings using
	''' the <code>Collator</code> for the default locale.
	''' <blockquote>
	''' <pre>{@code
	''' // Compare two strings in the default locale
	''' Collator myCollator = Collator.getInstance();
	''' if( myCollator.compare("abc", "ABC") < 0 )
	'''     System.out.println("abc is less than ABC");
	''' else
	'''     System.out.println("abc is greater than or equal to ABC");
	''' }</pre>
	''' </blockquote>
	''' 
	''' <p>
	''' You can set a <code>Collator</code>'s <em>strength</em> property
	''' to determine the level of difference considered significant in
	''' comparisons. Four strengths are provided: <code>PRIMARY</code>,
	''' <code>SECONDARY</code>, <code>TERTIARY</code>, and <code>IDENTICAL</code>.
	''' The exact assignment of strengths to language features is
	''' locale dependant.  For example, in Czech, "e" and "f" are considered
	''' primary differences, while "e" and "&#283;" are secondary differences,
	''' "e" and "E" are tertiary differences and "e" and "e" are identical.
	''' The following shows how both case and accents could be ignored for
	''' US English.
	''' <blockquote>
	''' <pre>
	''' //Get the Collator for US English and set its strength to PRIMARY
	''' Collator usCollator = Collator.getInstance(Locale.US);
	''' usCollator.setStrength(Collator.PRIMARY);
	''' if( usCollator.compare("abc", "ABC") == 0 ) {
	'''     System.out.println("Strings are equivalent");
	''' }
	''' </pre>
	''' </blockquote>
	''' <p>
	''' For comparing <code>String</code>s exactly once, the <code>compare</code>
	''' method provides the best performance. When sorting a list of
	''' <code>String</code>s however, it is generally necessary to compare each
	''' <code>String</code> multiple times. In this case, <code>CollationKey</code>s
	''' provide better performance. The <code>CollationKey</code> class converts
	''' a <code>String</code> to a series of bits that can be compared bitwise
	''' against other <code>CollationKey</code>s. A <code>CollationKey</code> is
	''' created by a <code>Collator</code> object for a given <code>String</code>.
	''' <br>
	''' <strong>Note:</strong> <code>CollationKey</code>s from different
	''' <code>Collator</code>s can not be compared. See the class description
	''' for <seealso cref="CollationKey"/>
	''' for an example using <code>CollationKey</code>s.
	''' </summary>
	''' <seealso cref=         RuleBasedCollator </seealso>
	''' <seealso cref=         CollationKey </seealso>
	''' <seealso cref=         CollationElementIterator </seealso>
	''' <seealso cref=         Locale
	''' @author      Helena Shih, Laura Werner, Richard Gillam </seealso>

	Public MustInherit Class Collator
		Implements IComparer(Of Object), Cloneable

		''' <summary>
		''' Collator strength value.  When set, only PRIMARY differences are
		''' considered significant during comparison. The assignment of strengths
		''' to language features is locale dependant. A common example is for
		''' different base letters ("a" vs "b") to be considered a PRIMARY difference. </summary>
		''' <seealso cref= java.text.Collator#setStrength </seealso>
		''' <seealso cref= java.text.Collator#getStrength </seealso>
		Public Const PRIMARY As Integer = 0
		''' <summary>
		''' Collator strength value.  When set, only SECONDARY and above differences are
		''' considered significant during comparison. The assignment of strengths
		''' to language features is locale dependant. A common example is for
		''' different accented forms of the same base letter ("a" vs "\u00E4") to be
		''' considered a SECONDARY difference. </summary>
		''' <seealso cref= java.text.Collator#setStrength </seealso>
		''' <seealso cref= java.text.Collator#getStrength </seealso>
		Public Const SECONDARY As Integer = 1
		''' <summary>
		''' Collator strength value.  When set, only TERTIARY and above differences are
		''' considered significant during comparison. The assignment of strengths
		''' to language features is locale dependant. A common example is for
		''' case differences ("a" vs "A") to be considered a TERTIARY difference. </summary>
		''' <seealso cref= java.text.Collator#setStrength </seealso>
		''' <seealso cref= java.text.Collator#getStrength </seealso>
		Public Const TERTIARY As Integer = 2

		''' <summary>
		''' Collator strength value.  When set, all differences are
		''' considered significant during comparison. The assignment of strengths
		''' to language features is locale dependant. A common example is for control
		''' characters ("&#092;u0001" vs "&#092;u0002") to be considered equal at the
		''' PRIMARY, SECONDARY, and TERTIARY levels but different at the IDENTICAL
		''' level.  Additionally, differences between pre-composed accents such as
		''' "&#092;u00C0" (A-grave) and combining accents such as "A&#092;u0300"
		''' (A, combining-grave) will be considered significant at the IDENTICAL
		''' level if decomposition is set to NO_DECOMPOSITION.
		''' </summary>
		Public Const IDENTICAL As Integer = 3

		''' <summary>
		''' Decomposition mode value. With NO_DECOMPOSITION
		''' set, accented characters will not be decomposed for collation. This
		''' is the default setting and provides the fastest collation but
		''' will only produce correct results for languages that do not use accents. </summary>
		''' <seealso cref= java.text.Collator#getDecomposition </seealso>
		''' <seealso cref= java.text.Collator#setDecomposition </seealso>
		Public Const NO_DECOMPOSITION As Integer = 0

		''' <summary>
		''' Decomposition mode value. With CANONICAL_DECOMPOSITION
		''' set, characters that are canonical variants according to Unicode
		''' standard will be decomposed for collation. This should be used to get
		''' correct collation of accented characters.
		''' <p>
		''' CANONICAL_DECOMPOSITION corresponds to Normalization Form D as
		''' described in
		''' <a href="http://www.unicode.org/unicode/reports/tr15/tr15-23.html">Unicode
		''' Technical Report #15</a>. </summary>
		''' <seealso cref= java.text.Collator#getDecomposition </seealso>
		''' <seealso cref= java.text.Collator#setDecomposition </seealso>
		Public Const CANONICAL_DECOMPOSITION As Integer = 1

		''' <summary>
		''' Decomposition mode value. With FULL_DECOMPOSITION
		''' set, both Unicode canonical variants and Unicode compatibility variants
		''' will be decomposed for collation.  This causes not only accented
		''' characters to be collated, but also characters that have special formats
		''' to be collated with their norminal form. For example, the half-width and
		''' full-width ASCII and Katakana characters are then collated together.
		''' FULL_DECOMPOSITION is the most complete and therefore the slowest
		''' decomposition mode.
		''' <p>
		''' FULL_DECOMPOSITION corresponds to Normalization Form KD as
		''' described in
		''' <a href="http://www.unicode.org/unicode/reports/tr15/tr15-23.html">Unicode
		''' Technical Report #15</a>. </summary>
		''' <seealso cref= java.text.Collator#getDecomposition </seealso>
		''' <seealso cref= java.text.Collator#setDecomposition </seealso>
		Public Const FULL_DECOMPOSITION As Integer = 2

		''' <summary>
		''' Gets the Collator for the current default locale.
		''' The default locale is determined by java.util.Locale.getDefault. </summary>
		''' <returns> the Collator for the default locale.(for example, en_US) </returns>
		''' <seealso cref= java.util.Locale#getDefault </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		PublicShared ReadOnly Propertyinstance As Collator
			Get
				Return getInstance(java.util.Locale.default)
			End Get
		End Property

		''' <summary>
		''' Gets the Collator for the desired locale. </summary>
		''' <param name="desiredLocale"> the desired locale. </param>
		''' <returns> the Collator for the desired locale. </returns>
		''' <seealso cref= java.util.Locale </seealso>
		''' <seealso cref= java.util.ResourceBundle </seealso>
		Public Shared Function getInstance(ByVal desiredLocale As java.util.Locale) As Collator
			Dim ref As SoftReference(Of Collator) = cache.get(desiredLocale)
			Dim result As Collator = If(ref IsNot Nothing, ref.get(), Nothing)
			If result Is Nothing Then
				Dim adapter As sun.util.locale.provider.LocaleProviderAdapter
				adapter = sun.util.locale.provider.LocaleProviderAdapter.getAdapter(GetType(java.text.spi.CollatorProvider), desiredLocale)
				Dim provider As java.text.spi.CollatorProvider = adapter.collatorProvider
				result = provider.getInstance(desiredLocale)
				If result Is Nothing Then result = sun.util.locale.provider.LocaleProviderAdapter.forJRE().collatorProvider.getInstance(desiredLocale)
				Do
					If ref IsNot Nothing Then cache.remove(desiredLocale, ref)
					ref = cache.putIfAbsent(desiredLocale, New SoftReference(Of )(result))
					If ref Is Nothing Then Exit Do
					Dim cachedColl As Collator = ref.get()
					If cachedColl IsNot Nothing Then
						result = cachedColl
						Exit Do
					End If
				Loop
			End If
			Return CType(result.clone(), Collator) ' make the world safe
		End Function

		''' <summary>
		''' Compares the source string to the target string according to the
		''' collation rules for this Collator.  Returns an integer less than,
		''' equal to or greater than zero depending on whether the source String is
		''' less than, equal to or greater than the target string.  See the Collator
		''' class description for an example of use.
		''' <p>
		''' For a one time comparison, this method has the best performance. If a
		''' given String will be involved in multiple comparisons, CollationKey.compareTo
		''' has the best performance. See the Collator class description for an example
		''' using CollationKeys. </summary>
		''' <param name="source"> the source string. </param>
		''' <param name="target"> the target string. </param>
		''' <returns> Returns an integer value. Value is less than zero if source is less than
		''' target, value is zero if source and target are equal, value is greater than zero
		''' if source is greater than target. </returns>
		''' <seealso cref= java.text.CollationKey </seealso>
		''' <seealso cref= java.text.Collator#getCollationKey </seealso>
		Public MustOverride Function compare(ByVal source As String, ByVal target As String) As Integer

		''' <summary>
		''' Compares its two arguments for order.  Returns a negative integer,
		''' zero, or a positive integer as the first argument is less than, equal
		''' to, or greater than the second.
		''' <p>
		''' This implementation merely returns
		'''  <code> compare((String)o1, (String)o2) </code>.
		''' </summary>
		''' <returns> a negative integer, zero, or a positive integer as the
		'''         first argument is less than, equal to, or greater than the
		'''         second. </returns>
		''' <exception cref="ClassCastException"> the arguments cannot be cast to Strings. </exception>
		''' <seealso cref= java.util.Comparator
		''' @since   1.2 </seealso>
		Public Overrides Function compare(ByVal o1 As Object, ByVal o2 As Object) As Integer
		Return compare(CStr(o1), CStr(o2))
		End Function

		''' <summary>
		''' Transforms the String into a series of bits that can be compared bitwise
		''' to other CollationKeys. CollationKeys provide better performance than
		''' Collator.compare when Strings are involved in multiple comparisons.
		''' See the Collator class description for an example using CollationKeys. </summary>
		''' <param name="source"> the string to be transformed into a collation key. </param>
		''' <returns> the CollationKey for the given String based on this Collator's collation
		''' rules. If the source String is null, a null CollationKey is returned. </returns>
		''' <seealso cref= java.text.CollationKey </seealso>
		''' <seealso cref= java.text.Collator#compare </seealso>
		Public MustOverride Function getCollationKey(ByVal source As String) As CollationKey

		''' <summary>
		''' Convenience method for comparing the equality of two strings based on
		''' this Collator's collation rules. </summary>
		''' <param name="source"> the source string to be compared with. </param>
		''' <param name="target"> the target string to be compared with. </param>
		''' <returns> true if the strings are equal according to the collation
		''' rules.  false, otherwise. </returns>
		''' <seealso cref= java.text.Collator#compare </seealso>
		Public Overrides Function Equals(ByVal source As String, ByVal target As String) As Boolean
			Return (compare(source, target) = Collator.EQUAL)
		End Function

		''' <summary>
		''' Returns this Collator's strength property.  The strength property determines
		''' the minimum level of difference considered significant during comparison.
		''' See the Collator class description for an example of use. </summary>
		''' <returns> this Collator's current strength property. </returns>
		''' <seealso cref= java.text.Collator#setStrength </seealso>
		''' <seealso cref= java.text.Collator#PRIMARY </seealso>
		''' <seealso cref= java.text.Collator#SECONDARY </seealso>
		''' <seealso cref= java.text.Collator#TERTIARY </seealso>
		''' <seealso cref= java.text.Collator#IDENTICAL </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property strength As Integer
			Get
				Return strength
			End Get
			Set(ByVal newStrength As Integer)
				If (newStrength <> PRIMARY) AndAlso (newStrength <> SECONDARY) AndAlso (newStrength <> TERTIARY) AndAlso (newStrength <> IDENTICAL) Then Throw New IllegalArgumentException("Incorrect comparison level.")
				strength = newStrength
			End Set
		End Property


		''' <summary>
		''' Get the decomposition mode of this Collator. Decomposition mode
		''' determines how Unicode composed characters are handled. Adjusting
		''' decomposition mode allows the user to select between faster and more
		''' complete collation behavior.
		''' <p>The three values for decomposition mode are:
		''' <UL>
		''' <LI>NO_DECOMPOSITION,
		''' <LI>CANONICAL_DECOMPOSITION
		''' <LI>FULL_DECOMPOSITION.
		''' </UL>
		''' See the documentation for these three constants for a description
		''' of their meaning. </summary>
		''' <returns> the decomposition mode </returns>
		''' <seealso cref= java.text.Collator#setDecomposition </seealso>
		''' <seealso cref= java.text.Collator#NO_DECOMPOSITION </seealso>
		''' <seealso cref= java.text.Collator#CANONICAL_DECOMPOSITION </seealso>
		''' <seealso cref= java.text.Collator#FULL_DECOMPOSITION </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property decomposition As Integer
			Get
				Return decmp
			End Get
			Set(ByVal decompositionMode As Integer)
				If (decompositionMode <> NO_DECOMPOSITION) AndAlso (decompositionMode <> CANONICAL_DECOMPOSITION) AndAlso (decompositionMode <> FULL_DECOMPOSITION) Then Throw New IllegalArgumentException("Wrong decomposition mode.")
				decmp = decompositionMode
			End Set
		End Property

		''' <summary>
		''' Returns an array of all locales for which the
		''' <code>getInstance</code> methods of this class can return
		''' localized instances.
		''' The returned array represents the union of locales supported
		''' by the Java runtime and by installed
		''' <seealso cref="java.text.spi.CollatorProvider CollatorProvider"/> implementations.
		''' It must contain at least a Locale instance equal to
		''' <seealso cref="java.util.Locale#US Locale.US"/>.
		''' </summary>
		''' <returns> An array of locales for which localized
		'''         <code>Collator</code> instances are available. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		PublicShared ReadOnly PropertyavailableLocales As java.util.Locale()
			Get
				Dim pool As sun.util.locale.provider.LocaleServiceProviderPool = sun.util.locale.provider.LocaleServiceProviderPool.getPool(GetType(java.text.spi.CollatorProvider))
				Return pool.availableLocales
			End Get
		End Property

		''' <summary>
		''' Overrides Cloneable
		''' </summary>
		Public Overrides Function clone() As Object
			Try
				Return CType(MyBase.clone(), Collator)
			Catch e As CloneNotSupportedException
				Throw New InternalError(e)
			End Try
		End Function

		''' <summary>
		''' Compares the equality of two Collators. </summary>
		''' <param name="that"> the Collator to be compared with this. </param>
		''' <returns> true if this Collator is the same as that Collator;
		''' false otherwise. </returns>
		Public Overrides Function Equals(ByVal that As Object) As Boolean
			If Me Is that Then Return True
			If that Is Nothing Then Return False
			If Me.GetType() IsNot that.GetType() Then Return False
			Dim other As Collator = CType(that, Collator)
			Return ((strength = other.strength) AndAlso (decmp = other.decmp))
		End Function

		''' <summary>
		''' Generates the hash code for this Collator.
		''' </summary>
		Public MustOverride Overrides Function GetHashCode() As Integer

		''' <summary>
		''' Default constructor.  This constructor is
		''' protected so subclasses can get access to it. Users typically create
		''' a Collator sub-class by calling the factory method getInstance. </summary>
		''' <seealso cref= java.text.Collator#getInstance </seealso>
		Protected Friend Sub New()
			strength = TERTIARY
			decmp = CANONICAL_DECOMPOSITION
		End Sub

		Private strength As Integer = 0
		Private decmp As Integer = 0
		Private Shared ReadOnly cache As java.util.concurrent.ConcurrentMap(Of java.util.Locale, SoftReference(Of Collator)) = New ConcurrentDictionary(Of java.util.Locale, SoftReference(Of Collator))

		'
		' FIXME: These three constants should be removed.
		'
		''' <summary>
		''' LESS is returned if source string is compared to be less than target
		''' string in the compare() method. </summary>
		''' <seealso cref= java.text.Collator#compare </seealso>
		Friend Const LESS As Integer = -1
		''' <summary>
		''' EQUAL is returned if source string is compared to be equal to target
		''' string in the compare() method. </summary>
		''' <seealso cref= java.text.Collator#compare </seealso>
		Friend Const EQUAL As Integer = 0
		''' <summary>
		''' GREATER is returned if source string is compared to be greater than
		''' target string in the compare() method. </summary>
		''' <seealso cref= java.text.Collator#compare </seealso>
		Friend Const GREATER As Integer = 1
	End Class

End Namespace