Imports System

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

Namespace javax.print



	''' <summary>
	''' This class is a collection of UI convenience methods which provide a
	''' graphical user dialog for browsing print services looked up through the Java
	''' Print Service API.
	''' <p>
	''' The dialogs follow a standard pattern of acting as a continue/cancel option
	''' for a user as well as allowing the user to select the print service to use
	''' and specify choices such as paper size and number of copies.
	''' <p>
	''' <p>
	''' The dialogs are designed to work with pluggable print services though the
	''' public APIs of those print services.
	''' <p>
	''' If a print service provides any vendor extensions these may be made
	''' accessible to the user through a vendor supplied tab panel Component.
	''' Such a vendor extension is encouraged to use Swing! and to support its
	''' accessibility APIs.
	''' The vendor extensions should return the settings as part of the
	''' AttributeSet.
	''' Applications which want to preserve the user settings should use those
	''' settings to specify the print job.
	''' Note that this class is not referenced by any other part of the Java
	''' Print Service and may not be included in profiles which cannot depend
	''' on the presence of the AWT packages.
	''' </summary>

	Public Class ServiceUI


		''' <summary>
		''' Presents a dialog to the user for selecting a print service (printer).
		''' It is displayed at the location specified by the application and
		''' is modal.
		''' If the specification is invalid or would make the dialog not visible it
		''' will be displayed at a location determined by the implementation.
		''' The dialog blocks its calling thread and is application modal.
		''' <p>
		''' The dialog may include a tab panel with custom UI lazily obtained from
		''' the PrintService's ServiceUIFactory when the PrintService is browsed.
		''' The dialog will attempt to locate a MAIN_UIROLE first as a JComponent,
		''' then as a Panel. If there is no ServiceUIFactory or no matching role
		''' the custom tab will be empty or not visible.
		''' <p>
		''' The dialog returns the print service selected by the user if the user
		''' OK's the dialog and null if the user cancels the dialog.
		''' <p>
		''' An application must pass in an array of print services to browse.
		''' The array must be non-null and non-empty.
		''' Typically an application will pass in only PrintServices capable
		''' of printing a particular document flavor.
		''' <p>
		''' An application may pass in a PrintService to be initially displayed.
		''' A non-null parameter must be included in the array of browsable
		''' services.
		''' If this parameter is null a service is chosen by the implementation.
		''' <p>
		''' An application may optionally pass in the flavor to be printed.
		''' If this is non-null choices presented to the user can be better
		''' validated against those supported by the services.
		''' An application must pass in a PrintRequestAttributeSet for returning
		''' user choices.
		''' On calling the PrintRequestAttributeSet may be empty, or may contain
		''' application-specified values.
		''' <p>
		''' These are used to set the initial settings for the initially
		''' displayed print service. Values which are not supported by the print
		''' service are ignored. As the user browses print services, attributes
		''' and values are copied to the new display. If a user browses a
		''' print service which does not support a particular attribute-value, the
		''' default for that service is used as the new value to be copied.
		''' <p>
		''' If the user cancels the dialog, the returned attributes will not reflect
		''' any changes made by the user.
		''' 
		''' A typical basic usage of this method may be :
		''' <pre>{@code
		''' PrintService[] services = PrintServiceLookup.lookupPrintServices(
		'''                            DocFlavor.INPUT_STREAM.JPEG, null);
		''' PrintRequestAttributeSet attributes = new HashPrintRequestAttributeSet();
		''' if (services.length > 0) {
		'''    PrintService service =  ServiceUI.printDialog(null, 50, 50,
		'''                                               services, services[0],
		'''                                               null,
		'''                                               attributes);
		'''    if (service != null) {
		'''     ... print ...
		'''    }
		''' }
		''' }</pre>
		''' <p>
		''' </summary>
		''' <param name="gc"> used to select screen. null means primary or default screen. </param>
		''' <param name="x"> location of dialog including border in screen coordinates </param>
		''' <param name="y"> location of dialog including border in screen coordinates </param>
		''' <param name="services"> to be browsable, must be non-null. </param>
		''' <param name="defaultService"> - initial PrintService to display. </param>
		''' <param name="flavor"> - the flavor to be printed, or null. </param>
		''' <param name="attributes"> on input is the initial application supplied
		''' preferences. This cannot be null but may be empty.
		''' On output the attributes reflect changes made by the user. </param>
		''' <returns> print service selected by the user, or null if the user
		''' cancelled the dialog. </returns>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <exception cref="IllegalArgumentException"> if services is null or empty,
		''' or attributes is null, or the initial PrintService is not in the
		''' list of browsable services. </exception>
		Public Shared Function printDialog(ByVal gc As java.awt.GraphicsConfiguration, ByVal x As Integer, ByVal y As Integer, ByVal services As PrintService(), ByVal defaultService As PrintService, ByVal flavor As DocFlavor, ByVal attributes As javax.print.attribute.PrintRequestAttributeSet) As PrintService
			Dim defaultIndex As Integer = -1

			If java.awt.GraphicsEnvironment.headless Then
				Throw New java.awt.HeadlessException
			ElseIf (services Is Nothing) OrElse (services.Length = 0) Then
				Throw New System.ArgumentException("services must be non-null " & "and non-empty")
			ElseIf attributes Is Nothing Then
				Throw New System.ArgumentException("attributes must be non-null")
			End If

			If defaultService IsNot Nothing Then
				For i As Integer = 0 To services.Length - 1
					If services(i).Equals(defaultService) Then
						defaultIndex = i
						Exit For
					End If
				Next i

				If defaultIndex < 0 Then Throw New System.ArgumentException("services must contain " & "defaultService")
			Else
				defaultIndex = 0
			End If

			' For now we set owner to null. In the future, it may be passed
			' as an argument.
			Dim owner As java.awt.Window = Nothing

			Dim gcBounds As java.awt.Rectangle = If(gc Is Nothing, java.awt.GraphicsEnvironment.localGraphicsEnvironment.defaultScreenDevice.defaultConfiguration.bounds, gc.bounds)

			Dim dialog As sun.print.ServiceDialog
			If TypeOf owner Is java.awt.Frame Then
				dialog = New sun.print.ServiceDialog(gc, x + gcBounds.x, y + gcBounds.y, services, defaultIndex, flavor, attributes, CType(owner, java.awt.Frame))
			Else
				dialog = New sun.print.ServiceDialog(gc, x + gcBounds.x, y + gcBounds.y, services, defaultIndex, flavor, attributes, CType(owner, java.awt.Dialog))
			End If
			Dim dlgBounds As java.awt.Rectangle = dialog.bounds

			' get union of all GC bounds
			Dim ge As java.awt.GraphicsEnvironment = java.awt.GraphicsEnvironment.localGraphicsEnvironment
			Dim gs As java.awt.GraphicsDevice() = ge.screenDevices
			For j As Integer = 0 To gs.Length - 1
				gcBounds = gcBounds.union(gs(j).defaultConfiguration.bounds)
			Next j

			' if portion of dialog is not within the gc boundary
			If Not gcBounds.contains(dlgBounds) Then dialog.locationRelativeTo = owner
			dialog.show()

			If dialog.status = sun.print.ServiceDialog.APPROVE Then
				Dim newas As javax.print.attribute.PrintRequestAttributeSet = dialog.attributes
				Dim dstCategory As Type = GetType(javax.print.attribute.standard.Destination)
				Dim amCategory As Type = GetType(sun.print.SunAlternateMedia)
				Dim fdCategory As Type = GetType(javax.print.attribute.standard.Fidelity)

				If attributes.containsKey(dstCategory) AndAlso (Not newas.containsKey(dstCategory)) Then attributes.remove(dstCategory)

				If attributes.containsKey(amCategory) AndAlso (Not newas.containsKey(amCategory)) Then attributes.remove(amCategory)

				attributes.addAll(newas)

				Dim fd As javax.print.attribute.standard.Fidelity = CType(attributes.get(fdCategory), javax.print.attribute.standard.Fidelity)
				If fd IsNot Nothing Then
					If fd Is javax.print.attribute.standard.Fidelity.FIDELITY_TRUE Then removeUnsupportedAttributes(dialog.printService, flavor, attributes)
				End If
			End If

			Return dialog.printService
		End Function

		''' <summary>
		''' POSSIBLE FUTURE API: This method may be used down the road if we
		''' decide to allow developers to explicitly display a "page setup" dialog.
		''' Currently we use that functionality internally for the AWT print model.
		''' </summary>
	'    
	'    public static void pageDialog(GraphicsConfiguration gc,
	'                                  int x, int y,
	'                                  PrintService service,
	'                                  DocFlavor flavor,
	'                                  PrintRequestAttributeSet attributes)
	'        throws HeadlessException
	'    {
	'        if (GraphicsEnvironment.isHeadless()) {
	'            throw new HeadlessException();
	'        } else if (service == null) {
	'            throw new IllegalArgumentException("service must be non-null");
	'        } else if (attributes == null) {
	'            throw new IllegalArgumentException("attributes must be non-null");
	'        }
	'
	'        ServiceDialog dialog = new ServiceDialog(gc, x, y, service,
	'                                                 flavor, attributes);
	'        dialog.show();
	'
	'        if (dialog.getStatus() == ServiceDialog.APPROVE) {
	'            PrintRequestAttributeSet newas = dialog.getAttributes();
	'            Class amCategory = SunAlternateMedia.class;
	'
	'            if (attributes.containsKey(amCategory) &&
	'                !newas.containsKey(amCategory))
	'            {
	'                attributes.remove(amCategory);
	'            }
	'
	'            attributes.addAll(newas.values());
	'        }
	'
	'        dialog.getOwner().dispose();
	'    }
	'    

		''' <summary>
		''' Removes any attributes from the given AttributeSet that are
		''' unsupported by the given PrintService/DocFlavor combination.
		''' </summary>
		Private Shared Sub removeUnsupportedAttributes(ByVal ps As PrintService, ByVal flavor As DocFlavor, ByVal aset As javax.print.attribute.AttributeSet)
			Dim asUnsupported As javax.print.attribute.AttributeSet = ps.getUnsupportedAttributes(flavor, aset)

			If asUnsupported IsNot Nothing Then
				Dim usAttrs As javax.print.attribute.Attribute() = asUnsupported.ToArray()

				For i As Integer = 0 To usAttrs.Length - 1
					Dim category As Type = usAttrs(i).category

					If ps.isAttributeCategorySupported(category) Then
						Dim attr As javax.print.attribute.Attribute = CType(ps.getDefaultAttributeValue(category), javax.print.attribute.Attribute)

						If attr IsNot Nothing Then
							aset.add(attr)
						Else
							aset.remove(category)
						End If
					Else
						aset.remove(category)
					End If
				Next i
			End If
		End Sub
	End Class

End Namespace