'
' * Copyright (c) 2005, 2006, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.plaf.nimbus



	''' <summary>
	''' Effect
	''' 
	''' @author Created by Jasper Potts (Jun 18, 2007)
	''' </summary>
	Friend MustInherit Class Effect
		Friend Enum EffectType
			UNDER
			BLENDED
			OVER
		End Enum

		' =================================================================================================================
		' Abstract Methods

		''' <summary>
		''' Get the type of this effect, one of UNDER,BLENDED,OVER. UNDER means the result of apply effect should be painted
		''' under the src image. BLENDED means the result of apply sffect contains a modified src image so just it should be
		''' painted. OVER means the result of apply effect should be painted over the src image.
		''' </summary>
		''' <returns> The effect type </returns>
		Friend MustOverride ReadOnly Property effectType As EffectType

		''' <summary>
		''' Get the opacity to use to paint the result effected image if the EffectType is UNDER or OVER.
		''' </summary>
		''' <returns> The opactity for the effect, 0.0f -> 1.0f </returns>
		Friend MustOverride ReadOnly Property opacity As Single

		''' <summary>
		''' Apply the effect to the src image generating the result . The result image may or may not contain the source
		''' image depending on what the effect type is.
		''' </summary>
		''' <param name="src"> The source image for applying the effect to </param>
		''' <param name="dst"> The dstination image to paint effect result into. If this is null then a new image will be created </param>
		''' <param name="w">   The width of the src image to apply effect to, this allow the src and dst buffers to be bigger than
		'''            the area the need effect applied to it </param>
		''' <param name="h">   The height of the src image to apply effect to, this allow the src and dst buffers to be bigger than
		'''            the area the need effect applied to it </param>
		''' <returns> The result of appl </returns>
		Friend MustOverride Function applyEffect(ByVal src As java.awt.image.BufferedImage, ByVal dst As java.awt.image.BufferedImage, ByVal w As Integer, ByVal h As Integer) As java.awt.image.BufferedImage

		' =================================================================================================================
		' Static data cache

		Protected Friend Property Shared arrayCache As ArrayCache
			Get
				Dim cache As ArrayCache = CType(sun.awt.AppContext.appContext.get(GetType(ArrayCache)), ArrayCache)
				If cache Is Nothing Then
					cache = New ArrayCache
					sun.awt.AppContext.appContext.put(GetType(ArrayCache),cache)
				End If
				Return cache
			End Get
		End Property

		Protected Friend Class ArrayCache
			Private tmpIntArray As SoftReference(Of Integer()) = Nothing
			Private tmpByteArray1 As SoftReference(Of SByte()) = Nothing
			Private tmpByteArray2 As SoftReference(Of SByte()) = Nothing
			Private tmpByteArray3 As SoftReference(Of SByte()) = Nothing

			Protected Friend Overridable Function getTmpIntArray(ByVal size As Integer) As Integer()
				Dim tmp As Integer()
				tmp = tmpIntArray.get()
				If tmpIntArray Is Nothing OrElse tmp Is Nothing OrElse tmp.Length < size Then
					' create new array
					tmp = New Integer(size - 1){}
					tmpIntArray = New SoftReference(Of Integer())(tmp)
				End If
				Return tmp
			End Function

			Protected Friend Overridable Function getTmpByteArray1(ByVal size As Integer) As SByte()
				Dim tmp As SByte()
				tmp = tmpByteArray1.get()
				If tmpByteArray1 Is Nothing OrElse tmp Is Nothing OrElse tmp.Length < size Then
					' create new array
					tmp = New SByte(size - 1){}
					tmpByteArray1 = New SoftReference(Of SByte())(tmp)
				End If
				Return tmp
			End Function

			Protected Friend Overridable Function getTmpByteArray2(ByVal size As Integer) As SByte()
				Dim tmp As SByte()
				tmp = tmpByteArray2.get()
				If tmpByteArray2 Is Nothing OrElse tmp Is Nothing OrElse tmp.Length < size Then
					' create new array
					tmp = New SByte(size - 1){}
					tmpByteArray2 = New SoftReference(Of SByte())(tmp)
				End If
				Return tmp
			End Function

			Protected Friend Overridable Function getTmpByteArray3(ByVal size As Integer) As SByte()
				Dim tmp As SByte()
				tmp = tmpByteArray3.get()
				If tmpByteArray3 Is Nothing OrElse tmp Is Nothing OrElse tmp.Length < size Then
					' create new array
					tmp = New SByte(size - 1){}
					tmpByteArray3 = New SoftReference(Of SByte())(tmp)
				End If
				Return tmp
			End Function
		End Class
	End Class

End Namespace