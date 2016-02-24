Imports System
Imports System.Runtime.CompilerServices
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

Namespace java.util.logging

	''' <summary>
	''' The Level class defines a set of standard logging levels that
	''' can be used to control logging output.  The logging Level objects
	''' are ordered and are specified by ordered integers.  Enabling logging
	''' at a given level also enables logging at all higher levels.
	''' <p>
	''' Clients should normally use the predefined Level constants such
	''' as Level.SEVERE.
	''' <p>
	''' The levels in descending order are:
	''' <ul>
	''' <li>SEVERE (highest value)
	''' <li>WARNING
	''' <li>INFO
	''' <li>CONFIG
	''' <li>FINE
	''' <li>FINER
	''' <li>FINEST  (lowest value)
	''' </ul>
	''' In addition there is a level OFF that can be used to turn
	''' off logging, and a level ALL that can be used to enable
	''' logging of all messages.
	''' <p>
	''' It is possible for third parties to define additional logging
	''' levels by subclassing Level.  In such cases subclasses should
	''' take care to chose unique integer level values and to ensure that
	''' they maintain the Object uniqueness property across serialization
	''' by defining a suitable readResolve method.
	''' 
	''' @since 1.4
	''' </summary>

	<Serializable> _
	Public Class Level
		Private Const defaultBundle As String = "sun.util.logging.resources.logging"

		''' <summary>
		''' @serial  The non-localized name of the level.
		''' </summary>
		Private ReadOnly name As String

		''' <summary>
		''' @serial  The integer value of the level.
		''' </summary>
		Private ReadOnly value As Integer

		''' <summary>
		''' @serial The resource bundle name to be used in localizing the level name.
		''' </summary>
		Private ReadOnly resourceBundleName As String

		' localized level name
		<NonSerialized> _
		Private localizedLevelName As String
		<NonSerialized> _
		Private cachedLocale As java.util.Locale

		''' <summary>
		''' OFF is a special level that can be used to turn off logging.
		''' This level is initialized to <CODE>Integer.MAX_VALUE</CODE>.
		''' </summary>
		Public Shared ReadOnly [OFF] As New Level("OFF",Integer.MAX_VALUE, defaultBundle)

		''' <summary>
		''' SEVERE is a message level indicating a serious failure.
		''' <p>
		''' In general SEVERE messages should describe events that are
		''' of considerable importance and which will prevent normal
		''' program execution.   They should be reasonably intelligible
		''' to end users and to system administrators.
		''' This level is initialized to <CODE>1000</CODE>.
		''' </summary>
		Public Shared ReadOnly SEVERE As New Level("SEVERE",1000, defaultBundle)

		''' <summary>
		''' WARNING is a message level indicating a potential problem.
		''' <p>
		''' In general WARNING messages should describe events that will
		''' be of interest to end users or system managers, or which
		''' indicate potential problems.
		''' This level is initialized to <CODE>900</CODE>.
		''' </summary>
		Public Shared ReadOnly WARNING As New Level("WARNING", 900, defaultBundle)

		''' <summary>
		''' INFO is a message level for informational messages.
		''' <p>
		''' Typically INFO messages will be written to the console
		''' or its equivalent.  So the INFO level should only be
		''' used for reasonably significant messages that will
		''' make sense to end users and system administrators.
		''' This level is initialized to <CODE>800</CODE>.
		''' </summary>
		Public Shared ReadOnly INFO As New Level("INFO", 800, defaultBundle)

		''' <summary>
		''' CONFIG is a message level for static configuration messages.
		''' <p>
		''' CONFIG messages are intended to provide a variety of static
		''' configuration information, to assist in debugging problems
		''' that may be associated with particular configurations.
		''' For example, CONFIG message might include the CPU type,
		''' the graphics depth, the GUI look-and-feel, etc.
		''' This level is initialized to <CODE>700</CODE>.
		''' </summary>
		Public Shared ReadOnly CONFIG As New Level("CONFIG", 700, defaultBundle)

		''' <summary>
		''' FINE is a message level providing tracing information.
		''' <p>
		''' All of FINE, FINER, and FINEST are intended for relatively
		''' detailed tracing.  The exact meaning of the three levels will
		''' vary between subsystems, but in general, FINEST should be used
		''' for the most voluminous detailed output, FINER for somewhat
		''' less detailed output, and FINE for the  lowest volume (and
		''' most important) messages.
		''' <p>
		''' In general the FINE level should be used for information
		''' that will be broadly interesting to developers who do not have
		''' a specialized interest in the specific subsystem.
		''' <p>
		''' FINE messages might include things like minor (recoverable)
		''' failures.  Issues indicating potential performance problems
		''' are also worth logging as FINE.
		''' This level is initialized to <CODE>500</CODE>.
		''' </summary>
		Public Shared ReadOnly FINE As New Level("FINE", 500, defaultBundle)

		''' <summary>
		''' FINER indicates a fairly detailed tracing message.
		''' By default logging calls for entering, returning, or throwing
		''' an exception are traced at this level.
		''' This level is initialized to <CODE>400</CODE>.
		''' </summary>
		Public Shared ReadOnly FINER As New Level("FINER", 400, defaultBundle)

		''' <summary>
		''' FINEST indicates a highly detailed tracing message.
		''' This level is initialized to <CODE>300</CODE>.
		''' </summary>
		Public Shared ReadOnly FINEST As New Level("FINEST", 300, defaultBundle)

		''' <summary>
		''' ALL indicates that all messages should be logged.
		''' This level is initialized to <CODE>Integer.MIN_VALUE</CODE>.
		''' </summary>
		Public Shared ReadOnly ALL As New Level("ALL", Integer.MIN_VALUE, defaultBundle)

		''' <summary>
		''' Create a named Level with a given integer value.
		''' <p>
		''' Note that this constructor is "protected" to allow subclassing.
		''' In general clients of logging should use one of the constant Level
		''' objects such as SEVERE or FINEST.  However, if clients need to
		''' add new logging levels, they may subclass Level and define new
		''' constants. </summary>
		''' <param name="name">  the name of the Level, for example "SEVERE". </param>
		''' <param name="value"> an integer value for the level. </param>
		''' <exception cref="NullPointerException"> if the name is null </exception>
		Protected Friend Sub New(ByVal name As String, ByVal value As Integer)
			Me.New(name, value, Nothing)
		End Sub

		''' <summary>
		''' Create a named Level with a given integer value and a
		''' given localization resource name.
		''' <p> </summary>
		''' <param name="name">  the name of the Level, for example "SEVERE". </param>
		''' <param name="value"> an integer value for the level. </param>
		''' <param name="resourceBundleName"> name of a resource bundle to use in
		'''    localizing the given name. If the resourceBundleName is null
		'''    or an empty string, it is ignored. </param>
		''' <exception cref="NullPointerException"> if the name is null </exception>
		Protected Friend Sub New(ByVal name As String, ByVal value As Integer, ByVal resourceBundleName As String)
			Me.New(name, value, resourceBundleName, True)
		End Sub

		' private constructor to specify whether this instance should be added
		' to the KnownLevel list from which Level.parse method does its look up
		Private Sub New(ByVal name As String, ByVal value As Integer, ByVal resourceBundleName As String, ByVal visible As Boolean)
			If name Is Nothing Then Throw New NullPointerException
			Me.name = name
			Me.value = value
			Me.resourceBundleName = resourceBundleName
			Me.localizedLevelName = If(resourceBundleName Is Nothing, name, Nothing)
			Me.cachedLocale = Nothing
			If visible Then KnownLevel.add(Me)
		End Sub

		''' <summary>
		''' Return the level's localization resource bundle name, or
		''' null if no localization bundle is defined.
		''' </summary>
		''' <returns> localization resource bundle name </returns>
		Public Overridable Property resourceBundleName As String
			Get
				Return resourceBundleName
			End Get
		End Property

		''' <summary>
		''' Return the non-localized string name of the Level.
		''' </summary>
		''' <returns> non-localized name </returns>
		Public Overridable Property name As String
			Get
				Return name
			End Get
		End Property

		''' <summary>
		''' Return the localized string name of the Level, for
		''' the current default locale.
		''' <p>
		''' If no localization information is available, the
		''' non-localized name is returned.
		''' </summary>
		''' <returns> localized name </returns>
		Public Overridable Property localizedName As String
			Get
				Return localizedLevelName
			End Get
		End Property

		' package-private getLevelName() is used by the implementation
		' instead of getName() to avoid calling the subclass's version
		Friend Property levelName As String
			Get
				Return Me.name
			End Get
		End Property

		Private Function computeLocalizedLevelName(ByVal newLocale As java.util.Locale) As String
			Dim rb As java.util.ResourceBundle = java.util.ResourceBundle.getBundle(resourceBundleName, newLocale)
			Dim localizedName_Renamed As String = rb.getString(name)

			Dim isDefaultBundle As Boolean = defaultBundle.Equals(resourceBundleName)
			If Not isDefaultBundle Then Return localizedName_Renamed

			' This is a trick to determine whether the name has been translated
			' or not. If it has not been translated, we need to use Locale.ROOT
			' when calling toUpperCase().
			Dim rbLocale As java.util.Locale = rb.locale
			Dim locale_Renamed As java.util.Locale = If(java.util.Locale.ROOT.Equals(rbLocale) OrElse name.Equals(localizedName_Renamed.ToUpper(java.util.Locale.ROOT)), java.util.Locale.ROOT, rbLocale)

			' ALL CAPS in a resource bundle's message indicates no translation
			' needed per Oracle translation guideline.  To workaround this
			' in Oracle JDK implementation, convert the localized level name
			' to uppercase for compatibility reason.
			Return If(java.util.Locale.ROOT.Equals(locale_Renamed), name, localizedName_Renamed.ToUpper(locale_Renamed))
		End Function

		' Avoid looking up the localizedLevelName twice if we already
		' have it.
		Friend Property cachedLocalizedLevelName As String
			Get
    
				If localizedLevelName IsNot Nothing Then
					If cachedLocale IsNot Nothing Then
						If cachedLocale.Equals(java.util.Locale.default) Then Return localizedLevelName
					End If
				End If
    
				If resourceBundleName Is Nothing Then Return name
    
				' We need to compute the localized name.
				' Either because it's the first time, or because our cached
				' value is for a different locale. Just return null.
				Return Nothing
			End Get
		End Property

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Property localizedLevelName As String
			Get
    
				' See if we have a cached localized name
				Dim cachedLocalizedName As String = cachedLocalizedLevelName
				If cachedLocalizedName IsNot Nothing Then Return cachedLocalizedName
    
				' No cached localized name or cache invalid.
				' Need to compute the localized name.
				Dim newLocale As java.util.Locale = java.util.Locale.default
				Try
					localizedLevelName = computeLocalizedLevelName(newLocale)
				Catch ex As Exception
					localizedLevelName = name
				End Try
				cachedLocale = newLocale
				Return localizedLevelName
			End Get
		End Property

		' Returns a mirrored Level object that matches the given name as
		' specified in the Level.parse method.  Returns null if not found.
		'
		' It returns the same Level object as the one returned by Level.parse
		' method if the given name is a non-localized name or integer.
		'
		' If the name is a localized name, findLevel and parse method may
		' return a different level value if there is a custom Level subclass
		' that overrides Level.getLocalizedName() to return a different string
		' than what's returned by the default implementation.
		'
		Shared Function findLevel(ByVal name As String) As Level
			If name Is Nothing Then Throw New NullPointerException

			Dim level_Renamed As KnownLevel

			' Look for a known Level with the given non-localized name.
			level_Renamed = KnownLevel.findByName(name)
			If level_Renamed IsNot Nothing Then Return level_Renamed.mirroredLevel

			' Now, check if the given name is an integer.  If so,
			' first look for a Level with the given value and then
			' if necessary create one.
			Try
				Dim x As Integer = Convert.ToInt32(name)
				level_Renamed = KnownLevel.findByValue(x)
				If level_Renamed Is Nothing Then
					' add new Level
					Dim levelObject As New Level(name, x)
					level_Renamed = KnownLevel.findByValue(x)
				End If
				Return level_Renamed.mirroredLevel
			Catch ex As NumberFormatException
				' Not an integer.
				' Drop through.
			End Try

			level_Renamed = KnownLevel.findByLocalizedLevelName(name)
			If level_Renamed IsNot Nothing Then Return level_Renamed.mirroredLevel

			Return Nothing
		End Function

		''' <summary>
		''' Returns a string representation of this Level.
		''' </summary>
		''' <returns> the non-localized name of the Level, for example "INFO". </returns>
		Public NotOverridable Overrides Function ToString() As String
			Return name
		End Function

		''' <summary>
		''' Get the integer value for this level.  This integer value
		''' can be used for efficient ordering comparisons between
		''' Level objects. </summary>
		''' <returns> the integer value for this level. </returns>
		Public Function intValue() As Integer
			Return value
		End Function

		Private Const serialVersionUID As Long = -8176160795706313070L

		' Serialization magic to prevent "doppelgangers".
		' This is a performance optimization.
		Private Function readResolve() As Object
			Dim o As KnownLevel = KnownLevel.matches(Me)
			If o IsNot Nothing Then Return o.levelObject

			' Woops.  Whoever sent us this object knows
			' about a new log level.  Add it to our list.
			Dim level_Renamed As New Level(Me.name, Me.value, Me.resourceBundleName)
			Return level_Renamed
		End Function

		''' <summary>
		''' Parse a level name string into a Level.
		''' <p>
		''' The argument string may consist of either a level name
		''' or an integer value.
		''' <p>
		''' For example:
		''' <ul>
		''' <li>     "SEVERE"
		''' <li>     "1000"
		''' </ul>
		''' </summary>
		''' <param name="name">   string to be parsed </param>
		''' <exception cref="NullPointerException"> if the name is null </exception>
		''' <exception cref="IllegalArgumentException"> if the value is not valid.
		''' Valid values are integers between <CODE>Integer.MIN_VALUE</CODE>
		''' and <CODE>Integer.MAX_VALUE</CODE>, and all known level names.
		''' Known names are the levels defined by this class (e.g., <CODE>FINE</CODE>,
		''' <CODE>FINER</CODE>, <CODE>FINEST</CODE>), or created by this class with
		''' appropriate package access, or new levels defined or created
		''' by subclasses.
		''' </exception>
		''' <returns> The parsed value. Passing an integer that corresponds to a known name
		''' (e.g., 700) will return the associated name (e.g., <CODE>CONFIG</CODE>).
		''' Passing an integer that does not (e.g., 1) will return a new level name
		''' initialized to that value. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Shared Function parse(ByVal name As String) As Level
			' Check that name is not null.
			name.length()

			Dim level_Renamed As KnownLevel

			' Look for a known Level with the given non-localized name.
			level_Renamed = KnownLevel.findByName(name)
			If level_Renamed IsNot Nothing Then Return level_Renamed.levelObject

			' Now, check if the given name is an integer.  If so,
			' first look for a Level with the given value and then
			' if necessary create one.
			Try
				Dim x As Integer = Convert.ToInt32(name)
				level_Renamed = KnownLevel.findByValue(x)
				If level_Renamed Is Nothing Then
					' add new Level
					Dim levelObject As New Level(name, x)
					level_Renamed = KnownLevel.findByValue(x)
				End If
				Return level_Renamed.levelObject
			Catch ex As NumberFormatException
				' Not an integer.
				' Drop through.
			End Try

			' Finally, look for a known level with the given localized name,
			' in the current default locale.
			' This is relatively expensive, but not excessively so.
			level_Renamed = KnownLevel.findByLocalizedLevelName(name)
			If level_Renamed IsNot Nothing Then Return level_Renamed.levelObject

			' OK, we've tried everything and failed
			Throw New IllegalArgumentException("Bad level """ & name & """")
		End Function

		''' <summary>
		''' Compare two objects for value equality. </summary>
		''' <returns> true if and only if the two objects have the same level value. </returns>
		Public Overrides Function Equals(ByVal ox As Object) As Boolean
			Try
				Dim lx As Level = CType(ox, Level)
				Return (lx.value = Me.value)
			Catch ex As Exception
				Return False
			End Try
		End Function

		''' <summary>
		''' Generate a hashcode. </summary>
		''' <returns> a hashcode based on the level value </returns>
		Public Overrides Function GetHashCode() As Integer
			Return Me.value
		End Function

		' KnownLevel class maintains the global list of all known levels.
		' The API allows multiple custom Level instances of the same name/value
		' be created. This class provides convenient methods to find a level
		' by a given name, by a given value, or by a given localized name.
		'
		' KnownLevel wraps the following Level objects:
		' 1. levelObject:   standard Level object or custom Level object
		' 2. mirroredLevel: Level object representing the level specified in the
		'                   logging configuration.
		'
		' Level.getName, Level.getLocalizedName, Level.getResourceBundleName methods
		' are non-final but the name and resource bundle name are parameters to
		' the Level constructor.  Use the mirroredLevel object instead of the
		' levelObject to prevent the logging framework to execute foreign code
		' implemented by untrusted Level subclass.
		'
		' Implementation Notes:
		' If Level.getName, Level.getLocalizedName, Level.getResourceBundleName methods
		' were final, the following KnownLevel implementation can be removed.
		' Future API change should take this into consideration.
		Friend NotInheritable Class KnownLevel
			Private Shared nameToLevels As IDictionary(Of String, IList(Of KnownLevel)) = New Dictionary(Of String, IList(Of KnownLevel))
			Private Shared intToLevels As IDictionary(Of Integer?, IList(Of KnownLevel)) = New Dictionary(Of Integer?, IList(Of KnownLevel))
			Friend ReadOnly levelObject As Level ' instance of Level class or Level subclass
			Friend ReadOnly mirroredLevel As Level ' mirror of the custom Level
			Friend Sub New(ByVal l As Level)
				Me.levelObject = l
				If l.GetType() Is GetType(Level) Then
					Me.mirroredLevel = l
				Else
					' this mirrored level object is hidden
					Me.mirroredLevel = New Level(l.name, l.value, l.resourceBundleName, False)
				End If
			End Sub

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Friend Shared Sub add(ByVal l As Level)
				' the mirroredLevel object is always added to the list
				' before the custom Level instance
				Dim o As New KnownLevel(l)
				Dim list As IList(Of KnownLevel) = nameToLevels(l.name)
				If list Is Nothing Then
					list = New List(Of )
					nameToLevels(l.name) = list
				End If
				list.Add(o)

				list = intToLevels(l.value)
				If list Is Nothing Then
					list = New List(Of )
					intToLevels(l.value) = list
				End If
				list.Add(o)
			End Sub

			' Returns a KnownLevel with the given non-localized name.
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Shared Function findByName(ByVal name As String) As KnownLevel
				Dim list As IList(Of KnownLevel) = nameToLevels(name)
				If list IsNot Nothing Then Return list(0)
				Return Nothing
			End Function

			' Returns a KnownLevel with the given value.
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Shared Function findByValue(ByVal value As Integer) As KnownLevel
				Dim list As IList(Of KnownLevel) = intToLevels(value)
				If list IsNot Nothing Then Return list(0)
				Return Nothing
			End Function

			' Returns a KnownLevel with the given localized name matching
			' by calling the Level.getLocalizedLevelName() method (i.e. found
			' from the resourceBundle associated with the Level object).
			' This method does not call Level.getLocalizedName() that may
			' be overridden in a subclass implementation
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Shared Function findByLocalizedLevelName(ByVal name As String) As KnownLevel
				For Each levels As IList(Of KnownLevel) In nameToLevels.Values
					For Each l As KnownLevel In levels
						Dim lname As String = l.levelObject.localizedLevelName
						If name.Equals(lname) Then Return l
					Next l
				Next levels
				Return Nothing
			End Function

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Shared Function matches(ByVal l As Level) As KnownLevel
				Dim list As IList(Of KnownLevel) = nameToLevels(l.name)
				If list IsNot Nothing Then
					For Each level_Renamed As KnownLevel In list
						Dim other As Level = level_Renamed.mirroredLevel
						If l.value = other.value AndAlso (l.resourceBundleName = other.resourceBundleName OrElse (l.resourceBundleName IsNot Nothing AndAlso l.resourceBundleName.Equals(other.resourceBundleName))) Then Return level_Renamed
					Next level_Renamed
				End If
				Return Nothing
			End Function
		End Class

	End Class

End Namespace