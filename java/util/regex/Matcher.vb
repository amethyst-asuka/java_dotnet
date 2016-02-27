Imports System

'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util.regex


	''' <summary>
	''' An engine that performs match operations on a {@link java.lang.CharSequence
	''' character sequence} by interpreting a <seealso cref="Pattern"/>.
	''' 
	''' <p> A matcher is created from a pattern by invoking the pattern's {@link
	''' Pattern#matcher matcher} method.  Once created, a matcher can be used to
	''' perform three different kinds of match operations:
	''' 
	''' <ul>
	''' 
	'''   <li><p> The <seealso cref="#matches matches"/> method attempts to match the entire
	'''   input sequence against the pattern.  </p></li>
	''' 
	'''   <li><p> The <seealso cref="#lookingAt lookingAt"/> method attempts to match the
	'''   input sequence, starting at the beginning, against the pattern.  </p></li>
	''' 
	'''   <li><p> The <seealso cref="#find find"/> method scans the input sequence looking for
	'''   the next subsequence that matches the pattern.  </p></li>
	''' 
	''' </ul>
	''' 
	''' <p> Each of these methods returns a boolean indicating success or failure.
	''' More information about a successful match can be obtained by querying the
	''' state of the matcher.
	''' 
	''' <p> A matcher finds matches in a subset of its input called the
	''' <i>region</i>. By default, the region contains all of the matcher's input.
	''' The region can be modified via the<seealso cref="#region region"/> method and queried
	''' via the <seealso cref="#regionStart regionStart"/> and <seealso cref="#regionEnd regionEnd"/>
	''' methods. The way that the region boundaries interact with some pattern
	''' constructs can be changed. See {@link #useAnchoringBounds
	''' useAnchoringBounds} and <seealso cref="#useTransparentBounds useTransparentBounds"/>
	''' for more details.
	''' 
	''' <p> This class also defines methods for replacing matched subsequences with
	''' new strings whose contents can, if desired, be computed from the match
	''' result.  The <seealso cref="#appendReplacement appendReplacement"/> and {@link
	''' #appendTail appendTail} methods can be used in tandem in order to collect
	''' the result into an existing string buffer, or the more convenient {@link
	''' #replaceAll replaceAll} method can be used to create a string in which every
	''' matching subsequence in the input sequence is replaced.
	''' 
	''' <p> The explicit state of a matcher includes the start and end indices of
	''' the most recent successful match.  It also includes the start and end
	''' indices of the input subsequence captured by each <a
	''' href="Pattern.html#cg">capturing group</a> in the pattern as well as a total
	''' count of such subsequences.  As a convenience, methods are also provided for
	''' returning these captured subsequences in string form.
	''' 
	''' <p> The explicit state of a matcher is initially undefined; attempting to
	''' query any part of it before a successful match will cause an {@link
	''' IllegalStateException} to be thrown.  The explicit state of a matcher is
	''' recomputed by every match operation.
	''' 
	''' <p> The implicit state of a matcher includes the input character sequence as
	''' well as the <i>append position</i>, which is initially zero and is updated
	''' by the <seealso cref="#appendReplacement appendReplacement"/> method.
	''' 
	''' <p> A matcher may be reset explicitly by invoking its <seealso cref="#reset()"/>
	''' method or, if a new input sequence is desired, its {@link
	''' #reset(java.lang.CharSequence) reset(CharSequence)} method.  Resetting a
	''' matcher discards its explicit state information and sets the append position
	''' to zero.
	''' 
	''' <p> Instances of this class are not safe for use by multiple concurrent
	''' threads. </p>
	''' 
	''' 
	''' @author      Mike McCloskey
	''' @author      Mark Reinhold
	''' @author      JSR-51 Expert Group
	''' @since       1.4
	''' @spec        JSR-51
	''' </summary>

	Public NotInheritable Class Matcher
		Implements MatchResult

		''' <summary>
		''' The Pattern object that created this Matcher.
		''' </summary>
		Friend parentPattern As Pattern

		''' <summary>
		''' The storage used by groups. They may contain invalid values if
		''' a group was skipped during the matching.
		''' </summary>
		Friend groups As Integer()

		''' <summary>
		''' The range within the sequence that is to be matched. Anchors
		''' will match at these "hard" boundaries. Changing the region
		''' changes these values.
		''' </summary>
		Friend [from], [to] As Integer

		''' <summary>
		''' Lookbehind uses this value to ensure that the subexpression
		''' match ends at the point where the lookbehind was encountered.
		''' </summary>
		Friend lookbehindTo As Integer

		''' <summary>
		''' The original string being matched.
		''' </summary>
		Friend text As CharSequence

		''' <summary>
		''' Matcher state used by the last node. NOANCHOR is used when a
		''' match does not have to consume all of the input. ENDANCHOR is
		''' the mode used for matching all the input.
		''' </summary>
		Friend Const ENDANCHOR As Integer = 1
		Friend Const NOANCHOR As Integer = 0
		Friend acceptMode As Integer = NOANCHOR

		''' <summary>
		''' The range of string that last matched the pattern. If the last
		''' match failed then first is -1; last initially holds 0 then it
		''' holds the index of the end of the last match (which is where the
		''' next search starts).
		''' </summary>
		Friend first As Integer = -1, last As Integer = 0

		''' <summary>
		''' The end index of what matched in the last match operation.
		''' </summary>
		Friend oldLast As Integer = -1

		''' <summary>
		''' The index of the last position appended in a substitution.
		''' </summary>
		Friend lastAppendPosition As Integer = 0

		''' <summary>
		''' Storage used by nodes to tell what repetition they are on in
		''' a pattern, and where groups begin. The nodes themselves are stateless,
		''' so they rely on this field to hold state during a match.
		''' </summary>
		Friend locals As Integer()

		''' <summary>
		''' Boolean indicating whether or not more input could change
		''' the results of the last match.
		''' 
		''' If hitEnd is true, and a match was found, then more input
		''' might cause a different match to be found.
		''' If hitEnd is true and a match was not found, then more
		''' input could cause a match to be found.
		''' If hitEnd is false and a match was found, then more input
		''' will not change the match.
		''' If hitEnd is false and a match was not found, then more
		''' input will not cause a match to be found.
		''' </summary>
		Friend hitEnd_Renamed As Boolean

		''' <summary>
		''' Boolean indicating whether or not more input could change
		''' a positive match into a negative one.
		''' 
		''' If requireEnd is true, and a match was found, then more
		''' input could cause the match to be lost.
		''' If requireEnd is false and a match was found, then more
		''' input might change the match but the match won't be lost.
		''' If a match was not found, then requireEnd has no meaning.
		''' </summary>
		Friend requireEnd_Renamed As Boolean

		''' <summary>
		''' If transparentBounds is true then the boundaries of this
		''' matcher's region are transparent to lookahead, lookbehind,
		''' and boundary matching constructs that try to see beyond them.
		''' </summary>
		Friend transparentBounds As Boolean = False

		''' <summary>
		''' If anchoringBounds is true then the boundaries of this
		''' matcher's region match anchors such as ^ and $.
		''' </summary>
		Friend anchoringBounds As Boolean = True

		''' <summary>
		''' No default constructor.
		''' </summary>
		Friend Sub New()
		End Sub

		''' <summary>
		''' All matchers have the state used by Pattern during a match.
		''' </summary>
		Friend Sub New(ByVal parent As Pattern, ByVal text As CharSequence)
			Me.parentPattern = parent
			Me.text = text

			' Allocate state storage
			Dim parentGroupCount As Integer = System.Math.Max(parent.capturingGroupCount, 10)
			groups = New Integer(parentGroupCount * 2 - 1){}
			locals = New Integer(parent.localCount - 1){}

			' Put fields into initial states
			reset()
		End Sub

		''' <summary>
		''' Returns the pattern that is interpreted by this matcher.
		''' </summary>
		''' <returns>  The pattern for which this matcher was created </returns>
		Public Function pattern() As Pattern
			Return parentPattern
		End Function

		''' <summary>
		''' Returns the match state of this matcher as a <seealso cref="MatchResult"/>.
		''' The result is unaffected by subsequent operations performed upon this
		''' matcher.
		''' </summary>
		''' <returns>  a <code>MatchResult</code> with the state of this matcher
		''' @since 1.5 </returns>
		Public Function toMatchResult() As MatchResult
			Dim result As New Matcher(Me.parentPattern, text.ToString())
			result.first = Me.first
			result.last = Me.last
			result.groups = Me.groups.clone()
			Return result
		End Function

		''' <summary>
		''' Changes the <tt>Pattern</tt> that this <tt>Matcher</tt> uses to
		''' find matches with.
		'''  
		''' <p> This method causes this matcher to lose information
		''' about the groups of the last match that occurred. The
		''' matcher's position in the input is maintained and its
		''' last append position is unaffected.</p>
		''' </summary>
		''' <param name="newPattern">
		'''         The new pattern used by this matcher </param>
		''' <returns>  This matcher </returns>
		''' <exception cref="IllegalArgumentException">
		'''          If newPattern is <tt>null</tt>
		''' @since 1.5 </exception>
		Public Function usePattern(ByVal newPattern As Pattern) As Matcher
			If newPattern Is Nothing Then Throw New IllegalArgumentException("Pattern cannot be null")
			parentPattern = newPattern

			' Reallocate state storage
			Dim parentGroupCount As Integer = System.Math.Max(newPattern.capturingGroupCount, 10)
			groups = New Integer(parentGroupCount * 2 - 1){}
			locals = New Integer(newPattern.localCount - 1){}
			For i As Integer = 0 To groups.Length - 1
				groups(i) = -1
			Next i
			For i As Integer = 0 To locals.Length - 1
				locals(i) = -1
			Next i
			Return Me
		End Function

		''' <summary>
		''' Resets this matcher.
		''' 
		''' <p> Resetting a matcher discards all of its explicit state information
		''' and sets its append position to zero. The matcher's region is set to the
		''' default region, which is its entire character sequence. The anchoring
		''' and transparency of this matcher's region boundaries are unaffected.
		''' </summary>
		''' <returns>  This matcher </returns>
		Public Function reset() As Matcher
			first = -1
			last = 0
			oldLast = -1
			For i As Integer = 0 To groups.Length - 1
				groups(i) = -1
			Next i
			For i As Integer = 0 To locals.Length - 1
				locals(i) = -1
			Next i
			lastAppendPosition = 0
			[from] = 0
			[to] = textLength
			Return Me
		End Function

		''' <summary>
		''' Resets this matcher with a new input sequence.
		''' 
		''' <p> Resetting a matcher discards all of its explicit state information
		''' and sets its append position to zero.  The matcher's region is set to
		''' the default region, which is its entire character sequence.  The
		''' anchoring and transparency of this matcher's region boundaries are
		''' unaffected.
		''' </summary>
		''' <param name="input">
		'''         The new input character sequence
		''' </param>
		''' <returns>  This matcher </returns>
		Public Function reset(ByVal input As CharSequence) As Matcher
			text = input
			Return reset()
		End Function

		''' <summary>
		''' Returns the start index of the previous match.
		''' </summary>
		''' <returns>  The index of the first character matched
		''' </returns>
		''' <exception cref="IllegalStateException">
		'''          If no match has yet been attempted,
		'''          or if the previous match operation failed </exception>
		Public Function start() As Integer Implements MatchResult.start
			If first < 0 Then Throw New IllegalStateException("No match available")
			Return first
		End Function

		''' <summary>
		''' Returns the start index of the subsequence captured by the given group
		''' during the previous match operation.
		''' 
		''' <p> <a href="Pattern.html#cg">Capturing groups</a> are indexed from left
		''' to right, starting at one.  Group zero denotes the entire pattern, so
		''' the expression <i>m.</i><tt>start(0)</tt> is equivalent to
		''' <i>m.</i><tt>start()</tt>.  </p>
		''' </summary>
		''' <param name="group">
		'''         The index of a capturing group in this matcher's pattern
		''' </param>
		''' <returns>  The index of the first character captured by the group,
		'''          or <tt>-1</tt> if the match was successful but the group
		'''          itself did not match anything
		''' </returns>
		''' <exception cref="IllegalStateException">
		'''          If no match has yet been attempted,
		'''          or if the previous match operation failed
		''' </exception>
		''' <exception cref="IndexOutOfBoundsException">
		'''          If there is no capturing group in the pattern
		'''          with the given index </exception>
		Public Function start(ByVal group As Integer) As Integer Implements MatchResult.start
			If first < 0 Then Throw New IllegalStateException("No match available")
			If group < 0 OrElse group > groupCount() Then Throw New IndexOutOfBoundsException("No group " & group)
			Return groups(group * 2)
		End Function

		''' <summary>
		''' Returns the start index of the subsequence captured by the given
		''' <a href="Pattern.html#groupname">named-capturing group</a> during the
		''' previous match operation.
		''' </summary>
		''' <param name="name">
		'''         The name of a named-capturing group in this matcher's pattern
		''' </param>
		''' <returns>  The index of the first character captured by the group,
		'''          or {@code -1} if the match was successful but the group
		'''          itself did not match anything
		''' </returns>
		''' <exception cref="IllegalStateException">
		'''          If no match has yet been attempted,
		'''          or if the previous match operation failed
		''' </exception>
		''' <exception cref="IllegalArgumentException">
		'''          If there is no capturing group in the pattern
		'''          with the given name
		''' @since 1.8 </exception>
		Public Function start(ByVal name As String) As Integer
			Return groups(getMatchedGroupIndex(name) * 2)
		End Function

		''' <summary>
		''' Returns the offset after the last character matched.
		''' </summary>
		''' <returns>  The offset after the last character matched
		''' </returns>
		''' <exception cref="IllegalStateException">
		'''          If no match has yet been attempted,
		'''          or if the previous match operation failed </exception>
		Public Function [end]() As Integer Implements MatchResult.end
			If first < 0 Then Throw New IllegalStateException("No match available")
			Return last
		End Function

		''' <summary>
		''' Returns the offset after the last character of the subsequence
		''' captured by the given group during the previous match operation.
		''' 
		''' <p> <a href="Pattern.html#cg">Capturing groups</a> are indexed from left
		''' to right, starting at one.  Group zero denotes the entire pattern, so
		''' the expression <i>m.</i><tt>end(0)</tt> is equivalent to
		''' <i>m.</i><tt>end()</tt>.  </p>
		''' </summary>
		''' <param name="group">
		'''         The index of a capturing group in this matcher's pattern
		''' </param>
		''' <returns>  The offset after the last character captured by the group,
		'''          or <tt>-1</tt> if the match was successful
		'''          but the group itself did not match anything
		''' </returns>
		''' <exception cref="IllegalStateException">
		'''          If no match has yet been attempted,
		'''          or if the previous match operation failed
		''' </exception>
		''' <exception cref="IndexOutOfBoundsException">
		'''          If there is no capturing group in the pattern
		'''          with the given index </exception>
		Public Function [end](ByVal group As Integer) As Integer Implements MatchResult.end
			If first < 0 Then Throw New IllegalStateException("No match available")
			If group < 0 OrElse group > groupCount() Then Throw New IndexOutOfBoundsException("No group " & group)
			Return groups(group * 2 + 1)
		End Function

		''' <summary>
		''' Returns the offset after the last character of the subsequence
		''' captured by the given <a href="Pattern.html#groupname">named-capturing
		''' group</a> during the previous match operation.
		''' </summary>
		''' <param name="name">
		'''         The name of a named-capturing group in this matcher's pattern
		''' </param>
		''' <returns>  The offset after the last character captured by the group,
		'''          or {@code -1} if the match was successful
		'''          but the group itself did not match anything
		''' </returns>
		''' <exception cref="IllegalStateException">
		'''          If no match has yet been attempted,
		'''          or if the previous match operation failed
		''' </exception>
		''' <exception cref="IllegalArgumentException">
		'''          If there is no capturing group in the pattern
		'''          with the given name
		''' @since 1.8 </exception>
		Public Function [end](ByVal name As String) As Integer
			Return groups(getMatchedGroupIndex(name) * 2 + 1)
		End Function

		''' <summary>
		''' Returns the input subsequence matched by the previous match.
		''' 
		''' <p> For a matcher <i>m</i> with input sequence <i>s</i>,
		''' the expressions <i>m.</i><tt>group()</tt> and
		''' <i>s.</i><tt>substring(</tt><i>m.</i><tt>start(),</tt>&nbsp;<i>m.</i><tt>end())</tt>
		''' are equivalent.  </p>
		''' 
		''' <p> Note that some patterns, for example <tt>a*</tt>, match the empty
		''' string.  This method will return the empty string when the pattern
		''' successfully matches the empty string in the input.  </p>
		''' </summary>
		''' <returns> The (possibly empty) subsequence matched by the previous match,
		'''         in string form
		''' </returns>
		''' <exception cref="IllegalStateException">
		'''          If no match has yet been attempted,
		'''          or if the previous match operation failed </exception>
		Public Function group() As String Implements MatchResult.group
			Return group(0)
		End Function

		''' <summary>
		''' Returns the input subsequence captured by the given group during the
		''' previous match operation.
		''' 
		''' <p> For a matcher <i>m</i>, input sequence <i>s</i>, and group index
		''' <i>g</i>, the expressions <i>m.</i><tt>group(</tt><i>g</i><tt>)</tt> and
		''' <i>s.</i><tt>substring(</tt><i>m.</i><tt>start(</tt><i>g</i><tt>),</tt>&nbsp;<i>m.</i><tt>end(</tt><i>g</i><tt>))</tt>
		''' are equivalent.  </p>
		''' 
		''' <p> <a href="Pattern.html#cg">Capturing groups</a> are indexed from left
		''' to right, starting at one.  Group zero denotes the entire pattern, so
		''' the expression <tt>m.group(0)</tt> is equivalent to <tt>m.group()</tt>.
		''' </p>
		''' 
		''' <p> If the match was successful but the group specified failed to match
		''' any part of the input sequence, then <tt>null</tt> is returned. Note
		''' that some groups, for example <tt>(a*)</tt>, match the empty string.
		''' This method will return the empty string when such a group successfully
		''' matches the empty string in the input.  </p>
		''' </summary>
		''' <param name="group">
		'''         The index of a capturing group in this matcher's pattern
		''' </param>
		''' <returns>  The (possibly empty) subsequence captured by the group
		'''          during the previous match, or <tt>null</tt> if the group
		'''          failed to match part of the input
		''' </returns>
		''' <exception cref="IllegalStateException">
		'''          If no match has yet been attempted,
		'''          or if the previous match operation failed
		''' </exception>
		''' <exception cref="IndexOutOfBoundsException">
		'''          If there is no capturing group in the pattern
		'''          with the given index </exception>
		Public Function group(ByVal group_Renamed As Integer) As String Implements MatchResult.group
			If first < 0 Then Throw New IllegalStateException("No match found")
			If group_Renamed < 0 OrElse group_Renamed > groupCount() Then Throw New IndexOutOfBoundsException("No group " & group_Renamed)
			If (groups(group_Renamed*2) = -1) OrElse (groups(group_Renamed*2+1) = -1) Then Return Nothing
			Return getSubSequence(groups(group_Renamed * 2), groups(group_Renamed * 2 + 1)).ToString()
		End Function

		''' <summary>
		''' Returns the input subsequence captured by the given
		''' <a href="Pattern.html#groupname">named-capturing group</a> during the previous
		''' match operation.
		''' 
		''' <p> If the match was successful but the group specified failed to match
		''' any part of the input sequence, then <tt>null</tt> is returned. Note
		''' that some groups, for example <tt>(a*)</tt>, match the empty string.
		''' This method will return the empty string when such a group successfully
		''' matches the empty string in the input.  </p>
		''' </summary>
		''' <param name="name">
		'''         The name of a named-capturing group in this matcher's pattern
		''' </param>
		''' <returns>  The (possibly empty) subsequence captured by the named group
		'''          during the previous match, or <tt>null</tt> if the group
		'''          failed to match part of the input
		''' </returns>
		''' <exception cref="IllegalStateException">
		'''          If no match has yet been attempted,
		'''          or if the previous match operation failed
		''' </exception>
		''' <exception cref="IllegalArgumentException">
		'''          If there is no capturing group in the pattern
		'''          with the given name
		''' @since 1.7 </exception>
		Public Function group(ByVal name As String) As String
			Dim group_Renamed As Integer = getMatchedGroupIndex(name)
			If (groups(group_Renamed*2) = -1) OrElse (groups(group_Renamed*2+1) = -1) Then Return Nothing
			Return getSubSequence(groups(group_Renamed * 2), groups(group_Renamed * 2 + 1)).ToString()
		End Function

		''' <summary>
		''' Returns the number of capturing groups in this matcher's pattern.
		''' 
		''' <p> Group zero denotes the entire pattern by convention. It is not
		''' included in this count.
		''' 
		''' <p> Any non-negative integer smaller than or equal to the value
		''' returned by this method is guaranteed to be a valid group index for
		''' this matcher.  </p>
		''' </summary>
		''' <returns> The number of capturing groups in this matcher's pattern </returns>
		Public Function groupCount() As Integer Implements MatchResult.groupCount
			Return parentPattern.capturingGroupCount - 1
		End Function

		''' <summary>
		''' Attempts to match the entire region against the pattern.
		''' 
		''' <p> If the match succeeds then more information can be obtained via the
		''' <tt>start</tt>, <tt>end</tt>, and <tt>group</tt> methods.  </p>
		''' </summary>
		''' <returns>  <tt>true</tt> if, and only if, the entire region sequence
		'''          matches this matcher's pattern </returns>
		Public Function matches() As Boolean
			Return match([from], ENDANCHOR)
		End Function

		''' <summary>
		''' Attempts to find the next subsequence of the input sequence that matches
		''' the pattern.
		''' 
		''' <p> This method starts at the beginning of this matcher's region, or, if
		''' a previous invocation of the method was successful and the matcher has
		''' not since been reset, at the first character not matched by the previous
		''' match.
		''' 
		''' <p> If the match succeeds then more information can be obtained via the
		''' <tt>start</tt>, <tt>end</tt>, and <tt>group</tt> methods.  </p>
		''' </summary>
		''' <returns>  <tt>true</tt> if, and only if, a subsequence of the input
		'''          sequence matches this matcher's pattern </returns>
		Public Function find() As Boolean
			Dim nextSearchIndex As Integer = last
			If nextSearchIndex = first Then nextSearchIndex += 1

			' If next search starts before region, start it at region
			If nextSearchIndex < [from] Then nextSearchIndex = [from]

			' If next search starts beyond region then it fails
			If nextSearchIndex > [to] Then
				For i As Integer = 0 To groups.Length - 1
					groups(i) = -1
				Next i
				Return False
			End If
			Return search(nextSearchIndex)
		End Function

		''' <summary>
		''' Resets this matcher and then attempts to find the next subsequence of
		''' the input sequence that matches the pattern, starting at the specified
		''' index.
		''' 
		''' <p> If the match succeeds then more information can be obtained via the
		''' <tt>start</tt>, <tt>end</tt>, and <tt>group</tt> methods, and subsequent
		''' invocations of the <seealso cref="#find()"/> method will start at the first
		''' character not matched by this match.  </p>
		''' </summary>
		''' <param name="start"> the index to start searching for a match </param>
		''' <exception cref="IndexOutOfBoundsException">
		'''          If start is less than zero or if start is greater than the
		'''          length of the input sequence.
		''' </exception>
		''' <returns>  <tt>true</tt> if, and only if, a subsequence of the input
		'''          sequence starting at the given index matches this matcher's
		'''          pattern </returns>
		Public Function find(ByVal start As Integer) As Boolean
			Dim limit As Integer = textLength
			If (start < 0) OrElse (start > limit) Then Throw New IndexOutOfBoundsException("Illegal start index")
			reset()
			Return search(start)
		End Function

		''' <summary>
		''' Attempts to match the input sequence, starting at the beginning of the
		''' region, against the pattern.
		''' 
		''' <p> Like the <seealso cref="#matches matches"/> method, this method always starts
		''' at the beginning of the region; unlike that method, it does not
		''' require that the entire region be matched.
		''' 
		''' <p> If the match succeeds then more information can be obtained via the
		''' <tt>start</tt>, <tt>end</tt>, and <tt>group</tt> methods.  </p>
		''' </summary>
		''' <returns>  <tt>true</tt> if, and only if, a prefix of the input
		'''          sequence matches this matcher's pattern </returns>
		Public Function lookingAt() As Boolean
			Return match([from], NOANCHOR)
		End Function

		''' <summary>
		''' Returns a literal replacement <code>String</code> for the specified
		''' <code>String</code>.
		''' 
		''' This method produces a <code>String</code> that will work
		''' as a literal replacement <code>s</code> in the
		''' <code>appendReplacement</code> method of the <seealso cref="Matcher"/> class.
		''' The <code>String</code> produced will match the sequence of characters
		''' in <code>s</code> treated as a literal sequence. Slashes ('\') and
		''' dollar signs ('$') will be given no special meaning.
		''' </summary>
		''' <param name="s"> The string to be literalized </param>
		''' <returns>  A literal string replacement
		''' @since 1.5 </returns>
		Public Shared Function quoteReplacement(ByVal s As String) As String
			If (s.IndexOf("\"c) = -1) AndAlso (s.IndexOf("$"c) = -1) Then Return s
			Dim sb As New StringBuilder
			For i As Integer = 0 To s.length() - 1
				Dim c As Char = s.Chars(i)
				If c = "\"c OrElse c = "$"c Then sb.append("\"c)
				sb.append(c)
			Next i
			Return sb.ToString()
		End Function

		''' <summary>
		''' Implements a non-terminal append-and-replace step.
		''' 
		''' <p> This method performs the following actions: </p>
		''' 
		''' <ol>
		''' 
		'''   <li><p> It reads characters from the input sequence, starting at the
		'''   append position, and appends them to the given string buffer.  It
		'''   stops after reading the last character preceding the previous match,
		'''   that is, the character at index {@link
		'''   #start()}&nbsp;<tt>-</tt>&nbsp;<tt>1</tt>.  </p></li>
		''' 
		'''   <li><p> It appends the given replacement string to the string buffer.
		'''   </p></li>
		''' 
		'''   <li><p> It sets the append position of this matcher to the index of
		'''   the last character matched, plus one, that is, to <seealso cref="#end()"/>.
		'''   </p></li>
		''' 
		''' </ol>
		''' 
		''' <p> The replacement string may contain references to subsequences
		''' captured during the previous match: Each occurrence of
		''' <tt>${</tt><i>name</i><tt>}</tt> or <tt>$</tt><i>g</i>
		''' will be replaced by the result of evaluating the corresponding
		''' <seealso cref="#group(String) group(name)"/> or <seealso cref="#group(int) group(g)"/>
		''' respectively. For  <tt>$</tt><i>g</i>,
		''' the first number after the <tt>$</tt> is always treated as part of
		''' the group reference. Subsequent numbers are incorporated into g if
		''' they would form a legal group reference. Only the numerals '0'
		''' through '9' are considered as potential components of the group
		''' reference. If the second group matched the string <tt>"foo"</tt>, for
		''' example, then passing the replacement string <tt>"$2bar"</tt> would
		''' cause <tt>"foobar"</tt> to be appended to the string buffer. A dollar
		''' sign (<tt>$</tt>) may be included as a literal in the replacement
		''' string by preceding it with a backslash (<tt>\$</tt>).
		''' 
		''' <p> Note that backslashes (<tt>\</tt>) and dollar signs (<tt>$</tt>) in
		''' the replacement string may cause the results to be different than if it
		''' were being treated as a literal replacement string. Dollar signs may be
		''' treated as references to captured subsequences as described above, and
		''' backslashes are used to escape literal characters in the replacement
		''' string.
		''' 
		''' <p> This method is intended to be used in a loop together with the
		''' <seealso cref="#appendTail appendTail"/> and <seealso cref="#find find"/> methods.  The
		''' following code, for example, writes <tt>one dog two dogs in the
		''' yard</tt> to the standard-output stream: </p>
		''' 
		''' <blockquote><pre>
		''' Pattern p = Pattern.compile("cat");
		''' Matcher m = p.matcher("one cat two cats in the yard");
		''' StringBuffer sb = new StringBuffer();
		''' while (m.find()) {
		'''     m.appendReplacement(sb, "dog");
		''' }
		''' m.appendTail(sb);
		''' System.out.println(sb.toString());</pre></blockquote>
		''' </summary>
		''' <param name="sb">
		'''         The target string buffer
		''' </param>
		''' <param name="replacement">
		'''         The replacement string
		''' </param>
		''' <returns>  This matcher
		''' </returns>
		''' <exception cref="IllegalStateException">
		'''          If no match has yet been attempted,
		'''          or if the previous match operation failed
		''' </exception>
		''' <exception cref="IllegalArgumentException">
		'''          If the replacement string refers to a named-capturing
		'''          group that does not exist in the pattern
		''' </exception>
		''' <exception cref="IndexOutOfBoundsException">
		'''          If the replacement string refers to a capturing group
		'''          that does not exist in the pattern </exception>
		Public Function appendReplacement(ByVal sb As StringBuffer, ByVal replacement As String) As Matcher

			' If no match, return error
			If first < 0 Then Throw New IllegalStateException("No match available")

			' Process substitution string to replace group references with groups
			Dim cursor As Integer = 0
			Dim result As New StringBuilder

			Do While cursor < replacement.length()
				Dim nextChar As Char = replacement.Chars(cursor)
				If nextChar = "\"c Then
					cursor += 1
					If cursor = replacement.length() Then Throw New IllegalArgumentException("character to be escaped is missing")
					nextChar = replacement.Chars(cursor)
					result.append(nextChar)
					cursor += 1
				ElseIf nextChar = "$"c Then
					' Skip past $
					cursor += 1
					' Throw IAE if this "$" is the last character in replacement
					If cursor = replacement.length() Then Throw New IllegalArgumentException("Illegal group reference: group index is missing")
					nextChar = replacement.Chars(cursor)
					Dim refNum As Integer = -1
					If nextChar = "{"c Then
						cursor += 1
						Dim gsb As New StringBuilder
						Do While cursor < replacement.length()
							nextChar = replacement.Chars(cursor)
							If ASCII.isLower(nextChar) OrElse ASCII.isUpper(nextChar) OrElse ASCII.isDigit(nextChar) Then
								gsb.append(nextChar)
								cursor += 1
							Else
								Exit Do
							End If
						Loop
						If gsb.length() = 0 Then Throw New IllegalArgumentException("named capturing group has 0 length name")
						If nextChar <> "}"c Then Throw New IllegalArgumentException("named capturing group is missing trailing '}'")
						Dim gname As String = gsb.ToString()
						If ASCII.isDigit(gname.Chars(0)) Then Throw New IllegalArgumentException("capturing group name {" & gname & "} starts with digit character")
						If Not parentPattern.namedGroups().containsKey(gname) Then Throw New IllegalArgumentException("No group with name {" & gname & "}")
						refNum = parentPattern.namedGroups().get(gname)
						cursor += 1
					Else
						' The first number is always a group
						refNum = AscW(nextChar) - AscW("0"c)
						If (refNum < 0) OrElse (refNum > 9) Then Throw New IllegalArgumentException("Illegal group reference")
						cursor += 1
						' Capture the largest legal group string
						Dim done As Boolean = False
						Do While Not done
							If cursor >= replacement.length() Then Exit Do
							Dim nextDigit As Integer = AscW(AscW(replacement.Chars(cursor)) - AscW("0"c))
							If (nextDigit < 0) OrElse (nextDigit > 9) Then ' not a number Exit Do
							Dim newRefNum As Integer = (refNum * 10) + nextDigit
							If groupCount() < newRefNum Then
								done = True
							Else
								refNum = newRefNum
								cursor += 1
							End If
						Loop
					End If
					' Append group
					If start(refNum) <> -1 AndAlso [end](refNum) <> -1 Then result.append(text, start(refNum), [end](refNum))
				Else
					result.append(nextChar)
					cursor += 1
				End If
			Loop
			' Append the intervening text
			sb.append(text, lastAppendPosition, first)
			' Append the match substitution
			sb.append(result)

			lastAppendPosition = last
			Return Me
		End Function

		''' <summary>
		''' Implements a terminal append-and-replace step.
		''' 
		''' <p> This method reads characters from the input sequence, starting at
		''' the append position, and appends them to the given string buffer.  It is
		''' intended to be invoked after one or more invocations of the {@link
		''' #appendReplacement appendReplacement} method in order to copy the
		''' remainder of the input sequence.  </p>
		''' </summary>
		''' <param name="sb">
		'''         The target string buffer
		''' </param>
		''' <returns>  The target string buffer </returns>
		Public Function appendTail(ByVal sb As StringBuffer) As StringBuffer
			sb.append(text, lastAppendPosition, textLength)
			Return sb
		End Function

		''' <summary>
		''' Replaces every subsequence of the input sequence that matches the
		''' pattern with the given replacement string.
		''' 
		''' <p> This method first resets this matcher.  It then scans the input
		''' sequence looking for matches of the pattern.  Characters that are not
		''' part of any match are appended directly to the result string; each match
		''' is replaced in the result by the replacement string.  The replacement
		''' string may contain references to captured subsequences as in the {@link
		''' #appendReplacement appendReplacement} method.
		''' 
		''' <p> Note that backslashes (<tt>\</tt>) and dollar signs (<tt>$</tt>) in
		''' the replacement string may cause the results to be different than if it
		''' were being treated as a literal replacement string. Dollar signs may be
		''' treated as references to captured subsequences as described above, and
		''' backslashes are used to escape literal characters in the replacement
		''' string.
		''' 
		''' <p> Given the regular expression <tt>a*b</tt>, the input
		''' <tt>"aabfooaabfooabfoob"</tt>, and the replacement string
		''' <tt>"-"</tt>, an invocation of this method on a matcher for that
		''' expression would yield the string <tt>"-foo-foo-foo-"</tt>.
		''' 
		''' <p> Invoking this method changes this matcher's state.  If the matcher
		''' is to be used in further matching operations then it should first be
		''' reset.  </p>
		''' </summary>
		''' <param name="replacement">
		'''         The replacement string
		''' </param>
		''' <returns>  The string constructed by replacing each matching subsequence
		'''          by the replacement string, substituting captured subsequences
		'''          as needed </returns>
		Public Function replaceAll(ByVal replacement As String) As String
			reset()
			Dim result As Boolean = find()
			If result Then
				Dim sb As New StringBuffer
				Do
					appendReplacement(sb, replacement)
					result = find()
				Loop While result
				appendTail(sb)
				Return sb.ToString()
			End If
			Return text.ToString()
		End Function

		''' <summary>
		''' Replaces the first subsequence of the input sequence that matches the
		''' pattern with the given replacement string.
		''' 
		''' <p> This method first resets this matcher.  It then scans the input
		''' sequence looking for a match of the pattern.  Characters that are not
		''' part of the match are appended directly to the result string; the match
		''' is replaced in the result by the replacement string.  The replacement
		''' string may contain references to captured subsequences as in the {@link
		''' #appendReplacement appendReplacement} method.
		''' 
		''' <p>Note that backslashes (<tt>\</tt>) and dollar signs (<tt>$</tt>) in
		''' the replacement string may cause the results to be different than if it
		''' were being treated as a literal replacement string. Dollar signs may be
		''' treated as references to captured subsequences as described above, and
		''' backslashes are used to escape literal characters in the replacement
		''' string.
		''' 
		''' <p> Given the regular expression <tt>dog</tt>, the input
		''' <tt>"zzzdogzzzdogzzz"</tt>, and the replacement string
		''' <tt>"cat"</tt>, an invocation of this method on a matcher for that
		''' expression would yield the string <tt>"zzzcatzzzdogzzz"</tt>.  </p>
		''' 
		''' <p> Invoking this method changes this matcher's state.  If the matcher
		''' is to be used in further matching operations then it should first be
		''' reset.  </p>
		''' </summary>
		''' <param name="replacement">
		'''         The replacement string </param>
		''' <returns>  The string constructed by replacing the first matching
		'''          subsequence by the replacement string, substituting captured
		'''          subsequences as needed </returns>
		Public Function replaceFirst(ByVal replacement As String) As String
			If replacement Is Nothing Then Throw New NullPointerException("replacement")
			reset()
			If Not find() Then Return text.ToString()
			Dim sb As New StringBuffer
			appendReplacement(sb, replacement)
			appendTail(sb)
			Return sb.ToString()
		End Function

		''' <summary>
		''' Sets the limits of this matcher's region. The region is the part of the
		''' input sequence that will be searched to find a match. Invoking this
		''' method resets the matcher, and then sets the region to start at the
		''' index specified by the <code>start</code> parameter and end at the
		''' index specified by the <code>end</code> parameter.
		''' 
		''' <p>Depending on the transparency and anchoring being used (see
		''' <seealso cref="#useTransparentBounds useTransparentBounds"/> and
		''' <seealso cref="#useAnchoringBounds useAnchoringBounds"/>), certain constructs such
		''' as anchors may behave differently at or around the boundaries of the
		''' region.
		''' </summary>
		''' <param name="start">
		'''         The index to start searching at (inclusive) </param>
		''' <param name="end">
		'''         The index to end searching at (exclusive) </param>
		''' <exception cref="IndexOutOfBoundsException">
		'''          If start or end is less than zero, if
		'''          start is greater than the length of the input sequence, if
		'''          end is greater than the length of the input sequence, or if
		'''          start is greater than end. </exception>
		''' <returns>  this matcher
		''' @since 1.5 </returns>
		Public Function region(ByVal start As Integer, ByVal [end] As Integer) As Matcher
			If (start < 0) OrElse (start > textLength) Then Throw New IndexOutOfBoundsException("start")
			If ([end] < 0) OrElse ([end] > textLength) Then Throw New IndexOutOfBoundsException("end")
			If start > [end] Then Throw New IndexOutOfBoundsException("start > end")
			reset()
			[from] = start
			[to] = [end]
			Return Me
		End Function

		''' <summary>
		''' Reports the start index of this matcher's region. The
		''' searches this matcher conducts are limited to finding matches
		''' within <seealso cref="#regionStart regionStart"/> (inclusive) and
		''' <seealso cref="#regionEnd regionEnd"/> (exclusive).
		''' </summary>
		''' <returns>  The starting point of this matcher's region
		''' @since 1.5 </returns>
		Public Function regionStart() As Integer
			Return [from]
		End Function

		''' <summary>
		''' Reports the end index (exclusive) of this matcher's region.
		''' The searches this matcher conducts are limited to finding matches
		''' within <seealso cref="#regionStart regionStart"/> (inclusive) and
		''' <seealso cref="#regionEnd regionEnd"/> (exclusive).
		''' </summary>
		''' <returns>  the ending point of this matcher's region
		''' @since 1.5 </returns>
		Public Function regionEnd() As Integer
			Return [to]
		End Function

		''' <summary>
		''' Queries the transparency of region bounds for this matcher.
		''' 
		''' <p> This method returns <tt>true</tt> if this matcher uses
		''' <i>transparent</i> bounds, <tt>false</tt> if it uses <i>opaque</i>
		''' bounds.
		''' 
		''' <p> See <seealso cref="#useTransparentBounds useTransparentBounds"/> for a
		''' description of transparent and opaque bounds.
		''' 
		''' <p> By default, a matcher uses opaque region boundaries.
		''' </summary>
		''' <returns> <tt>true</tt> iff this matcher is using transparent bounds,
		'''         <tt>false</tt> otherwise. </returns>
		''' <seealso cref= java.util.regex.Matcher#useTransparentBounds(boolean)
		''' @since 1.5 </seealso>
		Public Function hasTransparentBounds() As Boolean
			Return transparentBounds
		End Function

		''' <summary>
		''' Sets the transparency of region bounds for this matcher.
		''' 
		''' <p> Invoking this method with an argument of <tt>true</tt> will set this
		''' matcher to use <i>transparent</i> bounds. If the boolean
		''' argument is <tt>false</tt>, then <i>opaque</i> bounds will be used.
		''' 
		''' <p> Using transparent bounds, the boundaries of this
		''' matcher's region are transparent to lookahead, lookbehind,
		''' and boundary matching constructs. Those constructs can see beyond the
		''' boundaries of the region to see if a match is appropriate.
		''' 
		''' <p> Using opaque bounds, the boundaries of this matcher's
		''' region are opaque to lookahead, lookbehind, and boundary matching
		''' constructs that may try to see beyond them. Those constructs cannot
		''' look past the boundaries so they will fail to match anything outside
		''' of the region.
		''' 
		''' <p> By default, a matcher uses opaque bounds.
		''' </summary>
		''' <param name="b"> a boolean indicating whether to use opaque or transparent
		'''         regions </param>
		''' <returns> this matcher </returns>
		''' <seealso cref= java.util.regex.Matcher#hasTransparentBounds
		''' @since 1.5 </seealso>
		Public Function useTransparentBounds(ByVal b As Boolean) As Matcher
			transparentBounds = b
			Return Me
		End Function

		''' <summary>
		''' Queries the anchoring of region bounds for this matcher.
		''' 
		''' <p> This method returns <tt>true</tt> if this matcher uses
		''' <i>anchoring</i> bounds, <tt>false</tt> otherwise.
		''' 
		''' <p> See <seealso cref="#useAnchoringBounds useAnchoringBounds"/> for a
		''' description of anchoring bounds.
		''' 
		''' <p> By default, a matcher uses anchoring region boundaries.
		''' </summary>
		''' <returns> <tt>true</tt> iff this matcher is using anchoring bounds,
		'''         <tt>false</tt> otherwise. </returns>
		''' <seealso cref= java.util.regex.Matcher#useAnchoringBounds(boolean)
		''' @since 1.5 </seealso>
		Public Function hasAnchoringBounds() As Boolean
			Return anchoringBounds
		End Function

		''' <summary>
		''' Sets the anchoring of region bounds for this matcher.
		''' 
		''' <p> Invoking this method with an argument of <tt>true</tt> will set this
		''' matcher to use <i>anchoring</i> bounds. If the boolean
		''' argument is <tt>false</tt>, then <i>non-anchoring</i> bounds will be
		''' used.
		''' 
		''' <p> Using anchoring bounds, the boundaries of this
		''' matcher's region match anchors such as ^ and $.
		''' 
		''' <p> Without anchoring bounds, the boundaries of this
		''' matcher's region will not match anchors such as ^ and $.
		''' 
		''' <p> By default, a matcher uses anchoring region boundaries.
		''' </summary>
		''' <param name="b"> a boolean indicating whether or not to use anchoring bounds. </param>
		''' <returns> this matcher </returns>
		''' <seealso cref= java.util.regex.Matcher#hasAnchoringBounds
		''' @since 1.5 </seealso>
		Public Function useAnchoringBounds(ByVal b As Boolean) As Matcher
			anchoringBounds = b
			Return Me
		End Function

		''' <summary>
		''' <p>Returns the string representation of this matcher. The
		''' string representation of a <code>Matcher</code> contains information
		''' that may be useful for debugging. The exact format is unspecified.
		''' </summary>
		''' <returns>  The string representation of this matcher
		''' @since 1.5 </returns>
		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder
			sb.append("java.util.regex.Matcher")
			sb.append("[pattern=" & pattern())
			sb.append(" region=")
			sb.append(regionStart() & "," & regionEnd())
			sb.append(" lastmatch=")
			If (first >= 0) AndAlso (group() IsNot Nothing) Then sb.append(group())
			sb.append("]")
			Return sb.ToString()
		End Function

		''' <summary>
		''' <p>Returns true if the end of input was hit by the search engine in
		''' the last match operation performed by this matcher.
		''' 
		''' <p>When this method returns true, then it is possible that more input
		''' would have changed the result of the last search.
		''' </summary>
		''' <returns>  true iff the end of input was hit in the last match; false
		'''          otherwise
		''' @since 1.5 </returns>
		Public Function hitEnd() As Boolean
			Return hitEnd_Renamed
		End Function

		''' <summary>
		''' <p>Returns true if more input could change a positive match into a
		''' negative one.
		''' 
		''' <p>If this method returns true, and a match was found, then more
		''' input could cause the match to be lost. If this method returns false
		''' and a match was found, then more input might change the match but the
		''' match won't be lost. If a match was not found, then requireEnd has no
		''' meaning.
		''' </summary>
		''' <returns>  true iff more input could change a positive match into a
		'''          negative one.
		''' @since 1.5 </returns>
		Public Function requireEnd() As Boolean
			Return requireEnd_Renamed
		End Function

		''' <summary>
		''' Initiates a search to find a Pattern within the given bounds.
		''' The groups are filled with default values and the match of the root
		''' of the state machine is called. The state machine will hold the state
		''' of the match as it proceeds in this matcher.
		''' 
		''' Matcher.from is not set here, because it is the "hard" boundary
		''' of the start of the search which anchors will set to. The from param
		''' is the "soft" boundary of the start of the search, meaning that the
		''' regex tries to match at that index but ^ won't match there. Subsequent
		''' calls to the search methods start at a new "soft" boundary which is
		''' the end of the previous match.
		''' </summary>
		Friend Function search(ByVal [from] As Integer) As Boolean
			Me.hitEnd_Renamed = False
			Me.requireEnd_Renamed = False
			[from] = If([from] < 0, 0, [from])
			Me.first = [from]
			Me.oldLast = If(oldLast < 0, [from], oldLast)
			For i As Integer = 0 To groups.Length - 1
				groups(i) = -1
			Next i
			acceptMode = NOANCHOR
			Dim result As Boolean = parentPattern.root.match(Me, [from], text)
			If Not result Then Me.first = -1
			Me.oldLast = Me.last
			Return result
		End Function

		''' <summary>
		''' Initiates a search for an anchored match to a Pattern within the given
		''' bounds. The groups are filled with default values and the match of the
		''' root of the state machine is called. The state machine will hold the
		''' state of the match as it proceeds in this matcher.
		''' </summary>
		Friend Function match(ByVal [from] As Integer, ByVal anchor As Integer) As Boolean
			Me.hitEnd_Renamed = False
			Me.requireEnd_Renamed = False
			[from] = If([from] < 0, 0, [from])
			Me.first = [from]
			Me.oldLast = If(oldLast < 0, [from], oldLast)
			For i As Integer = 0 To groups.Length - 1
				groups(i) = -1
			Next i
			acceptMode = anchor
			Dim result As Boolean = parentPattern.matchRoot.match(Me, [from], text)
			If Not result Then Me.first = -1
			Me.oldLast = Me.last
			Return result
		End Function

		''' <summary>
		''' Returns the end index of the text.
		''' </summary>
		''' <returns> the index after the last character in the text </returns>
		Friend Property textLength As Integer
			Get
				Return text.length()
			End Get
		End Property

		''' <summary>
		''' Generates a String from this Matcher's input in the specified range.
		''' </summary>
		''' <param name="beginIndex">   the beginning index, inclusive </param>
		''' <param name="endIndex">     the ending index, exclusive </param>
		''' <returns> A String generated from this Matcher's input </returns>
		Friend Function getSubSequence(ByVal beginIndex As Integer, ByVal endIndex As Integer) As CharSequence
			Return text.subSequence(beginIndex, endIndex)
		End Function

		''' <summary>
		''' Returns this Matcher's input character at index i.
		''' </summary>
		''' <returns> A char from the specified index </returns>
		Friend Function charAt(ByVal i As Integer) As Char
			Return text.Chars(i)
		End Function

		''' <summary>
		''' Returns the group index of the matched capturing group.
		''' </summary>
		''' <returns> the index of the named-capturing group </returns>
		Friend Function getMatchedGroupIndex(ByVal name As String) As Integer
			java.util.Objects.requireNonNull(name, "Group name")
			If first < 0 Then Throw New IllegalStateException("No match found")
			If Not parentPattern.namedGroups().containsKey(name) Then Throw New IllegalArgumentException("No group with name <" & name & ">")
			Return parentPattern.namedGroups().get(name)
		End Function
	End Class

End Namespace