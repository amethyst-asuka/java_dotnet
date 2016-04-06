Imports System

'
' * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.net


	''' <summary>
	''' Parses a string containing a host/domain name and port range
	''' </summary>
	Friend Class HostPortrange

		Friend hostname_Renamed As String
		Friend scheme As String
		Friend portrange_Renamed As Integer()

		Friend wildcard_Renamed As Boolean
		Friend literal_Renamed As Boolean
		Friend ipv6, ipv4 As Boolean
		Friend Const PORT_MIN As Integer = 0
		Friend Shared ReadOnly PORT_MAX As Integer = (1 << 16) -1

		Friend Overrides Function Equals(  that As HostPortrange) As Boolean
			Return Me.hostname_Renamed.Equals(that.hostname_Renamed) AndAlso Me.portrange_Renamed(0) = that.portrange_Renamed(0) AndAlso Me.portrange_Renamed(1) = that.portrange_Renamed(1) AndAlso Me.wildcard_Renamed = that.wildcard_Renamed AndAlso Me.literal_Renamed = that.literal_Renamed
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return hostname_Renamed.GetHashCode() + portrange_Renamed(0) + portrange_Renamed(1)
		End Function

		Friend Sub New(  scheme As String,   str As String)
			' Parse the host name.  A name has up to three components, the
			' hostname, a port number, or two numbers representing a port
			' range.   "www.sun.com:8080-9090" is a valid host name.

			' With IPv6 an address can be 2010:836B:4179::836B:4179
			' An IPv6 address needs to be enclose in []
			' For ex: [2010:836B:4179::836B:4179]:8080-9090
			' Refer to RFC 2732 for more information.

			' first separate string into two fields: hoststr, portstr
			Dim hoststr As String, portstr As String = Nothing
			Me.scheme = scheme

			' check for IPv6 address
			If str.Chars(0) = "["c Then
					literal_Renamed = True
					ipv6 = literal_Renamed
				Dim rb As Integer = str.IndexOf("]"c)
				If rb <> -1 Then
					hoststr = str.Substring(1, rb - 1)
				Else
					Throw New IllegalArgumentException("invalid IPv6 address: " & str)
				End If
				Dim sep As Integer = str.IndexOf(":"c, rb + 1)
				If sep <> -1 AndAlso str.length() > sep Then portstr = str.Substring(sep + 1)
				' need to normalize hoststr now
				Dim ip As SByte() = sun.net.util.IPAddressUtil.textToNumericFormatV6(hoststr)
				If ip Is Nothing Then Throw New IllegalArgumentException("illegal IPv6 address")
				Dim sb As New StringBuilder
				Dim formatter As New java.util.Formatter(sb, java.util.Locale.US)
				formatter.format("%02x%02x:%02x%02x:%02x%02x:%02x" & "%02x:%02x%02x:%02x%02x:%02x%02x:%02x%02x", ip(0), ip(1), ip(2), ip(3), ip(4), ip(5), ip(6), ip(7), ip(8), ip(9), ip(10), ip(11), ip(12), ip(13), ip(14), ip(15))
				hostname_Renamed = sb.ToString()
			Else
				' not IPv6 therefore ':' is the port separator

				Dim sep As Integer = str.IndexOf(":"c)
				If sep <> -1 AndAlso str.length() > sep Then
					hoststr = str.Substring(0, sep)
					portstr = str.Substring(sep + 1)
				Else
					hoststr = If(sep = -1, str, str.Substring(0, sep))
				End If
				' is this a domain wildcard specification?
				If hoststr.LastIndexOf("*"c) > 0 Then
					Throw New IllegalArgumentException("invalid host wildcard specification")
				ElseIf hoststr.StartsWith("*") Then
					wildcard_Renamed = True
					If hoststr.Equals("*") Then
						hoststr = ""
					ElseIf hoststr.StartsWith("*.") Then
						hoststr = ToLower(hoststr.Substring(1))
					Else
						Throw New IllegalArgumentException("invalid host wildcard specification")
					End If
				Else
					' check if ipv4 (if rightmost label a number)
					' The normal way to specify ipv4 is 4 decimal labels
					' but actually three, two or single label formats valid also
					' So, we recognise ipv4 by just testing the rightmost label
					' being a number.
					Dim lastdot As Integer = hoststr.LastIndexOf("."c)
					If lastdot <> -1 AndAlso (hoststr.length() > 1) Then
						Dim ipv4 As Boolean = True

						Dim i As Integer = lastdot + 1
						Dim len As Integer = hoststr.length()
						Do While i < len
							Dim c As Char = hoststr.Chars(i)
							If c < "0"c OrElse c > "9"c Then
								ipv4 = False
								Exit Do
							End If
							i += 1
						Loop
							Me.literal_Renamed = ipv4
							Me.ipv4 = Me.literal_Renamed
						If ipv4 Then
							Dim ip As SByte() = sun.net.util.IPAddressUtil.textToNumericFormatV4(hoststr)
							If ip Is Nothing Then Throw New IllegalArgumentException("illegal IPv4 address")
							Dim sb As New StringBuilder
							Dim formatter As New java.util.Formatter(sb, java.util.Locale.US)
							formatter.format("%d.%d.%d.%d", ip(0), ip(1), ip(2), ip(3))
							hoststr = sb.ToString()
						Else
							' regular domain name
							hoststr = ToLower(hoststr)
						End If
					End If
				End If
				hostname_Renamed = hoststr
			End If

			Try
				portrange_Renamed = parsePort(portstr)
			Catch e As Exception
				Throw New IllegalArgumentException("invalid port range: " & portstr)
			End Try
		End Sub

		Friend Shared ReadOnly CASE_DIFF As Integer = AscW("A"c) - AscW("a"c)

		''' <summary>
		''' Convert to lower case, and check that all chars are ascii
		''' alphanumeric, '-' or '.' only.
		''' </summary>
		Friend Shared Function toLowerCase(  s As String) As String
			Dim len As Integer = s.length()
			Dim sb As StringBuilder = Nothing

			For i As Integer = 0 To len - 1
				Dim c As Char = s.Chars(i)
				If (c >= "a"c AndAlso c <= "z"c) OrElse (c = "."c) Then
					If sb IsNot Nothing Then sb.append(c)
				ElseIf (c >= "0"c AndAlso c <= "9"c) OrElse (c = "-"c) Then
					If sb IsNot Nothing Then sb.append(c)
				ElseIf c >= "A"c AndAlso c <= "Z"c Then
					If sb Is Nothing Then
						sb = New StringBuilder(len)
						sb.append(s, 0, i)
					End If
					sb.append(CChar(AscW(c) - CASE_DIFF))
				Else
					Throw New IllegalArgumentException("Invalid characters in hostname")
				End If
			Next i
			Return If(sb Is Nothing, s, sb.ToString())
		End Function


		Public Overridable Function literal() As Boolean
			Return literal_Renamed
		End Function

		Public Overridable Function ipv4Literal() As Boolean
			Return ipv4
		End Function

		Public Overridable Function ipv6Literal() As Boolean
			Return ipv6
		End Function

		Public Overridable Function hostname() As String
			Return hostname_Renamed
		End Function

		Public Overridable Function portrange() As Integer()
			Return portrange_Renamed
		End Function

		''' <summary>
		''' returns true if the hostname part started with *
		''' hostname returns the remaining part of the host component
		''' eg "*.foo.com" -> ".foo.com" or "*" -> ""
		''' 
		''' @return
		''' </summary>
		Public Overridable Function wildcard() As Boolean
			Return wildcard_Renamed
		End Function

		' these shouldn't leak outside the implementation
		Friend Shared ReadOnly HTTP_PORT As Integer() = {80, 80}
		Friend Shared ReadOnly HTTPS_PORT As Integer() = {443, 443}
		Friend Shared ReadOnly NO_PORT As Integer() = {-1, -1}

		Friend Overridable Function defaultPort() As Integer()
			If scheme.Equals("http") Then
				Return HTTP_PORT
			ElseIf scheme.Equals("https") Then
				Return HTTPS_PORT
			End If
			Return NO_PORT
		End Function

		Friend Overridable Function parsePort(  port As String) As Integer()

			If port Is Nothing OrElse port.Equals("") Then Return defaultPort()

			If port.Equals("*") Then Return New Integer() {PORT_MIN, PORT_MAX}

			Try
				Dim dash As Integer = port.IndexOf("-"c)

				If dash = -1 Then
					Dim p As Integer = Convert.ToInt32(port)
					Return New Integer() {p, p}
				Else
					Dim low As String = port.Substring(0, dash)
					Dim high As String = port.Substring(dash+1)
					Dim l, h As Integer

					If low.Equals("") Then
						l = PORT_MIN
					Else
						l = Convert.ToInt32(low)
					End If

					If high.Equals("") Then
						h = PORT_MAX
					Else
						h = Convert.ToInt32(high)
					End If
					If l < 0 OrElse h < 0 OrElse h<l Then Return defaultPort()
					Return New Integer() {l, h}
				End If
			Catch e As IllegalArgumentException
				Return defaultPort()
			End Try
		End Function
	End Class

End Namespace