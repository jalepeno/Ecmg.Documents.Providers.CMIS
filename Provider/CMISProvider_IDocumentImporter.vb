'  ---------------------------------------------------------------------------------
'  ---------------------------------------------------------------------------------
'   Document    :  CMISProvider_IDocumentImporter.vb
'   Description :  [type_description_here]
'   Created     :  2/19/2014 9:01:01 AM
'   <copyright company="ECMG">
'       Copyright (c) Enterprise Content Management Group, LLC. All rights reserved.
'       Copying or reuse without permission is strictly forbidden.
'   </copyright>
'  ---------------------------------------------------------------------------------
'  ---------------------------------------------------------------------------------

#Region "Imports"

Imports Documents.Core
Imports Documents.Utilities
'Imports DotCMIS
'Imports DotCMIS.Client.Impl
'Imports DotCMIS.Data.Impl
Imports System.IO
'Imports DotCMIS.Client
'Imports DotCMIS.Data
Imports PortCMIS
Imports PortCMIS.Data
Imports Documents.Providers
Imports Documents.Arguments

#End Region

Partial Public Class CMISProvider
  Implements IDocumentImporter

#Region "IDocumentImporter Implementation"

  Public Event DocumentImported(ByVal sender As Object, ByVal e As DocumentImportedEventArgs) Implements IDocumentImporter.DocumentImported

  Public Event DocumentImportError(ByVal sender As Object, ByVal e As DocumentImportErrorEventArgs) Implements IDocumentImporter.DocumentImportError, IBasicContentServicesProvider.DocumentImportError

  Public Event DocumentImportMessage(ByVal sender As Object, ByVal e As WriteMessageArgs) Implements IDocumentImporter.DocumentImportMessage

  Public ReadOnly Property EnforceClassificationCompliance() As Boolean Implements IDocumentImporter.EnforceClassificationCompliance
    Get
      Return mblnEnforceClassificationCompliance
    End Get
  End Property

  Public Function ImportDocument(ByRef Args As ImportDocumentArgs) As Boolean Implements IDocumentImporter.ImportDocument

    Dim lblnResult As Boolean = False

    Try

      Dim lobjDocumentProperties As Dictionary(Of String, Object) = CtsPropertiesToCmisPropertyDictionary(Args.Document.Properties)
      Dim lobjVersionProperties As Dictionary(Of String, Object) = CtsPropertiesToCmisPropertyDictionary(Args.Document.LatestVersion.Properties)
      Dim lobjCombinedProperties As New Dictionary(Of String, Object)

      ' Add the document properties
      For Each lstrKey As String In lobjDocumentProperties.Keys
        If Not lobjCombinedProperties.ContainsKey(lstrKey) Then
          lobjCombinedProperties.Add(lstrKey, lobjDocumentProperties(lstrKey))
        End If
      Next

      ' Add the version properties
      For Each lstrKey As String In lobjVersionProperties.Keys
        If Not lobjCombinedProperties.ContainsKey(lstrKey) Then
          lobjCombinedProperties.Add(lstrKey, lobjVersionProperties(lstrKey))
        End If
      Next

      ' lobjCombinedProperties(PropertyIds.ObjectTypeId) = Args.Document.DocumentClass

      Dim lobjTargetDocumentClass As DocumentClass = DocumentClasses(Args.Document.DocumentClass)

      If lobjTargetDocumentClass IsNot Nothing Then
        lobjCombinedProperties(PropertyIds.ObjectTypeId) = lobjTargetDocumentClass.ID
      End If

      Dim lobjVersionContent As Content = Args.Document.LatestVersion.PrimaryContent
      Dim lobjSourceStream As Stream = lobjVersionContent.ToMemoryStream()

      ' Get the contents
      Dim lobjContentStream As New ContentStream
      With lobjContentStream
        .FileName = lobjVersionContent.FileName
        .MimeType = lobjVersionContent.MIMEType
        .Length = lobjSourceStream.Length
        .Stream = lobjSourceStream
      End With

      lobjCombinedProperties(PropertyIds.Name) = IO.Path.GetFileNameWithoutExtension(lobjVersionContent.FileName)

      ' Changed to test for ADT (September 9, 2015 2:35PM)
      ' Dim lobjRootFolder As Client.IFolder = CType(Me.RootFolder, CMISFolder).SourceFolder

      'Dim lobjCmisDocument As Object = Session.CreateDocument(lobjCombinedProperties,
      '                                                                  lobjRootFolder,
      '                                                                  lobjContentStream,
      '                                                                  Nothing)

      ''Dim lobjCmisDocument As Object = Session.CreateDocument(lobjCombinedProperties,
      ''                                                                  Nothing,
      ''                                                                  lobjContentStream,
      ''                                                                  Nothing)

      ' End ADT Change (September 9, 2015 2:35PM)

      Dim lobjImportFolder As Client.IFolder = GetTemporaryImportFolder()

      Dim lblnCanUnFile As Boolean = CanUnfile

      If lobjImportFolder Is Nothing Then
        Throw New InvalidOperationException("Unable to locate a temporary import folder")
      End If

      Dim lobjCmisDocument As Client.IDocument = lobjImportFolder.CreateDocument(lobjCombinedProperties, lobjContentStream, Nothing)

      If lobjCmisDocument IsNot Nothing Then
        Args.Document.ID = lobjCmisDocument.Id
        Args.Document.ObjectID = lobjCmisDocument.Id
        If lblnCanUnFile Then
          lobjCmisDocument.RemoveFromFolder(lobjImportFolder)
        End If
        lblnResult = True
      End If

    Catch ex As Exception
      ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
      ' Re-throw the exception to the caller
      Throw
    End Try

    Return lblnResult

  End Function

  Private Function GetTemporaryImportFolder() As Client.IFolder
    Try

      ' Recurse from the root folder looking for one we have permission to write to
      ' This is because we can't actually create an unfiled document.  We need to 
      ' create the document in a folder and then un-file it from that folder.
      ' http://mail-archives.apache.org/mod_mbox/chemistry-dev/201402.mbox/%3COFA8BB531E.9C21E691-ON88257C89.0065C3F6-88257C89.006622C4@us.ibm.com%3E
      '
      'From Jay Brown <jay.brown@us.ibm.com>
      'Subject Re:  "Bad Request" exception when attempting to create unfiled document in Apache Chemistry 0.10.0
      'Date  Mon, 24 Feb 2014 18: 35:35 GMT

      '  Unfortunately to remain fully compliant with CMIS 1.0 there is no way to create an unfiled 
      '  document when using the AtomPub binding.

      '  You would have to just designate a folder somewhere to use as your unfiled target.  
      '  There is no substantial additional performance overhead to doing this in FileNet.

      '  If it is really then important for some reason that this document not be linked
      '  to any folder you can either unfile with the client Or have an event action
      '  on the folder that will just automatically unfile anything that gets filed to that folder.
      '  I know this Is not optimal, but we are bound to the spec on this.


      'Jay Brown
      'Senior Engineer, ECM Development
      'IBM Software Group
      'jay.brown@us.ibm.com

      Return GetWritableFolder(Session.GetRootFolder())

    Catch ex As Exception
      ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
      ' Re-throw the exception to the caller
      Throw
    End Try
  End Function

  Private Function GetWritableFolder(lpParentFolder As Client.IFolder) As Client.IFolder
    Try

      If CanWrite(lpParentFolder) Then
        Return lpParentFolder
      End If

      If lpParentFolder.AllowableActions.Actions.Contains(Enums.Action.CanGetChildren) Then
        'Dim lobjChildren As IItemEnumerable(Of ICmisObject) = lpParentFolder.GetChildren()
        'If lobjChildren IsNot Nothing AndAlso lobjChildren.Count > 0 Then
        '  For Each lobjFolder As Client.IFolder In lobjChildren
        '    Return GetWritableFolder(lobjFolder)
        '  Next
        'End If
        For Each lobjFolder As Client.IFolder In lpParentFolder.GetChildren()
          Return GetWritableFolder(lobjFolder)
        Next
      End If

      ' We made it this far, there are no writable folders
      Return Nothing

    Catch ex As Exception
      ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
      ' Re-throw the exception to the caller
      Throw
    End Try
  End Function

  Private Function CanWrite(lpFolder As Client.IFolder) As Boolean
    Try

      If lpFolder Is Nothing Then
        Throw New ArgumentNullException("lpFolder")
      End If

      If lpFolder.AllowableActions.Actions.Contains(Enums.Action.CanCreateDocument) Then
        Return True
      Else
        Return False
      End If

    Catch ex As Exception
      ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
      ' Re-throw the exception to the caller
      Throw
    End Try
  End Function

  ' <Removed by: Ernie at: 9/29/2014-11:23:21 AM on machine: ERNIE-THINK>
  '   Public Overrides Property ImportPath() As String Implements IDocumentImporter.ImportPath
  '     Get
  '       Return MyBase.ImportPath
  '     End Get
  '     Set(ByVal value As String)
  '       MyBase.ImportPath = value
  '     End Set
  '   End Property
  ' </Removed by: Ernie at: 9/29/2014-11:23:21 AM on machine: ERNIE-THINK>

  Public Sub OnDocumentImported(ByRef e As DocumentImportedEventArgs) Implements IDocumentImporter.OnDocumentImported
    RaiseEvent DocumentImported(Me, e)
  End Sub

  Public Sub OnDocumentImportError(ByRef e As DocumentImportErrorEventArgs) Implements IDocumentImporter.OnDocumentImportError
    RaiseEvent DocumentImportError(Me, e)
  End Sub

#End Region

End Class
