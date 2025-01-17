'  ---------------------------------------------------------------------------------
'  ---------------------------------------------------------------------------------
'   Document    :  CMISProvider_IDelete.vb
'   Description :  [type_description_here]
'   Created     :  2/19/2014 9:10:45 AM
'   <copyright company="ECMG">
'       Copyright (c) Enterprise Content Management Group, LLC. All rights reserved.
'       Copying or reuse without permission is strictly forbidden.
'   </copyright>
'  ---------------------------------------------------------------------------------
'  ---------------------------------------------------------------------------------

#Region "Imports"

Imports Documents.Exceptions
Imports Documents.Providers
Imports Documents.Utilities
Imports PortCMIS

#End Region

Partial Public Class CMISProvider
  Implements IDelete

#Region "IDelete Implementation"

  Public Function DeleteDocument(ByVal lpId As String, ByVal lpSessionId As String) As Boolean Implements IDelete.DeleteDocument
    Try

      Return DeleteDocument(lpId)

    Catch ex As Exception
      ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
      ' Re-throw the exception to the caller
      Throw
    End Try
  End Function

  Public Function DeleteDocument(lpId As String, lpSessionId As String, lpDeleteAllVersions As Boolean) As Boolean _
  Implements IDelete.DeleteDocument
    Try
      Return DeleteDocument(lpId, lpDeleteAllVersions)
    Catch ex As Exception
      ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
      ' Re-throw the exception to the caller
      Throw
    End Try
  End Function

  Public Function DeleteVersion(ByVal lpDocumentId As String,
                            ByVal lpCriterion As String,
                             ByVal lpSessionId As String) As Boolean Implements IDelete.DeleteVersion
    Throw New NotImplementedException
  End Function

#End Region

  Private Function DeleteDocument(lpId As String, Optional lpDeleteAllVersions As Boolean = False) As Boolean
    Try
      Dim lblnSuccess As Boolean = False

      Dim lobjCmisDocId As Client.IObjectId = Session.CreateObjectId(lpId)
      Dim lobjCmisDocument As Client.IDocument

      If Session.Exists(lobjCmisDocId) Then
        lobjCmisDocument = Session.GetObject(lobjCmisDocId)
        lobjCmisDocument.Delete(lpDeleteAllVersions)
        If Not Session.Exists(lobjCmisDocId) Then
          lblnSuccess = True
        Else
          lblnSuccess = False
        End If
      Else
        Throw New ItemDoesNotExistException(lpId)
      End If

    Catch ex As Exception
      ApplicationLogging.LogException(ex, Reflection.MethodBase.GetCurrentMethod)
      ' Re-throw the exception to the caller
      Throw
    End Try
  End Function

End Class
