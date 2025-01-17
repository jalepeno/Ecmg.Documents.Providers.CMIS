'  ---------------------------------------------------------------------------------
'  ---------------------------------------------------------------------------------
'   Document    :  CMISProvider_IDocumentExporter.vb
'   Description :  [type_description_here]
'   Created     :  2/19/2014 8:58:29 AM
'   <copyright company="ECMG">
'       Copyright (c) Enterprise Content Management Group, LLC. All rights reserved.
'       Copying or reuse without permission is strictly forbidden.
'   </copyright>
'  ---------------------------------------------------------------------------------
'  ---------------------------------------------------------------------------------

#Region "Imports"

Imports Documents.Arguments
Imports Documents.Core
Imports Documents.Providers
Imports Documents.Utilities
'Imports DotCMIS
'Imports DotCMIS.Data
Imports PortCMIS
Imports PortCMIS.Data

#End Region

Partial Public Class CMISProvider
  Implements IDocumentExporter

#Region "IDocumentExporter Implementation"

  Public Function DocumentCount(ByVal lpFolderPath As String, Optional ByVal lpRecursionLevel As RecursionLevel = RecursionLevel.ecmThisLevelOnly) As Long Implements IDocumentExporter.DocumentCount
    Try
      Throw New NotImplementedException
    Catch ex As Exception
      ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
      ' Re-throw the exception to the caller
      Throw
    End Try
  End Function

  Public Event DocumentExported(ByVal sender As Object, ByRef e As DocumentExportedEventArgs) Implements IDocumentExporter.DocumentExported

  Public Event DocumentExportError(ByVal sender As Object, ByVal e As DocumentExportErrorEventArgs) Implements IDocumentExporter.DocumentExportError, IBasicContentServicesProvider.DocumentExportError

  Public Event DocumentExportMessage(ByVal sender As Object, ByVal e As WriteMessageArgs) Implements IDocumentExporter.DocumentExportMessage

  Public Function ExportDocument(ByVal Args As ExportDocumentEventArgs) As Boolean Implements IDocumentExporter.ExportDocument
    Try
      Dim lobjDocument As New Document(Me)

      ' Create some magic here...

      Dim lobjCmisDocId As Client.IObjectId = Session.CreateObjectId(Args.Id)
      Dim lobjCmisDocument As Client.IDocument = Session.GetObject(lobjCmisDocId)
      Dim lobjCmisVersions As IList(Of Client.IDocument) = lobjCmisDocument.GetAllVersions()

      'lobjDocument.ID = Helper.CleanFile(Args.Id, "_")
      lobjDocument.ID = Args.Id

      ' lobjDocument.DocumentClass = "CMIS Document"
      lobjDocument.DocumentClass = lobjCmisDocument.ObjectType.DisplayName

      'Dim lobjFoldersProperty As IMultiValuedProperty = PropertyFactory.Create(PropertyType.ecmString, _
      '                                                                        "FoldersFiledIn", Cardinality.ecmMultiValued)

      For Each lstrPath As String In lobjCmisDocument.Paths
        'lobjFoldersProperty.Values.Add(lstrPath)
        lobjDocument.AddFolderPath(GetFolderFromPath(lstrPath))
      Next

      'lobjDocument.Properties.Add(lobjFoldersProperty)

      lobjDocument.Versions.Add(New Version(lobjDocument))

      ' lobjDocument.FirstVersion.SetPropertyValue("Name", lobjCmisDocument.Name, True, PropertyType.ecmString)
      lobjDocument.FirstVersion.Properties.Add(CmisPropertiesToCtsProperties(lobjCmisDocument.Properties))

      ' Get the content
      Dim lobjCmisContent As IContentStream = lobjCmisDocument.GetContentStream
      If lobjCmisContent IsNot Nothing AndAlso lobjCmisContent.Stream IsNot Nothing Then
        'Dim lobjContent As New Content(Helper.CopyStream(lobjCmisContent.Stream), lobjCmisContent.FileName, _
        '                               Content.StorageTypeEnum.Reference, False, True)
        Dim lobjContent As New Content(lobjCmisContent.Stream, lobjCmisContent.FileName,
                                       Content.StorageTypeEnum.Reference, False, True)
        lobjContent.MIMEType = lobjCmisContent.MimeType
        lobjDocument.FirstVersion.Contents.Add(lobjContent)
      End If

      Args.Document = lobjDocument

      Return ExportDocumentComplete(Me, Args)

    Catch ex As Exception
      ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
      Args.ErrorMessage = ex.Message
      RaiseEvent DocumentExportError(Me, New DocumentExportErrorEventArgs(Args, ex))
      Return False
    End Try
  End Function

  Public Function ExportDocument(ByVal lpID As String) As Boolean Implements IDocumentExporter.ExportDocument
    Try
      Return ExportDocument(New ExportDocumentEventArgs(lpID))

    Catch ex As Exception
      ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
      ' Re-throw the exception to the caller
      Throw
    End Try
  End Function

  ' <Removed by: Ernie at: 9/26/2014-10:49:57 AM on machine: ERNIE-THINK>
  '   Public Overloads Function ExportDocuments(ByVal Args As Arguments.ExportDocumentsEventArgs) As Boolean Implements IDocumentExporter.ExportDocuments
  '     Try
  '       Return MyBase.ExportDocuments(Me, Args, AddressOf ExportDocument)
  ' 
  '     Catch ex As Exception
  '       ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
  '       ' Re-throw the exception to the caller
  '       Throw
  '     End Try
  '   End Function
  ' </Removed by: Ernie at: 9/26/2014-10:49:57 AM on machine: ERNIE-THINK>

  ' <Removed by: Ernie at: 9/29/2014-2:00:05 PM on machine: ERNIE-THINK>
  '   Public Overloads Sub ExportFolder(ByVal Args As Arguments.ExportFolderEventArgs) Implements IDocumentExporter.ExportFolder
  '     Try
  '       MyBase.ExportFolder(Me, Args)
  ' 
  '     Catch ex As Exception
  '       ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
  '       ' Re-throw the exception to the caller
  '       Throw
  '     End Try
  '   End Sub
  ' </Removed by: Ernie at: 9/29/2014-2:00:05 PM on machine: ERNIE-THINK>

  Public Overrides Property ExportPath() As String Implements IDocumentExporter.ExportPath
    Get
      Return MyBase.ExportPath
    End Get
    Set(ByVal value As String)
      MyBase.ExportPath = value
    End Set
  End Property

  Public Event FolderDocumentExported(ByVal sender As Object, ByRef e As FolderDocumentExportedEventArgs) Implements IDocumentExporter.FolderDocumentExported

  Public Event FolderExported(ByVal sender As Object, ByRef e As FolderExportedEventArgs) Implements IDocumentExporter.FolderExported

  Public Sub OnDocumentExported(ByRef e As DocumentExportedEventArgs) Implements IDocumentExporter.OnDocumentExported
    RaiseEvent DocumentExported(Me, e)
  End Sub

  Public Sub OnDocumentExportError(ByRef e As DocumentExportErrorEventArgs) Implements IDocumentExporter.OnDocumentExportError
    RaiseEvent DocumentExportError(Me, e)
  End Sub

  Public Sub OnDocumentExportMessage(ByRef e As WriteMessageArgs) Implements IDocumentExporter.OnDocumentExportMessage
    RaiseEvent DocumentExportMessage(Me, e)
  End Sub

  Public Sub OnFolderDocumentExported(ByRef e As FolderDocumentExportedEventArgs) Implements IDocumentExporter.OnFolderDocumentExported
    RaiseEvent FolderDocumentExported(Me, e)
  End Sub

  Public Sub OnFolderExported(ByRef e As FolderExportedEventArgs) Implements IDocumentExporter.OnFolderExported
    RaiseEvent FolderExported(Me, e)
  End Sub

  ' <Removed by: Ernie at: 9/29/2014-2:08:03 PM on machine: ERNIE-THINK>
  '   Public Function SetDocumentAsReadOnly(ByVal lpID As String) As Boolean Implements IDocumentExporter.SetDocumentAsReadOnly
  ' 
  '   End Function
  ' </Removed by: Ernie at: 9/29/2014-2:08:03 PM on machine: ERNIE-THINK>

#End Region


End Class
