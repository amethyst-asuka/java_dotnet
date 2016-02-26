Imports Microsoft.VisualBasic
Imports System
Imports System.Text

'
' * Copyright (c) 2002, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management


	''' <summary>
	''' <p>Permission controlling access to MBeanServer operations.  If a
	''' security manager has been set using {@link
	''' System#setSecurityManager}, most operations on the MBean Server
	''' require that the caller's permissions imply an MBeanPermission
	''' appropriate for the operation.  This is described in detail in the
	''' documentation for the <seealso cref="MBeanServer"/> interface.</p>
	''' 
	''' <p>As with other <seealso cref="Permission"/> objects, an MBeanPermission can
	''' represent either a permission that you <em>have</em> or a
	''' permission that you <em>need</em>.  When a sensitive operation is
	''' being checked for permission, an MBeanPermission is constructed
	''' representing the permission you need.  The operation is only
	''' allowed if the permissions you have <seealso cref="#implies imply"/> the
	''' permission you need.</p>
	''' 
	''' <p>An MBeanPermission contains four items of information:</p>
	''' 
	''' <ul>
	''' 
	''' <li><p>The <em>action</em>.  For a permission you need,
	''' this is one of the actions in the list <a
	''' href="#action-list">below</a>.  For a permission you have, this is
	''' a comma-separated list of those actions, or <code>*</code>,
	''' representing all actions.</p>
	''' 
	''' <p>The action is returned by <seealso cref="#getActions()"/>.</p>
	''' 
	''' <li><p>The <em>class name</em>.</p>
	''' 
	''' <p>For a permission you need, this is the class name of an MBean
	''' you are accessing, as returned by {@link
	''' MBeanServer#getMBeanInfo(ObjectName)
	''' MBeanServer.getMBeanInfo(name)}.{@link MBeanInfo#getClassName()
	''' getClassName()}.  Certain operations do not reference a class name,
	''' in which case the class name is null.</p>
	''' 
	''' <p>For a permission you have, this is either empty or a <em>class
	''' name pattern</em>.  A class name pattern is a string following the
	''' Java conventions for dot-separated class names.  It may end with
	''' "<code>.*</code>" meaning that the permission grants access to any
	''' class that begins with the string preceding "<code>.*</code>".  For
	''' instance, "<code>javax.management.*</code>" grants access to
	''' <code>javax.management.MBeanServerDelegate</code> and
	''' <code>javax.management.timer.Timer</code>, among other classes.</p>
	''' 
	''' <p>A class name pattern can also be empty or the single character
	''' "<code>*</code>", both of which grant access to any class.</p>
	''' 
	''' <li><p>The <em>member</em>.</p>
	''' 
	''' <p>For a permission you need, this is the name of the attribute or
	''' operation you are accessing.  For operations that do not reference
	''' an attribute or operation, the member is null.</p>
	''' 
	''' <p>For a permission you have, this is either the name of an attribute
	''' or operation you can access, or it is empty or the single character
	''' "<code>*</code>", both of which grant access to any member.</p>
	''' 
	''' <li id="MBeanName"><p>The <em>object name</em>.</p>
	''' 
	''' <p>For a permission you need, this is the <seealso cref="ObjectName"/> of the
	''' MBean you are accessing.  For operations that do not reference a
	''' single MBean, it is null.  It is never an object name pattern.</p>
	''' 
	''' <p>For a permission you have, this is the <seealso cref="ObjectName"/> of the
	''' MBean or MBeans you can access.  It may be an object name pattern
	''' to grant access to all MBeans whose names match the pattern.  It
	''' may also be empty, which grants access to all MBeans whatever their
	''' name.</p>
	''' 
	''' </ul>
	''' 
	''' <p>If you have an MBeanPermission, it allows operations only if all
	''' four of the items match.</p>
	''' 
	''' <p>The class name, member, and object name can be written together
	''' as a single string, which is the <em>name</em> of this permission.
	''' The name of the permission is the string returned by {@link
	''' Permission#getName() getName()}.  The format of the string is:</p>
	''' 
	''' <blockquote>
	''' <code>className#member[objectName]</code>
	''' </blockquote>
	''' 
	''' <p>The object name is written using the usual syntax for {@link
	''' ObjectName}.  It may contain any legal characters, including
	''' <code>]</code>.  It is terminated by a <code>]</code> character
	''' that is the last character in the string.</p>
	''' 
	''' <p>One or more of the <code>className</code>, <code>member</code>,
	''' or <code>objectName</code> may be omitted.  If the
	''' <code>member</code> is omitted, the <code>#</code> may be too (but
	''' does not have to be).  If the <code>objectName</code> is omitted,
	''' the <code>[]</code> may be too (but does not have to be).  It is
	''' not legal to omit all three items, that is to have a <em>name</em>
	''' that is the empty string.</p>
	''' 
	''' <p>One or more of the <code>className</code>, <code>member</code>,
	''' or <code>objectName</code> may be the character "<code>-</code>",
	''' which is equivalent to a null value.  A null value is implied by
	''' any value (including another null value) but does not imply any
	''' other value.</p>
	''' 
	''' <p><a name="action-list">The possible actions are these:</a></p>
	''' 
	''' <ul>
	''' <li>addNotificationListener</li>
	''' <li>getAttribute</li>
	''' <li>getClassLoader</li>
	''' <li>getClassLoaderFor</li>
	''' <li>getClassLoaderRepository</li>
	''' <li>getDomains</li>
	''' <li>getMBeanInfo</li>
	''' <li>getObjectInstance</li>
	''' <li>instantiate</li>
	''' <li>invoke</li>
	''' <li>isInstanceOf</li>
	''' <li>queryMBeans</li>
	''' <li>queryNames</li>
	''' <li>registerMBean</li>
	''' <li>removeNotificationListener</li>
	''' <li>setAttribute</li>
	''' <li>unregisterMBean</li>
	''' </ul>
	''' 
	''' <p>In a comma-separated list of actions, spaces are allowed before
	''' and after each action.</p>
	''' 
	''' @since 1.5
	''' </summary>
	Public Class MBeanPermission
		Inherits java.security.Permission

		Private Const serialVersionUID As Long = -2416928705275160661L

		''' <summary>
		''' Actions list.
		''' </summary>
		Private Const AddNotificationListener As Integer = &H1
		Private Const GetAttribute As Integer = &H2
		Private Const GetClassLoader As Integer = &H4
		Private Const GetClassLoaderFor As Integer = &H8
		Private Const GetClassLoaderRepository As Integer = &H10
		Private Const GetDomains As Integer = &H20
		Private Const GetMBeanInfo As Integer = &H40
		Private Const GetObjectInstance As Integer = &H80
		Private Const Instantiate As Integer = &H100
		Private Const Invoke As Integer = &H200
		Private Const IsInstanceOf As Integer = &H400
		Private Const QueryMBeans As Integer = &H800
		Private Const QueryNames As Integer = &H1000
		Private Const RegisterMBean As Integer = &H2000
		Private Const RemoveNotificationListener As Integer = &H4000
		Private Const SetAttribute As Integer = &H8000
		Private Const UnregisterMBean As Integer = &H10000

		''' <summary>
		''' No actions.
		''' </summary>
		Private Const NONE As Integer = &H0

		''' <summary>
		''' All actions.
		''' </summary>
		Private Shared ReadOnly ALL As Integer = AddNotificationListener Or GetAttribute Or GetClassLoader Or GetClassLoaderFor Or GetClassLoaderRepository Or GetDomains Or GetMBeanInfo Or GetObjectInstance Or Instantiate Or Invoke Or IsInstanceOf Or QueryMBeans Or QueryNames Or RegisterMBean Or RemoveNotificationListener Or SetAttribute Or UnregisterMBean

		''' <summary>
		''' The actions string.
		''' </summary>
		Private actions As String

		''' <summary>
		''' The actions mask.
		''' </summary>
		<NonSerialized> _
		Private mask As Integer

		''' <summary>
		''' The classname prefix that must match.  If null, is implied by any
		''' classNamePrefix but does not imply any non-null classNamePrefix.
		''' </summary>
		<NonSerialized> _
		Private classNamePrefix As String

		''' <summary>
		''' True if classNamePrefix must match exactly.  Otherwise, the
		''' className being matched must start with classNamePrefix.
		''' </summary>
		<NonSerialized> _
		Private classNameExactMatch As Boolean

		''' <summary>
		''' The member that must match.  If null, is implied by any member
		''' but does not imply any non-null member.
		''' </summary>
		<NonSerialized> _
		Private member As String

		''' <summary>
		''' The objectName that must match.  If null, is implied by any
		''' objectName but does not imply any non-null objectName.
		''' </summary>
		<NonSerialized> _
		Private objectName As ObjectName

		''' <summary>
		''' Parse <code>actions</code> parameter.
		''' </summary>
		Private Sub parseActions()

			Dim ___mask As Integer

			If actions Is Nothing Then Throw New System.ArgumentException("MBeanPermission: " & "actions can't be null")
			If actions.Equals("") Then Throw New System.ArgumentException("MBeanPermission: " & "actions can't be empty")

			___mask = getMask(actions)

			If (___mask And ALL) <> ___mask Then Throw New System.ArgumentException("Invalid actions mask")
			If ___mask = NONE Then Throw New System.ArgumentException("Invalid actions mask")
			Me.mask = ___mask
		End Sub

		''' <summary>
		''' Parse <code>name</code> parameter.
		''' </summary>
		Private Sub parseName()
			Dim name As String = name

			If name Is Nothing Then Throw New System.ArgumentException("MBeanPermission name " & "cannot be null")

			If name.Equals("") Then Throw New System.ArgumentException("MBeanPermission name " & "cannot be empty")

	'         The name looks like "class#member[objectname]".  We subtract
	'           elements from the right as we parse, so after parsing the
	'           objectname we have "class#member" and after parsing the
	'           member we have "class".  Each element is optional.  

			' Parse ObjectName

			Dim openingBracket As Integer = name.IndexOf("[")
			If openingBracket = -1 Then
				' If "[on]" missing then ObjectName("*:*")
				'
				objectName = ObjectName.WILDCARD
			Else
				If Not name.EndsWith("]") Then
					Throw New System.ArgumentException("MBeanPermission: " & "The ObjectName in the " & "target name must be " & "included in square " & "brackets")
				Else
					' Create ObjectName
					'
					Try
						' If "[]" then ObjectName("*:*")
						'
						Dim [on] As String = name.Substring(openingBracket + 1, name.Length - 1 - (openingBracket + 1))
						If [on].Equals("") Then
							objectName = ObjectName.WILDCARD
						ElseIf [on].Equals("-") Then
							objectName = Nothing
						Else
							objectName = New ObjectName([on])
						End If
					Catch e As MalformedObjectNameException
						Throw New System.ArgumentException("MBeanPermission: " & "The target name does " & "not specify a valid " & "ObjectName", e)
					End Try
				End If

				name = name.Substring(0, openingBracket)
			End If

			' Parse member

			Dim poundSign As Integer = name.IndexOf("#")

			If poundSign = -1 Then
				member = "*"
			Else
				Dim memberName As String = name.Substring(poundSign + 1)
				member = memberName
				name = name.Substring(0, poundSign)
			End If

			' Parse className

			className = name
		End Sub

		''' <summary>
		''' Assign fields based on className, member, and objectName
		''' parameters.
		''' </summary>
		Private Sub initName(ByVal className As String, ByVal member As String, ByVal ___objectName As ObjectName)
			className = className
			member = member
			Me.objectName = ___objectName
		End Sub

		Private Property className As String
			Set(ByVal className As String)
				If className Is Nothing OrElse className.Equals("-") Then
					classNamePrefix = Nothing
					classNameExactMatch = False
				ElseIf className.Equals("") OrElse className.Equals("*") Then
					classNamePrefix = ""
					classNameExactMatch = False
				ElseIf className.EndsWith(".*") Then
					' Note that we include the "." in the required prefix
					classNamePrefix = className.Substring(0, className.Length - 1)
					classNameExactMatch = False
				Else
					classNamePrefix = className
					classNameExactMatch = True
				End If
			End Set
		End Property

		Private Property member As String
			Set(ByVal member As String)
				If member Is Nothing OrElse member.Equals("-") Then
					Me.member = Nothing
				ElseIf member.Equals("") Then
					Me.member = "*"
				Else
					Me.member = member
				End If
			End Set
		End Property

		''' <summary>
		''' <p>Create a new MBeanPermission object with the specified target name
		''' and actions.</p>
		''' 
		''' <p>The target name is of the form
		''' "<code>className#member[objectName]</code>" where each part is
		''' optional.  It must not be empty or null.</p>
		''' 
		''' <p>The actions parameter contains a comma-separated list of the
		''' desired actions granted on the target name.  It must not be
		''' empty or null.</p>
		''' </summary>
		''' <param name="name"> the triplet "className#member[objectName]". </param>
		''' <param name="actions"> the action string.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the <code>name</code> or
		''' <code>actions</code> is invalid. </exception>
		Public Sub New(ByVal name As String, ByVal actions As String)
			MyBase.New(name)

			parseName()

			Me.actions = actions
			parseActions()
		End Sub

		''' <summary>
		''' <p>Create a new MBeanPermission object with the specified target name
		''' (class name, member, object name) and actions.</p>
		''' 
		''' <p>The class name, member and object name parameters define a
		''' target name of the form
		''' "<code>className#member[objectName]</code>" where each part is
		''' optional.  This will be the result of <seealso cref="#getName()"/> on the
		''' resultant MBeanPermission.</p>
		''' 
		''' <p>The actions parameter contains a comma-separated list of the
		''' desired actions granted on the target name.  It must not be
		''' empty or null.</p>
		''' </summary>
		''' <param name="className"> the class name to which this permission applies.
		''' May be null or <code>"-"</code>, which represents a class name
		''' that is implied by any class name but does not imply any other
		''' class name. </param>
		''' <param name="member"> the member to which this permission applies.  May
		''' be null or <code>"-"</code>, which represents a member that is
		''' implied by any member but does not imply any other member. </param>
		''' <param name="objectName"> the object name to which this permission
		''' applies.  May be null, which represents an object name that is
		''' implied by any object name but does not imply any other object
		''' name. </param>
		''' <param name="actions"> the action string. </param>
		Public Sub New(ByVal className As String, ByVal member As String, ByVal ___objectName As ObjectName, ByVal actions As String)

			MyBase.New(makeName(className, member, ___objectName))
			initName(className, member, ___objectName)

			Me.actions = actions
			parseActions()
		End Sub

		Private Shared Function makeName(ByVal className As String, ByVal member As String, ByVal ___objectName As ObjectName) As String
			Dim name As New StringBuilder
			If className Is Nothing Then className = "-"
			name.Append(className)
			If member Is Nothing Then member = "-"
			name.Append("#" & member)
			If ___objectName Is Nothing Then
				name.Append("[-]")
			Else
				name.Append("[").append(___objectName.canonicalName).append("]")
			End If

	'         In the interests of legibility for Permission.toString(), we
	'           transform the empty string into "*".  
			If name.Length = 0 Then
				Return "*"
			Else
				Return name.ToString()
			End If
		End Function

		''' <summary>
		''' Returns the "canonical string representation" of the actions. That is,
		''' this method always returns present actions in alphabetical order.
		''' </summary>
		''' <returns> the canonical string representation of the actions. </returns>
		Public Overridable Property actions As String
			Get
    
				If actions Is Nothing Then actions = getActions(Me.mask)
    
				Return actions
			End Get
		End Property

		''' <summary>
		''' Returns the "canonical string representation"
		''' of the actions from the mask.
		''' </summary>
		Private Shared Function getActions(ByVal mask As Integer) As String
			Dim sb As New StringBuilder
			Dim comma As Boolean = False

			If (mask And AddNotificationListener) = AddNotificationListener Then
				comma = True
				sb.Append("addNotificationListener")
			End If

			If (mask And GetAttribute) = GetAttribute Then
				If comma Then
					sb.Append(","c)
				Else
					comma = True
				End If
				sb.Append("getAttribute")
			End If

			If (mask And GetClassLoader) = GetClassLoader Then
				If comma Then
					sb.Append(","c)
				Else
					comma = True
				End If
				sb.Append("getClassLoader")
			End If

			If (mask And GetClassLoaderFor) = GetClassLoaderFor Then
				If comma Then
					sb.Append(","c)
				Else
					comma = True
				End If
				sb.Append("getClassLoaderFor")
			End If

			If (mask And GetClassLoaderRepository) = GetClassLoaderRepository Then
				If comma Then
					sb.Append(","c)
				Else
					comma = True
				End If
				sb.Append("getClassLoaderRepository")
			End If

			If (mask And GetDomains) = GetDomains Then
				If comma Then
					sb.Append(","c)
				Else
					comma = True
				End If
				sb.Append("getDomains")
			End If

			If (mask And GetMBeanInfo) = GetMBeanInfo Then
				If comma Then
					sb.Append(","c)
				Else
					comma = True
				End If
				sb.Append("getMBeanInfo")
			End If

			If (mask And GetObjectInstance) = GetObjectInstance Then
				If comma Then
					sb.Append(","c)
				Else
					comma = True
				End If
				sb.Append("getObjectInstance")
			End If

			If (mask And Instantiate) = Instantiate Then
				If comma Then
					sb.Append(","c)
				Else
					comma = True
				End If
				sb.Append("instantiate")
			End If

			If (mask And Invoke) = Invoke Then
				If comma Then
					sb.Append(","c)
				Else
					comma = True
				End If
				sb.Append("invoke")
			End If

			If (mask And IsInstanceOf) = IsInstanceOf Then
				If comma Then
					sb.Append(","c)
				Else
					comma = True
				End If
				sb.Append("isInstanceOf")
			End If

			If (mask And QueryMBeans) = QueryMBeans Then
				If comma Then
					sb.Append(","c)
				Else
					comma = True
				End If
				sb.Append("queryMBeans")
			End If

			If (mask And QueryNames) = QueryNames Then
				If comma Then
					sb.Append(","c)
				Else
					comma = True
				End If
				sb.Append("queryNames")
			End If

			If (mask And RegisterMBean) = RegisterMBean Then
				If comma Then
					sb.Append(","c)
				Else
					comma = True
				End If
				sb.Append("registerMBean")
			End If

			If (mask And RemoveNotificationListener) = RemoveNotificationListener Then
				If comma Then
					sb.Append(","c)
				Else
					comma = True
				End If
				sb.Append("removeNotificationListener")
			End If

			If (mask And SetAttribute) = SetAttribute Then
				If comma Then
					sb.Append(","c)
				Else
					comma = True
				End If
				sb.Append("setAttribute")
			End If

			If (mask And UnregisterMBean) = UnregisterMBean Then
				If comma Then
					sb.Append(","c)
				Else
					comma = True
				End If
				sb.Append("unregisterMBean")
			End If

			Return sb.ToString()
		End Function

		''' <summary>
		''' Returns the hash code value for this object.
		''' </summary>
		''' <returns> a hash code value for this object. </returns>
		Public Overrides Function GetHashCode() As Integer
			Return Me.name.GetHashCode() + Me.actions.GetHashCode()
		End Function

		''' <summary>
		''' Converts an action String to an integer action mask.
		''' </summary>
		''' <param name="action"> the action string. </param>
		''' <returns> the action mask. </returns>
		Private Shared Function getMask(ByVal action As String) As Integer

	'        
	'         * BE CAREFUL HERE! PARSING ORDER IS IMPORTANT IN THIS ALGORITHM.
	'         *
	'         * The 'string length' test must be performed for the lengthiest
	'         * strings first.
	'         *
	'         * In this permission if the "unregisterMBean" string length test is
	'         * performed after the "registerMBean" string length test the algorithm
	'         * considers the 'unregisterMBean' action as being the 'registerMBean'
	'         * action and a parsing error is returned.
	'         

			Dim ___mask As Integer = NONE

			If action Is Nothing Then Return ___mask

			If action.Equals("*") Then Return ALL

			Dim a As Char() = action.ToCharArray()

			Dim i As Integer = a.Length - 1
			If i < 0 Then Return ___mask

			Do While i <> -1
				Dim c As Char

				' skip whitespace
				c = a(i)
				Do While (i<>-1) AndAlso (c = " "c OrElse c = ControlChars.Cr OrElse c = ControlChars.Lf OrElse c = ControlChars.FormFeed OrElse c = ControlChars.Tab)
					i -= 1
					c = a(i)
				Loop

				' check for the known strings
				Dim matchlen As Integer

				If i >= 25 AndAlso (a(i-25) = "r"c) AndAlso (a(i-24) = "e"c) AndAlso (a(i-23) = "m"c) AndAlso (a(i-22) = "o"c) AndAlso (a(i-21) = "v"c) AndAlso (a(i-20) = "e"c) AndAlso (a(i-19) = "N"c) AndAlso (a(i-18) = "o"c) AndAlso (a(i-17) = "t"c) AndAlso (a(i-16) = "i"c) AndAlso (a(i-15) = "f"c) AndAlso (a(i-14) = "i"c) AndAlso (a(i-13) = "c"c) AndAlso (a(i-12) = "a"c) AndAlso (a(i-11) = "t"c) AndAlso (a(i-10) = "i"c) AndAlso (a(i-9) = "o"c) AndAlso (a(i-8) = "n"c) AndAlso (a(i-7) = "L"c) AndAlso (a(i-6) = "i"c) AndAlso (a(i-5) = "s"c) AndAlso (a(i-4) = "t"c) AndAlso (a(i-3) = "e"c) AndAlso (a(i-2) = "n"c) AndAlso (a(i-1) = "e"c) AndAlso (a(i) = "r"c) Then ' removeNotificationListener
					matchlen = 26
					___mask = ___mask Or RemoveNotificationListener ' getClassLoaderRepository
				ElseIf i >= 23 AndAlso (a(i-23) = "g"c) AndAlso (a(i-22) = "e"c) AndAlso (a(i-21) = "t"c) AndAlso (a(i-20) = "C"c) AndAlso (a(i-19) = "l"c) AndAlso (a(i-18) = "a"c) AndAlso (a(i-17) = "s"c) AndAlso (a(i-16) = "s"c) AndAlso (a(i-15) = "L"c) AndAlso (a(i-14) = "o"c) AndAlso (a(i-13) = "a"c) AndAlso (a(i-12) = "d"c) AndAlso (a(i-11) = "e"c) AndAlso (a(i-10) = "r"c) AndAlso (a(i-9) = "R"c) AndAlso (a(i-8) = "e"c) AndAlso (a(i-7) = "p"c) AndAlso (a(i-6) = "o"c) AndAlso (a(i-5) = "s"c) AndAlso (a(i-4) = "i"c) AndAlso (a(i-3) = "t"c) AndAlso (a(i-2) = "o"c) AndAlso (a(i-1) = "r"c) AndAlso (a(i) = "y"c) Then
					matchlen = 24
					___mask = ___mask Or GetClassLoaderRepository ' addNotificationListener
				ElseIf i >= 22 AndAlso (a(i-22) = "a"c) AndAlso (a(i-21) = "d"c) AndAlso (a(i-20) = "d"c) AndAlso (a(i-19) = "N"c) AndAlso (a(i-18) = "o"c) AndAlso (a(i-17) = "t"c) AndAlso (a(i-16) = "i"c) AndAlso (a(i-15) = "f"c) AndAlso (a(i-14) = "i"c) AndAlso (a(i-13) = "c"c) AndAlso (a(i-12) = "a"c) AndAlso (a(i-11) = "t"c) AndAlso (a(i-10) = "i"c) AndAlso (a(i-9) = "o"c) AndAlso (a(i-8) = "n"c) AndAlso (a(i-7) = "L"c) AndAlso (a(i-6) = "i"c) AndAlso (a(i-5) = "s"c) AndAlso (a(i-4) = "t"c) AndAlso (a(i-3) = "e"c) AndAlso (a(i-2) = "n"c) AndAlso (a(i-1) = "e"c) AndAlso (a(i) = "r"c) Then
					matchlen = 23
					___mask = ___mask Or AddNotificationListener ' getClassLoaderFor
				ElseIf i >= 16 AndAlso (a(i-16) = "g"c) AndAlso (a(i-15) = "e"c) AndAlso (a(i-14) = "t"c) AndAlso (a(i-13) = "C"c) AndAlso (a(i-12) = "l"c) AndAlso (a(i-11) = "a"c) AndAlso (a(i-10) = "s"c) AndAlso (a(i-9) = "s"c) AndAlso (a(i-8) = "L"c) AndAlso (a(i-7) = "o"c) AndAlso (a(i-6) = "a"c) AndAlso (a(i-5) = "d"c) AndAlso (a(i-4) = "e"c) AndAlso (a(i-3) = "r"c) AndAlso (a(i-2) = "F"c) AndAlso (a(i-1) = "o"c) AndAlso (a(i) = "r"c) Then
					matchlen = 17
					___mask = ___mask Or GetClassLoaderFor ' getObjectInstance
				ElseIf i >= 16 AndAlso (a(i-16) = "g"c) AndAlso (a(i-15) = "e"c) AndAlso (a(i-14) = "t"c) AndAlso (a(i-13) = "O"c) AndAlso (a(i-12) = "b"c) AndAlso (a(i-11) = "j"c) AndAlso (a(i-10) = "e"c) AndAlso (a(i-9) = "c"c) AndAlso (a(i-8) = "t"c) AndAlso (a(i-7) = "I"c) AndAlso (a(i-6) = "n"c) AndAlso (a(i-5) = "s"c) AndAlso (a(i-4) = "t"c) AndAlso (a(i-3) = "a"c) AndAlso (a(i-2) = "n"c) AndAlso (a(i-1) = "c"c) AndAlso (a(i) = "e"c) Then
					matchlen = 17
					___mask = ___mask Or GetObjectInstance ' unregisterMBean
				ElseIf i >= 14 AndAlso (a(i-14) = "u"c) AndAlso (a(i-13) = "n"c) AndAlso (a(i-12) = "r"c) AndAlso (a(i-11) = "e"c) AndAlso (a(i-10) = "g"c) AndAlso (a(i-9) = "i"c) AndAlso (a(i-8) = "s"c) AndAlso (a(i-7) = "t"c) AndAlso (a(i-6) = "e"c) AndAlso (a(i-5) = "r"c) AndAlso (a(i-4) = "M"c) AndAlso (a(i-3) = "B"c) AndAlso (a(i-2) = "e"c) AndAlso (a(i-1) = "a"c) AndAlso (a(i) = "n"c) Then
					matchlen = 15
					___mask = ___mask Or UnregisterMBean ' getClassLoader
				ElseIf i >= 13 AndAlso (a(i-13) = "g"c) AndAlso (a(i-12) = "e"c) AndAlso (a(i-11) = "t"c) AndAlso (a(i-10) = "C"c) AndAlso (a(i-9) = "l"c) AndAlso (a(i-8) = "a"c) AndAlso (a(i-7) = "s"c) AndAlso (a(i-6) = "s"c) AndAlso (a(i-5) = "L"c) AndAlso (a(i-4) = "o"c) AndAlso (a(i-3) = "a"c) AndAlso (a(i-2) = "d"c) AndAlso (a(i-1) = "e"c) AndAlso (a(i) = "r"c) Then
					matchlen = 14
					___mask = ___mask Or GetClassLoader ' registerMBean
				ElseIf i >= 12 AndAlso (a(i-12) = "r"c) AndAlso (a(i-11) = "e"c) AndAlso (a(i-10) = "g"c) AndAlso (a(i-9) = "i"c) AndAlso (a(i-8) = "s"c) AndAlso (a(i-7) = "t"c) AndAlso (a(i-6) = "e"c) AndAlso (a(i-5) = "r"c) AndAlso (a(i-4) = "M"c) AndAlso (a(i-3) = "B"c) AndAlso (a(i-2) = "e"c) AndAlso (a(i-1) = "a"c) AndAlso (a(i) = "n"c) Then
					matchlen = 13
					___mask = ___mask Or RegisterMBean ' getAttribute
				ElseIf i >= 11 AndAlso (a(i-11) = "g"c) AndAlso (a(i-10) = "e"c) AndAlso (a(i-9) = "t"c) AndAlso (a(i-8) = "A"c) AndAlso (a(i-7) = "t"c) AndAlso (a(i-6) = "t"c) AndAlso (a(i-5) = "r"c) AndAlso (a(i-4) = "i"c) AndAlso (a(i-3) = "b"c) AndAlso (a(i-2) = "u"c) AndAlso (a(i-1) = "t"c) AndAlso (a(i) = "e"c) Then
					matchlen = 12
					___mask = ___mask Or GetAttribute ' getMBeanInfo
				ElseIf i >= 11 AndAlso (a(i-11) = "g"c) AndAlso (a(i-10) = "e"c) AndAlso (a(i-9) = "t"c) AndAlso (a(i-8) = "M"c) AndAlso (a(i-7) = "B"c) AndAlso (a(i-6) = "e"c) AndAlso (a(i-5) = "a"c) AndAlso (a(i-4) = "n"c) AndAlso (a(i-3) = "I"c) AndAlso (a(i-2) = "n"c) AndAlso (a(i-1) = "f"c) AndAlso (a(i) = "o"c) Then
					matchlen = 12
					___mask = ___mask Or GetMBeanInfo ' isInstanceOf
				ElseIf i >= 11 AndAlso (a(i-11) = "i"c) AndAlso (a(i-10) = "s"c) AndAlso (a(i-9) = "I"c) AndAlso (a(i-8) = "n"c) AndAlso (a(i-7) = "s"c) AndAlso (a(i-6) = "t"c) AndAlso (a(i-5) = "a"c) AndAlso (a(i-4) = "n"c) AndAlso (a(i-3) = "c"c) AndAlso (a(i-2) = "e"c) AndAlso (a(i-1) = "O"c) AndAlso (a(i) = "f"c) Then
					matchlen = 12
					___mask = ___mask Or IsInstanceOf ' setAttribute
				ElseIf i >= 11 AndAlso (a(i-11) = "s"c) AndAlso (a(i-10) = "e"c) AndAlso (a(i-9) = "t"c) AndAlso (a(i-8) = "A"c) AndAlso (a(i-7) = "t"c) AndAlso (a(i-6) = "t"c) AndAlso (a(i-5) = "r"c) AndAlso (a(i-4) = "i"c) AndAlso (a(i-3) = "b"c) AndAlso (a(i-2) = "u"c) AndAlso (a(i-1) = "t"c) AndAlso (a(i) = "e"c) Then
					matchlen = 12
					___mask = ___mask Or SetAttribute ' instantiate
				ElseIf i >= 10 AndAlso (a(i-10) = "i"c) AndAlso (a(i-9) = "n"c) AndAlso (a(i-8) = "s"c) AndAlso (a(i-7) = "t"c) AndAlso (a(i-6) = "a"c) AndAlso (a(i-5) = "n"c) AndAlso (a(i-4) = "t"c) AndAlso (a(i-3) = "i"c) AndAlso (a(i-2) = "a"c) AndAlso (a(i-1) = "t"c) AndAlso (a(i) = "e"c) Then
					matchlen = 11
					___mask = ___mask Or Instantiate ' queryMBeans
				ElseIf i >= 10 AndAlso (a(i-10) = "q"c) AndAlso (a(i-9) = "u"c) AndAlso (a(i-8) = "e"c) AndAlso (a(i-7) = "r"c) AndAlso (a(i-6) = "y"c) AndAlso (a(i-5) = "M"c) AndAlso (a(i-4) = "B"c) AndAlso (a(i-3) = "e"c) AndAlso (a(i-2) = "a"c) AndAlso (a(i-1) = "n"c) AndAlso (a(i) = "s"c) Then
					matchlen = 11
					___mask = ___mask Or QueryMBeans ' getDomains
				ElseIf i >= 9 AndAlso (a(i-9) = "g"c) AndAlso (a(i-8) = "e"c) AndAlso (a(i-7) = "t"c) AndAlso (a(i-6) = "D"c) AndAlso (a(i-5) = "o"c) AndAlso (a(i-4) = "m"c) AndAlso (a(i-3) = "a"c) AndAlso (a(i-2) = "i"c) AndAlso (a(i-1) = "n"c) AndAlso (a(i) = "s"c) Then
					matchlen = 10
					___mask = ___mask Or GetDomains ' queryNames
				ElseIf i >= 9 AndAlso (a(i-9) = "q"c) AndAlso (a(i-8) = "u"c) AndAlso (a(i-7) = "e"c) AndAlso (a(i-6) = "r"c) AndAlso (a(i-5) = "y"c) AndAlso (a(i-4) = "N"c) AndAlso (a(i-3) = "a"c) AndAlso (a(i-2) = "m"c) AndAlso (a(i-1) = "e"c) AndAlso (a(i) = "s"c) Then
					matchlen = 10
					___mask = ___mask Or QueryNames ' invoke
				ElseIf i >= 5 AndAlso (a(i-5) = "i"c) AndAlso (a(i-4) = "n"c) AndAlso (a(i-3) = "v"c) AndAlso (a(i-2) = "o"c) AndAlso (a(i-1) = "k"c) AndAlso (a(i) = "e"c) Then
					matchlen = 6
					___mask = ___mask Or Invoke
				Else
					' parse error
					Throw New System.ArgumentException("Invalid permission: " & action)
				End If

				' make sure we didn't just match the tail of a word
				' like "ackbarfaccept".  Also, skip to the comma.
				Dim seencomma As Boolean = False
				Do While i >= matchlen AndAlso Not seencomma
					Select Case a(i-matchlen)
					Case ","c
						seencomma = True
					Case " "c, ControlChars.Cr, ControlChars.Lf, ControlChars.FormFeed, ControlChars.Tab
					Case Else
						Throw New System.ArgumentException("Invalid permission: " & action)
					End Select
					i -= 1
				Loop

				' point i at the location of the comma minus one (or -1).
				i -= matchlen
			Loop

			Return ___mask
		End Function

		''' <summary>
		''' <p>Checks if this MBeanPermission object "implies" the
		''' specified permission.</p>
		''' 
		''' <p>More specifically, this method returns true if:</p>
		''' 
		''' <ul>
		''' 
		''' <li> <i>p</i> is an instance of MBeanPermission; and</li>
		''' 
		''' <li> <i>p</i> has a null className or <i>p</i>'s className
		''' matches this object's className; and</li>
		''' 
		''' <li> <i>p</i> has a null member or <i>p</i>'s member matches this
		''' object's member; and</li>
		''' 
		''' <li> <i>p</i> has a null object name or <i>p</i>'s
		''' object name matches this object's object name; and</li>
		''' 
		''' <li> <i>p</i>'s actions are a subset of this object's actions</li>
		''' 
		''' </ul>
		''' 
		''' <p>If this object's className is "<code>*</code>", <i>p</i>'s
		''' className always matches it.  If it is "<code>a.*</code>", <i>p</i>'s
		''' className matches it if it begins with "<code>a.</code>".</p>
		''' 
		''' <p>If this object's member is "<code>*</code>", <i>p</i>'s
		''' member always matches it.</p>
		''' 
		''' <p>If this object's objectName <i>n1</i> is an object name pattern,
		''' <i>p</i>'s objectName <i>n2</i> matches it if
		''' <seealso cref="ObjectName#equals <i>n1</i>.equals(<i>n2</i>)"/> or if
		''' <seealso cref="ObjectName#apply <i>n1</i>.apply(<i>n2</i>)"/>.</p>
		''' 
		''' <p>A permission that includes the <code>queryMBeans</code> action
		''' is considered to include <code>queryNames</code> as well.</p>
		''' </summary>
		''' <param name="p"> the permission to check against. </param>
		''' <returns> true if the specified permission is implied by this object,
		''' false if not. </returns>
		Public Overridable Function implies(ByVal p As java.security.Permission) As Boolean
			If Not(TypeOf p Is MBeanPermission) Then Return False

			Dim that As MBeanPermission = CType(p, MBeanPermission)

			' Actions
			'
			' The actions in 'this' permission must be a
			' superset of the actions in 'that' permission
			'

			' "queryMBeans" implies "queryNames" 
			If (Me.mask And QueryMBeans) = QueryMBeans Then
				If ((Me.mask Or QueryNames) And that.mask) <> that.mask Then Return False
			Else
				If (Me.mask And that.mask) <> that.mask Then Return False
			End If

			' Target name
			'
			' The 'className' check is true iff:
			' 1) the className in 'this' permission is omitted or "*", or
			' 2) the className in 'that' permission is omitted or "*", or
			' 3) the className in 'this' permission does pattern
			'    matching with the className in 'that' permission.
			'
			' The 'member' check is true iff:
			' 1) the member in 'this' permission is omitted or "*", or
			' 2) the member in 'that' permission is omitted or "*", or
			' 3) the member in 'this' permission equals the member in
			'    'that' permission.
			'
			' The 'object name' check is true iff:
			' 1) the object name in 'this' permission is omitted or "*:*", or
			' 2) the object name in 'that' permission is omitted or "*:*", or
			' 3) the object name in 'this' permission does pattern
			'    matching with the object name in 'that' permission.
			'

	'         Check if this.className implies that.className.
	'
	'           If that.classNamePrefix is empty that means the className is
	'           irrelevant for this permission check.  Otherwise, we do not
	'           expect that "that" contains a wildcard, since it is a
	'           needed permission.  So we assume that.classNameExactMatch.  

			If that.classNamePrefix Is Nothing Then
				' bottom is implied
			ElseIf Me.classNamePrefix Is Nothing Then
				' bottom implies nothing but itself
				Return False
			ElseIf Me.classNameExactMatch Then
				If Not that.classNameExactMatch Then Return False ' exact never implies wildcard
				If Not that.classNamePrefix.Equals(Me.classNamePrefix) Then Return False ' exact match fails
			Else
				' prefix match, works even if "that" is also a wildcard
				' e.g. a.* implies a.* and a.b.*
				If Not that.classNamePrefix.StartsWith(Me.classNamePrefix) Then Return False
			End If

			' Check if this.member implies that.member 

			If that.member Is Nothing Then
				' bottom is implied
			ElseIf Me.member Is Nothing Then
				' bottom implies nothing but itself
				Return False
			ElseIf Me.member.Equals("*") Then
				' wildcard implies everything (including itself)
			ElseIf Not Me.member.Equals(that.member) Then
				Return False
			End If

			' Check if this.objectName implies that.objectName 

			If that.objectName Is Nothing Then
				' bottom is implied
			ElseIf Me.objectName Is Nothing Then
				' bottom implies nothing but itself
				Return False
			ElseIf Not Me.objectName.apply(that.objectName) Then
	'             ObjectName.apply returns false if that.objectName is a
	'               wildcard so we also allow equals for that case.  This
	'               never happens during real permission checks, but means
	'               the implies relation is reflexive.  
				If Not Me.objectName.Equals(that.objectName) Then Return False
			End If

			Return True
		End Function

		''' <summary>
		''' Checks two MBeanPermission objects for equality. Checks
		''' that <i>obj</i> is an MBeanPermission, and has the same
		''' name and actions as this object.
		''' <P> </summary>
		''' <param name="obj"> the object we are testing for equality with this object. </param>
		''' <returns> true if obj is an MBeanPermission, and has the
		''' same name and actions as this MBeanPermission object. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If obj Is Me Then Return True

			If Not(TypeOf obj Is MBeanPermission) Then Return False

			Dim that As MBeanPermission = CType(obj, MBeanPermission)

			Return (Me.mask = that.mask) AndAlso (Me.name.Equals(that.name))
		End Function

		''' <summary>
		''' Deserialize this object based on its name and actions.
		''' </summary>
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
			[in].defaultReadObject()
			parseName()
			parseActions()
		End Sub
	End Class

End Namespace