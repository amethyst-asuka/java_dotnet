'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.tools


	''' <summary>
	''' Interface for diagnostics from tools.  A diagnostic usually reports
	''' a problem at a specific position in a source file.  However, not
	''' all diagnostics are associated with a position or a file.
	''' 
	''' <p>A position is a zero-based character offset from the beginning of
	''' a file.  Negative values (except <seealso cref="#NOPOS"/>) are not valid
	''' positions.
	''' 
	''' <p>Line and column numbers begin at 1.  Negative values (except
	''' <seealso cref="#NOPOS"/>) and 0 are not valid line or column numbers.
	''' </summary>
	''' @param <S> the type of source object used by this diagnostic
	''' 
	''' @author Peter von der Ah&eacute;
	''' @author Jonathan Gibbons
	''' @since 1.6 </param>
	Public Interface Diagnostic(Of S)

		''' <summary>
		''' Kinds of diagnostics, for example, error or warning.
		''' 
		''' The kind of a diagnostic can be used to determine how the
		''' diagnostic should be presented to the user. For example,
		''' errors might be colored red or prefixed with the word "Error",
		''' while warnings might be colored yellow or prefixed with the
		''' word "Warning". There is no requirement that the Kind
		''' should imply any inherent semantic meaning to the message
		''' of the diagnostic: for example, a tool might provide an
		''' option to report all warnings as errors.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		enum Kind
	'	{
	'		''' <summary>
	'		''' Problem which prevents the tool's normal completion.
	'		''' </summary>
	'		ERROR,
	'		''' <summary>
	'		''' Problem which does not usually prevent the tool from
	'		''' completing normally.
	'		''' </summary>
	'		WARNING,
	'		''' <summary>
	'		''' Problem similar to a warning, but is mandated by the tool's
	'		''' specification.  For example, the Java&trade; Language
	'		''' Specification mandates warnings on certain
	'		''' unchecked operations and the use of deprecated methods.
	'		''' </summary>
	'		MANDATORY_WARNING,
	'		''' <summary>
	'		''' Informative message from the tool.
	'		''' </summary>
	'		NOTE,
	'		''' <summary>
	'		''' Diagnostic which does not fit within the other kinds.
	'		''' </summary>
	'		OTHER,
	'	}

		''' <summary>
		''' Used to signal that no position is available.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public final static long NOPOS = -1;

		''' <summary>
		''' Gets the kind of this diagnostic, for example, error or
		''' warning. </summary>
		''' <returns> the kind of this diagnostic </returns>
		ReadOnly Property kind As Kind

		''' <summary>
		''' Gets the source object associated with this diagnostic.
		''' </summary>
		''' <returns> the source object associated with this diagnostic.
		''' {@code null} if no source object is associated with the
		''' diagnostic. </returns>
		ReadOnly Property source As S

		''' <summary>
		''' Gets a character offset from the beginning of the source object
		''' associated with this diagnostic that indicates the location of
		''' the problem.  In addition, the following must be true:
		''' 
		''' <p>{@code getStartPostion() <= getPosition()}
		''' <p>{@code getPosition() <= getEndPosition()}
		''' </summary>
		''' <returns> character offset from beginning of source; {@link
		''' #NOPOS} if <seealso cref="#getSource()"/> would return {@code null} or if
		''' no location is suitable </returns>
		ReadOnly Property position As Long

		''' <summary>
		''' Gets the character offset from the beginning of the file
		''' associated with this diagnostic that indicates the start of the
		''' problem.
		''' </summary>
		''' <returns> offset from beginning of file; <seealso cref="#NOPOS"/> if and
		''' only if <seealso cref="#getPosition()"/> returns <seealso cref="#NOPOS"/> </returns>
		ReadOnly Property startPosition As Long

		''' <summary>
		''' Gets the character offset from the beginning of the file
		''' associated with this diagnostic that indicates the end of the
		''' problem.
		''' </summary>
		''' <returns> offset from beginning of file; <seealso cref="#NOPOS"/> if and
		''' only if <seealso cref="#getPosition()"/> returns <seealso cref="#NOPOS"/> </returns>
		ReadOnly Property endPosition As Long

		''' <summary>
		''' Gets the line number of the character offset returned by
		''' <seealso cref="#getPosition()"/>.
		''' </summary>
		''' <returns> a line number or <seealso cref="#NOPOS"/> if and only if {@link
		''' #getPosition()} returns <seealso cref="#NOPOS"/> </returns>
		ReadOnly Property lineNumber As Long

		''' <summary>
		''' Gets the column number of the character offset returned by
		''' <seealso cref="#getPosition()"/>.
		''' </summary>
		''' <returns> a column number or <seealso cref="#NOPOS"/> if and only if {@link
		''' #getPosition()} returns <seealso cref="#NOPOS"/> </returns>
		ReadOnly Property columnNumber As Long

		''' <summary>
		''' Gets a diagnostic code indicating the type of diagnostic.  The
		''' code is implementation-dependent and might be {@code null}.
		''' </summary>
		''' <returns> a diagnostic code </returns>
		ReadOnly Property code As String

		''' <summary>
		''' Gets a localized message for the given locale.  The actual
		''' message is implementation-dependent.  If the locale is {@code
		''' null} use the default locale.
		''' </summary>
		''' <param name="locale"> a locale; might be {@code null} </param>
		''' <returns> a localized message </returns>
		Function getMessage(ByVal locale As java.util.Locale) As String
	End Interface

End Namespace