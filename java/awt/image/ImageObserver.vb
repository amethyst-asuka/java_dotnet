'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.image



	''' <summary>
	''' An asynchronous update interface for receiving notifications about
	''' Image information as the Image is constructed.
	''' 
	''' @author      Jim Graham
	''' </summary>
	Public Interface ImageObserver
		''' <summary>
		''' This method is called when information about an image which was
		''' previously requested using an asynchronous interface becomes
		''' available.  Asynchronous interfaces are method calls such as
		''' getWidth(ImageObserver) and drawImage(img, x, y, ImageObserver)
		''' which take an ImageObserver object as an argument.  Those methods
		''' register the caller as interested either in information about
		''' the overall image itself (in the case of getWidth(ImageObserver))
		''' or about an output version of an image (in the case of the
		''' drawImage(img, x, y, [w, h,] ImageObserver) call).
		''' 
		''' <p>This method
		''' should return true if further updates are needed or false if the
		''' required information has been acquired.  The image which was being
		''' tracked is passed in using the img argument.  Various constants
		''' are combined to form the infoflags argument which indicates what
		''' information about the image is now available.  The interpretation
		''' of the x, y, width, and height arguments depends on the contents
		''' of the infoflags argument.
		''' <p>
		''' The <code>infoflags</code> argument should be the bitwise inclusive
		''' <b>OR</b> of the following flags: <code>WIDTH</code>,
		''' <code>HEIGHT</code>, <code>PROPERTIES</code>, <code>SOMEBITS</code>,
		''' <code>FRAMEBITS</code>, <code>ALLBITS</code>, <code>ERROR</code>,
		''' <code>ABORT</code>.
		''' </summary>
		''' <param name="img">   the image being observed. </param>
		''' <param name="infoflags">   the bitwise inclusive OR of the following
		'''               flags:  <code>WIDTH</code>, <code>HEIGHT</code>,
		'''               <code>PROPERTIES</code>, <code>SOMEBITS</code>,
		'''               <code>FRAMEBITS</code>, <code>ALLBITS</code>,
		'''               <code>ERROR</code>, <code>ABORT</code>. </param>
		''' <param name="x">   the <i>x</i> coordinate. </param>
		''' <param name="y">   the <i>y</i> coordinate. </param>
		''' <param name="width">    the width. </param>
		''' <param name="height">   the height. </param>
		''' <returns>    <code>false</code> if the infoflags indicate that the
		'''            image is completely loaded; <code>true</code> otherwise.
		''' </returns>
		''' <seealso cref= #WIDTH </seealso>
		''' <seealso cref= #HEIGHT </seealso>
		''' <seealso cref= #PROPERTIES </seealso>
		''' <seealso cref= #SOMEBITS </seealso>
		''' <seealso cref= #FRAMEBITS </seealso>
		''' <seealso cref= #ALLBITS </seealso>
		''' <seealso cref= #ERROR </seealso>
		''' <seealso cref= #ABORT </seealso>
		''' <seealso cref= Image#getWidth </seealso>
		''' <seealso cref= Image#getHeight </seealso>
		''' <seealso cref= java.awt.Graphics#drawImage </seealso>
		Function imageUpdate(  img As java.awt.Image,   infoflags As Integer,   x As Integer,   y As Integer,   width As Integer,   height As Integer) As Boolean

		''' <summary>
		''' This flag in the infoflags argument to imageUpdate indicates that
		''' the width of the base image is now available and can be taken
		''' from the width argument to the imageUpdate callback method. </summary>
		''' <seealso cref= Image#getWidth </seealso>
		''' <seealso cref= #imageUpdate </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		Public Shared final int WIDTH = 1;

		''' <summary>
		''' This flag in the infoflags argument to imageUpdate indicates that
		''' the height of the base image is now available and can be taken
		''' from the height argument to the imageUpdate callback method. </summary>
		''' <seealso cref= Image#getHeight </seealso>
		''' <seealso cref= #imageUpdate </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		Public Shared final int HEIGHT = 2;

		''' <summary>
		''' This flag in the infoflags argument to imageUpdate indicates that
		''' the properties of the image are now available. </summary>
		''' <seealso cref= Image#getProperty </seealso>
		''' <seealso cref= #imageUpdate </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		Public Shared final int PROPERTIES = 4;

		''' <summary>
		''' This flag in the infoflags argument to imageUpdate indicates that
		''' more pixels needed for drawing a scaled variation of the image
		''' are available.  The bounding box of the new pixels can be taken
		''' from the x, y, width, and height arguments to the imageUpdate
		''' callback method. </summary>
		''' <seealso cref= java.awt.Graphics#drawImage </seealso>
		''' <seealso cref= #imageUpdate </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		Public Shared final int SOMEBITS = 8;

		''' <summary>
		''' This flag in the infoflags argument to imageUpdate indicates that
		''' another complete frame of a multi-frame image which was previously
		''' drawn is now available to be drawn again.  The x, y, width, and height
		''' arguments to the imageUpdate callback method should be ignored. </summary>
		''' <seealso cref= java.awt.Graphics#drawImage </seealso>
		''' <seealso cref= #imageUpdate </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		Public Shared final int FRAMEBITS = 16;

		''' <summary>
		''' This flag in the infoflags argument to imageUpdate indicates that
		''' a static image which was previously drawn is now complete and can
		''' be drawn again in its final form.  The x, y, width, and height
		''' arguments to the imageUpdate callback method should be ignored. </summary>
		''' <seealso cref= java.awt.Graphics#drawImage </seealso>
		''' <seealso cref= #imageUpdate </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		Public Shared final int ALLBITS = 32;

		''' <summary>
		''' This flag in the infoflags argument to imageUpdate indicates that
		''' an image which was being tracked asynchronously has encountered
		''' an error.  No further information will become available and
		''' drawing the image will fail.
		''' As a convenience, the ABORT flag will be indicated at the same
		''' time to indicate that the image production was aborted. </summary>
		''' <seealso cref= #imageUpdate </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		Public Shared final int ERROR = 64;

		''' <summary>
		''' This flag in the infoflags argument to imageUpdate indicates that
		''' an image which was being tracked asynchronously was aborted before
		''' production was complete.  No more information will become available
		''' without further action to trigger another image production sequence.
		''' If the ERROR flag was not also set in this image update, then
		''' accessing any of the data in the image will restart the production
		''' again, probably from the beginning. </summary>
		''' <seealso cref= #imageUpdate </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		Public Shared final int ABORT = 128;
	End Interface

End Namespace