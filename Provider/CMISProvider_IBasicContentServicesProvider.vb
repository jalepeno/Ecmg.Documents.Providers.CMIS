'  ---------------------------------------------------------------------------------
'  ---------------------------------------------------------------------------------
'   Document    :  CMISProvider_IBasicContentServicesProvider.vb
'   Description :  [type_description_here]
'   Created     :  2/19/2014 9:08:37 AM
'   <copyright company="ECMG">
'       Copyright (c) Enterprise Content Management Group, LLC. All rights reserved.
'       Copying or reuse without permission is strictly forbidden.
'   </copyright>
'  ---------------------------------------------------------------------------------
'  ---------------------------------------------------------------------------------

#Region "Imports"

Imports Documents.Utilities
Imports Documents.Core
Imports Documents.Providers
Imports Documents.Arguments

#End Region

Partial Public Class CMISProvider
  Implements IBasicContentServicesProvider
  Implements IUpdateProperties

#Region "IBasicContentServicesProvider Implementation"

  Public Function AddDocument(ByVal lpDocument As Document) As Boolean Implements IBasicContentServicesProvider.AddDocument
    Try
      Throw New NotImplementedException
    Catch ex As Exception
      ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
      ' Re-throw the exception to the caller
      Throw
    End Try
  End Function

  Public Function AddDocument(ByVal lpDocument As Document, ByVal lpFolderPath As String) As Boolean Implements IBasicContentServicesProvider.AddDocument
    Try
      Throw New NotImplementedException
    Catch ex As Exception
      ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
      ' Re-throw the exception to the caller
      Throw
    End Try
  End Function

  Function IsCheckedOut(ByVal lpID As String) As Boolean Implements IBasicContentServicesProvider.IsCheckedOut, IVersion.IsCheckedOut
    Try
      Throw New NotImplementedException
    Catch ex As Exception
      ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
      ' Re-throw the exception to the caller
      Throw
    End Try
  End Function

  Public Function CancelCheckoutDocument(ByVal lpID As String) As Boolean Implements IBasicContentServicesProvider.CancelCheckoutDocument
    Try
      Throw New NotImplementedException
    Catch ex As Exception
      ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
      ' Re-throw the exception to the caller
      Throw
    End Try
  End Function

  Public Function CheckinDocument(ByVal lpID As String, _
                                ByVal lpContentContainer As IContentContainer, _
                                ByVal lpAsMajorVersion As Boolean) As Boolean _
  Implements IBasicContentServicesProvider.CheckinDocument
    Throw New NotImplementedException
  End Function

  Public Function CheckinDocument(ByVal lpID As String, ByVal lpContentPath As String, ByVal lpAsMajorVersion As Boolean) As Boolean Implements IBasicContentServicesProvider.CheckinDocument
    Try
      Throw New NotImplementedException
    Catch ex As Exception
      ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
      ' Re-throw the exception to the caller
      Throw
    End Try
  End Function

  Public Function CheckinDocument(ByVal lpID As String, ByVal lpContentPaths() As String, ByVal lpAsMajorVersion As Boolean) As Boolean Implements IBasicContentServicesProvider.CheckinDocument
    Try
      Throw New NotImplementedException
    Catch ex As Exception
      ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
      ' Re-throw the exception to the caller
      Throw
    End Try
  End Function

  Public Function CheckoutDocument(ByVal lpID As String, ByVal lpDestinationFolder As String, ByRef lpOutputFileNames() As String) As Boolean Implements IBasicContentServicesProvider.CheckoutDocument
    Try
      Throw New NotImplementedException
    Catch ex As Exception
      ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
      ' Re-throw the exception to the caller
      Throw
    End Try
  End Function

  Public Function CopyOutDocument(ByVal lpID As String, ByVal lpDestinationFolder As String, ByRef lpOutputFileNames() As String) As Boolean Implements IBasicContentServicesProvider.CopyOutDocument
    Try
      Throw New NotImplementedException
    Catch ex As Exception
      ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
      ' Re-throw the exception to the caller
      Throw
    End Try
  End Function

  Public Function DeleteDocument(ByVal lpID As String) As Boolean Implements IBasicContentServicesProvider.DeleteDocument
    Try
      Throw New NotImplementedException
    Catch ex As Exception
      ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
      ' Re-throw the exception to the caller
      Throw
    End Try
  End Function

  Public Function DeleteVersion(ByVal lpDocumentId As String, _
                           ByVal lpCriterion As String) As Boolean _
                         Implements IBasicContentServicesProvider.DeleteVersion
    Try
      Throw New NotImplementedException
    Catch ex As Exception
      ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
      ' Re-throw the exception to the caller
      Throw
    End Try
  End Function

  Public Event DocumentAdded(ByVal sender As Object, ByVal e As DocumentAddedEventArgs) Implements IBasicContentServicesProvider.DocumentAdded

  Public Event DocumentCheckedIn(ByVal sender As Object, ByVal e As DocumentCheckedInEventArgs) Implements IBasicContentServicesProvider.DocumentCheckedIn

  Public Event DocumentCheckedOut(ByVal sender As Object, ByVal e As DocumentCheckedOutEventArgs) Implements IBasicContentServicesProvider.DocumentCheckedOut

  Public Event DocumentCheckOutCancelled(ByVal sender As Object, ByVal e As DocumentCheckoutCancelledEventArgs) Implements IBasicContentServicesProvider.DocumentCheckOutCancelled

  Public Event DocumentCopiedOut(ByVal sender As Object, ByVal e As DocumentCopiedOutEventArgs) Implements IBasicContentServicesProvider.DocumentCopiedOut

  Public Event DocumentDeleted(ByVal sender As Object, ByVal e As DocumentDeletedEventArgs) Implements IBasicContentServicesProvider.DocumentDeleted

  Public Event DocumentEvent(ByVal sender As Object, ByVal e As DocumentEventArgs) Implements IBasicContentServicesProvider.DocumentEvent

  Public Event DocumentFiled(ByVal sender As Object, ByVal e As DocumentFiledEventArgs) Implements IBasicContentServicesProvider.DocumentFiled

  Public Event DocumentUnFiled(ByVal sender As Object, ByVal e As DocumentUnFiledEventArgs) Implements IBasicContentServicesProvider.DocumentUnFiled

  Public Event DocumentUpdated(ByVal sender As Object, ByVal e As DocumentUpdatedEventArgs) Implements IBasicContentServicesProvider.DocumentUpdated

  Public Function FileDocument(ByVal lpID As String, ByVal lpFolderPath As String) As Boolean Implements IBasicContentServicesProvider.FileDocument
    Throw New NotImplementedException
  End Function

  Public Function GetDocumentWithContent(ByVal lpID As String, ByVal lpDestinationFolder As String) As Document Implements IBasicContentServicesProvider.GetDocumentWithContent
    Throw New NotImplementedException
  End Function

  Public Function GetDocumentWithContent(ByVal lpID As String, ByVal lpDestinationFolder As String, ByVal lpStorageType As Content.StorageTypeEnum) As Document Implements IBasicContentServicesProvider.GetDocumentWithContent
    Throw New NotImplementedException
  End Function

  Public Function GetDocumentWithoutContent(ByVal lpID As String) As Document Implements IBasicContentServicesProvider.GetDocumentWithoutContent
    Throw New NotImplementedException
  End Function

  Public Function UnFileDocument(ByVal lpID As String, ByVal lpFolderPath As String) As Boolean Implements IBasicContentServicesProvider.UnFileDocument
    Throw New NotImplementedException
  End Function

  Public Function UpdateDocumentProperties(ByVal Args As DocumentPropertyArgs) As Boolean Implements IBasicContentServicesProvider.UpdateDocumentProperties, IUpdateProperties.UpdateDocumentProperties
    Throw New NotImplementedException
  End Function


  Public Function AddDocument(lpDocument As Document, lpAsMajorVersion As Boolean) As Boolean Implements IBasicContentServicesProvider.AddDocument
    Try
      Throw New NotImplementedException
    Catch Ex As Exception
      ApplicationLogging.LogException(Ex, Reflection.MethodBase.GetCurrentMethod)
      ' Re-throw the exception to the caller
      Throw
    End Try
  End Function

  Public Function AddDocument(lpDocument As Document, lpFolderPath As String, lpAsMajorVersion As Boolean) As Boolean Implements IBasicContentServicesProvider.AddDocument
    Try
      Throw New NotImplementedException
    Catch Ex As Exception
      ApplicationLogging.LogException(Ex, Reflection.MethodBase.GetCurrentMethod)
      ' Re-throw the exception to the caller
      Throw
    End Try
  End Function

  Public Function CheckinDocument(lpId As String, lpContentContainer As IContentContainer, lpAsMajorVersion As Boolean, lpProperties As IProperties) As Boolean Implements IBasicContentServicesProvider.CheckinDocument
    Try
      Throw New NotImplementedException
    Catch Ex As Exception
      ApplicationLogging.LogException(Ex, Reflection.MethodBase.GetCurrentMethod)
      ' Re-throw the exception to the caller
      Throw
    End Try
  End Function

  Public Function CheckinDocument(lpId As String, lpContentPath As String, lpAsMajorVersion As Boolean, lpProperties As IProperties) As Boolean Implements IBasicContentServicesProvider.CheckinDocument
    Try
      Throw New NotImplementedException
    Catch Ex As Exception
      ApplicationLogging.LogException(Ex, Reflection.MethodBase.GetCurrentMethod)
      ' Re-throw the exception to the caller
      Throw
    End Try
  End Function

  Public Function CheckinDocument(lpId As String, lpContentPaths() As String, lpAsMajorVersion As Boolean, lpProperties As IProperties) As Boolean Implements IBasicContentServicesProvider.CheckinDocument
    Try
      Throw New NotImplementedException
    Catch Ex As Exception
      ApplicationLogging.LogException(Ex, Reflection.MethodBase.GetCurrentMethod)
      ' Re-throw the exception to the caller
      Throw
    End Try
  End Function

  'Public Event DocumentExportError(sender As Object, e As DocumentExportErrorEventArgs) Implements IBasicContentServicesProvider.DocumentExportError

  'Public Event DocumentImportError(sender As Object, e As DocumentImportErrorEventArgs) Implements IBasicContentServicesProvider.DocumentImportError

  Public Function GetDocumentContents(lpId As String) As Contents Implements IBasicContentServicesProvider.GetDocumentContents
    Try
      Throw New NotImplementedException
    Catch Ex As Exception
      ApplicationLogging.LogException(Ex, Reflection.MethodBase.GetCurrentMethod)
      ' Re-throw the exception to the caller
      Throw
    End Try
  End Function

  Public Function GetDocumentContents(lpId As String, lpVersionScope As VersionScopeEnum, lpMaxVersionCount As Integer) As Contents Implements IBasicContentServicesProvider.GetDocumentContents
    Try
      Throw New NotImplementedException
    Catch Ex As Exception
      ApplicationLogging.LogException(Ex, Reflection.MethodBase.GetCurrentMethod)
      ' Re-throw the exception to the caller
      Throw
    End Try
  End Function

  Public Function GetDocumentWithoutContent1(lpId As String, lpPropertyFilter As List(Of String)) As Document Implements IBasicContentServicesProvider.GetDocumentWithoutContent
    Try
      Throw New NotImplementedException
    Catch Ex As Exception
      ApplicationLogging.LogException(Ex, Reflection.MethodBase.GetCurrentMethod)
      ' Re-throw the exception to the caller
      Throw
    End Try
  End Function

#End Region

End Class
